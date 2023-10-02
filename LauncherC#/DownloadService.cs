using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LauncherC_
{
  public class DownloadService
  {
    private List<Task<Download>> tasks = new List<Task<Download>>();
    public List<Download> downloads = new List<Download>();
    private Stopwatch stopwatch = new Stopwatch();
    private static readonly object ConsoleLock = new object();
    private FilesService filesService = new FilesService();
    private Config config = new Config();

    public async Task AddDownloadQueue(string fullPathName, ApiData apiData)
    {
      downloads.Add(new Download(Config.UrlFiles + fullPathName, apiData));
    }

    public async Task AddDownloadQueue(ApiData apiData)
    {
      downloads.Add(new Download(Config.UrlFiles + apiData.Path + apiData.Name, apiData));
    }

    public async Task<List<Download>> GetDownloadQueue() => downloads;

    public async Task DownloadAllAsync()
    {
      if (downloads.Count == 0)
        return;

      foreach (var download in downloads)
      {
        await DownloadAsync(download.Url, download.ApiData);
      }
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
        client.DownloadProgressChanged += async (sender, downloadProgressChangedEventArgs) =>
        {
          if (downloadProgressChangedEventArgs.ProgressPercentage > 0 && downloadProgressChangedEventArgs.ProgressPercentage < 100)
          {
            lock (ConsoleLock)
            {
              Lines.WriteLine(Lines.InfoLineNumber + 1, $"Прогресс: {downloadProgressChangedEventArgs.BytesReceived} / {downloadProgressChangedEventArgs.TotalBytesToReceive} ({downloadProgressChangedEventArgs.ProgressPercentage}%)");
              Lines.Write($"Скорость: {Utils.GetDownloadSpeed(downloadProgressChangedEventArgs.BytesReceived, stopwatch.Elapsed.TotalSeconds)}");
            }
          }
         };

        string fullPath = config.FilesPath + "\\" + apiData.Name;
        try
        {
          stopwatch.Start();
          await client.DownloadFileTaskAsync(new Uri(url), fullPath);
          stopwatch.Stop();
          await filesService.Add(apiData);
        }
        catch (WebException ex)
        {
          Console.WriteLine($"Ошибка загрузки файла {apiData.Name}: {ex.Message}");
        }
      }
    }
  }
}
