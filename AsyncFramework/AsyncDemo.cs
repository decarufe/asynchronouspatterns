using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncFramework
{
  public class AsyncDemo
  {
    public AsyncDemo()
    {
      // For Event-based Asynchronous Pattern
      InitialiseDelegates();
    }

   // The method to be executed asynchronously. 
    public string TestMethod(int callDuration)
    {
      Console.WriteLine("Test method begins on thread {0} waiting for {1}sec.", Thread.CurrentThread.ManagedThreadId,
                        callDuration);
      Thread.Sleep(callDuration);
      Console.WriteLine("Test method ends on thread {0} waited for {1}sec.", Thread.CurrentThread.ManagedThreadId,
                        callDuration);
      return String.Format("My call time was {0}.", callDuration.ToString(CultureInfo.InvariantCulture));
    }

    #region Asynchronous Programming Model

    private AsyncMethodCaller _caller;

    private delegate string AsyncMethodCaller(int callDuration);

    public IAsyncResult BeginTestMethod(int callDuration)
    {
      // Create the delegate.
      _caller = TestMethod;

      // Initiate the asychronous call.
      return _caller.BeginInvoke(3000, null, null);
    }

    public string EndTestMehod(IAsyncResult result)
    {
      return _caller.EndInvoke(result);
    }

    #endregion

    #region Event-based Asynchronous Pattern

    private void InitialiseDelegates()
    {
      _onCompletedDelegate = TestMethodComplete;
    }

    private readonly HybridDictionary _userStateToLifetime = new HybridDictionary();
    private SendOrPostCallback _onCompletedDelegate;

    public event EventHandler<TestMethodCompletedEventArgs> TestMethodCompleted;

    protected virtual void OnTestMethodCompleted(TestMethodCompletedEventArgs e)
    {
      var handler = TestMethodCompleted;
      if (handler != null) handler(this, e);
    }

    private delegate void WorkerEventHandler(int callDuration, AsyncOperation asyncOp);

    public Guid TestMethodAsync(int callDuration)
    {
      // Create a ne taskId
      var taskId = Guid.NewGuid();


      // Create an AsyncOperation for taskId.
      AsyncOperation asyncOp = AsyncOperationManager.CreateOperation(taskId);

      // Multiple threads will access the task dictionary,
      // so it must be locked to serialize access.
      lock (_userStateToLifetime.SyncRoot)
      {
        if (_userStateToLifetime.Contains(taskId))
        {
          throw new InvalidOperationException("Task ID parameter must be unique");
        }

        _userStateToLifetime[taskId] = asyncOp;
      }

      // Start the asynchronous operation.
      var workerDelegate = new WorkerEventHandler(TestMethodWorker);
      workerDelegate.BeginInvoke(
        callDuration,
        asyncOp,
        null,
        null);

      return taskId;
    }

    private void TestMethodWorker(int callduration, AsyncOperation asyncOp)
    {
      Exception e = null;

      // Check that the task is still active.
      // The operation may have been canceled before
      // the thread was scheduled.
      string result = String.Empty;
      if (!TaskCanceled(asyncOp.UserSuppliedState))
      {
        try
        {
          result = TestMethod(callduration);
        }
        catch (Exception ex)
        {
          e = ex;
        }
      }

      CompletionMethod(
        callduration,
        result,
        e,
        TaskCanceled(asyncOp.UserSuppliedState),
        asyncOp);
    }

    // Utility method for determining if a 
    // task has been canceled.
    private bool TaskCanceled(object taskId)
    {
      return (_userStateToLifetime[taskId] == null);
    }

    private void CompletionMethod(
      int callDuration,
      string result,
      Exception exception,
      bool canceled,
      AsyncOperation asyncOp)
    {
      // If the task was not previously canceled,
      // remove the task from the lifetime collection.
      if (!canceled)
      {
        lock (_userStateToLifetime.SyncRoot)
        {
          _userStateToLifetime.Remove(asyncOp.UserSuppliedState);
        }
      }

      // Package the results of the operation in a 
      // CalculatePrimeCompletedEventArgs.
      var e = new TestMethodCompletedEventArgs(
        callDuration,
        result,
        exception,
        canceled,
        asyncOp.UserSuppliedState);

      // End the task. The asyncOp object is responsible 
      // for marshaling the call.
      asyncOp.PostOperationCompleted(_onCompletedDelegate, e);

      // Note that after the call to OperationCompleted, 
      // asyncOp is no longer usable, and any attempt to use it
      // will cause an exception to be thrown.
    }

    private void TestMethodComplete(object operationState)
    {
      var e = operationState as TestMethodCompletedEventArgs;

      OnTestMethodCompleted(e);
    }

    public class TestMethodCompletedEventArgs : AsyncCompletedEventArgs
    {
      private readonly int _callDuration;
      private string _result;

      public TestMethodCompletedEventArgs(
        int numberToTest,
        string result,
        Exception e,
        bool canceled,
        object state)
        : base(e, canceled, state)
      {
        _callDuration = numberToTest;
        _result = result;
      }

      public int CallDuration
      {
        get
        {
          // Raise an exception if the operation failed or was canceled.
          RaiseExceptionIfNecessary();

          // If the operation was successful, return the property value.
          return _callDuration;
        }
      }

      public string Result
      {
        get
        {
          // Raise an exception if the operation failed or was canceled.
          RaiseExceptionIfNecessary();

          // If the operation was successful, return the property value.
          return _result;
        }
      }
    }

    #endregion

    #region Task-based Asynchronous Pattern

    public Task<string> TestMethodTask(int callDuration)
    {
      var tcs = new TaskCompletionSource<string>();
      var caller = new AsyncMethodCaller(TestMethod);
      caller.BeginInvoke(callDuration, ar =>
      {
        try { tcs.SetResult(caller.EndInvoke(ar)); }
        catch (Exception exc) { tcs.SetException(exc); }
      }, null);
      return tcs.Task;
    }

    #endregion
  }
}