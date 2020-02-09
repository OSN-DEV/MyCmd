using MyCmd.AppUtil;
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
using MyLib.Util;
using MyCmd.Component;

namespace MyCmd.Component {
    // https://qiita.com/skitoy4321/items/10c47eea93e5c6145d48


    /// <summary>
    /// Console.xaml の相互作用ロジック
    /// </summary>
    public partial class ConsoleWindow : UserControl {

        #region Declaration
        public delegate void ShowCommandLineHandler(List<string> commandLine);
        public event ShowCommandLineHandler ShowCommandLine = null;

        private CmdWrap _cmd;
        private List<string> _commandBuf = new List<string>();
        private FlowDocument _flowDoc = new FlowDocument();
        private int _buffIndex = 0;
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

        #region Public Method
        public List<string>CommandBuf {
            get { return this._commandBuf; }
        }
        #endregion

        #region Public Method
        /// <summary>
        /// close process
        /// </summary>
        /// <param name="isClosing">true:app is closing, false:otherwise</param>
        public void CloseProc(bool isClosing = false) {
            this._cmd.Dispose();
        }
        #endregion

        #region Event
        private void Command_KeyDown(object sender, KeyEventArgs e) {
            AppCommon.DebugLog("key"  + e.Key.ToString());
            if (Common.IsModifierPressed(ModifierKeys.Control) && e.Key == Key.B) {
                    e.Handled = true;
                    this._cmd.Break();
                return;
            }
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
                    //if (this.cCommand.Text.StartsWith("git")) {

                    //     git = new GitWrap(this, @"E:\UserData\my-dic\books\「わかりやすい」文章を書く全技術100");
                    //    git.CmdOutputDataReceived += (line) => {
                    //        this.AddLine(line);
                    //    };
                    //    git.CmdErrorDataReceived += (line) => {
                    //        this.AddLine(line);
                    //    };
                    //    git.Start();
                    //    // git.SendCommand(this.cCommand.Text);

                    //} else {
                    //    this._cmd.SendCommand(this.cCommand.Text);
                    //}
                    this._cmd.SendCommand(this.cCommand.Text);
                    this.cCommand.Text = "";
                    break;
                case Key.Up:
                    if (Common.IsModifierPressed(ModifierKeys.Control)) {
                        e.Handled = true;
                        if (0 < this._commandBuf.Count) {
                            this.ShowCommandLine?.Invoke(this._commandBuf);
                        }
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
        /// std data received
        /// </summary>
        /// <param name="line"></param>
        private void CmdOutputDataReceived(string line) {
            this.AddLine(line);
        }

        /// <summary>
        /// std error received
        /// </summary>
        /// <param name="line"></param>
        private void CmdErrorDataReceived(string line) {
            this.AddLine(line);
        }
        #endregion

        #region Private Method
        /// <summary>
        /// initialize
        /// </summary>
        private void Initialize() {
            // initialize component
            this._cmd = new CmdWrap(this);
            this.cResult.Document = this._flowDoc;

            // set background
            this.cResult.Foreground = ColorDef.CosoleForecground;
            this.cResult.Background = ColorDef.ConsoleBackground;

            // Add Event
            this.Loaded += (sender, e) => {
                this.cCommand.Focus();
            };
            this._cmd.CmdOutputDataReceived += CmdOutputDataReceived;
            this._cmd.CmdErrorDataReceived += CmdErrorDataReceived;

            // start process
            this._cmd.Start();
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
        #endregion

    }
}
