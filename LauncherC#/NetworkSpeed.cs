using System.Collections.Concurrent;
using System;
using System.Threading;
using System.Linq;

public class NetworkSpeed
{
  public static double TotalSpeed
  {
    get { return totalSpeed; }
  }

  private static double totalSpeed = 0;

  private const uint Seconds = 3;

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

  public static void Clear()
  {
    while (ReceivedStorage.TryDequeue(out _)){}
    while (LastSpeeds.TryDequeue(out _)){}
    totalSpeed = 0;
  }

  public static void AddInfo(long received) => ReceivedStorage.Enqueue(received);

  public static event Action<double> Updated;

  private class LimitedConcurrentQueue<T> : ConcurrentQueue<T>
  {
    public uint Limit { get; }

    public new void Enqueue(T item)
    {
      while (Count >= Limit)
        TryDequeue(out _);
      base.Enqueue(item);
    }

    public LimitedConcurrentQueue(uint limit) => Limit = limit;

  }

  private static void OnUpdated(double obj) => Updated?.Invoke(obj);
}