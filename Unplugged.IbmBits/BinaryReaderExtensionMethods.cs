using System;
using System.IO;

namespace Unplugged.IbmBits
{
    public static class BinaryReaderExtensionMethods
    {
        /// <summary>
        /// Reads the requested number of 8-bit EBCDIC encoded characters from the stream and converts them to a 
        /// Unicode string.
        /// </summary>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>
        /// Unicode encoded string converted from bytes read from the stream.
        /// The length of the string might be less than the number of bytes requested if the end of the stream is reached.
        /// </returns>
        public static string ReadStringEbcdic(this BinaryReader reader, int count)
        {
            if (ReferenceEquals(null, reader))
                throw new ArgumentNullException("reader");
            var bytes = reader.ReadBytes(count);
            return IbmConverter.ToString(bytes);
        }

        /// <summary>
        /// Reads a 16-bit integer from the stream that has been encoded as big endian.
        /// </summary>
        public static Int16 ReadInt16BigEndian(this BinaryReader reader)
        {
            if (ReferenceEquals(null, reader))
                throw new ArgumentNullException("reader");
            var bytes = ReadBytes(reader, 2);
            return IbmConverter.ToInt16(bytes);
        }

        /// <summary>
        /// Reads a 32-bit integer from the stream that has been encoded as big endian.
        /// </summary>
        public static Int32 ReadInt32BigEndian(this BinaryReader reader)
        {
            if (ReferenceEquals(null, reader))
                throw new ArgumentNullException("reader");
            var bytes = ReadBytes(reader, 4);
            return IbmConverter.ToInt32(bytes);
        }

        /// <summary>
        /// Reads a single precision 32-bit floating point number from the stream 
        /// that has been encoded in IBM System/360 Floating Point format
        /// </summary>
        /// <returns>IEEE formatted single precision floating point</returns>
        public static float ReadSingleIbm(this BinaryReader reader)
        {
            if (ReferenceEquals(null, reader))
                throw new ArgumentNullException("reader");
            var bytes = ReadBytes(reader, 4);
            return IbmConverter.ToSingle(bytes);
        }
        
        /// <summary>
        /// Reads a pack decimal from the stream
        /// </summary>
        /// <param name="reader">The reader from which the bytes will be read</param>
        /// <param name="storageLength">The total storage length of the packed decimal</param>
        /// <param name="scale">The scale of the decimal (number of number after the .)</param>
        /// <returns>The decimal read from the stream</returns>
        public static decimal ReadPackedDecimalIbm(this BinaryReader reader, byte storageLength, byte scale)
        {
            if (ReferenceEquals(null, reader))
                throw new ArgumentNullException("reader");
            var bytes = ReadBytes(reader, storageLength);
            return IbmConverter.ToUnpackedDecimal(bytes, scale);
        }

        static byte[] ReadBytes(BinaryReader reader, int count)
        {
            var bytes = reader.ReadBytes(count);
            if (bytes.Length < count)
                throw new EndOfStreamException();
            return bytes;
        }
    }
}