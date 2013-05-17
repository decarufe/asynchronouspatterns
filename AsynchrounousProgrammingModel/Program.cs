using System;
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
      var asyncDemo = new AsyncDemo();

      var result = asyncDemo.BeginTestMethod(3000);

      Thread.Sleep(10);
      Console.WriteLine("Main thread {0} does some work.",
                        Thread.CurrentThread.ManagedThreadId);

      // TODO: Use WaitHandle
      // TODO: Wait on multiple async calls
      // TODO: Use polling to wait for completion
      // TODO: Wait for a short time (less than 2 sec)

      // Call EndInvoke to wait for the asynchronous call to complete, 
      // and to retrieve the results. 
      string returnValue = asyncDemo.EndTestMehod(result);

      Console.WriteLine("The call executed with return value \"{0}\".", returnValue);
    }
  }
}
