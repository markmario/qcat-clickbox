namespace ClickBox.Util
{
    using System;

    public static class GuidExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Wraps the BASE32 Encoding to give you a short GUID
        /// </summary>
        /// <param name="id">
        /// the GUID to parse
        /// </param>
        /// <returns>
        /// short GUID string
        /// </returns>
        public static string ToShortString(this Guid id)
        {
            return Base32Encoding.ToString(id.ToByteArray()).ToLower();
        }

        /// <summary>
        /// Converts the BASE32 Encoded string to a valid GUID
        /// </summary>
        /// <param name="shortId">
        /// the short string
        /// </param>
        /// <returns>
        /// Returns a new Id
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the short string is not valid
        /// </exception>
        public static Guid ParseShortString(this string shortId) 
        {
            return new Guid(Base32Encoding.ToBytes(shortId));
        }

        #endregion
    }
}