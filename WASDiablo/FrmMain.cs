using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WASDiablo
{
    public partial class FrmMain : Form
    {
        Process _diablo;
        Thread _worker;

        public FrmMain()
        {
            InitializeComponent();
            Task.Run(() => Check());
        }

        private void Check()
        {
            this.Invoke(new MethodInvoker(() =>
                lblStatus.Text = "Waiting for Diablo III.exe"));

            while (true)
            {
                Process[] procs = Process.GetProcessesByName("Diablo III");

                if (procs.Length > 0)
                {
                    _diablo = procs[0];
                    _worker = new Thread(Run);
                    _worker.IsBackground = true;
                    _worker.Start();
                    return;
                }
                Thread.Sleep(200);
            }
        }

        private void Run()
        {
            this.Invoke(new MethodInvoker(() =>
                lblStatus.Text = "Attached to Diablo III.exe ("
                    + _diablo.Id + ")"));

            int lastState = 0;

            while (!_diablo.HasExited)
            {
                Thread.Sleep((int)numDelay.Value);

                if (Native.GetForegroundWindow() != _diablo.MainWindowHandle)
                    continue;

                Size size = Native.GetWindowSize(_diablo);
                Point center = new Point(size.Width / 2, size.Height / 2 - size.Height / 25);
                Point target = new Point(center.X, center.Y);

                int yOffset = (int)(size.Height * numScale.Value);
                int xOffset = (int)(size.Width * numScale.Value);

                if (Native.GetKeyState((int)Keys.W) < 0) target.Y -= yOffset;
                if (Native.GetKeyState((int)Keys.A) < 0) target.X -= xOffset;
                if (Native.GetKeyState((int)Keys.S) < 0) target.Y += yOffset;
                if (Native.GetKeyState((int)Keys.D) < 0) target.X += xOffset;
                
                if (center.X == target.X && center.Y == target.Y)
                {
                    if (lastState != 1)
                    {
                        lastState = 1;
                        Native.BackgroundMouseUp(_diablo, center.X, center.Y);
                    }
                }
                else
                {
                    lastState = 0;
                    Native.BackgroundMouseDown(_diablo, target.X, target.Y);
                }
            }

            Task.Run(() => Check());
        }
    }
}
