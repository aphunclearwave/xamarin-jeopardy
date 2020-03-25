using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Jeopardy.Helpers
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }

    /// <summary>
    /// A command that handles asynchronous methods actions. 
    /// This class was created by reviewing the code from Xamarin.Forms for <see cref="Command"/>
    /// and making a new command to work for asynchrous methods. 
    /// 
    /// References:
    /// https://github.com/xamarin/Xamarin.Forms/blob/master/Xamarin.Forms.Core/Command.cs
    /// 
    /// </summary>
    public class AsyncCommand : IAsyncCommand
    {
        private readonly Func<object, bool> _canExecute;
        private readonly Func<object, Task> _execute;

        public event EventHandler CanExecuteChanged;

        public AsyncCommand(Func<object, Task> execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public AsyncCommand(Func<object, Task> execute, Func<object, bool> canExecute)
            : this(execute)
        {
            _execute = execute;
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        public AsyncCommand(Func<Task> execute)
            : this(o => execute())
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
        }

        public AsyncCommand(Func<Task> execute, Func<bool> canExecute)
            : this(o => execute(), o => canExecute())
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
            if (canExecute == null)
                throw new ArgumentNullException(nameof(canExecute));
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute(parameter);

            return true;
        }

        public async Task ExecuteAsync(object parameter)
        {
            try
            {

                await _execute(parameter).ConfigureAwait(false);
            }
            finally
            {
                ChangeCanExecute();
            }
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter).ConfigureAwait(false);
        }

        public void ChangeCanExecute()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class AsyncCommand<T> : AsyncCommand
    {

        public AsyncCommand(Func<T, Task> execute) : base(async (o) => {
            if (IsValidParameter(o))
            {
                await execute((T)o);
            }
        })
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
        }

        public AsyncCommand(Func<T, Task> execute, Func<T, bool> canExecute) : base(async (o) => {
            if (IsValidParameter(o))
            {
                await execute((T)o);
            }
        }, (o) => IsValidParameter(o) && canExecute((T)o))
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            if (canExecute == null)
                throw new ArgumentNullException(nameof(canExecute));
        }

        static bool IsValidParameter(object o)
        {
            if (o != null)
            {
                // The parameter isn't null, so we don't have to worry whether null is a valid option
                return o is T;
            }

            var t = typeof(T);

            // The parameter is null. Is T Nullable?
            if (Nullable.GetUnderlyingType(t) != null)
            {
                return true;
            }

            // Not a Nullable, if it's a value type then null is not valid
            return !t.GetTypeInfo().IsValueType;
        }
    }
}

