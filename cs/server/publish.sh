for ostype in linux win osx; do
    echo "<RuntimeIdentifier>${ostype}-x64</RuntimeIdentifier>"
    emacs ../client/shootingame.csproj
    dotnet publish -c Release -r ${ostype}-x64 --self-contained true
done

cd bin/Release/netcoreapp3.0
for ostype in linux win osx; do
    cd ${ostype}-x64
    zip -r shootingame-server-${ostype} publish > /dev/null && echo "zipped ${ostype} successfully"
    mv shootingame-server-${ostype}.zip ../../../../../../

    cd -
done
