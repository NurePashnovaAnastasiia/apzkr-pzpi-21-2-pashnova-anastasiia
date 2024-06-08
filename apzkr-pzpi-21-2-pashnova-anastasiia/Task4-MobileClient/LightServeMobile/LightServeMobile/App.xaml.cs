using LightServeMobile.Extension;
using System.Globalization;

namespace LightServeMobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Translator.Instance.CultureInfo = new CultureInfo("uk-UA");
            MainPage = new AppShell();
        }
    }
}
