DROP FUNCTION IF EXISTS fnLstControl;
CREATE OR REPLACE FUNCTION fnLstControl(IN fecha VARCHAR, IN caja VARCHAR)
RETURNS TABLE(	
	secuencial VARCHAR(10),
	ingresos NUMERIC(10,2),
	egresos NUMERIC(10,2),	
	compras NUMERIC(10,2),
	ventas NUMERIC(10,2),
	inicio NUMERIC(10,2),
	cierre NUMERIC(10,2)
)
AS $$
DECLARE
	_ventas NUMERIC(10,2);
	_ingresos NUMERIC(10,2);
	_egresos NUMERIC(10,2);
	_compras NUMERIC(10,2);
	_inicio NUMERIC(10,2);
	_cierre NUMERIC(10,2);
	_secuencial VARCHAR(10);
	_incVentCarrier BOOLEAN;
BEGIN
	SELECT desencryptar(confval)::boolean INTO _incVentCarrier FROM tbConfParam WHERE confnid=41;

	SELECT SUM(vaegval) INTO _egresos FROM tbCajaVaEg WHERE vaegfch=fecha::DATE AND cajacod=caja;
	SELECT SUM(vaigval) INTO _ingresos FROM tbCajaVaIg WHERE vaigfch=fecha::DATE AND cajacod=caja;
	
	IF _incVentCarrier = true THEN
		SELECT 	SUM(venttot) INTO _ventas 
		FROM 	tbPeddDVent 
		WHERE 	ventfch=fecha::DATE AND cajacod=caja 
				AND ventStd IN ('F','P');
	ELSE
		SELECT 	SUM(venttot) INTO _ventas 
		FROM 	tbPeddDVent 
		WHERE 	ventfch=fecha::DATE AND cajacod=caja 
				AND ventStd IN ('F','P') AND ventcrr = '0000';	
	END IF;
	
	SELECT SUM(comptot) INTO _compras FROM tbCompDComp WHERE compfch=fecha::DATE AND codcaja=caja AND compAtv=true;
	SELECT ctrlSec, ctrlVin, ctrlVcr INTO _secuencial, _inicio, _cierre FROM tbCtrlCaja WHERE ctrlFch=fecha::DATE AND cajacod=caja;
	
	RETURN QUERY
		SELECT 	COALESCE(_secuencial, '') AS secuencial,
				COALESCE(_ingresos,0) AS ingresos, 
				COALESCE(_egresos,0) AS egresos, 
				COALESCE(_compras,0) AS compras, 
				COALESCE(_ventas,0) AS ventas,
				COALESCE(_inicio,0) AS inicio,
				COALESCE(_cierre,0) AS cierre;
END;
$$
LANGUAGE 'plpgsql';

DROP FUNCTION IF EXISTS fnLstFechas;
CREATE OR REPLACE FUNCTION fnLstFechas(IN inicio VARCHAR)
RETURNS TABLE(	
	dia DATE
)
AS $$
BEGIN
	RETURN QUERY
		SELECT fecha::DATE
		FROM	 (SELECT (inicio::DATE + (interval '1' day * generate_series(0,31)))::date AS fecha) d
		WHERE 	d.fecha BETWEEN inicio::DATE AND 
				(SELECT (date_trunc('MONTH', inicio::DATE) + INTERVAL '1 MONTH - 1 day')::date);
END;
$$
LANGUAGE 'plpgsql';

DROP FUNCTION IF EXISTS fnRptControlCaja;
CREATE OR REPLACE FUNCTION fnRptControlCaja(IN anio INT, IN mes INT, IN detalle BOOLEAN)
RETURNS TABLE(	
	fecha	DATE,
	caja	VARCHAR(20),
	inicio NUMERIC(10,2),
	ingreso NUMERIC(10,2),
	egreso NUMERIC(10,2),
	cierre NUMERIC(10,2),
	sistema NUMERIC(10,2),
	diferencia NUMERIC(10,2)
)
AS $$
DECLARE
	_inicio 	DATE;
	_fin 		DATE;
