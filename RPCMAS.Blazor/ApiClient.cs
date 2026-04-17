using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using RPCMAS.Blazor.Authentication;
using RPCMAS.Core.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

#nullable enable

namespace RPCMAS.Blazor;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ProtectedLocalStorage _localStorage;
    private readonly NavigationManager _navigationManager;
    private readonly AuthenticationStateProvider _authStateProvider;
    public ApiClient(
        HttpClient httpClient, 
        ProtectedLocalStorage localStorage, 
        NavigationManager navigationManager, 
        AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _navigationManager = navigationManager;
        _authStateProvider = authStateProvider;
    }
    public async Task SetAuthorizeHeader()
    {
        try
        {
            var sessionState = (await _localStorage.GetAsync<LoginResponseModel>("sessionState")).Value;
            if (sessionState != null && !string.IsNullOrEmpty(sessionState.Token))
            {
                if (sessionState.TokenExpired < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                {
                    await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsLoggedOut();
                    _navigationManager.NavigateTo("/login");
                }
                else if (sessionState.TokenExpired < DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds())
                {
                    var res = await _httpClient.GetFromJsonAsync<LoginResponseModel>($"/api/auth/loginByRefeshToken?refreshToken={sessionState.RefreshToken}");
                    if (res != null)
                    {
                        await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsAuthenticated(res);
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", res.Token);
                    }
                    else
                    {
                        await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsLoggedOut();
                        _navigationManager.NavigateTo("/login");
                    }
                }
                else
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessionState.Token);
                }

                var requestCulture = new RequestCulture(
                        CultureInfo.CurrentCulture,
                        CultureInfo.CurrentUICulture
                    );
                var cultureCookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);

                _httpClient.DefaultRequestHeaders.Add("Cookie", $"{CookieRequestCultureProvider.DefaultCookieName}={cultureCookieValue}");
            }
        }
        catch (Exception ex)
        {
            _navigationManager.NavigateTo("/login");
        }
    }

    public async Task<T?> GetFromJsonAsync<T>(string path)
    {
        await SetAuthorizeHeader();
        return await _httpClient.GetFromJsonAsync<T>(path);
    }

    public async Task<T1?> PostAsync<T1, T2>(string path, T2 postModel)
    {
        await SetAuthorizeHeader();
        var res = await _httpClient.PostAsJsonAsync(path, postModel);
        if (res != null && res.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<T1>(await res.Content.ReadAsStringAsync());
        }
        return default;
    }

    public async Task<T1?> PutAsync<T1, T2>(string path, T2 postModel)
    {
        await SetAuthorizeHeader();
        var res = await _httpClient.PutAsJsonAsync(path, postModel);
        if (res != null && res.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<T1>(await res.Content.ReadAsStringAsync());
        }
        return default;
    }

    public async Task<T?> DeleteAsync<T>(string path)
    {
        await SetAuthorizeHeader();
        return await _httpClient.DeleteFromJsonAsync<T>(path);
    }

}

#nullable restore
