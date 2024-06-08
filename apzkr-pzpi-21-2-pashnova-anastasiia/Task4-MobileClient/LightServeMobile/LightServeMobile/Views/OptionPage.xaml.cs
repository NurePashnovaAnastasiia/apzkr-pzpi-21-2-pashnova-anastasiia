using LightServeMobile.Extension;
using System.Globalization;

namespace LightServeMobile.Views;

public partial class OptionPage : ContentPage
{
	public OptionPage()
	{
		InitializeComponent();
	}

    public async void OnViewTableClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TablePage());
    }

    public async void OnViewOrderClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new OrdersPage());
    }

    public async void OnChangePasswordClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PasswordPage());
    }

    public async void OnLogoutClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }

    private void rbEnglish_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        Translator.Instance.CultureInfo = new CultureInfo("en-US");
        Translator.Instance.OnPropertyChanged();
    }

    private void rbUkranian_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        Translator.Instance.CultureInfo = new CultureInfo("uk-UA");
        Translator.Instance.OnPropertyChanged();
    }
}