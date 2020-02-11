using MyCmd.AppUtil;
using MyLib.Util;
using OsnCsLib.Common;
using OsnCsLib.File;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace MyCmd.Component {
    // https://qiita.com/skitoy4321/items/10c47eea93e5c6145d48


    /// <summary>
    /// Console.xaml の相互作用ロジック
    /// </summary>
    public partial class ConsoleWindow : UserControl {

        #region Declaration
        private readonly List<string> _commandBuf = new List<string>();
        private int _buffIndex = 0;

        private readonly FlowDocument _flowDoc = new FlowDocument();
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public ConsoleWindow() {
            InitializeComponent();
            this.Initialize();
            this.AddLine(this.CurrentPath);
        }
        #endregion

        #region Public Property
        /// <summary>
        /// current absolute path
        /// </summary>
        public string CurrentPath { set; get; } = Environment.CurrentDirectory;
        #endregion

        #region Public Method
        #endregion

        #region Event
        private void Command_KeyDown(object sender, KeyEventArgs e) {
            AppCommon.DebugLog("key"  + e.Key.ToString());
            switch (e.Key) {
                case Key.Enter:
                    if (0 == this.cCommand.Text.Length) {
                        return;
                    }
                    e.Handled = true;
                    if (this._commandBuf.Contains(this.cCommand.Text)) {
                        this._commandBuf.Remove(this.cCommand.Text);
                        this._commandBuf.Insert(0, this.cCommand.Text);
                    } else {
                        this._commandBuf.Add(this.cCommand.Text);
                        if (Constant.MaxCommandBuff < this._commandBuf.Count) {
                            this._commandBuf.RemoveAt(0);
                        }
                    }
                    //this._cmd.SendCommand(this.cCommand.Text);
                    this.cCommand.Text = "";
                    break;

                case Key.Tab:
                    e.Handled = true;
                    this.ComplementPath();
                    break;

                case Key.Up:
                    if (Common.IsModifierPressed(ModifierKeys.Control)) {
                        //e.Handled = true;
                        //if (0 < this._commandBuf.Count) {
                        //    this.ShowCommandLine?.Invoke(this._commandBuf);
                        //}
                    } else {
                        this._buffIndex++;
                        if (this.SetBufferCommand()) {
                            e.Handled = true;
                        }
                    }
                    break;
                case Key.Down:
                    this._buffIndex--;
                    if (this.SetBufferCommand()) {
                        e.Handled = true;
                    }
                    break;
                default:
                    this._buffIndex = 0;
                    break;
            }
        }
        #endregion

        #region Private Method
        /// <summary>
        /// initialize
        /// </summary>
        private void Initialize() {
            this.cResult.Document = this._flowDoc;

            // set background
            this.cResult.Foreground = ColorDef.CosoleForecground;
            this.cResult.Background = ColorDef.ConsoleBackground;

            // Add Event
            this.Loaded += (sender, e) => {
                this.cPageList.Visibility = Visibility.Collapsed;
                this.cCommand.Focus();
            };
            this.cPageList.DataSelected += (isCancel, data) => {
                this.cPageList.Visibility = Visibility.Collapsed;
                this.cCommand.Focus();
                if (!isCancel) {
                    var p = this.cPageList.Tag as List<string>;
                    this.cPageList.Tag = null;
                    this.AddPath(p[0], p[1], p[2] +data);
                }
            };
        }

        /// <summary>
        /// add line
        /// </summary>
        /// <param name="line">line</param>
        private void AddLine(string line) {
            this.cResult.Dispatcher.Invoke(() => {
                if (0 == _flowDoc.Blocks.Count) {
                    var p = new Paragraph();
                    p.Inlines.Add(new Run(line));
                    _flowDoc.Blocks.Add(p);
                    while (Constant.BufferSize < _flowDoc.Blocks.Count) {
                        _flowDoc.Blocks.Remove(_flowDoc.Blocks.FirstBlock);
                    }
                } else {
                    ((Paragraph)_flowDoc.Blocks.FirstBlock).Inlines.Add(new Run('\n' + line));
                }
                this.cResult.ScrollToEnd();
                System.Windows.Forms.Application.DoEvents();
            });
        }

        /// <summary>
        /// set buffer command
        /// </summary>
        /// <returns>true:set buffer, false:otherwiser</returns>
        private bool SetBufferCommand() {
            if (0 == this._commandBuf.Count) {
                this._buffIndex = 0;
                return false;
            }
            if (this._commandBuf.Count <= this._buffIndex) {
                this._buffIndex = 0;
            } else if (this._buffIndex < 0) {
                this._buffIndex = this._commandBuf.Count - 1;
            }
            this.cCommand.Text = this._commandBuf[this._buffIndex];
            return true;
        }

        /// <summary>
        /// complement path
        /// </summary>
        private void ComplementPath() {
            var command = this.cCommand.Text.Trim();

            string rest = "";
            string fragment = "";
            string path;
            var pos = command.LastIndexOf(" ");
            if (-1 == pos) {
                path = command;
            } else {
                rest = command.Substring(0, pos);
                path = command.Substring(pos + 1);
            }
            if (!PathUtil.IsAbsolute(path)) {
                fragment = this.CurrentPath;
            }

            var file = new PathUtil(fragment + path);
            switch(file.FileType) {
                case PathUtil.FileTypes.Directory:
                case PathUtil.FileTypes.ValidDirectry:
                    break;
                default:
                    return;
            }
            var list = file.GetChildren();
            if (null == list) {
                return;
            }

            if (1 == list.Count) {
                this.AddPath(rest, fragment, path + list[0].Name);
                return;
            } else {
                this.cPageList.Tag = new List<string> {rest, fragment, path };
                this.cPageList.Visibility = Visibility.Visible;
                this.cPageList.Setup(list);
                Util.DoEvents();
                this.cPageList.SetFocus();
            }
        }

        /// <summary>
        /// add commplement path
        /// </summary>
        /// <param name="rest">rest part of command</param>
        /// <param name="fragment">fragment</param>
        /// <param name="path">add path</param>
        private void AddPath(string rest, string fragment, string val) {
            if (new PathUtil(fragment + val).IsDirectory) {
                val += @"\";
            }
            this.cCommand.Text = $"{rest.Trim()} {val}";
            this.cCommand.SelectionStart = this.cCommand.Text.Length;
        }
        #endregion

    }
}
