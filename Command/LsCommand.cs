using OsnCsLib.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCmd.Command {
    class LsCommand : CommandBase {

        #region PublicField
        public static readonly string Key = "ls";
        #endregion

        #region Public Property
        /// <summary>
        /// command key
        /// </summary>
        public override string CommandKey { get => Key; }

        public List<string> Options { set; get; } = new List<string>();
        #endregion


        #region Public Method
        protected override void RunCommand(string command) {
            var chidren = new PathUtil(this.CurrentPath).GetChildren();

            var data = new StringBuilder();
            if (0 == this.Options.Count) {
                foreach (var child in chidren) {
                    data.Append(child.Name).Append("  ");
                }
            }
            base.RaiseDataReceived(data.ToString().Trim());
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
            var optionsBase = command.Split(' ');
            foreach(var option in optionsBase) {
                if ("" != option && Key != option) {
                    this.Options.Add(option);
                }
            }
            return (Key == command || command.StartsWith(Key + " "));
        }
        #endregion
    }
}
