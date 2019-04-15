namespace Squalr.Engine.Common.Observables
{
    using System;
    using System.Collections.Generic;

    public class Unsubscriber<T> : IDisposable
    {
        public Unsubscriber(HashSet<IObserver<T>> observers, IObserver<T> observer)
        {
            this.Observers = observers;
            this.Observer = observer;
        }

        private HashSet<IObserver<T>> Observers { get; set; }

        private IObserver<T> Observer { get; set; }

        public void Dispose()
        {
            if (Observers.Contains(this.Observer))
            {
                Observers.Remove(this.Observer);
            }
        }
    }
    //// End class
}
//// End namespace