git reset --hard
git checkout develop
git pull
npm install
npm run build
cp -r dist/web-gui/* 'C:\inetpub\wwwroot\XIN_YKIEN_BTV\btv.blueskytech.vn'

echo "Publish => DONE"
read -p "Press enter to exit"