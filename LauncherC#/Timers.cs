using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LauncherC_
{
  internal class Timers
  {
    private Action callback;
    private int interval;
    private Timer timer;
    private CancellationTokenSource cancellationTokenSource;

    public Timers(Action callback, int interval = 5000)
    {
      this.callback = callback;
      this.interval = interval;
      cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task Start()
    {
      timer = new Timer(async state => await Callback(), null, 0, interval);
      await Task.Delay(Timeout.Infinite, cancellationTokenSource.Token);
    }

    public void Stop()
    {
      cancellationTokenSource.Cancel();
      timer?.Dispose();
    }

    private async Task Callback()
    {
      try
      {
        await Task.Run(() => callback.Invoke());
      }
      catch (Exception ex)
      {
        Lines.ShowErrorInfo(ex.Message);
      }
    }
  }
}
