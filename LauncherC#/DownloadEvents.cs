using System.Net;
using System.Threading.Tasks;

namespace LauncherC_
{
  /// <summary>
  /// События загрузки.
  /// </summary>
  public class DownloadEvents
  {
    /// <summary>
    /// Потокобезопасность.
    /// </summary>
    private static readonly object ConsoleLock = new object();

    /// <summary>
    /// Последние данные байтов.
    /// </summary>
    private long lastRecorded;

    /// <summary>
    /// Событие скачивания файла.
    /// </summary>
    public async Task ProgressCallbackAsync(object? sender, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
    {
      lock (ConsoleLock)
      {
        var rec = lastRecorded;
        NetworkSpeed.AddInfo(downloadProgressChangedEventArgs.BytesReceived - lastRecorded);
        lastRecorded = downloadProgressChangedEventArgs.BytesReceived;
        if (Lines.DownloadLineNumber > 0 && rec > 0 && NetworkSpeed.TotalSpeed > 0 && downloadProgressChangedEventArgs.ProgressPercentage > 0)
        { 
          Lines.WriteLine(Lines.DownloadLineNumber + 1, $"Прогресс: {downloadProgressChangedEventArgs.BytesReceived} / {downloadProgressChangedEventArgs.TotalBytesToReceive} ({downloadProgressChangedEventArgs.ProgressPercentage}%)");
          Lines.WriteLine(Lines.DownloadLineNumber + 2, $"Скорость: {Utils.HumanizeByteSize(NetworkSpeed.TotalSpeed)}");
        }
      }
    }
  }
}
