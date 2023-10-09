using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace LauncherC_
{
  /// <summary>
  /// Сервис для работы с данными API.
  /// </summary>
  public class ApiDataService
  {
    #region Поля и свойства

    /// <summary>
    /// Обработчик событий обновления версии сборки.
    /// </summary>
    public event EventHandler VersionCompleted;

    /// <summary>
    /// Экземляр работы с версией сборки API.
    /// </summary>
    public static ApiDataApp apiVersion = new ApiDataApp();

    /// <summary>
    /// Экземпляр конфигураций.
    /// </summary>
    private Config config = new Config();
    
    /// <summary>
    /// Экземпляр работы с файлами API и ключем.
    /// </summary>
    private Dictionary<string, ApiData> apiData = new Dictionary<string, ApiData>();

    #endregion

    #region Методы

    /// <summary>
    /// Загрузка данных из API.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Получить данные API.
    /// </summary>
    /// <returns>Возвращает список сущности файлов с ключом из API.</returns>
    public async Task<Dictionary<string, ApiData>> GetData()
    {
      if (apiData.Count == 0)
        apiData = await LoadingData();
      return apiData;
    }

    /// <summary>
    /// Очистка API файлов.
    /// </summary>
    public void ClearData() => apiData.Clear();

    /// <summary>
    /// Актуальные данные версии сборки с сайта API.
    /// </summary>
    /// <returns>Сущность актуальных данных сборки API.</returns>
    public async Task<ApiDataApp> GetActualVersionAPI() => 
      JsonSerializer.Deserialize<ApiDataApp>(new WebClient().DownloadString(Config.AppUpdate));

    /// <summary>
    /// Актуальные данные сборки из локального файла.
    /// </summary>
    /// <returns></returns>
    public async Task<ApiDataApp> GetActualVersion()
    {
      if (!File.Exists(config.VersionSave))
        return null;

      string json = File.ReadAllText(config.VersionSave);
      ApiDataApp apidataapp = JsonSerializer.Deserialize<ApiDataApp>(json);
      return apidataapp;
    }

    /// <summary>
    /// Установка актуальных данных сборки API в память.
    /// </summary>
    /// <param name="acticalVersion">Сущность актуальной сборки.</param>
    /// <param name="apiDataService">Сущность сервиса API.</param>
    /// <param name="callback">Калбэк с актуальными данными.</param>
    /// <returns></returns>
    public async Task SetVersion(ApiDataApp acticalVersion, ApiDataService apiDataService = null, Action<ApiDataApp, ApiDataService> callback = null)
    {
      apiVersion = acticalVersion;
      string json = JsonSerializer.Serialize(apiVersion);
      File.WriteAllText(config.VersionSave, json);
      callback?.Invoke(acticalVersion, apiDataService);
    }

    /// <summary>
    /// Данные версии API в памяти.
    /// </summary>
    /// <returns>Сущность с данными версии сборки.</returns>
    public ApiDataApp GetVersion() => apiVersion;

    /// <summary>
    /// Вывести не аутальные данные в files.json
    /// </summary>
    /// <returns>Список неактуальных файлов.</returns>
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

    #endregion
  }
}
