using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace LauncherC_
{
  public class ApiDataService
  {
    private Dictionary<string, ApiData> apiData = new Dictionary<string, ApiData>();

    private async Task<Dictionary<string, ApiData>> LoadingData()
    {
      HttpClient client = new HttpClient();
      var request = await client.GetAsync(Config.FilesHash);
      var response = await request.Content.ReadAsStringAsync();
      var result = JsonSerializer.Deserialize<ApiDataRoot>(response);
      client.Dispose();

      foreach (var file in result.apidata)
      {
        if (file.Value.hash.Length != 0)
          apiData.Add(file.Key, file.Value);
      }
      return apiData;
    }

    public async Task<Dictionary<string, ApiData>> GetData()
    {
      if (apiData.Count == 0)
        apiData = await LoadingData();

      return apiData;
    }

    public async Task<ApiDataApp> GetVersion()
    {
      return JsonSerializer.Deserialize<ApiDataApp>(new WebClient().DownloadString(Config.AppUpdate));
    }
  }
}
