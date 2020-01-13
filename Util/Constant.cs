using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MyLib.Util;
namespace MyCmd.Util {
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
        internal static readonly Brush CosoleForecground = GetBrushFromHex("#626264");

        /// <summary>
        /// console background color
        /// </summary>
        //        internal static readonly Brush ConsoleBackground = GetBrushFromHex("#006064");
        internal static readonly Brush ConsoleBackground = GetBrushFromHex("#EBEEF5");

        /// <summary>
        /// TabBackground
        /// </summary>
        internal static readonly Brush TabBackground = GetBrushFromHex("#F1F1E6");

        /// <summary>
        /// get color brush from Hex string 
        /// </summary>
        /// <param name="color">hex string</param>
        /// <returns>brush</returns>
        private static SolidColorBrush GetBrushFromHex(string color) {
            return new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString(color));
        }
    }
}
