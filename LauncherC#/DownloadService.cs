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
using System.Threading.Tasks;

namespace LauncherC_
{
  public class DownloadService
  {
    private Queue<Download> downloads;
    private List<Task<Download>> tasks = new List<Task<Download>>();
    public DownloadService() => downloads = new Queue<Download>();
    private Stopwatch stopwatch = new Stopwatch();

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

      stopwatch.Start();
      await DownloadFile(download.Url, fileName);
      await StartDownload();
    }

    public async Task DownloadFile(string url, string path)
    {
      try
      {
        using (var client = new WebClient())
        {
          DownloadEvents downloadEvents = new DownloadEvents();

          client.DownloadProgressChanged += (sender, args) => 
            downloadEvents.ProgressCallback(path, sender, args, stopwatch);

          client.DownloadFileCompleted += (sender, args) => 
            downloadEvents.ComplitedCallback(path, sender, args, stopwatch);

          await client.DownloadFileTaskAsync(new Uri(url), path);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }
  }
}
