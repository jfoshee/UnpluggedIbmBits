CALL    "%ProgramFiles(x86)%\Microsoft Visual Studio 10.0\VC\vcvarsall.bat" x86
MSBUILD windows.msbuild
MSTEST  /testcontainer:Unplugged.IbmBits.Tests\bin\Debug\Unplugged.IbmBits.Tests.dll
PAUSE
