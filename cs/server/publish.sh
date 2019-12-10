for ostype in linux win osx; do
    echo "<RuntimeIdentifier>${ostype}-x64</RuntimeIdentifier>"
    emacs ../client/shootingame.csproj
    dotnet publish -c Release -r ${ostype}-x64 --self-contained true
done

BINDIR=cs/server/bin/Release/netcoreapp3.0
cd ../..
for ostype in linux win osx; do
    cd ${BINDIR}/${ostype}-x64
    zip -r shootingame-server-${ostype} publish > /dev/null && echo "zipped ${ostype} successfully"
    cd -
    mv ${BINDIR}/${ostype}-x64/shootingame-server-${ostype}.zip .
    zip shootingame-server-${ostype}.zip -r levels
done
