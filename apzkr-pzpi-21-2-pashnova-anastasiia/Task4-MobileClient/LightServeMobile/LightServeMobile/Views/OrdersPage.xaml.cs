using LightServeMobile.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace LightServeMobile.Views;

public partial class OrdersPage : ContentPage
{
    private const string BaseApiUrl = "https://lightservewebapi202405261630041211.azurewebsites.net/";
    private HttpClient _httpClient = new HttpClient();
    public ObservableCollection<Order> Orders { get; } = new ObservableCollection<Order>();

    public OrdersPage()
	{
		InitializeComponent();
        BindingContext = this;

        Task.Run(async () => await FetchOrdersAsync());
    }

    private async Task FetchOrdersAsync()
    {
        try
        {
            string url = $"{BaseApiUrl}api/Order/getAllCafeOrders?id=4";
            var orders = await _httpClient.GetFromJsonAsync<List<Order>>(url);

            if (orders != null)
            {
                Orders.Clear();
                foreach (var order in orders)
                {
                    Orders.Add(order);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to fetch orders: {ex.Message}", "OK");
        }
    }

    private async void ViewOrderDetails(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var order = (Order)button.BindingContext;

        await Navigation.PushAsync(new OrderDetailsPage(order.Id));
    }

    private async void ChangeOrderStatus(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var order = (Order)button.BindingContext;

        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseApiUrl);

                var response = await client.PostAsJsonAsync($"api/Order/changeOrderStatus?orderId={order.Id}", !order.IsDone);

                if (response.IsSuccessStatusCode)
                {
                    int index = Orders.IndexOf(order);
                    if (index != -1)
                    {
                        Orders[index].IsDone = !order.IsDone;
                        TaskCollectionView.ItemsSource = null;
                        TaskCollectionView.ItemsSource = Orders;
                    }
                }
                else
                {
                    await DisplayAlert("Error", $"Failed to change order status: {response.ReasonPhrase}", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }
}