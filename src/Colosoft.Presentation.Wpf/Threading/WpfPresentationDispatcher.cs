using System;
using System.Threading;
using System.Threading.Tasks;

namespace Colosoft.Presentation.Threading
{
    public class WpfPresentationDispatcher : Colosoft.Threading.IDispatcher
    {
        private readonly System.Windows.Threading.Dispatcher innerDispatcher;

        public WpfPresentationDispatcher()
        {
            this.innerDispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
        }

        public WpfPresentationDispatcher(System.Windows.Threading.Dispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            this.innerDispatcher = dispatcher;
        }

        public bool CheckAccess()
        {
            return this.innerDispatcher.CheckAccess() || System.Threading.Thread.CurrentThread.ManagedThreadId == 1;
        }

        public void VerifyAccess()
        {
            this.innerDispatcher.VerifyAccess();
        }

        public object Invoke(Delegate method, params object[] args)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (this.CheckAccess() || !this.Thread.IsAlive)
            {
                return method.DynamicInvoke(args);
            }
            else
            {
                return this.innerDispatcher.Invoke(method, args);
            }
        }

        public object Invoke(Delegate method, Colosoft.Threading.DispatcherPriority priority, object[] args)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var wpfPriority = (System.Windows.Threading.DispatcherPriority)Enum
                .Parse(typeof(System.Windows.Threading.DispatcherPriority), priority.ToString());

            if (this.CheckAccess() || !this.Thread.IsAlive)
            {
                return method.DynamicInvoke(args);
            }
            else
            {
                return this.innerDispatcher.Invoke(method, wpfPriority, args);
            }
        }

        public void Invoke(Action action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (this.CheckAccess() || !this.Thread.IsAlive)
            {
                action();
            }
            else
            {
                this.innerDispatcher.Invoke(action);
            }
        }

        public T Invoke<T>(Func<T> func)
        {
            if (func is null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            if (this.CheckAccess() || !this.Thread.IsAlive)
            {
                return func();
            }
            else
            {
                return this.innerDispatcher.Invoke(func);
            }
        }

        public System.Threading.Thread Thread => this.innerDispatcher.Thread;

        public Colosoft.Threading.IDispatcherOperation BeginInvoke(Delegate method, params object[] args)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            return new WpfPresentationDispatcherOperation(this, this.innerDispatcher.BeginInvoke(method, args));
        }

        public Colosoft.Threading.IDispatcherOperation BeginInvoke(Action method)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            return new WpfPresentationDispatcherOperation(this, this.innerDispatcher.BeginInvoke(method));
        }

        public Colosoft.Threading.IDispatcherOperation BeginInvoke(Colosoft.Threading.DispatcherPriority priority, Delegate method)
        {
            var wpfPriority = (System.Windows.Threading.DispatcherPriority)Enum
                .Parse(typeof(System.Windows.Threading.DispatcherPriority), priority.ToString());

            return new WpfPresentationDispatcherOperation(this, this.innerDispatcher.BeginInvoke(wpfPriority, method));
        }

        public Colosoft.Threading.IDispatcherOperation BeginInvoke(Delegate method, Colosoft.Threading.DispatcherPriority priority, params object[] args)
        {
            var wpfPriority = (System.Windows.Threading.DispatcherPriority)Enum
                .Parse(typeof(System.Windows.Threading.DispatcherPriority), priority.ToString());

            return new WpfPresentationDispatcherOperation(this, this.innerDispatcher.BeginInvoke(method, wpfPriority, args));
        }

        public Colosoft.Threading.IDispatcherOperation BeginInvoke(Colosoft.Threading.DispatcherPriority priority, Delegate method, object arg)
        {
            var wpfPriority = (System.Windows.Threading.DispatcherPriority)Enum
                .Parse(typeof(System.Windows.Threading.DispatcherPriority), priority.ToString());

            return new WpfPresentationDispatcherOperation(this, this.innerDispatcher.BeginInvoke(wpfPriority, method, arg));
        }

        public Colosoft.Threading.IDispatcherOperation BeginInvoke(Colosoft.Threading.DispatcherPriority priority, Delegate method, object arg, params object[] args)
        {
            var wpfPriority = (System.Windows.Threading.DispatcherPriority)Enum
                .Parse(typeof(System.Windows.Threading.DispatcherPriority), priority.ToString());

            return new WpfPresentationDispatcherOperation(this, this.innerDispatcher.BeginInvoke(wpfPriority, method, arg, args));
        }

        public void DoEvents()
        {
            DispatcherHelper.DoEvents();
        }

        public async Task<T> Invoke<T>(Func<CancellationToken, Task<T>> method, Colosoft.Threading.DispatcherPriority priority, CancellationToken cancellationToken)
        {
            var wpfPriority = (System.Windows.Threading.DispatcherPriority)Enum
                .Parse(typeof(System.Windows.Threading.DispatcherPriority), priority.ToString());

            var operation = this.innerDispatcher.InvokeAsync(
                () => method(cancellationToken),
                wpfPriority,
                cancellationToken);

            await operation.Task;
            return await operation.Result;
        }

        public async Task Invoke(Func<CancellationToken, Task> method, Colosoft.Threading.DispatcherPriority priority, CancellationToken cancellationToken)
        {
            var wpfPriority = (System.Windows.Threading.DispatcherPriority)Enum
                .Parse(typeof(System.Windows.Threading.DispatcherPriority), priority.ToString());

            var operation = this.innerDispatcher.InvokeAsync(
                async () => await method(cancellationToken),
                wpfPriority,
                cancellationToken);

            await operation.Task;
        }

        public Task<T> Invoke<T>(Func<CancellationToken, Task<T>> method, CancellationToken cancellationToken) =>
            this.Invoke<T>(method, Colosoft.Threading.DispatcherPriority.Normal, cancellationToken);

        public Task Invoke(Func<CancellationToken, Task> method, CancellationToken cancellationToken) =>
            this.Invoke(method, Colosoft.Threading.DispatcherPriority.Normal, cancellationToken);
    }
}
