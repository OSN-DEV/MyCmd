using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MyCmd.Command {
    internal abstract class CommandBase {

        #region Declaration
        #endregion

        #region Public Method
        /// <summary>
        /// check if command is match
        /// </summary>
        /// <param name="command">comand that checks</param>
        /// <returns>true:match, false:otherwise</returns>
        public abstract bool IsMatch(string command);
        #endregion
    }
}
