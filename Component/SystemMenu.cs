using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace MyCmd.Component{
    class SystemMenu : IDisposable {

        #region Declaration
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        private static extern bool InsertMenu(IntPtr hMenu, Int32 wPosition, Int32 wFlags, Int32 wIDNewItem, string lpNewItem);

        private readonly Int32 MF_BYPOSITION = 0x400;
        private readonly Int32 MF_SEPARATOR = 0x800;
        private const Int32 ITEMONEID = 1000;
        private const Int32 ITEMTWOID = 1001;
        private readonly Int32 WM_SYSCOMMAND = 0x112;

        HwndSource _hwndSource;
        IntPtr _menuHandle;
        int _menuPos = 5;
        List<Int32> _menuIds = new List<int>();

        public class SystemMenuEventArgs : EventArgs {
            public int MenuId { set; get; }
        }
        public delegate void SystemMenuEventHandler(SystemMenuEventArgs e);
        public event SystemMenuEventHandler SystemMenuEvent;
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        public SystemMenu(Window window) {
            IntPtr windowhandle = new WindowInteropHelper(window).Handle;
            this._hwndSource = HwndSource.FromHwnd(windowhandle);
            this._menuHandle = GetSystemMenu(windowhandle, false);
            InsertMenu(this._menuHandle, this._menuPos++, MF_BYPOSITION | MF_SEPARATOR, 0, string.Empty);
        }
        #endregion

        #region Public Method
        /// <summary>
        /// add system menu
        /// </summary>
        /// <param name="text">menu text</param>
        /// <param name="id">menu item id(1000～)</param>
        public void AddMenu(string text, Int32 id) {
            InsertMenu(this._menuHandle, this._menuPos++, MF_BYPOSITION, id, text);
            this._menuIds.Add(id);
        }

        /// <summary>
        /// start hook
        /// </summary>
        public void StartHook() {
            this._hwndSource.AddHook(new HwndSourceHook(WndProc));
        }

        public void Dispose() {
            if (null != this._hwndSource) {
                this._hwndSource.RemoveHook(WndProc);
            }
            this._hwndSource = null;
        }

        #endregion

        #region Event
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wparam"></param>
        /// <param name="lparam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled) {
            if (msg == WM_SYSCOMMAND) {
                var id = wparam.ToInt32();
                if (this._menuIds.Contains(id)) {
                    handled = true;
                    var args = new SystemMenuEventArgs() { MenuId = id };
                    this.SystemMenuEvent?.Invoke(args);
                }
            }
            return IntPtr.Zero;
        }
        #endregion

    }
}
