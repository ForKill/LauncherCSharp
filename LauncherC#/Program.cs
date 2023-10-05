using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
/*
 * 
      // benchmarkdotnet
 * 
 * */
namespace LauncherC_
{
  internal class Program
  {
    private static ApiDataApp apiVersion;
    static async Task Main(string[] args)
    {
      Console.Title = "LauncherCSharp";
      Console.WindowWidth = Config.ConsoleWidth;
      Console.WindowHeight = Config.ConsoleHeight;
      Console.OutputEncoding = System.Text.Encoding.Default;

      ApiDataService apiDataService = new ApiDataService();
      DownloadService downloadService = new DownloadService();
      FilesService filesService = new FilesService();

      apiVersion = await apiDataService.GetActualVersion();
      Lines.VersionLineNumber = Lines.WriteLineInfo($"Сборка: v{apiVersion.Version} | HASH:{apiVersion.Hash}");
      Lines.WriteLineInfo(" 1 - Выполнить полную проверку файлов.");
      Lines.WriteLineInfo(" 2 - Просмотреть очередь на скачивание.");
      Lines.WriteLineInfo(" 3 - Начать загрузку файлов.");
      Lines.WriteLineInfo(" 4 - Проверка измененных файлов по дате изменения.");
      Lines.ErrorInfoLineNumber = Lines.WriteLine(new String(' ', Config.ConsoleWidth));
      Lines.InfoLineNumber = Lines.WriteLine(string.Empty);
    
      Lines.SetDefaultColor();

      var timer = new Timers(async () =>
      {
        ApiDataApp version = await apiDataService.GetActualVersion();
        if (version.Hash != apiVersion.Hash)
        {
          Lines.ShowInfo($"Сборка файлов обновлена", ConsoleColor.Yellow);
          Lines.WriteLineInfo(Lines.VersionLineNumber, $"Сборка: v{apiVersion.Version} | HASH:{apiVersion.Hash}");
        }
      });
      var task = timer.Start();

      ConsoleKeyInfo KeyInfo;
      do
      {
        KeyInfo = Console.ReadKey(true);
        if (Lines.ErrorTimerEnabled == false)
        {
          int intKey = (int)KeyInfo.Key;
          switch (intKey)
          {
            case 49:
              {
                Lines.DeleteFromLast(Lines.InfoLineNumber + 1);
                Lines.ShowInfo($"Получаем API списка файлов.", ConsoleColor.Gray);

                Dictionary<string, ApiData> apiData = await apiDataService.GetData();

                if(apiData.Count == 0)
                {
                  Lines.ShowErrorInfo("Данных API нет.");
                  break;
                }

                var uFiles = await apiDataService.GetUnnecessaryFiles();

                foreach (var file in uFiles)
                {
                  File.Delete(file);
                  Lines.WriteLine($"Файл \"{file}\" удален.");
                }

                var jFiles = await filesService.GetFiles();

                if(jFiles != null)
                {
                  foreach(var file in jFiles)
                  {
                    if(!apiData.Keys.Contains(file.Path))
                    {
                      await filesService.Delete(file);
                    }
                  }
                }


                await downloadService.Clear();
                foreach (var apidata in apiData)
                {
                  /*if(File.Exists(apidata.Key))
                  { 
                    Files data = await filesService.GetFileData(apidata.Key);
                    if (data != null && data.Size == apidata.Value.Size)
                      continue;
                  }*/
                  await downloadService.AddDownloadQueue(apidata.Key, apidata.Value);
                  Lines.WriteLine($"Файл \"{apidata.Key}\" добавлен в очередь на скачивание.");
                }
                break;
              }
            case 50:
              {
                Lines.DeleteFromLast(Lines.InfoLineNumber + 1);
                var downloadList = await downloadService.GetDownloadQueue();

                foreach (var download in downloadList)
                  Lines.WriteLine($"{downloadList.IndexOf(download) + 1}. {download.ApiData} ({download.Url})");
                
                break;
              }
            case 51:
              {
                Lines.DeleteFromLast(Lines.InfoLineNumber + 1);
                await downloadService.DownloadAllAsync();
                break;
              }
            case 46:
              {

                break;
              }
            default:
              {

                break;
              }
          }
        }
      } while (KeyInfo.Key != ConsoleKey.Escape);

      timer.Stop();
      task.Wait();
    }
  }
}
