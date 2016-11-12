using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Xaml;

namespace CloudAppBrowser.Views.Subsystems
{
    public class EnvironmentSubsystemView : Panel
    {
        public EnvironmentSubsystemView()
        {
            XamlReader.Load(this);
        }
    }
}
