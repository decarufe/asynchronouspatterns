using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadSynchronization
{
  class Program
  {
    static void Main(string[] args)
    {
      var eventWaitHandle = new AutoResetEvent(false);

      var task = Task.Factory.StartNew(() => DoSometing(1, eventWaitHandle));
      Thread.Sleep(5);
      task.ContinueWith(t =>
      {
        Task.Factory.StartNew(() => DoSometing(2, eventWaitHandle));
      }, TaskContinuationOptions.NotOnFaulted);
      task.ContinueWith(t => Console.WriteLine(t.Exception.InnerExceptions.First().Message), TaskContinuationOptions.OnlyOnFaulted);

      Thread.Sleep(5);
      Console.WriteLine("Waiting for enter");
      Console.ReadLine();
      eventWaitHandle.Set();
      Console.ReadLine();
      eventWaitHandle.Set();
    }

    static void DoSometing(int id, EventWaitHandle eventWaitHandle)
    {
      Console.WriteLine("Starting {0} on thread {1}", id, Thread.CurrentThread.ManagedThreadId);
      while (!eventWaitHandle.WaitOne())
      {
        Console.WriteLine("Waiting {0} on thread {1}", id, Thread.CurrentThread.ManagedThreadId);
      }
      Console.WriteLine("Finishing {0} on thread {1}", id, Thread.CurrentThread.ManagedThreadId);
    }
  }
}
