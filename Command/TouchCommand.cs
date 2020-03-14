using MyCmd.AppUtil;
using OsnCsLib.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static OsnCsLib.File.PathUtil;

namespace MyCmd.Command {
    class TouchCommand : CommandBase {

        #region Declration
        private static Regex regex = new Regex(@"^touch\s+(?<args>.+)");
        private PathUtil _file = new PathUtil();
        #endregion

        #region PublicField
        public static readonly string Key = "touch";
        #endregion

        #region Public Property
        /// <summary>
        /// command key
        /// </summary>
        public override string CommandKey { get => Key; }

        /// <summary>
        /// args
        /// </summary>
        public string Args { private set; get; }
        #endregion


        #region Protected
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        protected override void RunCommand(string command) {
            var file = new FileOperator(this._file.Path);
            if (file.Create()) {
                base.RaiseDataReceivedOnce(file.FilePath + " is created.");
            } else {
                base.RaiseErrorReceivedOnce($"fail to create {file.FilePath}");
            }
        }
        #endregion

        #region Private Method
        /// <summary>
        /// parse command
        /// </summary>
        /// <param name="command">command</param>
        /// <returns>true:valid command, false: otherwise</returns>
        protected override bool Parse(string command) {
            this.Args = "";
            var match = regex.Match(command);
            if (match.Success) {
                this.Args = match.Groups["args"].ToString().Trim();
                if (0==this.Args.Length) {
                    base.CommandStatus = CommandState.InvalidParams;
                } else {
                    this._file.SetPath(base.CurrentPath, this.Args);
                    if (FileTypes.File == this._file.FileType) {
                        base.CommandStatus = CommandState.FileIsExist;
                    } else if (FileTypes.ValidDirectry != this._file.FileType) {
                        base.CommandStatus = CommandState.InalidFile;
                    }
                }
                return true;
            } else {
                return command.StartsWith(Key);
            }
        }
        #endregion
    }
}
