using MyCmd.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace MyCmd.Component {
    /// <summary>
    /// Tab
    /// </summary>
    public partial class Tab : UserControl {
        
        #region Declaration
        public delegate void TabChangedHandler(int index);
        public event TabChangedHandler TabChanged;
        
        RadioButton[] _items;
        private bool _preventEvent = false;
        #endregion

        #region Constructor
        public Tab() {
            InitializeComponent();
            this._items = new RadioButton[] {
                this.cItem1,
                this.cItem2,
                this.cItem3,
                this.cItem4,
                this.cItem5
            };
            this.Initialize();
        }
        #endregion

        #region Public Method
        /// <summary>
        /// select tab. if disabled, set enable.
        /// </summary>
        /// <param name="index"></param>
        public void SelectTab(int index) {
            if (!this._items[index].IsEnabled) {
                this._items[index].IsEnabled = true;
            }
            this._preventEvent = true;
            this._items[index].IsChecked = true;
        }
        #endregion

        #region Private Method
        /// <summary>
        /// initialize
        /// </summary>
        private void Initialize() {
            this.Background = ColorDef.TabBackground;

            this._items[0].IsChecked = true;
            foreach (var item in this._items.Select((v, i) => new { v, i })) { 
                item.v.IsEnabled = false;
                item.v.Checked += (sender, e) => {
                    if (this._preventEvent) {
                        this._preventEvent = false;
                    } else {
                        var button = sender as RadioButton;
                        if (true == button.IsChecked) {
                            TabChanged?.Invoke(item.i);
                        }
                    }
                };
            }
            this._items[0].IsEnabled = true;
        }


        #endregion
    }
}
