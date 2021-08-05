using System;
using System.Windows.Input;

namespace Joserras.Client.Torneo.Utils
{
	public class DelegateCommand<T> : ICommand
  {
    private readonly Action<object> ExecuteAction;
    private readonly Func<object, bool> CanExecuteFunc;

    public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecuteFunc)
    {
      ExecuteAction = executeAction;
      CanExecuteFunc = canExecuteFunc;
    }

    public bool CanExecute(object parameter)
    {
      return CanExecuteFunc == null || CanExecuteFunc( parameter );
    }

    public void Execute(object parameter)
    {
      ExecuteAction( parameter );
    }

    public event EventHandler CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
      CanExecuteChanged?.Invoke( this, EventArgs.Empty );
    }

  }


  public class DelegateCommand : ICommand
  {
    private readonly Action ExecuteAction;
    private readonly Func<object, bool> CanExecuteFunc;

    public DelegateCommand(Action executeAction, Func<object, bool> canExecuteFunc)
    {
      ExecuteAction = executeAction;
      CanExecuteFunc = canExecuteFunc;
    }

    public bool CanExecute(object parameter)
    {
      return CanExecuteFunc == null || CanExecuteFunc( parameter );
    }

    public void Execute(object parameter)
    {
      ExecuteAction();
    }

    public event EventHandler CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
      CanExecuteChanged?.Invoke( this, EventArgs.Empty );
    }

  }

}