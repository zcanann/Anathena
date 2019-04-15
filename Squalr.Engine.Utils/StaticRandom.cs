namespace Squalr.Engine.Common
{
    using System;
    using System.Threading;

    /// <summary>
    /// A thread safe static random class.
    /// </summary>
    public static class StaticRandom
    {
        /// <summary>
        /// The thread safe random class instance.
        /// </summary>
        private static readonly ThreadLocal<Random> Random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        /// <summary>
        /// The random seed.
        /// </summary>
        private static Int32 seed = Environment.TickCount;

        /// <summary>
        /// Returns a thread safe random integer.
        /// </summary>
        /// <returns>A random integer.</returns>
        public static Int32 Next()
        {
            return Random.Value.Next();
        }

        /// <summary>
        /// Returns a thread safe random integer.
        /// </summary>
        /// <param name="min">The inclusive lower bound of the number returned.</param>
        /// <param name="max">The exclusive upper bound of the number returned.</param>
        /// <returns>A random integer.</returns>
        public static Int32 Next(Int32 min, Int32 max)
        {
            return Random.Value.Next(min, max);
        }
    }
    //// End class
}
//// End namespace