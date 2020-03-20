using MyCmd.AppUtil;
using MyCmd.Command;
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

        private List<CommandBase> _commandList;
        private PathUtil _path = new PathUtil(System.Environment.CurrentDirectory, @"\");

        private enum DataTypes {
            Complement,
            BookMark,
        }
        private class UserData {
            public DataTypes DataType;
            public string StringData;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public ConsoleWindow() {
            InitializeComponent();
            this.Initialize();
        }
        #endregion

        #region Public Property
        #endregion

        #region Public Method
        /// <summary>
        /// show bookmark list
        /// </summary>
        /// <param name="bookmarks"></param>
        public void ShowBookmarkList(List<string>bookmarks) {
            var list = new List<PathInfo>();
            foreach(var bookmark in bookmarks) {
                list.Add(new PathInfo(bookmark));
            }

            this.cPageList.Visibility = Visibility.Visible;
            var userData = new UserData() {
                DataType = DataTypes.BookMark
            };
            this.cPageList.Setup(list, userData);
            Util.DoEvents();
            this.cPageList.SetFocus();
        }
        #endregion

        #region Event
        private void Command_KeyDown(object sender, KeyEventArgs e) {
            AppCommon.DebugLog("key" + e.Key.ToString());
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
                    this.SendCommand();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <param name="userData"></param>
        private void CommandErrorReceived(string key, string command, string data, object userData) {
            this.AddLine(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <param name="userData"></param>
        private void CommandDataReceived(string key, string command, string data, object userData) {
            this.AddLine(data);
            if (CdCommand.Key == key) {
                this._path.SetPath(data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <param name="userData"></param>
        private void CommandEnd(string key, string command, string data, object userData) {
            this.AddLine("");
        }
        #endregion

        #region Private Method
        /// <summary>
        /// initialize
        /// </summary>
        private void Initialize() {
            this.cResult.Document = this._flowDoc;

            // set background
            this.cResult.Foreground = ColorDef.ConsoleForeground;
            this.cResult.Background = ColorDef.ConsoleBackground;


            // set command list
            this._commandList = new List<CommandBase>() {
                new CdCommand(),
                new LsCommand(),
                new TouchCommand(),
            };

            // Add Event
            this.Loaded += (sender, e) => {
                this.cPageList.Visibility = Visibility.Collapsed;
                this.cCommand.Focus();
            };
            this.cPageList.DataSelected += (selection, userData) => {
                this.cPageList.Visibility = Visibility.Collapsed;
                this.cCommand.Focus();
                this._path.SetAddironalPath(selection);
                var data = userData as UserData;
                switch(data.DataType) {
                    case DataTypes.Complement:
                        this.AddPath(data.StringData, "");
                        break;
                    case DataTypes.BookMark:
                        this.cCommand.Text = $"cd {selection}";
                        this.SendCommand();
                        break;
                }
            };
            this.cPageList.Canceled += (sender, e) => {
                this.cPageList.Visibility = Visibility.Collapsed;
                this.cCommand.Focus();
            };
            foreach (var command in this._commandList) {
                command.DataReceived += CommandDataReceived;
                command.ErrorReceived += CommandErrorReceived;
                command.CommandEnd += CommandEnd;
            }


            //
            this.AddLine(this._path.CurrentPath);
            this.AddLine("");

        }

        /// <summary>
        /// add line
        /// </summary>
        /// <param name="line">line</param>
        /// <param name="isError">true: error message, false: otherwise</param>
        private void AddLine(string line, bool isError = false) {
            this.cResult.Dispatcher.Invoke(() => {
                
                if (0 == _flowDoc.Blocks.Count) {
                    var p = new Paragraph();
                    p.Inlines.Add(new Run(line));
                    _flowDoc.Blocks.Add(p);
                    while (Constant.BufferSize < _flowDoc.Blocks.Count) {
                        _flowDoc.Blocks.Remove(_flowDoc.Blocks.FirstBlock);
                    }
                } else {
                    var r = new Run('\n' + line);
                    if (isError) {
                        r.Foreground = ColorDef.ConsoleErrorForeground;
                    } else {
                        r.Foreground = ColorDef.ConsoleForeground;
                    }
                    // ((Paragraph)_flowDoc.Blocks.FirstBlock).Inlines.Add(new Run('\n' + line));
                    ((Paragraph)_flowDoc.Blocks.FirstBlock).Inlines.Add(r);
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
            string path;
            var pos = command.LastIndexOf(" ");
            if (-1 == pos) {
                path = command;
            } else {
                rest = command.Substring(0, pos);
                path = command.Substring(pos + 1);
            }
            this._path.SetAddironalPath(path);
            switch (this._path.FileType) {
                case PathUtil.FileTypes.Directory:
                    break;
                case PathUtil.FileTypes.ValidDirectry:
                    this._path.CollectAdditionalPath();
                    break;
                default:
                    return;
            }
            var list = this._path.GetChildren();
            if (null == list) {
                return;
            }


            if (1 == list.Count) {
                this.AddPath(rest, list[0].Name);
                return;
            } else {
                this.cPageList.Visibility = Visibility.Visible;
                var userData = new UserData() {
                    DataType = DataTypes.Complement,
                    StringData = rest
                };

                this.cPageList.Setup(list, userData);
                Util.DoEvents();
                this.cPageList.SetFocus();
            }
        }

        /// <summary>
        /// add commplement path
        /// </summary>
        /// <param name="rest">rest part of command</param>
        /// <param name="path">add path</param>
        private void AddPath(string rest, string val) {
            if (this._path.IsDirectory) {
                val += @"\";
            }
            this.cCommand.Text = $"{rest.Trim()} {this._path.AdditionalPath}{val}".TrimStart();
            this.cCommand.SelectionStart = this.cCommand.Text.Length;
        }

        /// <summary>
        /// send command
        /// </summary>
        private void SendCommand() {
            var commandLine = this.cCommand.Text.TrimStart();
            this.AddLine("> " + commandLine);


            foreach (var command in this._commandList) {
                command.CurrentPath = this._path.CurrentPath;
                if (command.IsMatch(commandLine)) {
                    command.RunCommand(commandLine);
                    return;
                }
            }

            // if command is single, try run app if possible
            bool runApp() {
                if (-1 != commandLine.IndexOf(" ")) {
                    return false;
                }

                var file = new PathUtil(this._path.CurrentPath, commandLine);
                if (!file.IsFile) {
                    return false;
                }
                if (!Util.RunApp(file.Path)) {
                    return false;
                }

                return true;
            };
            if (!runApp()) {
                this.AddLine(ErrorMessage.InvalidCommand, true);
                this.AddLine("");
            }
        }
        #endregion

    }
}
