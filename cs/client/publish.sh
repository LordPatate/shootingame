emacs ../client/shootingame.csproj

dotnet publish -c Release -r win-x64 --self-contained true
dotnet publish -c Release -r osx-x64 --self-contained true
dotnet publish -c Release -r linux-x64 --self-contained true

BINDIR=cs/client/bin/Release/netcoreapp3.0
cd ../..
for ostype in linux win osx; do
    cd ${BINDIR}/${ostype}-x64
    zip -r shootingame-${ostype} publish > /dev/null && echo "zipped ${ostype} successfully"
    cd -
    mv ${BINDIR}/${ostype}-x64/shootingame-${ostype}.zip .
    zip shootingame-${ostype}.zip -r resources/
done
