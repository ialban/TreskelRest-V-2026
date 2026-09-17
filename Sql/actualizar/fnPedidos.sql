CREATE OR REPLACE FUNCTION public.fnpedidos(
	accion character varying,
	data character varying,
	detalle character varying)
    RETURNS TABLE(codigo integer, descripcion text) 
    LANGUAGE 'plpgsql'

    COST 100
    VOLATILE 
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