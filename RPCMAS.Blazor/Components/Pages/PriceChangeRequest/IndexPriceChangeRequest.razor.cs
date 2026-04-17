using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using RPCMAS.Core.Entities;
using RPCMAS.Core.Models;

#nullable enable

namespace RPCMAS.Blazor.Components.Pages.PriceChangeRequest
{
    public partial class IndexPriceChangeRequest
    {
        [Inject]
        public ApiClient ApiClient { get; set; } = default!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [SupplyParameterFromQuery(Name = "mode")]
        public string? Mode { get; set; }

        [SupplyParameterFromQuery(Name = "id")]
        public Guid? QueryRequestId { get; set; }

        public List<PriceChangeRequestHeaderModel> requestHeaders { get; set; } = new();
        public List<ItemCatalogModel> itemCatalogs { get; set; } = new();
        public PriceChangeRequestFilter filter { get; set; } = new();
        public string? selectedStatus { get; set; }
        public string? selectedChangeType { get; set; }
        public string? requestDateText { get; set; }
        public string errorMessage { get; set; } = string.Empty;
        public string successNotificationMessage { get; set; } = string.Empty;
        public bool isLoading { get; set; } = true;
        public bool isActionRunning { get; set; }
        private int successNotificationVersion { get; set; }

        public bool showModal { get; set; }
        public bool isModalLoading { get; set; }
        public bool isModalSaving { get; set; }
        public bool isEditingModal { get; set; }
        public bool canEditModal { get; set; }
        public Guid? editingRequestId { get; set; }
        public string modalTitle { get; set; } = "Create Request";
        public string modalErrorMessage { get; set; } = string.Empty;
        public List<string> modalValidationMessages { get; set; } = new();
        public PriceChangeRequestHeaderModel? modalRequestModel { get; set; }
        public string currentUserName { get; set; } = string.Empty;

        private string? lastHandledMode;
        private Guid? lastHandledRequestId;

        protected IReadOnlyList<RequestStatusEnum> requestStatuses { get; } = Enum.GetValues<RequestStatusEnum>();
        protected IReadOnlyList<ChangeTypeEnum> changeTypes { get; } = Enum.GetValues<ChangeTypeEnum>();
        protected IReadOnlyList<DepartmentEnum> departments { get; } = Enum.GetValues<DepartmentEnum>();

        protected override async Task OnInitializedAsync()
        {
            await LoadCurrentUserAsync();
            await LoadItemCatalogsAsync();
            await LoadRequestsAsync();
            await base.OnInitializedAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (string.Equals(Mode, lastHandledMode, StringComparison.OrdinalIgnoreCase)
                && QueryRequestId == lastHandledRequestId)
            {
                return;
            }

            lastHandledMode = Mode;
            lastHandledRequestId = QueryRequestId;

            if (string.Equals(Mode, "create", StringComparison.OrdinalIgnoreCase))
            {
                await OpenCreateModalAsync();
                ClearQueryString();
            }
            else if (string.Equals(Mode, "edit", StringComparison.OrdinalIgnoreCase) && QueryRequestId.HasValue)
            {
                await OpenEditModalAsync(QueryRequestId.Value);
                ClearQueryString();
            }
        }

