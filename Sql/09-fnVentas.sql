DROP FUNCTION IF EXISTS fnGetPeddVent;

CREATE OR REPLACE FUNCTION fnGetPeddVent(IN codigo VARCHAR)
RETURNS TABLE(

)
AS$$
DECLARE
BEGIN

END
$$
LANGUAGE 'plpgsql';



DROP FUNCTION IF EXISTS fnListPeddVent;

CREATE OR REPLACE FUNCTION fnListPeddVent(IN codigo VARCHAR)
RETURNS TABLE(
	ventScl VARCHAR(10),
	peddScl VARCHAR(10),
	peddLta VARCHAR(1),
	usuaCod VARCHAR(4),
	peddFec DATE,
	peddTip VARCHAR(1),
	ventCrr VARCHAR(4),
	peddMsa VARCHAR(3),
	peddObv VARCHAR(100),
	nidNid  VARCHAR(20),
	nidNbr  VARCHAR(100),
	ventFac BOOLEAN,
	ventSbt DECIMAL(10,2), 
	ventDsc DECIMAL(10,2), 
	ventIva DECIMAL(10,2), 
	ventTot DECIMAL(10,2), 
	ventRcb DECIMAL(10,2), 
	ventVto DECIMAL(10,2),
	crrprc	NUMERIC(10,2),
	crrpct	BOOLEAN,
	clieNid VARCHAR(20),
	clieNbr VARCHAR(100),
	clieDir VARCHAR(200),
	clieTlf VARCHAR(50),
	clieRef VARCHAR(100),
	ventstd VARCHAR(1)
)
AS $$
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
		SELECT 	COALESCE(sclVent,'-1') AS sclVent, p.peddScl, p.peddLta, p.usuaCod, p.peddFec, p.peddTip, p.peddCcr, 
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
				COALESCE(venStd,'') AS ventStd
		FROM 	tbPeddDPedd p 
				LEFT JOIN
				tbPeddDAtcli c ON p.peddScl = c.peddScl
		WHERE p.peddScl = codigo;

END;
$$
LANGUAGE 'plpgsql';



DROP FUNCTION IF EXISTS fnVentas;
CREATE OR REPLACE FUNCTION fnVentas( IN accion VARCHAR, IN data VARCHAR, IN detalle VARCHAR, IN pagos VARCHAR)
RETURNS TABLE(
	codigo INT,
	descripcion TEXT
)
AS $$
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
	
	clienid 	VARCHAR(20);
	 
	subtotal 	NUMERIC(14,6);
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
					t.ventsep
					INTO 
					caja, usuario, clienid, tipo, carrier, mesa, letra, secPedd, subtotal, descuento, pdescuento, 
					iva, total, recibido, vuelto, observ, estado, facturado, separado
            FROM json_to_record(data::json) AS 
				  t(cajacod VARCHAR(4), empdnid VARCHAR(4), nidnid VARCHAR(20), venttip VARCHAR(1), ventccr VARCHAR(4),
					ventmsa VARCHAR(3), ventlta VARCHAR(1), peddscl VARCHAR(10), ventsbt NUMERIC(14,6),
					ventdsc NUMERIC(14,6), ventdpc INT, ventiva NUMERIC(14,6), venttot NUMERIC(14,6), 
					ventrcb NUMERIC(14,6), ventvto NUMERIC(14,6), ventobv TEXT, ventatv BOOLEAN, ventfac BOOLEAN,
				    ventsep VARCHAR(1));												

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
				 ventstd, ventsep)
			VALUES(secVent, caja, usuario, fecha, clienid, tipo, carrier, pctcarrier, valcarrier, mesa, letra, secPedd, 
				   CURRENT_TIME, subtotal, descuento, pdescuento, iva, total, recibido, vuelto, '', true, true, 
				   CASE WHEN recibido > 0 THEN true ELSE false END,
				   CASE WHEN recibido > 0 THEN 'P' ELSE 'F' END,
				   separado);

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
$$
LANGUAGE 'plpgsql';

-- Agregando DIDI Food 
INSERT INTO tbAdsiCrrie VALUES ('0008','DiDi','DID',10,true,true,'I');

--SELECT * FROM fnPedidos('','');