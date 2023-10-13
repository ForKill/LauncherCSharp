using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LauncherC_
{
  /// <summary>
  /// Сервис для загрузки файлов.
  /// </summary>
  public class DownloadService
  {
    #region Поля и свойства

    /// <summary>
    /// Список файлов которые требуется скачать.
    /// </summary>
    private List<Download> downloads = new List<Download>();

    /// <summary>
    /// Экземпляр для работы с сервисом файлов.
    /// </summary>
    private FilesService filesService = new FilesService();

    /// <summary>
    /// Экземпляр конфигураций.
    /// </summary>
    private Config config = new Config();

    #endregion

    #region Методы

    /// <summary>
    /// Добавление файла в лист скачивания.
    /// </summary>
    /// <param name="fullPathName">Полный путь скачивания.</param>
    /// <param name="apiData">Сущность API данных файла.</param>
    public async Task AddDownloadQueue(string fullPathName, ApiData apiData)
    {
      downloads.Add(new Download(Config.UrlFiles + fullPathName, apiData));
    }

    /// <summary>
    /// Добавление файла в лист скачивания.
    /// </summary>
    /// <param name="apiData">Сущность API данных файла.</param>
    public async Task AddDownloadQueue(ApiData apiData)
    {
      downloads.Add(new Download(Config.UrlFiles + apiData.Path + apiData.Name, apiData));
    }

    /// <summary>
    /// Удаление из списка загрузки.
    /// </summary>
    /// <param name="download">Сущность удаляемого файла.</param>
    public async Task RemoveDownloadQueue(Download download)
    {
      downloads.Remove(download);
    }

    /// <summary>
    /// Очистка списка скачеваемых файлов.
    /// </summary>
    public async Task Clear()
    {
      downloads.Clear();
    }

    /// <summary>
    /// Список файлов для скачивания.
    /// </summary>
    /// <returns>Список сущности файлов.</returns>
    public async Task<List<Download>> GetDownloadQueue()
    {
      return downloads;
    }
    /// <summary>
    /// Кол-во в очереди.
    /// </summary>
    /// <returns>Кол-во файлов требующих скачать.</returns>
    public async Task<int> GetDownloadQueueCount()
    {
      return downloads.Count;
    }

    /// <summary>
    /// Скачать все файлы в списке.
    /// </summary>
    public async Task DownloadAllAsync()
    {
      if (downloads.Count == 0)
        return;

      Lines.DownloadLineNumber = Lines.WriteLine(Lines.InfoLineNumber + 1, $"Начинаем загрузку файлов...");
      var d = downloads.ToList();
      foreach (var download in d)
      {
        await DownloadAsync(download.Url, download.ApiData);
        await RemoveDownloadQueue(download);
      }
      Lines.LineNumber = Lines.DownloadLineNumber + 50;
      Lines.DownloadLineNumber = 0;
      Lines.DeleteFromLast(Lines.InfoLineNumber + 1);
      Lines.WriteLine("Все файлы загружены.");
    }

    /// <summary>
    /// Загрузка файла.
    /// </summary>
    /// <param name="url">Ссылка на файл.</param>
    /// <param name="apiData">Сущность файла.</param>
    private async Task DownloadAsync(string url, ApiData apiData)
    {
      string filePath = config.FilesPath + "\\" + apiData.Path;
      if (Directory.Exists(filePath) == false)
        Directory.CreateDirectory(filePath);

      using (WebClient client = new WebClient())
      {
        DownloadEvents downloadEvents = new DownloadEvents();

        client.DownloadProgressChanged += async (sender, args) =>
          downloadEvents.ProgressCallbackAsync(sender, args);

        string fullPathName = filePath + "\\" + apiData.Name;
        try
        {
          if (Lines.DownloadLineNumber == 0)
            Lines.DownloadLineNumber = Lines.WriteLine($"Загрузка файла {apiData.Name}");
          else
            Lines.WriteLine(Lines.DownloadLineNumber, $"Загрузка файла {apiData.Name}");          

          await client.DownloadFileTaskAsync(new Uri(url), fullPathName);
          
          await filesService.Add(apiData.Path + apiData.Name, apiData.Hash);
        }
        catch (WebException ex)
        {
          Lines.WriteLine($"Ошибка загрузки файла {apiData.Name}: {ex.Message}");
        }
      }
    }

    /// <summary>
    /// Скорость загрузки.
    /// </summary>
    /// <param name="downloadProgressChangedEventArgs">Данные загрузки.</param>
    /// <returns>Скорость скачивания.</returns>
    public static string GetDownloadSpeed(DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
    {
      double bytesIn = double.Parse(downloadProgressChangedEventArgs.BytesReceived.ToString());
      double totalBytes = double.Parse(downloadProgressChangedEventArgs.TotalBytesToReceive.ToString());
      double percentage = bytesIn / totalBytes * 100;

      double speed = (bytesIn / 1024) / downloadProgressChangedEventArgs.ProgressPercentage;
      return speed.ToString("0.00");
    }

    #endregion
  }
}
