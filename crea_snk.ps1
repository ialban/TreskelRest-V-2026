# 1. Cargar el certificado PFX en memoria con su contraseña
$pfxPath = "C:\Desarrollo\TreskelSoftV2026\TreskelRest-V-2026\CertificadoTreskelSoft.pfx"
$password = "Tr3sk3lS0ft"
$cert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($pfxPath, $password, [System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]::Exportable)

# 2. Extraer los componentes de la clave privada/pública (Blob CSP)
$privateKey = [System.Security.Cryptography.RSACryptoServiceProvider]$cert.PrivateKey
$snkBytes = $privateKey.ExportCspBlob($true)

# 3. Guardar el archivo .snk limpio en tu disco duro
$snkPath = "C:\Desarrollo\TreskelSoftV2026\TreskelRest-V-2026\CertificadoTreskelSoft.snk"
[System.IO.File]::WriteAllBytes($snkPath, $snkBytes)

Write-Host "¡Archivo .snk generado con éxito en $snkPath!" -ForegroundColor Green
