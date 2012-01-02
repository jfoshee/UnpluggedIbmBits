# Legacy IBM Data Reading Library in C# #

Helps read and convert from legacy IBM System formats to .NET types. 
Includes converters and BinaryReader extensions for EBCDIC string, 
Big Endian Int16, Big Endian Int32 and 
IBM System/360 single precision floating point format.

Issues welcome.

### Example of using BinaryReader extension methods

```C#
using System.IO;
using Unplugged.IbmBits;
```

```C#
using (var stream = File.OpenRead("punchcard.bin"))
using (var reader = new BinaryReader(stream))
{
    string text = reader.ReadStringEbcdic(12);
    float f = reader.ReadSingleIbm();
    int i16 = reader.ReadInt16BigEndian();
    int i32 = reader.ReadInt32BigEndian();
}
```
