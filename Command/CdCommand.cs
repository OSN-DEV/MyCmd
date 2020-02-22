using OsnCsLib.Common;
using OsnCsLib.File;
using System;
using System.Text.RegularExpressions;

namespace MyCmd.Command {
    class CdCommand : CommandBase {

        #region Declration
        private static Regex regex = new Regex(@"cd (?<args>.+?)(?=\s)|cd (?<args>.+)");
        #endregion

        #region PublicField
        public static readonly string Key = "cd";
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

        #region Public Method
        #endregion

        #region Protected
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        protected override void RunCommand(string command) {
            if (0 == this.Args?.Length) {
                base.RaiseDataReceivedOnce(this.CurrentPath);
                return;
            }

            var path = new PathUtil(this.CurrentPath, this.Args);


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
                return true;
            } else {
                return command.StartsWith(Key);
            }
        }
        #endregion
    }
}
