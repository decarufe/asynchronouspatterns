using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AsyncFramework;

namespace EventbasedAsynchronousPattern
{
  class Program
  {
    static void Main(string[] args)
    {
      // Create an instance of the test class.
      var asyncDemo = new AsyncDemo();

      asyncDemo.TestMethodCompleted += AsyncDemoOnTestMethodCompleted;

      var taskId = asyncDemo.TestMethodAsync(3000);

      Thread.Sleep(10);
      Console.WriteLine("Main thread {0} does some work.",
                        Thread.CurrentThread.ManagedThreadId);

      Console.WriteLine("Press enter to quit.");
      asyncDemo.TestMethodAsyncCancel();
      Console.ReadLine();
    }

    private static void AsyncDemoOnTestMethodCompleted(object sender, AsyncDemo.TestMethodCompletedEventArgs args)
    {
      Console.WriteLine("The call executed with return value \"{0}\".", args.Result);
    }
  }
}
