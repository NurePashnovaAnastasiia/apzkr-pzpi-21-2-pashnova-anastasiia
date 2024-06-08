using LightServeMobile.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LightServeMobile.Views;

public partial class PasswordPage : ContentPage
{
    private readonly string Baseurl = "https://lightservewebapi202405261630041211.azurewebsites.net/";
    private string loggedInUsername = Preferences.Get("LoggedInUsername", string.Empty);

    public PasswordPage()
	{
		InitializeComponent();
	}

    private void OnBackClicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    public async void OnChangeClicked(object sender, EventArgs e)
    {
        string oldPassword = OldPassoword.Text;
        string newPassword = NewPasswordEntry.Text;

        if (!string.IsNullOrWhiteSpace(oldPassword) && !string.IsNullOrWhiteSpace(newPassword))
        {
            var workerDto = new WorkerMobileDto()
            {
                Username = loggedInUsername,
                OldPassword = oldPassword,
                Password = newPassword,
            };

            using (var client = new HttpClient())
            {
                var registerEndpoint = "api/Worker/resetPassword";

                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.PutAsJsonAsync(registerEndpoint, workerDto);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Успіх", "Пароль успішно змінено", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Помилка", "Робітника не знайдено", "OK");
                }
            }
        }
        else
        {
            await DisplayAlert("Помилка", "Старий пароль та новий пароль не можуть бути пустими полями", "OK");
        }
    }
}