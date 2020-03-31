using MyCmd.AppUtil;
using OsnCsLib.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyCmd.Command {
    internal class LsCommand : CommandBase {

        #region Declration
        private static Regex regex = new Regex(@"ls$|ls\s");
        #endregion

        #region PublicField
        public static readonly string Key = "ls";
        #endregion

        #region Public Property
        /// <summary>
        /// command key
        /// </summary>
        public override string CommandKey { get => Key; }

        public List<string> Options { set; get; } = new List<string>();

        public string Filter { set; get; } = "";
        #endregion


        #region Public Method
        protected override void RunCommand(string command) {
            var chidren = new PathUtil(this.CurrentPath).GetChildren();

            var data = new StringBuilder();
            if (0 == this.Options.Count) {
                foreach (var child in chidren) {
                    if (0 < this.Filter.Length) {
                        if ((child.Name.StartsWith(this.Filter))) {
                            data.Append(child.Name).Append("  ");
                        }
                    } else {
                        data.Append(child.Name).Append("  ");
                    }
                }
            }
            base.RaiseDataReceivedOnce(data.ToString().Trim());
        }
        #endregion

        #region Protected Method
        /// <summary>
        /// parse command
        /// </summary>
        /// <param name="command">command</param>
        /// <returns>true:valid command, false: otherwise</returns>
        protected override bool Parse(string command) {
            this.Options.Clear();
            var match = regex.Match(command);
            if (!match.Success) {
                return false;
            }

            var optionsBase = command.Split(' ');
            foreach(var option in optionsBase) {
                if ("" != option && Key != option) {
                    if (option.StartsWith("-")) {
                        this.Options.Add(option);
                    } else {
                        if (0 < this.Filter.Length) {
                            base.CommandStatus = CommandState.InvalidParams;
                            return true;
                        }
                        this.Filter = option;
                    }
                }
            }
            return true;
        }
        #endregion
    }
}
