using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using RPCMAS.Core.Entities;
using RPCMAS.Core.Models;

#nullable enable

namespace RPCMAS.Blazor.Components.Pages.ItemCatalog
{
    public partial class IndexItemCatalog
    {
        [Inject]
        public ApiClient ApiClient { get; set; } = default!;

        public List<ItemCatalogModel> itemCatalogModels { get; set; } = new();
        public ItemCatalogModel? selectedItemCatalog { get; set; }
        public string searchText { get; set; } = string.Empty;
        public string errorMessage { get; set; } = string.Empty;
        public bool isLoading { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadItemCatalogsAsync();

            await base.OnInitializedAsync();
        }

        public async Task SearchItemsAsync()
        {
            await LoadItemCatalogsAsync(searchText);
        }

        public async Task HandleSearchInput(ChangeEventArgs args)
        {
            searchText = args.Value?.ToString() ?? string.Empty;
            await SearchItemsAsync();
        }

        public async Task ViewItemDetailsAsync(Guid id)
        {
            var res = await ApiClient.GetFromJsonAsync<BaseResponseModel>($"/api/ItemCatalog/{id}");

            if (res != null && res.IsSuccess && res.Data != null)
            {
                selectedItemCatalog = JsonConvert.DeserializeObject<ItemCatalogModel>(res.Data.ToString() ?? string.Empty);
            }
        }

        private async Task LoadItemCatalogsAsync(string? filter = null)
        {
            isLoading = true;
            errorMessage = string.Empty;

            var path = "/api/ItemCatalog";

            if (!string.IsNullOrWhiteSpace(filter))
            {
                path += $"?filter={Uri.EscapeDataString(filter)}";
            }

            var res = await ApiClient.GetFromJsonAsync<BaseResponseModel>(path);

            if (res != null && res.IsSuccess && res.Data != null)
            {
                itemCatalogModels = JsonConvert.DeserializeObject<List<ItemCatalogModel>>(res.Data.ToString() ?? string.Empty)
                    ?? new List<ItemCatalogModel>();

                if (!itemCatalogModels.Any())
                {
                    selectedItemCatalog = null;
                }
                else if (selectedItemCatalog == null || !itemCatalogModels.Any(item => item.Id == selectedItemCatalog.Id))
                {
                    await ViewItemDetailsAsync(itemCatalogModels.First().Id);
                }
            }
            else
            {
                itemCatalogModels = new List<ItemCatalogModel>();
                selectedItemCatalog = null;
                errorMessage = res?.ErrorMessage ?? "Unable to load item catalog.";
            }

            isLoading = false;
        }
    }
}

#nullable restore
