using LightServeMobile.Models;
using LightServeMobile.Views;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LightServeMobile
{
    public partial class MainPage : ContentPage
    {
        private readonly string Baseurl = "https://lightservewebapi202405261630041211.azurewebsites.net/";

        public MainPage()
        {
            InitializeComponent();
        }

        public async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text;
            string password = PasswordEntry.Text;

            if (!string.IsNullOrWhiteSpace(username) || !string.IsNullOrWhiteSpace(password))
            {
                var workerDto = new WorkerMobileDto()
                {
                    Username = username,
                    Password = password,
                    OldPassword = password,
                };

                using (var client = new HttpClient())
                {
                    var registerEndpoint = "api/Worker/login";

                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync(registerEndpoint, workerDto);

                    if (response.IsSuccessStatusCode)
                    {
                        Preferences.Set("LoggedInUsername", username);
                        await Navigation.PushAsync(new OptionPage());
                    }
                    else
                    {
                        await DisplayAlert("Помилка", "Неправильний пароль", "OK");
                    }
                }
            }
            else
            {
                await DisplayAlert("Помилка", "Username та пароль не можуть бути пустими полями", "OK");
            }
        }
    }
}
