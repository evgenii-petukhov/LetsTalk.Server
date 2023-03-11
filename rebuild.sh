branch="master"
while getopts b: args
do
    case "${args}" in
        b) branch=${OPTARG};;
    esac
done

systemctl stop letstalk.service
systemctl status letstalk.service --no-pager
git reset --hard
git fetch origin
git checkout origin/${branch}
git pull origin ${branch}
cd LetsTalk.Server.API
dotnet ef database update
cd ..
dotnet publish
systemctl start letstalk.service
systemctl status letstalk.service --no-pager
