branch="master"
while getopts b: args
do
    case "${args}" in
        b) branch=${OPTARG};;
    esac
done

systemctl stop letstalk-api.service
systemctl status letstalk-api.service --no-pager
git reset --hard
git fetch origin
git checkout origin/${branch}
git pull origin ${branch}
cd LetsTalk.Server.API
dotnet ef database update
cd ..
dotnet publish
systemctl start letstalk-api.service
systemctl status letstalk-api.service --no-pager
