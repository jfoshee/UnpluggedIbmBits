# Legacy IBM Data Reading Library in C# #

[![MIT License](https://img.shields.io/badge/license-MIT-blue.svg)](./license.txt)
[![NuGet](https://img.shields.io/nuget/dt/Unplugged.IbmBits.svg)](https://www.nuget.org/packages/Unplugged.IbmBits/)
[![Build status](https://ci.appveyor.com/api/projects/status/e2x9pjs8t1tk3mhp?svg=true)](https://ci.appveyor.com/project/jfoshee/unpluggedibmbits)

Helps read/write and convert between legacy IBM System formats and .NET types. 
Includes IbmConverter class as well as BinaryReader & BinaryWriter extensions 
for EBCDIC string, Big Endian Int16, Big Endian Int32 and 
IBM System/360 single precision floating point format.

Issues welcome. Do not fold, spindle, or mutilate.

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

### Example of using BinaryWriter extension methods

```C#
    using (var stream = File.OpenWrite("punchcard.bin"))
    using (var writer = new BinaryWriter(stream))
    {
        writer.WriteEbcdic("Hello, World");
        writer.WriteIbmSingle(3.14f);
        writer.WriteBigEndian((Int16) 13);
        writer.WriteBigEndian((Int32) 54321);
    }
```
