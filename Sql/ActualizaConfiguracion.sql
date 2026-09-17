-- FUNCTION: public.fnconfiguracion(character varying)

-- DROP FUNCTION public.fnconfiguracion(character varying);

CREATE OR REPLACE FUNCTION public.fnconfiguracion(
	data character varying)
    RETURNS TABLE(codigo integer, descripcion text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
	cparameters refcursor;
	codigo int;
	descripcion character varying(200);
BEGIN
	/*
	SELECT * FROM fnconfiguracion('[{}]');
	*/
	RAISE NOTICE 'Eliminando tabla temporal en caso de existir';
	DROP TABLE IF EXISTS tmpConfig;
	
	RAISE NOTICE 'Obteniendo los datos y generando tabla temporal';
	CREATE TEMP TABLE tmpConfig AS
	SELECT 	t.codigo, REPLACE(t.valor,chr(92),'\\') AS valor
	FROM 	json_to_recordset(data::json) AS 
			t(codigo INT, valor TEXT);
				
	RAISE NOTICE 'iniciando transaccion de actualización de parametros';
	BEGIN	
		RAISE NOTICE 'Verificación de la existencia de Parámetros';
		-- USAR ESTE QUERY PARA OBTENER LOS PARAMETROS ACTUALES, PERO ELIMINANDO EL ULTIMO "UNION"
		-- select 'SELECT '||confnid||' AS confnid, '''||confdsc|| '''::CHARACTER VARYING(200) AS confdsc UNION ' from tbConfParam order by confnid asc
		CREATE TEMP TABLE tmp_parametros(
			confnid INT,
			confdsc CHARACTER VARYING(200)
		);

		INSERT INTO tmp_parametros
		SELECT * 
		FROM (
			SELECT 1 AS confnid, 'Precios con IVA incluido'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 2 AS confnid, 'Valor del IVA'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 3 AS confnid, 'Usa Touch Scree'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 4 AS confnid, 'Cantidad de mesas'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 5 AS confnid, 'Envia correo al propietario en caso de eliminación'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 6 AS confnid, 'Servidor de correo'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 7 AS confnid, 'Puerto Smpt de correo'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 8 AS confnid, 'Usuario de correo'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 9 AS confnid, 'Contraseña de correo'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 10 AS confnid, 'Correo usa autenticación'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 11 AS confnid, 'Correo usa TLS'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 12 AS confnid, 'Ejecuta control de inventario'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 13 AS confnid, 'Cantidad de caracteres para el password'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 14 AS confnid, 'Correo 1'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 15 AS confnid, 'Copiar a correos'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 16 AS confnid, 'Correo 2'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 17 AS confnid, 'Correo 3'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 18 AS confnid, 'Correo 4'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 19 AS confnid, 'Habilita envío de email'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 20 AS confnid, 'Usa password numérico'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 21 AS confnid, 'Usa descuentos manuales'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 22 AS confnid, 'Tema para reportes'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 23 AS confnid, 'Cambio rápido entre facturación y pedidos'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 24 AS confnid, 'Inicio de secuencial de pedidos'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 25 AS confnid, 'Imprime hora en talonario de cocina'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 26 AS confnid, 'Cantidad de provincias'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 27 AS confnid, 'Ticket de cocina con 1=cantidad/producto, 0=producto/cantidad'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 28 AS confnid, 'Imprimir ticket para carrier automáticamente'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 29 AS confnid, 'Hora de inicio del Backup'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 30 AS confnid, 'Cantidad de Archivos de backup'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 31 AS confnid, 'Confirmación vía mail de realización del backup'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 32 AS confnid, 'Cantidad de lineas de alimentacion para impresión'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 33 AS confnid, 'Permitir upload archivos de backup'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 34 AS confnid, 'Url del servidor upload'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 35 AS confnid, 'Usuario server upload'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 36 AS confnid, 'Clave server upload'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 37 AS confnid, 'Hora inicio backup 2'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 38 AS confnid, 'Hora inicio backup 3'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 39 AS confnid, 'Modo Pedido-Facturación'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 40 AS confnid, 'Tipo botones para productos a usar'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 41 AS confnid, 'Usa Ventas por carrier en Cierre de caja'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 42 AS confnid, 'Cierre de caja Acumulativo'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 43 AS confnid, 'Ejecutar Backup'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 44 AS confnid, 'Ubicación Pg_dump'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 45 AS confnid, 'Organización de items en pedidos'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 46 AS confnid, 'Usa impresora proforma'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 47 AS confnid, 'Permite Enviar pedidos a parrilla'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 48 AS confnid, 'Formato numérico Reportes Excel'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 49 AS confnid, 'Permite Enviar Pedidos a Bar'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 50 AS confnid, 'Cantidad mesas Sala Alterna'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 51 AS confnid, 'Cantidad mesas Exterior'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 52 AS confnid, 'Cantidad mesas Terraza'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 53 AS confnid, 'Cantidad mesas 2do piso'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 54 AS confnid, 'Usa facturación electrónica'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 55 AS confnid, 'Envio al SRI Automático'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 56 AS confnid, 'Envío automático'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 57 AS confnid, 'Ruta archivo firma'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 58 AS confnid, 'Hora de inicio'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 59 AS confnid, 'Minutos para reintento'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 60 AS confnid, 'GUID de la empresa para Facturación Electronica'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 61 AS confnid, 'URL Treskel WebService'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 62 AS confnid, 'Usuario Treskel WebService'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 63 AS confnid, 'Clave Treskel WebService'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 64 AS confnid, 'Clave para la firma electrónica'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 65 AS confnid, 'Ruta Carpeta para grabar XML generados en Facturacion Electronica'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 66 AS confnid, 'Ruta imagen Logotipo Factura'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 67 AS confnid, 'Número de Facturero'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 68 AS confnid, 'Empresa modo RIMPE'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 69 AS confnid, 'Tamaño botones menú Pedido'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 70 AS confnid, 'Tamaño botones menú Facturas'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 71 AS confnid, 'Cantidad de caracteres para decimal Codigo Barras'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 72 AS confnid, 'Servicio de atención al cliente'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 73 AS confnid, 'Identificación del cálculo del servicio'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 74 AS confnid, 'Código a utilizar en Barcode para indicar producto por peso'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 75 AS confnid, 'Cantidad de dígitos para definir el código en un Barcode'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 76 AS confnid, 'Cantidad de dígitos de la parte entera del peso en Barcode'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 77 AS confnid, 'Tipo de código de barras'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 78 AS confnid, 'Impresión: Separador'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 79 AS confnid, 'Impresión: Cantidad de columnas'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 80 AS confnid, 'Impresión: Imprime Nombre de empresa'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 81 AS confnid, 'Impresión: Imprime Dirección de empresa'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 82 AS confnid, 'Impresión: Imprime RUC'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 83 AS confnid, 'Impresión: Usa Negrita en cabecera'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 84 AS confnid, 'Impresión: Alineación de la cabecera'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 85 AS confnid, 'Impresión: Imprime separador al final de la cabecera'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 86 AS confnid, 'Impresión: Imprime cabecera en negrita del contenido'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 87 AS confnid, 'Impresión: Lineas a saltar al final de los items'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 88 AS confnid, 'Impresión: Imprime separador al final de los items'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 89 AS confnid, 'Impresión: Cantidad de decimales a imprimir en los items'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 90 AS confnid, 'Impresión: Imprime la forma de pago'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 91 AS confnid, 'Impresión: Incluye información del mesero'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 92 AS confnid, 'Impresión: Incluye agradecimiento'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 93 AS confnid, 'Imprime ticket de cierre de caja'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 94 AS confnid, 'Validar compra existente en retenciones'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 95 AS confnid, 'Tipo de Impresora 0=Impacto, 1=Térmica'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 96 AS confnid, 'Imprime cabecera en Comandas'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 97 AS confnid, 'Cantidad de columnas en Grupo Menu Vertical'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 98 AS confnid, 'Documentos al crear al grabar Factura'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 99 AS confnid, 'Secuencial para Factura Electrónica'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 100 AS confnid, 'Secuencial para Retención Electrónica'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 101 AS confnid, 'Secuencial para Notas de Crédito Electrónica'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 102 AS confnid, 'Secuencial para Notas de Débito Electrónica'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 103 AS confnid, 'Incluye salto en cabecera comanda'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 104 AS confnid, 'Secuencial previo de Factura Electrónica'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 105 AS confnid, 'Secuencial previo de Retención Electrónica'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 106 AS confnid, 'Secuencial previo de Notas de Crédito Electrónica'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 107 AS confnid, 'Secuencial previo de Notas de Débito Electrónica'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 108 AS confnid, 'Imprimir clave de descarga en factura'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 109 AS confnid, 'Dirección URL de aplicación web'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 110 AS confnid, 'Imprime Comprobante de Retención'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 111 AS confnid, 'Usa Servicio API para envio de mail'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 112 AS confnid, 'Token del servicio API para envio de mail'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 113 AS confnid, 'Pantalla de Pedido/Venta Vertical u Horizonal'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 114 AS confnid, 'Puerto del Servidor FTP cloud'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 115 AS confnid, 'Contro sequencia Backup File'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 116 AS confnid, 'Correo institucional de la empresa'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 117 AS confnid, 'Precio de envase en pedidos para llevar: I = Incluido, E = Excluido del precio del Item'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 118 AS confnid, 'Precio del envase por unidad $##.##'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 119 AS confnid, 'Muestra Resumen de Facturas Eliminadas o Anuladas en Cierre de Caja'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 120 AS confnid, 'Usa saltos de linea entre impresión de items'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 121 AS confnid, 'Tipo Impresora Facturacion'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 122 AS confnid, 'Tipo Impresora Cocina'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 123 AS confnid, 'Tipo Impresora Parrilla'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 124 AS confnid, 'Tipo Impresora Bar'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 125 AS confnid, 'Marca Impresora Facturacion'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 126 AS confnid, 'Marca Impresora Cocina'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 127 AS confnid, 'Marca Impresora Parrilla'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 128 AS confnid, 'Marca Impresora Bar'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 129 AS confnid, 'Usa Espacios Facturacion'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 130 AS confnid, 'Usa Espacios Cocina'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 131 AS confnid, 'Usa Espacios Parrilla'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 132 AS confnid, 'Usa Espacios Bar'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 133 AS confnid, 'Cantidad de copias de factura a imprimir'::CHARACTER VARYING(200) AS confdsc UNION 
			SELECT 134 AS confnid, 'Aplica modo expandido en Cabecera (Empresa/Ruc)'::CHARACTER VARYING(200) AS confdsc UNION
			SELECT 135 AS confnid, 'Usa letra grande en COMANDA COCINA'::CHARACTER VARYING(200) AS confdsc UNION
			SELECT 136 AS confnid, 'Usa letra grande en COMANDA PARRILLA'::CHARACTER VARYING(200) AS confdsc UNION
			SELECT 137 AS confnid, 'Usa letra grande en COMANDA BAR'::CHARACTER VARYING(200) AS confdsc UNION
			SELECT 138 AS confnid, 'Permite imprimir comandas en la pantalla de Facturación de Market'::CHARACTER VARYING(200) AS confdsc UNION
			SELECT 139 AS confnid, 'Permite que aparezca la opción de seleccionar mesas en Market'::CHARACTER VARYING(200) AS confdsc
		) a;

		RAISE NOTICE 'Abriendo el cursor cparameters';
		OPEN cparameters FOR 
			SELECT * FROM tmp_parametros ORDER BY confnid;
		
		RAISE NOTICE 'recorriendo el cursor cparameters';
		LOOP
			FETCH NEXT FROM cparameters INTO codigo, descripcion;
			EXIT WHEN NOT FOUND;
			
			IF NOT EXISTS(SELECT 1 FROM tbConfParam WHERE confnid = codigo) THEN
				RAISE NOTICE 'SE PROCEDERÁ A CREAR UN PARAMETRO';
				RAISE NOTICE 'EL PARAMETRO CON CODIGO = % descripcion = %', codigo, descripcion;
				INSERT INTO tbConfParam (confnid, confdsc, confval, confatv) 
					VALUES (codigo, descripcion, encryptar(''), true);
					
				RAISE NOTICE 'EL PARAMETRO CON CODIGO = % HA SIDO CREADO', codigo;
			END IF;
		END LOOP;
		
		CLOSE cparameters;

		RAISE NOTICE 'Actualizando parámetros';
		UPDATE tbConfParam
		SET 	confVal = encryptar(A.valor)
		FROM (
			SELECT t.codigo, t.valor FROM tmpConfig t
		) A
		WHERE tbConfParam.confNid = A.codigo;
		
		RAISE NOTICE 'Antes de crear o actualizar la información';
		RAISE NOTICE 'Cantidad de registros n = %', (SELECT COUNT (*) FROM tbAdsiEmpr);
		IF EXISTS(SELECT * FROM tbAdsiEmpr) THEN		
			RAISE NOTICE 'Actualizando nombre de la empresa.';
			UPDATE 	tbAdsiEmpr
			SET		emprnbr = (SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9999);

			RAISE NOTICE 'Actualizando ruc de la empresa.';
			UPDATE 	tbAdsiEmpr
			SET		emprnid = (SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9998);

			RAISE NOTICE 'Actualizando validador de la empresa.';
			UPDATE 	tbAdsiEmpr
			SET		emprvld = (SELECT t.valor FROM tmpConfig t WHERE t.codigo = 9997);

			RAISE NOTICE 'Actualizando codigo de la empresa.';
			UPDATE 	tbAdsiEmpr
			SET		emprcrd = (SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9996);

			RAISE NOTICE 'Actualizando imagen de la empresa.';
			UPDATE 	tbAdsiEmpr
			SET		emprimg = (SELECT CASE t.valor
										WHEN null THEN null
										WHEN '*' THEN tbAdsiEmpr.emprimg
										ELSE decode(t.valor, 'base64')
									 END
							  FROM tmpConfig t WHERE t.codigo = 9995);

			RAISE NOTICE 'Actualizando direccion local de la empresa.';
			UPDATE 	tbAdsiEmpr
			SET		emprdirl = (SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9994);

			RAISE NOTICE 'Actualizando direcion matriz de la empresa.';
			UPDATE 	tbAdsiEmpr
			SET		emprdirm = (SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9993);

			RAISE NOTICE 'Actualizando codigo establecimiento de la empresa.';
			UPDATE 	tbAdsiEmpr
			SET		emprecod = (SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = '9992');

			RAISE NOTICE 'Actualizando razon social de la empresa.';
			UPDATE 	tbAdsiEmpr
			SET		emprrsoc = (SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9991);

			RAISE NOTICE 'Actualizando usa retencion de la empresa.';
			UPDATE 	tbAdsiEmpr
			SET		emprretc = (SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9990);

			RAISE NOTICE 'Actualizando usa contabilidad de la empresa. ';
			UPDATE 	tbAdsiEmpr
			SET		emprcont = (SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9989);
			
			RAISE NOTICE 'Actualizando Logotipo para Facturación electrónica.';
			UPDATE 	tbAdsiEmpr
			SET		emprlofc = (SELECT CASE t.valor
										WHEN null THEN null
										WHEN '*' THEN tbAdsiEmpr.emprlofc
										ELSE decode(t.valor, 'base64')
									 END
							  FROM tmpConfig t WHERE t.codigo = 9988);
		ELSE
			RAISE NOTICE 'Registrando datos de la empresa.';
			INSERT INTO tbAdsiEmpr (
				emprnbr
				,emprnid
				,emprvld
				,emprcrd
				,emprimg
				,emprdirl
				,emprdirm
				,emprecod
				,emprrsoc
				,emprretc
				,emprcont
				,emprlofc
			) VALUES (
				(SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9999),
				(SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9998),
				(SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9997),
				(SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9996),
				(SELECT decode(t.valor,'base64') FROM tmpConfig t WHERE t.codigo = 9995),
				(SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9994),
				(SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9993),
				(SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9992),
				(SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9991),
				(SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9990),
				(SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9989),
				(SELECT decode(t.valor,'base64') FROM tmpConfig t WHERE t.codigo = 9988)
			);
		END IF;
		
		RAISE NOTICE 'eliminando tabla temporal';
		--COMMIT;			
		DROP TABLE tmpConfig;
		DROP TABLE tmp_parametros;
		RAISE NOTICE 'finalizo correctamente y retornará mensaje';
		RETURN QUERY
			SELECT 1 AS codigo, 'Configuración grabada correctamente ' AS descripcion;			
	EXCEPTION WHEN OTHERS THEN
		DROP TABLE tmpConfig;
		RAISE NOTICE 'HUBO un error y realizará un rollback';
		RAISE NOTICE 'Codigo de error = %', SQLSTATE;
		--ROLLBACK;
		
		RETURN QUERY
			SELECT 0 AS codigo, 'Configuración no pudo ser grabada' AS descripcion;
	END;
END;
$BODY$;

ALTER FUNCTION public.fnconfiguracion(character varying)
    OWNER TO postgres;
