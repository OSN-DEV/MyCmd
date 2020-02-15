using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyCmd.Command {
    class CdCommand : CommandBase {

        #region Declration
        private static Regex regex = new Regex(@"cd (?<args>.+?)(?=\s)|cd (?<args>.+)");
        #endregion

        #region Public Property
        /// <summary>
        /// command key
        /// </summary>
        public override string Key {
            get { return "cd"; }
        }

        /// <summary>
        /// args
        /// </summary>
        public string Args { private set; get; }
        #endregion

        #region Public Method
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public override bool IsMatch(string command) {
            this.Args = "";
            if (2 < command.Split(' ').Length) {
                return false;
            }
            var match = regex.Match(command);
            if (match.Success) {
                this.Args = match.Groups["args"].ToString();
                return true;
            } else {
                return command.StartsWith(this.Key);
            }
        }

        #endregion

        #region Protected
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        protected override void RunCommand(string command) {

        }
        #endregion
    }
}
