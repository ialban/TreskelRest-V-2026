INSERT INTO tbConfParam VALUES (68, 'Empresa modo RIMPE', encryptar('0'), true);

INSERT INTO tbConfParam VALUES (69,'Tamaño botones menú Pedido', encryptar('1'),true);
INSERT INTO tbConfParam VALUES (70,'Tamaño botones menú Facturas', encryptar('1'),true);
INSERT INTO tbConfParam VALUES (71,'Cantidad de caracteres para decimal Codigo Barras',encryptar('3'),true);
INSERT INTO tbConfParam VALUES (72,'Servicio de atención al cliente', encryptar('10'), true);
INSERT INTO tbConfParam VALUES (73,'Identificación del cálculo del servicio', encryptar('P'), true);

INSERT INTO tbConfParam VALUES (74,'Código a utilizar en Barcode para indicar producto por peso', encryptar('210'), true);
INSERT INTO tbConfParam VALUES (75,'Cantidad de dígitos para definir el código en un Barcode', encryptar('6'), true);
INSERT INTO tbConfParam VALUES (76,'Cantidad de dígitos de la parte entera del peso en Barcode', encryptar('3'), true);
INSERT INTO tbConfParam VALUES (77,'Tipo de código de barras', encryptar('1'), true); -- 1 = EAN-13 (13), 2 = UPC-A (12)

-- Configuración para impresión en POS
INSERT INTO tbConfParam VALUES (78,'Impresión: Separador', encryptar('-'), true); 
INSERT INTO tbConfParam VALUES (79,'Impresión: Cantidad de columnas', encryptar('48'), true); 
INSERT INTO tbConfParam VALUES (80,'Impresión: Imprime Nombre de empresa', encryptar('1'), true); 
INSERT INTO tbConfParam VALUES (81,'Impresión: Imprime Dirección de empresa', encryptar('1'), true);
INSERT INTO tbConfParam VALUES (82,'Impresión: Imprime RUC', encryptar('1'), true);
INSERT INTO tbConfParam VALUES (83,'Impresión: Usa Negrita en cabecera', encryptar('1'), true);
INSERT INTO tbConfParam VALUES (84,'Impresión: Alineación de la cabecera', encryptar('I'), true); -- I = IZQUIERDA, D = DERECHA, C = CENTRO
INSERT INTO tbConfParam VALUES (85,'Impresión: Imprime separador al final de la cabecera', encryptar('1'), true);

INSERT INTO tbConfParam VALUES (86,'Impresión: Imprime cabecera en negrita del contenido', encryptar('1'), true);
INSERT INTO tbConfParam VALUES (87,'Impresión: Lineas a saltar al final de los items', encryptar('5'), true);
INSERT INTO tbConfParam VALUES (88,'Impresión: Imprime separador al final de los items', encryptar('1'), true);
INSERT INTO tbConfParam VALUES (89,'Impresión: Cantidad de decimales a imprimir en los items', encryptar('4'), true);
INSERT INTO tbConfParam VALUES (90,'Impresión: Imprime la forma de pago', encryptar('1'), true);
INSERT INTO tbConfParam VALUES (91,'Impresión: Incluye información del mesero', encryptar('1'), true);
INSERT INTO tbConfParam VALUES (92,'Impresión: Incluye agradecimiento', encryptar('1'), true);

INSERT INTO tbConfParam VALUES (93, 'Imprime ticket de cierre de caja', encryptar('1'), true);
INSERT INTO tbConfParam VALUES (94, 'Validar compra existente en retenciones', encryptar('0'), true);

update tbConfParam set confval = encryptar('0') where confnid = 72;
update tbConfParam set confval = encryptar('') where confnid = 73;

DROP TABLE IF EXISTS tbPeddMRete;
DROP TABLE IF EXISTS tbPeddDRete;

CREATE TABLE tbPeddDRete(
	reteScl VARCHAR(10) NOT NULL,
	cajaCod VARCHAR(4) NOT NULL,
	usuaCod VARCHAR(4) NOT NULL,
	reteTip VARCHAR(1) NOT NULL DEFAULT 'C' CHECK (reteTip IN ('C','V')),
	reteFch DATE NOT NULL,
	nidNid  VARCHAR(25) NOT NULL,
	docuTip VARCHAR(1) NOT NULL DEFAULT 'F',
	docuSer VARCHAR(10), -- Secuencial de la compra o venta
	docuSri VARCHAR(17) NOT NULL, -- numero de factura ###-###-#########
	docuNAu VARCHAR(49),
	reteTot NUMERIC(14,6) NOT NULL,
	reteFcr TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT CURRENT_DATE,
	reteEtd VARCHAR(1) NOT NULL DEFAULT 'A',
	CONSTRAINT pk_PeddDRete PRIMARY KEY (reteScl),
	CONSTRAINT fk_PeddDReteCaja FOREIGN KEY (cajaCod) REFERENCES tbCajaCaja (cajaCod),
	CONSTRAINT fk_PeddDReteUsua FOREIGN KEY (usuaCod) REFERENCES tbAdsiUsua (usuaCod)
);

COMMENT ON COLUMN tbPeddDRete.reteTip IS 'Tipo de retencion: C = COMPRA, V = VENTA';
COMMENT ON COLUMN tbPeddDRete.docuTip IS 'Tipo de Documento: F = Factura, N = Nota de Venta, L = Liquidaciones y prestación de servicios, T = Tickets, O = Otros';
COMMENT ON COLUMN tbPeddDRete.docuSri IS 'Serial del número del documento ###-###-########## son los primeros 6 dígitos, ejemplo 001-001';
COMMENT ON COLUMN tbPeddDRete.docuNAu IS 'Número de autorización en caso de provenir de factura electrónica';
COMMENT ON COLUMN tbPeddDRete.reteFch IS 'Fecha de emisión de la retención';
COMMENT ON COLUMN tbPeddDRete.reteFcr IS 'Fecha de creación de la retención';
COMMENT ON COLUMN tbPeddDRete.reteEtd IS 'Estado de la retención A = Activo, I = Inactivo, L = Anulado/Eliminado';

CREATE TABLE tbPeddMRete(
	reteScl VARCHAR(10),
	anioFis VARCHAR(7),
	reteTip VARCHAR(1) NOT NULL DEFAULT 'I',
	reteCod VARCHAR(3) NOT NULL,
	reteSri VARCHAR(5),
	reteBas NUMERIC(10,2) NOT NULL,
	retePct DECIMAL(10,2) NOT NULL,
	reteVal DECIMAL(10,2) NOT NULL,
	CONSTRAINT fk_PeddMReteDRete FOREIGN KEY (reteScl) REFERENCES tbPeddDRete (reteScl)
);

CREATE TABLE tbReteElect(
	retescl VARCHAR(10) NOT NULL,
	autosri VARCHAR(100),
	retetip VARCHAR(1),
	numefac VARCHAR(50),
	secuencial VARCHAR(9) NOT NULL,
	etdofac VARCHAR(1) NOT NULL DEFAULT 'N',
	etdoeml VARCHAR(1),
	fechenv TIMESTAMP WITHOUT TIME ZONE,
	fechpro TIMESTAMP WITHOUT TIME ZONE,
	mensaje TEXT,
	CONSTRAINT fk_ReteElect_PeddDRete FOREIGN KEY (retescl) REFERENCES tbPeddDRete(retescl)
);


ALTER TABLE public.tbctrlcaja ADD COLUMN ctrlchq numeric(10, 2);
ALTER TABLE public.tbctrlcaja ADD COLUMN ctrlefc numeric(10, 2);
ALTER TABLE public.tbctrlcaja ADD COLUMN ctrltrj numeric(10, 2);
ALTER TABLE public.tbctrlcaja DROP CONSTRAINT uk_ctrlcajafecha;

ALTER TABLE public.tbctrlcierre ADD COLUMN ctrlchq numeric(10, 2);
ALTER TABLE public.tbctrlcierre ADD COLUMN ctrlefc numeric(10, 2);
ALTER TABLE public.tbctrlcierre ADD COLUMN ctrltrj numeric(10, 2);

ALTER TABLE public.tbivtritem ADD COLUMN barcode character varying(20);
ALTER TABLE public.tbivtritem ADD COLUMN itempara character varying(1) DEFAULT 'T'::character varying;
ALTER TABLE public.tbivtritem ADD COLUMN itemund character varying(4) DEFAULT 'UND'::character varying;
	
ALTER TABLE public.tbpedddvent ADD COLUMN ventstn numeric(14, 6) DEFAULT 0;	

	

