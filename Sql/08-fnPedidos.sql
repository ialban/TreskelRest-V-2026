DROP FUNCTION IF EXISTS fnPedidos;
CREATE OR REPLACE FUNCTION public.fnpedidos(
	accion character varying,
	data character varying,
	detalle character varying)
    RETURNS TABLE(codigo integer, descripcion text) 
AS $$
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
$$
LANGUAGE 'plpgsql';

--SELECT * FROM fnPedidos('','');

DROP FUNCTION fnRptPedidos;
CREATE OR REPLACE FUNCTION fnRptPedidos(IN desde VARCHAR(10), IN hasta VARCHAR(10))
RETURNS TABLE(
	fecha 	varchar(10),
	tipo 	varchar(15),
	ltipo 	varchar(15),
	lcnt 	int,
	lpago	NUMERIC(10,2),
	dtipo	varchar(15),	
	dcnt	int,
	dpago	NUMERIC(10,2),
	ctipo	varchar(15),
	ccnt	int,
	cpago	NUMERIC(10,2),
	ctotal 	int,
	ptotal	NUMERIC(10,2)
)
AS $$
DECLARE
	_inicio 	DATE;
	_fin		DATE;
BEGIN
	_inicio := desde::DATE;
	_fin 	:= hasta::DATE;
	
	create temporary table tmp_pedidos(
		fecha 	date,
		tipo 	varchar(1),
		ltipo 	varchar(15),
		lcnt 	int,
		lpago	NUMERIC(10,2),
		dtipo	varchar(15),	
		dcnt	int,
		dpago	NUMERIC(10,2),
		ctipo	varchar(15),
		ccnt	int,
		cpago	NUMERIC(10,2)
	);

	INSERT INTO tmp_pedidos
	SELECT 	fecha.fecha::date, 'V' as tipo, 
			'LOCAL' as ltipo, 0 as lcnt, 0 as lpago,
			'DOMICILIO' as dtipo, 0 as dcnt, 0 as dpago,
			'LLEVAR' as ctipo, 0 as ccnt, 0 as cpago
	FROM generate_series ( desde::timestamp, hasta::timestamp, '1 day'::interval) as fecha;

	INSERT INTO tmp_pedidos 
	SELECT 	fecha.fecha::date, 'E' as tipo,
			'LOCAL' as ltipo, 0 as lcnt, 0 as lpago,
			'DOMICILIO' as dtipo, 0 as dcnt, 0 as dpago,
			'LLEVAR' as ctipo, 0 as ccnt, 0 as cpago
	FROM generate_series( desde::timestamp, hasta::timestamp, '1 day'::interval) as fecha;

	-- TIPO C PARA LOS PEDIDOS NO VENDIDOS, PERO QUE NO FUERON ELIMINADOS
	INSERT INTO tmp_pedidos 
	SELECT 	fecha.fecha::date, 'C' AS tipo,
			'LOCAL' AS ltipo, 0 AS lcnt, 0 AS lpago,
			'DOMICILIO' AS dtipo, 0 AS dcnt, 0 as dpago,
			'LLEVAR' AS ctipo, 0 AS ccnt, 0 AS cpago
	FROM generate_series( desde::timestamp, hasta::timestamp, '1 day'::interval) AS fecha;

	UPDATE tmp_pedidos 
	SET
		lcnt = a.cantidad, 
		lpago = a.pago
	FROM (	
		SELECT 	p.peddfec, SUM(v.venttot) AS pago,
				COUNT(*) AS cantidad
		FROM 	tbPeddDPedd p
				INNER JOIN
				tbPeddDVent v ON p.peddScl=v.peddScl
		WHERE	p.peddtip='L'
				AND p.peddfec >= _inicio AND p.peddfec <= _fin
		GROUP BY peddfec
	) a
	WHERE 	tmp_pedidos.fecha=a.peddfec AND
			tmp_pedidos.tipo='V';

	UPDATE tmp_pedidos 
	SET
		dcnt = a.cantidad, 
		dpago = a.pago
	FROM (	
		SELECT 	p.peddfec, sum(v.venttot) AS pago,
				count(*) AS cantidad
		FROM 	tbPeddDPedd p
				INNER JOIN
				tbPeddDVent v ON p.peddScl=v.peddScl
		WHERE	(p.peddtip='D' OR p.peddtip='E')
				AND p.peddfec >= _inicio AND p.peddfec <= _fin
		GROUP BY peddfec
	) a
	WHERE 	tmp_pedidos.fecha=a.peddfec AND
			tmp_pedidos.tipo='V';		

	update tmp_pedidos 
	set
		ccnt = a.cantidad, 
		cpago = a.pago
	from (	
	select 	p.peddfec, sum(v.venttot) as pago,
			count(*) as cantidad
	from 	tbPeddDPedd p
			inner join
			tbPeddDVent v ON p.peddScl=v.peddScl
	where	(p.peddtip='C' OR p.peddtip='R')
			and p.peddfec >= _inicio and p.peddfec <= _fin
	group by peddfec
	) a
	where 	tmp_pedidos.fecha=a.peddfec and
			tmp_pedidos.tipo='V';		

	update tmp_pedidos 
	set
		lcnt = a.cantidad, 
		lpago = a.pago
	from (			
	select 	peddfec, count(*) as cantidad, 0 as pago
	from 	tbPeddDPedd
	where	peddtip = 'L'
			and peddfec >= _inicio and peddfec <= _fin
			and peddhfn is null 
			and peddeli = false
	group by peddfec		
	) a 	
	where 	tmp_pedidos.fecha=a.peddfec and
			tmp_pedidos.tipo='C';

	update tmp_pedidos 
	set
		dcnt = a.cantidad, 
		dpago = a.pago
	from (			
	select 	peddfec, count(*) as cantidad, 0 as pago
	from 	tbPeddDPedd
	where	(peddtip = 'D' or peddtip='E')
			and peddfec >= _inicio and peddfec <= _fin
			and peddhfn is null 
			and peddeli = false
	group by peddfec		
	) a 	
	where 	tmp_pedidos.fecha=a.peddfec and
			tmp_pedidos.tipo='C';

	update tmp_pedidos 
	set
		ccnt = a.cantidad, 
		cpago = a.pago
	from (			
	select 	peddfec, count(*) as cantidad, 0 as pago
	from 	tbPeddDPedd
	where	(peddtip = 'C' OR peddtip = 'R')
			and peddfec >= _inicio and peddfec <= _fin
			and peddhfn is null 
			and peddeli = false
	group by peddfec		
	) a 	
	where 	tmp_pedidos.fecha=a.peddfec and
			tmp_pedidos.tipo='C';

	update tmp_pedidos 
	set
		lcnt = a.cantidad, 
		lpago = a.pago
	from (			
	select 	peddfec, count(*) as cantidad, 0 as pago
	from 	tbPeddDPedd
	where	peddtip = 'L'
			and peddfec >= _inicio and peddfec <= _fin
			and peddhfn is null 
			and peddeli = true
	group by peddfec		
	) a 	
	where 	tmp_pedidos.fecha=a.peddfec and
			tmp_pedidos.tipo='E';

	update tmp_pedidos 
	set
		dcnt = a.cantidad, 
		dpago = a.pago
	from (			
	select 	peddfec, count(*) as cantidad, 0 as pago
	from 	tbPeddDPedd
	where	(peddtip = 'D' or peddtip = 'E')
			and peddfec >= _inicio and peddfec <= _fin
			and peddhfn is null 
			and peddeli = true
	group by peddfec		
	) a 	
	where 	tmp_pedidos.fecha=a.peddfec and
			tmp_pedidos.tipo='E';

	update tmp_pedidos 
	set
		ccnt = a.cantidad, 
		cpago = a.pago
	from (			
	select 	peddfec, count(*) as cantidad, 0 as pago
	from 	tbPeddDPedd
	where	(peddtip = 'C' or peddtip = 'R')
			and peddfec >= _inicio and peddfec <= _fin
			and peddhfn is null 
			and peddeli = true
	group by peddfec		
	) a 	
	where 	tmp_pedidos.fecha=a.peddfec and
			tmp_pedidos.tipo='E';

	RETURN QUERY
		SELECT 	to_char(p.fecha,'yyyy-mm-dd')::varchar(10) as fecha, 
				case p.tipo
					when 'V' then 'FACTURADOS'
					when 'C' then 'NO FACTURADOS'
					when 'E' then 'ELIMINADOS'
				end :: varchar(15) as tipo, 
				p.ltipo,p.lcnt,p.lpago, p.dtipo,p.dcnt,p.dpago,p.ctipo,p.ccnt,p.cpago,
				p.lcnt+p.dcnt+p.ccnt AS ctotal, p.lpago+p.dpago+p.cpago AS ptotal 
		FROM 	tmp_pedidos p
		order by p.tipo desc, p.fecha asc;

	drop table tmp_pedidos;
	RETURN;
END;
$$
LANGUAGE 'plpgsql';

--select * from fnRptPedidos('2020-01-01','2020-01-31')	