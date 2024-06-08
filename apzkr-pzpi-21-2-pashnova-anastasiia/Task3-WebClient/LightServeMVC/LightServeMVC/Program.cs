using LightServeMVC.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Localization;
using Microsoft.CodeAnalysis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();


builder.Services.AddSingleton<LanguageService>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMvc()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
        {
            var assemblyName = new AssemblyName(typeof(ShareResource).GetTypeInfo().Assembly.FullName);
            return factory.Create("ShareResource", assemblyName.Name);
        };
    });

var localizationOptions = new RequestLocalizationOptions();
var suportedCultures = new List<CultureInfo>
    {
        new CultureInfo("en-US"),
        new CultureInfo("uk-UA"),
    };

localizationOptions.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
localizationOptions.SupportedCultures = suportedCultures;
localizationOptions.SupportedUICultures = suportedCultures;
localizationOptions.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());

builder.Services.AddSingleton<User>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Register";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });



var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRequestLocalization(localizationOptions);
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); //
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}");

app.Run();
