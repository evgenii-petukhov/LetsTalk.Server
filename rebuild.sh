systemctl stop letstalk.service
systemctl status letstalk.service --no-pager
git reset --hard
git checkout origin/master
git pull origin master
cd LetsTalk.Server.API
dotnet ef database update
cd ..
dotnet publish
systemctl start letstalk.service
systemctl status letstalk.service --no-pager
