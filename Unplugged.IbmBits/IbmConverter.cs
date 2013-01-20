using System;
using System.Text;

namespace Unplugged.IbmBits
{
    public static class IbmConverter
    {
        #region String

        static readonly Encoding _unicode = Encoding.Unicode;
        static readonly Encoding _ebcdic = Encoding.GetEncoding("IBM037");

        /// <summary>
        /// Returns a Unicode string converted from a byte array of EBCDIC encoded characters
        /// </summary>
        public static string ToString(byte[] value)
        {
            return ToString(value, 0);
        }

        /// <summary>
        /// Returns a Unicode string converted from a byte array of EBCDIC encoded characters 
        /// starting at the specified position
        /// </summary>
        /// <param name="startingIndex">
        /// Zero-based index of starting position in value array
        /// </param>
        public static string ToString(byte[] value, int startingIndex)
        {
            if (ReferenceEquals(null, value))
                throw new ArgumentNullException("value");
            return ToString(value, startingIndex, value.Length - startingIndex);
        }

        /// <summary>
        /// Returns a Unicode string converted from a byte array of EBCDIC encoded characters 
        /// starting at the specified position of the given length
        /// </summary>
        /// <param name="startingIndex">
        /// Zero-based index of starting position in value array
        /// </param>
        /// <param name="length">
        /// Number of characters to convert
        /// </param>
        public static string ToString(byte[] value, int startingIndex, int length)
        {
            var unicodeBytes = Encoding.Convert(_ebcdic, _unicode, value, startingIndex, length);
            return _unicode.GetString(unicodeBytes);
        }
        
        /// <summary>
        /// Returns a byte array of EBCDIC encoded characters converted from a Unicode string
        /// </summary>
        public static byte[] GetBytes(string value)
        {
            return GetBytes(value, 0);
        }

        /// <summary>
        /// Returns a byte array of EBCDIC encoded characters converted from a Unicode substring 
        /// starting at the specified position
        /// </summary>
        /// <param name="startingIndex">
        /// Zero-based starting index of substring
        /// </param>
        public static byte[] GetBytes(string value, int startingIndex)
        {
            return GetBytes(value, startingIndex, value.Length - startingIndex);
        }

        /// <summary>
        /// Returns a byte array of EBCDIC encoded characters converted from a Unicode substring 
        /// starting at the specified position with the given length
        /// </summary>
        /// <param name="startingIndex">
        /// Zero-based starting index of substring
        /// </param>
        /// <param name="length">
        /// Number of characters to convert
        /// </param>
        public static byte[] GetBytes(string value, int startingIndex, int length)
        {
            if (ReferenceEquals(null, value))
                throw new ArgumentNullException("value");
            var unicodeBytes = _unicode.GetBytes(value.ToCharArray(startingIndex, length));
            return Encoding.Convert(_unicode, _ebcdic, unicodeBytes);
        }

        #endregion

        /// <summary>
        /// Returns a 16-bit signed integer converted from two bytes encoding a big endian 16-bit signed integer
        /// </summary>
        public static Int16 ToInt16(byte[] value)
        {
            return ToInt16(value, 0);
        }

        /// <summary>
        /// Returns a 16-bit signed integer converted from two bytes in a specified position encoding a big endian 16-bit signed integer
        /// </summary>
        public static Int16 ToInt16(byte[] value, int startIndex)
        {
            if (ReferenceEquals(null, value))
                throw new ArgumentNullException("value");
            var bytes = new byte[] { value[startIndex + 1], value[startIndex] };
            return BitConverter.ToInt16(bytes, 0);
        }

        /// <summary>
        /// Returns a 32-bit signed integer converted from four bytes encoding a big endian 32-bit signed integer
        /// </summary>
        public static int ToInt32(byte[] value)
        {
            return ToInt32(value, 0);
        }

        /// <summary>
        /// Returns a 32-bit signed integer converted from four bytes at a specified position encoding a big endian 32-bit signed integer
        /// </summary>
        public static Int32 ToInt32(byte[] value, int startIndex)
        {
            if (ReferenceEquals(null, value))
                throw new ArgumentNullException("value");
            var bytes = new byte[] { value[startIndex + 3], value[startIndex + 2], value[startIndex + 1], value[startIndex] };
            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        /// Returns a 32-bit IEEE single precision floating point number from four bytes encoding
        /// a single precision number in IBM System/360 Floating Point format
        /// </summary>
        public static float ToSingle(byte[] value)
        {
            if (ReferenceEquals(null, value))
                throw new ArgumentNullException("value");
            if (0 == BitConverter.ToInt32(value, 0))
                return 0;

            // The first bit is the sign.  The next 7 bits are the exponent.
            int exponentBits = value[0];
            var sign = +1.0;
            // Remove sign from first bit
            if (exponentBits >= 128)
            {
                sign = -1.0;
                exponentBits -= 128;
            }
            // Remove the bias of 64 from the exponent
            exponentBits -= 64;
            var ibmBase = 16;
            var exponent = Math.Pow(ibmBase, exponentBits);

            // The fractional part is Big Endian unsigned int to the right of the radix point
            // So we reverse the bytes and pack them back into an int
            var fractionBytes = new byte[] { value[3], value[2], value[1], 0 };
            // Note: The sign bit for int32 is in the last byte of the array, which is zero, so we don't have to convert to uint
            var mantissa = BitConverter.ToInt32(fractionBytes, 0);
            // And divide by 2^(8 * 3) to move the decimal all the way to the left
            var dividend = 16777216; // Math.Pow(2, 8 * 3);
            var fraction = mantissa / (float)dividend;

            return (float)(sign * exponent * fraction);
        }
    }
}
