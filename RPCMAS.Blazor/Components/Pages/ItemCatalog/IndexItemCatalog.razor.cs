using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using RPCMAS.Core.Entities;
using RPCMAS.Core.Models;

namespace RPCMAS.Blazor.Components.Pages.ItemCatalog
{
    public partial class IndexItemCatalog
    {
        [Inject]
        public ApiClient ApiClient { get; set; }
        public List<ItemCatalogModel> itemCatalogModels { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var res = await ApiClient.GetFromJsonAsync<BaseResponseModel>("/api/ItemCatalog");

            if (res != null && res.IsSuccess) {
                itemCatalogModels = JsonConvert.DeserializeObject<List<ItemCatalogModel>>(res.Data.ToString());
            }

            await base.OnInitializedAsync();
        }
    }
}
