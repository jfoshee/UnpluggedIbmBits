using System;
using System.IO;

namespace Unplugged.IbmBits
{
    public static class BinaryWriterExtensionMethods
    {
        /// <summary>
        /// Writes the given Unicode string as an 8-bit EBCDIC encoded character string
        /// </summary>
        public static void WriteEbcdic(this BinaryWriter writer, string value)
        {
            var bytes = IbmConverter.GetBytes(value);
            writer.Write(bytes);
        }

        /// <summary>
        /// Writes a big endian encoded Int16 to the stream
        /// </summary>
        public static void WriteBigEndian(this BinaryWriter writer, Int16 value)
        {
            var bytes = IbmConverter.GetBytes(value);
            writer.Write(bytes);
        }

        /// <summary>
        /// Writes a big endian encoded Int32 to the stream
        /// </summary>
        public static void WriteBigEndian(this BinaryWriter writer, Int32 value)
        {
            var bytes = IbmConverter.GetBytes(value);
            writer.Write(bytes);
        }

        /// <summary>
        /// Writes an IBM System/360 Floating Point encoded Single to the stream
        /// </summary>
        public static void WriteIbmSingle(this BinaryWriter writer, Single value)
        {
            var bytes = IbmConverter.GetBytes(value);
            writer.Write(bytes);
        }
    }
}
