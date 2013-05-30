using System;
using System.Linq;
using System.Threading;

namespace AsyncFramework
{
  public class Safe : IDisposable
  {
    private readonly object[] _padlocks;
    private readonly bool[] _securedFlags;

    private Safe(object padlock, int milliSecondTimeout)
    {
      _padlocks = new[] { padlock };
      _securedFlags = new[] { Monitor.TryEnter(padlock, milliSecondTimeout) };
    }

    private Safe(object[] padlocks, int milliSecondTimeout)
    {
      _padlocks = padlocks;
      _securedFlags = new bool[_padlocks.Length];
      using (var timeoutRemaining = new Countdown(milliSecondTimeout))
      {
        for (int i = 0; i < _padlocks.Length; i++)
          _securedFlags[i] = Monitor.TryEnter(padlocks[i], timeoutRemaining.RemainingMilliseconds);
      }
    }

    public bool Secured
    {
      get { return _securedFlags.All(s => s); }
    }

    public static void Lock(object[] padlocks, int millisecondTimeout, Action codeToRun)
    {
      using (var bolt = new Safe(padlocks, millisecondTimeout))
        if (bolt.Secured)
          codeToRun();
        else
          throw new TimeoutException(string.Format("Safe.Lock wasn't able to acquire a lock in {0}ms",
                                                   millisecondTimeout));
    }

    public static void Lock(object padlock, int millisecondTimeout, Action codeToRun)
    {
      using (var bolt = new Safe(padlock, millisecondTimeout))
        if (bolt.Secured)
          codeToRun();
        else
          throw new TimeoutException(string.Format("Safe.Lock wasn't able to acquire a lock in {0}ms",
                                                   millisecondTimeout));
    }

    #region Implementation of IDisposable

    public void Dispose()
    {
      for (int i = 0; i < _securedFlags.Length; i++)
        if (_securedFlags[i])
        {
          Monitor.Exit(_padlocks[i]);
          _securedFlags[i] = false;
        }
    }

    #endregion
  }

  class Countdown : IDisposable
  {
    private TimeSpan _remaining;
    private DateTime _initialTime;

    public Countdown(int timeoutMilliseconds)
    {
      _remaining = TimeSpan.FromMilliseconds(timeoutMilliseconds);
      _initialTime = DateTime.Now;
    }

    public int RemainingMilliseconds
    {
      get
      {
        var elapsed = DateTime.Now - _initialTime;
        if (elapsed > _remaining) return 0;
        return (int) Math.Truncate((_remaining - elapsed).TotalMilliseconds);
      }
    }

    public void Dispose()
    {
      
    }
  }
}
