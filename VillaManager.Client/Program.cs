using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using VillaManager.Client;
using VillaManager.Client.Services;
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<VillaApiService>(sp =>
    new VillaApiService(new HttpClient { BaseAddress = new Uri("http://localhost:5190/") })
);
builder.Services.AddBootstrapBlazor();
await builder.Build().RunAsync();
