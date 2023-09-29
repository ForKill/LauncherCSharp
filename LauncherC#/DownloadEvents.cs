using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LauncherC_
{
  public class DownloadEvents
  {
    public async Task ProgressCallbackAsync(ApiData apiData, object? sender, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs, Stopwatch stopwatch)
    {
      Console.WriteLine($"Прогресс: {downloadProgressChangedEventArgs.BytesReceived} / {downloadProgressChangedEventArgs.TotalBytesToReceive} ({downloadProgressChangedEventArgs.ProgressPercentage}%)");
      Console.WriteLine($"Скорость: {Utils.GetDownloadSpeed(downloadProgressChangedEventArgs.BytesReceived, stopwatch.Elapsed.TotalSeconds)}");

      await Task.Yield();
    }

    public async Task ComplitedCallbackAsync(ApiData apiData, object? sender, AsyncCompletedEventArgs asyncCompletedEventArgs, Stopwatch stopwatch)
    {
      Console.WriteLine($"Загрузка завершена за {stopwatch.Elapsed.TotalSeconds}. ({apiData.Name})");

      if (asyncCompletedEventArgs.Cancelled)
        Console.WriteLine("Загрузка отменена.");

      if (asyncCompletedEventArgs.Error != null)
        Console.WriteLine($"Ошибка загрузки: {asyncCompletedEventArgs.Error.ToString()}");

      FilesService filesService = new FilesService();
      await filesService.Add(apiData);
      await Task.Yield();
    }
  }
}
