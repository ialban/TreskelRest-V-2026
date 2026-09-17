DROP FUNCTION IF EXISTS fnLogger;
CREATE OR REPLACE FUNCTION fnLogger( IN tipo VARCHAR, IN descrip VARCHAR, IN usuario VARCHAR, IN dispositivo VARCHAR)
RETURNS TABLE(
	codigo INT,
	descripcion TEXT
)
AS $$
DECLARE 
	loggScl INT;
BEGIN
	loggScl := 0;
	
	RAISE NOTICE 'Iniciando registro de actividad en el logger';
	BEGIN	
		RAISE NOTICE 'Obteniendo los datos ';
		
		SELECT COALESCE(MAX(loggNid),1) INTO loggScl FROM tbLogger; 
		
		INSERT INTO tbLogger 
		(loggNid, loggTip, loggDsc, loggFch, loggHra, loggUsr, loggOrg)
			VALUES 
		(loggScl, tipo, descrip, CURRENT_DATE, CURRENT_TIME, usuario, dispositivo);
		
		--COMMIT;			

		RAISE NOTICE 'Finalizó registro de actividad en el logger';
		RETURN QUERY
			SELECT 1 AS codigo, 'Actividad registrada correctamente ' AS descripcion;			
	EXCEPTION WHEN OTHERS THEN
		RAISE NOTICE 'HUBO un error y realizará un rollback';
		RAISE NOTICE 'Codio de error = %', SQLSTATE;
		--ROLLBACK;

		SELECT 0 AS codigo, 'Actividad no pudo ser registrada en el logger' AS descripcion;
	END;
	
END;
$$
LANGUAGE 'plpgsql';
