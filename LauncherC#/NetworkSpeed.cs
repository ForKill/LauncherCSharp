using System.Collections.Concurrent;
using System;
using System.Threading;
using System.Linq;

/// <summary>
/// Класс для вычисления скорости сети на основе полученных данных.
/// </summary>
public class NetworkSpeed
{
  #region Поля и свойства

  /// <summary>
  /// Возвращает общую скорость сети в байтах в секунду.
  /// </summary>
  private static double totalSpeed = 0;
  public static double TotalSpeed
  {
    get { return totalSpeed; }
  }

  /// <summary>
  /// Количество секунд для расчета средней скорости.
  /// </summary>
  private const uint Seconds = 3;

  /// <summary>
  /// Интервал в миллисекундах для обновления скорости.
  /// </summary>
  private const uint TimerInterval = 1000;

  private static Timer speedTimer = new Timer(state =>
  {
    var now = 0L;
    while (ReceivedStorage.TryDequeue(out var added))
      now += added;
    LastSpeeds.Enqueue(now);
    totalSpeed = LastSpeeds.Average();
    OnUpdated(totalSpeed);
  }, null, 0, TimerInterval);

  private static readonly LimitedConcurrentQueue<double> LastSpeeds = new LimitedConcurrentQueue<double>(Seconds);

  private static readonly ConcurrentQueue<long> ReceivedStorage = new ConcurrentQueue<long>();

  #endregion

  #region Методы

  /// <summary>
  /// Очищает полученные данные и сбрасывает общую скорость.
  /// </summary>
  public static void Clear()
  {
    while (ReceivedStorage.TryDequeue(out _)){}
    while (LastSpeeds.TryDequeue(out _)){}
    totalSpeed = 0;
  }

  /// <summary>
  /// Добавляет полученные данные к расчету.
  /// </summary>
  /// <param name="received">Объем полученных данных в байтах.</param>
  public static void AddInfo(long received)
  {
    ReceivedStorage.Enqueue(received);
  }

  /// <summary>
  /// Событие срабатывает при обновлении общей скорости.
  /// </summary>
  public static event Action<double> Updated;

  private class LimitedConcurrentQueue<T> : ConcurrentQueue<T>
  {
    /// <summary>
    /// Получает лимит очереди.
    /// </summary>
    public uint Limit { get; }

    /// <summary>
    /// Добавляет элемент в очередь и гарантирует, что очередь не превысит лимит.
    /// </summary>
    /// <param name="item">Элемент, который нужно добавить в очередь.</param>
    public new void Enqueue(T item)
    {
      while (Count >= Limit)
        TryDequeue(out _);
      base.Enqueue(item);
    }
    /// <summary>
    /// Инициализирует новый экземпляр класса LimitedConcurrentQueue с указанным ограничением.
    /// </summary>
    /// <param name="limit">Максимальное количество элементов, которые может содержать очередь.</param>
    public LimitedConcurrentQueue(uint limit)
    {
      Limit = limit;
    }
  }

  private static void OnUpdated(double obj)
  {
    Updated?.Invoke(obj);
  }

  #endregion
}