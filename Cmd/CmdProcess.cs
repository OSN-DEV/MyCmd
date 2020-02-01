using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCmd.Cmd {
    /// <summary>
    /// Cmdプロセスのラッパー
    /// </summary>
    class CmdProcess {

        #region Declaration
        public delegate void DataReceivedHandler(bool isError, string data);
        public DataReceivedHandler DataReceive = null;

        public delegate void ProcessExitedHadler();
        public ProcessExitedHadler ProcessExited = null;

        private Process _process = null;
        #endregion

        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="currentDir">current dir</param>
        /// <param name="isUtf8">true:utf-8, false:default</param>
        public CmdProcess(string currentDir, bool isUtf8 = false) {
            this.Init(currentDir, isUtf8);
        }
        #endregion

        #region Event
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exited(object sender, EventArgs e) {
            this.ProcessExited?.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ErrorDataReceived(object sender, DataReceivedEventArgs e) {
            this.DataReceive?.Invoke(true, e.Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutputDataReceived(object sender, DataReceivedEventArgs e) {
            this.DataReceive?.Invoke(false, e.Data);
        }
        #endregion

        #region Public Method
        /// <summary>
        /// set data receive listener
        /// </summary>
        /// <param name="listener">listener</param>
        public void SetOuputDataReceivedListener(DataReceivedHandler listener) {
                this.DataReceive = listener;
        }
        #endregion

        #region Private Method
        /// <summary>
        /// initialize class
        /// </summary>
        /// <param name="currentDir">current dir</param>
        /// <param name="isUtf8">true:utf-8, false:default</param>
        private void Init(string currentDir, bool isUtf8 = false) {
            var startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = Environment.GetEnvironmentVariable("ComSpec");
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;
            if (0 < currentDir.Length) {
                startInfo.WorkingDirectory = currentDir;
            }
            if (isUtf8) {
                startInfo.StandardOutputEncoding = Encoding.UTF8;
                startInfo.StandardErrorEncoding = Encoding.UTF8;
            }
            this._process = new Process();
            this._process.StartInfo = startInfo;
            this._process.EnableRaisingEvents = true;
            this._process.Start();
            this._process.BeginOutputReadLine();
            this._process.BeginErrorReadLine();

            this._process.OutputDataReceived += OutputDataReceived;
            this._process.ErrorDataReceived += ErrorDataReceived;
            this._process.Exited += Exited;
        }
        #endregion
    }
}
