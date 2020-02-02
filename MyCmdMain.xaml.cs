using MyCmd.Component;
using MyCmd.Data;
using MyCmd.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using MyLib.Util;

namespace MyCmd {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MyCmdMain : Window {

        #region Declaration
        private AppRepository _settings;
        private List<ConsoleWindow> _consoleList = new List<ConsoleWindow>();
        private ConsoleWindow _currentConsole = null;
        private SystemMenu _systemMenu;

        private const int SysMenuSettings = 1000;
        #endregion

        // https://dobon.net/vb/dotnet/process/standardoutput.html
        // https://qiita.com/superriver/items/47fd81b206a59a230c32
        // https://qiita.com/skitoy4321/items/10c47eea93e5c6145d48
        #region Constructor
        public MyCmdMain() {
            InitializeComponent();
            this.Initialize();
        }
        #endregion

        #region Event
            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void Window_KeyDown(object sender, KeyEventArgs e) {

            if (Key.T == e.Key && Common.IsModifierPressed(ModifierKeys.Control)) {
                // Ctrl + T : add new Tab
                e.Handled = true;
                this.AddTab();
                return;
            }

            if (Key.C == e.Key && Common.IsModifierPressed(ModifierKeys.Control)) {
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandLine"></param>
        private void Console_ShowCommandLine(List<string> commandLine) {
            var dialog = new CommandList(commandLine);
            dialog.Owner = this;
            if (true != dialog.ShowDialog()) {
                return;
            }
        }


        /// <summary>
        /// SystemMenu
        /// </summary>
        /// <param name="e"></param>
        private void SystemMenu_SystemMenuEvent(SystemMenu.SystemMenuEventArgs e) {
            switch(e.MenuId) {
                case SysMenuSettings:
                    break;
            }
        }
        #endregion

        #region Private Method
        /// <summary>
        /// initialize
        /// </summary>
        private void Initialize() {
            this._settings = AppRepository.GetInstance();


            // restore window position
            double pos = Common.GetWindowPosition(this._settings.Pos.X, this._settings.Size.W, SystemParameters.VirtualScreenWidth);
            if (0 <= pos) {
                this.Left = pos;
            }
            pos = Common.GetWindowPosition(this._settings.Pos.Y, this._settings.Size.W, SystemParameters.VirtualScreenWidth);
            if (0 <= pos) {
                this.Top = pos;
            }

            // restore window size
            this.Width = Common.GetWindowSize(this.Width, this._settings.Size.W, SystemParameters.WorkArea.Width);
            this.Height = Common.GetWindowSize(this.Height, this._settings.Size.H, SystemParameters.WorkArea.Height);


            // show title
            var fullname = typeof(App).Assembly.Location;
            var info = System.Diagnostics.FileVersionInfo.GetVersionInfo(fullname);
            this.Title= $"MyCmd({info.ProductMajorPart}.{info.ProductMinorPart}.{info.ProductPrivatePart})";

            // add console
            this.AddConsole();

            // Add Event
            this.Closing += (sender, e) => {
                this._settings.Pos.X = this.Left;
                this._settings.Pos.Y = this.Top;
                this._settings.Size.H = this.Height;
                this._settings.Size.W = this.Width;
                this._settings.Save();

                foreach (var console in this._consoleList) {
                    console.CloseProc(true);
                }
            };
            this.cTab.TabChanged += (index) => {
                for(int i=0; i < this._consoleList.Count; i++) {
                    this._consoleList[i].Visibility = (i == index) ? Visibility.Visible : Visibility.Collapsed;
                }
            };

            // Add SystemMenu
            //this.Loaded += (sender, e) => {
            //    this._systemMenu = new SystemMenu(this);
            //    this._systemMenu.SystemMenuEvent += SystemMenu_SystemMenuEvent;
            //    this._systemMenu.AddMenu("設定", SysMenuSettings);
            //    this._systemMenu.StartHook();
            //};
            //this.Closed += (sender, e) => {
            //    this._systemMenu.Dispose();
            //};

        }


        /// <summary>
        /// add tab
        /// </summary>
        private void AddTab() {
            if (Constant.MaxTabCount <= this._consoleList.Count) {
                AppCommon.DebugLog("can't add new tab any more");
                return;
            }
            this.AddConsole();
            this.cTab.SelectTab(this._consoleList.Count - 1);
            foreach(var console in this._consoleList) {
                if(this._currentConsole == console) {
                    console.Visibility = Visibility.Visible;
                } else {
                    console.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// add console
        /// </summary>
        private void AddConsole() {
            this._currentConsole = new ConsoleWindow();
            this._currentConsole.ShowCommandLine += Console_ShowCommandLine;
            Grid.SetRow(this._currentConsole, 1);
            Grid.SetColumn(this._currentConsole, 0);
            this.cContainer.Children.Add(this._currentConsole);
            this._consoleList.Add(this._currentConsole);
        }


        #endregion
    }
}
