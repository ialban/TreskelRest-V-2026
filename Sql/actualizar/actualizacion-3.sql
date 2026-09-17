ALTER TABLE tbCtrlCaja ADD COLUMN ctrlefc NUMERIC(10,2);
ALTER TABLE tbCtrlCaja ADD COLUMN ctrltrj NUMERIC(10,2);
ALTER TABLE tbCtrlCaja ADD COLUMN ctrlchq NUMERIC(10,2);

ALTER TABLE tbCtrlCierre ADD COLUMN ctrlefc NUMERIC(10,2);
ALTER TABLE tbCtrlCierre ADD COLUMN ctrltrj NUMERIC(10,2);
ALTER TABLE tbCtrlCierre ADD COLUMN ctrlchq NUMERIC(10,2);

DROP FUNCTION public.fnbuscainiciocierre(character varying, character varying);

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
BEGIN
	/*
	SELECT * FROM fnBuscaInicioCierre('2023-01-19','0001');
	*/
	SELECT desencryptar(confVal) INTO _cierreAcum FROM tbConfParam WHERE confNid = 42;
	
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
		WHERE 	c.ctrlfchini = fecha::DATE;
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
		WHERE 	c.ctrlfchini = fecha::DATE 
				AND c.cajacod=caja;
	END IF;
END;
$BODY$;

