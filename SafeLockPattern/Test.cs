using System;
using System.Text;
using System.Threading;
using AsyncFramework;

namespace SafeLockPattern
{
  class Test : AsyncBase
  {
    private readonly int _loop;
    private readonly Account _account1;
    private readonly Account _account2;

    public Test(int loop, Account account1, Account account2)
    {
      _loop = loop;
      _account1 = account1;
      _account2 = account2;
    }

    private void DoIt(int loop, Account account1, Account account2)
    {
      //Console.Write(String.Format("== {0} loop {1}", Thread.CurrentThread.ManagedThreadId, loop));
      //Console.Write(" ");
      //Console.Write(Display(account1, account2));
      Account.Transfer(50, account1, account2);
      Console.Write(".");
      //Console.Write(" ");
      //Console.Write(Display(account1, account2));
      //Console.WriteLine();
    }

    private static string Display(params Account[] accounts)
    {
      foreach (var account in accounts)
      {
        Console.Write(String.Format(" Account: {0}, {1}", account.AccountNumber, account.Balance));
      }
      return string.Empty;
    }

    protected override void Execute()
    {
      DoIt(_loop, _account1, _account2);
    }
  }
}