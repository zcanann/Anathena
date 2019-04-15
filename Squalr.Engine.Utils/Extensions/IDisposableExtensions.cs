namespace Squalr.Engine.Common.Extensions
{
    using Observables;
    using System;

    /// <summary>
    /// Extension methods for the IDisposable interface.
    /// </summary>
    public static class IDisposableExtensions
    {
        public static IDisposable WeakSubscribe<T>(this IObservable<T> observable, IObserver<T> observer)
        {
            return new WeakObserver<T>(observable, observer);
        }
    }
    //// End class
}
//// End namespace