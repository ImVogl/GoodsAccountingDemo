psql -U postgres -f .\CreateDataBase.sql
SET PGCLIENTENCODING=utf-8

psql -U postgres -d goods_account -a -f .\CreateGoodsTable.sql
psql -U postgres -d goods_account -a -f .\CreateUsersTable.sql
psql -U postgres -d goods_account -a -f .\CreateWorkShiftTable.sql
psql -U postgres -d goods_account -a -f .\CreateGoodsStates.sql