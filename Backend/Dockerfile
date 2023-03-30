FROM postgres
EXPOSE 5432

COPY ["./CreateDataBaseUnix.sql", "/storage/CreateDataBase.sql"]
COPY ["./CreateGoodsStates.sql", "/storage/CreateGoodsStates.sql"]
COPY ["./CreateGoodsTable.sql", "/storage/CreateGoodsTable.sql"]
COPY ["./CreateUsersTable.sql", "/storage/CreateUsersTable.sql"]
COPY ["./CreateWorkShiftTable.sql", "/storage/CreateWorkShiftTable.sql"]
COPY ["./InitDataBase.sh", "/storage/InitDataBase.sh"]

RUN apt-get update && apt-get -y dist-upgrade
RUN apt install -y netcat
CMD ["postgres"]
