using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using RPCMAS.Core.Entities;
using RPCMAS.Core.Models;

#nullable enable

namespace RPCMAS.Blazor.Components.Pages.PriceChangeRequest
{
    public partial class ViewPriceChangeRequest
    {
        [Parameter]
        public Guid Id { get; set; }

        [Inject]
        public ApiClient ApiClient { get; set; } = default!;

        public PriceChangeRequestHeaderModel? requestModel { get; set; }
        public string errorMessage { get; set; } = string.Empty;
        public bool isLoading { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadRequestAsync();
            await base.OnInitializedAsync();
        }

        private async Task LoadRequestAsync()
        {
            isLoading = true;
            errorMessage = string.Empty;

            var response = await ApiClient.GetFromJsonAsync<BaseResponseModel>($"/api/PriceChangeRequest/{Id}");

            if (response != null && response.IsSuccess && response.Data != null)
            {
                requestModel = DeserializeResponse<PriceChangeRequestHeaderModel>(response.Data);
            }
            else
            {
                requestModel = null;
                errorMessage = response?.ErrorMessage ?? "Unable to load request details.";
            }

            isLoading = false;
        }

        private static string GetChangeTypeLabel(ChangeTypeEnum changeType)
        {
            return changeType switch
            {
                ChangeTypeEnum.RegularPriceUpdate => "Regular Price Update",
                ChangeTypeEnum.Markdown => "Markdown",
                ChangeTypeEnum.Clearance => "Clearance",
                ChangeTypeEnum.PromoAdjustment => "Promo Adjustment",
                ChangeTypeEnum.PriceCorrection => "Price Correction",
                _ => changeType.ToString()
            };
        }

        private static T? DeserializeResponse<T>(object data)
        {
            return JsonConvert.DeserializeObject<T>(data.ToString() ?? string.Empty);
        }
    }
}

#nullable restore
