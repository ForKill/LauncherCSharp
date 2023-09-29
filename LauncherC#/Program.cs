using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LauncherC_
{
  internal class Program
  {
    static async Task Main(string[] args)
    {
      ApiDataService apiDataService = new ApiDataService();
      DownloadService downloadService = new DownloadService();
      FilesService filesService = new FilesService();

      Console.WriteLine("Загрузка apiDataService.GetActualVersion()");
      var apiVersion = await apiDataService.GetActualVersion();
      Console.WriteLine($"{apiVersion.Hash} | {apiVersion.Version}");

      Console.WriteLine("Загрузка apiDataService.GetData()");
      Dictionary<string, ApiData> apiData = await apiDataService.GetData();

      Console.WriteLine("Удаляем лишние файлы apiDataService.RemoveUnnecessaryFiles()");
      await apiDataService.RemoveUnnecessaryFiles();

      foreach (var apidata in apiData)
      {
        await downloadService.AddDownloadQueue(apidata.Key, apidata.Value);
      }

      var downloadList = await downloadService.GetDownloadQueue();

      foreach(var download in downloadList)
      {
        Console.WriteLine($"{download.Url} | {download.ApiData.Name}");
      }
      await downloadService.StartDownload();

      await filesService.CheckFiles();
      downloadList = await downloadService.GetDownloadQueue();

      foreach (var download in downloadList)
      {
        Console.WriteLine($"ПОВТНОРНАЯ ПРОВЕРКА {download.Url} | {download.ApiData.Name}");
      }
    }
  }
}
