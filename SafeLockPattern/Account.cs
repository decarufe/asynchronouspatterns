using System;
using System.Threading;
using AsyncFramework;

namespace SafeLockPattern
{
  public class Account
  {
    private readonly string _accountNumber;
    private double _balance;

    private Account(string accountNumber, double amount)
    {
      _accountNumber = accountNumber;
      Balance = amount;
    }

    public string AccountNumber
    {
      get { return _accountNumber; }
    }

    public double Balance
    {
      get { return _balance; }
      set { _balance = value; }
    }

    public static Account OpenNew(string accountNumber, double amount)
    {
      return new Account(accountNumber, amount);
    }

    public void Deposit(double amount)
    {
      Interlocked.Exchange(ref _balance, _balance + amount);
      //Thread.Sleep(1);
    }

    public void Withdraw(double amount)
    {
      Interlocked.Exchange(ref _balance, _balance - amount);
      //Thread.Sleep(2);
    }

    public static void Transfer(double amount, Account fromAccount, Account toAccount)
    {
      var retries = 10;

      while (Interlocked.Decrement(ref retries) > 0)
      {
        try
        {
          Safe.Lock(new[] { fromAccount, toAccount }, 10, () =>
          {
            fromAccount.Withdraw(amount);
            toAccount.Deposit(amount);
          });
          break;
        }
        catch (TimeoutException e)
        {
          if (retries == 0)
            throw;
          Console.Write(retries);
          Thread.Sleep(10);
        }
      }
    }

    public override string ToString()
    {
      return string.Format("{0} (Balance = {1})", _accountNumber, _balance);
    }
  }
}