DROP FUNCTION IF EXISTS fnConfiguracion;
CREATE OR REPLACE FUNCTION fnConfiguracion( IN data VARCHAR)
RETURNS TABLE(
	codigo INT,
	descripcion TEXT
)
AS $$
DECLARE 
	
BEGIN
	
	RAISE NOTICE 'Eliminando tabla temporal en caso de existir';
	DROP TABLE IF EXISTS tmpConfig;
	
	RAISE NOTICE 'Obteniendo los datos y generando tabla temporal';
	CREATE TEMP TABLE tmpConfig AS
	SELECT 	t.codigo, t.valor AS valor
	FROM 	json_to_recordset(data::json) AS 
			t(codigo INT, valor TEXT);
				
	RAISE NOTICE 'iniciando transaccion de actualización de parametros';
	BEGIN	
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
			) VALUES (
				(SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9999),
				(SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9998),
				(SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9997),
				(SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9996),
				(SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9995),
				(SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9994),
				(SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9993),
				(SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9992),
				(SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9991),
				(SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9990),
				(SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9989)
			);
		END IF;
		
		RAISE NOTICE 'eliminando tabla temporal';
		--COMMIT;			
		DROP TABLE tmpConfig;
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
$$
LANGUAGE 'plpgsql';
