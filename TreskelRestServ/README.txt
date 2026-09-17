------------------------------------------------------------------------------------
-- PROCESO PARA PONER EN FUNCIONAMIENTO EL SERVICIO TreskelRestServ EN EL SERVIDOR
-- DE BASE DE DATOS.
------------------------------------------------------------------------------------
1.- Ubicar el programa con todas sus librerías en el ruta C:\TreskelSoft
    Si esta ruta no existe, proceda a crearla.
2.- Antes de poner a ejecutar el programa, verificar que exista el archivo config.xml
3.- Editar y modificar los parámetros del archivo acorde con la información necesaria
    para el correcto funcionamiento. No debería tener que modificarse la información, 
    puesto que el archivo tiene la información general con la que funciona el sistema

    La información que deberia presente en el archivo es la siguiente:
    <?xml version="1.0" standalone="yes"?>
    <dsConfig xmlns="dsConfig.xsd">
        <SysConfig>
            <usaTouch>false</usaTouch>
            <printerFactura />
            <printerProforma />
            <printerCocina />
            <caja />
        </SysConfig>
        <Database>
            <server>localhost</server>
            <base>trsklrest</base>
            <puerto>5432</puerto>
            <usuario>C9xGFv/ouGb5dgerC8zy0A==</usuario>
            <clave>lDOBxJw0aZoQb0F7fESY2g==</clave>
        </Database>
    </dsConfig>
4.- Ejecutar el programa, para ello es necesario ingresar a la ruta mencionada al 
    inicio (en el punto 1), mediante el comando:
    cd c:\TreskelSoft
5.- Ejecutar el programa mediante el comando:
    .\TreskelService.exe
    .\TreskelRestServ.exe
6.- Para hacer que el programa se inicie cada vez que el sistema operativo inicie, se
    debe realizar lo siguiente
    - Ingrese al directorio c:\TreskelSoft
    - Cree un acceso directo al programa TreskelService.exe
    - Pulsa las teclas Windows + R. Seguidamente tienes que escribir shell:startup
    - Ubicar en el folder el acceso directo a la aplicación TreskelService.exe

------------------------------------------------------------------------------------
-- VERIFICACIÓN DE FUNCIONAMIENTO DEL PROGRAMA DE SERVICIO TreskelRestServ
------------------------------------------------------------------------------------
1.- Iniciamos la ventana de consola de comandos de windows o PowerShell
2.- Ejecutar la sentencia: 
        tasklist | findstr /I TreskelRestServ
    El resultado debería ser similar al mostrado a continuación, en caso de estar el
    programa en ejecución.
        TreskelRestServ.exe             792 Console                    1     34,660 K
    Donde el segundo dato, en este caso el numero 792 representa el PID de la apli-
    cacion, es decir el Identificador del programa, este valor es importante puesto 
    que servirá en caso de querer finalizarlo.

------------------------------------------------------------------------------------
-- FINALIZACIÓN DEL SERVICIO TreskelRestServ
------------------------------------------------------------------------------------
1.- Verifique si la aplicación se encuentra en ejecución, mire los pasos anteriores
2.- Identifique el PID del programa
3.- Ejecutar la sentencia 

    taskkill /pid <Numero_PID>

    Reemplace <Numero_PID> con el valor correspondiente a la aplicación que desea 
    finalizar, ejemplo:

    taskkill /pid 792

    En caso de no funcionar puede usar la sentencia

    taskkill /F /PID <Numero_PID>
