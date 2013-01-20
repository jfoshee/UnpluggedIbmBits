using System;
using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unplugged.IbmBits.Tests
{
    [TestClass]
    public class BinaryWriterExtensionMethodsTest
    {
        #region WriteStringEbcdic

        [TestMethod]
        public void WriteEbcdicShouldWriteConvertedBytes()
        {
            var value = "Nope";
            var expected = new byte[] { 0xD5, 0x96, 0x97, 0x85 };
            VerifyBytesWritten(w => w.WriteEbcdic(value), expected);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowWhenArgumentNullForWriteEbcdic()
        {
            (null as BinaryWriter).WriteEbcdic(" ");
        }

        #endregion

        public static void VerifyBytesWritten(Action<BinaryWriter> act, byte[] expected)
        {
            var bytes = new byte[expected.Length];
            using (var stream = new MemoryStream(bytes))
            using (var writer = new BinaryWriter(stream))
            {
                // Act
                act(writer);

                // Assert
                stream.Position.Should().Be(expected.Length, "Wrong number of bytes were written.");
            }
            bytes.Should().Equal(expected);
        }

    }
}
