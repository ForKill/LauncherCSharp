using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LauncherC_
{
  public class DownloadService
  {
    private Queue<Download> downloads;
    private List<Task<Download>> tasks = new List<Task<Download>>();
    public DownloadService() => downloads = new Queue<Download>();

    public async Task AddDownloadQueue(string fullPathName, ApiData apiData)
    {
      await Task.Run(() =>
        downloads.Enqueue(new Download(Config.UrlFiles + fullPathName, apiData))
      );
    }

    public async Task<List<Download>> GetDownloadQueue() =>
      await Task.Run(() => downloads.ToList());

    public async Task StartDownload()
    {
      if (downloads.Count == 0)
        return;
      Config config = new Config();

      var download = downloads.Dequeue();

      string filePath = config.FilesPath + "\\" + download.ApiData.Path;
      if (Directory.Exists(filePath) == false)
        Directory.CreateDirectory(filePath);

      string fileName = filePath + "\\" + download.ApiData.Name;
      await DownloadFile(download.Url, fileName);
      await StartDownload();
    }

    public async Task DownloadFile(string url, string path)
    {
      try
      {
        using (var client = new WebClient())
        {
          client.DownloadFileCompleted += (sender, args) => ComplitedCallback(path, sender, args);
          client.DownloadProgressChanged += (sender, args) => ProgressCallback(path, sender, args);
          await client.DownloadFileTaskAsync(new Uri(url), path);

        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }

    }

    private void ComplitedCallback(string path, object? sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
    {
      Console.WriteLine($"Загрузка завершена. ({path})");
      Console.WriteLine("/exit - Завершить работу программы");

      if (asyncCompletedEventArgs.Cancelled)
        Console.WriteLine("Загрузка отменена.");

      if (asyncCompletedEventArgs.Error != null)
        Console.WriteLine($"Ошибка загрузки: {asyncCompletedEventArgs.Error.ToString()}");
    }

    private void ProgressCallback(string path, object? sender, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
    {
      Console.WriteLine($"Запрошенноебакетов: {downloadProgressChangedEventArgs.TotalBytesToReceive}");
      Console.WriteLine($"Полученноебакетов: {downloadProgressChangedEventArgs.BytesReceived}");
      Console.WriteLine($"Процент: {downloadProgressChangedEventArgs.ProgressPercentage}");
    }
  }
}
