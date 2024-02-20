using Colosoft.Threading;
using System;

namespace Colosoft.Presentation.Threading
{
    public class WpfPresentationDispatcherOperation : IDispatcherOperation
    {
        private readonly System.Windows.Threading.DispatcherOperation innerOperation;
        private readonly IDispatcher innerDispatcher;

        public event EventHandler Aborted
        {
            add
            {
                this.innerOperation.Aborted += value;
            }

            remove
            {
                this.innerOperation.Aborted -= value;
            }
        }

        public event EventHandler Completed
        {
            add
            {
                this.innerOperation.Completed += value;
            }

            remove
            {
                this.innerOperation.Completed -= value;
            }
        }

        public IDispatcher Dispatcher
        {
            get { return this.innerDispatcher; }
        }

        public System.Threading.Tasks.Task Task => innerOperation.Task;

        public DispatcherPriority Priority
        {
            get
            {
                return (DispatcherPriority)Enum.Parse(typeof(DispatcherPriority), this.innerOperation.Priority.ToString());
            }
            set
            {
                this.innerOperation.Priority = (System.Windows.Threading.DispatcherPriority)Enum
                    .Parse(typeof(System.Windows.Threading.DispatcherPriority), value.ToString());
            }
        }

        public object Result => this.innerOperation.Result;

        public DispatcherOperationStatus Status
        {
            get
            {
                switch (this.innerOperation.Status)
                {
                    case System.Windows.Threading.DispatcherOperationStatus.Aborted: return DispatcherOperationStatus.Aborted;
                    case System.Windows.Threading.DispatcherOperationStatus.Completed: return DispatcherOperationStatus.Completed;
                    case System.Windows.Threading.DispatcherOperationStatus.Executing: return DispatcherOperationStatus.Executing;
                    case System.Windows.Threading.DispatcherOperationStatus.Pending: return DispatcherOperationStatus.Pending;
                    default: return DispatcherOperationStatus.Pending;
                }
            }
        }

        public WpfPresentationDispatcherOperation(
            IDispatcher dispatcher,
            System.Windows.Threading.DispatcherOperation operation)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }
            else if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            this.innerDispatcher = dispatcher;
            this.innerOperation = operation;
        }

        public bool Abort()
        {
            return this.innerOperation.Abort();
        }

        public DispatcherOperationStatus Wait()
        {
            switch (this.innerOperation.Wait())
            {
                case System.Windows.Threading.DispatcherOperationStatus.Aborted: return DispatcherOperationStatus.Aborted;
                case System.Windows.Threading.DispatcherOperationStatus.Completed: return DispatcherOperationStatus.Completed;
                case System.Windows.Threading.DispatcherOperationStatus.Executing: return DispatcherOperationStatus.Executing;
                case System.Windows.Threading.DispatcherOperationStatus.Pending: return DispatcherOperationStatus.Pending;
                default: return DispatcherOperationStatus.Pending;
            }
        }

        public DispatcherOperationStatus Wait(TimeSpan timeout)
        {
            switch (this.innerOperation.Wait(timeout))
            {
                case System.Windows.Threading.DispatcherOperationStatus.Aborted: return DispatcherOperationStatus.Aborted;
                case System.Windows.Threading.DispatcherOperationStatus.Completed: return DispatcherOperationStatus.Completed;
                case System.Windows.Threading.DispatcherOperationStatus.Executing: return DispatcherOperationStatus.Executing;
                case System.Windows.Threading.DispatcherOperationStatus.Pending: return DispatcherOperationStatus.Pending;
                default: return DispatcherOperationStatus.Pending;
            }
        }
    }
}
