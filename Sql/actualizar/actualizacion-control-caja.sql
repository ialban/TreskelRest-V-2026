CREATE OR REPLACE FUNCTION public.fncontrolcaja(
	tipo character varying,
	caja character varying,
	secuencial character varying,
	fecha character varying,
	hora character varying,
	valor character varying,
	sistema character varying,
	diferencia character varying,
	efectivo character varying,
	tarjeta character varying,
	cheque character varying)
    RETURNS TABLE(codigo integer, descripcion text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
	_secuencial INT;	
	_cierreAcum INT;
	_fecha TIMESTAMP WITHOUT TIME ZONE;
	_fchIn TIMESTAMP WITHOUT TIME ZONE;
	_fchFn TIMESTAMP WITHOUT TIME ZONE;
BEGIN
	/*
	select * from fnControlCaja('I', '0001', '', '2023-01-22', '16:23', '60', '', '', '', '', '')
	*/
	SELECT desencryptar(confVal) INTO _cierreAcum FROM tbConfParam WHERE confNid = 42;
	
	IF hora::TIME > '00:00'::TIME AND hora::TIME < '04:01'::TIME THEN
		_fecha := fecha::DATE + INTERVAL '-1 day';
		
		_fchIn := (_fecha + INTERVAL '+7 hour')::TIMESTAMP WITHOUT TIME ZONE;
		_fchFn := (fecha || ' 04:59:59')::TIMESTAMP WITHOUT TIME ZONE;
	ELSE
		_fecha := fecha::DATE + INTERVAL '+1 day';
		
		_fchIn := (fecha || ' 07:00:00')::TIMESTAMP WITHOUT TIME ZONE;
		_fchFn := (_fecha + INTERVAL '+5 hour')::TIMESTAMP WITHOUT TIME ZONE;
	END IF;
	
	IF tipo = 'I' THEN
		BEGIN	
			RAISE NOTICE 'CREANDO INICIO DE CAJA';
			RAISE NOTICE 'OBTENIENDO SECUENCIAL';
			IF _cierreAcum = 1 THEN
				RAISE NOTICE 'CIERRE ACUMULATIVO';
				SELECT COALESCE(MAX(ctrlSec::int)+1,1) INTO _secuencial FROM tbCtrlCierre;

				RAISE NOTICE 'SECUENCIAL = %', _secuencial;	
				
				IF NOT EXISTS(SELECT * FROM tbCtrlCierre WHERE ctrlFchIni = fecha::DATE) THEN
					RAISE NOTICE 'ANTES DE GRABAR EL INICIO DE CAJA';
					
					INSERT INTO tbCtrlCierre (ctrlSec, ctrlFchIni, ctrlHin, ctrlVin)
						VALUES (LPAD(_secuencial::TEXT,10,'0'), fecha::DATE, hora::TIME, valor::NUMERIC(10,2));

					RAISE NOTICE 'INGRESO CREADO CORRECTAMENTE';

					RETURN QUERY
					SELECT 1 AS codigo, 'Inicio de caja registrado correctamente' AS descripcion;
				ELSE
					RAISE NOTICE 'YA HABIA UN INGRESO DE CAJA REGISTRADO PREVIAMENTE';

					RETURN QUERY
					SELECT 0 AS codigo, 'Ya existe un inicio de caja registrado para el '|| fecha AS descripcion;
				END IF;
				
			ELSE
				RAISE NOTICE 'CIERRE POR CAJA';
				SELECT COALESCE(MAX(ctrlSec::int)+1,1) INTO _secuencial FROM tbCtrlCaja;

				RAISE NOTICE 'SECUENCIAL = %', _secuencial;			

				IF NOT EXISTS(SELECT * FROM tbCtrlCaja WHERE ctrlFchIni = fecha::DATE AND cajaCod=caja) THEN
					RAISE NOTICE 'ANTES DE GRABAR EL INICIO DE CAJA';

					INSERT INTO tbCtrlCaja (ctrlSec, ctrlFchIni, cajaCod, ctrlHin, ctrlVin)
						VALUES (LPAD(_secuencial::TEXT,10,'0'), fecha::DATE, caja, hora::TIME, valor::NUMERIC(10,2));

					RAISE NOTICE 'INGRESO CREADO CORRECTAMENTE';

					RETURN QUERY
					SELECT 1 AS codigo, 'Inicio de caja registrado correctamente' AS descripcion;
				ELSE
					RAISE NOTICE 'YA HABIA UN INGRESO DE CAJA REGISTRADO PREVIAMENTE';

					RETURN QUERY
					SELECT 0 AS codigo, 'Ya existe un inicio de caja registrado para el '|| fecha AS descripcion;
				END IF;
			END IF;
			
		EXCEPTION WHEN OTHERS THEN
			RAISE NOTICE 'Codigo de error = %', SQLSTATE;
			RETURN QUERY
				SELECT 0 AS codigo, 'Inicio de caja no pudo ser registrado' AS descripcion;
		END;
	ELSE
		BEGIN
			RAISE  NOTICE 'iniciando cierre de caja';
			IF _cierreAcum = 1 THEN
				RAISE NOTICE 'Cierre de caja acumulado';
				IF NOT EXISTS(SELECT * 
							  FROM 	tbCtrlCierre 
							  WHERE (ctrlFchIni BETWEEN _fchIn AND _fchFn) --= fecha::DATE 
							  		AND ctrlFchFin IS NOT NULL 
							  		AND ctrlHfn IS NOT NULL) THEN
					RAISE NOTICE 'Fecha Inicio = %, Fecha Fin = %', _fchIn::varchar(20), _fchFn::varchar(20);
					RAISE NOTICE 'Secuencial= %', secuencial;
					
					UPDATE tbCtrlCierre 
					SET 	ctrlFchFin=fecha::DATE,
							ctrlHfn=hora::TIME,
							ctrlVcr=valor::NUMERIC(10,2),
							ctrlefc=efectivo::NUMERIC(10,2), 
							ctrltrj=tarjeta::NUMERIC(10,2), 
							ctrlchq=cheque::NUMERIC(10,2),
							ctrlVsi=sistema::NUMERIC(10,2),
							ctrlDif=diferencia::NUMERIC(10,2)
					WHERE	ctrlSec=secuencial;

					RAISE NOTICE 'Actualización realizada';
					RETURN QUERY
						SELECT 1 AS codigo, 'Cierre de caja registrado correctamente' AS descripcion;
				ELSE
					RAISE NOTICE 'CIERRE DE CAJA YA FUE REGISTRADO';
					RETURN QUERY
						SELECT 0 AS codigo, 'El cierre de caja correspondiente al '|| fecha ||' fue registrado previamente' AS descripcion;
				END IF;
			ELSE
				IF NOT EXISTS(SELECT * 
							  FROM 	tbCtrlCaja 
							  WHERE (ctrlFchIni BETWEEN _fchIn AND _fchFn) --ctrlFchIni = fecha::DATE 
							  		AND ctrlFchFin IS NOT NULL 
							  		AND ctrlHfn IS NOT NULL 
							  		AND cajacod=caja) THEN
					UPDATE tbCtrlCaja 
						SET ctrlFchFin=fecha::DATE,
							ctrlHfn=hora::TIME,
							ctrlVcr=valor::NUMERIC(10,2),
							ctrlefc=efectivo::NUMERIC(10,2), 
							ctrltrj=tarjeta::NUMERIC(10,2), 
							ctrlchq=cheque::NUMERIC(10,2),
							ctrlVsi=sistema::NUMERIC(10,2),
							ctrlDif=diferencia::NUMERIC(10,2)
					WHERE	ctrlSec=secuencial;

					RAISE NOTICE 'Actualización realizada';
					RETURN QUERY
						SELECT 1 AS codigo, 'Cierre de caja registrado correctamente' AS descripcion;
				ELSE
					RAISE NOTICE 'CIERRE DE CAJA YA FUE REGISTRADO';
					RETURN QUERY
						SELECT 0 AS codigo, 'El cierre de caja correspondiente al '|| fecha ||' fue registrado previamente' AS descripcion;
				END IF;
			END IF;
		EXCEPTION WHEN OTHERS THEN
			RAISE NOTICE 'Codigo de error = %', SQLSTATE;
			RETURN QUERY
				SELECT 0 AS codigo, 'Cierre de caja no pudo ser registrado' AS descripcion;
		END;
	END IF;
END;
$BODY$;