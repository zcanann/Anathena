namespace Squalr.Engine.Common.DataTypes
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// A class representing a serializable data type.
    /// </summary>
    [DataContract]
    public class DataType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataType" /> class.
        /// </summary>
        public DataType() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataType" /> class.
        /// </summary>
        /// <param name="type">The default type.</param>
        public DataType(Type type)
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
        public static readonly DataType ByteArray = new DataType(typeof(Byte[]));

        /// <summary>
        /// DataType for a boolean.
        /// </summary>
        public static readonly DataType Boolean = new DataType(typeof(Boolean));

        /// <summary>
        /// DataType for a signed byte.
        /// </summary>
        public static readonly DataType SByte = new DataType(typeof(SByte));

        /// <summary>
        /// DataType for a 16-bit integer.
        /// </summary>
        public static readonly DataType Int16 = new DataType(typeof(Int16));

        /// <summary>
        /// DataType for a 32-bit integer.
        /// </summary>
        public static readonly DataType Int32 = new DataType(typeof(Int32));

        /// <summary>
        /// DataType for a 64-bit integer.
        /// </summary>
        public static readonly DataType Int64 = new DataType(typeof(Int64));

        /// <summary>
        /// DataType for a byte.
        /// </summary>
        public static readonly DataType Byte = new DataType(typeof(Byte));

        /// <summary>
        /// DataType for an unsigned 16-bit integer.
        /// </summary>
        public static readonly DataType UInt16 = new DataType(typeof(UInt16));

        /// <summary>
        /// DataType for an unsigned 32-bit integer.
        /// </summary>
        public static readonly DataType UInt32 = new DataType(typeof(UInt32));

        /// <summary>
        /// DataType for an unsigned 64-bit integer.
        /// </summary>
        public static readonly DataType UInt64 = new DataType(typeof(UInt64));

        /// <summary>
        /// DataType for a single precision floating point value.
        /// </summary>
        public static readonly DataType Single = new DataType(typeof(Single));

        /// <summary>
        /// DataType for a double precision floating point value.
        /// </summary>
        public static readonly DataType Double = new DataType(typeof(Double));

        /// <summary>
        /// DataType for a char.
        /// </summary>
        public static readonly DataType Char = new DataType(typeof(Char));

        /// <summary>
        /// DataType for a string.
        /// </summary>
        public static readonly DataType String = new DataType(typeof(String));

        /// <summary>
        /// DataType for an integer pointer.
        /// </summary>
        public static readonly DataType IntPtr = new DataType(typeof(IntPtr));

        /// <summary>
        /// DataType for an unsigned integer pointer.
        /// </summary>
        public static readonly DataType UIntPtr = new DataType(typeof(UIntPtr));

        /// <summary>
        /// The list of scannable data types.
        /// </summary>
        private static readonly DataType[] ScannableDataTypes = new DataType[]
        {
            DataType.Boolean,
            DataType.SByte,
            DataType.Int16,
            DataType.Int32,
            DataType.Int64,
            DataType.Byte,
            DataType.UInt16,
            DataType.UInt32,
            DataType.UInt64,
            DataType.Single,
            DataType.Double,
        };

        /// <summary>
        /// Gets primitive types that are available for scanning.
        /// </summary>
        /// <returns>An enumeration of scannable types.</returns>
        public static IEnumerable<DataType> GetScannableDataTypes()
        {
            return DataType.ScannableDataTypes;
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
        public static implicit operator Type(DataType dataType)
        {
            return dataType?.Type;
        }

        /// <summary>
        /// Implicitly converts a Type to a DataType for comparisons.
        /// </summary>
        /// <param name="type">The Type to convert.</param>
        public static implicit operator DataType(Type type)
        {
            return new DataType(type);
        }

        /// <summary>
        /// Indicates whether this object is equal to another.
        /// </summary>
        /// <param name="self">The object being compared.</param>
        /// <param name="other">The other object.</param>
        /// <returns>True if equal, otherwise false.</returns>
        public static Boolean operator ==(DataType self, DataType other)
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
        public static Boolean operator !=(DataType self, DataType other)
        {
            return !(self == other);
        }

        /// <summary>
        /// Indicates whether this object is equal to another.
        /// </summary>
        /// <param name="self">The object being compared.</param>
        /// <param name="other">The other object.</param>
        /// <returns>True if equal, otherwise false.</returns>
        public static Boolean operator ==(DataType self, Type other)
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
        public static Boolean operator !=(DataType self, Type other)
        {
            return !(self == other);
        }

        /// <summary>
        /// Indicates whether this object is equal to another.
        /// </summary>
        /// <param name="self">The object being compared.</param>
        /// <param name="other">The other object.</param>
        /// <returns>True if equal, otherwise false.</returns>
        public static Boolean operator ==(Type self, DataType other)
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
        public static Boolean operator !=(Type self, DataType other)
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
        /// Indicates whether <see cref="DataType" /> objects are equal.
        /// </summary>
        /// <param name="dataType">The other <see cref="DataType" />.</param>
        /// <returns>True if the objects have an equal value.</returns>
        public override Boolean Equals(Object dataType)
        {
            return this.Type == (dataType as DataType)?.Type;
        }

        /// <summary>
        /// Indicates whether <see cref="DataType" /> objects are equal.
        /// </summary>
        /// <param name="dataType">The other <see cref="DataType" />.</param>
        /// <returns>True if the objects have an equal value.</returns>
        public Boolean Equals(DataType dataType)
        {
            return this.Type == dataType?.Type;
        }

        /// <summary>
        /// Returns a <see cref="String" /> representing the name of the current <see cref="DataType" />.
        /// </summary>
        /// <returns>The <see cref="String" /> representing the name of the current <see cref="DataType" /></returns>
        public override String ToString()
        {
            return this.Type?.ToString();
        }
    }
    //// End class
}
//// End namespace