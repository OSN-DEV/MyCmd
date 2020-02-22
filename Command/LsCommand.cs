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
        #endregion


        #region Public Method
        protected override void RunCommand(string command) {
            base.RaiseDataReceived("aaa bbb ccc ddd eee fff ggg hhh iii jjj kkk lll mmm nnn ooo ppp qqq rrr");
        }
        #endregion

        #region Protected Method
        /// <summary>
        /// parse command
        /// </summary>
        /// <param name="command">command</param>
        /// <returns>true:valid command, false: otherwise</returns>
        protected override bool Parse(string command) {
            return true;
        }
        #endregion
    }
}
