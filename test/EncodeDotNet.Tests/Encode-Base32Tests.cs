using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using EncodeDotNet;

namespace EncodeDotNet.Tests
{
    public class EncodeBase32Tests
    {

        [Theory, MemberData(nameof(InterestingNumbers))]
        public void EncodedValuesCanBeDecodedBackToOriginal(long number)
        {
            string withSeperator = Encode.ToBase32(number);
            string noSeperator = Encode.ToBase32(number, false);

            long actualWithSeperator = Encode.FromBase32(withSeperator);
            long actualNoSeperator = Encode.FromBase32(noSeperator);
            Assert.Equal(number, actualWithSeperator);
            Assert.Equal(number, actualNoSeperator);
        }

        [Theory, MemberData(nameof(InterestingStrings))]
        public void DecodedStringMatchesExpectValue(string input, long expectedValue)
        {
            long actual = Encode.FromBase32(input);
            Assert.Equal(expectedValue, actual);
        }

        [Theory, MemberData(nameof(CleanedUpInterestingStrings))]
        public void EncodedValueMatchesExpectedString(string expectedString, long input)
        {
            string actual= Encode.ToBase32(input);
            Assert.Equal(expectedString, actual);
        }

        [Theory]
        [InlineData(0, 12)]
        [InlineData(100000, 12)]
        [InlineData(0x0FFFFFFFFFFFFFFF, 12)]
        [InlineData(0x1FFFFFFFFFFFFFFF, 13)]
        [InlineData(0x7FFFFFFFFFFFFFFF, 13)]
        public void EncodingUses12CharsForAllButTheLargestNumbers(long number, int expectedLength)
        {
            string base32 = Encode.ToBase32(number, false);
            Assert.Equal(expectedLength, base32.Length);
        }

        [Fact]
        public void DecodingThrowsForTooBigANumber()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Encode.FromBase32("8ZZZZ-ZZZZ-ZZZZ"));
        }

        private static IEnumerable<object[]> InterestingNumbers()
        {
            var numbers = new[] {0, 1, 1000, 183317821147582464, long.MaxValue};
            return numbers.Select(x => new object[] { x });
        }

        private static IEnumerable<object[]> InterestingStrings()
        {
            return new[]
            {
                new object[] { "52TG-KSAW-0400", 183328850812342272 },
                new object[] { "ZZZZ-ZZZZ-ZZZZ", 0x0FFFFFFFFFFFFFFF },   // largest 12 char value
                new object[] { "1ZZZZ-ZZZZ-ZZZZ", 0x1FFFFFFFFFFFFFFF },  // 13 chars
                new object[] { "7ZZZZ-ZZZZ-ZZZZ", long.MaxValue },       // 13 chars
                new object[] { "1234-1101-ABCD", 38390583430294925 },    // normal characters
                new object[] { "1234-ILO1-ABCD", 38390583430294925 },    // similar looking characters
                new object[] { "1234-ilo1-abcd", 38390583430294925 }     // similar looking lowercase characters
            };
        }

        private static IEnumerable<object[]> CleanedUpInterestingStrings()
        {
            return InterestingStrings().Select(x => new[]
            {
                x[0].ToString().ToUpperInvariant()
                    .Replace("I", "1")
                    .Replace("L", "1")
                    .Replace("O", "0"),
                x[1]
            });
        }

    }
}
