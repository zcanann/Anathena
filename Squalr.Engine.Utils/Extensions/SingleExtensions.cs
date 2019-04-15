namespace Squalr.Engine.Common.Extensions
{
    using System;

    /// <summary>
    /// Contains extension methods for singles.
    /// </summary>
    public static class SingleExtensions
    {
        /// <summary>
        /// Determines if two doubles are almost equal in value via https://en.wikipedia.org/wiki/Unit_in_the_last_place distance.
        /// Note that, ULP tests do not work well for small numbers, so this will fall back on a delta test if the ULP fails.
        /// </summary>
        /// <param name="float1">The first float.</param>
        /// <param name="float2">The second float.</param>
        /// <returns>Returns true if the floats are almost equal.</returns>
        public static unsafe Boolean AlmostEquals(this Single float1, Single float2)
        {
            const Int32 MaxDeltaBits = 16;
            const Single MaxDelta = 0.001f;

            // Step 1: Try a ULP distance test
            Int32 int1 = *((Int32*)&float1);
            if (int1 < 0)
            {
                int1 = Int32.MinValue - int1;
            }

            Int32 int2 = *((Int32*)&float2);
            if (int2 < 0)
            {
                int2 = Int32.MinValue - int2;
            }

            Int32 intDiff = int1 - int2;
            Int32 absoluteValueIntDiff = intDiff > 0 ? intDiff : -intDiff;

            if (absoluteValueIntDiff <= (1 << MaxDeltaBits))
            {
                return true;
            }

            // Step 2: Try a delta test
            Single delta = float1 - float2;
            Single absoluteDelta = delta > 0 ? delta : -delta;

            return absoluteDelta < MaxDelta;
        }
    }
    //// End class
}
//// End namespace