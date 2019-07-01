namespace Squalr.Engine.Common.DataTypes
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// A class representing a serializable data type.
    /// </summary>
    [DataContract]
    public class DataTypeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTypeBase" /> class.
        /// </summary>
        public DataTypeBase() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTypeBase" /> class.
        /// </summary>
        /// <param name="type">The default type.</param>
        public DataTypeBase(Type type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Gets or sets the type wrapped by this class.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// DataType for an array of bytes.
        /// </summary>
        public static readonly DataTypeBase ByteArray = new DataTypeBase(typeof(Byte[]));

        /// <summary>
        /// DataType for a boolean.
        /// </summary>
        public static readonly DataTypeBase Boolean = new DataTypeBase(typeof(Boolean));

        /// <summary>
        /// DataType for a signed byte.
        /// </summary>
        public static readonly DataTypeBase SByte = new DataTypeBase(typeof(SByte));

        /// <summary>
        /// DataType for a 16-bit integer.
        /// </summary>
        public static readonly DataTypeBase Int16 = new DataTypeBase(typeof(Int16));

        /// <summary>
        /// DataType for a 32-bit integer.
        /// </summary>
        public static readonly DataTypeBase Int32 = new DataTypeBase(typeof(Int32));

        /// <summary>
        /// DataType for a 64-bit integer.
        /// </summary>
        public static readonly DataTypeBase Int64 = new DataTypeBase(typeof(Int64));

        /// <summary>
        /// DataType for a byte.
        /// </summary>
        public static readonly DataTypeBase Byte = new DataTypeBase(typeof(Byte));

        /// <summary>
        /// DataType for an unsigned 16-bit integer.
        /// </summary>
        public static readonly DataTypeBase UInt16 = new DataTypeBase(typeof(UInt16));

        /// <summary>
        /// DataType for an unsigned 32-bit integer.
        /// </summary>
        public static readonly DataTypeBase UInt32 = new DataTypeBase(typeof(UInt32));

        /// <summary>
        /// DataType for an unsigned 64-bit integer.
        /// </summary>
        public static readonly DataTypeBase UInt64 = new DataTypeBase(typeof(UInt64));

        /// <summary>
        /// DataType for a single precision floating point value.
        /// </summary>
        public static readonly DataTypeBase Single = new DataTypeBase(typeof(Single));

        /// <summary>
        /// DataType for a double precision floating point value.
        /// </summary>
        public static readonly DataTypeBase Double = new DataTypeBase(typeof(Double));

        /// <summary>
        /// DataType for a char.
        /// </summary>
        public static readonly DataTypeBase Char = new DataTypeBase(typeof(Char));

        /// <summary>
        /// DataType for a string.
        /// </summary>
        public static readonly DataTypeBase String = new DataTypeBase(typeof(String));

        /// <summary>
        /// DataType for an integer pointer.
        /// </summary>
        public static readonly DataTypeBase IntPtr = new DataTypeBase(typeof(IntPtr));

        /// <summary>
        /// DataType for an unsigned integer pointer.
        /// </summary>
        public static readonly DataTypeBase UIntPtr = new DataTypeBase(typeof(UIntPtr));

        /// <summary>
        /// The list of scannable data types.
        /// </summary>
        private static readonly DataTypeBase[] ScannableDataTypes = new DataTypeBase[]
        {
            DataTypeBase.Boolean,
            DataTypeBase.SByte,
            DataTypeBase.Int16,
            DataTypeBase.Int32,
            DataTypeBase.Int64,
            DataTypeBase.Byte,
            DataTypeBase.UInt16,
            DataTypeBase.UInt32,
            DataTypeBase.UInt64,
            DataTypeBase.Single,
            DataTypeBase.Double,
        };

        /// <summary>
        /// Gets primitive types that are available for scanning.
        /// </summary>
        /// <returns>An enumeration of scannable types.</returns>
        public static IEnumerable<DataTypeBase> GetScannableDataTypes()
        {
            return DataTypeBase.ScannableDataTypes;
        }

        public Int32 Size
        {
            get
            {
                return Conversions.SizeOf(this);
            }
        }

        /// <summary>
        /// Gets or sets the string of the full namespace path representing this type.
        /// </summary>
        [DataMember]
        private String TypeString
        {
            get
            {
                return this.Type?.FullName;
            }

            set
            {
                this.Type = value == null ? null : Type.GetType(value);
            }
        }

        /// <summary>
        /// Implicitly converts a DataType to a Type for comparisons.
        /// </summary>
        /// <param name="dataType">The DataType to convert.</param>
        public static implicit operator Type(DataTypeBase dataType)
        {
            return dataType?.Type;
        }

        /// <summary>
        /// Implicitly converts a Type to a DataType for comparisons.
        /// </summary>
        /// <param name="type">The Type to convert.</param>
        public static implicit operator DataTypeBase(Type type)
        {
            return new DataTypeBase(type);
        }

        /// <summary>
        /// Indicates whether this object is equal to another.
        /// </summary>
        /// <param name="self">The object being compared.</param>
        /// <param name="other">The other object.</param>
        /// <returns>True if equal, otherwise false.</returns>
        public static Boolean operator ==(DataTypeBase self, DataTypeBase other)
        {
            if (Object.ReferenceEquals(self, other))
            {
                return true;
            }

            return self?.Type == other?.Type;
        }

        /// <summary>
        /// Indicates whether this object is not equal to another.
        /// </summary>
        /// <param name="self">The object being compared.</param>
        /// <param name="other">The other object.</param>
        /// <returns>True if not equal, otherwise false.</returns>
        public static Boolean operator !=(DataTypeBase self, DataTypeBase other)
        {
            return !(self == other);
        }

        /// <summary>
        /// Indicates whether this object is equal to another.
        /// </summary>
        /// <param name="self">The object being compared.</param>
        /// <param name="other">The other object.</param>
        /// <returns>True if equal, otherwise false.</returns>
        public static Boolean operator ==(DataTypeBase self, Type other)
        {
            if (Object.ReferenceEquals(self, other))
            {
                return true;
            }

            return self?.Type == other;
        }

        /// <summary>
        /// Indicates whether this object is not equal to another.
        /// </summary>
        /// <param name="self">The object being compared.</param>
        /// <param name="other">The other object.</param>
        /// <returns>True if not equal, otherwise false.</returns>
        public static Boolean operator !=(DataTypeBase self, Type other)
        {
            return !(self == other);
        }

        /// <summary>
        /// Indicates whether this object is equal to another.
        /// </summary>
        /// <param name="self">The object being compared.</param>
        /// <param name="other">The other object.</param>
        /// <returns>True if equal, otherwise false.</returns>
        public static Boolean operator ==(Type self, DataTypeBase other)
        {
            if (Object.ReferenceEquals(self, other))
            {
                return true;
            }

            return self == other?.Type;
        }

        /// <summary>
        /// Indicates whether this object is not equal to another.
        /// </summary>
        /// <param name="self">The object being compared.</param>
        /// <param name="other">The other object.</param>
        /// <returns>True if not equal, otherwise false.</returns>
        public static Boolean operator !=(Type self, DataTypeBase other)
        {
            return !(self == other);
        }

        /// <summary>
        /// Returns a hashcode for this instance.
        /// </summary>
        /// <returns>A hashcode for this instance.</returns>
        public override Int32 GetHashCode()
        {
            return this.Type.GetHashCode();
        }

        /// <summary>
        /// Indicates whether <see cref="DataTypeBase" /> objects are equal.
        /// </summary>
        /// <param name="dataType">The other <see cref="DataTypeBase" />.</param>
        /// <returns>True if the objects have an equal value.</returns>
        public override Boolean Equals(Object dataType)
        {
            return this.Type == (dataType as DataTypeBase)?.Type;
        }

        /// <summary>
        /// Indicates whether <see cref="DataTypeBase" /> objects are equal.
        /// </summary>
        /// <param name="dataType">The other <see cref="DataTypeBase" />.</param>
        /// <returns>True if the objects have an equal value.</returns>
        public Boolean Equals(DataTypeBase dataType)
        {
            return this.Type == dataType?.Type;
        }

        /// <summary>
        /// Returns a <see cref="String" /> representing the name of the current <see cref="DataTypeBase" />.
        /// </summary>
        /// <returns>The <see cref="String" /> representing the name of the current <see cref="DataTypeBase" /></returns>
        public override String ToString()
        {
            return this.Type?.ToString();
        }
    }
    //// End class
}
//// End namespace