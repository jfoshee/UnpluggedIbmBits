﻿using System;
using System.Collections;
using FluentAssertions;
using NUnit.Framework;

namespace Unplugged.IbmBits.Tests
{
    public class IbmConverterTest
    {
        #region ToString()

        const byte _comma = 0x6B;
        const byte _bang = 0x5A;
        const byte _H = 0xC8;
        const byte _e = 0x85;
        const byte _l = 0x93;
        const byte _o = 0x96;

        [Test]
        public void ShouldConvertFromEbcdicToUnicode()
        {
            // Arrange
            var bytes = new byte[] { _H, _e, _l, _l, _o, _comma };

            // Act
            string result = IbmConverter.ToString(bytes);

            // Assert
            result.Should().Be("Hello,");
        }

        [Test]
        public void ShouldConvertFromEbcdicToUnicode2()
        {
            // Arrange
            var bytes = new byte[] { _l, _o, _l, _bang };

            // Act
            string result = IbmConverter.ToString(bytes);

            // Assert
            result.Should().Be("lol!");
        }

        [Test]
        public void ShouldConvertToStringGivenStartingIndex()
        {
            // Arrange
            var bytes = new byte[] { _bang, _bang, _H, _e };
            var startingIndex = 2;

            // Act
            string result = IbmConverter.ToString(bytes, startingIndex);

            // Assert
            result.Should().Be("He");
        }

        [Test]
        public void ShouldConvertToStringGivenStartingIndex2()
        {
            // Arrange
            var bytes = new byte[] { _H, _e, _l, _l, _o };
            var startingIndex = 1;

            // Act
            string result = IbmConverter.ToString(bytes, startingIndex);

            // Assert
            result.Should().Be("ello");
        }

        [Test]
        public void ShouldConvertToStringGivenIndexAndLength()
        {
            // Arrange
            var bytes = new byte[] { _H, _o, _H, _o, _H, _o, _l, _e, _bang };
            var startingIndex = 4;
            var length = 4;

            // Act
            string result = IbmConverter.ToString(bytes, startingIndex, length);

            // Assert
            result.Should().Be("Hole");
        }

