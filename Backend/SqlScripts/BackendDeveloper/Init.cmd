psql -U postgres -f ..\Common\DropDataBase.sql
psql -U postgres -f ..\Common\CreateDataBase.sql
SET PGCLIENTENCODING=utf-8

psql -U postgres -d goods_account -a -f .\CreateGoodsTable.sql
psql -U postgres -d goods_account -a -f ..\Common\CreateUsersTable.sql
psql -U postgres -d goods_account -a -f .\CreateWorkShiftTable.sql
psql -U postgres -d goods_account -a -f .\CreateGoodsStates.sql