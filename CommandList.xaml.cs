using Microsoft.Win32;
using MyCmd.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyCmd {
    /// <summary>
    /// command list
    /// </summary>
    public partial class CommandList : Window {

        #region Declaration
        private class CommandModel {
            public int Index { set; get; }
            public string Command { set; get; }
        }
        private ObservableCollection<CommandModel> _model = new ObservableCollection<CommandModel>();
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public CommandList() {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandList"></param>
        public CommandList(List<string> commandList) {
            InitializeComponent();
            for (int i = 0; i < commandList.Count; i++) {
                this._model.Add(new CommandModel() {
                    Index = i,
                    Command = commandList[i]
                });
            }
            for (int i = this._model.Count; i < Constant.MaxCommandBuff; i++) {
                this._model.Add(new CommandModel() {
                    Index = i,
                    Command = ""
                });
            }
            this.cCommandList.DataContext = this._model;
        }
        #endregion

        private void CommandList_MouseDoubleClick(object sender, MouseButtonEventArgs e) {

        }

        private void Edit_Click(object sender, RoutedEventArgs e) {
            var model = ((Button)e.Source).DataContext as CommandModel;

        }
        private void Delete_Click(object sender, RoutedEventArgs e) {
            var model = ((Button)sender).Tag as CommandModel;
        }
    }

}