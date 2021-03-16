using System.Text.RegularExpressions;

namespace Global
{
    public static class Extensions
    {
        /// <summary>
        /// Check if a string is null, empty, or whitespace
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static bool IsEmptyNullWhitespace(this string Source)
        {
            bool IsEmptyNullWhite = string.IsNullOrEmpty(Source) || string.IsNullOrWhiteSpace(Source);
            return IsEmptyNullWhite;
        }
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Return the value of the string used for sorting
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static string SortValue(this string Source)
        {
            string Parsed = Regex.Replace(Source, @"[^a-zA-Z0-9]+", "", RegexOptions.IgnoreCase).ToLower().Trim();
            return Parsed;
        }
        //-----------------------------------------------------------------------------------------------------------------------------
    }
}
