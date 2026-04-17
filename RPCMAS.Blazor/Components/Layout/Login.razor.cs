using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using RPCMAS.Blazor.Authentication;
using RPCMAS.Core.Models;

namespace RPCMAS.Blazor.Components.Layout
{
    public partial class Login
    {
        [Inject]
        public ApiClient ApiClient { get; set; } = default!;

        [Inject]
        public NavigationManager Navigation { get; set; } = default!;

        [Inject]
        public AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        private LoginModel loginModel { get; set; } = new()
        {
            Username = string.Empty,
            Password = string.Empty
        };

        private string errorMessage { get; set; } = string.Empty;
        private bool isSubmitting { get; set; }

        private async Task HandleLoginAsync()
        {
            isSubmitting = true;
            errorMessage = string.Empty;

            var res = await ApiClient.PostAsync<LoginResponseModel, LoginModel>("/api/auth/login", loginModel);

            if (res != null && !string.IsNullOrWhiteSpace(res.Token))
            {
                await ((CustomAuthStateProvider)AuthStateProvider).MarkUserAsAuthenticated(res);
                Navigation.NavigateTo("/");
            }
            else
            {
                errorMessage = "Invalid username or password.";
            }

            isSubmitting = false;
        }
    }
}
