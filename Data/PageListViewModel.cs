using MyCmd.AppUtil;
using System;
using System.Windows.Media;

namespace MyCmd.Data {
    public class PageListViewModel {
        public Byte[]Icon { set; get; } 
        public string DisplayName { set; get; }
        public Brush TextColor {
            get { return ColorDef.PageListForeground; }
        }
    }
}
