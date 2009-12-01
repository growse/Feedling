using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace Feedling
{
    class NativeMethods
    {
        [DllImportAttribute("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern System.UInt32 GetWindowLong(IntPtr hWnd, int nIndex);
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
        const UInt32 WS_EX_TOOLWINDOW = 0x00000080;
        const Int32 GWL_EXSTYLE = (-20);
        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HT_CAPTION = 0x2;
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        public static void SendWpfWindowBack(Window window)
        {
            var hWnd = new WindowInteropHelper(window).Handle;
            SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE);
        }

        public static void SetWindowLongToolWindow(Window window)
        {
            WindowInteropHelper wh = new WindowInteropHelper(window);
            uint exStyle = GetWindowLong(wh.Handle, GWL_EXSTYLE);
            exStyle |= WS_EX_TOOLWINDOW;
            SetWindowLong(wh.Handle, GWL_EXSTYLE, (int)exStyle);

        }
        public static void MakeWindowMovable(Window window)
        {
            WindowInteropHelper wh = new WindowInteropHelper(window);
            NativeMethods.ReleaseCapture();
            NativeMethods.SendMessage(wh.Handle, NativeMethods.WM_NCLBUTTONDOWN, NativeMethods.HT_CAPTION, 0);
        }
    }
}
