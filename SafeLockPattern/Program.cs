using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SafeLockPattern
{
  class Program
  {
    static void Main(string[] args)
    {
      var account1 = Account.OpenNew("1", 10000);
      var account2 = Account.OpenNew("2", 10000);

      List<IAsyncResult> runners = new List<IAsyncResult>();

      IAsyncResult lastRunner = null;

      for (int j = 0; j < 10000; j++)
      {
        for (int i = 0; i < 60; i++)
        {
          int loop = i;

          var test = new Test(loop, account1, account2);
          lastRunner = test.Start();
        }
        lastRunner.AsyncWaitHandle.WaitOne();
      }

    }
  }
}
