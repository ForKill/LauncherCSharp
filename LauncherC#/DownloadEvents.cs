using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace LauncherC_
{
  public class DownloadEvents
  {
    private static readonly object ConsoleLock = new object();
    private long lastRecorded;

    public async Task ProgressCallbackAsync(object? sender, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
    {
      lock (ConsoleLock)
      {
        NetworkSpeed.AddInfo(downloadProgressChangedEventArgs.BytesReceived - lastRecorded);
        lastRecorded = downloadProgressChangedEventArgs.BytesReceived;
        if (Lines.DownloadLineNumber > 0)
        { 
          Lines.WriteLine(Lines.DownloadLineNumber + 1, $"Прогресс: {downloadProgressChangedEventArgs.BytesReceived} / {downloadProgressChangedEventArgs.TotalBytesToReceive} ({downloadProgressChangedEventArgs.ProgressPercentage}%)");
          Lines.WriteLine(Lines.DownloadLineNumber + 2, $"Скорость: {Utils.HumanizeByteSize(NetworkSpeed.TotalSpeed)}");
        }
      }
    }
  }
}
