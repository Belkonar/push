using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using shared.Interfaces;
using shared.services;
using shared.Services;
using ui;
using ui.Services;
using OrganizationService = ui.Services.OrganizationService;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<ApiAuthorizationMessageHandler>();

// ---- START Services ----
builder.Services.AddTransient<IThingController, ThingService>();
// ----- END Services -----

builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiRoot")!);
}).AddHttpMessageHandler<ApiAuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("api"));

builder.Services.AddSingleton<ToastService>();
builder.Services.AddScoped<OrganizationService>();

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Auth0", options.ProviderOptions);
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.AdditionalProviderParameters.Add("audience", builder.Configuration["Auth0:Audience"]);
});

await builder.Build().RunAsync();
