namespace Squalr.Engine.Common.OS
{
    using System;
    using System.Numerics;

    public static class Vectors
    {
        /// <summary>
        /// Gets a value indicating if the archiecture has vector instruction support.
        /// </summary>
        public static Boolean HasVectorSupport
        {
            get
            {
                return Vector.IsHardwareAccelerated;
            }
        }

        /// <summary>
        /// Gets the vector size supported by the current architecture.
        /// If vectors are not supported, returns the lowest common denominator vector size for the architecture.
        /// </summary>
        public static Int32 VectorSize
        {
            get
            {
                return Vector<Byte>.Count;
            }
        }
    }
    //// End class
}
//// End namespace