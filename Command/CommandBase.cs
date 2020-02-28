using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using OsnCsLib.Common;

namespace MyCmd.Command {
    internal abstract class CommandBase {

        #region Declaration
        private string _command;
        private object _userData;
        #endregion

        #region Public Property 
        /// <summary>
        /// command key
        /// </summary>
        public abstract string  CommandKey { get; }

        /// <summary>
        /// current absolute path
        /// </summary>
        public string CurrentPath { set; get; }
        #endregion

        #region Public Event
        public delegate void CommandEventHandler(string key, string command,  string data, object userData);
        public event CommandEventHandler DataReceived;
        public event CommandEventHandler ErrorReceived;
        public event CommandEventHandler CommandEnd;
        #endregion

        #region Public Method
        /// <summary>
        /// check if command is match
        /// </summary>
        /// <param name="command">comand that checks</param>
        /// <returns>true:match, false:otherwise</returns>
        public bool IsMatch(string command) => this.Parse(command);

        /// <summary>
        /// Run command
        /// </summary>
        /// <param name="command">command</param>
        /// <param name="userData">user data</param>
        public  void RunCommand(string command, object userData = null) {
            this._command = command;
            this._userData = userData;
            this.RunCommand(command);
        }
        #endregion

        #region Protected Method
        /// <summary>
        /// run command
        /// </summary>
        /// <param name="command">command</param>
        protected abstract void RunCommand(string command);

        /// <summary>
        /// raise DataReceived event
        /// </summary>
        /// <param name="data">data</param>
        protected void RaiseDataReceived(string data) {
            this.DataReceived?.Invoke(this.CommandKey, this._command, data, this._userData);
        }

        /// <summary>
        /// raise DataReceived event.
        /// </summary>
        /// <param name="data">data</param>
        protected void RaiseDataReceivedOnce(string data) {
            this.DataReceived?.Invoke(this.CommandKey, this._command, data, this._userData);
            this.CommandEnd?.Invoke(this.CommandKey, this._command, "", this._userData);
        }

        /// <summary>
        /// raise RaiseErrorReceived event
        /// </summary>
        /// <param name="data">data</param>
        protected void RaiseErrorReceived(string data) {
            this.ErrorReceived?.Invoke(this.CommandKey, this._command, data, this._userData);
        }

        /// <summary>
        /// raise RaiseErrorReceived event
        /// </summary>
        /// <param name="data">data</param>
        protected void RaiseErrorReceivedOnce(string data) {
            this.ErrorReceived?.Invoke(this.CommandKey, this._command, data, this._userData);
            this.CommandEnd?.Invoke(this.CommandKey, this._command, "", this._userData);
        }

        /// <summary>
        /// raise CommandEnd event
        /// </summary>
        protected void RaiseCommandEnd() {
            this.CommandEnd?.Invoke(this.CommandKey, this._command, "", this._userData);
        }

        /// <summary>
        /// parse command
        /// </summary>
        /// <param name="command">command</param>
        /// <returns>true:valid command, false: otherwise</returns>
        protected abstract bool Parse(string command);
        #endregion
    }
}
