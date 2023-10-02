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
    private static readonly object ConsoleLock = new object();
    public async Task ProgressCallbackAsync(ApiData apiData, object? sender, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs, Stopwatch stopwatch)
    {
      lock (ConsoleLock)
      {
        Lines.WriteLine(Lines.InfoLineNumber + 1, $"Прогресс: {downloadProgressChangedEventArgs.BytesReceived} / {downloadProgressChangedEventArgs.TotalBytesToReceive} ({downloadProgressChangedEventArgs.ProgressPercentage}%)");
        Lines.Write($"Скорость: {Utils.GetDownloadSpeed(downloadProgressChangedEventArgs.BytesReceived, stopwatch.Elapsed.TotalSeconds)}");
      }
    }

    public async Task ComplitedCallbackAsync(ApiData apiData, object? sender, AsyncCompletedEventArgs asyncCompletedEventArgs, Stopwatch stopwatch)
    {
      Lines.DeleteFromLast(Lines.InfoLineNumber + 1);
      Lines.WriteLine($"Загрузка завершена за {stopwatch.Elapsed.TotalSeconds}. ({apiData.Name})");

      if (asyncCompletedEventArgs.Cancelled)
        Lines.WriteLine("Загрузка отменена.");

      if (asyncCompletedEventArgs.Error != null)
        Lines.WriteLine($"Ошибка загрузки: {asyncCompletedEventArgs.Error.ToString()}");

      FilesService filesService = new FilesService();
      await filesService.Add(apiData);
      await Task.Yield();
    }
  }
}
