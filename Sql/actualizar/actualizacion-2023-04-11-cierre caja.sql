ALTER TABLE tbCtrlCierre ADD COLUMN ctrldlv NUMERIC(10,2);
ALTER TABLE tbCtrlCaja ADD COLUMN ctrldlv NUMERIC(10,2);

DROP FUNCTION public.fncontrolcaja(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying);

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
	cheque character varying,
	delivery character varying)
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
							ctrldlv=delivery::NUMERIC(10,2),
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
							ctrldlv=delivery::NUMERIC(10,2),
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

DROP FUNCTION public.fnlstcontrol(character varying, character varying);

CREATE OR REPLACE FUNCTION public.fnlstcontrol(
	fecha character varying,
	caja character varying)
    RETURNS TABLE(secuencial character varying, ingresos numeric, egresos numeric, compras numeric, ventas numeric, inicio numeric, cierre numeric, efectivo numeric, tarjeta numeric, cheque numeric, delivery numeric) 
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
	_delivery NUMERIC(10,2);
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
				compfch BETWEEN _fchIn AND _fchFn
				AND compAtv=true AND usecaja = true;
		
		SELECT 	ctrlSec, ctrlVin, ctrlVcr, ctrlefc, ctrltrj, ctrlchq, ctrldlv 
		INTO 	_secuencial, _inicio, _cierre, _efectivo, _tarjeta, _cheque, _delivery
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
				compfch BETWEEN _fchIn AND _fchFn
				AND codcaja=caja 
				AND compAtv=true
				AND usecaja = true;
		
		RAISE NOTICE 'OBTENIENTO DATOS DEL CIERRE DE CAJA';
		SELECT 	ctrlSec, ctrlVin, ctrlVcr, ctrlefc, ctrltrj, ctrlchq, ctrldlv 
		INTO 	_secuencial, _inicio, _cierre, _efectivo, _tarjeta, _cheque, _delivery
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
				COALESCE(_cheque,0) AS cheque,
				COALESCE(_delivery,0) AS delivery;
END;
$BODY$;



