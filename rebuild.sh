branch="master"
while getopts b: args
do
    case "${args}" in
        b) branch=${OPTARG};;
    esac
done

systemctl daemon-reload
systemctl stop letstalk-api.service
systemctl status letstalk-api.service --no-pager
systemctl stop letstalk-auth.service
systemctl status letstalk-auth.service --no-pager
systemctl stop letstalk-notifications.service
systemctl status letstalk-notifications.service --no-pager
systemctl stop letstalk-linkpreview.service
systemctl status letstalk-linkpreview.service --no-pager
git reset --hard
git fetch origin
git checkout origin/${branch}
git pull origin ${branch}
cd LetsTalk.Server.API
dotnet ef database update
cd ..
rm -rf LetsTalk.Server.API/bin/Debug/net7.0/publish/*
rm -rf LetsTalk.Server.Authentication/bin/Debug/net7.0/publish/*
dotnet publish
cp ../LetsTalk.Private/config/applications/LetsTalk.Server.API.appsettings.json LetsTalk.Server.API/bin/Debug/net7.0/publish/appsettings.json
cp ../LetsTalk.Private/config/applications/LetsTalk.Server.Notifications.appsettings.json LetsTalk.Server.Notifications/bin/Debug/net7.0/publish/appsettings.json
systemctl start letstalk-api.service
systemctl status letstalk-api.service --no-pager
systemctl start letstalk-auth.service
systemctl status letstalk-auth.service --no-pager
systemctl start letstalk-notifications.service
systemctl status letstalk-notifications.service --no-pager
systemctl start letstalk-linkpreview.service
systemctl status letstalk-linkpreview.service --no-pager