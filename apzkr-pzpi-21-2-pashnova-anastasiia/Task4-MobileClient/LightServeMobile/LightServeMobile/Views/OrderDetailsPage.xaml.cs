using LightServeMobile.Models;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace LightServeMobile.Views;
public partial class OrderDetailsPage : ContentPage, INotifyPropertyChanged
{
    private const string BaseApiUrl = "https://lightservewebapi202405261630041211.azurewebsites.net/api";
    private readonly HttpClient _httpClient = new HttpClient();

    private Order _order;
    public Order Order
    {
        get => _order;
        set
        {
            _order = value;
            OnPropertyChanged(nameof(Order));
        }
    }

    public OrderDetailsPage(int orderId)
    {
        InitializeComponent();
        FetchOrderDetails(orderId);
        BindingContext = this;
    }

    private async Task FetchOrderDetails(int orderId)
    {
        try
        {
            string url = $"{BaseApiUrl}/Order/getOrderById?orderId={orderId}";
            Order = await _httpClient.GetFromJsonAsync<Order>(url);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to fetch order details: {ex.Message}", "OK");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
