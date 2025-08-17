#git reset --hard
git checkout develop
git pull
dotnet build
dotnet publish
# Copy all files
cp -r bin/Release/net7.0/publish/* 'C:\inetpub\wwwroot\XIN-Y-KIEN\btv-api.blueskytech.vn'
echo "Publish => DONE"
read -p "Press enter to exit"