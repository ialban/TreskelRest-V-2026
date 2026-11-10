# README #

Este README contiene información acerca del y los proyectos que forman parte de TreskelRest.
Este proyecto fue creado por Ignacio Albán Idrovo.

### What is this repository for? ###

* Quick summary
* Desarrollado en Visual Studio 2019
* Version: 3.0
* [Learn Markdown](https://bitbucket.org/tutorials/markdowndemo)

### How do I get set up? ###

* Summary of set up
* Configuration
* Dependencies
	* FoxLean
	* Npgsql
* Database configuration
* How to run tests
* Deployment instructions

### Contribution guidelines ###

* Writing tests
* Code review
* Other guidelines

### Who do I talk to? ###

* Repo owner or admin
* Other community or team contact

### Creación de Firma de Desarrollo
Ejecutar los siguientes comandos desde PowerShell en modo Administrador

1.- $password = ConvertTo-SecureString -String "Tr3sk3lS0ft" -Force -AsPlainText
2.- $cert = New-SelfSignedCertificate -Type CodeSigningCert -Subject "CN=TreskelSoft" -FriendlyName "Certificado TreskelSoft" -KeyUsage DigitalSignature -NotAfter (Get-Date).AddYears(10)
3.- Export-PfxCertificate -Cert $cert -FilePath "C:\Desarrollo\TreskelSoftV2026\TreskelRest-V-2026\CertificadoTreskelSoft.pfx" -Password $password
4.- Export-Certificate -Cert $cert -FilePath "C:\Desarrollo\TreskelSoftV2026\TreskelRest-V-2026\CertificadoTreskelSoft.cer"

### Importar la clave del certificado en Visual Studio
Abra la consola de comandos PowerShell Developer de Visual Studio, y ejecute

sn -i "C:\Desarrollo\TreskelSoftV2026\TreskelRest-V-2026\CertificadoTreskelSoft.pfx" VS_KEY_7D1A07EBA7626E65

Introduzca la Clave definida previamente (Tr3sk3lS0ft)

### Ejecutar el script generador de Archivo SNK
PowerShell -ExecutionPolicy Bypass -File .\crea_snk.ps1

### Uso del archivo .pfx con SignTool
Para automatizar el firmado de las dll es necesario agregar la siguiente sentencia

"C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64\signtool.exe" sign /fd SHA256 /f "C:\Desarrollo\TreskelSoftV2026\TreskelRest-V-2026\CertificadoTreskelSoft.pfx" /p "Tr3sk3lS0ft" /v "$(TargetPath)"

Teniendo en cuenta que las rutas del signtool y del certificado podrían variar dependiendo de la ubicación del proyecto.

Proyecto
  -> Propiedades
     -> Compilación
	    -> Eventos
		   Evento posterior a la compilación
		   
