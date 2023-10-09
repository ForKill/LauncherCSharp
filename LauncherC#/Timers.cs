using System;
using System.Threading;
using System.Threading.Tasks;

namespace LauncherC_
{
  /// <summary>
  /// Таймер.
  /// </summary>
  internal class Timers
  {
    #region Поля и свойства

    /// <summary>
    /// Событие таймера.
    /// </summary>
    private Action callback;

    /// <summary>
    /// Интервал.
    /// </summary>
    private int Interval;

    /// <summary>
    /// Сущность таймера.
    /// </summary>
    private Timer timer;

    private CancellationTokenSource cancellationTokenSource;

    #endregion

    #region Методы

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="callback">Обработчик события.</param>
    /// <param name="interval">Интервал таймера.</param>
    public Timers(Action callback, int interval = 5000)
    {
      this.callback = callback;
      Interval = interval;
      cancellationTokenSource = new CancellationTokenSource();
    }

    /// <summary>
    /// Запуск таймера.
    /// </summary>
    /// <returns></returns>
    public async Task Start()
    {
      timer = new Timer(async state => await Callback(), null, 0, Interval);
      await Task.Delay(Timeout.Infinite, cancellationTokenSource.Token);
    }

    /// <summary>
    /// Остановка таймера.
    /// </summary>
    public void Stop()
    {
      cancellationTokenSource.Cancel();
      timer?.Dispose();
    }

    /// <summary>
    /// Событие.
    /// </summary>
    /// <returns></returns>
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

    #endregion
  }
}
