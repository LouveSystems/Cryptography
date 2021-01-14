cd TrackMasters.Shared

dotnet publish -c Release -f net5.0
mkdir ..\Release\net5.0
copy bin\Release\net5.0\publish\LouveSystems.Cryptography.dll ..\Release\net5.0\.

dotnet publish -c Release -f net472
mkdir ..\Release\net472
copy bin\Release\net472\publish\LouveSystems.Cryptography.dll ..\Release\net472\.

PAUSE