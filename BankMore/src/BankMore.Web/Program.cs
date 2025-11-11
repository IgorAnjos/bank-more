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
    // Usar URL relativa - Nginx fará o proxy para bankmore-api-conta:8080
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

builder.Services.AddHttpClient<TransferenciaService>(client =>
{
    // Usar URL relativa - Nginx fará o proxy para bankmore-api-transferencia:8080
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

builder.Services.AddHttpClient<AuthService>(client =>
{
    // Usar URL relativa - Nginx fará o proxy para bankmore-api-conta:8080
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

await builder.Build().RunAsync();