CREATE OR REPLACE FUNCTION public.fnlstcontrolcompvent(
	fecha character varying,
	caja character varying,
	tipo character varying)
    RETURNS TABLE(origen character varying, hora character varying, ventscl character varying, venttip character varying, venttot numeric, ventdif numeric, fpgotip character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
	_secuencial VARCHAR(10);
	_incVentCarrier BOOLEAN;
	_acumulativo BOOLEAN;
BEGIN
	/*
	-- Control Ventas
	-- Por cajas
	UPDATE tbConfParam set confval = encryptar('0') where confnid = 42;
	SELECT * FROM fnlstcontrolCompVent('2023-01-19','0001','V');
	
	UPDATE tbConfParam set confval = encryptar('0') where confnid = 42;
	SELECT * FROM fnlstcontrolCompVent('2023-01-16','0002','V');
	
	-- Acumulativo
	UPDATE tbConfParam set confval = encryptar('1') where confnid = 42;
	SELECT * FROM fnlstcontrolCompVent('2023-01-16','0000','V');
	
	SELECT * FROM fnlstcontrolCompVent('2023-02-02','0002','V');
	
	*/
	SELECT desencryptar(confval)::boolean INTO _incVentCarrier FROM tbConfParam WHERE confnid=41;
	SELECT desencryptar(confval)::boolean INTO _acumulativo FROM tbConfParam WHERE confnid=42;
	
	CREATE TEMPORARY TABLE tmp_Resultado(
		origen 		VARCHAR(2),
		hora		VARCHAR(10),
		secuencial 	VARCHAR(10),
		tipo 		VARCHAR(15),
		total 		NUMERIC,
		diferencia 	NUMERIC,
		tipopago 	VARCHAR(20)
	);
	
	IF _acumulativo = true THEN
		RAISE NOTICE 'OBTENIENDO EL VENTAS ACUMULATIVO';
		IF _incVentCarrier = true THEN
			INSERT INTO tmp_Resultado
			SELECT 	'V' AS origen,
					'' as hora,
					v.ventscl,
					CASE v.venttip
						WHEN 'L' THEN 'LOCAL'
						WHEN 'D' THEN 'DOMICILIO'
						WHEN 'C' THEN 'LLEVAR'
						WHEN 'E' THEN 'DELIVERY'
						WHEN 'R' THEN 'RETIRAR'
					END::CHARACTER VARYING AS otipo,
					SUM(v.venttot)::NUMERIC AS venttot, 
					SUM(p.fpgoval - p.fpgoVto)::NUMERIC AS ventdif,
					CASE p.fpgotip
						WHEN 'E' then 'EFECTIVO'
						WHEN 'T' THEN 'TARJETA CREDITO'
						ELSE 'CHEQUE'
					END:: CHARACTER VARYING AS fpgotip
			FROM 	tbPeddDVent v
					JOIN tbPeddDFpgo p ON v.ventscl = p.ventscl
			WHERE 	v.ventfch= fecha::DATE AND v.ventstd IN ('F', 'P')
			GROUP BY v.venttip, v.ventscl, p.fpgotip;
		ELSE
			INSERT INTO tmp_Resultado
			SELECT 	'V' AS origen,
					'' AS hora,
					v.ventscl,
					CASE v.venttip
						WHEN 'L' THEN 'LOCAL'
						WHEN 'D' THEN 'DOMICILIO'
						WHEN 'C' THEN 'LLEVAR'
						WHEN 'E' THEN 'DELIVERY'
						WHEN 'R' THEN 'RETIRAR'
					END::CHARACTER VARYING AS otipo,
					SUM(v.venttot)::NUMERIC AS venttot,
					SUM(p.fpgoval - p.fpgoVto)::NUMERIC AS fpgodif,
					CASE p.fpgotip
						WHEN 'E' then 'EFECTIVO'
						WHEN 'T' THEN 'TARJETA CREDITO'
						ELSE 'CHEQUE'
					END :: CHARACTER VARYING AS fpgotip
				FROM 	tbPeddDVent v
						JOIN tbPeddDFpgo p ON v.ventscl = p.ventscl
				WHERE 	v.ventfch= fecha::DATE AND v.ventcrr = '0000' AND v.ventstd IN ('F', 'P')
				GROUP BY v.venttip, v.ventscl, p.fpgotip;
		END IF;
		
		INSERT INTO tmp_Resultado
		SELECT 	'C' AS origen,
				'' AS hora,
				c.compnid AS ventscl,
				'':: CHARACTER VARYING AS otipo,
				COALESCE(SUM(c.comptot),0)::NUMERIC AS venttot, 
				0::NUMERIC AS ventdif,
				''::CHARACTER VARYING AS fpgotip
		FROM	tbCompDComp c
		WHERE 	c.compfch=fecha::DATE 
				AND c.compAtv = true 
				AND c.usecaja = true
		GROUP BY c.compnid;
		
		INSERT INTO tmp_Resultado
		SELECT 	'I' AS origen,
				'' AS hora,
				vaigsec,
				'' AS otipo,
				COALESCE(SUM(vaigval),0) AS valor,
				0 AS diferencia,
				'' AS tipo
		FROM 	tbCajaVaIg
		WHERE 	vaigfch = fecha::DATE 
				AND vaigfel IS NULL
		GROUP BY vaigsec;
		
		INSERT INTO tmp_Resultado
		SELECT 	'E' AS origen,
				'' AS hora,
				vaegsec,
				'' AS otipo,
				COALESCE(SUM(vaegval),0) AS valor,
				0 AS diferencia,
				'' AS tipo
		FROM 	tbCajaVaEg
		WHERE 	vaegfch = fecha::DATE 
				AND vaegfel IS NULL
		GROUP BY vaegsec;
		
		INSERT INTO tmp_Resultado
		SELECT 	'IC' AS origen,
				'' AS hora,
				ctrlsec,
				'' as otipo, 
				COALESCE(ctrlvin,0) AS valor,
				0 AS diferencia,
				'' AS tipo
		FROM 	tbCtrlCierre 
		WHERE 	ctrlfchIni = fecha::DATE;
		
		INSERT INTO tmp_Resultado
		SELECT 	'CC' AS origen,
				ctrlhfn AS hora,
				ctrlsec,
				'' as otipo, 
				COALESCE(ctrlvcr,0) AS valor,
				0 AS diferencia,
				'' AS tipo
		FROM 	tbCtrlCierre 
		WHERE 	ctrlfchIni = fecha::DATE;
		
		INSERT INTO tmp_Resultado
		SELECT 	'CE' AS origen,
				TO_CHAR(ctrlhfn,'HH24:MI:SS') AS hora,
				ctrlsec,
				'' as otipo, 
				COALESCE(ctrlefc,0) AS valor,
				0 AS diferencia,
				'' AS tipo
		FROM 	tbCtrlCierre 
		WHERE 	ctrlfchIni = fecha::DATE;
				
		INSERT INTO tmp_Resultado
		SELECT 	'CT' AS origen,
				TO_CHAR(ctrlhfn,'HH24:MI:SS') AS hora,
				ctrlsec,
				'' as otipo, 
				COALESCE(ctrltrj,0) AS valor,
				0 AS diferencia,
				'' AS tipo
		FROM 	tbCtrlCierre 
		WHERE 	ctrlfchIni = fecha::DATE;
				
		INSERT INTO tmp_Resultado
		SELECT 	'CQ' AS origen,
				TO_CHAR(ctrlhfn,'HH24:MI:SS') AS hora,
				ctrlsec,
				'' as otipo, 
				COALESCE(ctrlchq,0) AS valor,
				0 AS diferencia,
				'' AS tipo
		FROM 	tbCtrlCierre 
		WHERE 	ctrlfchIni = fecha::DATE;
		
		INSERT INTO tmp_Resultado
		SELECT 	'DL' AS origen,
				TO_CHAR(ctrlhfn,'HH24:MI:SS') AS hora,
				ctrlsec,
				'' as otipo, 
				COALESCE(ctrldlv,0) AS valor,
				0 AS diferencia,
				'' AS tipo
		FROM 	tbCtrlCierre 
		WHERE 	ctrlfchIni = fecha::DATE;
	ELSE
		IF _incVentCarrier = true THEN
			INSERT INTO tmp_Resultado
			SELECT 	'V' AS origen,
					'' AS hora,
					v.ventscl,
					CASE v.venttip
						WHEN 'L' THEN 'LOCAL'
						WHEN 'D' THEN 'DOMICILIO'
						WHEN 'C' THEN 'LLEVAR'
						WHEN 'E' THEN 'DELIVERY'
						WHEN 'R' THEN 'RETIRAR'
					END ::CHARACTER VARYING AS otipo,
					SUM(v.venttot)::NUMERIC AS venttot,
					SUM(p.fpgoval - p.fpgoVto):: NUMERIC AS fpgodif,
					CASE p.fpgotip
						WHEN 'E' then 'EFECTIVO'
						WHEN 'T' THEN 'TARJETA CREDITO'
						ELSE 'CHEQUE'
					END :: CHARACTER VARYING AS fpgotip
				FROM 	tbPeddDVent v
						JOIN tbPeddDFpgo p ON v.ventscl = p.ventscl
				WHERE 	v.ventfch= fecha::DATE AND v.cajacod = caja AND v.ventstd IN ('F', 'P')
				GROUP BY v.venttip, v.ventscl, p.fpgotip;
		ELSE
			INSERT INTO tmp_Resultado
			SELECT 	v.ventscl,
					'' AS hora,
					CASE v.venttip
						WHEN 'L' THEN 'LOCAL'
						WHEN 'D' THEN 'DOMICILIO'
						WHEN 'C' THEN 'LLEVAR'
						WHEN 'E' THEN 'DELIVERY'
						WHEN 'R' THEN 'RETIRAR'
					END::CHARACTER VARYING AS otipo,
					SUM(v.venttot)::NUMERIC AS venttot,
					SUM(p.fpgoval - p.fpgoVto):: NUMERIC AS fpgodif,
					CASE p.fpgotip
						WHEN 'E' then 'EFECTIVO'
						WHEN 'T' THEN 'TARJETA CREDITO'
						ELSE 'CHEQUE'
					END :: CHARACTER VARYING AS fpgotip
			FROM 	tbPeddDVent v
					JOIN tbPeddDFpgo p ON v.ventscl = p.ventscl
			WHERE 	v.ventfch= fecha::DATE AND v.cajacod = caja
					AND v.ventcrr = '0000' AND v.ventstd IN ('F', 'P')
			GROUP BY v.venttip, v.ventscl, p.fpgotip;
		END IF;
		
		INSERT INTO tmp_Resultado
		SELECT 	'C' AS origen,
				'' AS hora,
				c.compnid AS ventscl,
				'':: CHARACTER VARYING AS venttip,
				COALESCE(SUM(c.comptot),0)::NUMERIC AS venttot, 
				0::NUMERIC AS ventdif,
				''::CHARACTER VARYING AS fpgotip
		FROM	tbCompDComp c
		WHERE 	c.compfch=fecha::DATE 
				AND c.codcaja = caja
				AND c.compAtv = true 
				AND c.usecaja = true
		GROUP BY c.compnid;
		
		INSERT INTO tmp_Resultado
		SELECT 	'I' AS origen,
				'' AS hora,
				vaigsec,
				'' AS tipo,
				COALESCE(SUM(vaigval),0) AS valor,
				0 AS diferencia,
				'' AS tipo
		FROM 	tbCajaVaIg
		WHERE 	vaigfch = fecha::DATE 
				AND cajacod = caja
				AND vaigfel IS NULL
		GROUP BY vaigsec;
		
		INSERT INTO tmp_Resultado
		SELECT 	'E' AS origen,
				'' AS hora,
				vaegsec,
				'' AS tipo,
				COALESCE(SUM(vaegval),0) AS valor,
				0 AS diferencia,
				'' AS tipo
		FROM 	tbCajaVaEg
		WHERE 	vaegfch = fecha::DATE 
				AND cajacod = caja
				AND vaegfel IS NULL
		GROUP BY vaegsec;
		
		INSERT INTO tmp_Resultado
		SELECT 	'IC' AS origen,
				'' AS hora,
				ctrlsec,
				'' as otipo, 
				COALESCE(ctrlvin,0) AS valor,
				0 AS diferencia,
				'' AS tipo
		FROM 	tbCtrlCaja 
		WHERE 	ctrlfchIni = fecha::DATE
				AND cajacod = caja;
		
		INSERT INTO tmp_Resultado
		SELECT 	'CC' AS origen,
				TO_CHAR(ctrlhfn,'HH24:MI:SS') AS hora,
				ctrlsec,
				'' as otipo, 
				COALESCE(ctrlvcr,0) AS valor,
				0 AS diferencia,
				'' AS tipo
		FROM 	tbCtrlCaja 
		WHERE 	ctrlfchIni = fecha::DATE
				AND cajacod = caja;

		INSERT INTO tmp_Resultado
		SELECT 	'CE' AS origen,
				TO_CHAR(ctrlhfn,'HH24:MI:SS') AS hora,
				ctrlsec,
				'' as otipo, 
				COALESCE(ctrlefc,0) AS valor,
				0 AS diferencia,
				'' AS tipo
		FROM 	tbCtrlCaja 
		WHERE 	ctrlfchIni = fecha::DATE
				AND cajacod = caja;
				
		INSERT INTO tmp_Resultado
		SELECT 	'CT' AS origen,
				TO_CHAR(ctrlhfn,'HH24:MI:SS') AS hora,
				ctrlsec,
				'' as otipo, 
				COALESCE(ctrltrj,0) AS valor,
				0 AS diferencia,
				'' AS tipo
		FROM 	tbCtrlCaja 
		WHERE 	ctrlfchIni = fecha::DATE
				AND cajacod = caja;
				
		INSERT INTO tmp_Resultado
		SELECT 	'CQ' AS origen,
				TO_CHAR(ctrlhfn,'HH24:MI:SS') AS hora,
				ctrlsec,
				'' as otipo, 
				COALESCE(ctrlchq,0) AS valor,
				0 AS diferencia,
				'' AS tipo
		FROM 	tbCtrlCaja 
		WHERE 	ctrlfchIni = fecha::DATE
				AND cajacod = caja;
				
		INSERT INTO tmp_Resultado
		SELECT 	'DL' AS origen,
				TO_CHAR(ctrlhfn,'HH24:MI:SS') AS hora,
				ctrlsec,
				'' as otipo, 
				COALESCE(ctrldlv,0) AS valor,
				0 AS diferencia,
				'' AS tipo
		FROM 	tbCtrlCaja 
		WHERE 	ctrlfchIni = fecha::DATE
				AND cajacod = caja;
	END IF;
	
	RETURN QUERY
	SELECT * FROM tmp_Resultado;
	
	DROP TABLE tmp_Resultado;
	
END;
$BODY$;

DROP FUNCTION public.fnbuscainiciocierre(character varying, character varying);

CREATE OR REPLACE FUNCTION public.fnbuscainiciocierre(
	fecha character varying,
	caja character varying)
    RETURNS TABLE(ctrlsec character varying, ctrlfchini date, ctrlhin time without time zone, ctrlvin numeric, ctrlhfn time without time zone, ctrlvcr numeric, ctrlvsi numeric, ctrldif numeric, ctrlfchfin date, ctrlefc numeric, ctrltrj numeric, ctrlchq numeric, ctrldlv numeric) 
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
				COALESCE(c.ctrlchq,0) AS ctrlchq,
				COALESCE(c.ctrldlv,0) AS ctrldlv
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
				COALESCE(c.ctrlchq,0) AS ctrlchq,
				COALESCE(c.ctrldlv,0) AS ctrldlv
		FROM 	tbCtrlCaja c
		WHERE 	(c.ctrlfchini + c.ctrlhin) BETWEEN _fchIn AND _fchFn
		--WHERE 	c.ctrlfchini = fecha::DATE 
				AND c.cajacod=caja;
	END IF;
END;
$BODY$;
