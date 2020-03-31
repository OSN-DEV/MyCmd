using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using MyCmd.AppUtil;

namespace MyCmd.Command {
    internal class GitCommand : CommandBase {
        #region Declaration
        private Repository _repo = null;
        #endregion

        #region PublicField
        public static readonly string Key = "git";
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
            try {
                if (null==_repo || this.CurrentPath != this._repo.Info.Path) {
                    this._repo = new Repository(this.CurrentPath);
                }
            } catch(RepositoryNotFoundException ) {
                base.RaiseErrorReceivedOnce(ErrorMessage.NotFound);
            }
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
            if (!command.StartsWith(Key + " ")) {
                return false;
            }

            return true;
        }
        #endregion

    }
}
