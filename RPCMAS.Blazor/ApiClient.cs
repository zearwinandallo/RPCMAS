using Newtonsoft.Json;
using RPCMAS.Core.Models;

#nullable enable

namespace RPCMAS.Blazor;

public class ApiClient(HttpClient httpClient)
{
    public async Task<T?> GetFromJsonAsync<T>(string path)
    {
        return await httpClient.GetFromJsonAsync<T>(path);
    }

    public async Task<T1?> PostAsync<T1, T2>(string path, T2 postModel)
    {
        var res = await httpClient.PostAsJsonAsync(path, postModel);
        if (res != null && res.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<T1>(await res.Content.ReadAsStringAsync());
        }
        return default;
    }

    public async Task<T1?> PutAsync<T1, T2>(string path, T2 postModel)
    {
        var res = await httpClient.PutAsJsonAsync(path, postModel);
        if (res != null && res.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<T1>(await res.Content.ReadAsStringAsync());
        }
        return default;
    }

    public async Task<T?> DeleteAsync<T>(string path)
    {
        return await httpClient.DeleteFromJsonAsync<T>(path);
    }

    public Task<BaseResponseModel?> GetItemCatalogsAsync(string? filter = null)
    {
        var path = "/api/ItemCatalog";

        if (!string.IsNullOrWhiteSpace(filter))
        {
            path += $"?filter={Uri.EscapeDataString(filter)}";
        }

        return GetFromJsonAsync<BaseResponseModel>(path);
    }

    public Task<BaseResponseModel?> GetItemCatalogByIdAsync(Guid id)
    {
        return GetFromJsonAsync<BaseResponseModel>($"/api/ItemCatalog/{id}");
    }

}

#nullable restore
