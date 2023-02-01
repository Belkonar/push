using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace ui.Services;

public class ApiAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public ApiAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigation, IConfiguration config) : base(provider, navigation)
    {
        ConfigureHandler(
            authorizedUrls: new []{ config.GetValue<string>("ApiRoot") });
    }
}