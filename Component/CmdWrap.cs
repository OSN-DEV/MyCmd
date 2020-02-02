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


        private CmdProcess _currentCmd = null;
        private CmdProcess _normalCmd;
        private CmdProcess _utf8Cmd;

        private Control _control;
        private string _currentDir = "";

        public delegate void CmdEventHandler(bool isError, string line);
        public event CmdEventHandler CmdEvent = null;


        private enum CommandStatus {
            Normal,
            CheckPath
        }
        private CommandStatus _status = CommandStatus.Normal;
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

        #region Event
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isError"></param>
        /// <param name="data"></param>
        public void DataReceived(bool isError, string data) {
            if (CommandStatus.CheckPath == this._status) {
                if (data.EndsWith(">cd")) {
                    return;
                }
                this._currentDir = data;
                this._status = CommandStatus.Normal;
                return;
            }
            this._control.Dispatcher.Invoke(() => {
                this.DoEvents();
                CmdEvent?.Invoke(isError, data);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        public void CmdExit() {
            // NOOP
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
            if (this.IsNormalCommand(command)) {
                this._currentCmd = this._normalCmd;
                this._utf8Cmd.SetReceivedListener(null);
                this._utf8Cmd.SetExitedListener(null);
            } else {
                this._normalCmd.SetReceivedListener(null);
                this._normalCmd.SetExitedListener(null);
                this._currentCmd = this._utf8Cmd;

            }
            this._currentCmd.SetReceivedListener(this.DataReceived);
            this._utf8Cmd.SetExitedListener(this.CmdExit);
            if (this._currentCmd.IsProcessValid()) {
                if (command.StartsWith("cd")) {
                    this._normalCmd.WriteLine(command);
                    this._utf8Cmd.WriteLine(command);
                } else {
                    this._currentCmd.WriteLine(command);
                }
            } 
        }

        /// <summary>
        /// break command
        /// </summary>
        public void Break() {
            // if process is not valid, return;
            if (!this._currentCmd.IsProcessValid()) {
                return;
            }

            if (AttachConsole((uint)this._currentCmd.ProcessId)) {
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
            this._normalCmd.Kill();
            this._utf8Cmd.Kill();
            this._normalCmd = null;
            this._utf8Cmd = null;
        }
        #endregion

        #region Private Method
        /// <summary>
        /// Create Process
        /// </summary>
        private void StartProcess() {
            this._normalCmd = new CmdProcess(this._currentDir, false);
            this._utf8Cmd = new CmdProcess(this._currentDir, true);
            this._currentCmd = this._normalCmd;

            // Sleepをかけないとcdコマンドの実行結果以外の情報が表示される
            // 50msが妥当化は微妙
            this._control.Dispatcher.Invoke(() => {
                Thread.Sleep(50);
                this.DoEvents();
                this._status = CommandStatus.CheckPath;
                this.SendCommand("cd");
            });
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


        private List<string> NormalCommand = new List<string> { "cd", "dir"};
        /// <summary>
        /// check commadn
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private bool IsNormalCommand(string command) {
            var result = true;
            //var f = command.Split(' ');
            //if (NormalCommand.Contains(f[0])) {
            //    result = true;
            //}
            if (command.StartsWith("git")) {
                result = false;
            }
            return result;
        }
        #endregion
    }
}
