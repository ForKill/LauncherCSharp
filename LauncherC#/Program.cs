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
      Lines.ErrorInfoLineNumber = Lines.WriteLine(new String(' ', Config.ConsoleWidth));
      Lines.InfoLineNumber = Lines.WriteLine(string.Empty);
    
      Lines.SetDefaultColor();

      var timer = new Timers(async () =>
      {
        await apiDataManager.CheckUpdate(apiDataService);
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

                using (var client = new WebClient())
                {
                  var contents = client.DownloadString("https://test.criminalrussia.org/launcher_c_sharp/check.php?token=ad232fbxsdf43&class=createfile");
                  Lines.WriteLine($"Файл {contents} создан успешно.");
                }
                break;
              }
            case 55:
              {
                Lines.DeleteFromLast(Lines.InfoLineNumber + 1);
                using (var client = new WebClient())
                {
                  var contents = client.DownloadString("https://test.criminalrussia.org/launcher_c_sharp/check.php?token=ad232fbxsdf43&class=updatefile");
                  var content = contents.Split("<br />");
                  Lines.WriteLine($"Файлов обновивших информацию: {content.Length - 1}");
                }
                break;
              }
            case 56:
              {
                Lines.DeleteFromLast(Lines.InfoLineNumber + 1);
                using (var client = new WebClient())
                {
                  var contents = client.DownloadString("https://test.criminalrussia.org/launcher_c_sharp/check.php?token=ad232fbxsdf43&class=deletefile");
                  var content = contents.Split("<br />");
                  Lines.WriteLine($"Файло удалено: {content.Length - 1}");
                }
                break;
              }
            case 57:
              {
                Lines.DeleteFromLast(Lines.InfoLineNumber + 1);
                Lines.WriteLine($"Обновление API (Ожидайте).");
                using (var client = new WebClient())
                {
                  var contents = client.DownloadString("https://test.criminalrussia.org/launcher_c_sharp/check.php?token=ad232fbxsdf43&class=education");
                }
                Lines.WriteLine($"API обновлен.");
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
      task.Wait();
    }
  }
}
