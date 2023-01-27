using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Unplugged.IbmBits
{
    public static class IbmConverter
    {
        static IbmConverter()
        {
            // .NET Standard does not include the EBCDIC encoding by default.
            // So it is necessary to reference System.Text.Encoding.CodePages
            // and register CodePagesEncodingProvider
            // "For code pages that otherwise are available only in the desktop .NET Framework"
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        #region String

        static Encoding Unicode => Encoding.Unicode;

        /// <summary>
        /// IBM EBCDIC Encoding for US / Canada
        /// <see href="https://en.wikipedia.org/wiki/Code_page_37"/>
        /// </summary>
        static Encoding Ebcdic => Encoding.GetEncoding("IBM037");

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
            var unicodeBytes = Encoding.Convert(Ebcdic, Unicode, value, startingIndex, length);
            return Unicode.GetString(unicodeBytes);
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
            var unicodeBytes = Unicode.GetBytes(value.ToCharArray(startingIndex, length));
            return Encoding.Convert(Unicode, Ebcdic, unicodeBytes);
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

        #region Packed Decimal

        /// <summary>
        /// Unpack the byte array into a decimal
        /// </summary>
        /// <remarks>
        /// From the java version made by p4w3l located here : http://cobol2j.cvs.sourceforge.net/viewvc/cobol2j/cobol2j/src/main/java/net/sf/cobol2j/RecordSet.java?revision=1.25&view=markup from
        /// the open source projet cobol2j at http://sourceforge.net/projects/cobol2j/ under LGPLV2
        /// </remarks>
        public static Decimal ToUnpackedDecimal(byte[] inputData, int scale)
        {
            var inputLength = inputData.Length;
            var strbuf = new StringBuilder();
            int tempData;
            int tempData1;

            for (int i = 0; i < inputData.Length; i++)
            {
                tempData = inputData[i];
                tempData1 = tempData & 0xF0;
                int tempData2 = tempData1 >> 4;
                strbuf.Append(tempData2);

                if (i < (inputLength - 1))
                {
                    tempData = inputData[i];
                    tempData1 = tempData & 0x0F;
                    strbuf.Append(tempData1);
                }
            }

            if ((scale > 0 && strbuf.Length -scale>0))
                strbuf.Insert(strbuf.Length - scale, '.');

            var result = decimal.Parse(strbuf.ToString());

            tempData = inputData[inputLength - 1];
            tempData1 = tempData & 0x0F;

            if ((tempData1 == 0x0F) || (tempData1 == 0x0C))
                return result;

            if (tempData1 == 0x0D)
                return -result;

            return result;
        }

        /// <summary>
        /// Convert the decimal value into its packed value
        /// </summary>
        /// <param name="originalValue">The value to pack</param>
        /// <returns>The packed value as a byte[]</returns>
        /// <remarks>
        /// James Howey Copyright (c) Microsoft Corporation.  All rights reserved. Microsoft Shared Source Permissive License
        /// </remarks>
        public static byte[] GetBytes(decimal originalValue)
        {
            var value = long.Parse(originalValue.ToString(CultureInfo.InvariantCulture).Replace(".", ""));
            var comp3 = new Stack<byte>(10);

            byte currentByte;
            if (value < 0)
            {
                currentByte = 0x0d;
                value = -value;
            }
            else
                currentByte = 0x0c;

            var byteComplete = false;
            while (value != 0)
            {
                if (byteComplete)
                    currentByte = (byte)(value % 10);
                else
                    currentByte |= (byte)((value % 10) << 4);

                value /= 10;
                byteComplete = !byteComplete;

                if (byteComplete)
                    comp3.Push(currentByte);
            }

            if (!byteComplete)
                comp3.Push(currentByte);

            return comp3.ToArray();
        }

        #endregion
    }
}
