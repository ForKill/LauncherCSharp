using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LauncherC_
{
  internal class Program
  {
    static async Task Main(string[] args)
    {
      Console.Title = "PhoneBook";
      Console.WindowWidth = Config.ConsoleWidth;
      Console.WindowHeight = Config.ConsoleHeight;
      Console.OutputEncoding = System.Text.Encoding.Default;

      ApiDataService apiDataService = new ApiDataService();
      DownloadService downloadService = new DownloadService();
      FilesService filesService = new FilesService();

      var apiVersion = await apiDataService.GetActualVersion();
      Console.WriteLine($"{apiVersion.Hash} | {apiVersion.Version}");
      Lines.VersionLineNumber = Lines.WriteLineInfo($"Сборка: v.{apiVersion.Version} | HASH:{apiVersion.Hash} | (Требуется проверка)");

      Lines.WriteLineInfo(" 1 - Выполнить полную проверку файлов.");
      Lines.WriteLineInfo(" 2 - Просмотреть очередь на скачивание.");
      Lines.WriteLineInfo(" 3 - Начать загрузку файлов.");
      Lines.WriteLineInfo(" 4 - Проверка измененных файлов по дате изменения.");
      Lines.ErrorInfoLineNumber = Lines.WriteLine(new String(' ', Config.ConsoleWidth));
      Lines.InfoLineNumber = Lines.WriteLine(string.Empty);
    
      Lines.SetDefaultColor();

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
                }
                else
                {
                  Lines.ShowInfo($"API получен, удаляем получаем и удаляем лишние файлы в папке если они существуют.", ConsoleColor.Gray);

                  var files = await apiDataService.GetUnnecessaryFiles();

                  foreach (var file in files)
                  {
                    File.Delete(file);
                    Lines.WriteLine($"Файл \"{file}\" удален.");
                  }

                  Lines.ShowInfo($"Удаление завершено, добавляем файлы для скачивания в очередь.", ConsoleColor.Gray);
                  foreach (var apidata in apiData)
                  {
                    await downloadService.AddDownloadQueue(apidata.Key, apidata.Value);
                    Lines.WriteLine($"Файл \"{apidata.Key}\" добавлен в очередь на скачивание.");
                  }
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
            case 52:
              {
                Lines.DeleteFromLast(Lines.InfoLineNumber + 1);
                var modifiedFiles = await filesService.GetModifiedFiles();
                foreach (var modifiedFile in modifiedFiles)
                {
                  await downloadService.AddDownloadQueue(modifiedFile);
                  Lines.WriteLine($"Файл \"{modifiedFile.Name}\\\\{modifiedFile.Name}\" изменен, требуется обновление.");
                }
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
    }
  }
}
