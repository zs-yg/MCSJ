dotnet publish MCSJ.csproj -c Release -r win-x86 --self-contained false /p:Optimize=true /p:DebugType=None
dotnet publish MCSJ.csproj -c Release -r win-x64 --self-contained false /p:Optimize=true /p:DebugType=None
cd bin\Release\net8.0
ren win-x64 MCSJ-x64
ren win-x86 MCSJ-x86
rmdir /s /q MCSJ-x64\publish
rmdir /s /q MCSJ-x86\publish
7z a -tzip MCSJ-x64.zip MCSJ-x64
7z a -tzip MCSJ-x86.zip MCSJ-x86