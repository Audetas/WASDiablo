using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WASDiablo
{
    public static class Native
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam); //MAYBE USE SEND

        [DllImport("USER32.dll")]
        public static extern short GetKeyState(int nVirtKey);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();


        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        private const uint WM_KEYDOWN = 0x100;
        private const uint WM_KEYUP = 0x101;

        public static Size GetWindowSize(Process p)
        {
            Rect rect = new Rect();
            GetWindowRect(p.MainWindowHandle, ref rect);

            Size s = new Size();
            s.Width = rect.right - rect.left;
            s.Height = rect.bottom - rect.top;

            return s;
        }

        public static void BackgroundMouseDown(Process p, int x, int y)
        {
            PostMessage(p.MainWindowHandle, 513, (uint)1, GetLParam(x, y));
        }

        public static void BackgroundMouseUp(Process p, int x, int y)
        {
            PostMessage(p.MainWindowHandle, 514, (uint)1, GetLParam(x, y));
        }

        public static void SendDown(Process p, Keys k)
        {
            PostMessage(p.MainWindowHandle, WM_KEYDOWN, ((IntPtr)k), (IntPtr)0);
        }

        public static void SendUp(Process p, Keys k)
        {
            PostMessage(p.MainWindowHandle, WM_KEYUP, ((IntPtr)k), (IntPtr)0);
        }

        private static uint GetLParam(int x, int y)
        {
            return (uint)(y << 16 | x & (int)ushort.MaxValue);
        }
    }
}
