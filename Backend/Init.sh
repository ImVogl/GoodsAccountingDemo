#!/bin/bash

apt-get update && apt-get -y dist-upgrade
apt install -y netcat

echo "Waiting PostreSQL to launch on 5432..."
count = 0
while [! nc -z localhost 5432] && [$count < 1000]; do   
  sleep 0.1
  $count = $count + 1
  # netstat -tunlp
done

echo "PostreSQL launched"
psql -U postgres -f /storage/CreateDataBase.sql
psql -U postgres -d goods_account -a -f /storage/CreateGoodsTable.sql
psql -U postgres -d goods_account -a -f /storage/CreateUsersTable.sql
