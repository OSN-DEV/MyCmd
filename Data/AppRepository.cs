using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private static string _settingFile;
        #endregion

        #region Public Method
        /// <summary>
        /// initialize
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static AppRepository Init(string file) {
            _settingFile = file;
            GetInstanceBase(file);
            if (!System.IO.File.Exists(file)) {
                _instance.Save();
            }
            return _instance;
        }

        /// <summary>
        /// get instance
        /// </summary>
        /// <returns></returns>
        public static AppRepository GetInstance() {
            return GetInstanceBase();
        }

        /// <summary>
        /// save settings
        /// </summary>
        public void Save() {
            GetInstanceBase().SaveToXml(_settingFile);
        }
        #endregion
    }
}
