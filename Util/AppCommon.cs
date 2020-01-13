using System.Runtime.CompilerServices;

namespace MyCmd.Util {
    internal class AppCommon {
        #region Public Method
        ///// <summary>
        ///// アプリの実行パスを取得する。
        ///// </summary>
        ///// <returns>アプリの実行パス</returns>
        //internal static string GetAppPath() {
        //    var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        //    if (path.EndsWith(@"\")) {
        //        return path;
        //    } else {
        //        return path + @"\";
        //    }
        //}

        /// <summary>
        /// デバッグログを出力する。
        /// </summary>
        /// <param name="log">ログメッセージ</param>
        /// <param name="file">呼び出し元のファイル名</param>
        /// <param name="line">呼び出し元の行番号</param>
        /// <param name="member">呼び出し元のメンバー名(メソッド・プロパティ・イベント名等)</param>
        internal static void DebugLog(string log,
                [CallerFilePath] string file = "",
                [CallerLineNumber] int line = 0,
                [CallerMemberName] string member = "") {
            System.Diagnostics.Debug.WriteLine($"[{file}][{line}][{member}]{log}");
        }
        #endregion
    }
}
