Write-Host "Creating https certificate"

$password = "TGwc04cUx0W764uFcw8Lug=="
$certificate = New-SelfSignedCertificate -certstorelocation cert:\localmachine\my -dnsname localhost
$securePassword = ConvertTo-SecureString -String $password -Force -AsPlainText

$pfxPath = "./server.pfx"
$outPath = "./server.pem"
Export-PfxCertificate -Cert $certificate -FilePath $pfxPath -Password $securePassword | Out-Null
Import-PfxCertificate -Password $securePassword -FilePath $pfxPath -CertStoreLocation Cert:\LocalMachine\Root | Out-Null

$keyPath = "./server.key"
$certPath = "./server.crt"

openssl pkcs12 -in $pfxPath -nocerts -out $keyPath -nodes -passin pass:$password
openssl pkcs12 -in $pfxPath -nokeys -out $certPath -nodes -passin pass:$password

$key = Get-Content -Path $keyPath
$cert = Get-Content -Path $certPath
$key + $cert | Out-File $outPath -Encoding ASCII

Write-Host "Https certificate written to $outPath"