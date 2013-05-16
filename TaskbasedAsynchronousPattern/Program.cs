using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AsyncFramework;

namespace TaskbasedAsynchronousPattern
{
  class Program
  {
    static void Main(string[] args)
    {
      // Create an instance of the test class.
      var asyncDemo = new AsyncDemo();

      var task = asyncDemo.TestMethodTask(3000);

      Thread.Sleep(10);
      Console.WriteLine("Main thread {0} does some work.",
                        Thread.CurrentThread.ManagedThreadId);

      task.Wait(); 
      Console.WriteLine("The call executed with return value \"{0}\".", task.Result);
    }
  }
}