CREATE OR REPLACE FUNCTION public.fngrabaretencion(
	p_accion character varying,
	p_retescl character varying,
	p_ventscl character varying,
	p_cajacod character varying,
	p_usuacod character varying,
	p_retetip character varying,
	p_retefch date,
	p_cliente character varying,
	p_docutip character varying,
	p_docusri character varying,
	p_docunau character varying,
	p_retetot numeric,
	p_reteetd character varying,
	p_detalle text)
    RETURNS TABLE(codigo integer, descripcion text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
    secRete VARCHAR(10);
BEGIN
/*
-- Agregar una nueva retencion
SELECT * FROM fnGrabaRetencion('N'::character varying, 
	'-1'::character varying, '0000000001'::character varying, 
	'0001'::character varying, '0011'::character varying, 
	'C'::character varying, '2023-01-18'::Date, 
	'0914556634001'::character varying, 'R'::character varying, 
	'001-001-000000017'::character varying, ''::character varying, 
	1.7999999::numeric, 'A'::character varying, 
	'[{"codigo":"002","codsri":"9","aniofisc":2023,"tipoimpo":"IVA","baseimpo":18.0,"pctrete":10.0,"valorrete":1.8}]'::text)
*/
	BEGIN
		RAISE NOTICE 'ANTES DEL IF';
		IF p_accion = 'N' THEN
			RAISE NOTICE 'CREANDO RETENCION';
			select LPAD((COALESCE(MAX(retescl::int),0)+1)::TEXT,10,'0') INTO secRete FROM tbPeddDRete;
			
			RAISE NOTICE 'SECUENCIAL = %', secRete;
			
			RAISE NOTICE 'ANTES DEL INSERT';
			RAISE NOTICE 'FECHA %', p_retefch;
			
			RAISE NOTICE 'VIENDO DETALLE';
			RAISE NOTICE 'SECUENCIAL = %', secRete;
			RAISE NOTICE 'CAJA       = %', p_cajacod;
			RAISE NOTICE 'USUARIO    = %', p_usuacod;
			RAISE NOTICE 'TIPO       = %', p_retetip;
			RAISE NOTICE 'FECHA      = %', p_retefch;
			RAISE NOTICE 'PROVEEDOR  = %', p_cliente;
			RAISE NOTICE 'TIPO DOC P = %', p_docutip;
			RAISE NOTICE 'NO FACTURA = %', p_docusri;
			RAISE NOTICE 'AUTORIZACI = %', COALESCE(p_docunau,'***');
			RAISE NOTICE 'TOTAL      = %', p_retetot;
			RAISE NOTICE 'FECHA GRAB = %', CURRENT_DATE;
			
			INSERT INTO tbPeddDRete 
				(retescl, cajacod, usuacod, retetip, retefch, nidnid, docutip, docuser, docusri, 
				 docunau, retetot, retefcr, reteetd)
			VALUES
				(secRete, p_cajacod, p_usuacod, p_retetip, p_retefch, p_cliente, p_docutip, p_ventscl, p_docusri, 
				 p_docunau, p_retetot, CURRENT_DATE, 'A');
			
			RAISE NOTICE 'ANTES DE OBTENER EL DETALLE';
			
			INSERT INTO tbPeddMRete
			SELECT 	secRete AS retescl
					,t.aniofisc AS aniofis
					,CASE 
						WHEN UPPER(t.tipoimpo) = 'RENTA' THEN 'R'
						ELSE 'I'
					END AS retetip
					,t.codigo AS retecod
					,t.codsri AS retesri
					,t.baseimpo AS retebas
					,t.pctrete AS retepct
					,t.valorrete AS reteval
			FROM JSON_TO_RECORDSET(p_detalle::json) AS
					t(aniofisc INT, tipoimpo VARCHAR(10), codigo VARCHAR(3), codsri VARCHAR(5), 
					baseimpo NUMERIC(10,2), pctrete NUMERIC(10,2), valorrete NUMERIC(10,2));
						
			RAISE NOTICE 'LUEGO DE GRABAR EL DETALLE';
		ELSE
			-- HAY QUE CORREGIR LA MODIFICACIÓN DE LA RETENCIÓN
			RAISE NOTICE 'Modificando RETENCION';
			UPDATE 	tbReserv
			SET		nidnid  = p_cliente,
					resecnt = p_cantidad,
					resedes = p_desde,
					resehas = p_hasta,
					resemot = p_motivo,
					resenot = p_nota,
					resemes = p_mesas,
					reseetd = p_estado
			WHERE	resenid = p_resescl;
		END IF;
		RETURN QUERY
		SELECT 1 AS codigo, 'La Retención fue grabada correctamente.' AS descripcion;
	EXCEPTION WHEN OTHERS THEN
		RAISE NOTICE 'ERROR %', SQLSTATE;
		RAISE NOTICE 'ERROR DETAIL = %', SQLERRM;
		RETURN QUERY
		SELECT 0 AS codigo, 'La Retención no pudo ser grabada.' AS descripcion;
	END;
END;
$BODY$;

DROP FUNCTION public.fnlstcontrol(character varying, character varying);

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
BEGIN
	/*
	-- Control por cajas
	UPDATE tbConfParam set confval = encryptar('0') where confnid = 42;
	SELECT * FROM fnlstcontrol('2023-01-18','0001');
	
	-- Control acumulativo
	UPDATE tbConfParam set confval = encryptar('1') where confnid = 42;
	SELECT * FROM fnlstcontrol('2023-01-18','0001')
	*/
	SELECT desencryptar(confval)::boolean INTO _incVentCarrier FROM tbConfParam WHERE confnid=41;
	SELECT desencryptar(confval)::boolean INTO _acumulativo FROM tbConfParam WHERE confnid=42;
	
	IF _acumulativo = true THEN
		RAISE NOTICE 'OBTENIENDO EL CONTROL DE CAJA ACUMULATIVO';
		SELECT SUM(vaegval) INTO _egresos FROM tbCajaVaEg WHERE vaegfch=fecha::DATE;
		SELECT SUM(vaigval) INTO _ingresos FROM tbCajaVaIg WHERE vaigfch=fecha::DATE;
		
		IF _incVentCarrier = true THEN
			SELECT 	SUM(venttot) INTO _ventas 
			FROM 	tbPeddDVent 
			WHERE 	ventfch=fecha::DATE 
					AND ventStd IN ('F','P');
		ELSE
			SELECT 	SUM(venttot) INTO _ventas 
			FROM 	tbPeddDVent 
			WHERE 	ventfch=fecha::DATE 
					AND ventStd IN ('F','P') AND ventcrr = '0000';	
		END IF;

		SELECT 	COALESCE(SUM(comptot),0) INTO _compras 
		FROM	tbCompDComp 
		WHERE 	compfch=fecha::DATE AND compAtv=true AND usecaja = true;
		
		SELECT 	ctrlSec, ctrlVin, ctrlVcr, ctrlefc, ctrltrj, ctrlchq INTO _secuencial, _inicio, _cierre, _efectivo, _tarjeta, _cheque 
		FROM 	tbCtrlCierre 
		WHERE 	ctrlFchIni=fecha::DATE;
	ELSE
		RAISE NOTICE 'OBTENIENDO EL CONTROL DE CAJA POR CAJA';
		SELECT SUM(vaegval) INTO _egresos FROM tbCajaVaEg WHERE vaegfch=fecha::DATE AND cajacod=caja;
		SELECT SUM(vaigval) INTO _ingresos FROM tbCajaVaIg WHERE vaigfch=fecha::DATE AND cajacod=caja;
		
		IF _incVentCarrier = true THEN
			RAISE NOTICE 'OBTENIENTO TOTAL VENTAS CON DELIVERY INCLUIDO';
			SELECT 	SUM(venttot) INTO _ventas 
			FROM 	tbPeddDVent 
			WHERE 	ventfch=fecha::DATE AND cajacod=caja 
					AND ventStd IN ('F','P');
		ELSE
			RAISE NOTICE 'OBTENIENTO TOTAL VENTAS SIN DELIVERY';
			SELECT 	SUM(venttot) INTO _ventas 
			FROM 	tbPeddDVent 
			WHERE 	ventfch=fecha::DATE AND cajacod=caja 
					AND ventStd IN ('F','P') AND ventcrr = '0000';	
		END IF;

		RAISE NOTICE 'OBTENIENTO TOTAL COMPRAS';
		SELECT 	SUM(comptot) INTO _compras 
		FROM 	tbCompDComp 
		WHERE compfch=fecha::DATE AND codcaja=caja AND compAtv=true;
		
		RAISE NOTICE 'OBTENIENTO DATOS DEL CIERRE DE CAJA';
		SELECT 	ctrlSec, ctrlVin, ctrlVcr, ctrlefc, ctrltrj, ctrlchq INTO _secuencial, _inicio, _cierre, _efectivo, _tarjeta, _cheque
		FROM 	tbCtrlCaja 
		WHERE 	ctrlFchIni=fecha::DATE 
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

DROP FUNCTION public.fnlstcontrolcompvent(character varying, character varying, character varying);

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
				ctrlvin AS valor,
				0 AS diferencia,
				'' AS tipo
		FROM 	tbCtrlCierre 
		WHERE 	ctrlfchIni = fecha::DATE;
		
		INSERT INTO tmp_Resultado
		SELECT 	'CC' AS origen,
				ctrlhfn AS hora,
				ctrlsec,
				'' as otipo, 
				ctrlvcr AS valor,
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
		FROM 	tbCtrlCaja 
		WHERE 	ctrlfchIni = fecha::DATE;
				
		INSERT INTO tmp_Resultado
		SELECT 	'CT' AS origen,
				TO_CHAR(ctrlhfn,'HH24:MI:SS') AS hora,
				ctrlsec,
				'' as otipo, 
				COALESCE(ctrltrj,0) AS valor,
				0 AS diferencia,
				'' AS tipo
		FROM 	tbCtrlCaja 
		WHERE 	ctrlfchIni = fecha::DATE;
				
		INSERT INTO tmp_Resultado
		SELECT 	'CQ' AS origen,
				TO_CHAR(ctrlhfn,'HH24:MI:SS') AS hora,
				ctrlsec,
				'' as otipo, 
				COALESCE(ctrlchq,0) AS valor,
				0 AS diferencia,
				'' AS tipo
		FROM 	tbCtrlCaja 
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
				ctrlvin AS valor,
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
				ctrlvcr AS valor,
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
	END IF;
	
	RETURN QUERY
	SELECT * FROM tmp_Resultado;
	
	DROP TABLE tmp_Resultado;
	
END;
$BODY$;