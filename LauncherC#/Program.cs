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

      Console.WriteLine("Загрузка apiDataService.GetActualVersion()");
      var apiVersion = await apiDataService.GetActualVersion();
      Console.WriteLine($"{apiVersion.hash} | {apiVersion.version}");

      Console.WriteLine("Загрузка apiDataService.GetData()");
      Dictionary<string, ApiData> apiData = await apiDataService.GetData();

      foreach (var apidata in apiData)
      {
        Console.WriteLine($"{apidata.Key} | {apidata.Value.name} | {apidata.Value.hash}");
      }

      Console.WriteLine("Hello World!");
    }
  }
}
