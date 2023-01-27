namespace Unplugged.IbmBits.Tests;

public class BinaryReaderExtensionMethodsTest
{
    #region ReadStringEbcdic

    [Fact]
    public void ReadEbcdicShouldConsumeRequestedBytes()
    {
        var expected = 23;
        VerifyBytesConsumed(r => r.ReadStringEbcdic(expected), expected);
    }

    [Fact]
    public void ShouldConvertCharacters()
    {
        VerifyValueFromByteStream("J", r => r.ReadStringEbcdic(1), new byte[] { 0xD1 });
    }

    [Fact]
    public void ShouldThrowWhenArgumentNullForReadStringEbcdic()
    {
        Assert.Throws<ArgumentNullException>(() => (null as BinaryReader).ReadStringEbcdic(1));
    }

    #endregion

    #region ReadInt16BigEndian()

    [Fact]
    public void Int16ShouldConsume2Bytes()
    {
        VerifyBytesConsumed(r => r.ReadInt16BigEndian(), 2);
    }

    [Fact]
    public void Int16ShouldThrowIfNot2BytesInStream()
    {
        VerifyThrowsExceptionIfStreamTooShort(r => r.ReadInt16BigEndian(), 2);
    }

    [Fact]
    public void ShouldConvertToInt16()
    {
        VerifyValueFromByteStream(2, r => r.ReadInt16BigEndian(), new byte[] { 0, 2 });
    }

    [Fact]
    public void ShouldThrowWhenArgumentNullForReadInt16BigEndian()
    {
        Assert.Throws<ArgumentNullException>(() => (null as BinaryReader).ReadInt16BigEndian());
    }

    #endregion

    #region ReadInt32BigEndian()

    [Fact]
    public void Int32ShouldConsume4Bytes()
    {
        VerifyBytesConsumed(r => r.ReadInt32BigEndian(), 4);
    }

    [Fact]
    public void Int32ShouldThrowIfNot4BytesInStream()
    {
        VerifyThrowsExceptionIfStreamTooShort(r => r.ReadInt32BigEndian(), 4);
    }

    [Fact]
    public void ShouldConvertToInt32()
    {
        VerifyValueFromByteStream(3, r => r.ReadInt32BigEndian(), new byte[] { 0, 0, 0, 3 });
    }

    [Fact]
    public void ShouldThrowWhenArgumentNullForReadInt32BigEndian()
    {
        Assert.Throws<ArgumentNullException>(() => (null as BinaryReader).ReadInt32BigEndian());
    }

    #endregion

    #region ReadSingle()

    [Fact]
    public void SingleShouldConsume4Bytes()
    {
        VerifyBytesConsumed(r => r.ReadSingleIbm(), 4);
    }

    [Fact]
    public void SingleShouldThrowIfNot4BytesInStream()
    {
        VerifyThrowsExceptionIfStreamTooShort(r => r.ReadSingleIbm(), 4);
    }

    [Fact]
    public void ShouldThrowSameExceptionAsNativeMethods()
    {
        VerifyThrowsExceptionIfStreamTooShort(r => r.ReadSingle(), 4);
    }

    [Fact]
    public void ShouldConvertToSingle()
    {
        VerifyValueFromByteStream(1f, r => r.ReadSingleIbm(), new byte[] { 65, 16, 0, 0 });
    }

    [Fact]
    public void ShouldThrowWhenArgumentNullForReadSingleIbm()
    {
        Assert.Throws<ArgumentNullException>(() => (null as BinaryReader).ReadSingleIbm());
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

    private static void VerifyBytesConsumed(Action<BinaryReader> act, int expectedNumberOfBytes)
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
            action.Should().Throw<EndOfStreamException>();
            // TODO: This fails on iOS hardware because it uses JIT compilation
        }
    }
}
