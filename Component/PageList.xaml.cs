using MyCmd.AppUtil;
using MyCmd.Data;
using OsnCsLib.Common;
using OsnCsLib.WPFComponent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyCmd.Component {
    /// <summary>
    /// PageList
    /// </summary>
    public partial class PageList : UserControl {

        #region Declaration
        private const int CountPerPage = 5;
        private int _currentPage = 0;
        private int _TotalPage = 0;
        List<PageListViewModel> _listData;
        #endregion

        #region Public Event
        /// <summary>
        /// raise event when data is selected or canceld
        /// </summary>
        /// <param name="isCancel">true:selection canceled, false:otherwise</param>
        /// <param name="data">selected text, if canceled set empty string</param>
        public delegate void DataSelectedHander(bool isCancel, string data);
        public event DataSelectedHander DataSelected;
        #endregion

        #region Constructor
        public PageList() {
            InitializeComponent();
            this.Initialize();
        }
        #endregion

        #region Local Event
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListData_SizeChanged(object sender, SizeChangedEventArgs e) {
            var view = this.cListData.View as GridView;
            var w = this.cListData.ActualWidth - view.Columns[0].ActualWidth;
            const double OffSet = 25;
            if (OffSet <= w) {
                w -= OffSet;
            }
            view.Columns[1].Width = w;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListData_PreviewKeyDown(object sender, KeyEventArgs e) {
            void Handled() {
                e.Handled = true;
            }
            void ShowPage(bool nextPage) {
                Handled();
                if (nextPage) {
                    this._currentPage = Util.GetNextPage(this._currentPage, this._TotalPage);
                } else {
                    this._currentPage = Util.GetPrevPage(this._currentPage, this._TotalPage);
                }
                this.ShowCurrentPage();
            }

            switch (e.Key) {
                case Key.Left:
                    ShowPage(false);
                    this.cListData.SelectedIndex = 0;
                    break;

                case Key.Right:
                    ShowPage(true);
                    this.cListData.SelectedIndex = 0;
                    break;

                case Key.Up:
                    if (0 == this.cListData.SelectedIndex) {
                        ShowPage(false);
                        this.cListData.SelectedIndex = this.cListData.Items.Count - 1;
                    } else {
                        Handled();
                        this.cListData.SelectedIndex--;
                    }
                    break;
                case Key.Down:
                    if (this.cListData.SelectedIndex == this.cListData.Items.Count - 1) {
                        ShowPage(true);
                        this.cListData.SelectedIndex = 0;
                    } else {
                        Handled();
                        this.cListData.SelectedIndex++;
                    }
                    break;

                case Key.Escape:
                    Handled();
                    this.DataSelected?.Invoke(true, "");
                    break;

                case Key.Enter:
                    Handled();
                    var item = this.cListData.SelectedItem as PageListViewModel;
                    this.DataSelected?.Invoke(true, item?.DisplayName);
                    break;

                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                    var index = e.Key - Key.D1;
                    if (index < this.cListData.Items.Count) {
                        Handled();
                        var list = this.cListData.DataContext as ObservableCollection<PageListViewModel>;
                        this.DataSelected?.Invoke(true, list[index].DisplayName);
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListData_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (!(cListData.GetItemAt(Mouse.GetPosition(this.cListData))?.DataContext is PageListViewModel item)) {
                return;
            }
            this.DataSelected?.Invoke(true, item?.DisplayName);
        }

        #endregion

        #region Public Method
        /// <summary>
        /// set up list data
        /// </summary>
        /// <param name="src"></param>
        public void Setup(List<PageListViewModel> src) {
            this._listData = src;
            this._currentPage = 0;
            this._TotalPage = Util.CalcPageCount(this._listData.Count, CountPerPage);
            this.ShowCurrentPage();
        }
        #endregion

        #region Private Method
        /// <summary>
        /// initialize page list
        /// </summary>
        private void Initialize() {
            this.cPage.Foreground = ColorDef.PageListForeground;
            this.Background = ColorDef.PageListBackground;

            this.cListData.DataContext = new ObservableCollection<PageListViewModel>();

            var data = new List<PageListViewModel>();
            for (int i = 0; i < 21; i++) {
                data.Add(new PageListViewModel() {
                    DisplayName = i.ToString() + "item"
                });
            }
            this.Setup(data);
        }

        /// <summary>
        /// show current page data
        /// </summary>
        private void ShowCurrentPage() {
            this.cPage.Text = $"{(this._currentPage + 1)} / {this._TotalPage}";
            var list = this.cListData.DataContext as ObservableCollection<PageListViewModel>;
            list.Clear();

            var start = CountPerPage * this._currentPage;
            var end = Math.Min(start + CountPerPage, this._listData.Count);
            for (var i = start; i < end; i++) {
                list.Add(this._listData[i]);
            }
        }
        #endregion
    }
}
