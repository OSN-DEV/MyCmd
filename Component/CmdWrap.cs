using MyCmd.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MyCmd.Component {
    /// <summary>
    /// console call back event
    /// </summary>
    /// <param name="line">line</param>
    public delegate void ConsoleCallback(string line);

    internal class CmdWrap {

        #region Declaration
        internal const int CTRL_C_EVENT = 0;
        [DllImport("kernel32.dll")]
        internal static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool AttachConsole(uint dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern bool FreeConsole();
        [DllImport("kernel32.dll")]
        static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate HandlerRoutine, bool Add);
        delegate Boolean ConsoleCtrlDelegate(uint CtrlType);

        
        private Process _process;
        private Process _processUtf8;
        private Control _control;
        private string _currentDir = "";

        public delegate void CmdEventHandler(string line);
        public event CmdEventHandler CmdOutputDataReceived = null;
        public event CmdEventHandler CmdErrorDataReceived = null;
        #endregion

        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="window">window</param>
        /// <param name="currentDir">current dir</param>
        internal CmdWrap(Control control, string currentDir = "") {
            this._control = control;
            this._currentDir = currentDir;
            if (0 == this._currentDir.Length) {
                this._currentDir = Environment.CurrentDirectory;
            }
        }
        #endregion

        #region Public Method
        /// <summary>
        /// start
        /// </summary>
        public void Start() {
            this.StartProcess();
        }

        /// <summary>
        /// send command
        /// </summary>
        /// <param name="command">command</param>
        public void SendCommand(string command) {
            if (!this.IsProcessValid()) {
                return;
            }
            this._process.StandardInput.WriteLine(command);
        }

        /// <summary>
        /// break command
        /// </summary>
        public void Break() {
            // if process is not valid, return;
            if (!this.IsProcessValid()) {
                return;
            }

            if (AttachConsole((uint)this._process.Id)) {
                SetConsoleCtrlHandler(null, true);
                try {
                    if (!GenerateConsoleCtrlEvent(CTRL_C_EVENT, 0)) {
                        return;
                    }
                } finally {
                    FreeConsole();
                    SetConsoleCtrlHandler(null, false);
                }
                this.StartProcess();
            }
        }

        /// <summary>
        /// dispose
        /// </summary>
        public void Dispose() {
            if (this.IsProcessValid()) {
                this._process.Kill();
            }
            this._process = null;
        }
        #endregion

        #region Event
        /// <summary>
        /// std output data received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OutputDataReceived(object sender, DataReceivedEventArgs e) {
            this._control.Dispatcher.Invoke(() => {
                this.DoEvents();

                //var a = Encoding.GetEncoding("Shift_JIS").GetString(Encoding.GetEncoding("Shift_JIS").GetBytes(e.Data));
                //string b = Encoding.UTF8.GetString(Encoding.GetEncoding("Shift_JIS").GetBytes(a));

                var a = Encoding.GetEncoding("Shift_JIS").GetString(Encoding.UTF8.GetBytes(e.Data));
               string b = Encoding.UTF8.GetString(Encoding.GetEncoding("Shift_JIS").GetBytes(e.Data));
                System.Diagnostics.Debug.Print(e.Data);
                System.Diagnostics.Debug.Print(a);
               System.Diagnostics.Debug.Print(b);
                System.Diagnostics.Debug.Print("");
                CmdOutputDataReceived?.Invoke(e.Data);
            });
        }

        /// <summary>
        /// std error data received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ErrorDataReceived(object sender, DataReceivedEventArgs e) {
            this._control.Dispatcher.Invoke(() => {
                this.DoEvents();
                CmdErrorDataReceived?.Invoke(e.Data);
            });
        }

        /// <summary>
        /// exit 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exited(object sender, EventArgs e) {
            //this.Dispose();
            //if (this._needRestore) {
            //    this.StartProcess();
            //}
        }
        #endregion

        #region Private Method
        /// <summary>
        /// Create Process
        /// </summary>
        private void StartProcess() {
            var startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = Environment.GetEnvironmentVariable("ComSpec");
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;
            //startInfo.StandardOutputEncoding = Encoding.UTF8;
            //startInfo.StandardErrorEncoding = Encoding.UTF8;
            if (0 < this._currentDir.Length) {
                startInfo.WorkingDirectory = this._currentDir;
            }

            this._process = new Process();
            this._process.StartInfo = startInfo;
            this._process.OutputDataReceived += OutputDataReceived;
            this._process.ErrorDataReceived += ErrorDataReceived;
            this._process.Exited += Exited;
            this._process.EnableRaisingEvents = true;
            this._process.Start();
            this._process.BeginOutputReadLine();
            this._process.BeginErrorReadLine();

    
        }


        /// <summary>
        /// do events
        /// </summary>
        private void DoEvents() {
            DispatcherFrame frame = new DispatcherFrame();
            var callback = new DispatcherOperationCallback(ExitFrames);
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, callback, frame);
            Dispatcher.PushFrame(frame);
        }
        private object ExitFrames(object obj) {
            ((DispatcherFrame)obj).Continue = false;
            return null;
        }

        /// <summary>
        /// check if process is valid
        /// </summary>
        /// <returns>true: valid, false: otherwise</returns>
        private bool IsProcessValid() {
            if (null == this._process) {
                return false;
            }
            bool result =  !this._process.HasExited;
            if (!result) {
                AppCommon.DebugLog("process is invalid");
            }
            return result;
        }
        #endregion
    }
}