BEGIN
	_inicio :=  (anio ||'-'|| lpad(mes::text,2,'0')   || '-01')::DATE;
	IF mes + 1 > 12 THEN
		anio := anio + 1;
		mes := 1;
	END IF;
	_fin 	:=  (anio ||'-'|| lpad((mes+1)::text,2,'0') || '-01')::DATE;

	IF detalle = false THEN
		RETURN QUERY	
		SELECT d.dia as fecha, c.cajaNbr AS caja, 	
				COALESCE((SELECT ctrlvin FROM tbCtrlCaja WHERE ctrlFch=dia AND cajaCod=c.cajaCod),0)::NUMERIC(10,2) AS inicio, 
				0::NUMERIC(10,2) AS ingreso, 
				0::NUMERIC(10,2) AS egreso, 
				COALESCE((SELECT ctrlvcr FROM tbCtrlCaja WHERE ctrlFch=dia AND cajaCod=c.cajaCod),0)::NUMERIC(10,2) AS cierre, 
				COALESCE((SELECT ctrlvsi FROM tbCtrlCaja WHERE ctrlFch=dia AND cajaCod=c.cajaCod),0)::NUMERIC(10,2) AS sistema, 
				COALESCE((SELECT ctrldif FROM tbCtrlCaja WHERE ctrlFch=dia AND cajaCod=c.cajaCod),0)::NUMERIC(10,2) diferencia 		
		FROM	fnLstFechas(_inicio::DATE) d,
				tbCajaCaja c  
		ORDER BY c.cajaNbr, d.dia;		
	ELSE
		RETURN QUERY
		SELECT a.fecha, a.caja, a.inicio, a.ingreso, a.egreso, 
				a.cierre,a.sistema::NUMERIC(10,2) AS sistema, 
				--(a.cierre - a.sistema)::NUMERIC(10,2) AS diferencia
				a.diferencia::NUMERIC(10,2) AS diferencia
		FROM (
			SELECT 	d.dia as fecha, c.cajaCod, c.cajaNbr AS caja, 
					COALESCE((SELECT ctrlVin FROM tbCtrlCaja WHERE ctrlFch=d.dia AND cajaCod=c.cajaCod),0)::NUMERIC(10,2) AS inicio, 
					COALESCE((SELECT SUM(vaigval) FROM tbCajaVaIg WHERE cajaCod=c.cajaCod AND vaigFch=d.dia),0)::NUMERIC(10,2) AS ingreso, 
					COALESCE((SELECT SUM(vaegval) FROM tbCajaVaEg WHERE cajaCod=c.cajaCod AND vaegFch=d.dia),0)::NUMERIC(10,2) AS egreso, 
					COALESCE((SELECT SUM(ctrlVcr) FROM tbCtrlCaja WHERE ctrlFch=d.dia AND cajaCod=c.cajaCod),0)::NUMERIC(10,2) AS cierre, 
					COALESCE((SELECT SUM(ctrlvsi) FROM tbCtrlCaja WHERE ctrlFch=d.dia AND cajaCod=c.cajaCod),0)::NUMERIC(10,2) AS sistema
					COALESCE((SELECT SUM(ctrldif) FROM tbCtrlCaja WHERE ctrlFch=d.dia AND cajaCod=c.cajaCod),0)::NUMERIC(10,2) AS diferencia
					--0::NUMERIC(10,2) diferencia
			FROM 	fnLstFechas(_inicio) d,
					tbCajaCaja c 
			ORDER BY c.cajaNbr
		) a;

	END IF;
END;
$$
LANGUAGE 'plpgsql';

