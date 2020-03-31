using MyLib.Util;
using System.Windows.Media;
namespace MyCmd.AppUtil {
    /// <summary>
    /// constant definition
    /// </summary>
    internal static class Constant {
        /// <summary>
        /// app setting file
        /// </summary>
        internal static readonly string SettingFile = Common.GetAppPath() + @"app.settings";

        /// <summary>
        /// maxium count of tabs
        /// </summary>
        internal static readonly int MaxTabCount = 5;

        /// <summary>
        /// console buffer
        /// </summary>
        internal static readonly int BufferSize = 100;

        /// <summary>
        /// maxium count of commands
        /// </summary>
        internal static readonly int MaxCommandBuff = 10;
    }

    /// <summary>
    /// color definition
    /// </summary>
    internal static class ColorDef {
        /// <summary>
        /// console text color
        /// </summary>
//        internal static readonly Brush CosoleForecground = GetBrushFromHex("#FFFFFF");
        internal static readonly Brush ConsoleForeground = GetBrushFromHex("#626264");

        /// <summary>
        /// console error text color
        /// </summary>
        internal static readonly Brush ConsoleErrorForeground = GetBrushFromHex("#FF9999");

        /// <summary>
        /// console background color
        /// </summary>
        //        internal static readonly Brush ConsoleBackground = GetBrushFromHex("#006064");
        internal static readonly Brush ConsoleBackground = GetBrushFromHex("#EBEEF5");

        /// <summary>
        /// Tab Background
        /// </summary>
        internal static readonly Brush TabBackground = GetBrushFromHex("#F1F1E6");


        /// <summary>
        /// PageList text color
        /// </summary>
        internal static readonly Brush PageListForeground = GetBrushFromHex("#333333");


        /// <summary>
        /// PageList Background color
        /// </summary>
        internal static readonly Brush PageListBackground = GetBrushFromHex("#d3d3c8");



        /// <summary>
        /// get color brush from Hex string 
        /// </summary>
        /// <param name="color">hex string</param>
        /// <returns>brush</returns>
        private static SolidColorBrush GetBrushFromHex(string color) {
            return new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString(color));
        }
    }

    /// <summary>
    /// error message
    /// </summary>
    internal static class ErrorMessage {
        internal static readonly string InvalidCommand = "invalid command";
        internal static readonly string InvalidPath = "invalid path";
        internal static readonly string InvalidParams = "invalid parameters";
        internal static readonly string InvalidFile = "invalid file";
        internal static readonly string FileIsExist = "file is already exist";
        internal static readonly string NotFound = "{0} not found";
    }
}
