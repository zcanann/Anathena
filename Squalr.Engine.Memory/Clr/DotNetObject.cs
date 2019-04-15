namespace Squalr.Engine.Memory.Clr
{
    using Squalr.Engine.Common.DataTypes;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// An object representing a .Net object in an external process.
    /// </summary>
    public class DotNetObject : IEnumerable, IComparable<DotNetObject>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetObject" /> class.
        /// </summary>
        /// <param name="parent">The parent object of this object.</param>
        /// <param name="objectReference">The address of this object.</param>
        /// <param name="elementType">The data type this object represents.</param>
        /// <param name="name">The name of this object as a full .Net namespace path.</param>
        public DotNetObject(DotNetObject parent, UInt64 objectReference, Type elementType, String name)
        {
            this.Parent = parent;
            this.ObjectReference = unchecked(objectReference);
            this.ElementType = elementType;

            // Trim root name from all objects if applicable
            String rootName = this.GetRootName();

            if (name.StartsWith(rootName))
            {
                name = name.Remove(0, rootName.Length);
            }

            this.Name = name;

            this.Children = new List<DotNetObject>();
        }

        /// <summary>
        /// Gets or sets the address of the object. The CLR can change this at any time, in which case it will eventually be re-resolved.
        /// </summary>
        [ReadOnly(true)]
        //// [TypeConverter(typeof(AddressConverter))]
        [Category("Properties"), DisplayName("Address"), Description("Address of the object. The CLR can change this at any time")]
        public UInt64 ObjectReference { get; set; }

        /// <summary>
        /// Gets or sets the name of the .Net object.
        /// </summary>
        [ReadOnly(true)]
        [Category("Properties"), DisplayName("Name"), Description("Name of the .NET object")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the data type of the .Net object.
        /// </summary>
        [ReadOnly(true)]
        //// [TypeConverter(typeof(DataTypeConverter))]
        [Category("Properties"), DisplayName("Value Type"), Description("Data type of the address")]
        public DataType ElementType { get; set; }

        /// <summary>
        /// Gets or sets the children of this .Net object.
        /// </summary>
        public List<DotNetObject> Children { get; set; }

        /// <summary>
        /// Gets or sets the parent of this .Net object.
        /// </summary>
        private DotNetObject Parent { get; set; }

        /// <summary>
        /// Gets the full namespace name of this object
        /// </summary>
        /// <returns>The full namespace name of this object</returns>
        public String GetFullName()
        {
            return this.GetFullNamespace(String.Empty);
        }

        /// <summary>
        /// Compares this to another .Net object for sorting. Comparison done alphabetically by name.
        /// </summary>
        /// <param name="other">The .Net object to compare to.</param>
        /// <returns>An integer indicating sorting priority.</returns>
        public Int32 CompareTo(DotNetObject other)
        {
            String name = this.Name == null ? String.Empty : this.Name;
            String otherName = other.Name == null ? String.Empty : other.Name;

            return name.CompareTo(otherName);
        }

        /// <summary>
        /// Gets the enumerator for the children of this object.
        /// </summary>
        /// <returns>The enumerator for the children of this object.</returns>
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)this.Children).GetEnumerator();
        }

        /// <summary>
        /// Gets the root namespace name of this object.
        /// </summary>
        /// <returns>The root namespace name.</returns>
        private String GetRootName()
        {
            if (this.Parent == null)
            {
                return this.Name == null ? String.Empty : this.Name;
            }

            return this.Parent.GetRootName();
        }

        /// <summary>
        /// Gets the full namespace that contains this object recursively.
        /// </summary>
        /// <param name="currentNamespace">The namespace constructed so far.</param>
        /// <returns>The full namespace of this object.</returns>
        private String GetFullNamespace(String currentNamespace)
        {
            if (this.Parent == null)
            {
                return this.GetRootName();
            }

            return this.Parent.GetFullNamespace(currentNamespace) + "." + this.Name;
        }
    }
    //// End class
}
//// End namespace