using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace LauncherC_
{
  public class ApiDataService
  {
    public static ApiDataApp apiVersion = new ApiDataApp();

    private Config config = new Config();
    private Dictionary<string, ApiData> apiData = new Dictionary<string, ApiData>();
    public event EventHandler VersionCompleted;

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

    public void ClearData() => apiData.Clear();

    public async Task<ApiDataApp> GetActualVersionAPI() => 
      JsonSerializer.Deserialize<ApiDataApp>(new WebClient().DownloadString(Config.AppUpdate));

    public async Task<ApiDataApp> GetActualVersion()
    {
      if (!File.Exists(config.VersionSave))
        return null;

      string json = File.ReadAllText(config.VersionSave);
      ApiDataApp apidataapp = JsonSerializer.Deserialize<ApiDataApp>(json);
      return apidataapp;
    }

    public async Task SetVersion(ApiDataApp acticalVersion, ApiDataService apiDataService = null, Action<ApiDataApp, ApiDataService> callback = null)
    {
      apiVersion = acticalVersion;
      string json = JsonSerializer.Serialize(apiVersion);
      File.WriteAllText(config.VersionSave, json);
      callback?.Invoke(acticalVersion, apiDataService);
    }

    public ApiDataApp GetVersion() => apiVersion;

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
