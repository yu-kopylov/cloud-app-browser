using Eto.Forms;

namespace CloudAppBrowser.Views.Controls
{
    public class SelectableLabel : TextBox
    {
        public SelectableLabel()
        {
            ReadOnly = true;
            ShowBorder = false;
        }
    }
}