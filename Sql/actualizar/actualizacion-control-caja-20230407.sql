ALTER TABLE tbCompDComp ADD COLUMN compHra TIME WITHOUT TIME ZONE  DEFAULT NOW();

CREATE OR REPLACE FUNCTION public.fnbuscainiciocierre(
	fecha character varying,
	caja character varying)
    RETURNS TABLE(ctrlsec character varying, ctrlfchini date, ctrlhin time without time zone, ctrlvin numeric, ctrlhfn time without time zone, ctrlvcr numeric, ctrlvsi numeric, ctrldif numeric, ctrlfchfin date, ctrlefc numeric, ctrltrj numeric, ctrlchq numeric) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
    _cierreAcum INT;
	_fecha TIMESTAMP WITHOUT TIME ZONE;
	_fchIn TIMESTAMP WITHOUT TIME ZONE;
	_fchFn TIMESTAMP WITHOUT TIME ZONE;
	hora TIME WITHOUT TIME ZONE;
BEGIN
	/*
	SELECT * FROM fnBuscaInicioCierre('2023-01-19','0001');
	*/
	hora := current_time;
	
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
	
	IF _cierreAcum = 1 THEN
		RETURN QUERY
		SELECT 	c.ctrlsec, 
				c.ctrlfchini,
				c.ctrlhin,
				c.ctrlvin,
				c.ctrlhfn,
				c.ctrlvcr,
				c.ctrlvsi,
				c.ctrldif,
				c.ctrlfchfin,
				COALESCE(c.ctrlefc,0) AS ctrlefc,
				COALESCE(c.ctrltrj,0) AS ctrltrj,
				COALESCE(c.ctrlchq,0) AS ctrlchq
		FROM 	tbCtrlCierre c
		--WHERE 	c.ctrlfchini = fecha::DATE BETWEEN ;
		WHERE 	(c.ctrlfchini + c.ctrlhin) BETWEEN _fchIn AND _fchFn;
	ELSE
		RETURN QUERY
		SELECT 	c.ctrlsec, 
				c.ctrlfchini,
				c.ctrlhin,
				c.ctrlvin,
				c.ctrlhfn,
				c.ctrlvcr,
				c.ctrlvsi,
				c.ctrldif,
				c.ctrlfchfin,
				COALESCE(c.ctrlefc,0) AS ctrlefc,
				COALESCE(c.ctrltrj,0) AS ctrltrj,
				COALESCE(c.ctrlchq,0) AS ctrlchq
		FROM 	tbCtrlCaja c
		WHERE 	(c.ctrlfchini + c.ctrlhin) BETWEEN _fchIn AND _fchFn
		--WHERE 	c.ctrlfchini = fecha::DATE 
				AND c.cajacod=caja;
	END IF;
END;
$BODY$;

CREATE OR REPLACE FUNCTION public.fnlstcontrol(
	fecha character varying,
	caja character varying)
    RETURNS TABLE(secuencial character varying, ingresos numeric, egresos numeric, compras numeric, ventas numeric, inicio numeric, cierre numeric, efectivo numeric, tarjeta numeric, cheque numeric) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
	_ventas NUMERIC(10,2);
	_ingresos NUMERIC(10,2);
	_egresos NUMERIC(10,2);
	_compras NUMERIC(10,2);
	_inicio NUMERIC(10,2);
	_cierre NUMERIC(10,2);
	_secuencial VARCHAR(10);
	_incVentCarrier BOOLEAN;
	_acumulativo BOOLEAN;
	_efectivo NUMERIC(10,2);
	_tarjeta NUMERIC(10,2);
	_cheque NUMERIC(10,2);
	_hora TIME WITHOUT TIME ZONE;
	_fchFn TIMESTAMP WITHOUT TIME ZONE;
	_fchIn TIMESTAMP WITHOUT TIME ZONE;
	_fecha DATE;