        public async Task LoadRequestsAsync()
        {
            isLoading = true;
            errorMessage = string.Empty;

            ApplyFilterSelections();

            var response = await ApiClient.GetFromJsonAsync<BaseResponseModel>(BuildFilterPath());

            if (response != null && response.IsSuccess && response.Data != null)
            {
                requestHeaders = DeserializeResponse<List<PriceChangeRequestHeaderModel>>(response.Data)
                    ?.OrderByDescending(x => x.RequestDate)
                    .ThenByDescending(x => x.RequestNumber)
                    .ToList()
                    ?? new List<PriceChangeRequestHeaderModel>();
            }
            else
            {
                requestHeaders = new List<PriceChangeRequestHeaderModel>();
                errorMessage = response?.ErrorMessage ?? "Unable to load price change requests.";
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

        public void HandleFilterDepartmentChanged(ChangeEventArgs args)
        {
            filter.Department = args.Value?.ToString();
        }

        public async Task SubmitRequestAsync(Guid id)
        {
            isActionRunning = true;
            errorMessage = string.Empty;

            var response = await ApiClient.PostAsync<BaseResponseModel, object>($"/api/PriceChangeRequest/{id}/submit", new { });

            if (response != null && response.IsSuccess)
            {
                await LoadRequestsAsync();
                await ShowSuccessNotificationAsync("Request submitted successfully.");
            }
            else
            {
                errorMessage = response?.ErrorMessage ?? "Unable to submit the request.";
            }

            isActionRunning = false;
        }

        public async Task CancelRequestAsync(Guid id)
        {
            isActionRunning = true;
            errorMessage = string.Empty;

            var response = await ApiClient.PostAsync<BaseResponseModel, object>($"/api/PriceChangeRequest/{id}/cancel", new { });

            if (response != null && response.IsSuccess)
            {
                await LoadRequestsAsync();
                await ShowSuccessNotificationAsync("Request cancelled successfully.");
            }
            else
            {
                errorMessage = response?.ErrorMessage ?? "Unable to cancel the request.";
            }

            isActionRunning = false;
        }

        public Task OpenCreateModalAsync()
        {
            showModal = true;
            isModalLoading = false;
            isEditingModal = false;
            canEditModal = true;
            editingRequestId = null;
            modalTitle = "Create Request";
            modalErrorMessage = string.Empty;
            modalValidationMessages.Clear();
            modalRequestModel = CreateEmptyRequest();
            ApplyCurrentUserToModalRequest();
            AddDetailRow();
            return Task.CompletedTask;
        }

        public async Task OpenEditModalAsync(Guid id)
        {
            showModal = true;
            isModalLoading = true;
            isEditingModal = true;
            canEditModal = false;
            editingRequestId = id;
            modalTitle = "Edit Request";
            modalErrorMessage = string.Empty;
            modalValidationMessages.Clear();
            modalRequestModel = null;

            var response = await ApiClient.GetFromJsonAsync<BaseResponseModel>($"/api/PriceChangeRequest/{id}");

            if (response != null && response.IsSuccess && response.Data != null)
            {
                modalRequestModel = DeserializeResponse<PriceChangeRequestHeaderModel>(response.Data);
                ApplyCurrentUserToModalRequest();
                canEditModal = modalRequestModel?.Status == RequestStatusEnum.Draft;
            }
            else
            {
                modalErrorMessage = response?.ErrorMessage ?? "Unable to load request.";
            }

            isModalLoading = false;
        }

        private void ApplyCurrentUserToModalRequest()
        {
            if (modalRequestModel == null || string.IsNullOrWhiteSpace(currentUserName))
            {
                return;
            }

            modalRequestModel.RequestedBy = currentUserName;
        }

        public void CloseModal()
        {
            showModal = false;
            isModalLoading = false;
            isModalSaving = false;
            isEditingModal = false;
            canEditModal = false;
            editingRequestId = null;
            modalErrorMessage = string.Empty;
            modalValidationMessages.Clear();
            modalRequestModel = null;
        }

        public void AddDetailRow()
        {
            modalRequestModel?.Details.Add(new PriceChangeRequestDetailModel
            {
                EffectiveDate = DateTime.Today,
                Remarks = string.Empty,
                SKU = string.Empty,
                ItemName = string.Empty
            });
        }

        public void RemoveDetailRow(PriceChangeRequestDetailModel? detail)
        {
            if (modalRequestModel == null || detail == null)
            {
                return;
            }

            modalRequestModel.Details.Remove(detail);
        }

        public Task HandleItemSelected(PriceChangeRequestDetailModel? detail)
        {
            if (detail == null)
            {
                return Task.CompletedTask;
            }

            var item = itemCatalogs.FirstOrDefault(x => x.SKU == detail.SKU);

            if (item == null)
            {
                detail.SKU = string.Empty;
                detail.ItemName = string.Empty;
                detail.CurrentPrice = 0;
                detail.ProposedNewPrice = 0;
                detail.MarkdownPercentage = 0;
                StateHasChanged();
                return Task.CompletedTask;
            }

            detail.SKU = item.SKU;
            detail.ItemName = item.ItemName;
            detail.CurrentPrice = item.CurrentPrice;

            UpdateMarkdownPercentage(detail);
            StateHasChanged();
            return Task.CompletedTask;
        }

        public void HandleModalDepartmentChanged(ChangeEventArgs args)
        {
            if (modalRequestModel != null)
            {
                modalRequestModel.Department = args.Value?.ToString() ?? string.Empty;
            }
        }

        public void HandleProposedPriceChanged(PriceChangeRequestDetailModel? detail, string? proposedPriceText)
        {
            if (detail == null)
            {
                return;
            }

            if (!decimal.TryParse(proposedPriceText, out var proposedPrice))
            {
                proposedPrice = 0;
            }

            detail.ProposedNewPrice = proposedPrice;
            UpdateMarkdownPercentage(detail);
        }

        public decimal GetMarkdownPercentage(decimal currentPrice, decimal proposedNewPrice)
        {
            if (currentPrice <= 0)
            {
                return 0;
            }

            return Math.Round(((currentPrice - proposedNewPrice) / currentPrice) * 100, 2);
        }

        public async Task SaveDraftFromModalAsync()
        {
            await SaveModalAsync(submitAfterSave: false);
        }

        public async Task SaveAndSubmitFromModalAsync()
        {
            await SaveModalAsync(submitAfterSave: true);
        }

        private async Task SaveModalAsync(bool submitAfterSave)
        {
            modalValidationMessages.Clear();
            modalErrorMessage = string.Empty;

            if (!ValidateModalRequest())
            {
                return;
            }

            if (modalRequestModel == null)
            {
                modalErrorMessage = "Request is not ready to save.";
                return;
            }

            isModalSaving = true;
            ApplyCurrentUserToModalRequest();
            PrepareDetailsForSave();

            BaseResponseModel? response;
            Guid requestId;

            if (isEditingModal && editingRequestId.HasValue)
            {
                response = await ApiClient.PutAsync<BaseResponseModel, PriceChangeRequestHeaderModel>($"/api/PriceChangeRequest/{editingRequestId.Value}", modalRequestModel);
                requestId = editingRequestId.Value;
            }
            else
            {
                response = await ApiClient.PostAsync<BaseResponseModel, PriceChangeRequestHeaderModel>("/api/PriceChangeRequest", modalRequestModel);
                requestId = Guid.Empty;
            }

            if (response == null || !response.IsSuccess || response.Data == null)
            {
                modalErrorMessage = response?.ErrorMessage ?? "Unable to save request.";
                isModalSaving = false;
                return;
            }

            var savedRequest = DeserializeResponse<PriceChangeRequestHeaderModel>(response.Data);

            if (savedRequest == null)
            {
                modalErrorMessage = "Unable to read saved request.";
                isModalSaving = false;
                return;
            }

            requestId = savedRequest.Id;

            if (submitAfterSave)
            {
                var submitResponse = await ApiClient.PostAsync<BaseResponseModel, object>($"/api/PriceChangeRequest/{requestId}/submit", new { });

                if (submitResponse == null || !submitResponse.IsSuccess)
                {
                    modalErrorMessage = submitResponse?.ErrorMessage ?? "Draft was saved but could not be submitted.";
                    isModalSaving = false;
                    return;
                }
            }

            isModalSaving = false;
            CloseModal();
            await LoadRequestsAsync();
            await ShowSuccessNotificationAsync(submitAfterSave
                ? (isEditingModal ? "Request updated and submitted successfully." : "Request created and submitted successfully.")
                : (isEditingModal ? "Draft request updated successfully." : "Draft request created successfully."));
        }

        private async Task LoadItemCatalogsAsync()
        {
            var response = await ApiClient.GetFromJsonAsync<BaseResponseModel>("/api/ItemCatalog");

            if (response != null && response.IsSuccess && response.Data != null)
            {
                itemCatalogs = DeserializeResponse<List<ItemCatalogModel>>(response.Data)
                    ?.OrderBy(x => x.ItemName)
                    .ToList()
                    ?? new List<ItemCatalogModel>();
            }
            else
            {
                itemCatalogs = new List<ItemCatalogModel>();
                errorMessage = response?.ErrorMessage ?? "Unable to load item catalog.";
            }
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

        private bool ValidateModalRequest()
        {
            if (modalRequestModel == null)
            {
                modalValidationMessages.Add("Request could not be loaded.");
                return false;
            }

            if (isEditingModal && modalRequestModel.Status != RequestStatusEnum.Draft)
            {
                modalValidationMessages.Add("Only Draft requests can be edited.");
            }

            if (string.IsNullOrWhiteSpace(modalRequestModel.Department))
            {
                modalValidationMessages.Add("Department is required.");
            }

            if (string.IsNullOrWhiteSpace(modalRequestModel.RequestedBy))
            {
                modalValidationMessages.Add("Requested By is required.");
            }

            if (string.IsNullOrWhiteSpace(modalRequestModel.ReasonOrJustification))
            {
                modalValidationMessages.Add("Reason / Justification is required.");
            }

            if (modalRequestModel.Details.Count == 0)
            {
                modalValidationMessages.Add("A request must contain at least one item.");
            }

            foreach (var detail in modalRequestModel.Details.Select((value, index) => new { value, index }))
            {
                if (string.IsNullOrWhiteSpace(detail.value.SKU))
                {
                    modalValidationMessages.Add($"Row {detail.index + 1}: Item is required.");
                }

                if (detail.value.ProposedNewPrice <= 0)
                {
                    modalValidationMessages.Add($"Row {detail.index + 1}: Proposed new price must be greater than zero.");
                }

                if (detail.value.ProposedNewPrice == detail.value.CurrentPrice && detail.value.CurrentPrice > 0)
                {
                    modalValidationMessages.Add($"Row {detail.index + 1}: Proposed new price cannot equal current price.");
                }
            }

            return modalValidationMessages.Count == 0;
        }

        private void PrepareDetailsForSave()
        {
            if (modalRequestModel == null)
            {
                return;
            }

            foreach (var detail in modalRequestModel.Details)
            {
                detail.MarkdownPercentage = GetMarkdownPercentage(detail.CurrentPrice, detail.ProposedNewPrice);
                if (detail.EffectiveDate == default)
                {
                    detail.EffectiveDate = DateTime.Today;
                }
            }
        }

        private void UpdateMarkdownPercentage(PriceChangeRequestDetailModel detail)
        {
            detail.MarkdownPercentage = GetMarkdownPercentage(detail.CurrentPrice, detail.ProposedNewPrice);
        }

        private void ClearQueryString()
        {
            if (string.IsNullOrWhiteSpace(Mode) && !QueryRequestId.HasValue)
            {
                return;
            }

            NavigationManager.NavigateTo("/pricechangerequest", replace: true);
        }

        private static PriceChangeRequestHeaderModel CreateEmptyRequest()
        {
            return new PriceChangeRequestHeaderModel
            {
                Department = GetDepartmentLabel(DepartmentEnum.MensWear),
                RequestedBy = string.Empty,
                ReasonOrJustification = string.Empty,
                ChangeType = ChangeTypeEnum.RegularPriceUpdate,
                Status = RequestStatusEnum.Draft
            };
        }

        private async Task LoadCurrentUserAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            currentUserName = authState.User.Identity?.Name ?? string.Empty;
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
