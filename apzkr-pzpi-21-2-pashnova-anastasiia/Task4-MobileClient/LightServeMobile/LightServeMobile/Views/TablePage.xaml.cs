using LightServeMobile.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Maui.Controls;

namespace LightServeMobile.Views;

public partial class TablePage : ContentPage
{
    private ObservableCollection<Table> Tables;
    private readonly string Baseurl = "https://lightservewebapi202405261630041211.azurewebsites.net/";

    public TablePage()
	{
		InitializeComponent();
        InitializeTasks();
    }

    public async void InitializeTasks()
    {
        using (HttpClient client = new HttpClient())
        {
            client.BaseAddress = new Uri(Baseurl);

            HttpResponseMessage response = await client.GetAsync($"api/Table/getAllTables?cafeId=4");

            if (response.IsSuccessStatusCode)
            {
                var tableData = await response.Content.ReadFromJsonAsync<List<Table>>();

                Tables = new ObservableCollection<Table>(tableData);
                BindingContext = this;
                TaskCollectionView.ItemsSource = Tables;
            }
            else
            {
                Tables = new ObservableCollection<Table>();
            }
        }
    }

    public void OnBackButtonClicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private async void ChangeTableAvailability(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var table = (Table)button.BindingContext;

        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);

                var response = await client.PostAsJsonAsync($"api/Table/changeTableStatus?tableId={table.Id}", !table.IsAvailable);
                if (response.IsSuccessStatusCode)
                {
                    int index = Tables.IndexOf(table);
                    if (index != -1)
                    {
                        Tables[index].IsAvailable = !table.IsAvailable;
                        TaskCollectionView.ItemsSource = null;
                        TaskCollectionView.ItemsSource = Tables;
                    }
                }
                else
                {
                    await DisplayAlert("Error", $"Failed to update table status: {response.ReasonPhrase}", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }
}