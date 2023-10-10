using System;
using System.Net;
using System.Runtime.InteropServices;
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
    #region Кастыль юникода
    // Кастыль для нормального отображения юникода (а именно подчеркивание)
    const int STD_OUTPUT_HANDLE = -11;
    const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 4;

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
    #endregion

    static async Task Main(string[] args)
    {
      #region Кастыль юникода
      var handle = GetStdHandle(STD_OUTPUT_HANDLE);
      uint mode;
      GetConsoleMode(handle, out mode);
      mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
      SetConsoleMode(handle, mode);
      #endregion

      Console.Title = "LauncherCSharp";
      Console.WindowWidth = Config.ConsoleWidth;
      Console.WindowHeight = Config.ConsoleHeight;
      Console.OutputEncoding = System.Text.Encoding.Default;

      ApiDataService apiDataService = new ApiDataService();
      ApiDataManager apiDataManager = new ApiDataManager();
      DownloadService downloadService = new DownloadService();
      FilesService filesService = new FilesService();
      FilesManager filesManager = new FilesManager();

      ApiDataApp apiDataApp = await apiDataService.GetActualVersion();
      if (apiDataApp == null)
      {
        filesManager.DownloadAll(true);
        apiDataApp = await apiDataService.GetActualVersionAPI();
      }

      await apiDataService.SetVersion(apiDataApp);

      apiDataApp = apiDataService.GetVersion();
      Lines.VersionLineNumber = Lines.WriteLineInfo($"Сборка: v{apiDataApp.Version} | HASH:{apiDataApp.Hash}");
      Lines.WriteLineInfo(" 1 - Выполнить полную проверку файлов.");
      Lines.WriteLineInfo(" 2 - Просмотреть очередь на скачивание.");
      Lines.WriteLineInfo(" 3 - Начать загрузку файлов.");
      Lines.WriteLineInfo(" 6 - Сгенерировать новый рандомный файл API. (Эмуляция для обновления сборки)");
      Lines.WriteLineInfo(" 7 - Обновить все сгенерированные .txt файлы API. (Эмуляция обновления существующего файла)");
      Lines.WriteLineInfo(" 8 - Удалить все сгенерированные .txt файлы API. (Эмуляция обновления с удаление файлов)");
      Lines.WriteLineInfo(" 9 - Обновить сборку API. (Эмуляция для обновления сборки)");
      Lines.WriteLineInfo(" 0 - Автоматическая эмуляция сборки.");
      Lines.ErrorInfoLineNumber = Lines.WriteLine(new String(' ', Config.ConsoleWidth));
      Lines.InfoLineNumber = Lines.WriteLine(string.Empty);
    
      Lines.SetDefaultColor();

      bool autoUpdate = false;
      if(args.Length > 0)
      {
        foreach (string arg in args)
        {
          if (arg == "-autoupdate")
          {
            autoUpdate = true;

            await filesManager.CheckingForDownload(apiDataService, filesService, downloadService);
            if (await downloadService.GetDownloadQueueCount() > 0)
            {
              filesManager.DownloadAll(false);
              await downloadService.DownloadAllAsync();
            }
            else
            {
              Lines.ShowInfo($"Автообновление включено.", ConsoleColor.Green);
            }
          }
        }
      }

      var timer = new Timers(async () =>
      {
        if (await apiDataManager.CheckUpdate(apiDataService) && autoUpdate)
        {
          await filesManager.CheckingForDownload(apiDataService, filesService, downloadService);
          filesManager.DownloadAll(false);
          await downloadService.DownloadAllAsync();
        }
      });
      var task = timer.Start();

      int lastPressKey = 0;
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
                await filesManager.CheckingForDownload(apiDataService, filesService, downloadService);
                filesManager.DownloadAll(false);
                break;
              }
            case 50:
              {
                Lines.DeleteFromLast(Lines.InfoLineNumber + 1);
                var downloadList = await downloadService.GetDownloadQueue();

                foreach (var download in downloadList)
                  Lines.WriteLine($"{downloadList.IndexOf(download) + 1}. {download.ApiData.Path + download.ApiData.Name} ({download.Url})");
                
                break;
              }
            case 51:
              {
                Lines.DeleteFromLast(Lines.InfoLineNumber + 1);
                await downloadService.DownloadAllAsync();
                break;
              }
            case 54:
              {
                if(lastPressKey != intKey)
                  Lines.DeleteFromLast(Lines.InfoLineNumber + 1);

                try
                {
                  using (var client = new WebClient())
                  {
                    var contents = client.DownloadString("https://test.criminalrussia.org/launcher_c_sharp/check.php?token=ad232fbxsdf43&class=createfile");
                    Lines.WriteLine($"Файл {contents} создан успешно.");
                  }
                }
                catch (Exception ex)
                {
                  Lines.ShowErrorInfo(ex.Message);
                }
                break;
              }
            case 55:
              {
                Lines.DeleteFromLast(Lines.InfoLineNumber + 1);
                try
                {
                  using (var client = new WebClient())
                  {
                    var contents = client.DownloadString("https://test.criminalrussia.org/launcher_c_sharp/check.php?token=ad232fbxsdf43&class=updatefile");
                    var content = contents.Split("<br />");
                    Lines.WriteLine($"Файлов обновивших информацию: {content.Length - 1}");
                  }
                }
                catch (Exception ex)
                {
                  Lines.ShowErrorInfo(ex.Message);
                }
                break;
              }
            case 56:
              {
                Lines.DeleteFromLast(Lines.InfoLineNumber + 1);
                try
                {
                  using (var client = new WebClient())
                  {
                    var contents = client.DownloadString("https://test.criminalrussia.org/launcher_c_sharp/check.php?token=ad232fbxsdf43&class=deletefile");
                    var content = contents.Split("<br />");
                    Lines.WriteLine($"Файлов удалено: {content.Length - 1}");
                  }
                }
                catch (Exception ex)
                {
                  Lines.ShowErrorInfo(ex.Message);
                }
                break;
              }
            case 57:
              {
                Lines.DeleteFromLast(Lines.InfoLineNumber + 1);
                try
                {
                  Lines.WriteLine($"Обновление API (Ожидайте).");
                  using (var client = new WebClient())
                  {
                    var contents = client.DownloadString("https://test.criminalrussia.org/launcher_c_sharp/check.php?token=ad232fbxsdf43&class=education");
                  }
                  Lines.WriteLine($"API обновлен.");
                }
                catch (Exception ex)
                {
                  Lines.ShowErrorInfo(ex.Message);
                }
                break;
              }
            case 48:
              {
                Lines.DeleteFromLast(Lines.InfoLineNumber + 1);
                try
                {
                  Lines.WriteLine($"Автоэмитация, ожидайте.");
                  using (var client = new WebClient())
                  {
                    Random rnd = new Random();
                    string contents;

                    if (rnd.Next(2) > 0)
                    {
                      contents = client.DownloadString("https://test.criminalrussia.org/launcher_c_sharp/check.php?token=ad232fbxsdf43&class=deletefile");
                      var content = contents.Split("<br />");
                      Lines.WriteLine($"Файлов удалено: {content.Length - 1}");
                    }

                    if (rnd.Next(2) > 0)
                    {
                      contents = client.DownloadString("https://test.criminalrussia.org/launcher_c_sharp/check.php?token=ad232fbxsdf43&class=updatefile");
                      var content = contents.Split("<br />");
                      Lines.WriteLine($"Файлов обновивших информацию: {content.Length - 1}");
                    }

                    for (int i = 0; i < rnd.Next(0, 60); i++)
                    {
                      contents = client.DownloadString("https://test.criminalrussia.org/launcher_c_sharp/check.php?token=ad232fbxsdf43&class=createfile");
                      Lines.WriteLine($"Файл {contents} создан успешно.");
                    }
                    contents = client.DownloadString("https://test.criminalrussia.org/launcher_c_sharp/check.php?token=ad232fbxsdf43&class=education");
                  }
                  Lines.WriteLine($"API обновлен.");
                }
                catch (Exception ex)
                {
                  Lines.ShowErrorInfo(ex.Message);
                }
                break;
              }
            default:
              {

                break;
              }
          }
          lastPressKey = intKey;
        }
      } while (KeyInfo.Key != ConsoleKey.Escape);

      timer.Stop();
      task.Dispose();
    }
  }
}
