using LightServeMobile.Resources.Strings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightServeMobile.Extension
{
    public class Translator : INotifyPropertyChanged
    {
        public string this[string key]
        {
            get => AppResources.ResourceManager.GetString(key, CultureInfo);
        }

        public CultureInfo CultureInfo { get; set; }

        public static Translator Instance { get; set; } = new Translator();


        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
