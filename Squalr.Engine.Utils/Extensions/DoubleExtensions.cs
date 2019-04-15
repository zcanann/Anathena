namespace Squalr.Engine.Common.Extensions
{
    using System;

    /// <summary>
    /// Contains extension methods for doubles.
    /// </summary>
    public static class DoubleExtensions
    {
        /// <summary>
        /// Determines if two doubles are almost equal in value via https://en.wikipedia.org/wiki/Unit_in_the_last_place distance.
        /// Note that, ULP tests do not work well for small numbers, so this will fall back on a delta test if the ULP fails.
        /// </summary>
        /// <param name="double1">The first double.</param>
        /// <param name="double2">The second double.</param>
        /// <returns>Returns true if the doubles are almost equal.</returns>
        public static unsafe Boolean AlmostEquals(this Double double1, Double double2)
        {
            const Int32 MaxDeltaBits = 32;
            const Single MaxDelta = 0.001f;

            // Step 1: Try a ULP distance test
            Int64 int1 = *((Int64*)&double1);
            if (int1 < 0)
            {
                int1 = Int64.MinValue - int1;
            }

            Int64 int2 = *((Int64*)&double2);
            if (int2 < 0)
            {
                int2 = Int64.MinValue - int2;
            }

            Int64 intDiff = int1 - int2;
            Int64 absoluteValueIntDiff = intDiff > 0 ? intDiff : -intDiff;

            if (absoluteValueIntDiff <= (1L << MaxDeltaBits))
            {
                return true;
            }

            // Step 2: Try a delta test
            Double delta = double1 - double2;
            Double absoluteDelta = delta > 0 ? delta : -delta;

            return absoluteDelta < MaxDelta;
        }
    }
    //// End class
}
//// End namespace