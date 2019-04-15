namespace Squalr.Engine.Common
{
    using Squalr.Engine.Common.Extensions;
    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a task on which the progress can be tracked.
    /// </summary>
    public abstract class TrackableTask : INotifyPropertyChanged
    {
        /// <summary>
        /// A universal identifier that can be used to enforce uniqueness for a suite of tasks.
        /// </summary>
        public static readonly String UniversalIdentifier = Guid.NewGuid().ToString();

        public delegate void OnTaskCanceled(TrackableTask task);

        public delegate void OnTaskCompleted(TrackableTask task);

        public delegate void OnProgressUpdate(Single progress);

        public delegate void UpdateProgress(Single progress);

        public event OnTaskCanceled OnCanceledEvent;

        public event OnTaskCompleted OnCompletedEvent;

        public event OnProgressUpdate OnProgressUpdatedEvent;

        private Single progress;

        private String name;

        private String taskIdentifier;

        private Boolean isCanceled;

        private Boolean isCompleted;

        public TrackableTask(String name)
        {
            this.Name = name;

            this.AccessLock = new Object();
            this.CancellationTokenSource = new CancellationTokenSource();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected static ConcurrentDictionary<String, TrackableTask> uniqueTaskPool = new ConcurrentDictionary<String, TrackableTask>();

        /// <summary>
        /// Indicates that a given property in this project item has changed.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        protected void RaisePropertyChanged(String propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Single Progress
        {
            get
            {
                return this.progress;
            }

            set
            {
                this.progress = value;
                this.RaisePropertyChanged(nameof(this.Progress));
                this.OnProgressUpdatedEvent?.Invoke(value);
            }
        }

        public String Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
                this.RaisePropertyChanged(nameof(this.Name));
            }
        }

        public String TaskIdentifier
        {
            get
            {
                return this.taskIdentifier;
            }

            protected set
            {
                this.taskIdentifier = value;
                this.RaisePropertyChanged(nameof(this.TaskIdentifier));
            }
        }

        public Boolean IsCanceled
        {
            get
            {
                return this.isCanceled;
            }

            protected set
            {
                lock (this.AccessLock)
                {
                    if (this.isCanceled == value)
                    {
                        return;
                    }

                    this.isCanceled = value;
                    this.RaisePropertyChanged(nameof(this.IsCanceled));

                    Task.Run(() =>
                    {
                        this.OnCanceledEvent?.Invoke(this);
                    });
                }
            }
        }

        public Boolean IsCompleted
        {
            get
            {
                return this.isCompleted;
            }

            protected set
            {
                lock (this.AccessLock)
                {
                    if (this.isCompleted == value)
                    {
                        return;
                    }

                    if (this.TaskIdentifier != null)
                    {
                        TrackableTask.uniqueTaskPool.TryRemove(this.TaskIdentifier, out _);
                    }

                    this.isCompleted = value;
                    this.RaisePropertyChanged(nameof(this.IsCompleted));

                    Task.Run(() =>
                    {
                        this.OnCompletedEvent?.Invoke(this);
                    });
                }
            }
        }

        public CancellationToken CancellationToken
        {
            get
            {
                return this.CancellationTokenSource.Token;
            }
        }

        protected CancellationTokenSource CancellationTokenSource { get; set; }

        private Object AccessLock { get; set; }

        public void Cancel()
        {
            this.CancellationTokenSource.Cancel();

            this.IsCanceled = true;
        }
    }

    public class TrackableTask<T> : TrackableTask
    {
        private TrackableTask(String name) : base(name)
        {
        }

        /// <summary>
        /// Creates an instance of a trackable task.
        /// </summary>
        /// <param name="name">The name of the task.</param>
        /// <param name="taskIdentifier">The unique identifier to prevent duplicate tasks.</param>
        /// <param name="progressUpdater">The progress updater callback object.</param>
        /// <param name="cancellationToken">The task cancellation token.</param>
        /// <returns>An instance of a trackable task.</returns>
        public static TrackableTask<T> Create(String name, String taskIdentifier, out UpdateProgress progressUpdater, out CancellationToken cancellationToken)
        {
            if (taskIdentifier.IsNullOrEmpty())
            {
                return TrackableTask<T>.Create(name, out progressUpdater, out cancellationToken);
            }

            if (TrackableTask.uniqueTaskPool.ContainsKey(taskIdentifier))
            {
                throw new TaskConflictException();
            }

            TrackableTask<T> instance = TrackableTask<T>.Create(name, out progressUpdater, out cancellationToken);
            instance.TaskIdentifier = taskIdentifier;

            if (!TrackableTask.uniqueTaskPool.TryAdd(taskIdentifier, instance))
            {
                throw new TaskConflictException();
            }

            return instance;
        }

        public static TrackableTask<T> Create(String name, out UpdateProgress progressUpdater, out CancellationToken cancellationToken)
        {
            TrackableTask<T> instance = new TrackableTask<T>(name);

            progressUpdater = instance.UpdateProgressCallback;
            cancellationToken = instance.CancellationTokenSource.Token;

            return instance;
        }

        public TrackableTask<T> With(Task<T> task)
        {
            this.Task = task;

            if (task.Status == TaskStatus.Created)
            {
                task.Start();
            }

            this.AwaitCompletion();

            return this;
        }

        public T Result
        {
            get
            {
                T result = this.Task.Result;

                this.IsCompleted = true;

                return result;
            }
        }

        private void UpdateProgressCallback(Single progress)
        {
            this.Progress = progress;
        }

        private Task<T> Task { get; set; }

        private async void AwaitCompletion()
        {
            await this.Task;

            this.IsCompleted = true;
        }
    }
    //// End class
}
//// End namespace