CREATE OR REPLACE FUNCTION public.actualizafactelect(
	p_ventscl character varying,
	p_secuencial character varying,
	p_claveacceso character varying,
	p_estado character varying,
	p_estadomail character varying,
	p_mensaje character varying)
    RETURNS TABLE(codigo integer, descripcion text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
	sclRete VARCHAR(3);
	l_context text;
BEGIN
	BEGIN
		UPDATE tbFactelect
		SET	etdofac = p_estado,
			etdoeml = p_estadomail,
			fechpro = CURRENT_DATE,
			mensaje = p_mensaje
		WHERE ventscl = p_ventscl 
				AND autosri = p_claveacceso
				AND secuencial = p_secuencial;

		RETURN QUERY
			SELECT 1 AS codigo, 'Factura creada' AS descripcion;	
	EXCEPTION WHEN OTHERS THEN
		GET STACKED DIAGNOSTICS l_context = PG_EXCEPTION_CONTEXT;
    	RAISE NOTICE 'ERROR:%', l_context;
		
		RETURN QUERY
			SELECT 0 AS codigo, 'No se pudo crear la factura' AS descripcion;	
	END;
END
$BODY$;

CREATE OR REPLACE FUNCTION public.actualizafactelect(
	p_ventscl character varying,
	p_secuencial character varying,
	p_claveacceso character varying,
	p_estado character varying,
	p_estadomail character varying,
	p_mensaje text)
    RETURNS TABLE(codigo integer, descripcion text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
	sclRete VARCHAR(3);
	l_context text;
BEGIN
	BEGIN
		UPDATE tbFactelect
		SET	etdofac = p_estado,
			etdoeml = p_estadomail,
			fechpro = CURRENT_DATE,
			mensaje = p_mensaje
		WHERE ventscl = p_ventscl 
				AND autosri = p_claveacceso
				AND secuencial = p_secuencial;

		RETURN QUERY
			SELECT 1 AS codigo, 'Factura creada' AS descripcion;	
	EXCEPTION WHEN OTHERS THEN
		GET STACKED DIAGNOSTICS l_context = PG_EXCEPTION_CONTEXT;
    	RAISE NOTICE 'ERROR:%', l_context;
		
		RETURN QUERY
			SELECT 0 AS codigo, 'No se pudo crear la factura' AS descripcion;	
	END;
END
$BODY$;

CREATE OR REPLACE FUNCTION public.actualizareteelect(
	p_retescl character varying,
	p_secuencial character varying,
	p_claveacceso character varying,
	p_estado character varying,
	p_estadomail character varying,
	p_mensaje text)
    RETURNS TABLE(codigo integer, descripcion text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
	sclRete VARCHAR(3);
	l_context text;
BEGIN
	BEGIN
		UPDATE tbReteElect
		SET	etdofac = p_estado,
			etdoeml = p_estadomail,
			fechpro = CURRENT_DATE,
			mensaje = p_mensaje
		WHERE retescl = p_retescl 
				AND autosri = p_claveacceso
				AND secuencial = p_secuencial;

		RETURN QUERY
			SELECT 1 AS codigo, 'Retención actualizada' AS descripcion;	
	EXCEPTION WHEN OTHERS THEN
		GET STACKED DIAGNOSTICS l_context = PG_EXCEPTION_CONTEXT;
    	RAISE NOTICE 'ERROR:%', l_context;
		
		RETURN QUERY
			SELECT 0 AS codigo, 'No se pudo actualizar la retención' AS descripcion;	
	END;
END
$BODY$;

CREATE OR REPLACE FUNCTION public.creafactelect(
	p_ventscl character varying,
	p_secuencial character varying,
	p_claveacceso character varying)
    RETURNS TABLE(codigo integer, descripcion text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
	sclRete VARCHAR(3);
BEGIN
	BEGIN
		IF NOT EXISTS(SELECT 1 FROM tbFactElect WHERE ventscl = p_ventscl AND secuencial = p_secuencial) THEN
			INSERT INTO tbFactelect (ventscl, autosri, secuencial, fechenv)
			VALUES(
				p_ventscl, p_claveacceso, p_secuencial, CURRENT_DATE
			);
		END IF;

		RETURN QUERY
			SELECT 1 AS codigo, 'Factura creada' AS descripcion;	
	EXCEPTION WHEN OTHERS THEN
		RETURN QUERY
			SELECT 0 AS codigo, 'No se pudo crear la factura' AS descripcion;	
	END;
END
$BODY$;

CREATE OR REPLACE FUNCTION public.creareteelect(
	p_retescl character varying,
	p_secuencial character varying,
	p_retetip character varying,
	p_claveacceso character varying)
    RETURNS TABLE(codigo integer, descripcion text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
	sclRete VARCHAR(3);
BEGIN
	BEGIN
		IF NOT EXISTS(SELECT 1 FROM tbReteElect WHERE ventscl = p_ventscl AND secuencial = p_secuencial) THEN
			INSERT INTO tbReteElect (retescl, autosri, retetip, secuencial, fechenv)
			VALUES(
				p_retescl, p_claveacceso, p_retetip, p_secuencial, CURRENT_DATE
			);
		END IF;

		RETURN QUERY
			SELECT 1 AS codigo, 'Retención creada' AS descripcion;	
	EXCEPTION WHEN OTHERS THEN
		RETURN QUERY
			SELECT 0 AS codigo, 'No se pudo crear la Retención' AS descripcion;	
	END;
END
$BODY$;

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

CREATE OR REPLACE FUNCTION public.fncaja(
	accion character varying,
	codcaja character varying,
	nombre character varying)
    RETURNS TABLE(codigo integer, descripcion text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
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
$BODY$;

CREATE OR REPLACE FUNCTION public.fncompras(
	accion character varying,
	usacajachica boolean,
	data character varying,
	detalle character varying,
	pagos character varying)
    RETURNS TABLE(codigo integer, descripcion text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
	secComp VARCHAR(10);
	usuario VARCHAR(4);	
	fecha DATE;	
	
	compfac VARCHAR(17);
	provnid VARCHAR(20);
	codcaja VARCHAR(4);
	compsbt NUMERIC(10,2);
	compiva NUMERIC(10,2);
	comptot NUMERIC(10,2);
	compatv BOOLEAN;
	
	CONTROL_INV BOOLEAN := FALSE;
	reg RECORD;	
	CINV CURSOR FOR 
	select p.prdcod AS codigo, 
			fnConvertUnidades(d.compcnt, d.compund, p.prdund) AS cantidad,
			p.prdund AS unidad
	from (
	SELECT t.prdcod, t.compcnt, t.compund, t.compprc, t.comppto
				FROM JSON_TO_RECORDSET(detalle::json) AS
						t(prdcod VARCHAR(20), compcnt NUMERIC(10,2), compund varchar(1), 
						compprc NUMERIC(10,2), comppto NUMERIC(10,2))
	) d
	inner join tbIvtrPrd p ON d.prdcod = p.prdcod;
BEGIN
	RAISE NOTICE 'accion a realizar %', accion;
	IF accion = 'N' THEN
		RAISE NOTICE 'iniciando transaccion de creado de COMPRA';
		BEGIN	
			RAISE NOTICE 'obteniendo secuencial';
			
			SELECT LPAD(COALESCE(MAX(compNid::int)+1,1)::TEXT,10,'0') INTO secComp FROM tbCompDComp;
			
			RAISE NOTICE 'secuencial %', secComp;
			RAISE NOTICE 'data = %', data;
			
			fecha := CURRENT_DATE;
			RAISE NOTICE 'despues de obtener la fecha %', fecha;
			
			SELECT	t.codempr, t.numfact, t.fecha, t.subtot, t.iva, t.total, t.compatv, t.codcaja
					INTO provnid, compfac, fecha, compsbt, compiva, comptot, compatv, codcaja
            FROM 	json_to_record(data::json) AS 
				  	t(codempr VARCHAR(20),numfact VARCHAR(17), fecha DATE, subtot NUMERIC(10,2), 
					  iva NUMERIC(10,2), total NUMERIC(10,2), compatv BOOLEAN, codcaja VARCHAR(4));
			
			RAISE NOTICE 'despues de obtener los datos de DATA';
			INSERT INTO tbCompDComp
				(compnid, compfac, provnid, compfch, compsbt, compiva, comptot, compatv, codcaja, useCaja)
			VALUES (secComp, compfac, provnid, fecha, compsbt, compiva, comptot, true, codcaja, COALESCE(usaCajaChica, false));
					
			

			RAISE NOTICE 'paso el INGRESO DE DATOS PRIMARIOS';
			RAISE NOTICE 'detalle = %', detalle;
			
			RAISE NOTICE 'antes del ingreso DEL DETALLE';
			
			INSERT INTO tbCompMComp
			SELECT 	secComp, t.itemcod, t.itemcnt, t.itemund, t.itemprc, t.itempto
			FROM 	JSON_TO_RECORDSET(detalle::json) AS
					t(itemcod VARCHAR(6), itemcnt NUMERIC(10,2), itemund varchar(10), itemprc NUMERIC(10,2), itempto NUMERIC(10,2));
					
			RAISE NOTICE 'PASO EL INGRESO DEL DETALLE';
			RAISE NOTICE 'ANTES DEL INGRESO DE PAGOS';
			RAISE NOTICE 'secuencial %', secComp;
			
			INSERT INTO tbCompFpgo
			SELECT secComp, 
					CASE t.isefectivo 
						WHEN TRUE THEN 'E' 
						ELSE 
							CASE t.ischeque 
								WHEN TRUE THEN 'C'
								ELSE 'T' 
							END
					END AS fpgotip, 
					t.valor, 
					t.tarjeta, 
			        CASE WHEN t.iscorriente = TRUE THEN 'C'
				         WHEN t.isdiferido = TRUE THEN 'D'
					 	 ELSE 'N'
				    END AS fpgotpt, t.meses,
					t.banco, t.numcheque
            FROM JSON_TO_RECORDSET(pagos::JSON) AS
					t(isefectivo BOOLEAN, ischeque BOOLEAN, valor DECIMAL(14,6), iscorriente BOOLEAN, 
					  isdiferido BOOLEAN, meses INT, tarjeta VARCHAR(1), banco VARCHAR(2), numcheque VARCHAR(6));

			RAISE NOTICE 'DESPUES DEL INGRESO DE PAGOS';			
			RAISE NOTICE 'EJECUTANDO CONTROL DE INVENTARIO';
			
			select case desencryptar(confval) when '1' then true else false end INTO CONTROL_INV 
			from tbConfParam where confNid=12;
			
			RAISE NOTICE 'PARAMETRO CONTROL INV = %', CONTROL_INV;
			IF CONTROL_INV = TRUE THEN
				FOR reg IN CINV LOOP
					RAISE NOTICE 'COD = %, CANT=%', reg.codigo, reg.cantidad;

					INSERT INTO tbIvtrDInvMT
					VALUES (reg.codigo, 'Compra de productos según factura '||compfac, 'I', current_date, reg.cantidad, reg.unidad);
					call fnKardex(reg.codigo,2019,2,reg.cantidad,0);
				END LOOP;
			END IF;
			--COMMIT;			

			RAISE NOTICE 'finalizo correctamente y retornará mensaje';
			RETURN QUERY
				SELECT 1 AS codigo, 'Compra creada correctamente con el secuencial '|| secComp::VARCHAR AS descripcion;			
		EXCEPTION WHEN OTHERS THEN
			RAISE NOTICE 'HUBO un error y realizará un rollback';
			RAISE NOTICE 'Codio de error = %', SQLSTATE;
			--ROLLBACK;
			
			SELECT 0 AS codigo, 'La Compra no pudo ser creada' AS descripcion;
		END;
	END IF;
	
	IF accion = 'A' THEN
		BEGIN
			RAISE NOTICE 'VA A ANULAR UNA compra';
			secComp := data;
			RAISE NOTICE 'OBTENIENDO EL SECUENCIAL';
			UPDATE tbCompDComp
			SET 	compAtv = false --, ventObv=detalle
			WHERE	compNid = data;
			RAISE NOTICE 'LUEGO DE ANULAR LA COMPRA';
			
			RETURN QUERY
				SELECT 1 AS codigo, 'Compra con el secuencial '|| secComp || ' ha sido anulado correctamente' AS descripcion;	
		EXCEPTION WHEN OTHERS THEN
			RAISE NOTICE 'HUBO un error y realizará un rollback';
			RAISE NOTICE 'Codio de error = %', SQLSTATE;
			
			SELECT 0 AS codigo, 'Compra con el secuencial '|| secComp ||' no pudo ser anulado' AS descripcion;
		END;
	END IF;	
END;
$BODY$;

CREATE OR REPLACE FUNCTION public.fncontrolcaja(
	tipo character varying,
	caja character varying,
	secuencial character varying,
	fecha character varying,
	hora character varying,
	valor character varying,
	sistema character varying,
	diferencia character varying)
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
$BODY$;

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

CREATE OR REPLACE FUNCTION public.fngrabaretencion(
	accion character varying,
	ventscl character varying,
	cajacod character varying,
	usuacod character varying,
	retetip character varying,
	retefch date,
	cliente character varying,
	docutip character varying,
	docusir character varying,
	docunau character varying,
	retetot numeric,
	detalle text)
    RETURNS TABLE(codigo integer, descripcion text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
    secRete VARCHAR(10);
BEGIN
	BEGIN
		RAISE NOTICE 'ANTES DEL IF';
		IF accion = 'N' THEN
			RAISE NOTICE 'Creando Reserva';
			select LPAD((COALESCE(MAX(retescl::int),0)+1)::TEXT,10,'0') INTO secRete FROM tbPeddDRete;
			
			RAISE NOTICE 'SECUENCIAL = %', secRete;
			
			RAISE NOTICE 'ANTES DEL INSERT';
			INSERT INTO tbPeddDRete VALUES(secRete, cajacod, usuacod, retetip, retefch, cliente, docutip, docusri, docunau, retetot, CURRENT_DATE);
			
		ELSE
			RAISE NOTICE 'Modificando Reservacion';
			UPDATE 	tbReserv
			SET		nidnid  = cliente,
					resecnt = cantidad,
					resedes = desde,
					resehas = hasta,
					resemot = motivo,
					resenot = nota,
					resemes = mesas
			WHERE	resenid = codres;
		END IF;
		RETURN QUERY
		SELECT 1 AS codigo, 'La reservación fue grabada correctamente.' AS descripcion;
	EXCEPTION WHEN OTHERS THEN
		RETURN QUERY
		SELECT 0 AS codigo, 'La reservación no pudo ser grabada.' AS descripcion;
	END;
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
					t(aniofisc VARCHAR(7), tipoimpo VARCHAR(10), codigo VARCHAR(3), codsri VARCHAR(5), 
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

CREATE OR REPLACE FUNCTION public.fngrabaretenciones(
	accion character varying,
	secuencia character varying,
	codsri character varying,
	nombre character varying,
	valor numeric,
	tipo character varying,
	activo boolean)
    RETURNS TABLE(codigo integer, descripcion text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
	sclRete VARCHAR(3);
BEGIN
	IF accion = 'N' THEN
		SELECT LPAD(COALESCE(MAX(retescl::int)+1,1)::TEXT,3,'0') INTO sclRete FROM tbPctRete;
		
		INSERT INTO tbPctRete VALUES (sclRete, codsri, nombre, valor, tipo, activo);
		RETURN QUERY
			SELECT 1 AS codigo, 'Retención creada correctamente con el secuencial '|| sclRete::VARCHAR AS descripcion;	
	ELSE
		UPDATE 	tbPctRete
		SET		reteSri = codsri,
				reteDsc = nombre,
				reteVal = valor,
				reteApl = tipo,
				reteAct = activo
		WHERE	retescl = secuencia;
		
		RETURN QUERY
			SELECT 1 AS codigo, 'Retención actualizada correctamente con el secuencial '|| secuencia::VARCHAR AS descripcion;	
	END IF;
	
END
$BODY$;

CREATE OR REPLACE FUNCTION public.fngrabreserva(
	accion character varying,
	codres character varying,
	cliente character varying,
	cantidad integer,
	desde character varying,
	hasta character varying,
	motivo character varying,
	nota character varying,
	mesas character varying)
    RETURNS TABLE(codigo integer, descripcion text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
    secRese VARCHAR(10);
BEGIN
	BEGIN
		RAISE NOTICE 'ANTES DEL IF';
		IF accion = 'N' THEN
			RAISE NOTICE 'Creando Reserva';
			select LPAD((COALESCE(MAX(reseNid::int),0)+1)::TEXT,10,'0') INTO secRese FROM tbReserv;
			
			RAISE NOTICE 'SECUENCIAL = %', secRese;
			
			RAISE NOTICE 'ANTES DEL INSERT';
			INSERT INTO tbReserv 
			VALUES(
				secRese, 
				cliente, 
				cantidad, 
				desde::TIMESTAMP WITHOUT TIME ZONE, 
				hasta::TIMESTAMP WITHOUT TIME ZONE, 
				motivo, 
				nota::TEXT, 
				mesas::TEXT
			);
		ELSE
			RAISE NOTICE 'Modificando Reservacion';
			UPDATE 	tbReserv
			SET		nidnid  = cliente,
					resecnt = cantidad,
					resedes = desde,
					resehas = hasta,
					resemot = motivo,
					resenot = nota,
					resemes = mesas
			WHERE	resenid = codres;
		END IF;
		RETURN QUERY
		SELECT 1 AS codigo, 'La reservación fue grabada correctamente.' AS descripcion;
	EXCEPTION WHEN OTHERS THEN
		RETURN QUERY
		SELECT 0 AS codigo, 'La reservación no pudo ser grabada.' AS descripcion;
	END;
END;
$BODY$;

CREATE OR REPLACE FUNCTION public.fnimportar(
	vitems text,
	vprods text)
    RETURNS TABLE(codigo integer, descripcion text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
	BEGIN
		------------ CREANDO LOS GRUPOS ---------------------
		RAISE NOTICE 'CREANDO GRUPOS';
		INSERT INTO tbIvtrGrp
		SELECT 	LPAD((b.rownum::INT + b.maxcod)::VARCHAR,4,'0') AS grpcod,
				b.grpnbr,
				b.grpatv
		FROM (
			SELECT 	
					ROW_NUMBER() OVER (ORDER BY a.grupo) AS ROWNUM,
					a.grupo AS grpnbr, 
					true AS grpatv,
					(SELECT COALESCE(MAX(grpcod),'0')::INT FROM tbIvtrGrp) AS MAXCOD
			FROM (
				SELECT 	DISTINCT 
						UPPER(t.grupo) AS grupo
				FROM 	JSON_TO_RECORDSET(vitems::json) AS
						t(grupo VARCHAR(100))
				WHERE	t.grupo NOT IN (SELECT grpnbr FROM tbIvtrGrp)
			) a
		) b;

		------------- CREANDO SUBGRUPOS ----------------------
		RAISE NOTICE 'CREANDO SUB-GRUPOS';
		INSERT INTO tbIvtrSgrp
		SELECT 	LPAD((b.rownum::INT + b.maxcod)::VARCHAR,4,'0') AS grpcod,
				b.sgrpnbr,
				b.grpcod
		FROM (
			SELECT 	
					ROW_NUMBER() OVER (ORDER BY a.subgrupo) AS ROWNUM,
					a.subgrupo AS sgrpnbr, 
					(SELECT grpcod FROM tbIvtrGrp WHERE grpnbr = a.grupo) AS grpcod,
					(SELECT COALESCE(MAX(sgrpcod),'0')::INT FROM tbIvtrsGrp) AS MAXCOD
			FROM (
				SELECT 	DISTINCT 
						UPPER(t.subgrupo) AS subgrupo, 
						UPPER(t.grupo) AS grupo
				FROM 	JSON_TO_RECORDSET(vitems::json) AS
						t(subgrupo VARCHAR(100), grupo VARCHAR(100))
				WHERE	t.subgrupo NOT IN (SELECT sgrpnbr FROM tbIvtrSGrp)
			) a
		) b;

		------------- CREANDO ITEMS O PRODUCTOS -----------------
		RAISE NOTICE 'CREANDO ITEMS';
		INSERT INTO tbIvtrItem
		(itemCod, sgrpCod, itemNbr, itemIva, itemprc, itempiv, itemimg, itematv, prdcod, itemcoc, itemdelcoc, itemparr, itembar, itemgiva, barcode, itemund, itempara)
		SELECT 	LPAD((b.rownum::INT + b.maxcod)::VARCHAR,4,'0') AS itemcod,
				b.sgrpcod,
				b.itemnbr,
				b.itemiva,
				b.itemprc,
				b.itempiv,
				b.itemimg,
				b.itematv,
				b.prdcod,
				b.itemcoc,
				b.itemdelcoc,
				b.itemparr,
				b.itembar,
				b.itemgiva,
				b.barcode,
				b.itemund,
				b.itempara
		FROM (
			SELECT 	
					ROW_NUMBER() OVER (ORDER BY a.itemcod) AS ROWNUM,
					(SELECT sgrpcod FROM tbIvtrSGrp WHERE UPPER(sgrpnbr) = UPPER(a.subgrupo)) AS sgrpcod,
					a.itemnbr,
					a.itemiva,
					a.itemprc,
					CASE 
						WHEN a.itemiva = TRUE THEN ROUND(a.itemprc/1.12,6)
						ELSE 0
					END AS itempiv,
					NULL::BYTEA AS itemimg,
					true AS itematv,
					NULL AS prdcod,
					a.itemcoc,
					false AS itemsbg,
					false AS itemdelcoc,
					a.itemparr,
					a.itembar,
					a.itemgiva,
					a.barcode,
					a.itemund,
					a.itempara,
					(SELECT COALESCE(MAX(itemcod),'0')::INT FROM tbIvtrItem) AS MAXCOD
			FROM (
				SELECT 	DISTINCT 
						t.itemcod, UPPER(t.itemnbr) AS itemnbr, t.subgrupo, t.grupo, t.itemprc, t.itemund, 
						t.itemgiva, t.itemiva, TRIM(t.barcode) AS barcode, t.itemcoc, t.itemparr, t.itembar, t.itempara
				FROM 	JSON_TO_RECORDSET(vitems::json) AS
						t(itemcod VARCHAR(4), itemnbr VARCHAR(100), subgrupo VARCHAR(100), grupo VARCHAR(100), itemprc NUMERIC(14,2), 
						  itemund VARCHAR(3), itemgiva BOOLEAN, itemiva BOOLEAN, barcode VARCHAR(20), itemcoc BOOLEAN, itemparr BOOLEAN, itembar BOOLEAN, itempara VARCHAR(1))
				WHERE	t.itemcod NOT IN (SELECT itemcod FROM tbIvtrItem)
			) a
		) b;	
		
		RETURN QUERY
		SELECT 1 AS codigo, 'Importación de datos finalizada correctamente.' AS descripcion;
	EXCEPTION WHEN OTHERS THEN
		RAISE NOTICE 'Codigo de error = %', SQLSTATE;
		RETURN QUERY
		SELECT 0 AS codigo, 'Hubo errores durante la importación de datos.' AS descripcion;
	END;
END;
$BODY$;

DROP FUNCTION fnitemstopeddv2(character varying,character varying);

CREATE OR REPLACE FUNCTION public.fnitemstopeddv2(
	item character varying,
	carrier character varying)
    RETURNS TABLE(codigo character varying, producto character varying, precio numeric, iva numeric, gravaiva boolean, usaiva boolean, preciocarr numeric, carrpct boolean, prdcod character varying, itemcoc boolean, itembar boolean) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
	itemcod VARCHAR(4);
	prcCarr DECIMAL(10,6);
	ptjCarr BOOLEAN;
	aplCarr VARCHAR(1);
	
	prdNbr  VARCHAR(100);
	prdPrc  DECIMAL(10,6);
	prdPro  DECIMAL(10,6); -- PRECIO ORIGINAL
	prdCrr  DECIMAL(10,6);
	prdFin  DECIMAL(10,6);
	usaIva  BOOLEAN;
	grvIva  BOOLEAN;
	prdCod  VARCHAR(6);
	prdCoc  BOOLEAN;
	prdBar	BOOLEAN;
	prcPPs  BOOLEAN; -- PRECIO POR PESO
	
	valIva  DECIMAL(10,6);
	valDec  INT;
	
	codpri  INT;
	codcod  VARCHAR(4);
	pesovl  DECIMAL(10,4);
	
BEGIN
	/*
	SELECT * FROM fnItemsToPeddV2('0007', '0000');
	
	SELECT * FROM fnItemsToPeddV2('', '0000');
	
	SELECT * FROM fnItemsToPeddV2('7868001042237', '0000');
	
	SELECT * FROM fnItemsToPeddV2('2102051016202', '0000');
	
	SELECT * FROM fnItemsToPeddV2('2106009005706', '0000');
	
	*/
	prcPPs := FALSE;
	
	IF item = '' THEN
		item := '0000';
	END IF;
	
	IF LENGTH(item) < 4 THEN
		item := LPAD(item, 4, '0');
	END IF;
	
	SELECT 	crriprc::DECIMAL(10,6), crripct, crriapi 
			INTO prcCarr, ptjCarr, aplCarr 
	FROM 	tbAdsiCrrie 
	WHERE 	crriNid = carrier AND carrier NOT IN ('0000');
	
	SELECT 	(desencryptar(confval)::DECIMAL/100)::DECIMAL(10,6) 
			INTO valIva 
	FROM 	tbConfParam 
	WHERE 	confNid=2;
	
	SELECT 	desencryptar(confval)::DECIMAL INTO valDec 
	FROM 	tbConfParam 
	WHERE 	confNid=71;

	IF LENGTH(item) > 4 THEN
		SELECT SUBSTRING(item, 1,3)::INT INTO codpri;
		IF  (codpri >=  20 and codpri <=  29) OR
			(codpri >=  40 and codpri <=  49) OR 
			(codpri >= 200 and codpri <= 299) THEN
			IF LENGTH(item) = 13 THEN
				SELECT SUBSTRING(item, 4,4) INTO codcod;
				RAISE NOTICE 'P1 = %',SUBSTRING(item, 8, 5-valDec)::DECIMAL(10,2);
				RAISE NOTICE 'P2 = %',SUBSTRING(item, 13-valDec,valDec)::DECIMAL(10,2)/(10^valdec);
				SELECT ROUND(SUBSTRING(item, 8, 5-valDec)::DECIMAL(10,2) + 
							 SUBSTRING(item, 13-valDec,valDec)::DECIMAL(10,2)/((10^valdec)::DECIMAL(10,2)),2) INTO pesovl;
			ELSE 
				SELECT SUBSTRING(item, 4,3) INTO codcod;
				RAISE NOTICE '*P1 = %',SUBSTRING(item, 7, 5-valDec)::DECIMAL(10,2);
				RAISE NOTICE '*P2 = %',SUBSTRING(item, 12-valDec,valDec)::DECIMAL(10,2)/(10^valdec);
				SELECT ROUND(SUBSTRING(item, 7, 5-valDec)::DECIMAL(10,2) + 
							 SUBSTRING(item, 12-valDec,valDec)::DECIMAL(10,2)/((10^valdec)::DECIMAL(10,2)),2) INTO pesovl;
			END IF;
		END IF;
	END IF;
	
	RAISE NOTICE 'COD PAIS = %', codpri;
	RAISE NOTICE 'COD PAIS = %', codcod;
	RAISE NOTICE 'COD PAIS = %', pesovl;
	
	IF codpri >= 200 AND codpri <= 299 THEN
		RAISE NOTICE 'PRODUCTO CON PRECIO DEPENDIENTE DEL PESO';
		item := codcod;
		prcPPs := TRUE;
	END IF;
	
	RAISE NOTICE 'ITEM = %', item;
	SELECT 	i.itemcod, i.itemNbr, 	
			CASE 
				WHEN prcPPs = TRUE THEN i.itemPrc * pesovl
				ELSE i.itemPrc
			END, 	
			i.itemIva, 	i.prdCod,  	i.itemCoc, i.itemGIva, 	i.itemPrc, i.itembar
			INTO	
			itemcod, prdNbr, 	prdPrc, 	usaIVA, 	prdCod, 	
			prdCoc,  grvIva, 	prdPro, 	prdBar
	FROM 	tbIvtrItem i 
	WHERE 	i.itemCod = item 
			OR i.barcode = item;
	
	IF usaIVA = TRUE AND grvIVA = TRUE THEN
		prdPrc := prdPrc/(1 + valIva);
	END IF;
	
	IF aplCarr = 'I' THEN
		RAISE NOTICE 'CUANDO TIENE CARRIER';
		IF ptjCarr = TRUE THEN
			prdCrr := (PrdPrc + PrdPrc*(prcCarr/100));
		ELSE
			prdCrr := PrdPrc + prcCarr;
		END IF;
		
		IF grvIva = TRUE THEN
			PrdFin := prdCrr + prdCrr*valIva;
		ELSE
			prdFin := prdCrr;
		END IF;
	
		RAISE NOTICE 'RETORNANDO DATA';
		RETURN QUERY
			SELECT 	itemcod AS codigo, 
					prdNbr AS producto, 
					prdCrr AS precio, 
					PrdFin AS IVA, 
					grvIva AS gravaIVA,
					usaIva AS usaIVA, 
					prcCarr AS precioCarr,
					ptjCarr AS carrpct, 
					prdCod AS prdCod, 
					prdCoc AS itemCoc,
					prdBar AS itemBar;
	ELSE
		RAISE NOTICE 'CUANDO NO HAY CARRIER';
		
		IF grvIva = TRUE THEN
			PrdFin := prdPrc + prdPrc*valIva;
		ELSE
			PrdFin := prdPrc;
		END IF;
			
		RAISE NOTICE 'RETORNANDO DATA';
		RETURN QUERY
			SELECT 	itemcod AS codigo,
					prdNbr AS producto, 
					prdPrc AS precio, 
					PrdFin AS IVA, 
					grvIva AS gravaIVA,
					usaIva AS usaIVA, 
					0::numeric AS precioCarr, 
					COALESCE(ptjCarr, FALSE) AS carrpct, 
					prdCod AS prdCod, 
					prdCoc AS itemcoc,
					prdBar AS itembar;
	END IF;
	
END;
$BODY$;

DROP FUNCTION fnlistpeddvent(character varying);

CREATE OR REPLACE FUNCTION public.fnlistpeddvent(
	codigo character varying)
    RETURNS TABLE(ventscl character varying, peddscl character varying, peddlta character varying, usuacod character varying, peddfec date, peddtip character varying, ventcrr character varying, peddmsa character varying, peddobv character varying, nidnid character varying, nidnbr character varying, ventfac boolean, ventsbt numeric, ventdsc numeric, ventiva numeric, venttot numeric, ventrcb numeric, ventvto numeric, crrprc numeric, crrpct boolean, clienid character varying, clienbr character varying, cliedir character varying, clietlf character varying, clieref character varying, ventstd character varying, ventsbn numeric) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
	sclVent VARCHAR(10)   = '-1';
	isFact  BOOLEAN;
	cliCod  VARCHAR(20)   = '';
	cliNbr  VARCHAR(100)  = '';
	venSbt  DECIMAL(10,2) = 0;
	venDsc 	DECIMAL(10,2) = 0;
	venIva	DECIMAL(10,2) = 0;
	venTot 	DECIMAL(10,2) = 0;
	venRcb 	DECIMAL(10,2) = 0;
	venVto	DECIMAL(10,2) = 0;
	venStd  VARCHAR(1)    = '';
	cntVnt 	INT;
BEGIN
	SELECT COUNT(*) INTO cntVnt FROM tbPeddDVent v WHERE v.peddscl = codigo;

	/*IF cntVnt > 0 THEN
	
	END IF;

	SELECT 	v.ventScl, c.nidNid, c.nidNbr, v.ventsbt, v.ventdsc, v.ventIva, 
			v.ventTot, v.ventRcb, v.ventvto, v.ventFac, v.ventStd
			INTO 
			sclVent, cliCod, cliNbr, venSbt, venDsc, venIva, 
			venTot, venRcb, venVto, isFact, venStd
	FROM 	tbPeddDVent v
			INNER JOIN
			tbAdsiNid c ON c.nidNid = v.nidnid
	WHERE
			v.peddScl=codigo;*/
		
	RETURN QUERY 
		SELECT 	COALESCE(sclVent,'-1') AS sclVent, p.peddScl, p.peddLta, 
				p.usuaCod, p.peddFec, p.peddTip, p.peddCcr, 
				p.peddMsa, p.peddObv, COALESCE(cliCod,'') AS nidNid, 
				COALESCE(cliNbr,'') AS nidNbr, COALESCE(isFact, FALSE) AS ventFac,
				COALESCE(venSbt, 0) AS ventSbt, COALESCE(venDsc, 0) AS ventDsc,
				COALESCE(venIva, 0) AS ventIva, COALESCE(venTot, 0) AS ventTot,
				COALESCE(venRcb, 0) AS ventRcb, COALESCE(venVto, 0) AS ventVto,
				(SELECT crriprc FROM tbAdsiCrrie WHERE crrinid=p.peddCcr) AS crrprc,
				(SELECT crripct FROM tbAdsiCrrie WHERE crrinid=p.peddCcr) AS crrpct,
				COALESCE(c.clieNid,'') AS clieNid, COALESCE(c.clieNbr,'') AS clieNbr, 
				COALESCE(c.clieDir,'') AS clieDir, COALESCE(c.clieTlf,'') AS clieTlf, 
				COALESCE(c.clieRef,'') AS clieRef,
				COALESCE(venStd,'') AS ventStd,
				COALESCE(ventSbn,0) AS ventSbn
		FROM 	tbPeddDPedd p 
				LEFT JOIN
				tbPeddDAtcli c ON p.peddScl = c.peddScl
		WHERE p.peddScl = codigo;

END;
$BODY$;

DROP FUNCTION fnlstcontrol(character varying,character varying);

CREATE OR REPLACE FUNCTION public.fnlstcarriers(
	nombre character varying,
	estatus character varying)
    RETURNS TABLE(codigo character varying, carrier character varying, icono character varying, valor numeric, porcentaje character varying, estado character varying, imagen bytea) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
	
BEGIN	
	RETURN QUERY
		SELECT c.crriNid AS Codigo, c.crriNbr AS carrier,
				c.crriIcn AS icono,
				c.crriPrc AS valor,
				CASE c.crriPct
					WHEN TRUE THEN 'SI'
					ELSE 'NO'
				END::varchar(2) AS porcentaje,
				CASE crriAtv
					WHEN TRUE THEN 'Activo'
					WHEN FALSE THEN 'Inactivo'
				END::varchar(10) AS estado,
				crriImg AS imagen
		FROM 	tbAdsiCrrie c
		WHERE	(nombre='' OR c.crriNbr LIKE '%'||nombre||'%') AND
				(estatus='' OR c.crriAtv=CASE estatus 
				 							WHEN 'A' THEN TRUE 
				 							WHEN 'I' THEN FALSE
				 							ELSE TRUE
										END)
				AND c.crriNid <> '0000';
				
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

CREATE OR REPLACE FUNCTION public.fnlstitem(
	vnombre character varying,
	vgrupo character varying)
    RETURNS TABLE(codigo character varying, producto text, grupo text, subgrupo text, precio numeric, estado text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
    RETURN QUERY 
	SELECT i.itemCod, 
			UPPER(i.itemNbr) AS producto,
			UPPER(g.grpNbr) AS grupo,
			UPPER(s.sgrpNbr) AS subgrupo,
			i.itemPrc AS precio,
			CASE i.itemAtv WHEN TRUE THEN 'ACTIVO' ELSE 'INACTIVO' END AS estado
	FROM tbIvtrItem i
		INNER JOIN
		tbIvtrSgrp s ON i.sgrpCod = s.sgrpCod
		INNER JOIN
		tbIvtrGrp g ON g.grpCod = s.grpCod
	WHERE 
		('' = vnombre OR 
		 i.itemNbr like vnombre OR  
		 TRIM(i.barcode) = REPLACE(TRIM(vnombre),'%','') OR 
		 i.itemcod = REPLACE(TRIM(vnombre),'%','')) AND
		('' = vgrupo OR g.grpCod = vgrupo);
	
END;
$BODY$;

CREATE OR REPLACE FUNCTION public.fnlstnid(
	vapellido character varying,
	vtipo character varying)
    RETURNS TABLE(codigo character varying, tipo text, cliente text, email character varying, direccion text, estado text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
    RETURN QUERY 
	SELECT a.nidNid AS codigo, 
			(CASE a.nidTip WHEN 'C' THEN 'CÉDULA' WHEN 'P' THEN 'PASAPORTE' ELSE 'RUC' END) AS tipo,
			UPPER(a.nidNbr)  As cliente, 
			a.nidEml AS email,
			UPPER(a.nidDir) AS direccion,
			UPPER(a.nidStdo) AS estado
	FROM tbAdsiNid a
	WHERE 
		('' = vtipo OR a.nidTip = vtipo) AND
		(('' = vapellido OR UPPER(a.nidNbr) like UPPER(vapellido)) OR
		('' = vapellido OR a.nidnid LIKE UPPER(vapellido)));
	
END;
$BODY$;

CREATE OR REPLACE FUNCTION public.fnpedidos(
	accion character varying,
	data character varying,
	detalle character varying)
    RETURNS TABLE(codigo integer, descripcion text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
	secPedd VARCHAR(10);
	mesa VARCHAR(3);
	tipo VARCHAR(1);
	carrier VARCHAR(4);
	crrcod VARCHAR(10);
	crrobv VARCHAR(100);
	letra VARCHAR(1) := NULL;
	usuario VARCHAR(4);
	secIni INT := 0;
	
	clienid VARCHAR(20);
	clienbr VARCHAR(50);
	cliedir VARCHAR(100);
	clieref VARCHAR(100);
	clietlf VARCHAR(50);	
	
	observ TEXT;
	fecha DATE;
	rdet RECORD;
	_cursor CURSOR FOR 
			SELECT secPedd, t.codprd, t.cntprd, t.usaiva, t.prcprd, t.ivaprd, t.ptoprd, t.peddatv
			FROM JSON_TO_RECORDSET(detalle::json) AS
				  t(codprd VARCHAR(4), cntprd INT, usaiva BOOLEAN, prcprd NUMERIC(10,6), 
					ivaprd NUMERIC(10,6), ptoprd NUMERIC(10,6), peddatv BOOLEAN);
	
BEGIN
	RAISE NOTICE 'accion a realizar %', accion;
	IF accion = 'N' THEN
		RAISE NOTICE 'iniciando transaccion de creado del pedido';
		BEGIN	
			RAISE NOTICE 'obteniendo secuencial';
			
			SELECT desencryptar(confval)::INT INTO secIni FROM tbConfParam WHERE confNid=24;
			
			SELECT LPAD(COALESCE(MAX(peddScl::int)+1,1)::TEXT,10,'0') INTO secPedd FROM tbPeddDPedd;
			
			IF secPedd::INT < secIni THEN
				secPedd := LPAD(secIni::TEXT,10,'0');
			END IF;
			
			RAISE NOTICE 'secuencial %', secPedd;
			RAISE NOTICE 'data = %', data;
			
			fecha := CURRENT_DATE;
			RAISE NOTICE 'despues de obtener la fecha %', fecha;
			
			RAISE NOTICE 'antes de obtener los datos y pasarlos a las variables';
			
			SELECT t.usuacod, t.tippedd, t.codcrri, t.msapedd, t.obvpedd, t.clienid, t.clienbr, t.cliedir, t.clietlf, t.clieref, t.crriCod, t.crriObv
			        INTO usuario, tipo, carrier, mesa, observ, clienid, clienbr, cliedir, clietlf, clieref, crrcod, crrobv
			FROM json_to_record(data::json) AS 
				  t(usuacod VARCHAR(4), tippedd VARCHAR(1), codcrri VARCHAR(4),
					msapedd VARCHAR(3), obvpedd TEXT, clienid VARCHAR(20),
				    clienbr VARCHAR(50), cliedir VARCHAR(100), clietlf VARCHAR(50),
				    clieref VARCHAR(100), crricod VARCHAR(10), crriobv VARCHAR(100));					
					
			RAISE NOTICE 'usu = % tip = % msa = % obv = % ', usuario, tipo, mesa, observ;
			RAISE NOTICE 'antes de obtener la letra';
			RAISE NOTICE 'crrCod = %, crrObv = %', crrcod, crrobv;
			
			SELECT CHR(MAX(ASCII(p.peddlta::CHAR))+1) INTO letra
			FROM   tbPeddDPedd p 
			WHERE  p.peddfec = fecha AND
					p.peddmsa = mesa AND
					p.peddatv = true AND p.peddeli = false;
			
			RAISE NOTICE 'despues de obtener letra';
			RAISE NOTICE 'letra = %',letra;
			IF letra IS NULL THEN 
				letra := 'A';
			END IF;
			
			RAISE NOTICE 'letra despues del if = %',letra;
			
			INSERT INTO tbPeddDPedd 
				(peddScl, peddLta, usuaCod, peddFec, peddTip, peddCcr, peddMsa, peddObv, peddHin, peddHFn, peddAtv, peddEli)
			VALUES(secPedd, letra, usuario, fecha, tipo, carrier, mesa, observ, CURRENT_TIME, null, true, false);					

			RAISE NOTICE 'paso el ingreso de datos';
			RAISE NOTICE 'detalle = %', detalle;
			
			IF tipo = 'D' THEN
				INSERT INTO tbPeddDatCli 
				VALUES (secPedd, clienid, clienbr, cliedir, clietlf, clieref);
			END IF;
			
			IF tipo = 'E' THEN
				INSERT INTO tbPeddDatCarr 
				VALUES(secPedd, carrier, crrCod, crrObv);
			END IF;
			
			INSERT INTO tbPeddMPedd
				SELECT secPedd, t.codprd, t.cntprd, t.usaiva, t.prcprd, t.ivaprd, t.ptoprd, true
				FROM JSON_TO_RECORDSET(detalle::json) AS
					  t(codprd VARCHAR(4), cntprd INT, usaiva BOOLEAN, prcprd NUMERIC(10,6), 
						ivaprd NUMERIC(10,6), ptoprd NUMERIC(10,6));

			RAISE NOTICE 'paso el ingreso del detalle';
			--COMMIT;			

			RAISE NOTICE 'finalizo correctamente y retornará mensaje';
			RETURN QUERY
				SELECT 1 AS codigo, 'Pedido creado correctamente con el secuencial '|| secPedd::VARCHAR AS descripcion;			
		EXCEPTION WHEN OTHERS THEN
			RAISE NOTICE 'HUBO un error y realizará un rollback';
			RAISE NOTICE 'Codio de error = %', SQLSTATE;
			--ROLLBACK;
			RETURN QUERY
			SELECT 0 AS codigo, 'Pedido no pudo ser creado' AS descripcion;
		END;
	END IF;
	IF accion = 'M' THEN
		BEGIN
			RAISE NOTICE 'INICIANDO';
			SELECT t.sclpedd, t.usuacod, t.ltapedd, t.tippedd, t.codcrri, t.msapedd, t.obvpedd 
				INTO secpedd, usuario, letra, tipo, carrier, mesa, observ
			FROM json_to_record(data::json) AS 
				  t(sclpedd VARCHAR(10), usuacod VARCHAR(4), ltapedd VARCHAR(1), 
					tippedd VARCHAR(1), codcrri VARCHAR(4), msapedd VARCHAR(3), obvpedd TEXT);

			RAISE NOTICE 'ANTES DEL UPDATE';
			RAISE NOTICE 'SECUENCIAL %', secpedd;
			RAISE NOTICE 'TIPO %', tipo;
			RAISE NOTICE 'MESA %', mesa;
			RAISE NOTICE 'LETRA %', letra;
			RAISE NOTICE 'OBSERVACION %', observ;
			
			UPDATE tbPeddDPedd 
			SET		peddlta = letra,  peddtip = tipo,
					peddmsa = mesa,   peddobv = observ,
					peddCcr = carrier
			WHERE  peddscl = secpedd;			
					
			RAISE NOTICE 'ANTES DE FOR';
			FOR rdet IN _cursor LOOP
				RAISE NOTICE 'secuencial del loop %',rdet.secpedd;
				IF EXISTS(SELECT * FROM tbPeddMPedd WHERE peddscl = rdet.secpedd AND itemcod = rdet.codprd) THEN
					RAISE NOTICE 'CUANDO SI EXISTE UN ITEM';
					RAISE NOTICE 'CANTIDAD %', rdet.cntprd;
					RAISE NOTICE 'PRODUCTO %', rdet.codprd;
					RAISE NOTICE 'USAIVA %', rdet.usaiva;
					RAISE NOTICE 'PRECIO %', rdet.prcprd;
					RAISE NOTICE 'IVA %', rdet.ivaprd;
					RAISE NOTICE 'ESTADO %', rdet.peddatv;
					
					UPDATE  tbPeddMPedd
					SET		itemcnt = rdet.cntprd, itemiva = rdet.usaiva, 
							itemprc = rdet.prcprd, itempiv = rdet.ivaprd, 
							itempto = rdet.ptoprd, peddatv = rdet.peddatv
					WHERE	peddscl = rdet.secpedd AND itemcod = rdet.codprd;
				ELSE
					RAISE NOTICE 'CUANDO NO EXISTE UN ITEM';
					
					INSERT INTO tbPeddMPedd
					VALUES (rdet.secpedd, rdet.codprd, rdet.cntprd, rdet.usaiva, rdet.prcprd, 
							 rdet.ivaprd, rdet.ptoprd, rdet.peddatv);
				END IF;				 
			END LOOP;
			
			-- ELIMINANDO ITEMS QUE FUERON QUITADOS DEL PEDIDO
			RAISE NOTICE 'CREANDO TABLA TEMPORAL TMPCODSDEL';
			
			DROP TABLE IF EXISTS tmpCodsDel;
			CREATE TEMPORARY TABLE tmpCodsDel(
				itemCod VARCHAR(4)
			);
			
			RAISE NOTICE 'ANTES DE INSERTAR DATOS EN TMPCODSDEL';
			INSERT INTO tmpCodsDel
			SELECT itemCod FROM	tbPeddMPedd WHERE peddScl = secpedd AND 
					  itemCod NOT IN (SELECT t.codprd FROM JSON_TO_RECORDSET(detalle::json) AS t (codprd VARCHAR(4)));
			RAISE NOTICE 'LUEGO DE INSERTAR DATOS EN TMPCODSDEL';
			
			IF EXISTS(SELECT * FROM tmpCodsDel) THEN
				RAISE NOTICE 'ELIMINANDO ITEMS DEL PEDIDO';
				DELETE FROM tbPeddMPedd
				WHERE
					peddScl = secpedd AND
					itemCod IN (
						SELECT itemCod FROM tmpCodsDel
					);
				RAISE NOTICE 'ITEMS ELIMINADOS DEL PEDIDO';
			END IF;
			
			DROP TABLE tmpCodsDel;
			
			RETURN QUERY
				SELECT 1 AS codigo, 'Pedido modificado correctamente con el secuencial '|| secPedd::VARCHAR AS descripcion;	
		EXCEPTION WHEN OTHERS THEN
			RAISE NOTICE 'HUBO un error y realizará un rollback';
			RAISE NOTICE 'Codio de error = %', SQLSTATE;
			--ROLLBACK;
			
			SELECT 0 AS codigo, 'Pedido con el secuencial '|| secpedd ||' no pudo ser modificado' AS descripcion;
		END;
	END IF;    
	IF accion = 'F' THEN
		BEGIN
			secpedd := data;
			
			RAISE NOTICE 'VA A FINALIZAR UN PEDIDO';			
			RAISE NOTICE 'OBTENIENDO EL SECUENCIAL';
			
			UPDATE tbPeddDPedd
			SET 	peddAtv = false, peddhfn = CURRENT_TIME
			WHERE	peddscl = secpedd;
			RAISE NOTICE 'LUEGO DE FINALIZAR EL PEDIDO';
			
			RETURN QUERY
				SELECT 1 AS codigo, 'Pedido con el secuencial '|| secpedd || ' ha sido finalizado correctamente' AS descripcion;	
		EXCEPTION WHEN OTHERS THEN
			RAISE NOTICE 'HUBO un error y realizará un rollback';
			RAISE NOTICE 'Codio de error = %', SQLSTATE;
			
			SELECT 0 AS codigo, 'Pedido con el secuencial '|| secpedd ||' no pudo ser finalizado' AS descripcion;
		END;
	END IF;
	
	IF accion = 'U' THEN
		BEGIN
			secpedd := data;
			
			RAISE NOTICE 'VA A DESHACER FINALIZAR UN PEDIDO';
			RAISE NOTICE 'OBTENIENDO EL SECUENCIAL';
			
			UPDATE tbPeddDPedd
			SET 	peddAtv = true, peddhfn = NULL
			WHERE	peddscl = secpedd;
			RAISE NOTICE 'LUEGO DE DESHACER FINALIZAR EL PEDIDO';
			
			RETURN QUERY
				SELECT 1 AS codigo, 'Pedido con el secuencial '|| secpedd || ' ha sido revertido correctamente' AS descripcion;	
		EXCEPTION WHEN OTHERS THEN
			RAISE NOTICE 'HUBO un error y realizará un rollback';
			RAISE NOTICE 'Codio de error = %', SQLSTATE;
			
			SELECT 0 AS codigo, 'Pedido con el secuencial '|| secpedd ||' no pudo ser revertido' AS descripcion;
		END;
	END IF;
END;
$BODY$;

CREATE OR REPLACE FUNCTION public.fnrptfactanuelife(
	anio integer,
	mes integer,
	dia integer)
    RETURNS TABLE(peddscl character varying, ventscl character varying, autosri character varying, cajanbr character varying, usuanbr character varying, ventfch character varying, ventnfc character varying, venttot numeric, ventobv character varying, ventstd character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE

BEGIN
	/*
	SELECT * FROM fnrptfactanueliFE(2022,12,18);
	*/
	RETURN QUERY
	SELECT 	DISTINCT 
			v.peddScl,
			v.ventscl,
			f.autosri,
			c.cajaNbr,
			u.usuaNbr,
			TO_CHAR(h.ventfch,'YYYY-MM-DD')::VARCHAR(10) AS ventfch,
			h.ventnfc,
			ROUND(v.venttot,2) AS venttot,
			h.ventobv,
			'ANULADO'::character varying AS ventstd
	FROM	tbPeddDVent v
			JOIN
			tbPeddHVent h ON h.ventscl = v.ventscl
			JOIN
			tbCajaCaja c ON c.cajacod = v.cajacod
			JOIN
			tbAdsiusua u ON u.usuacod = h.usuacod AND u.usuaatv = true
			JOIN
			tbFactelect f ON f.ventscl = v.ventscl
	WHERE 	f.etdofac = 'L' AND 
			EXTRACT( YEAR FROM v.ventfch) = anio AND 
			EXTRACT(MONTH FROM v.ventfch) = mes AND
			EXTRACT(DAY FROM v.ventfch) = dia;	
END;
$BODY$;

CREATE OR REPLACE FUNCTION public.fnrptpedidos(
	desde character varying,
	hasta character varying)
    RETURNS TABLE(fecha character varying, tipo character varying, lcnt integer, lpago numeric, dcnt integer, dpago numeric, ccnt integer, cpago numeric, vcnt integer, vpago numeric, rcnt integer, rpago numeric, ctotal integer, ptotal numeric) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
	_inicio 	DATE;
	_fin		DATE;
BEGIN
	_inicio := desde::DATE;
	_fin 	:= hasta::DATE;
	
	create temporary table tmp_pedidos(
		fecha 	date,
		tipo 	varchar(1),
		lcnt 	int,
		lpago	NUMERIC(10,2),
		dcnt	int,
		dpago	NUMERIC(10,2),
		ccnt	int,
		cpago	NUMERIC(10,2),
		vcnt	int,
		vpago	NUMERIC(10,2),
		rcnt	int,
		rpago	NUMERIC(10,2)
	);

	-- TIPO V PARA LOS PEDIDOS VENDIDOS (FACTURADOS Y PAGADOS)
	INSERT INTO tmp_pedidos
	SELECT 	fecha.fecha::date, 'V' as tipo, 
			0 as lcnt, 0 as lpago,
			0 as dcnt, 0 as dpago,
			0 as ccnt, 0 as cpago,
			0 as vcnt, 0 as vpago,
			0 as rcnt, 0 as rpago
	FROM generate_series ( desde::timestamp, hasta::timestamp, '1 day'::interval) as fecha;

	-- TIPO E PARA LOS PEDIDOS ELIMINADOS O FACTURAS ELIMINADAS
	INSERT INTO tmp_pedidos 
	SELECT 	fecha.fecha::date, 'E' as tipo,
			0 as lcnt, 0 as lpago,
			0 as dcnt, 0 as dpago,
			0 as ccnt, 0 as cpago,
			0 as vcnt, 0 as vpago,
			0 as rcnt, 0 as rpago
	FROM generate_series( desde::timestamp, hasta::timestamp, '1 day'::interval) as fecha;

	-- TIPO C PARA LOS PEDIDOS NO VENDIDOS, PERO QUE NO FUERON ELIMINADOS CON ESTADO A (ANULADO) O F(FACTURADO)
	INSERT INTO tmp_pedidos 
	SELECT 	fecha.fecha::date, 'C' AS tipo,
			0 AS lcnt, 0 AS lpago,
			0 AS dcnt, 0 as dpago,
			0 AS ccnt, 0 AS cpago,
			0 as vcnt, 0 as vpago,
			0 as rcnt, 0 as rpago
	FROM generate_series( desde::timestamp, hasta::timestamp, '1 day'::interval) AS fecha;

	---------------- FACTURADOS Y PAGADOS ----------------------------------
	UPDATE tmp_pedidos 
	SET
		lcnt = a.cantidad, 
		lpago = a.pago
	FROM (	
		SELECT 	v.ventfch, SUM(v.venttot) AS pago, COUNT(*) AS cantidad
		FROM 	tbPeddDVent v 
		WHERE	v.venttip = 'L' 
				AND v.ventstd = 'P'
				AND v.ventfch >= _inicio AND v.ventfch <= _fin
		GROUP BY v.ventfch
	) a
	WHERE 	tmp_pedidos.fecha=a.ventfch AND
			tmp_pedidos.tipo='V';

	UPDATE tmp_pedidos 
	SET
		dcnt = a.cantidad, 
		dpago = a.pago
	FROM (	
		SELECT 	v.ventfch, SUM(v.venttot) AS pago, COUNT(*) AS cantidad
		FROM 	tbPeddDVent v 
		WHERE	v.venttip = 'D' 
				AND v.ventstd = 'P'
				AND v.ventfch >= _inicio AND v.ventfch <= _fin
		GROUP BY v.ventfch
	) a
	WHERE 	tmp_pedidos.fecha=a.ventfch AND
			tmp_pedidos.tipo='V';		

	UPDATE tmp_pedidos 
	SET
		ccnt = a.cantidad, 
		cpago = a.pago
	FROM (	
		SELECT 	v.ventfch, SUM(v.venttot) AS pago, COUNT(*) AS cantidad
		FROM 	tbPeddDVent v 
		WHERE	v.venttip = 'C' 
				AND v.ventstd = 'P'
				AND v.ventfch >= _inicio AND v.ventfch <= _fin
		GROUP BY v.ventfch
	) a
	WHERE 	tmp_pedidos.fecha=a.ventfch AND
			tmp_pedidos.tipo='V';		

	UPDATE tmp_pedidos 
	SET
		vcnt = a.cantidad, 
		vpago = a.pago
	FROM (	
		SELECT 	v.ventfch, SUM(v.venttot) AS pago, COUNT(*) AS cantidad
		FROM 	tbPeddDVent v 
		WHERE	v.venttip = 'R' 
				AND v.ventstd = 'P'
				AND v.ventfch >= _inicio AND v.ventfch <= _fin
		GROUP BY v.ventfch
	) a
	WHERE 	tmp_pedidos.fecha=a.ventfch AND
			tmp_pedidos.tipo='V';
			
	UPDATE tmp_pedidos 
	SET
		rcnt = a.cantidad, 
		rpago = a.pago
	FROM (	
		SELECT 	v.ventfch, SUM(v.venttot) AS pago, COUNT(*) AS cantidad
		FROM 	tbPeddDVent v 
		WHERE	v.venttip = 'E' 
				AND v.ventstd = 'P'
				AND v.ventfch >= _inicio AND v.ventfch <= _fin
		GROUP BY v.ventfch
	) a
	WHERE 	tmp_pedidos.fecha=a.ventfch AND
			tmp_pedidos.tipo='V';
			
	-------------------------- PEDIDOS O FACTURAS ELIMINADAS -------------------------------
	UPDATE tmp_pedidos 
	SET
		lcnt = a.cantidad, 
		lpago = a.pago
	FROM (	
		SELECT b.fecha, b.pago, SUM(b.cantidad) AS cantidad
		FROM (
			SELECT 	v.ventfch AS fecha, SUM(v.venttot) AS pago, COUNT(*) AS cantidad
			FROM 	tbPeddDVent v 
			WHERE	v.venttip = 'L' 
					AND v.ventstd = 'E'
					AND v.ventfch >= _inicio AND v.ventfch <= _fin
			GROUP BY v.ventfch
			UNION 
			SELECT 	v.peddfec AS fecha, SUM(d.itempto) AS pago, COUNT(*) AS cantidad
			FROM 	tbPeddDPedd v 
					JOIN tbPeddMPedd d ON v.peddscl = d.peddscl
			WHERE	v.peddtip = 'L' 
					AND v.peddeli = true
					AND v.peddfec >= _inicio AND v.peddfec <= _fin
			GROUP BY v.peddfec
		) b GROUP BY b.fecha, b.pago
	) a
	WHERE 	tmp_pedidos.fecha=a.fecha AND
			tmp_pedidos.tipo='E';
	
	UPDATE tmp_pedidos 
	SET
		dcnt = a.cantidad, 
		dpago = a.pago
	FROM (	
		SELECT b.fecha, b.pago, SUM(b.cantidad) AS cantidad
		FROM (
			SELECT 	v.ventfch AS fecha, SUM(v.venttot) AS pago, COUNT(*) AS cantidad
			FROM 	tbPeddDVent v 
			WHERE	v.venttip = 'D' 
					AND v.ventstd = 'E'
					AND v.ventfch >= _inicio AND v.ventfch <= _fin
			GROUP BY v.ventfch
			UNION 
			SELECT 	v.peddfec AS fecha, SUM(d.itempto) AS pago, COUNT(*) AS cantidad
			FROM 	tbPeddDPedd v 
					JOIN tbPeddMPedd d ON v.peddscl = d.peddscl
			WHERE	v.peddtip = 'D' 
					AND v.peddeli = true
					AND v.peddfec >= _inicio AND v.peddfec <= _fin
			GROUP BY v.peddfec
		) b GROUP BY b.fecha, b.pago
	) a
	WHERE 	tmp_pedidos.fecha=a.fecha AND
			tmp_pedidos.tipo='E';
	
	UPDATE tmp_pedidos 
	SET
		ccnt = a.cantidad, 
		cpago = a.pago
	FROM (	
		SELECT b.fecha, b.pago, SUM(b.cantidad) AS cantidad
		FROM (
			SELECT 	v.ventfch AS fecha, SUM(v.venttot) AS pago, COUNT(*) AS cantidad
			FROM 	tbPeddDVent v 
			WHERE	v.venttip = 'C' 
					AND v.ventstd = 'E'
					AND v.ventfch >= _inicio AND v.ventfch <= _fin
			GROUP BY v.ventfch
			UNION 
			SELECT 	v.peddfec AS fecha, SUM(d.itempto) AS pago, COUNT(*) AS cantidad
			FROM 	tbPeddDPedd v 
					JOIN tbPeddMPedd d ON v.peddscl = d.peddscl
			WHERE	v.peddtip = 'C' 
					AND v.peddeli = true
					AND v.peddfec >= _inicio AND v.peddfec <= _fin
			GROUP BY v.peddfec
		) b GROUP BY b.fecha, b.pago
	) a
	WHERE 	tmp_pedidos.fecha=a.fecha AND
			tmp_pedidos.tipo='E';
			
	UPDATE tmp_pedidos 
	SET
		rcnt = a.cantidad, 
		rpago = a.pago
	FROM (	
		SELECT b.fecha, b.pago, SUM(b.cantidad) AS cantidad
		FROM (
			SELECT 	v.ventfch AS fecha, SUM(v.venttot) AS pago, COUNT(*) AS cantidad
			FROM 	tbPeddDVent v 
			WHERE	v.venttip = 'R' 
					AND v.ventstd = 'E'
					AND v.ventfch >= _inicio AND v.ventfch <= _fin
			GROUP BY v.ventfch
			UNION 
			SELECT 	v.peddfec AS fecha, SUM(d.itempto) AS pago, COUNT(*) AS cantidad
			FROM 	tbPeddDPedd v 
					JOIN tbPeddMPedd d ON v.peddscl = d.peddscl
			WHERE	v.peddtip = 'R' 
					AND v.peddeli = true
					AND v.peddfec >= _inicio AND v.peddfec <= _fin
			GROUP BY v.peddfec
		) b GROUP BY b.fecha, b.pago
	) a
	WHERE 	tmp_pedidos.fecha=a.fecha AND
			tmp_pedidos.tipo='E';
	
	UPDATE tmp_pedidos 
	SET
		rcnt = a.cantidad, 
		rpago = a.pago
	FROM (	
		SELECT b.fecha, b.pago, SUM(b.cantidad) AS cantidad
		FROM (
			SELECT 	v.ventfch AS fecha, SUM(v.venttot) AS pago, COUNT(*) AS cantidad
			FROM 	tbPeddDVent v 
			WHERE	v.venttip = 'E' 
					AND v.ventstd = 'E'
					AND v.ventfch >= _inicio AND v.ventfch <= _fin
			GROUP BY v.ventfch
			UNION 
			SELECT 	v.peddfec AS fecha, SUM(d.itempto) AS pago, COUNT(*) AS cantidad
			FROM 	tbPeddDPedd v 
					JOIN tbPeddMPedd d ON v.peddscl = d.peddscl
			WHERE	v.peddtip = 'E' 
					AND v.peddeli = true
					AND v.peddfec >= _inicio AND v.peddfec <= _fin
			GROUP BY v.peddfec
		) b GROUP BY b.fecha, b.pago
	) a
	WHERE 	tmp_pedidos.fecha=a.fecha AND
			tmp_pedidos.tipo='E';
		
	--------------------- PEDIDOS O FACTURAS NO PAGADOS O ANULADOS
	UPDATE tmp_pedidos 
	SET
		lcnt = a.cantidad, 
		lpago = a.pago
	FROM (	
		SELECT 	v.ventfch, SUM(v.venttot) AS pago, COUNT(*) AS cantidad
		FROM 	tbPeddDVent v 
		WHERE	v.venttip = 'L' 
				AND v.ventstd IN ('F', 'A')
				AND v.ventfch >= _inicio AND v.ventfch <= _fin
		GROUP BY v.ventfch	
	) a
	WHERE 	tmp_pedidos.fecha=a.ventfch AND
			tmp_pedidos.tipo='C';
		
	UPDATE tmp_pedidos 
	SET
		dcnt = a.cantidad, 
		dpago = a.pago
	FROM (	
		SELECT 	v.ventfch, SUM(v.venttot) AS pago, COUNT(*) AS cantidad
		FROM 	tbPeddDVent v 
		WHERE	v.venttip = 'D' 
				AND v.ventstd IN ('F', 'A')
				AND v.ventfch >= _inicio AND v.ventfch <= _fin
		GROUP BY v.ventfch	
	) a
	WHERE 	tmp_pedidos.fecha=a.ventfch AND
			tmp_pedidos.tipo='C';
		
	UPDATE tmp_pedidos 
	SET
		ccnt = a.cantidad, 
		cpago = a.pago
	FROM (	
		SELECT 	v.ventfch, SUM(v.venttot) AS pago, COUNT(*) AS cantidad
		FROM 	tbPeddDVent v 
		WHERE	v.venttip = 'C' 
				AND v.ventstd IN ('F', 'A')
				AND v.ventfch >= _inicio AND v.ventfch <= _fin
		GROUP BY v.ventfch	
	) a
	WHERE 	tmp_pedidos.fecha=a.ventfch AND
			tmp_pedidos.tipo='C';
		
	UPDATE tmp_pedidos 
	SET
		vcnt = a.cantidad, 
		vpago = a.pago
	FROM (	
		SELECT 	v.ventfch, SUM(v.venttot) AS pago, COUNT(*) AS cantidad
		FROM 	tbPeddDVent v 
		WHERE	v.venttip = 'R' 
				AND v.ventstd IN ('F', 'A')
				AND v.ventfch >= _inicio AND v.ventfch <= _fin
		GROUP BY v.ventfch	
	) a
	WHERE 	tmp_pedidos.fecha=a.ventfch AND
			tmp_pedidos.tipo='C';
		
	UPDATE tmp_pedidos 
	SET
		rcnt = a.cantidad, 
		rpago = a.pago
	FROM (	
		SELECT 	v.ventfch, SUM(v.venttot) AS pago, COUNT(*) AS cantidad
		FROM 	tbPeddDVent v 
		WHERE	v.venttip = 'E' 
				AND v.ventstd IN ('F', 'A')
				AND v.ventfch >= _inicio AND v.ventfch <= _fin
		GROUP BY v.ventfch	
	) a
	WHERE 	tmp_pedidos.fecha=a.ventfch AND
			tmp_pedidos.tipo='C';	
	--------------------------------------------------------------------------------
	RETURN QUERY
		SELECT 	to_char(p.fecha,'yyyy-mm-dd')::varchar(10) as fecha, 
				case p.tipo
					when 'V' then 'FACTURADOS'
					when 'C' then 'NO FACTURADOS'
					when 'E' then 'ELIMINADOS'
				end :: varchar(15) as tipo, 
				p.lcnt,	p.lpago, 
				p.dcnt, p.dpago,
				p.ccnt, p.cpago,
				p.vcnt, p.vpago,
				p.rcnt, p.rpago,
				p.lcnt+p.dcnt+p.ccnt+p.vcnt+p.rcnt AS ctotal, 
				p.lpago+p.dpago+p.cpago+p.vpago+p.rpago AS ptotal 
		FROM 	tmp_pedidos p
		order by p.tipo desc, p.fecha asc;

	drop table tmp_pedidos;
	RETURN;
END;
$BODY$;

CREATE OR REPLACE FUNCTION public.fntoelectfact(
	fecha character varying)
    RETURNS TABLE(ventscl character varying, cajacod character varying, usuacod character varying, ventfch date, nidnid character varying, venttip character varying, ventmsa character varying, ventlta character varying, peddscl character varying, venthra time without time zone, ventsbt numeric, ventdsc numeric, ventiva numeric, venttot numeric, ventrcb numeric, ventvto numeric, ventobv character varying, ventatv boolean, ventfac boolean, ventdpc integer, ventdpg boolean, ventcrr character varying, ventcpt numeric, ventcvl numeric, ventstd character varying, ventsep character varying, nidnbr character varying, nidtip character varying, niddir character varying, nideml character varying, nidtlf character varying, etdofac character varying, etdoeml character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
	
BEGIN
	RETURN QUERY
	SELECT 	v.*,
		c.nidNbr,
		c.nidtip,
		c.nidDir,
		c.nidEml,
		c.nidTlf,
		f.etdofac,
		f.etdoeml
FROM 	tbPeddDVent v
		LEFT JOIN 
		tbFactElect f ON v.ventscl = f.ventscl
		JOIN
		tbAdsiNid c ON v.nidnid = c.nidnid
WHERE	v.ventfch = fecha::DATE
		AND v.ventstd IN ('F', 'P')
		AND (f.etdofac IS NULL OR f.etdofac IN ('N', 'R'));
END;
$BODY$;

DROP FUNCTION fntoelectfact(character varying,character varying);

CREATE OR REPLACE FUNCTION public.fntoelectfact(
	fecha character varying,
	filtro character varying)
    RETURNS TABLE(ventscl character varying, cajacod character varying, usuacod character varying, tipo character varying, ventfch date, nidnid character varying, ventmsa character varying, ventlta character varying, peddscl character varying, venthra time without time zone, docuser character varying, docusri character varying, autosri character varying, ventsbt numeric, ventstn numeric, ventdsc numeric, ventiva numeric, venttot numeric, ventrcb numeric, ventvto numeric, nidnbr character varying, nidtip character varying, niddir character varying, nideml character varying, nidtlf character varying, etdofac character varying, etdoeml character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
	CodEmp VARCHAR(3);
	CodFac VARCHAR(3);
BEGIN
	/*
	SELECT * FROM fntoelectfact('2022-12-16','T');
	
	SELECT * FROM fntoelectfact('2022-12-18','T');
	
	SELECT * FROM fntoelectfact('2022-12-29','T');
	*/
	SELECT desencryptar(emprecod) INTO CodEmp FROM tbAdsiEmpr;
	SELECT desencryptar(confval) INTO CodFac FROM tbConfParam WHERE confnid=67;
	
	RETURN QUERY
	SELECT * FROM (
		SELECT 	v.ventscl,
				v.cajacod,
				v.usuacod,
				'F'::character varying AS tipo,
				v.ventfch,
				v.nidnid,
				v.ventmsa,
				v.ventlta,
				v.peddscl,
				v.venthra,
				COALESCE(f.secuencial,'000000000') AS docuser,
				(CodEmp || '-' || CodFac || '-' ||COALESCE(f.secuencial,'000000000'))::character varying AS docusri,
				f.autosri,
				v.ventsbt,
				COALESCE(v.ventstn,0) ventstn,
				v.ventdsc,
				v.ventiva,
				v.venttot::numeric as venttot,
				v.ventrcb,
				v.ventvto,
				c.nidNbr,
				c.nidtip,
				c.nidDir,
				c.nidEml,
				c.nidTlf,
				COALESCE(f.etdofac,'N') AS etdofac,
				COALESCE(f.etdoeml,'N') AS etdoeml
		FROM 	tbPeddDVent v
				LEFT JOIN 
				tbFactElect f ON v.ventscl = f.ventscl
				JOIN
				tbAdsiNid c ON v.nidnid = c.nidnid
		WHERE	v.ventfch = fecha::DATE
				AND v.ventstd = 'P'
				AND (f.etdofac IS NULL OR f.etdofac IN ('N', 'D', 'R', 'L'))
				AND (filtro = 'T' OR (v.venttip = filtro OR 
								  (CASE WHEN f.etdoeml IN ('N', 'D', 'I') THEN 'N' ELSE f.etdoeml END = filtro)))
		UNION
		SELECT 	r.retescl AS ventscl,
				r.cajacod,
				r.usuacod,
				r.retetip::character varying AS tipo,
				r.retefch AS ventfch,
				r.nidnid,
				'' AS ventmsa,
				'' AS ventlta,
				'' AS peddscl,
				NULL AS venthra,
				r.docuser,
				r.docusri:: character varying as docusri,
				f.autosri,
				NULL AS ventsbt,
				NULL AS ventsbn,
				NULL AS ventdsc,
				NULL AS ventiva,
				r.retetot::numeric AS venttot,
				NULL AS ventrcb,
				NULL AS ventvto
				,CASE
					WHEN r.retetip = 'C' THEN p.provnbr
					ELSE c.nidnbr
				END AS nidNbr
				,CASE 
					WHEN r.retetip = 'C' THEN 'R'
					ELSE c.nidtip
				END AS nidtip
				,CASE 
					WHEN r.retetip = 'C' THEN p.provdir
					ELSE c.niddir
				END AS niddir
				,CASE 
					WHEN r.retetip = 'C' THEN p.proveml
					ELSE c.nideml
				END AS nideml
				,CASE 
					WHEN r.retetip = 'C' THEN p.provtlf
					ELSE c.nidtlf
				END AS nidtlf
				,COALESCE(f.etdofac,'N') AS etdofac
				,COALESCE(f.etdoeml,'N') AS etdoeml
		FROM	tbPeddDRete r
				LEFT JOIN
				tbReteElect f ON f.retescl = r.retescl
				LEFT JOIN
				tbIvtrProv p ON p.provnid = r.nidnid
				LEFT JOIN
				tbAdsiNid c ON c.nidnid = r.nidnid
		WHERE	r.retefcr = fecha::DATE
					AND r.reteetd = 'A'
					AND (f.etdofac IS NULL OR f.etdofac IN ('N', 'R','L'))
					AND (filtro = 'T' OR (r.retetip = filtro OR 
									  (CASE WHEN f.etdoeml IN ('N','I') THEN 'N' ELSE f.etdoeml END = filtro)))
	) v ORDER BY v.ventscl ASC;
END;
$BODY$;

CREATE OR REPLACE FUNCTION public.fnusuarios(
	accion character varying,
	secuencial character varying,
	creador character varying,
	usuario character varying,
	clave character varying,
	detalle character varying,
	tipo character varying,
	empleado character varying,
	estado character varying,
	permisos character varying)
    RETURNS TABLE(codigo integer, descripcion text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
	_secuencial INT;	
	_usaClvNum BOOLEAN;
BEGIN
	_usaClvNum := FALSE;
	
	SELECT CASE desencryptar(confval) 
				WHEN '1' THEN TRUE 
				ELSE FALSE 
			END INTO _usaClvNum
	FROM 	tbConfParam 
	WHERE 	confNid=20;
	
	IF accion = 'N' THEN
		IF NOT EXISTS(SELECT usuaUsr FROM tbAdsiUsua WHERE usuaUsr=usuario) THEN
			BEGIN
				RAISE NOTICE 'CREANDO USUARIO';
				RAISE NOTICE 'OBTENIENDO SECUENCIAL';
				SELECT COALESCE(MAX(usuaCod::int)+1,1) INTO _secuencial FROM tbAdsiUsua;
				
				RAISE NOTICE 'SECUENCIAL = %', _secuencial;			
				
				IF _usaClvNum = FALSE THEN
					RAISE NOTICE 'INGRESANDO CON CLAVE NO NUMERICA';
					INSERT INTO tbAdsiUsua 
							(usuaCod, usuaNbr, usuaUsr, usuaClv, usuaTip, empdNid, usuaCad, usuaAtv)
							VALUES (LPAD(_secuencial::TEXT,4,'0'), 
									detalle, usuario, encryptar(clave), 
									tipo, empleado, encryptar(creador),
									CASE estado WHEN 'A' THEN TRUE ELSE FALSE END);
				ELSE
					RAISE NOTICE 'INGRESANDO CON CLAVE NUMERICA';
					INSERT INTO tbAdsiUsua 
							(usuaCod, usuaNbr, usuaUsr, usuaClv, usuaCnm, usuaTip, empdNid, usuaCad, usuaAtv)
							VALUES (LPAD(_secuencial::TEXT,4,'0'), 
									detalle, usuario, encryptar(''),encryptar(clave), 
									tipo, empleado, encryptar(creador),
									CASE estado WHEN 'A' THEN TRUE ELSE FALSE END);
				END IF;
								
				RAISE NOTICE 'INGRESANDO LOS PERMISOS DEL USUARIO';
				INSERT INTO tbAdsiUsPer
					SELECT LPAD(_secuencial::TEXT,4,'0'), t.codigo, t.estado
					FROM JSON_TO_RECORDSET(permisos::json) AS
							t(codigo VARCHAR(4), estado BOOLEAN);
								
				RAISE NOTICE 'INGRESO CREADO CORRECTAMENTE';

				RETURN QUERY
					SELECT 1 AS codigo, 'El usuario fue registrado correctamente' AS descripcion;
			EXCEPTION WHEN OTHERS THEN
				RAISE NOTICE 'Codigo de error = %', SQLSTATE;
				RETURN QUERY
					SELECT 0 AS codigo, 'El usuario no pudo ser registrado correctamente' AS descripcion;
			END;
		END IF;
	ELSE
		BEGIN
			RAISE  NOTICE 'Modificando un usuario';
			IF _usaClvNum = FALSE THEN
				RAISE NOTICE 'ACTUALIZANDO CON CLAVE NO NUMERICA';
				UPDATE tbAdsiUsua
					SET usuaClv=encryptar(clave),
						usuaNbr=detalle,
						usuaTip=tipo,
						empdNid=empleado,
						usuaAtv=CASE estado WHEN 'A' THEN TRUE ELSE FALSE END
				WHERE	usuaCod=secuencial;
			ELSE
				RAISE NOTICE 'ACTUALIZANDO CON CLAVE NUMERICA';
				UPDATE tbAdsiUsua
				SET usuaCnm=encryptar(clave),
					usuaNbr=detalle,
					usuaTip=tipo,
					empdNid=empleado,
					usuaAtv=CASE estado WHEN 'A' THEN TRUE ELSE FALSE END
			WHERE	usuaCod=secuencial;
			END IF;
			RAISE NOTICE 'VERIFICANDO SI EL USUARIO TIENE PERMISOS DEFINIDOS';
			IF NOT EXISTS(SELECT * FROM tbAdsiUsPer WHERE usuaCod=secuencial) THEN
				RAISE NOTICE 'CREANDO PERMISOS PORQUE USUARIO NO TENIA ASIGNADO';
				INSERT INTO tbAdsiUsPer
					SELECT LPAD(secuencial::TEXT,4,'0'), t.codigo, t.estado
					FROM JSON_TO_RECORDSET(permisos::json) AS
							t(codigo VARCHAR(4), estado BOOLEAN);
			ELSE
				RAISE NOTICE 'ACTUALIZANDO LOS PERMISOS DEL USUARIO';								
				UPDATE tbAdsiUsPer
				SET uperAtv = a.estado
				FROM(
					SELECT t.codigo, t.estado
					FROM 	JSON_TO_RECORDSET(permisos::json) AS
							t(codigo VARCHAR(4), estado BOOLEAN)
				) a
				WHERE tbAdsiUsPer.usuaCod = secuencial AND tbAdsiUsPer.pmsoCod=a.codigo;
			END IF;			
			
			RAISE NOTICE 'AGREGANDO PERMISOS AL USUARIO';
			INSERT INTO tbAdsiUsPer
				SELECT LPAD(secuencial::TEXT,4,'0'), t.codigo, t.estado
				FROM JSON_TO_RECORDSET(permisos::json) AS
						t(codigo VARCHAR(4), estado BOOLEAN)
				WHERE t.codigo NOT IN (SELECT a.pmsocod FROM tbAdsiUsPer a WHERE a.usuacod = secuencial);
			
			RAISE NOTICE 'Actualización realizada';
			RETURN QUERY
				SELECT 1 AS codigo, 'Los datos del usuario fueron modificados correctamente' AS descripcion;
		EXCEPTION WHEN OTHERS THEN
			RAISE NOTICE 'Codigo de error = %', SQLSTATE;
			RETURN QUERY
				SELECT 0 AS codigo, 'Los datos del usuario no pudieron ser modificados' AS descripcion;
		END;
	END IF;
END;
$BODY$;

CREATE OR REPLACE FUNCTION public.fnventas(
	accion character varying,
	data character varying,
	detalle character varying,
	pagos character varying)
    RETURNS TABLE(codigo integer, descripcion text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
	secVent 	VARCHAR(10);
	secPedd 	VARCHAR(10);
	mesa 		VARCHAR(3);
	caja 		VARCHAR(4);
	tipo 		VARCHAR(1);
	carrier 	VARCHAR(4);
	pctcarrier 	NUMERIC(10,2);
	valcarrier 	NUMERIC(10,2);
	letra 		VARCHAR(1) := NULL;
	usuario 	VARCHAR(4);
	secIni 		INT := 0;
	separado	VARCHAR(1);
	
	clienid 	VARCHAR(20);
	 
	subtotal 	NUMERIC(14,6);
	subtniva	NUMERIC(14,6);
	pdescuento 	INT;
	descuento 	NUMERIC(14,6);
	iva 		NUMERIC(14,6);
	total 		NUMERIC(14,6);
	recibido 	NUMERIC(14,6);
	vuelto 		NUMERIC(14,6);
	
	prcCarr 	DECIMAL(10,6);
	ptjCarr 	BOOLEAN;
	aplCarr 	VARCHAR(1);
	
	valIva  	DECIMAL(10,6);
	
	observ 		TEXT;
	estado 		BOOLEAN;
	facturado 	BOOLEAN;
	fecha 		DATE;	
	numfact		VARCHAR(20);
	rdet RECORD;
	_cursor CURSOR FOR 
			SELECT secPedd, t.itemcod, t.itemcnt, t.itemprc, t.itemiva, t.itempto
            FROM JSON_TO_RECORDSET(detale::json) AS
                  t(itemcod VARCHAR(4), itemcnt INT, itemprc NUMERIC(10,6), itemiva NUMERIC(10,6), itempto NUMERIC(10,6));
BEGIN
	RAISE NOTICE 'accion a realizar %', accion;
	pctcarrier := 0;
	valcarrier := 0;
	
	SELECT 	(desencryptar(confval)::DECIMAL/100)::DECIMAL(10,6) 
			INTO valIva 
	FROM 	tbConfParam 
	WHERE 	confNid=2;
	
	IF accion = 'N' THEN
		RAISE NOTICE 'iniciando transaccion de creado de VENTA';
		BEGIN	
			RAISE NOTICE 'obteniendo secuencial';
			
			SELECT desencryptar(confval)::INT INTO secIni FROM tbConfParam WHERE confNid=24;
			
			SELECT LPAD(COALESCE(MAX(ventScl::int)+1,1)::TEXT,10,'0') INTO secVent FROM tbPeddDVent;
			
			IF secVent::INT < secIni THEN
				secVent := LPAD(secIni::TEXT,10,'0');
			END IF;
			
			RAISE NOTICE 'secuencial %', secVent;
			RAISE NOTICE 'data = %', data;
			
			fecha := CURRENT_DATE;
			RAISE NOTICE 'despues de obtener la fecha %', fecha;
			
			RAISE NOTICE 'antes de obtener los datos y pasarlos a las variables';
			SELECT 	t.cajacod, t.empdnid, t.nidnid, t.venttip, t.ventccr, t.ventmsa, t.ventlta, t.peddscl, t.ventsbt,
		            t.ventdsc, t.ventdpc, t.ventiva, t.venttot, t.ventrcb, t.ventvto, t.ventobv, t.ventatv, t.ventfac,
					t.ventsep, t.ventsbn
					INTO 
					caja, usuario, clienid, tipo, carrier, mesa, letra, secPedd, subtotal, descuento, pdescuento, 
					iva, total, recibido, vuelto, observ, estado, facturado, separado, subtniva
            FROM json_to_record(data::json) AS 
				  t(cajacod VARCHAR(4), empdnid VARCHAR(4), nidnid VARCHAR(20), venttip VARCHAR(1), ventccr VARCHAR(4),
					ventmsa VARCHAR(3), ventlta VARCHAR(1), peddscl VARCHAR(10), ventsbt NUMERIC(14,6),
					ventdsc NUMERIC(14,6), ventdpc INT, ventiva NUMERIC(14,6), venttot NUMERIC(14,6), 
					ventrcb NUMERIC(14,6), ventvto NUMERIC(14,6), ventobv TEXT, ventatv BOOLEAN, ventfac BOOLEAN,
				    ventsep VARCHAR(1), ventsbn NUMERIC(14,6));												

			RAISE NOTICE 'PASO LA RECOLECCION DE DATOS';
			RAISE NOTICE 'letra despues del if = %',secPedd;
			RAISE NOTICE 'ANTES DEL INGRESO DE DATOS PRIMARIOS';
			
			IF carrier <> '0000' THEN
				RAISE NOTICE 'OBTENIENDO LOS DATOS DEL CARRIER';
				
				--SELECT 	crriprc::DECIMAL(10,6), crripct, crriapi INTO prcCarr, ptjCarr, aplCarr FROM tbAdsiCrrie WHERE crriNid = carrier;

				SELECT 	SUM(
							CASE
								WHEN i.itemiva = TRUE THEN ABS((i.itemprc/(1 + valIva))::decimal(10,6) - a.itemprc)*a.itemcnt
								ELSE (i.itemprc::DECIMAL(10,6) - a.itemprc)*itemcnt
							END
						)  
						INTO 
						valCarrier
				FROM(
					SELECT 	t.itemcod, t.itemcnt, t.itemprc, t.itemiva
					FROM 	JSON_TO_RECORDSET(detalle::json) AS
							t(itemcod VARCHAR(4), itemcnt INT, itemprc NUMERIC(10,6), itemiva NUMERIC(10,6))
					) a
				INNER JOIN
				tbIvtrItem i ON a.itemcod = i.itemcod;
				
				SELECT 	/*CASE crripct
							WHEN TRUE THEN valCarrier --total + total*crriprc/100
							ELSE subtotal - crriprc --total + crriprc
						END AS valor, */
						CASE crripct
							WHEN TRUE THEN crriprc
							ELSE 0
						END AS porcentaje
						INTO 
						pctcarrier
				FROM 	tbAdsiCrrie
				WHERE	crriNid = carrier;
				
				RAISE NOTICE 'SE OBTUVIERON LOS DATOS DEL CARRIER';
			END IF;
			
			INSERT INTO tbPeddDVent
				(ventscl, cajacod, usuacod, ventfch, nidnid, venttip, ventcrr, ventcpt, ventcvl, ventmsa, ventlta, peddscl,
				 venthra, ventsbt, ventdsc, ventdpc, ventiva, venttot, ventrcb, ventvto, ventobv, ventatv, ventfac, ventPgd,
				 ventstd, ventsep, ventstn)
			VALUES(secVent, caja, usuario, fecha, clienid, tipo, carrier, pctcarrier, valcarrier, mesa, letra, secPedd, 
				   CURRENT_TIME, subtotal, descuento, pdescuento, iva, total, recibido, vuelto, '', true, true, 
				   CASE WHEN recibido > 0 THEN true ELSE false END,
				   CASE WHEN recibido > 0 THEN 'P' ELSE 'F' END,
				   separado, subtniva);

			RAISE NOTICE 'paso el INGRESO DE DATOS PRIMARIOS';
			RAISE NOTICE 'detalle = %', detalle;
			
			RAISE NOTICE 'antes del ingreso DEL DETALLE';
			
			INSERT INTO tbPeddMVent
				SELECT secVent, t.itemcod, t.itemcnt, t.itemprc, t.itemiva, t.itempto
				FROM JSON_TO_RECORDSET(detalle::json) AS
						t(itemcod VARCHAR(4), itemcnt INT, itemprc NUMERIC(10,6), 
						itemiva NUMERIC(10,6), itempto NUMERIC(10,6));
					
			RAISE NOTICE 'PASO EL INGRESO DEL DETALLE';
			RAISE NOTICE 'ANTES DEL INGRESO DE PAGOS';
			RAISE NOTICE 'secuencial %', secVent;
			
			INSERT INTO tbPeddDFpgo
			SELECT secVent, 
					CASE t.isefectivo 
						WHEN TRUE THEN 'E' 
						ELSE 
							CASE t.ischeque 
								WHEN TRUE THEN 'C'
								ELSE 'T' 
							END
					END AS fpgotip, t.valor, 
					t.vuelto, t.tarjeta, 
			        CASE WHEN t.iscorriente = TRUE THEN 'C'
				         WHEN t.isdiferido = TRUE THEN 'D'
					 	 ELSE 'N'
				    END AS fpgotpt, t.meses,
					t.banco, t.numcheque
            FROM JSON_TO_RECORDSET(pagos::JSON) AS
					t(isefectivo BOOLEAN, valor DECIMAL(14,6), vuelto DECIMAL(14,6), 
					  iscorriente BOOLEAN, isdiferido BOOLEAN, meses INT, tarjeta VARCHAR(1),
					  ischeque BOOLEAN,banco VARCHAR(2), numcheque VARCHAR(6));
					  
			IF tipo = 'E' THEN
				UPDATE tbPeddDVent SET ventPgd = TRUE WHERE ventScl = secVent;
				RAISE NOTICE 'Actualizado venta con secuencial % a pagado en caso de ser tipo CARRIER', secVent;
			ELSE
				IF ((SELECT COUNT(*) FROM tbPeddDFpgo WHERE ventScl = secVent) > 0) THEN
					UPDATE tbPeddDVent SET ventPgd = TRUE WHERE ventScl = secVent;
					RAISE NOTICE 'Actualizando Venta con secuencial % a pagada', secVent;
				END IF;
			END IF;
			
			RAISE NOTICE 'DESPUES DEL INGRESO DE PAGOS';
			--COMMIT;			

			RAISE NOTICE 'finalizo correctamente y retornará mensaje';
			RETURN QUERY
				SELECT 1 AS codigo, 'Venta creada correctamente con el secuencial '|| secVent::VARCHAR AS descripcion;			
		EXCEPTION WHEN OTHERS THEN
			RAISE NOTICE 'HUBO un error y realizará un rollback';
			RAISE NOTICE 'Codio de error = %', SQLSTATE;
			--ROLLBACK;
			RETURN QUERY			
			SELECT 0 AS codigo, 'La Venta no pudo ser creada' AS descripcion;
		END;
	END IF;
	
	IF accion = 'M' THEN
		RAISE NOTICE 'Modificando factura, solo se registra el pago';
		BEGIN	
			RAISE NOTICE 'OBTENIENDO LOS DATOS DEL JSON DATA';
			
			SELECT 	t.ventscl, t.ventrcb, t.ventvto, t.nidnid, t.ventsbt,
					t.ventdsc, t.ventdpc, t.ventiva, t.venttot
			INTO	secVent, recibido, vuelto, clienid, subtotal, descuento, pdescuento,
					iva, total
			FROM 	json_to_record(data::json) AS 
					t(ventscl VARCHAR(10), ventrcb NUMERIC(14,6), ventvto NUMERIC(14,6), nidnid VARCHAR(20),
					  ventsbt NUMERIC(14,6),ventdsc NUMERIC(14,6), ventdpc INT, ventiva NUMERIC(14,6), venttot NUMERIC(14,6));	
			
			UPDATE 	tbPeddDVent
			SET		nidNid  = clienid,
					ventsbt = subtotal,
					ventdsc = descuento,
					ventdpc = pdescuento,
					ventiva = iva,
					venttot = total,
					venthra = CURRENT_TIME
			WHERE	ventScl = secVent;
			
			RAISE NOTICE 'SEC = %, RECIBIDO = %, VUELTO = %', secVent, recibido, vuelto;
			
			IF((SELECT COUNT(*) FROM tbPeddDFpgo WHERE ventScl = secVent)>0) THEN
				RAISE NOTICE 'ELIMINANDO FORMA DE PAGO EXISTENTE';
				DELETE FROM tbPeddDFpgo WHERE ventScl = secVent;
				RAISE NOTICE 'FORMA DE PAGO ELIMINADA';
			END IF;
			
			IF EXISTS(SELECT * FROM JSON_TO_RECORDSET(pagos::JSON) AS
						t(isefectivo BOOLEAN, valor DECIMAL(14,6), vuelto DECIMAL(14,6), 
						  iscorriente BOOLEAN, isdiferido BOOLEAN, meses INT, tarjeta VARCHAR(1),
						  ischeque BOOLEAN,banco VARCHAR(2), numcheque VARCHAR(6)))
			THEN
				INSERT INTO tbPeddDFpgo
				SELECT secVent, 
						CASE t.isefectivo 
							WHEN TRUE THEN 'E' 
							ELSE 
								CASE t.ischeque 
									WHEN TRUE THEN 'C'
									ELSE 'T' 
								END
						END AS fpgotip, t.valor, 
						t.vuelto, t.tarjeta, 
						CASE WHEN t.iscorriente = TRUE THEN 'C'
							 WHEN t.isdiferido = TRUE THEN 'D'
							 ELSE 'N'
						END AS fpgotpt, t.meses,
						t.banco, t.numcheque
				FROM JSON_TO_RECORDSET(pagos::JSON) AS
						t(isefectivo BOOLEAN, valor DECIMAL(14,6), vuelto DECIMAL(14,6), 
						  iscorriente BOOLEAN, isdiferido BOOLEAN, meses INT, tarjeta VARCHAR(1),
						  ischeque BOOLEAN,banco VARCHAR(2), numcheque VARCHAR(6));

													 

				RAISE NOTICE 'LUEGO DE INSERTAR LA FORMA DE PAGO';
			ELSE
				RAISE NOTICE 'NO HABIA FORMA DE PAGO PARA INSERTAR';
			END IF;
						
			IF ((SELECT COUNT(*) FROM tbPeddDFpgo WHERE ventScl = secVent) > 0) THEN
				UPDATE 	tbPeddDVent 
				SET 	ventRcb = recibido,
						ventVto = vuelto,
						ventObv = '',
						ventPgd = TRUE,
						ventFac = TRUE,
						ventStd = 'P',
						ventAtv = TRUE
				WHERE 	ventScl = secVent;
				 
				RAISE NOTICE 'Actualizando Venta con secuencial % a pagada', secVent;
			ELSE
				UPDATE 	tbPeddDVent 
				SET 	ventRcb = 0,
						ventVto = 0,
						ventPgd = CASE venttip WHEN 'E' THEN TRUE ELSE FALSE END,
						ventFac = TRUE,
						ventObv = '',
						ventStd = CASE venttip WHEN 'E' THEN 'P' ELSE 'F' END,
						ventAtv = TRUE
				WHERE 	ventScl = secVent;
				 
				RAISE NOTICE 'Actualizando Venta con secuencial % a facturado', secVent;
			END IF;

			RAISE NOTICE 'finalizo correctamente y retornará mensaje';
			RETURN QUERY
				SELECT 1 AS codigo, 'Pago de venta con el secuencial '|| secVent::VARCHAR ||' fue registrado correctamente.' AS descripcion;			
		EXCEPTION WHEN OTHERS THEN
			RAISE NOTICE 'HUBO un error y realizará un rollback';
			RAISE NOTICE 'Codio de error = %', SQLSTATE;
			--ROLLBACK;
			RETURN QUERY			
			SELECT 0 AS codigo, 'El pago de la Venta no pudo ser creada' AS descripcion;
		END;
	END IF;
	
	IF accion = 'G' THEN
		RAISE NOTICE 'iniciando transaccion de creado de REGALO';
		BEGIN	
			RAISE NOTICE 'obteniendo secuencial';
			
			SELECT LPAD(COALESCE(MAX(giftScl::int)+1,1)::TEXT,10,'0') INTO secVent FROM tbPeddDGift;
			
			RAISE NOTICE 'secuencial %', secVent;
			RAISE NOTICE 'data = %', data;
			
			fecha := CURRENT_DATE;
			RAISE NOTICE 'despues de obtener la fecha %', fecha;
			
			RAISE NOTICE 'antes de obtener los datos y pasarlos a las variables';
			SELECT t.cajacod, t.empdnid, t.nidnid, t.venttip, t.ventmsa, t.ventlta, t.peddscl, t.ventsbt,
		            t.ventiva, t.venttot, t.ventobv, t.ventatv
					INTO 
					caja, usuario, clienid, tipo, mesa, letra, secPedd, subtotal, iva, total, 
					observ, estado 
            FROM json_to_record(data::json) AS 
				  t(cajacod VARCHAR(4), empdnid VARCHAR(4), nidnid VARCHAR(20), venttip VARCHAR(1), 
					ventmsa VARCHAR(3), ventlta VARCHAR(1), peddscl VARCHAR(10), ventsbt NUMERIC(14,6),
					ventiva NUMERIC(14,6), venttot NUMERIC(14,6), 
					ventobv TEXT, ventatv BOOLEAN);												

			RAISE NOTICE 'PASO LA RECOLECCION DE DATOS';
			RAISE NOTICE 'letra despues del if = %',secPedd;
			RAISE NOTICE 'ANTES DEL INGRESO DE DATOS PRIMARIOS';
			
			INSERT INTO tbPeddDGift
			VALUES(secVent, caja, usuario, fecha, clienid, tipo, mesa, letra, secPedd, CURRENT_TIME, 
				    subtotal, iva, total, observ, true);

			RAISE NOTICE 'paso el INGRESO DE DATOS PRIMARIOS';
			RAISE NOTICE 'detalle = %', detalle;
			
			RAISE NOTICE 'antes del ingreso DEL DETALLE';
			
			INSERT INTO tbPeddMGift
				SELECT secVent, t.itemcod, t.itemcnt, t.itemprc, t.itemiva, t.itempto
				FROM JSON_TO_RECORDSET(detalle::json) AS
						t(itemcod VARCHAR(4), itemcnt INT, itemprc NUMERIC(10,6), 
						itemiva NUMERIC(10,6), itempto NUMERIC(10,6));
					
			RAISE NOTICE 'PASO EL INGRESO DEL DETALLE';
			RAISE NOTICE 'ANTES DEL INGRESO DE PAGOS';
			RAISE NOTICE 'secuencial %', secVent;			
			--COMMIT;			

			RAISE NOTICE 'finalizo correctamente y retornará mensaje';
			RETURN QUERY
				SELECT 1 AS codigo, 'Regalo creado correctamente con el secuencial '|| secVent::VARCHAR AS descripcion;			
		EXCEPTION WHEN OTHERS THEN
			RAISE NOTICE 'HUBO un error y realizará un rollback';
			RAISE NOTICE 'Codio de error = %', SQLSTATE;
			--ROLLBACK;
			
			RETURN QUERY
			SELECT 0 AS codigo, 'El regalo no pudo ser creado' AS descripcion;
		END;
	END IF;
	
	IF accion = 'A' THEN
		BEGIN
			RAISE NOTICE 'VA A ANULAR UNA FACTURA';
			secVent := data;
			RAISE NOTICE 'OBTENIENDO EL SECUENCIAL';
			
			SELECT t.ventobv, t.empdnid, t.numfact INTO observ, usuario, numfact 
            FROM json_to_record(detalle::json) AS 
				  t(ventobv VARCHAR(200), empdnid VARCHAR(4), numfact VARCHAR(20));	
			
			RAISE NOTICE 'OBTENIENDO LOS VALORES DE DETALLE';
			
			IF((SELECT COUNT(*) FROM tbPeddDFpgo WHERE ventScl = secVent)>0) THEN
				RAISE NOTICE 'ELIMINANDO FORMA DE PAGO EXISTENTE';
				DELETE FROM tbPeddDFpgo WHERE ventScl = secVent;
				RAISE NOTICE 'FORMA DE PAGO ELIMINADO';
			END IF;
			
			UPDATE 	tbPeddDVent
			SET 	ventAtv = true, 
					ventfac = false,
					ventPgd = false,
					ventObv = observ,
					ventStd = 'A'
			WHERE	ventscl = secVent;
			
			RAISE NOTICE 'LUEGO DE ANULAR LA FACTURA';
			
			-- CUANDO LA FACTURA PERTENECE A FACTURACION ELECTRONICA
			RAISE NOTICE 'NUMERO FACTURA = %', numfact;
			RAISE NOTICE 'SECUENCIAL = %',SPLIT_PART(numfact, '-',3);
			IF EXISTS(SELECT 1 FROM tbFactElect WHERE ventscl = secVent AND secuencial = SPLIT_PART(numfact, '-',3)) THEN
				RAISE NOTICE 'VA A PROCEDER A MARCAR COMO ANULADA LA FACTURA ELECTRONICA';
				UPDATE 	tbFactElect
				SET		etdofac = 'L'
				WHERE	ventscl = secVent 
						AND secuencial = SPLIT_PART(numfact, '-',3);
			END IF;
			
			INSERT INTO tbPeddHVent
				VALUES (data, 'A', LOCALTIMESTAMP, usuario, numfact, observ);
			
			RAISE NOTICE 'HISTORIAL DE ANULACIÓN REGISTRADO';
			RETURN QUERY
				SELECT 1 AS codigo, 'Factura con el secuencial '|| secVent || ' ha sido anulado correctamente' AS descripcion;	
		EXCEPTION WHEN OTHERS THEN
			RAISE NOTICE 'HUBO un error y realizará un rollback';
			RAISE NOTICE 'Codio de error = %', SQLSTATE;
			
			RETURN QUERY
			SELECT 0 AS codigo, 'Factura con el secuencial '|| secVent ||' no pudo ser anulado' AS descripcion;
		END;
	END IF;	
	
	IF accion = 'E' THEN
		BEGIN
			RAISE NOTICE 'VA A ELIMINAR UNA FACTURA';
			secVent := data;
			RAISE NOTICE 'OBTENIENDO EL SECUENCIAL';
			
			SELECT t.ventobv, t.empdnid, t.numfact INTO observ, usuario, numfact 
            FROM json_to_record(detalle::json) AS 
				  t(ventobv VARCHAR(200), empdnid VARCHAR(4), numfact VARCHAR(20));	
			
			RAISE NOTICE 'SECUENCIAL OBTENIDO';
			
			IF((SELECT COUNT(*) FROM tbPeddDFpgo WHERE ventScl = secVent)>0) THEN
				RAISE NOTICE 'ELIMINANDO FORMA DE PAGO EXISTENTE';
				DELETE FROM tbPeddDFpgo WHERE ventScl = secVent;
				RAISE NOTICE 'FORMA DE PAGO ELIMINADO';
			END IF;
			 
			UPDATE 	tbPeddDVent
			SET 	ventAtv = false, 
					ventfac = false,
					ventPgd = false,
					ventObv = observ,
					ventStd = 'E'
			WHERE	ventscl = secVent;
			
			RAISE NOTICE 'LUEGO DE ELIMINAR LA FACTURA';
			
			-- CUANDO LA FACTURA PERTENECE A FACTURACION ELECTRONICA
			
			RAISE NOTICE 'NUMERO FACTURA = %', numfact;
			RAISE NOTICE 'SECUENCIAL = %',SPLIT_PART(numfact, '-',3);
			IF EXISTS(SELECT 1 FROM tbFactElect WHERE ventscl = secVent AND secuencial = SPLIT_PART(numfact, '-',3)) THEN
				RAISE NOTICE 'VA A PROCEDER A MARCAR COMO ANULADA/ELIMINADA LA FACTURA ELECTRONICA';
				UPDATE 	tbFactElect
				SET		etdofac = 'L'
				WHERE	ventscl = secVent 
						AND secuencial = SPLIT_PART(numfact, '-',3);
			END IF;
			
			INSERT INTO tbPeddHVent
				VALUES (data, 'E', LOCALTIMESTAMP, usuario, numfact, observ);
				
			RAISE NOTICE 'HISTORIAL DE ELIMINACIÓN REGISTRADO';
			RETURN QUERY
				SELECT 1 AS codigo, 'Factura con el secuencial '|| secVent || ' ha sido eliminada correctamente' AS descripcion;	
		EXCEPTION WHEN OTHERS THEN
			RAISE NOTICE 'HUBO un error y realizará un rollback';
			RAISE NOTICE 'Codio de error = %', SQLSTATE;
			
			RETURN QUERY
			SELECT 0 AS codigo, 'Factura con el secuencial '|| secVent ||' no pudo ser eliminada' AS descripcion;
		END;
	END IF;	
END;
$BODY$;

CREATE OR REPLACE FUNCTION public.fnventaspd(
	accion character varying,
	data character varying,
	detalle character varying,
	pagos character varying)
    RETURNS TABLE(codigo integer, descripcion text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
	secVent 	VARCHAR(10);
	secPedd 	VARCHAR(10);
	mesa 		VARCHAR(3);
	caja 		VARCHAR(4);
	tipo 		VARCHAR(1);
	carrier 	VARCHAR(4);
	pctcarrier 	NUMERIC(10,2);
	valcarrier 	NUMERIC(10,2);
	letra 		VARCHAR(1) := NULL;
	usuario 	VARCHAR(4);
	secIni 		INT := 0;
	separado	VARCHAR(1);
	
	clienid 	VARCHAR(20);
	clienbr		VARCHAR(100);
	cliedir		VARCHAR(200);
	clietlf		VARCHAR(20);
	clieref 	VARCHAR(100);
	
	crrcod		VARCHAR(4);
	crrobv		VARCHAR(100);
	 
	subtotal 	NUMERIC(14,6);
	subtniva	NUMERIC(14,6);
	pdescuento 	INT;
	descuento 	NUMERIC(14,6);
	iva 		NUMERIC(14,6);
	total 		NUMERIC(14,6);
	recibido 	NUMERIC(14,6);
	vuelto 		NUMERIC(14,6);
	
	prcCarr 	DECIMAL(10,6);
	ptjCarr 	BOOLEAN;
	aplCarr 	VARCHAR(1);
	
	valIva  	DECIMAL(10,6);
	
	observ 		TEXT;
	estado 		BOOLEAN;
	facturado 	BOOLEAN;
	fecha 		DATE;	
	numfact		VARCHAR(20);
	rdet RECORD;
	_cursor CURSOR FOR 
			SELECT secPedd, t.itemcod, t.itemcnt, t.itemprc, t.itemiva, t.itempto
            FROM JSON_TO_RECORDSET(detale::json) AS
                  t(itemcod VARCHAR(4), itemcnt INT, itemprc NUMERIC(10,6), itemiva NUMERIC(10,6), itempto NUMERIC(10,6));
BEGIN
	RAISE NOTICE 'accion a realizar %', accion;
	pctcarrier := 0;
	valcarrier := 0;
	
	SELECT 	(desencryptar(confval)::DECIMAL/100)::DECIMAL(10,6) 
			INTO valIva 
	FROM 	tbConfParam 
	WHERE 	confNid=2;
	
	IF accion = 'N' THEN
		RAISE NOTICE 'iniciando transaccion de creado de VENTA';
		BEGIN	
			SELECT desencryptar(confval)::INT INTO secIni FROM tbConfParam WHERE confNid=24;
			
			RAISE NOTICE 'obteniendo secuencial para pedido';
			SELECT LPAD(COALESCE(MAX(peddScl::int)+1,1)::TEXT,10,'0') INTO secPedd FROM tbPeddDPedd;
			
			IF secPedd::INT < secIni THEN
				secPedd := LPAD(secIni::TEXT,10,'0');
			END IF;
			
			RAISE NOTICE 'secuencial Pedido %', secPedd;
			
			RAISE NOTICE 'obteniendo secuencial para venta';
			SELECT LPAD(COALESCE(MAX(ventScl::int)+1,1)::TEXT,10,'0') INTO secVent FROM tbPeddDVent;
			
			IF secVent::INT < secIni THEN
				secVent := LPAD(secIni::TEXT,10,'0');
			END IF;
			
			RAISE NOTICE 'secuencial Venta %', secVent;
			RAISE NOTICE 'data = %', data;
			
			fecha := CURRENT_DATE;
			RAISE NOTICE 'despues de obtener la fecha %', fecha;
			
			RAISE NOTICE 'antes de obtener los datos para pedido y pasarlos a las variables';
			SELECT t.empdnid, t.venttip, t.ventccr, t.ventmsa, t.ventlta, t.nidnid, t.clienbr, t.cliedir, t.clietlf, t.clieref, t.crriCod, t.crriObv
			        INTO usuario, tipo, carrier, mesa, letra, clienid, clienbr, cliedir, clietlf, clieref, crrcod, crrobv
			FROM json_to_record(data::json) AS 
				  t(empdnid VARCHAR(4), venttip VARCHAR(1), ventccr VARCHAR(4),
					ventmsa VARCHAR(3), ventlta VARCHAR(1), nidnid VARCHAR(20),
				    clienbr VARCHAR(50), cliedir VARCHAR(100), clietlf VARCHAR(50),
				    clieref VARCHAR(100), crricod VARCHAR(10), crriobv VARCHAR(100));
			
			RAISE NOTICE 'usu = % tip = % msa = % obv = % ', usuario, tipo, mesa, observ;
			RAISE NOTICE 'antes de obtener la letra';
			RAISE NOTICE 'crrCod = %, crrObv = %', crrcod, crrobv;
			
			SELECT CHR(MAX(ASCII(p.peddlta::CHAR))+1) INTO letra
			FROM   tbPeddDPedd p 
			WHERE  p.peddfec = fecha AND
					p.peddmsa = mesa AND
					p.peddatv = true AND p.peddeli = false;
					
			RAISE NOTICE 'despues de obtener letra';
			RAISE NOTICE 'letra = %',letra;
			IF letra IS NULL THEN 
				letra := 'A';
			END IF;
			
			RAISE NOTICE 'letra despues del if = %',letra;
			
			INSERT INTO tbPeddDPedd 
				(peddScl, peddLta, usuaCod, peddFec, peddTip, peddCcr, peddMsa, peddObv, peddHin, peddHFn, peddAtv, peddEli)
			VALUES(secPedd, letra, usuario, fecha, tipo, carrier, mesa, '', CURRENT_TIME, CURRENT_TIME, true, false);					

			RAISE NOTICE 'paso el ingreso de datos';
			RAISE NOTICE 'detalle = %', detalle;
			
			IF tipo = 'D' THEN
				INSERT INTO tbPeddDatCli 
				VALUES (secPedd, clienid, clienbr, cliedir, clietlf, clieref);
			END IF;
			
			IF tipo = 'E' THEN
				INSERT INTO tbPeddDatCarr 
				VALUES(secPedd, carrier, crrCod, crrObv);
			END IF;
			
			INSERT INTO tbPeddMPedd
				SELECT secPedd, t.itemcod, t.itemcnt, t.usaiva, t.itemprc, t.itemiva, t.itempto, true
				FROM JSON_TO_RECORDSET(detalle::json) AS
					   t(itemcod VARCHAR(4), itemcnt INT, usaiva BOOLEAN, itemprc NUMERIC(10,6), 
						itemiva NUMERIC(10,6), itempto NUMERIC(10,6));	
			
			RAISE NOTICE 'paso el ingreso del detalle de pedido';
			
			--{"cajacod":"0001","empdnid":"0003","nidnid":"9999999999999","clienbr":"","cliedir":"","clietlf":"","clieref":"","venttip":"L","ventccr":"0000","ventccd":"","ventcro":"","ventmsa":"","ventlta":"A","peddscl":"0000000000","ventsbt":2.232143,"ventdsc":0.0,"ventdpc":0,"ventiva":0.26785716000000004,"venttot":2.5000001600000004,"ventrcb":0.0,"ventvto":-2.5000001600000004,"ventobv":"","ventatv":true,"ventfac":false,"ventsep":"C"}
			
			RAISE NOTICE 'antes de obtener los datos para venta y pasarlos a las variables';
			SELECT 	t.cajacod, t.empdnid, t.nidnid, t.venttip, t.ventccr, t.ventmsa, t.ventlta, t.ventsbt,
		            t.ventdsc, t.ventdpc, t.ventiva, t.venttot, t.ventrcb, t.ventvto, t.ventobv, t.ventatv, t.ventfac,
					t.ventsep, t.ventsbn
					INTO 
					caja, usuario, clienid, tipo, carrier, mesa, letra, subtotal, descuento, pdescuento, 
					iva, total, recibido, vuelto, observ, estado, facturado, separado, subtniva
            FROM json_to_record(data::json) AS 
				  t(cajacod VARCHAR(4), empdnid VARCHAR(4), nidnid VARCHAR(20), venttip VARCHAR(1), ventccr VARCHAR(4),
					ventmsa VARCHAR(3), ventlta VARCHAR(1), ventsbt NUMERIC(14,6),
					ventdsc NUMERIC(14,6), ventdpc INT, ventiva NUMERIC(14,6), venttot NUMERIC(14,6), 
					ventrcb NUMERIC(14,6), ventvto NUMERIC(14,6), ventobv TEXT, ventatv BOOLEAN, ventfac BOOLEAN,
				    ventsep VARCHAR(1), ventsbn NUMERIC(14,6));												

			RAISE NOTICE 'PASO LA RECOLECCION DE DATOS';
			RAISE NOTICE 'letra despues del if = %',secPedd;
			RAISE NOTICE 'ANTES DEL INGRESO DE DATOS PRIMARIOS';
			
			IF carrier <> '0000' THEN
				RAISE NOTICE 'OBTENIENDO LOS DATOS DEL CARRIER';
				
				--SELECT 	crriprc::DECIMAL(10,6), crripct, crriapi INTO prcCarr, ptjCarr, aplCarr FROM tbAdsiCrrie WHERE crriNid = carrier;

				SELECT 	SUM(
							CASE
								WHEN i.itemiva = TRUE THEN ABS((i.itemprc/(1 + valIva))::decimal(10,6) - a.itemprc)*a.itemcnt
								ELSE (i.itemprc::DECIMAL(10,6) - a.itemprc)*itemcnt
							END
						)  
						INTO 
						valCarrier
				FROM(
					SELECT 	t.itemcod, t.itemcnt, t.itemprc, t.itemiva
					FROM 	JSON_TO_RECORDSET(detalle::json) AS
							t(itemcod VARCHAR(4), itemcnt INT, itemprc NUMERIC(10,6), itemiva NUMERIC(10,6))
					) a
				INNER JOIN
				tbIvtrItem i ON a.itemcod = i.itemcod;
				
				SELECT 	/*CASE crripct
							WHEN TRUE THEN valCarrier --total + total*crriprc/100
							ELSE subtotal - crriprc --total + crriprc
						END AS valor, */
						CASE crripct
							WHEN TRUE THEN crriprc
							ELSE 0
						END AS porcentaje
						INTO 
						pctcarrier
				FROM 	tbAdsiCrrie
				WHERE	crriNid = carrier;
				
				RAISE NOTICE 'SE OBTUVIERON LOS DATOS DEL CARRIER';
			END IF;
			
			INSERT INTO tbPeddDVent
				(ventscl, cajacod, usuacod, ventfch, nidnid, venttip, ventcrr, ventcpt, ventcvl, ventmsa, ventlta, peddscl,
				 venthra, ventsbt, ventdsc, ventdpc, ventiva, venttot, ventrcb, ventvto, ventobv, ventatv, ventfac, ventPgd,
				 ventstd, ventsep, ventstn)
			VALUES(secVent, caja, usuario, fecha, clienid, tipo, carrier, pctcarrier, valcarrier, mesa, letra, secPedd, 
				   CURRENT_TIME, subtotal, descuento, pdescuento, iva, total, recibido, vuelto, '', true, true, 
				   CASE WHEN recibido > 0 THEN true ELSE false END,
				   CASE WHEN recibido > 0 THEN 'P' ELSE 'F' END,
				   separado, subtniva);

			RAISE NOTICE 'paso el INGRESO DE DATOS PRIMARIOS';
			RAISE NOTICE 'detalle = %', detalle;
			
			RAISE NOTICE 'antes del ingreso DEL DETALLE';
			
			INSERT INTO tbPeddMVent
				SELECT secVent, t.itemcod, t.itemcnt, t.itemprc, t.itemiva, t.itempto
				FROM JSON_TO_RECORDSET(detalle::json) AS
						t(itemcod VARCHAR(4), itemcnt INT, itemprc NUMERIC(10,6), 
						itemiva NUMERIC(10,6), itempto NUMERIC(10,6));
					
			RAISE NOTICE 'PASO EL INGRESO DEL DETALLE';
			RAISE NOTICE 'ANTES DEL INGRESO DE PAGOS';
			RAISE NOTICE 'secuencial %', secVent;
			
			INSERT INTO tbPeddDFpgo
			SELECT secVent, 
					CASE t.isefectivo 
						WHEN TRUE THEN 'E' 
						ELSE 
							CASE t.ischeque 
								WHEN TRUE THEN 'C'
								ELSE 'T' 
							END
					END AS fpgotip, t.valor, 
					t.vuelto, t.tarjeta, 
			        CASE WHEN t.iscorriente = TRUE THEN 'C'
				         WHEN t.isdiferido = TRUE THEN 'D'
					 	 ELSE 'N'
				    END AS fpgotpt, t.meses,
					t.banco, t.numcheque
            FROM JSON_TO_RECORDSET(pagos::JSON) AS
					t(isefectivo BOOLEAN, valor DECIMAL(14,6), vuelto DECIMAL(14,6), 
					  iscorriente BOOLEAN, isdiferido BOOLEAN, meses INT, tarjeta VARCHAR(1),
					  ischeque BOOLEAN,banco VARCHAR(2), numcheque VARCHAR(6));
					  
			IF tipo = 'E' THEN
				UPDATE tbPeddDVent SET ventPgd = TRUE WHERE ventScl = secVent;
				RAISE NOTICE 'Actualizado venta con secuencial % a pagado en caso de ser tipo CARRIER', secVent;
			ELSE
				IF ((SELECT COUNT(*) FROM tbPeddDFpgo WHERE ventScl = secVent) > 0) THEN
					UPDATE tbPeddDVent SET ventPgd = TRUE WHERE ventScl = secVent;
					RAISE NOTICE 'Actualizando Venta con secuencial % a pagada', secVent;
				END IF;
			END IF;
			
			RAISE NOTICE 'DESPUES DEL INGRESO DE PAGOS';
			--COMMIT;			

			RAISE NOTICE 'finalizo correctamente y retornará mensaje';
			RETURN QUERY
				SELECT 1 AS codigo, 'Venta creada correctamente con el secuencial '|| secVent::VARCHAR AS descripcion;			
		EXCEPTION WHEN OTHERS THEN
			RAISE NOTICE 'HUBO un error y realizará un rollback';
			RAISE NOTICE 'Codio de error = %', SQLSTATE;
			--ROLLBACK;
			RETURN QUERY			
			SELECT 0 AS codigo, 'La Venta no pudo ser creada' AS descripcion;
		END;
	END IF;
	
	IF accion = 'A' THEN
		BEGIN
			RAISE NOTICE 'VA A ANULAR UNA FACTURA';
			secVent := data;
			RAISE NOTICE 'OBTENIENDO EL SECUENCIAL';
			
			SELECT t.ventobv, t.empdnid, t.numfact INTO observ, usuario, numfact 
            FROM json_to_record(detalle::json) AS 
				  t(ventobv VARCHAR(200), empdnid VARCHAR(4), numfact VARCHAR(20));	
			
			RAISE NOTICE 'OBTENIENDO LOS VALORES DE DETALLE';
			
			IF((SELECT COUNT(*) FROM tbPeddDFpgo WHERE ventScl = secVent)>0) THEN
				RAISE NOTICE 'ELIMINANDO FORMA DE PAGO EXISTENTE';
				DELETE FROM tbPeddDFpgo WHERE ventScl = secVent;
				RAISE NOTICE 'FORMA DE PAGO ELIMINADO';
			END IF;
			
			UPDATE 	tbPeddDVent
			SET 	ventAtv = true, 
					ventfac = false,
					ventPgd = false,
					ventObv = observ,
					ventStd = 'A'
			WHERE	ventscl = secVent;
			
			RAISE NOTICE 'LUEGO DE ANULAR LA FACTURA';
			
			-- CUANDO LA FACTURA PERTENECE A FACTURACION ELECTRONICA
			RAISE NOTICE 'NUMERO FACTURA = %', numfact;
			RAISE NOTICE 'SECUENCIAL = %',SPLIT_PART(numfact, '-',3);
			IF EXISTS(SELECT 1 FROM tbFactElect WHERE ventscl = secVent AND secuencial = SPLIT_PART(numfact, '-',3)) THEN
				RAISE NOTICE 'VA A PROCEDER A MARCAR COMO ANULADA LA FACTURA ELECTRONICA';
				UPDATE 	tbFactElect
				SET		etdofac = 'L'
				WHERE	ventscl = secVent 
						AND secuencial = SPLIT_PART(numfact, '-',3);
			END IF;
			
			INSERT INTO tbPeddHVent
				VALUES (data, 'A', LOCALTIMESTAMP, usuario, numfact, observ);
			
			RAISE NOTICE 'HISTORIAL DE ANULACIÓN REGISTRADO';
			RETURN QUERY
				SELECT 1 AS codigo, 'Factura con el secuencial '|| secVent || ' ha sido anulado correctamente' AS descripcion;	
		EXCEPTION WHEN OTHERS THEN
			RAISE NOTICE 'HUBO un error y realizará un rollback';
			RAISE NOTICE 'Codio de error = %', SQLSTATE;
			
			RETURN QUERY
			SELECT 0 AS codigo, 'Factura con el secuencial '|| secVent ||' no pudo ser anulado' AS descripcion;
		END;
	END IF;	
	
	IF accion = 'E' THEN
		BEGIN
			RAISE NOTICE 'VA A ELIMINAR UNA FACTURA';
			secVent := data;
			RAISE NOTICE 'OBTENIENDO EL SECUENCIAL';
			
			SELECT t.ventobv, t.empdnid, t.numfact INTO observ, usuario, numfact 
            FROM json_to_record(detalle::json) AS 
				  t(ventobv VARCHAR(200), empdnid VARCHAR(4), numfact VARCHAR(20));	
			
			RAISE NOTICE 'SECUENCIAL OBTENIDO';
			
			IF((SELECT COUNT(*) FROM tbPeddDFpgo WHERE ventScl = secVent)>0) THEN
				RAISE NOTICE 'ELIMINANDO FORMA DE PAGO EXISTENTE';
				DELETE FROM tbPeddDFpgo WHERE ventScl = secVent;
				RAISE NOTICE 'FORMA DE PAGO ELIMINADO';
			END IF;
			 
			UPDATE 	tbPeddDVent
			SET 	ventAtv = false, 
					ventfac = false,
					ventPgd = false,
					ventObv = observ,
					ventStd = 'E'
			WHERE	ventscl = secVent;
			
			RAISE NOTICE 'LUEGO DE ELIMINAR LA FACTURA';
			
			-- CUANDO LA FACTURA PERTENECE A FACTURACION ELECTRONICA
			
			RAISE NOTICE 'NUMERO FACTURA = %', numfact;
			RAISE NOTICE 'SECUENCIAL = %',SPLIT_PART(numfact, '-',3);
			IF EXISTS(SELECT 1 FROM tbFactElect WHERE ventscl = secVent AND secuencial = SPLIT_PART(numfact, '-',3)) THEN
				RAISE NOTICE 'VA A PROCEDER A MARCAR COMO ANULADA/ELIMINADA LA FACTURA ELECTRONICA';
				UPDATE 	tbFactElect
				SET		etdofac = 'L'
				WHERE	ventscl = secVent 
						AND secuencial = SPLIT_PART(numfact, '-',3);
			END IF;
			
			INSERT INTO tbPeddHVent
				VALUES (data, 'E', LOCALTIMESTAMP, usuario, numfact, observ);
				
			RAISE NOTICE 'HISTORIAL DE ELIMINACIÓN REGISTRADO';
			RETURN QUERY
				SELECT 1 AS codigo, 'Factura con el secuencial '|| secVent || ' ha sido eliminada correctamente' AS descripcion;	
		EXCEPTION WHEN OTHERS THEN
			RAISE NOTICE 'HUBO un error y realizará un rollback';
			RAISE NOTICE 'Codio de error = %', SQLSTATE;
			
			RETURN QUERY
			SELECT 0 AS codigo, 'Factura con el secuencial '|| secVent ||' no pudo ser eliminada' AS descripcion;
		END;
	END IF;	
END;
$BODY$;

CREATE OR REPLACE FUNCTION public.fnverificaventa(
	p_factura character varying,
	p_cliente character varying)
    RETURNS TABLE(ventscl character varying, ventfch date, nidnid character varying, ventsbt numeric, ventiva numeric, venttot numeric, ventstd character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
    CodEmp VARCHAR(3);
	CodFac VARCHAR(3);
BEGIN
	/*
	SELECT * FROM fnVerificaVenta('002-002-000000150'::character varying,'0914556634'::character varying);
	*/
	RAISE NOTICE 'OBTENIENDO VALORES DE CONFIGURACION';
	SELECT desencryptar(emprecod) INTO CodEmp FROM tbAdsiEmpr;
	SELECT desencryptar(confval) INTO CodFac FROM tbConfParam WHERE confnid=67;
	
	RAISE NOTICE 'OBTENIENDO RESULTADO';
	RETURN QUERY
		SELECT 	v.ventscl,
				v.ventfch,
				v.nidnid,
				v.ventsbt - v.ventdsc,
				v.ventiva,
				v.venttot,
				v.ventstd
		FROM 	tbPeddDVent v
				INNER JOIN tbFactElect f ON v.ventscl = f.ventscl
		WHERE 	1 = 1 
				AND CodEmp || '-' || CodFac || '-' || f.secuencial = p_factura
				AND v.nidnid = p_cliente
				AND v.ventstd = 'P';
END;
$BODY$;






CREATE OR REPLACE FUNCTION public.fnconfiguracion(
	data character varying)
    RETURNS TABLE(codigo integer, descripcion text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
	
BEGIN
	RAISE NOTICE 'Eliminando tabla temporal en caso de existir';
	DROP TABLE IF EXISTS tmpConfig;
	
	RAISE NOTICE 'Obteniendo los datos y generando tabla temporal';
	CREATE TEMP TABLE tmpConfig AS
	SELECT 	t.codigo, REPLACE(t.valor,'\','\\') AS valor
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
			
			RAISE NOTICE 'Actualizando Logotipo para Facturación electrónica.';
			UPDATE 	tbAdsiEmpr
			SET		emprlofc = (SELECT CASE t.valor
										WHEN null THEN null
										WHEN '*' THEN tbAdsiEmpr.emprlofc
										ELSE decode(t.valor, 'base64')
									 END
							  FROM tmpConfig t WHERE t.codigo = 9988);
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
				,emprlofc
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
				(SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9989),
				(SELECT encryptar(t.valor) FROM tmpConfig t WHERE t.codigo = 9988)
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
$BODY$;
