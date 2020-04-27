namespace Squalr.Engine.Projects.Items
{
    using Squalr.Engine.Common;
    using Squalr.Engine.Common.DataTypes;
    using Squalr.Engine.Common.Extensions;
    using Squalr.Engine.Memory;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines an address that can be added to the project explorer.
    /// </summary>
    [DataContract]
    public class PointerItem : AddressItem
    {
        /// <summary>
        /// The extension for this project item type.
        /// </summary>
        public new const String Extension = ".ptr";

        /// <summary>
        /// The identifier for the base address of this object.
        /// </summary>
        [DataMember]
        protected String moduleName;

        /// <summary>
        /// The base address of this object. This will be added as an offset from the resolved base identifier.
        /// </summary>
        [DataMember]
        protected UInt64 moduleOffset;

        /// <summary>
        /// The pointer offsets of this address item.
        /// </summary>
        [DataMember]
        protected IEnumerable<Int32> pointerOffsets;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressItem" /> class.
        /// </summary>
        public PointerItem() : this(0, DataTypeBase.Int32, "New Address")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressItem" /> class.
        /// </summary>
        /// <param name="baseAddress">The base address. This will be added as an offset from the resolved base identifier.</param>
        /// <param name="dataType">The data type of the value at this address.</param>
        /// <param name="description">The description of this address.</param>
        /// <param name="resolveType">The identifier type for this address item.</param>
        /// <param name="moduleName">The identifier for the base address of this object.</param>
        /// <param name="pointerOffsets">The pointer offsets of this address item.</param>
        /// <param name="isValueHex">A value indicating whether the value at this address should be displayed as hex.</param>
        /// <param name="value">The value at this address. If none provided, it will be figured out later. Used here to allow immediate view updates upon creation.</param>
        public PointerItem(
            UInt64 baseAddress,
            Type dataType,
            String description = "New Address",
            String moduleName = null,
            IEnumerable<Int32> pointerOffsets = null,
            Boolean isValueHex = false,
            Object value = null)
            : base(dataType, description, isValueHex, value)
        {
            // Bypass setters to avoid running setter code
            this.moduleOffset = baseAddress;
            this.moduleName = moduleName;
            this.pointerOffsets = pointerOffsets;
        }

        /// <summary>
        /// Gets or sets the identifier for the base address of this object.
        /// </summary>
        public virtual String ModuleName
        {
            get
            {
                return this.moduleName;
            }

            set
            {
                this.moduleName = value ?? String.Empty;
                this.RaisePropertyChanged(nameof(this.ModuleName));
                this.RaisePropertyChanged(nameof(this.IsStatic));
                this.RaisePropertyChanged(nameof(this.AddressSpecifier));
                this.Save();
            }
        }

        /// <summary>
        /// Gets or sets the base address of this object. This will be added as an offset from the resolved base identifier.
        /// </summary>
        public virtual UInt64 ModuleOffset
        {
            get
            {
                return this.moduleOffset;
            }

            set
            {
                if (this.moduleOffset == value)
                {
                    return;
                }

                this.CalculatedAddress = value;
                this.moduleOffset = value;
                this.RaisePropertyChanged(nameof(this.ModuleOffset));
                this.RaisePropertyChanged(nameof(this.AddressSpecifier));
                this.Save();
            }
        }

        /// <summary>
        /// Gets or sets the pointer offsets of this address item.
        /// </summary>
        public virtual IEnumerable<Int32> PointerOffsets
        {
            get
            {
                return this.pointerOffsets;
            }

            set
            {
                if (this.pointerOffsets != null && this.pointerOffsets.SequenceEqual(value))
                {
                    return;
                }

                this.pointerOffsets = value;
                this.RaisePropertyChanged(nameof(this.PointerOffsets));
                this.RaisePropertyChanged(nameof(this.IsPointer));
                this.RaisePropertyChanged(nameof(this.AddressSpecifier));
                this.Save();
            }
        }

        /// <summary>
        /// Gets the address specifier for this address. If a static address, this is 'ModuleName + offset', otherwise this is an address string.
        /// </summary>
        [Browsable(false)]
        public String AddressSpecifier
        {
            get
            {
                if (this.IsStatic)
                {
                    if (this.ModuleOffset.ToInt64() >= 0)
                    {
                        return this.ModuleName + "+" + Conversions.ToHex(this.ModuleOffset, formatAsAddress: false);
                    }
                    else
                    {
                        return this.ModuleName + "-" + Conversions.ParsePrimitiveAsHexString(DataTypeBase.UInt64, this.ModuleOffset, signHex: true).TrimStart('-');
                    }
                }
                else if (this.IsPointer)
                {
                    return Conversions.ToHex(this.CalculatedAddress);
                }
                else
                {
                    return Conversions.ToHex(this.ModuleOffset);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating if this pointer/address is static.
        /// </summary>
        [Browsable(false)]
        public Boolean IsStatic
        {
            get
            {
                return !this.ModuleName.IsNullOrEmpty();
            }
        }

        /// <summary>
        /// Gets a value indicating if this object is a true pointer and not just an address.
        /// </summary>
        [Browsable(false)]
        public Boolean IsPointer
        {
            get
            {
                return !this.PointerOffsets.IsNullOrEmpty();
            }
        }

        /// <summary>
        /// Gets the extension for this project item.
        /// </summary>
        public override String GetExtension()
        {
            return PointerItem.Extension;
        }

        /// <summary>
        /// Resolves the address of an address, pointer, or managed object.
        /// </summary>
        /// <returns>The base address of this object.</returns>
        protected override UInt64 ResolveAddress()
        {
            UInt64 pointer = MemoryQueryer.Instance.ResolveModule(null, this.ModuleName);

            pointer = pointer.Add(this.ModuleOffset);

            if (this.PointerOffsets == null || this.PointerOffsets.Count() == 0)
            {
                return pointer;
            }

            foreach (Int32 offset in this.PointerOffsets)
            {
                throw new NotImplementedException();
                /*
                if (Processes.Instance.IsOpenedProcess32Bit())
                {
                    pointer = MemoryReader.Instance.Read<Int32>(pointer, out successReading).ToUInt64();
                }
                else
                {
                    pointer = MemoryReader.Instance.Read<UInt64>(pointer, out successReading);
                }

                if (pointer == 0 || !successReading)
                {
                    pointer = 0;
                    break;
                }

                pointer = pointer.Add(offset);
                */
            }

            return pointer;
        }
    }
    //// End class
}
//// End namespace