using LightServeWebAPI.Database;
using LightServeWebAPI.Interfaces;
using LightServeWebAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization();
var localizationOptions = new RequestLocalizationOptions();

var supportedCultures = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("uk-UA")
};

localizationOptions.SupportedCultures = supportedCultures;
localizationOptions.SupportedUICultures = supportedCultures;
localizationOptions.SetDefaultCulture("en-US");
localizationOptions.ApplyCurrentCultureToResponseHeaders = true;


builder.Services.AddControllers()
    .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore); ;



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<LightServeContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddTransient<IOwnerRepository, OwnerRepository>();
builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();
builder.Services.AddTransient<IWorkerRepository, WorkerRepository>();
builder.Services.AddTransient<ICafeRepository, CafeRepository>();
builder.Services.AddTransient<IMenuRepository, MenuRepository>();
builder.Services.AddTransient<IDishRepository, DishRepository>();
builder.Services.AddTransient<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<IOrderDetailsRepository, OrderDetailsRepository>();
builder.Services.AddTransient<ITableRepository, TableRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
