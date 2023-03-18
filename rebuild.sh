branch="master"
while getopts b: args
do
    case "${args}" in
        b) branch=${OPTARG};;
    esac
done

systemctl stop letstalk-api.service
systemctl status letstalk-api.service --no-pager
systemctl stop letstalk-auth.service
systemctl status letstalk-auth.service --no-pager
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
systemctl start letstalk-api.service
systemctl status letstalk-api.service --no-pager
systemctl start letstalk-auth.service
systemctl status letstalk-auth.service --no-pager
