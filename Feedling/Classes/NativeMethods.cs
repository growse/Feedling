/*
Copyright © 2008-2011, Andrew Rowson
All rights reserved.

See LICENSE file for license details.
*/
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Feedling
{
    class NativeMethods
    {
        private NativeMethods() { }

        [DllImportAttribute("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            uint uFlags);

        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_NOACTIVATE = 0x0010;
        const UInt32 WS_EX_TOOLWINDOW = 0x00000080;
        const Int32 GWL_EXSTYLE = (-20);
        const Int32 WM_NCLBUTTONDOWN = 0xA1;
        static readonly IntPtr HT_CAPTION = new IntPtr(0x2);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        public static void SendWpfWindowBack(Window window)
        {
            var hWnd = new WindowInteropHelper(window).Handle;
            SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
        }

        public static void SetWindowLongToolWindow(Window window)
        {
            var wh = new WindowInteropHelper(window);
            var exStyle = GetWindowLong(wh.Handle, GWL_EXSTYLE);
            exStyle |= WS_EX_TOOLWINDOW;
            SetWindowLong(wh.Handle, GWL_EXSTYLE, (int)exStyle);
        }
        public static void MakeWindowMovable(Window window)
        {
            var wh = new WindowInteropHelper(window);
            ReleaseCapture();
            SendMessage(wh.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, IntPtr.Zero);
        }
    }
}
