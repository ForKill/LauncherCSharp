using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LauncherC_
{
  public class DownloadService
  {
    private List<Download> downloads = new List<Download>();
    private FilesService filesService = new FilesService();
    private Config config = new Config();

    public async Task AddDownloadQueue(string fullPathName, ApiData apiData) =>
      downloads.Add(new Download(Config.UrlFiles + fullPathName, apiData));

    public async Task AddDownloadQueue(ApiData apiData) =>
      downloads.Add(new Download(Config.UrlFiles + apiData.Path + apiData.Name, apiData));

    public async Task RemoveDownloadQueue(Download download) =>
      downloads.Remove(download);

    public async Task Clear() => downloads.Clear();

    public async Task<List<Download>> GetDownloadQueue() => downloads;

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
      Lines.LineNumber = Lines.DownloadLineNumber + 3;
      Lines.DownloadLineNumber = 0;
      Lines.DeleteFromLast(Lines.InfoLineNumber + 1);
      Lines.WriteLine("Все файлы загружены.");
    }

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

        string fullPath = filePath + "\\" + apiData.Name;
        try
        {
          if(Lines.DownloadLineNumber == 0)
            Lines.DownloadLineNumber = Lines.WriteLine($"Загрузка файла {apiData.Name}");
          else
            Lines.WriteLine(Lines.DownloadLineNumber, $"Загрузка файла {apiData.Name}");

          await client.DownloadFileTaskAsync(new Uri(url), fullPath);
          
          await filesService.Add(apiData.Path + apiData.Name, apiData.Hash);
        }
        catch (WebException ex)
        {
          Lines.WriteLine($"Ошибка загрузки файла {apiData.Name}: {ex.Message}");
        }
      }
    }

    public static string GetDownloadSpeed(DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
    {
      double bytesIn = double.Parse(downloadProgressChangedEventArgs.BytesReceived.ToString());
      double totalBytes = double.Parse(downloadProgressChangedEventArgs.TotalBytesToReceive.ToString());
      double percentage = bytesIn / totalBytes * 100;

      double speed = (bytesIn / 1024) / downloadProgressChangedEventArgs.ProgressPercentage;
      return speed.ToString("0.00");
    }
  }
}
