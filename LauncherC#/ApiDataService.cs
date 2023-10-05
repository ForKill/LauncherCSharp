using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace LauncherC_
{
  public class ApiDataService
  {
    private Config config = new Config();
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
        if (file.Value.Hash.Length != 0)
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

    public async Task<ApiDataApp> GetActualVersion() => 
      JsonSerializer.Deserialize<ApiDataApp>(new WebClient().DownloadString(Config.AppUpdate));
    
    public async Task<List<string>> GetUnnecessaryFiles()
    {
      if (!Directory.Exists(config.FilesPath))
        Directory.CreateDirectory(config.FilesPath);

      List<string> files = new List<string>();

      var enumerateFiles = Directory.EnumerateFiles(config.FilesPath, "*.*", SearchOption.AllDirectories);
      foreach (var file in enumerateFiles)
      {
        if (apiData.ContainsKey(file.Replace(config.FilesPath, "")) == false)
          files.Add(file);
      }
      return files;
    }
}
}
