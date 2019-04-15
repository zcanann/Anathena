namespace Squalr.Engine.Common.Extensions
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Extension methods for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Determines if an arbitrary string is a valid filename.
        /// </summary>
        /// <param name="str">The provided string.</param>
        /// <returns>True if the string is a valid filename, otherwise false.</returns>
        public static Boolean IsValidFileName(this String str)
        {
            return str != null && str.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0;
        }

        /// <summary>
        /// Removes the provided subjects from the given string. Will remove the first match only.
        /// </summary>
        /// <param name="str">The provided string.</param>
        /// <param name="suffixes">The suffixes to search for and remove.</param>
        /// <returns>The string with the trimmed suffix, if any were found.</returns>
        public static String RemoveSuffixes(this String str, Boolean ignoreCase, params String[] suffixes)
        {
            if (suffixes == null)
            {
                return str;
            }

            String strLower = ignoreCase ? str.ToLower() : str;
            String suffix = String.Empty;

            foreach (String nextSuffix in suffixes)
            {
                if (strLower.EndsWith(ignoreCase ? nextSuffix.ToLower() : nextSuffix))
                {
                    suffix = nextSuffix;
                    break;
                }
            }

            if (String.IsNullOrEmpty(suffix))
            {
                return str;
            }

            return str.Substring(0, str.Length - suffix.Length);
        }

        public static String TrimStartString(this String target, String trimChars)
        {
            return target.TrimStart(trimChars.ToCharArray());
        }

        public static String TrimEndString(this String target, String trimChars)
        {
            return target.TrimEnd(trimChars.ToCharArray());
        }

        /// <summary>
        /// Replaces any substring within a string with a new value. Allows for string comparison types.
        /// </summary>
        /// <param name="source">The original string.</param>
        /// <param name="oldValue">The value for which to search.</param>
        /// <param name="newValue">The replacement value.</param>
        /// <param name="comparisonType">The string comparison type between the old and replacement values.</param>
        /// <returns>A string with replaced values.</returns>
        public static String Replace(this String source, String oldValue, String newValue, StringComparison comparisonType)
        {
            if (source.Length == 0 || oldValue.Length == 0)
            {
                return source;
            }

            StringBuilder result = new StringBuilder();
            Int32 startingPos = 0;
            Int32 nextMatch;

            while ((nextMatch = source.IndexOf(oldValue, startingPos, comparisonType)) > -1)
            {
                result.Append(source, startingPos, nextMatch - startingPos);
                result.Append(newValue);
                startingPos = nextMatch + oldValue.Length;
            }

            result.Append(source, startingPos, source.Length - startingPos);

            return result.ToString();
        }
    }
    //// End class
}
//// End namespace