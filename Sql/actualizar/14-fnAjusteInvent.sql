INSERT INTO tbConfParam VALUES( 33, 'Permitir upload archivos de backup', encryptar('0'),true);
INSERT INTO tbConfParam VALUES( 34, 'Url del servidor upload', encryptar(''),true);
INSERT INTO tbConfParam VALUES( 35, 'Usuario server upload', encryptar(''),true);
INSERT INTO tbConfParam VALUES( 36, 'Clave server upload', encryptar(''),true);

INSERT INTO tbConfParam VALUES( 37, 'Hora inicio backup 2', encryptar(''),true);
INSERT INTO tbConfParam VALUES( 38, 'Hora inicio backup 3', encryptar(''),true);

INSERT INTO tbAdsiCrrie VALUES('0007','PEDIDOS YA','PEY',10,TRUE,TRUE);

INSERT INTO tbAdsiMtvos VALUES('0203','Anulación solicitada por cliente','A',true);

DROP FUNCTION IF EXISTS fnAjusInv;
CREATE OR REPLACE FUNCTION fnAjusInv(IN codProd VARCHAR, IN undProd VARCHAR, IN cntProd DOUBLE PRECISION, IN tipo VARCHAR)
RETURNS TABLE(
	codigo INT,
	descripcion TEXT
)
AS $$
DECLARE
	fecha DATE := CURRENT_DATE;
BEGIN
	BEGIN	
		RAISE NOTICE 'ANTES DE INSERT';
		RAISE NOTICE 'FECHA = %', fecha;
		INSERT INTO tbIvtrDInvMT
			VALUES (codProd, 'Ajuste de inventario', tipo, fecha, cntProd::NUMERIC(10,2), undProd);
		
		RAISE NOTICE 'ANTES DE KARDEX';
		IF tipo = 'I' THEN
			RAISE NOTICE 'KARDEX INGRESO';
			call fnKardex(codProd,EXTRACT(YEAR FROM fecha)::int,EXTRACT(MONTH FROM fecha)::int,cntProd::NUMERIC(10,2),0);
		ELSE
			RAISE NOTICE 'KARDEX EGRESO';
			call fnKardex(codProd,EXTRACT(YEAR FROM fecha)::int,EXTRACT(MONTH FROM fecha)::int,0,cntProd::NUMERIC(10,2));
		END IF;
						
		RAISE NOTICE 'ANTES DE RETURN';
		RETURN QUERY
			SELECT 1 AS codigo, 'Ajuste de inventario realizado correctamente' AS descripcion;			
	EXCEPTION WHEN OTHERS THEN
		RETURN QUERY
			SELECT 0 AS codigo, 'El ajuste de inventario no pudo ser realizado' AS descripcion;
	END;
END;
$$
LANGUAGE 'plpgsql';