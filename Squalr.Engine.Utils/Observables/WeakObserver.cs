namespace Squalr.Engine.Common.Observables
{
    using System;

    public class WeakObserver<T> : IDisposable, IObserver<T>
    {
        private readonly WeakReference reference;

        private readonly IDisposable subscription;

        private Boolean disposed;

        public WeakObserver(IObservable<T> observable, IObserver<T> observer)
        {
            this.reference = new WeakReference(observer);
            this.subscription = observable.Subscribe(this);
        }

        void IObserver<T>.OnCompleted()
        {
            IObserver<T> observer = (IObserver<T>)this.reference.Target;

            if (observer != null)
            {
                observer.OnCompleted();
            }
            else
            {
                this.Dispose();
            }
        }

        void IObserver<T>.OnError(Exception error)
        {
            IObserver<T> observer = (IObserver<T>)this.reference.Target;

            if (observer != null)
            {
                observer.OnError(error);
            }
            else
            {
                this.Dispose();
            }
        }

        void IObserver<T>.OnNext(T value)
        {
            IObserver<T> observer = (IObserver<T>)this.reference.Target;

            if (observer != null)
            {
                observer.OnNext(value);
            }
            else
            {
                this.Dispose();
            }
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                this.disposed = true;
                this.subscription.Dispose();
            }
        }
    }
    //// End class
}
//// End namespace