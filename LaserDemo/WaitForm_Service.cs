using DevExpress.XtraSplashScreen;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaserDemo
{
    public class WaitForm_Service
    {
        Control parentFormWait;
        bool isShow = false;
        public WaitForm_Service(Control parentFromWait)
        {
            this.parentFormWait = parentFromWait;
        }

        IOverlaySplashScreenHandle handle = null;
        public void ShowProgressPanel()
        {
            if (isShow == false)
            {
                OverlayWindowOptions options = new OverlayWindowOptions()
                {
                    BackColor = Color.Gray,
                    FadeIn = false,
                    FadeOut = false,
                    Opacity = 0.2,
                };
                handle = SplashScreenManager.ShowOverlayForm(this.parentFormWait, options);
                isShow = true;
            }

        }

        public void CloseProgressPanel()
        {
            if (this.handle != null)
            {
                isShow = false;
                SplashScreenManager.CloseOverlayForm(this.handle);
            }

        }
    }
}
