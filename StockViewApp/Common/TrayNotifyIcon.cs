
using System;
using System.Windows.Forms;

namespace StockViewApp
{
    public class TrayNotifyIcon : IDisposable
    {
        public NotifyIcon notifyIcon;
        
        public delegate void MouseClickHandler();
        public event MouseClickHandler MouseClick;

        public TrayNotifyIcon() 
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Visible = true;
            notifyIcon.MouseClick += new MouseEventHandler(targetNotifyIcon_MouseClick);
        }

        public void targetNotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            //notifyIconMousePosition = System.Windows.Forms.Control.MousePosition; 
            MouseClick(); // The mouse clicken on the notify Icon, raise the event
            
        }
        
        #region IDisposable Members

        /// <summary>
        /// Standard IDisposable interface implementation. If you dont dispose the windows notify icon, the application
        /// closes but the icon remains in the task bar until such time as you mouse over it.
        /// </summary>
        private bool _IsDisposed = false;

        ~TrayNotifyIcon()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            // Tell the garbage collector not to call the finalizer
            // since all the cleanup will already be done.
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool IsDisposing)
        {
            if (_IsDisposed)
                return;

            if (IsDisposing)
            {
                notifyIcon.Dispose();
            }

            // Free any unmanaged resources in this section
            _IsDisposed = true;

        #endregion
        }
    }
}
