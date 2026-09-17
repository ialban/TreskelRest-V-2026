DROP FUNCTION IF EXISTS fnMovimientoCaja;
CREATE OR REPLACE FUNCTION fnMovimientoCaja(IN accion VARCHAR, IN tipo VARCHAR, IN caja VARCHAR, IN secuencial VARCHAR,
										   IN fecha TIMESTAMP WITHOUT TIME ZONE, IN detalle VARCHAR, IN valor VARCHAR, IN usuario VARCHAR)
RETURNS TABLE(
	codigo INT,
	descripcion TEXT
)
AS $$
DECLARE
	fechaTra DATE := CURRENT_DATE;
	_secuencial INT;
	_tipo VARCHAR;
BEGIN
	IF accion = 'N' THEN
		BEGIN	
			RAISE NOTICE 'ANTES DE CREAR EL VALE';
			RAISE NOTICE 'FECHA = %', fechaTra;
			
			IF tipo = 'I' then			
				RAISE NOTICE 'CREANDO VALE DE INGRESO';
				RAISE NOTICE 'OBTENIENDO SECUENCIAL';
				
				SELECT LPAD(COALESCE(MAX(vaigSec::int)+1,1)::TEXT,10,'0') INTO _secuencial FROM tbCajaVaIg;
				
				RAISE NOTICE 'SECUENCIAL = %', _secuencial;
				_tipo := 'Ingreso';
				
				INSERT INTO tbCajaVaIg (vaigSec, vaigFch, cajaCod, vaigDet, vaigVal, usuaCod, vaigFgr)
					VALUES (_secuencial, fecha::DATE, caja, detalle, valor::NUMERIC(10,2), usuario, fechaTra);

				RAISE NOTICE 'VALE DE CAJA GRABADO CORRECTAMENTE';
			ELSE
				RAISE NOTICE 'CREANDO VALE DE EGRESO';
				RAISE NOTICE 'OBTENIENDO SECUENCIAL';
				
				SELECT LPAD(COALESCE(MAX(vaegSec::int)+1,1)::TEXT,10,'0') INTO _secuencial FROM tbCajaVaEg;
				
				RAISE NOTICE 'SECUENCIAL = %', _secuencial;
				_tipo := 'Egreso';
				
				INSERT INTO tbCajaVaEg (vaegSec, vaegFch, cajaCod, vaegDet, vaegVal, usuaCod, vaegFgr)
					VALUES (_secuencial, fecha::DATE, caja, detalle, valor::NUMERIC(10,2), usuario, fechaTra);

				RAISE NOTICE 'VALE DE CAJA GRABADO CORRECTAMENTE';
			END IF;

			RAISE NOTICE 'ANTES DE RETURN';
			RETURN QUERY
				SELECT 1 AS codigo, 'Vale de '|| _tipo ||' registrado correctamente' AS descripcion;			
		EXCEPTION WHEN OTHERS THEN
			RETURN QUERY
				SELECT 0 AS codigo, 'Vale de '|| _tipo ||' no pudo ser registrado' AS descripcion;
		END;
	END IF;
END;
$$
LANGUAGE 'plpgsql';