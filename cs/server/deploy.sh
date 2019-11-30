if [ $# -eq 0 ]; then
    SERVER="ubuntu@185.22.98.73:~/server"
else
    SERVER="$1"
fi
if dotnet build -c Release -r linux-x64; then
    echo "Copying bin/Release/netcoreapp3.0 into $SERVER"
    scp -q -i ~/.ssh/id_rsa.pub -r bin/Release/netcoreapp3.0 "$SERVER" && echo "Done"
fi
