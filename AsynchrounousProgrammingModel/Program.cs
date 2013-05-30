using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using AsyncFramework;

namespace AsynchrounousProgrammingModel
{
  static class Program
  {
    public static void Main()
    {
      // Create an instance of the test class.
      var asyncDemo1 = new AsyncDemo();
      var asyncDemo2 = new AsyncDemo();

      var stopwatch = Stopwatch.StartNew();

      var result1 = asyncDemo1.BeginTestMethod(3000, Callback, asyncDemo1);
      var result2 = asyncDemo2.BeginTestMethod(1000, Callback, asyncDemo2);

      Thread.Sleep(10);
      Console.WriteLine("Main thread {0} does some work.",
                        Thread.CurrentThread.ManagedThreadId);

      while (!result1.IsCompleted || !result2.IsCompleted)
      {
        Console.Write(".");
        Thread.Sleep(100);
      }

      Console.WriteLine("Duration {0}", stopwatch.ElapsedMilliseconds);

      // TODO: Use WaitHandle
      // TODO: Wait on multiple async calls
      // TODO: Use polling to wait for completion
      // TODO: Wait for a short time (less than 2 sec)

    }

    private static void Callback(IAsyncResult ar)
    {
      var asyncDemo = (AsyncDemo)ar.AsyncState;
      string returnValue1 = asyncDemo.EndTestMehod(ar);
      Console.WriteLine("The call executed with return value \"{0}\".", returnValue1);
    }
  }
}