        [Test]
        public void ShouldConvertToStringGivenIndexAndLength2()
        {
            // Arrange
            var bytes = new byte[] { _H, _o, _H, _o, _H, _o, _l, _e, _bang };
            var startingIndex = 0;
            var length = 6;

            // Act
            string result = IbmConverter.ToString(bytes, startingIndex, length);

            // Assert
            result.Should().Be("HoHoHo");
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowWhenArgumentNullForToStringGivenBytes()
        {
            IbmConverter.ToString(null);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowWhenArgumentNullForToStringGivenBytesAndIndex()
        {
            IbmConverter.ToString(null, 1);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowWhenArgumentNullForToStringGivenBytesAndIndexAndLength()
        {
            IbmConverter.ToString(null, 1, 1);
        }

        #endregion

        #region GetBytes from String

        [Test]
        public void ShouldConvertFromUnicodeToEbcdic()
        {
            // Arrange
            var value = "Hello,";

            // Act
            byte[] result = IbmConverter.GetBytes(value);

            // Assert
            result.Should().Equal(new byte[] { _H, _e, _l, _l, _o, _comma });
        }

        [Test]
        public void ShouldConvertFromStringGivenStartingIndex()
        {
            // Arrange
            var value = "lol SHe!";
            var startingIndex = 5;

            // Act
            byte[] result = IbmConverter.GetBytes(value, startingIndex);

            // Assert
            result.Should().Equal(new byte[] { _H, _e, _bang });
        }

        [Test]
        public void ShouldConvertFromStringGivenStartingIndexAndLength()
        {
            // Arrange
            var value = "Hole in the ground";
            var startingIndex = 1;
            var length = 3;

            // Act
            byte[] result = IbmConverter.GetBytes(value, startingIndex, length);

            // Assert
            result.Should().Equal(new byte[] { _o, _l, _e });
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowWhenArgumentNullForGetBytesFromString()
        {
            IbmConverter.GetBytes(null, 1, 1);
        }

        #endregion

        #region ToInt16()

        [Test]
        public void ShouldConvertZeroInt16()
        {
            // Arrange
            var value = new byte[2];

            // Act
            Int16 result = IbmConverter.ToInt16(value);

            // Assert
            result.Should().Be(0);
        }

        [Test]
        public void ShouldConvertNegativeInt16()
        {
            // Arrange
            var value = new byte[] { 0xAB, 0xCD };

            // Act
            var result = IbmConverter.ToInt16(value);

            // Assert
            result.Should().Be(-21555);
        }

        [Test]
        public void ShouldIgnoreTrailingBytesInt16()
        {
            // Arrange
            var value = new byte[] { 0, 1, 99, 99 };

            // Act
            var result = IbmConverter.ToInt16(value);

            // Assert
            result.Should().Be(1);
        }

        [Test]
        public void ShouldConvertInt16WithStartIndex()
        {
            // Arrange
            var value = new byte[] { 99, 99, 99, 0, 1, 99 };
            var startIndex = 3;

            // Act
            Int16 result = IbmConverter.ToInt16(value, startIndex);

            // Assert
            result.Should().Be(1);
        }

        [Test]
        public void ShouldConvertAnotherInt16WithStartIndex()
        {
            // Arrange
            var value = new byte[] { 99, 99, 2, 0, 99 };
            var startIndex = 2;

            // Act
            Int16 result = IbmConverter.ToInt16(value, startIndex);

            // Assert
            result.Should().Be(512);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowWhenArgumentNullForToInt16()
        {
            IbmConverter.ToInt16(null);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowWhenArgumentNullForToInt16WithStartIndex()
        {
            IbmConverter.ToInt16(null, 1);
        }

        #endregion

        #region GetBytes from Int16

        [Test]
        public void ShouldConvertFromInt16()
        {
            // Arrange
            Int16 value = -21555;

            // Act
            byte[] result = IbmConverter.GetBytes(value);

            // Assert
            result.Should().Equal(new byte[] { 0xAB, 0xCD });
        }

        #endregion

        #region ToInt32()

        [Test]
        public void ShouldConvertZeroInt32()
        {
            // Arrange
            var bytes = new byte[4];

            // Act
            Int32 result = IbmConverter.ToInt32(bytes);

            // Assert
            result.Should().Be(0);
        }

        [Test]
        public void ShouldConvertNegativeInt32()
        {
            // Arrange
            var bytes = new byte[] { 0x89, 0xAB, 0xCD, 0xEF };

            // Act
            var result = IbmConverter.ToInt32(bytes);

            // Assert
            result.Should().Be(-1985229329);
        }

        [Test]
        public void ShouldIgnoreTrailingBytesInt32()
        {
            // Arrange
            var bytes = new byte[] { 0, 0, 0, 1, 99, 99 };

            // Act
            var result = IbmConverter.ToInt32(bytes);

            // Assert
            result.Should().Be(1);
        }

        [Test]
        public void ShouldConvertInt32WithStartIndex()
        {
            // Arrange
            var value = new byte[] { 99, 99, 99, 0, 0, 0, 1, 99 };
            var startIndex = 3;

            // Act
            Int32 result = IbmConverter.ToInt32(value, startIndex);

            // Assert
            result.Should().Be(1);
        }

        [Test]
        public void ShouldConvertAnotherInt32WithStartIndex()
        {
            // Arrange
            var value = new byte[] { 99, 99, 1, 2, 4, 0, 99 };
            var startIndex = 2;

            // Act
            var result = IbmConverter.ToInt32(value, startIndex);

            // Assert
            result.Should().Be(16909312);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowWhenArgumentNullForToInt32()
        {
            IbmConverter.ToInt32(null);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowWhenArgumentNullForToInt32WithStartIndex()
        {
            IbmConverter.ToInt32(null, 1);
        }

        #endregion

        #region GetBytes from Int32

        [Test]
        public void ShouldConvertFromInt32()
        {
            // Arrange
            Int32 value = -1985229329;

            // Act
            byte[] result = IbmConverter.GetBytes(value);

            // Assert
            result.Should().Equal(new byte[] { 0x89, 0xAB, 0xCD, 0xEF });
        }

        #endregion

        #region ToSingle()

        private static void VerifyToSingleReturns(float expected, byte[] value)
        {
            // Act
            float result = IbmConverter.ToSingle(value);

            // Assert
            result.Should().BeInRange(expected - 0.0001f, expected + 0.0001f);
        }

        [Test]
        public void ZeroShouldBeTheSame()
        {
            float expected = 0.0f;
            var bytes = BitConverter.GetBytes(expected);
            VerifyToSingleReturns(expected, bytes);
        }

        [Test]
        public void One()
        {
            var expected = 1f;
            var bytes = new byte[4];
            bytes[0] = 64 + 1; // 16^1 with bias of 64
            bytes[1] = 16;     // 16 to the right of the decimal 
            VerifyToSingleReturns(expected, bytes);
        }

        [Test]
        public void NegativeOne()
        {
            var expected = -1f;
            var bytes = new byte[4];
            bytes[0] = 128 + 64 + 1; // +128 for negative sign in first bit
            bytes[1] = 16;           // 16 to the right of the decimal 
            VerifyToSingleReturns(expected, bytes);
        }

        [Test]
        public void SampleValueFromSegy()
        {
            var bytes = new byte[] { 0xc0, 0x1f, 0xf4, 0x62 };
            VerifyToSingleReturns(-0.1248f, bytes);
        }

        [Test]
        public void SampleValueFromWikipediaToSingle()
        {
            VerifyToSingleReturns(_wikipediaSingle, GetWikipediaSampleBytes());
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowWhenArgumentNullForToSingle()
        {
            IbmConverter.ToSingle(null);
        }

        [Test]
        public void VerifyFractionBytesCanBeConvertedToInt32()
        {
            var fractionBytes = new byte[] { 255, 255, 255, 0 };
            var i = BitConverter.ToInt32(fractionBytes, 0);
            var u = BitConverter.ToUInt32(fractionBytes, 0);
            i.Should().Be((int)u);
        }

        #endregion

        #region GetBytes from Single

        [Test]
        public void BytesFromZero()
        {
            VerifySingleConversion(0);
        }

        [Test]
        public void BytesFromOne()
        {
            VerifySingleConversion(1f);
        }

        [Test]
        public void BytesFromNegativeOne()
        {
            VerifySingleConversion(-1f);
        }

        [Test]
        public void SampleValueToSegy()
        {
            VerifySingleConversion(-0.1248f);
        }

        [Test]
        public void SampleValueFromWikipediaToBytes()
        {
            IbmConverter.GetBytes(_wikipediaSingle).Should().Equal(GetWikipediaSampleBytes());
        }

        [Test]
        public void SingleConversionForRandomNumbers()
        {
            var random = new Random(51293);
            for (int i = 0; i < 1000; i++)
            {
                var value = (Single)(random.NextDouble() * 100);
                VerifySingleConversion(value);
                VerifySingleConversion(-value);
            }
        }

        private static void VerifySingleConversion(Single value)
        {
            byte[] result = IbmConverter.GetBytes(value);
            var reverseValue = IbmConverter.ToSingle(result);
            var epsilon = 0.0001f;
            reverseValue.Should().BeInRange(value - epsilon, value + epsilon);
        }

        float _wikipediaSingle = -118.625f;

        byte[] GetWikipediaSampleBytes()
        {
            // This test comes from the example described here: http://en.wikipedia.org/wiki/IBM_Floating_Point_Architecture#An_Example
            // The difference is the bits have to be reversed per byte because the highest order bit is on the right
            // 0100 0011 0110 1110 0000 0101 0000 0000
            var bools = new bool[] 
            {
                false, true, false, false,  false, false, true, true,  
                false, true, true, false,  true, true, true, false,  
                false, false, false, false,  false, true, false, true,  
                false, false, false, false,  false, false, false, false, 
            };
            var bits = new BitArray(bools);
            var bytes = new byte[4];
            bits.CopyTo(bytes, 0);
            return bytes;
        }

        #endregion
        // TODO: Support for running on Big Endian architecture

        #region ToDecimal

        [Test]
        public void DecimalZeroShouldBeTheSame()
        {
            var expected = (decimal) 0;
            var bytes = IbmConverter.GetBytes(expected);
            var result = IbmConverter.ToUnpackedDecimal(bytes, 2);

            result.Should().Be(expected);
        }

        [Test]
        public void DecimalShouldBeTheSame()
        {
            var expected = (decimal)123.45;
            var bytes = IbmConverter.GetBytes(expected);
            var result = IbmConverter.ToUnpackedDecimal(bytes, 2);

            result.Should().Be(expected);
        }

        #endregion
    }
}
