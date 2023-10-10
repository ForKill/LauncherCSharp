using System;
using System.Threading.Tasks;

namespace LauncherC_
{
  /// <summary>
  /// Работа внутри API.
  /// </summary>
  internal class ApiDataManager
  {
    /// <summary>
    /// Проверка обновления сборки.
    /// </summary>
    /// <param name="apiDataService">Сущность версии.</param>
    public async Task<bool> CheckUpdate(ApiDataService apiDataService)
    {
      try
      {
        ApiDataApp apiDataApp = apiDataService.GetVersion();
        ApiDataApp apiVersionActual = await apiDataService.GetActualVersionAPI();
        if (!apiDataApp.Equals(apiVersionActual))
        {
          await apiDataService.SetVersion(apiVersionActual, apiDataService, OnChangeVersion);
          return true;
        }
      }
      catch (Exception ex)
      {
        Lines.ShowErrorInfo(ex.Message);
      }
      return false;
    }

    /// <summary>
    /// Калбэк смены версии.
    /// </summary>
    /// <param name="newDataApp">Новые данные сущности версии.</param>
    /// <param name="apiDataService">Сущность файлов API.</param>
    private static void OnChangeVersion(ApiDataApp newDataApp, ApiDataService apiDataService)
    {
      if(Lines.VersionLineNumber != -1)
        Lines.WriteLineInfo(Lines.VersionLineNumber, $"Сборка: v{newDataApp.Version} | HASH:{newDataApp.Hash}");

      apiDataService.ClearData();
      Lines.ShowInfo($"Вышло обновление сборки.", ConsoleColor.DarkGray);
    }
  }
}
