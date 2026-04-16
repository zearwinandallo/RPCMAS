namespace RPCMAS.Blazor;

public class ApiClient(HttpClient httpClient)
{
    public async Task<T> GetFromJsonAsync<T>(string path)
    {
        return await httpClient.GetFromJsonAsync<T>(path);
    }

}
