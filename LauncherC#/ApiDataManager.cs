using System.Reflection;
using System;
using System.Threading.Tasks;

namespace LauncherC_
{
  internal class ApiDataManager
  {
    public async Task CheckUpdate(ApiDataService apiDataService)
    {
      ApiDataApp apiDataApp = apiDataService.GetVersion();
      ApiDataApp apiVersionActual = await apiDataService.GetActualVersionAPI();
      if (!apiDataApp.Equals(apiVersionActual))
        await apiDataService.SetVersion(apiVersionActual, apiDataService, OnChangeVersion);
    }

    private static void OnChangeVersion(ApiDataApp newDataApp, ApiDataService apiDataService)
    {
      if(Lines.VersionLineNumber != -1)
        Lines.WriteLineInfo(Lines.VersionLineNumber, $"Сборка: v{newDataApp.Version} | HASH:{newDataApp.Hash}");

      apiDataService.ClearData();
      Lines.ShowInfo($"Вышло обновление сборки.", ConsoleColor.DarkGray);
    }
  }
}
