using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BankMore.Web;
using BankMore.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar HttpClients para cada API
builder.Services.AddScoped<TokenService>();

builder.Services.AddHttpClient<ContaService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5003");
});

builder.Services.AddHttpClient<TransferenciaService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5004");
});

builder.Services.AddHttpClient<AuthService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5003");
});

await builder.Build().RunAsync();
