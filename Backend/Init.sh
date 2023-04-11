#!/bin/bash

apt-get update && apt-get -y dist-upgrade
apt install -y netcat

echo "Waiting PostreSQL to launch on 5432..."
count=0
port=5432
temp_file=/tmp/init_postgres
rm -rf $temp_file || true
mknod $temp_file p
netcat -z -v localhost $port > $temp_file &
grep -o 'succeeded!' $temp_file -c
listening=$?
echo $listening
while (( $listening == 0 )) && (( $count < 1000 ))
do   
    sleep 0.1
    ((count++))
    netcat -z -v localhost $port > $temp_file &
    grep -o 'succeeded!' $temp_file -c
    listening=$?
done

echo "PostreSQL launched"
psql -U postgres -f CreateDataBaseUnix.sql
psql -U postgres -d goods_account -a -f CreateGoodsTableEmpty.sql
psql -U postgres -d goods_account -a -f CreateUsersTable.sql
psql -U postgres -d goods_account -a -f CreateWorkShiftTableEmpty.sql
psql -U postgres -d goods_account -a -f CreateGoodsStatesEmpty.sql
