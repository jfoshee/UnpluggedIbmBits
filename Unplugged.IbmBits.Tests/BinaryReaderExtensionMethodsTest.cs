using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Unplugged.IbmBits.Tests
{
    [TestClass]
    public class BinaryReaderExtensionMethodsTest
    {
        #region ReadStringEbcdic

        [TestMethod]
        public void ReadEbcdicShouldConsumeRequestedBytes()
        {
            var expected = 23;
            VerifyBytesConsumed(r => r.ReadStringEbcdic(expected), expected);
        }

        [TestMethod]
        public void ShouldConvertCharacters()
        {
            VerifyValueFromByteStream("J", r => r.ReadStringEbcdic(1), new byte[] { 0xD1 });
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowWhenArgumentNullForReadStringEbcdic()
        {
            (null as BinaryReader).ReadStringEbcdic(1);
        }

        #endregion

        #region ReadInt16BigEndian()

        [TestMethod]
        public void Int16ShouldConsume2Bytes()
        {
            VerifyBytesConsumed(r => r.ReadInt16BigEndian(), 2);
        }

        [TestMethod]
        public void Int16ShouldThrowIfNot2BytesInStream()
        {
            VerifyThrowsExceptionIfStreamTooShort(r => r.ReadInt16BigEndian(), 2);
        }

        [TestMethod]
        public void ShouldConvertToInt16()
        {
            VerifyValueFromByteStream(2, r => r.ReadInt16BigEndian(), new byte[] { 0, 2 });
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowWhenArgumentNullForReadInt16BigEndian()
        {
            (null as BinaryReader).ReadInt16BigEndian();
        }

        #endregion

        #region ReadInt32BigEndian()

        [TestMethod]
        public void Int32ShouldConsume4Bytes()
        {
            VerifyBytesConsumed(r => r.ReadInt32BigEndian(), 4);
        }

        [TestMethod]
        public void Int32ShouldThrowIfNot4BytesInStream()
        {
            VerifyThrowsExceptionIfStreamTooShort(r => r.ReadInt32BigEndian(), 4);
        }

        [TestMethod]
        public void ShouldConvertToInt32()
        {
            VerifyValueFromByteStream(3, r => r.ReadInt32BigEndian(), new byte[] { 0, 0, 0, 3 });
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowWhenArgumentNullForReadInt32BigEndian()
        {
            (null as BinaryReader).ReadInt32BigEndian();
        }

        #endregion

        #region ReadSingle()

        [TestMethod]
        public void SingleShouldConsume4Bytes()
        {
            VerifyBytesConsumed(r => r.ReadSingleIbm(), 4);
        }

        [TestMethod]
        public void SingleShouldThrowIfNot4BytesInStream()
        {
            VerifyThrowsExceptionIfStreamTooShort(r => r.ReadSingleIbm(), 4);
        }

        [TestMethod]
        public void ShouldThrowSameExceptionAsNativeMethods()
        {
            VerifyThrowsExceptionIfStreamTooShort(r => r.ReadSingle(), 4);
        }

        [TestMethod]
        public void ShouldConvertToSingle()
        {
            VerifyValueFromByteStream(1f, r => r.ReadSingleIbm(), new byte[] { 65, 16, 0, 0 });
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowWhenArgumentNullForReadSingleIbm()
        {
            (null as BinaryReader).ReadSingleIbm();
        }

        #endregion

        private static void VerifyValueFromByteStream<T>(T expected, Func<BinaryReader, T> act, byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            using (var reader = new BinaryReader(stream))
            {
                // Act
                T actual = act(reader);

                // Assert
                actual.Should().Be(expected);
            }
        }

        // TODO: Move VerifyBytesConsumed to TDD lib
        public static void VerifyBytesConsumed(Action<BinaryReader> act, int expectedNumberOfBytes)
        {
            var bytes = new byte[2 * expectedNumberOfBytes];
            using (var stream = new MemoryStream(bytes))
            using (var reader = new BinaryReader(stream))
            {
                // Act
                act(reader);

                // Assert
                stream.Position.Should().Be(expectedNumberOfBytes, "Wrong number of bytes were consumed.");
            }
        }

        private static void VerifyThrowsExceptionIfStreamTooShort(Action<BinaryReader> act, int requiredNumberOfBytes)
        {
            // Arrange
            var bytes = new byte[requiredNumberOfBytes - 1];
            using (var stream = new MemoryStream(bytes))
            using (var reader = new BinaryReader(stream))
            {
                Action action = () => act(reader);
                action.ShouldThrow<EndOfStreamException>();
            }
        }
    }
}
