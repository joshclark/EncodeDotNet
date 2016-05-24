using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EncodeDotNet
{
    public static class Encode
    {
        private static readonly char[] _encodeMap = "0123456789ABCDEFGHJKMNPQRSTVWXYZ".ToArray();
        private static readonly byte[] _decodeMap = GenerateDecodeMap(_encodeMap);
        private const byte UnusedEntry = 0xFF;
        private const long Largest12DigitValue = 0x0FFFFFFFFFFFFFFF;

        private static byte[] GenerateDecodeMap(char[] encodeMap)
        {
            int size = 'z' + 1;
            var result = new byte[size];
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = UnusedEntry;
            }

            // Add a lookup for both upper and lower case letters
            for (byte i = 0; i < encodeMap.Length; ++i)
            {
                result[encodeMap[i]] = i;
                result[char.ToLowerInvariant(encodeMap[i])] = i;
            }

            // Special cases for characters that look like numbers.
            result['O'] = 0;
            result['o'] = 0;
            result['I'] = 1;
            result['i'] = 1;
            result['L'] = 1;
            result['l'] = 1;

            return result;
        }





        public static string ToBase32(long number)
        {
            return ToBase32(number, true);
        }
        public static string ToBase32(long number, bool includeSeperator)
        {
            int size = 12 + (includeSeperator ? 2 : 0);
            size += (number > Largest12DigitValue) ? 1 : 0;

            char[] result = new char[size];

            int index = size - 1;
            do
            {
                if (includeSeperator && ((index == 10) || (index == 4)))
                {
                    result[index--] = '-';
                }

                result[index--] = _encodeMap[number & 0x1F];
                number >>= 5;
            } while (index >= 0);

            var id = new string(result, 0, size);
            return id;
        }

        public static long FromBase32(string str)
        {
            if (str == null)
                throw new ArgumentNullException();

            const int baseForDecoding = 32;
            const int maxAllowedEncodedDigits = 12;


            long value = 0;
            long currentBase = 1;
            int encodedDigitCount = 0;

            for (int i = str.Length - 1; i >= 0; --i)
            {
                char currentDigit = str[i];
                if (currentDigit == '-')
                    continue;

                byte decodedValue = currentDigit < _decodeMap.Length ? _decodeMap[currentDigit] : UnusedEntry;

                if (decodedValue == UnusedEntry)
                    throw new FormatException($"Illegal character '{currentDigit}' in encoded string '{str}'");

                
                if ((encodedDigitCount > maxAllowedEncodedDigits) ||
                    ((encodedDigitCount == maxAllowedEncodedDigits) && (decodedValue > 7)))
                {
                    throw new ArgumentOutOfRangeException($"Encoded string '{str}' is too large. It won't fit in a long.");
                }

                value += decodedValue * currentBase;
                currentBase *= baseForDecoding;
                encodedDigitCount++;
            }

            return value;
        }

    }
}
