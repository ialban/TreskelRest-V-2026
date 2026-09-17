ALTER TABLE tbCompDComp ADD codCaja VARCHAR(4) NOT NULL DEFAULT '0001';
21613
DROP FUNCTION IF EXISTS fnItemsInvent;
CREATE OR REPLACE FUNCTION fnItemsInvent(IN peddScl VARCHAR, IN data VARCHAR, IN detalle VARCHAR)
RETURNS INT
AS $$
DECLARE 
	fecha DATE;	
	reg RECORD;	
	
	cItems CURSOR FOR 
		SELECT codigo, 
				fnConvertUnidades(cantidad, unidad, unidst) AS cantidad,
				unidst AS unidad
		FROM (
			SELECT r.prdcod AS codigo,
				r.rctacnt*a.cantidad AS cantidad,
				r.rctaund AS unidad,
				p.prdund As unidst,
				a.id
			FROM tbIvtrRcta r
				INNER JOIN
				tbIvtrPrd p ON p.prdCod = r.prdcod
				INNER JOIN 
				(
					select * 
					from json_to_recordset(data::json)
					t(id INT, codigo varchar(4), cantidad int)
				) a ON a.codigo = r.itemcod
			Where r.rctainv = true 
			) b 
			WHERE
				b.codigo NOT IN (
					select codigo
					from json_to_recordset(detalle::json)
					t(id INT, codigo varchar(6))
					WHERE t.id = b.id
				);
	
	cItemsNoRcta CURSOR FOR
		SELECT codigo, 
				fnConvertUnidades(cantidad, unidad, unidst) AS cantidad,
				unidst AS unidad
		FROM (
			SELECT P.prdcod AS codigo,
				a.cantidad::numeric(10,2) AS cantidad,
				p.prdund AS unidad,
				p.prdund As unidst,
				a.id
			FROM tbIvtrPrd p 
				INNER JOIN
				tbIvtrItem i ON i.prdcod = p.prdcod
				INNER JOIN 
				(
					select * 
					from json_to_recordset(data::json)
					t(id INT, codigo varchar(4), cantidad int)
				) a ON a.codigo = i.itemcod
			) b 
			WHERE
				b.codigo NOT IN (
					select codigo
					from json_to_recordset(detalle::json)
					t(id INT, codigo varchar(6))
					WHERE t.id = b.id
				);
BEGIN
	fecha := CURRENT_DATE;
	
	FOR reg IN cItems LOOP
		RAISE NOTICE 'COD = %, CANT=%, UND=%', reg.codigo, reg.cantidad, reg.unidad;

		INSERT INTO tbIvtrDInvMT
		VALUES (reg.codigo, 'Preparación de alimentos pedido '||peddScl, 'E', current_date, reg.cantidad, reg.unidad);
		
		call fnKardex(reg.codigo,EXTRACT(YEAR FROM fecha)::INT,EXTRACT(MONTH FROM fecha)::INT,0,reg.cantidad);
	END LOOP;
	
	FOR reg IN cItemsNoRcta LOOP
		RAISE NOTICE 'COD = %, CANT=%, UND=%', reg.codigo, reg.cantidad, reg.unidad;

		INSERT INTO tbIvtrDInvMT
		VALUES (reg.codigo, 'Venta de Producto pedido '||peddScl, 'E', current_date, reg.cantidad, reg.unidad);
		
		call fnKardex(reg.codigo,EXTRACT(YEAR FROM fecha)::INT,EXTRACT(MONTH FROM fecha)::INT,0,reg.cantidad);
	END LOOP;
	RETURN 1;
END;
$$
LANGUAGE 'plpgsql';

DROP FUNCTION IF EXISTS fnRptControlCajaDiario;
CREATE OR REPLACE FUNCTION fnRptControlCajaDiario(IN anio INT, IN mes INT, in dia INT)
RETURNS TABLE(	
	fecha	DATE,
	caja	VARCHAR(20),
	resumen VARCHAR(100),
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
	_inicio :=  (anio ||'-'|| lpad(mes::text,2,'0') || '-01')::DATE;
	_fecha  :=  (anio ||'-'|| lpad(mes::text,2,'0') || '-' || lpad(dia::text,2,'0') )::DATE;
	
	RAISE NOTICE 'FECHA = %', _fecha;
	
	RETURN QUERY
	SELECT B.fecha, B.caja, B.resumen::VARCHAR(100), B.inicio, B.ingreso, B.egreso, B.cierre, B.sistema, B.diferencia
	FROM (
			SELECT	* 
			FROM (
					SELECT 	d.dia as fecha, c.cajaNbr AS caja, 'RESUMEN CAJA'	AS resumen,
							COALESCE((SELECT ctrlvin FROM tbCtrlCaja WHERE ctrlFch=d.dia AND cajaCod=c.cajaCod),0)::NUMERIC(10,2) AS inicio, 
							0::NUMERIC(10,2) AS ingreso, 
							0::NUMERIC(10,2) AS egreso, 
							COALESCE((SELECT ctrlvcr FROM tbCtrlCaja WHERE ctrlFch=d.dia AND cajaCod=c.cajaCod),0)::NUMERIC(10,2) AS cierre, 
							COALESCE((SELECT ctrlvsi FROM tbCtrlCaja WHERE ctrlFch=d.dia AND cajaCod=c.cajaCod),0)::NUMERIC(10,2) AS sistema, 
							COALESCE((SELECT ctrldif FROM tbCtrlCaja WHERE ctrlFch=d.dia AND cajaCod=c.cajaCod),0)::NUMERIC(10,2) diferencia ,
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
		WHERE 	p.fpgotip = 'E' AND
				v.ventatv = true AND
				v.ventpgd = true AND
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
		WHERE	p.fpgotip = 'E' AND
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


DROP FUNCTION IF EXISTS fnCompras;
CREATE OR REPLACE FUNCTION fnCompras( IN accion VARCHAR, IN data VARCHAR, IN detalle VARCHAR, IN pagos VARCHAR)
RETURNS TABLE(
	codigo INT,
	descripcion TEXT
)
AS $$
DECLARE 
	secComp VARCHAR(10);
	usuario VARCHAR(4);	
	fecha DATE;	
	
	compfac VARCHAR(6);
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
				  	t(codempr VARCHAR(20),numfact VARCHAR(10), fecha DATE, subtot NUMERIC(10,2), 
					  iva NUMERIC(10,2), total NUMERIC(10,2), compatv BOOLEAN, codcaja VARCHAR(4));
			
			RAISE NOTICE 'despues de obtener los datos de DATA';
			INSERT INTO tbCompDComp
				(compnid, compfac, provnid, compfch, compsbt, compiva, comptot, compatv, codcaja)
			VALUES (secComp, compfac, provnid, fecha, compsbt, compiva, comptot, true, codcaja);
					
			

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
$$
LANGUAGE 'plpgsql';