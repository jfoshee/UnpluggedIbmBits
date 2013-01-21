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

        #region Int16

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
        /// Returns two bytes encoding a big endian 16-bit signed integer given a 16-bit signed integer
        /// </summary>
        public static byte[] GetBytes(short value)
        {
            var b = BitConverter.GetBytes(value);
            return new byte[] { b[1], b[0] };
        }

        #endregion

        #region Int32

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

        public static byte[] GetBytes(int value)
        {
            var b = BitConverter.GetBytes(value);
            return new byte[] { b[3], b[2], b[1], b[0] };
        }

        #endregion

        #region Single

        static readonly int _ibmBase = 16;
        static readonly byte _exponentBias = 64;
        static readonly int _threeByteShift = 16777216;

        /// <summary>
        /// Returns a 32-bit IEEE single precision floating point number from four bytes encoding
        /// a single precision number in IBM System/360 Floating Point format
        /// </summary>
        public static Single ToSingle(byte[] value)
        {
            if (ReferenceEquals(null, value))
                throw new ArgumentNullException("value");
            if (0 == BitConverter.ToInt32(value, 0))
                return 0;

            // The first bit is the sign.  The next 7 bits are the exponent.
            byte exponentBits = value[0];
            var sign = +1.0;
            // Remove sign from first bit
            if (exponentBits >= 128)
            {
                sign = -1.0;
                exponentBits -= 128;
            }
            // Remove the bias from the exponent
            exponentBits -= _exponentBias;
            var exponent = Math.Pow(_ibmBase, exponentBits);

            // The fractional part is Big Endian unsigned int to the right of the radix point
            // So we reverse the bytes and pack them back into an int
            var fractionBytes = new byte[] { value[3], value[2], value[1], 0 };
            // Note: The sign bit for int32 is in the last byte of the array, which is zero, so we don't have to convert to uint
            var mantissa = BitConverter.ToInt32(fractionBytes, 0);
            // And divide by 2^(8 * 3) to move the decimal all the way to the left
            var fraction = mantissa / (float)_threeByteShift;

            return (float)(sign * exponent * fraction);
        }

        /// <summary>
        /// Given a 32-bit IEEE single precision floating point number, returns four bytes encoding
        /// a single precision number in IBM System/360 Floating Point format
        /// </summary>
        public static byte[] GetBytes(Single value)
        {
            var bytes = new byte[4];
            if (value == 0)
                return bytes;

            // Sign
            if (value < 0)
                bytes[0] = 128;
            var v = Math.Abs(value);

            // Fraction
            // Find the number of digits (in the IBM base) we need to move the radix point to get a value that is less than 1
            var moveRadix = (int)Math.Log(v, _ibmBase) + 1;
            var fraction = v / (Math.Pow(_ibmBase, moveRadix));
            var fractionInt = (int)(_threeByteShift * fraction);
            var fractionBytes = BitConverter.GetBytes(fractionInt);
            bytes[3] = fractionBytes[0];
            bytes[2] = fractionBytes[1];
            bytes[1] = fractionBytes[2];

            // Exponent
            var exponent = moveRadix + _exponentBias;
            bytes[0] += (byte)exponent;
            return bytes;
        }

        #endregion
    }
}
