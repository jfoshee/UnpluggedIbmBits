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
            if (ReferenceEquals(null, writer))
                throw new ArgumentNullException("writer");
            var bytes = IbmConverter.GetBytes(value);
            writer.Write(bytes);
        }
    }
}
