using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

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
        public abstract string Key { get; }
        #endregion

        #region Public Event
        public delegate void CommandEventHandler(string key, string command,  string data, object userData);
        public event CommandEventHandler DataReceived;
        public event CommandEventHandler ErrorReceived;
        #endregion

        #region Public Method
        /// <summary>
        /// check if command is match
        /// </summary>
        /// <param name="command">comand that checks</param>
        /// <returns>true:match, false:otherwise</returns>
        public abstract bool IsMatch(string command);

        /// <summary>
        /// Run command
        /// </summary>
        /// <param name="command">command</param>
        /// <param name="userData">user data</param>
        public  void RunCommand(string command, object userData = null) {
            this._command = command;
            this._userData = userData;
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
            this.DataReceived?.Invoke(this.Key, this._command, data, this._userData);
        }

        /// <summary>
        /// raise RaiseErrorReceived event
        /// </summary>
        /// <param name="data">data</param>
        protected void RaiseErrorReceived(string data) {
            this.ErrorReceived?.Invoke(this.Key, this._command, data, this._userData);
        }
        #endregion
    }
}