DROP FUNCTION IF EXISTS fnRptControlCajaDiario;
CREATE OR REPLACE FUNCTION fnRptControlCajaDiario(IN anio INT, IN mes INT, in dia INT)
RETURNS TABLE(	
	fecha	DATE,
	caja	VARCHAR(20),
	inicio NUMERIC(10,2),
	ingreso NUMERIC(10,2),
	egreso NUMERIC(10,2),
	cierre NUMERIC(10,2),
	sistema NUMERIC(10,2),
	diferencia NUMERIC(10,2)
)
AS $$
DECLARE
	_inicio 	DATE;
	_fecha 		DATE;
BEGIN
	_inicio :=  (anio ||'-'|| lpad(mes::text,2,'0')   || '-01')::DATE;
	_fecha  :=  (anio ||'-'|| lpad(mes::text,2,'0')   || '-' || lpad(dia::text,2,'0') )::DATE;
	
	RETURN QUERY
	SELECT fecha, caja, resumen, inicio, ingreso, egreso, cierre, sistema, diferencia
	FROM (
			SELECT	* 
			FROM (
					SELECT 	d.dia as fecha, c.cajaNbr AS caja, 'RESUMEN CAJA'	AS resumen,
							COALESCE((SELECT ctrlvin FROM tbCtrlCaja WHERE ctrlFch=dia AND cajaCod=c.cajaCod),0)::NUMERIC(10,2) AS inicio, 
							0::NUMERIC(10,2) AS ingreso, 
							0::NUMERIC(10,2) AS egreso, 
							COALESCE((SELECT ctrlvcr FROM tbCtrlCaja WHERE ctrlFch=dia AND cajaCod=c.cajaCod),0)::NUMERIC(10,2) AS cierre, 
							COALESCE((SELECT ctrlvsi FROM tbCtrlCaja WHERE ctrlFch=dia AND cajaCod=c.cajaCod),0)::NUMERIC(10,2) AS sistema, 
							COALESCE((SELECT ctrldif FROM tbCtrlCaja WHERE ctrlFch=dia AND cajaCod=c.cajaCod),0)::NUMERIC(10,2) AS diferencia,
							1 AS orden
					FROM	fnLstFechas(_inicio::DATE) d,
							tbCajaCaja c  
					WHERE 	d.dia = _fecha
					ORDER BY c.cajaNbr, d.dia
		) a 
		UNION
		SELECT 	v.ventfch, c.cajanbr, 
				'VENTAS' AS resumen, 0 AS inicio, 
				ROUND(SUM(v.venttot),2) AS ingreso, 
				0 AS egreso, 0 AS cierre, 0 AS sistema, 0 AS diferencia, 2 AS orden
		FROM 	tbPeddDVent v
				JOIN
				tbPeddDFpgo p ON p.ventscl = v.ventscl
				JOIN
				tbCajaCaja c ON c.cajacod = v.cajacod
		WHERE 	--p.fpgotip = 'E' AND
				--v.ventatv = true AND
				--v.ventpgd = true AND
				v.ventStd IN ('F','P') AND
				v.ventfch = _fecha
		GROUP BY c.cajanbr, v.ventfch
		UNION
		SELECT 	co.compfch, c.cajanbr,  
				'COMPRAS' AS resumen, 0 AS inicio, 
				0 AS ingreso, ROUND(SUM(co.comptot),2) AS egreso, 
				0 AS cierre, 0 AS sistema, 0 AS diferencia, 3 AS orden
		FROM 	tbCompDComp co
				join
				tbCompFpgo p ON co.compNid = p.compNid
				JOIN
				tbCajaCaja c ON c.cajacod = co.codcaja
		WHERE	--p.fpgotip = 'E' AND
				co.compatv = true AND
				co.compfch = _fecha
		GROUP BY co.compfch, c.cajanbr
		UNION
		SELECT 	v.vaigfch, c.cajanbr, 
				'VALES INGRESO' AS resumen, 0 AS inicio, 
				ROUND(SUM(v.vaigval),2) AS ingreso, 
				0 AS egreso,0 AS cierre, 0 AS sistema, 0 AS diferencia, 4 AS orden
		FROM	tbcajavaig v
				JOIN
				tbCajaCaja c ON c.cajacod = v.cajacod
		WHERE	v.vaigfch = _fecha
		GROUP BY v.vaigfch, c.cajanbr
		union
		SELECT 	v.vaegfch, c.cajanbr, 
				'VALES EGRESO' AS resumen, 0 AS inicio, 
				0 AS ingreso, ROUND(SUM(v.vaegval),2) AS egreso,
				0 AS cierre, 0 AS sistema, 0 AS diferencia, 5 AS orden
		FROM	tbcajavaeg v
				JOIN
				tbCajaCaja c ON c.cajacod = v.cajacod
		WHERE	v.vaegfch = _fecha
		GROUP BY v.vaegfch, c.cajanbr
	) B
	ORDER BY B.orden;
