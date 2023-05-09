psql -U postgres -f ..\Common\DropDataBase.sql
psql -U postgres -f ..\Common\CreateDataBase.sql
SET PGCLIENTENCODING=utf-8

psql -U postgres -d goods_account -a -f .\CreateGoodsTableEmpty.sql
psql -U postgres -d goods_account -a -f ..\Common\CreateUsersTable.sql
psql -U postgres -d goods_account -a -f .\CreateWorkShiftTableEmpty.sql
psql -U postgres -d goods_account -a -f .\CreateGoodsStatesEmpty.sql