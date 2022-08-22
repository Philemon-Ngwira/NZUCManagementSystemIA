using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using NZUCManagementSystemIA.Client;
using NZUCManagementSystemIA.Client.Interfaces;
using NZUCManagementSystemIA.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("NZUCManagementSystemIA.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("NZUCManagementSystemIA.ServerAPI"));
builder.Services.AddMudServices();
builder.Services.AddScoped<IGenericRepositoryService, GenericRepositoryService>();
builder.Services.AddScoped<IMailingServiceClient, MailingServiceClient>();


builder.Services.AddApiAuthorization();

await builder.Build().RunAsync();
