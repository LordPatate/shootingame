dotnet publish -c Release -r win-x64 --self-contained true
dotnet publish -c Release -r osx-x64 --self-contained true
dotnet publish -c Release -r linux-x64 --self-contained true

cd bin/Release/netcoreapp3.0
for ostype in linux win osx; do
    cd ${ostype}-x64
    zip -r shootingame-${ostype} publish > /dev/null && echo "zipped ${ostype} successfully"
    mv shootingame-${ostype}.zip ../../../../../../
    cd -
done