BEGIN
	/*
	-- Control por cajas
	UPDATE tbConfParam set confval = encryptar('0') where confnid = 42;
	SELECT * FROM fnlstcontrol('2023-01-18','0001');
	
	-- Control acumulativo
	UPDATE tbConfParam set confval = encryptar('1') where confnid = 42;
	SELECT * FROM fnlstcontrol('2023-01-26','0001')
	SELECT * FROM fnlstcontrol('2023-01-25','0001')
	*/
	SELECT desencryptar(confval)::boolean INTO _incVentCarrier FROM tbConfParam WHERE confnid=41;
	SELECT desencryptar(confval)::boolean INTO _acumulativo FROM tbConfParam WHERE confnid=42;
	
	_hora := CURRENT_TIME;
	--_hora := '02:15:35'::TIME WITHOUT TIME ZONE;
	--_fecha := CURRENT_DATE;
	RAISE NOTICE 'HORA ACTUAL %', _hora;
	--RAISE NOTICE 'FECHA ACTUAL %', _fecha;
	
	
	IF _hora::TIME > '00:00'::TIME AND _hora::TIME < '04:01'::TIME THEN
		_fecha := fecha::DATE + INTERVAL '-1 day';
		
		_fchIn := (_fecha + INTERVAL '+7 hour')::TIMESTAMP WITHOUT TIME ZONE;
		_fchFn := (fecha || ' 04:59:59')::TIMESTAMP WITHOUT TIME ZONE;
	ELSE
		_fecha := fecha::DATE + INTERVAL '+1 day';
		
		_fchIn := (fecha || ' 07:00:00')::TIMESTAMP WITHOUT TIME ZONE;
		--_fchFn := (_fecha + INTERVAL || '+5 hour')::TIMESTAMP WITHOUT TIME ZONE;
		_fchFn := (_fecha::VARCHAR  || ' 04:59:59')::TIMESTAMP WITHOUT TIME ZONE;
	END IF;
	
	RAISE NOTICE 'FECHA INICIO %', _fchin;
	RAISE NOTICE 'FECHA FIN    %', _fchFn;
	
	IF _acumulativo = true THEN
		RAISE NOTICE 'OBTENIENDO EL CONTROL DE CAJA ACUMULATIVO';
		SELECT SUM(vaegval) INTO _egresos FROM tbCajaVaEg WHERE vaegfch=fecha::DATE;
		SELECT SUM(vaigval) INTO _ingresos FROM tbCajaVaIg WHERE vaigfch=fecha::DATE;
		
		IF _incVentCarrier = true THEN
			SELECT 	SUM(venttot) INTO _ventas 
			FROM 	tbPeddDVent 
			WHERE 	--ventfch=fecha::DATE 
					(ventfch + venthra) BETWEEN _fchIn AND _fchFn
					AND ventStd IN ('F','P');
		ELSE
			SELECT 	SUM(venttot) INTO _ventas 
			FROM 	tbPeddDVent 
			WHERE 	--ventfch=fecha::DATE 
					(ventfch + venthra) BETWEEN _fchIn AND _fchFn
					AND ventStd IN ('F','P') AND ventcrr = '0000';	
		END IF;

		SELECT 	COALESCE(SUM(comptot),0) INTO _compras 
		FROM	tbCompDComp 
		WHERE 	--compfch=fecha::DATE 
				(compfch + compHra) BETWEEN _fchIn AND _fchFn
				AND compAtv=true AND usecaja = true;
		
		SELECT 	ctrlSec, ctrlVin, ctrlVcr, ctrlefc, ctrltrj, ctrlchq INTO _secuencial, _inicio, _cierre, _efectivo, _tarjeta, _cheque 
		FROM 	tbCtrlCierre 
		WHERE 	--ctrlFchIni=fecha::DATE;
				(ctrlfchini + ctrlhin) BETWEEN _fchIn AND _fchFn;
	ELSE
		RAISE NOTICE 'OBTENIENDO EL CONTROL DE CAJA POR CAJA';
		SELECT SUM(vaegval) INTO _egresos FROM tbCajaVaEg WHERE vaegfch=fecha::DATE AND cajacod=caja;
		SELECT SUM(vaigval) INTO _ingresos FROM tbCajaVaIg WHERE vaigfch=fecha::DATE AND cajacod=caja;
		
		IF _incVentCarrier = true THEN
			RAISE NOTICE 'OBTENIENTO TOTAL VENTAS CON DELIVERY INCLUIDO';
			SELECT 	SUM(venttot) INTO _ventas 
			FROM 	tbPeddDVent 
			WHERE 	--ventfch=fecha::DATE 
					(ventfch + venthra) BETWEEN _fchIn AND _fchFn
					AND cajacod=caja 
					AND ventStd IN ('F','P');
		ELSE
			RAISE NOTICE 'OBTENIENTO TOTAL VENTAS SIN DELIVERY';
			SELECT 	SUM(venttot) INTO _ventas 
			FROM 	tbPeddDVent 
			WHERE 	--ventfch=fecha::DATE 
					(ventfch + venthra) BETWEEN _fchIn AND _fchFn
					AND cajacod=caja 
					AND ventStd IN ('F','P') AND ventcrr = '0000';	
		END IF;

		RAISE NOTICE 'OBTENIENTO TOTAL COMPRAS';
		SELECT 	SUM(comptot) INTO _compras 
		FROM 	tbCompDComp 
		WHERE 	--compfch=fecha::DATE 
				(compfch + compHra) BETWEEN _fchIn AND _fchFn
				AND codcaja=caja 
				AND compAtv=true
				AND usecaja = true;
		
		RAISE NOTICE 'OBTENIENTO DATOS DEL CIERRE DE CAJA';
		SELECT 	ctrlSec, ctrlVin, ctrlVcr, ctrlefc, ctrltrj, ctrlchq INTO _secuencial, _inicio, _cierre, _efectivo, _tarjeta, _cheque
		FROM 	tbCtrlCaja 
		WHERE 	--ctrlFchIni=fecha::DATE 
				(ctrlfchini + ctrlhin) BETWEEN _fchIn AND _fchFn
				AND cajacod=caja;
	END IF;
	
	RAISE NOTICE 'RETORNANDO LA INFORMACION';
	RETURN QUERY
		SELECT 	COALESCE(_secuencial, '') AS secuencial,
				COALESCE(_ingresos,0) AS ingresos, 
				COALESCE(_egresos,0) AS egresos, 
				COALESCE(_compras,0) AS compras, 
				COALESCE(_ventas,0) AS ventas,
				COALESCE(_inicio,0) AS inicio,
				COALESCE(_cierre,0) AS cierre,
				COALESCE(_efectivo,0) AS efectivo,
				COALESCE(_tarjeta,0) AS tarjeta,
				COALESCE(_cheque,0) AS cheque;
END;
$BODY$;