END;
$$
LANGUAGE 'plpgsql';


DROP FUNCTION IF EXISTS fnControlCaja;
CREATE OR REPLACE FUNCTION fnControlCaja(IN tipo VARCHAR, IN caja VARCHAR, IN secuencial VARCHAR,
										    IN fecha VARCHAR, IN hora VARCHAR, IN valor VARCHAR, 
										    IN sistema VARCHAR, IN diferencia VARCHAR)
RETURNS TABLE(
	codigo INT,
	descripcion TEXT
)
AS $$
DECLARE
	_secuencial INT;	
	_cierreAcum INT;
	_fecha TIMESTAMP WITHOUT TIME ZONE;
	_fchIn TIMESTAMP WITHOUT TIME ZONE;
	_fchFn TIMESTAMP WITHOUT TIME ZONE;
BEGIN
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
				END IF;
				
			ELSE
				RAISE NOTICE 'CIERRE POR CAJA';
				SELECT COALESCE(MAX(ctrlSec::int)+1,1) INTO _secuencial FROM tbCtrlCaja;

				RAISE NOTICE 'SECUENCIAL = %', _secuencial;			

				IF NOT EXISTS(SELECT * FROM tbCtrlCaja WHERE ctrlFch = fecha::DATE AND cajaCod=caja) THEN
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
$$
LANGUAGE 'plpgsql';

CREATE OR REPLACE FUNCTION public.fnCaja(
	accion varchar(1),
	codcaja varchar(4),
	nombre varchar(20)
)
RETURNS TABLE (
	codigo INT, 
	descripcion TEXT
)
AS $$
DECLARE
    secCaja VARCHAR(4);
BEGIN
	BEGIN
		RAISE NOTICE 'ANTES DEL IF';
		IF accion = 'N' THEN
			RAISE NOTICE 'Creando Caja';
			select LPAD((COALESCE(MAX(cajacod::int),0)+1)::TEXT,4,'0') INTO secCaja FROM tbCajaCaja;
			
			RAISE NOTICE 'SECUENCIAL = %', secCaja;
			
			IF EXISTS(SELECT 1 FROM tbCajaCaja WHERE cajaNbr = nombre) THEN
				RAISE NOTICE 'CAJA YA EXISTE';
				RETURN QUERY
				SELECT 0 AS codigo, 'El nombre de caja ya existe.' AS descripcion;	
			END IF;
			RAISE NOTICE 'ANTES DEL INSERT';
			INSERT INTO tbCajaCaja VALUES(secCaja, nombre);
		ELSE
			RAISE NOTICE 'Modificando caja';
			UPDATE tbCajaCaja
			SET		cajaNbr = nombre
			WHERE	cajaCod = codcaja;
		END IF;
		RETURN QUERY
		SELECT 1 AS codigo, 'La caja grabada correctamente.' AS descripcion;
	EXCEPTION WHEN OTHERS THEN
		RETURN QUERY
		SELECT 0 AS codigo, 'La caja no pudo ser creada.' AS descripcion;
	END;
END;
$$
LANGUAGE 'plpgsql';