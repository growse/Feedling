/*
Copyright © 2008-2012, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Feedling.Classes
{
    class NativeMethods
    {
        private NativeMethods() { }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, IntPtr windowTitle);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int x,
            int y,
            int cx,
            int cy,
            uint uFlags);

        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_NOACTIVATE = 0x0010;
        const Int32 WS_EX_TOOLWINDOW = 0x00000080;
        const Int32 GWL_EXSTYLE = (-20);
        const Int32 WM_NCLBUTTONDOWN = 0xA1;
        const int GWL_HWNDPARENT = -8;
        static readonly IntPtr HT_CAPTION = new IntPtr(0x2);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        // This static method is required because legacy OSes do not support
        // SetWindowLongPtr 
        public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            return IntPtr.Size == 8 ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong) : new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }

        // This static method is required because Win32 does not support
        // GetWindowLongPtr directly
        public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
        {
            return IntPtr.Size == 8 ? GetWindowLongPtr64(hWnd, nIndex) : GetWindowLongPtr32(hWnd, nIndex);
        }

        public static void SendWpfWindowBack(Window window)
        {
            var hWnd = new WindowInteropHelper(window).Handle;
            SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
        }

        public static void HideFromAltTab(Window window)
        {
            var wh = new WindowInteropHelper(window);
            var exStyle = GetWindowLongPtr(wh.Handle, GWL_EXSTYLE).ToInt32();
            exStyle |= WS_EX_TOOLWINDOW;
            var result = SetWindowLongPtr(wh.Handle, GWL_EXSTYLE, new IntPtr(exStyle));
            if (result.Equals(IntPtr.Zero))
            {
                throw new Win32Exception(string.Concat("Error raised attempting to SetWindowLong: ", result));
            }
        }
        public static void MakeWindowMovable(Window window)
        {
            var wh = new WindowInteropHelper(window);
            ReleaseCapture();
            SendMessage(wh.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, IntPtr.Zero);
        }

        public static void SetParentWindowToDesktop(Window window)
        {
            var wh = new WindowInteropHelper(window);
            var hprog = FindWindowEx(
               FindWindowEx(
               FindWindow("Progman", "Program Manager"), IntPtr.Zero, "SHELLDLL_DefView", ""),
               IntPtr.Zero, "SysListView32", "FolderView");

            var result = SetWindowLongPtr(wh.Handle, GWL_HWNDPARENT, hprog);
            if (result.Equals(IntPtr.Zero))
            {
                throw new Win32Exception(string.Concat("Error raised attempting to SetWindowLong: ", result));
            }

        }
    }
}
