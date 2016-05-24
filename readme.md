#EncodeDotNet

This library can be used to encode a long into a Base32 string as described by [Douglas Crockford](http://www.crockford.com/wrmg/base32.html).

Encoded string can optionally include a dash separator to make the string more readable (`1234-ABCD-9876`).

As described by Crockford, encoding will not use `I`, `L`, or `O` but decoding will accept these ass `1`, `1`, and `0`. 
Lower case letters will be decoded as their uppercase equivalent.

To encode a 63 bit value, we require 13 base 32 digits (5 bits per character). If we ignore the last 3 bits, we can encode the value in 12 characters.  Since the vast majority of values can fit within 12 digits, the encoding process will always produce a 12 digits value (optionally plus 2 separators) unless the upper bits are used.  Values greater than 
`0x0FFFFFFFFFFFFFFF` will result in 13 digits.

So
    0x0FFFFFFFFFFFFFFF => "ZZZZ-ZZZZ-ZZZZ"
while
    0x1FFFFFFFFFFFFFFF => "1ZZZZ-ZZZZ-ZZZZ"


The check symbol described by Crockford is not currently implemented. 

## Getting Started
Install the [Nuget package](https://www.nuget.org/packages/EncodeDotNet/) and write the following code:

```c#
using System;
using EncodeDotNet;

public class Program
{
    public static void Main(string[] args)
    {
        long number = 1234567890;
        string encodedString = Encode.ToBase32(number);
        Console.WriteLine("{0} encoded: {1}", number, encodedString);

        long backAgain = Encode.FromBase32(encodedString);
        Console.WriteLine("{0} decoded: {1}", encodedString, backAgain);
    }
}
```

<hr>

[![Build status](https://ci.appveyor.com/api/projects/status/g0k9pgiswxqd6lwq?svg=true)](https://ci.appveyor.com/project/joshclark/encodedotnet) <a href="https://www.nuget.org/packages/EncodeDotNet/"><img src="http://img.shields.io/nuget/v/EncodeDotNet.svg?style=flat-square" alt="NuGet version" height="18"></a> 

