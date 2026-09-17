DROP FUNCTION IF EXISTS fnAjusInv;
CREATE OR REPLACE FUNCTION fnAjusInv(IN codProd VARCHAR, IN undProd VARCHAR, IN cntProd DOUBLE PRECISION, IN tipo VARCHAR)
RETURNS TABLE(
	codigo INT,
	descripcion TEXT
)
AS $$
DECLARE
	fecha DATE := CURRENT_DATE;
	invscl INT;
BEGIN
	BEGIN	
		RAISE NOTICE 'ANTES DE INSERT';
		RAISE NOTICE 'FECHA = %', fecha;
		
		SELECT COALESCE(MAX(invtscl)+1,1) INTO invscl FROM tbIvtrDInvMT;
		
		RAISE NOTICE 'EL SECUENCIAL = %', invscl;
		
		INSERT INTO tbIvtrDInvMT
			(invtscl, prdcod, dinvdsc, dinvtip, dinvfch, dinvcnt, dinvund, itematv)
			VALUES 
			(invscl, codProd, 'Ajuste de inventario', tipo, fecha, cntProd::NUMERIC(10,2), undProd, true);
		
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