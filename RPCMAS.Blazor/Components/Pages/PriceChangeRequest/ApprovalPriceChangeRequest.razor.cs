using System.Globalization;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using RPCMAS.Core.Entities;
using RPCMAS.Core.Models;

#nullable enable

namespace RPCMAS.Blazor.Components.Pages.PriceChangeRequest
{
    public partial class ApprovalPriceChangeRequest
    {
        [Parameter]
        public string? ViewMode { get; set; }

        [Inject]
        public ApiClient ApiClient { get; set; } = default!;

        public List<PriceChangeRequestHeaderModel> requestHeaders { get; set; } = new();
        public PriceChangeRequestFilter filter { get; set; } = new();
        public string? selectedStatus { get; set; }
        public string? selectedChangeType { get; set; }
        public string? requestDateText { get; set; }
        public string errorMessage { get; set; } = string.Empty;
        public string successNotificationMessage { get; set; } = string.Empty;
        public bool isLoading { get; set; } = true;
        public bool isActionRunning { get; set; }
        private int successNotificationVersion { get; set; }
        private string? lastHandledViewMode;

        protected IReadOnlyList<RequestStatusEnum> requestStatuses { get; } = Enum.GetValues<RequestStatusEnum>();
        protected IReadOnlyList<ChangeTypeEnum> changeTypes { get; } = Enum.GetValues<ChangeTypeEnum>();
        protected IReadOnlyList<DepartmentEnum> departments { get; } = Enum.GetValues<DepartmentEnum>();
        protected string pageTitle => isHistoryView ? "Approved / History List" : "Approval Queue";
        protected string pageSubtitle => isHistoryView
            ? "Non-draft requests remain visible for approval history and audit."
            : "Actionable requests for approval, rejection, and applying approved prices.";
        protected string tableTitle => isHistoryView ? "History Requests" : "Approval Requests";
        protected bool isHistoryView => string.Equals(ViewMode, "history", StringComparison.OrdinalIgnoreCase);

        protected override async Task OnInitializedAsync()
        {
            await LoadRequestsAsync();
            await base.OnInitializedAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (string.Equals(ViewMode, lastHandledViewMode, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            lastHandledViewMode = ViewMode;
            await LoadRequestsAsync();
        }

        public async Task LoadRequestsAsync()
        {
            isLoading = true;
            errorMessage = string.Empty;

            ApplyFilterSelections();

            var response = await ApiClient.GetFromJsonAsync<BaseResponseModel>(BuildFilterPath());

            if (response != null && response.IsSuccess && response.Data != null)
            {
                var results = DeserializeResponse<List<PriceChangeRequestHeaderModel>>(response.Data) ?? new List<PriceChangeRequestHeaderModel>();

                if (isHistoryView)
                {
                    results = results.Where(x => x.Status != RequestStatusEnum.Draft).ToList();
                }
                else
                {
                    results = results.Where(x => x.Status == RequestStatusEnum.Submitted || x.Status == RequestStatusEnum.Approved).ToList();
                }

                requestHeaders = results
                    .OrderByDescending(x => x.RequestDate)
                    .ThenByDescending(x => x.RequestNumber)
                    .ToList();
            }
            else
            {
                requestHeaders = new List<PriceChangeRequestHeaderModel>();
                errorMessage = response?.ErrorMessage ?? "Unable to load approval requests.";
            }

            isLoading = false;
        }

        public async Task ClearFiltersAsync()
        {
            filter = new PriceChangeRequestFilter();
            selectedStatus = null;
            selectedChangeType = null;
            requestDateText = null;
            await LoadRequestsAsync();
        }

        public void HandleRequestDateChanged(ChangeEventArgs args)
        {
            requestDateText = args.Value?.ToString();
        }

        public void HandleDepartmentChanged(ChangeEventArgs args)
        {
            filter.Department = args.Value?.ToString();
        }

        public async Task ApproveRequestAsync(Guid id)
        {
            await ChangeRequestStatusAsync(id, "approve", "Request approved successfully.");
        }

        public async Task RejectRequestAsync(Guid id)
        {
            await ChangeRequestStatusAsync(id, "reject", "Request rejected successfully.");
        }

        public async Task ApplyRequestAsync(Guid id)
        {
            await ChangeRequestStatusAsync(id, "apply", "Request applied successfully. Item current prices were updated.");
        }

        private async Task ChangeRequestStatusAsync(Guid id, string action, string successMessage)
        {
            isActionRunning = true;
            errorMessage = string.Empty;

            var response = await ApiClient.PostAsync<BaseResponseModel, object>($"/api/PriceChangeRequest/{id}/{action}", new { });

            if (response != null && response.IsSuccess)
            {
                await LoadRequestsAsync();
                await ShowSuccessNotificationAsync(successMessage);
            }
            else
            {
                errorMessage = response?.ErrorMessage ?? $"Unable to {action} the request.";
            }

            isActionRunning = false;
        }

        private void ApplyFilterSelections()
        {
            filter.Status = Enum.TryParse<RequestStatusEnum>(selectedStatus, out var statusValue) ? statusValue : null;
            filter.ChangeType = Enum.TryParse<ChangeTypeEnum>(selectedChangeType, out var changeTypeValue) ? changeTypeValue : null;
            filter.RequestDate = DateTime.TryParse(requestDateText, out var dateValue) ? dateValue.Date : null;
        }

        private string BuildFilterPath()
        {
            var values = new List<string>();

            if (!string.IsNullOrWhiteSpace(filter.RequestNumber))
            {
                values.Add($"RequestNumber={Uri.EscapeDataString(filter.RequestNumber.Trim())}");
            }

            if (filter.Status.HasValue)
            {
                values.Add($"Status={(int)filter.Status.Value}");
            }

            if (!string.IsNullOrWhiteSpace(filter.Department))
            {
                values.Add($"Department={Uri.EscapeDataString(filter.Department.Trim())}");
            }

            if (filter.ChangeType.HasValue)
            {
                values.Add($"ChangeType={(int)filter.ChangeType.Value}");
            }

            if (filter.RequestDate.HasValue)
            {
                values.Add($"RequestDate={Uri.EscapeDataString(filter.RequestDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))}");
            }

            return values.Count == 0
                ? "/api/PriceChangeRequest"
                : $"/api/PriceChangeRequest?{string.Join("&", values)}";
        }

        private async Task ShowSuccessNotificationAsync(string message)
        {
            successNotificationMessage = message;
            var currentVersion = ++successNotificationVersion;
            await InvokeAsync(StateHasChanged);
            await Task.Delay(2500);

            if (currentVersion == successNotificationVersion)
            {
                successNotificationMessage = string.Empty;
                await InvokeAsync(StateHasChanged);
            }
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

        private static string GetDepartmentLabel(DepartmentEnum department)
        {
            return department switch
            {
                DepartmentEnum.MensWear => "Men’s Wear",
                DepartmentEnum.LadiesWear => "Ladies’ Wear",
                DepartmentEnum.Shoes => "Shoes",
                DepartmentEnum.Cosmetics => "Cosmetics",
                DepartmentEnum.Housewares => "Housewares",
                _ => department.ToString()
            };
        }
    }
}

#nullable restore
