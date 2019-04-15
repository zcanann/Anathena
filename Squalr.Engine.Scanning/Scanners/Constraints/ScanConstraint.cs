namespace Squalr.Engine.Scanning.Scanners.Constraints
{
    using Squalr.Engine.Common.DataTypes;
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Class to define a constraint for certain types of scans.
    /// </summary>
    public class ScanConstraint : ConstraintNode, INotifyPropertyChanged
    {
        /// <summary>
        /// The constraint type.
        /// </summary>
        private ConstraintType constraint;

        /// <summary>
        /// The value associated with this constraint, if applicable.
        /// </summary>
        private Object constraintValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanConstraint" /> class.
        /// </summary>
        public ScanConstraint()
        {
            this.Constraint = ConstraintType.Changed;
            this.ConstraintValue = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanConstraint" /> class.
        /// </summary>
        /// <param name="valueConstraint">The constraint type.</param>
        /// <param name="addressValue">The value associated with this constraint.</param>
        public ScanConstraint(ConstraintType valueConstraint, Object addressValue = null, DataType elementType = null)
        {
            this.Constraint = valueConstraint;
            this.ConstraintValue = addressValue;
            this.SetElementType(elementType);
        }

        /// <summary>
        /// Occurs after a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the constraint type.
        /// </summary>
        public ConstraintType Constraint
        {
            get
            {
                return this.constraint;
            }

            set
            {
                this.constraint = value;
                this.RaisePropertyChanged(nameof(this.Constraint));

                // Force an update of the constraint value, to determine if it is still valid for the new constraint
                this.ConstraintValue = this.constraintValue;
            }
        }

        /// <summary>
        /// Gets or sets the value associated with this constraint, if applicable.
        /// </summary>
        public Object ConstraintValue
        {
            get
            {
                if (ScanConstraint.IsValuedConstraint(this.Constraint))
                {
                    return this.constraintValue;
                }
                else
                {
                    return null;
                }
            }

            set
            {
                this.constraintValue = value;
                this.RaisePropertyChanged(nameof(this.ConstraintValue));
            }
        }

        /// <summary>
        /// Gets the name associated with this constraint.
        /// </summary>
        public String ConstraintName
        {
            get
            {
                switch (this.Constraint)
                {
                    case ConstraintType.Equal:
                        return "Equal";
                    case ConstraintType.NotEqual:
                        return "Not Equal";
                    case ConstraintType.GreaterThan:
                        return "Greater Than";
                    case ConstraintType.GreaterThanOrEqual:
                        return "Greater Than Or Equal";
                    case ConstraintType.LessThan:
                        return "Less Than";
                    case ConstraintType.LessThanOrEqual:
                        return "Less Than Or Equal";
                    case ConstraintType.Changed:
                        return "Changed";
                    case ConstraintType.Unchanged:
                        return "Unchanged";
                    case ConstraintType.Increased:
                        return "Increased";
                    case ConstraintType.Decreased:
                        return "Decreased";
                    case ConstraintType.IncreasedByX:
                        return "Increased By X";
                    case ConstraintType.DecreasedByX:
                        return "Decreased By X";
                    default:
                        throw new Exception("Unrecognized Constraint");
                }
            }
        }

        public override void SetElementType(Type elementType)
        {
            base.SetElementType(elementType);

            if (this.ConstraintValue == null)
            {
                return;
            }

            try
            {
                // Attempt to cast the value to the new type.
                this.ConstraintValue = Convert.ChangeType(this.ConstraintValue, elementType);
            }
            catch
            {
                this.ConstraintValue = null;
            }
        }

        public override Boolean IsValid()
        {
            if (!base.IsValid())
            {
                return false;
            }

            if (!ScanConstraint.IsValuedConstraint(this.Constraint))
            {
                return true;
            }

            return this.ConstraintValue != null;
        }

        /// <summary>
        /// Clones this scan constraint.
        /// </summary>
        /// <returns>The cloned scan constraint.</returns>
        public ScanConstraint Clone()
        {
            return new ScanConstraint(this.Constraint, this.ConstraintValue, this.ElementType);
        }

        /// <summary>
        /// Determines if this constraint conflicts with another constraint.
        /// </summary>
        /// <param name="other">The other scan constraint.</param>
        /// <returns>True if the constraints conflict, otherwise false.</returns>
        public Boolean ConflictsWith(ScanConstraint other)
        {
            if (this.Constraint == other.Constraint)
            {
                return true;
            }

            if (ScanConstraint.IsRelativeConstraint(this.Constraint) && ScanConstraint.IsRelativeConstraint(other.Constraint))
            {
                return true;
            }

            if (ScanConstraint.IsValuedConstraint(this.Constraint) && ScanConstraint.IsValuedConstraint(other.Constraint))
            {
                if (!ScanConstraint.IsRelativeConstraint(this.Constraint) && !ScanConstraint.IsRelativeConstraint(other.Constraint))
                {
                    if ((this.Constraint == ConstraintType.LessThan || this.Constraint == ConstraintType.LessThanOrEqual || this.Constraint == ConstraintType.NotEqual) &&
                        (other.Constraint == ConstraintType.GreaterThan || other.Constraint == ConstraintType.GreaterThanOrEqual || other.Constraint == ConstraintType.NotEqual))
                    {
                        if ((dynamic)this.ConstraintValue <= (dynamic)other.ConstraintValue)
                        {
                            return true;
                        }

                        return false;
                    }

                    if ((this.Constraint == ConstraintType.GreaterThan || this.Constraint == ConstraintType.GreaterThanOrEqual || this.Constraint == ConstraintType.NotEqual) &&
                        (other.Constraint == ConstraintType.LessThan || other.Constraint == ConstraintType.LessThanOrEqual || other.Constraint == ConstraintType.NotEqual))
                    {
                        if ((dynamic)this.ConstraintValue >= (dynamic)other.ConstraintValue)
                        {
                            return true;
                        }

                        return false;
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether this constraint is a relative comparison constraint, requiring previous values.
        /// </summary>
        /// <returns>True if the constraint is a relative value constraint.</returns>
        public static Boolean IsRelativeConstraint(ScanConstraint.ConstraintType constraint)
        {
            switch (constraint)
            {
                case ConstraintType.Changed:
                case ConstraintType.Unchanged:
                case ConstraintType.Increased:
                case ConstraintType.Decreased:
                case ConstraintType.IncreasedByX:
                case ConstraintType.DecreasedByX:
                    return true;
                case ConstraintType.Equal:
                case ConstraintType.NotEqual:
                case ConstraintType.GreaterThan:
                case ConstraintType.GreaterThanOrEqual:
                case ConstraintType.LessThan:
                case ConstraintType.LessThanOrEqual:
                    return false;
                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this constraint requires a value.
        /// </summary>
        /// <returns>True if the constraint requires a value.</returns>
        public static Boolean IsValuedConstraint(ScanConstraint.ConstraintType constraint)
        {
            switch (constraint)
            {
                case ConstraintType.Equal:
                case ConstraintType.NotEqual:
                case ConstraintType.GreaterThan:
                case ConstraintType.GreaterThanOrEqual:
                case ConstraintType.LessThan:
                case ConstraintType.LessThanOrEqual:
                case ConstraintType.IncreasedByX:
                case ConstraintType.DecreasedByX:
                    return true;
                case ConstraintType.Changed:
                case ConstraintType.Unchanged:
                case ConstraintType.Increased:
                case ConstraintType.Decreased:
                    return false;
                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        /// Indicates that a given property in this project item has changed.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        protected void RaisePropertyChanged(String propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Enumeration of all possible scan constraints.
        /// </summary>
        public enum ConstraintType
        {
            /// <summary>
            /// Comparative: The values must be equal.
            /// </summary>
            Equal,

            /// <summary>
            /// Comparative: The values must not be equal.
            /// </summary>
            NotEqual,

            /// <summary>
            /// Relative: The value must have changed.
            /// </summary>
            Changed,

            /// <summary>
            /// Relative: The value must not have changed.
            /// </summary>
            Unchanged,

            /// <summary>
            /// Relative: The value must have increased.
            /// </summary>
            Increased,

            /// <summary>
            /// Relative: The value must have decreased.
            /// </summary>
            Decreased,

            /// <summary>
            /// Relative: The value must have increased by a specific value.
            /// </summary>
            IncreasedByX,

            /// <summary>
            /// Relative: The value must have decreased by a specific value.
            /// </summary>
            DecreasedByX,

            /// <summary>
            /// Comparative: The value must be greater than the other value.
            /// </summary>
            GreaterThan,

            /// <summary>
            /// Comparative: The value must be greater than or equal the other value.
            /// </summary>
            GreaterThanOrEqual,

            /// <summary>
            /// Comparative: The value must be less than the other value.
            /// </summary>
            LessThan,

            /// <summary>
            /// Comparative: The value must be less than or equal the other value.
            /// </summary>
            LessThanOrEqual,
        }
    }
    //// End class
}
//// End namespace