# GoodsAccountingDemo
This web application helps shop assistance and managers to account goods, calculate profit and make appropriate reports.
We wrote this programm to demonstrate our programming skills and master the React framework.

Web site dev maket: https://lk.wilda.ru/pub/97ab9260c26b086af41775dcfe8701b1 <br />
Web application icon source https://www.flaticon.com/ <br />
Forbidden page I got there https://github.com/dr5hn/403/ <br />
Setup trusted certificate: dotnet dev-certs https --export-path "./server.crt" --no-password --format "Pem"  --trust <br />
Or you can run \Certificate\GenerateCertificates.ps1

Docker instruction for frontend developers:
    - Build backend docker image: in cmd run CreateImage.cmd from .\Backend\Source\
    - Build frontend docker image: in cmd run CreateImage.cmd from .\Frontend\
    - Run next command from root: docker-compose up -d
