using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCmd.AppUtil;
using MyLib.Data;

namespace MyCmd.Data {
    public class AppRepository : AppDataBase<AppRepository> {

        #region Declaration
        #endregion

        #region Public Property
        public class Location {
            public double X { set; get; } = -1;
            public double Y { set; get; } = -1;
        }
        public class Rect {
            public double W { set; get; } = -1;
            public double H { set; get; } = -1;
        }
        public Location Pos { set; get; } = new Location();
        public Rect Size { set; get; } = new Rect();

        public List<string> Path { set; get; } = new List<string>();
        public List<string> StartUpCommand { set; get; } = new List<string>();
        public List<string> Bookmark { set; get; } = new List<string>();
        #endregion

        #region Public Method
        /// <summary>
        /// get instance
        /// </summary>
        /// <returns></returns>
        public static AppRepository GetInstance() {
            GetInstanceBase(Constant.SettingFile);
            if (!System.IO.File.Exists(Constant.SettingFile)) {
                _instance.Save();
            }
            return _instance;
        }

        /// <summary>
        /// save settings
        /// </summary>
        public void Save() {
            GetInstanceBase().SaveToXml(Constant.SettingFile);
        }
        #endregion
    }
}
