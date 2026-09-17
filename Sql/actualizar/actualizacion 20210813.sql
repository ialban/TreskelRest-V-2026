
INSERT INTO tbAdsiMtvos VALUES('0600','Cliente decidió eliminar el pedido','A',true);
INSERT INTO tbAdsiMtvos VALUES('0601','Cliente nunca llegó a retirar el pedido','A',true);
INSERT INTO tbAdsiMtvos VALUES('0602','Dueño(a) decidió eliminar el pedido','A',true);

CREATE TABLE tbPeddHVent(
	ventScl VARCHAR(10)	NOT NULL,
	histTip VARCHAR(1) NOT NULL DEFAULT 'A' CHECK (histTip IN ('A','E')),
	ventFch TIMESTAMP WITHOUT TIME ZONE NOT NULL,
	usuaCod VARCHAR(4) NOT NULL,
	ventNFc VARCHAR(20) NOT NULL,
	ventObv VARCHAR(200) NOT NULL,
	CONSTRAINT fk_PeddDHVentTip FOREIGN KEY (ventScl) REFERENCES tbPeddDVent(ventScl)	
);

COMMENT ON COLUMN tbPeddHVent.histTip IS 'Tipo de Histórico: A = Anulación, E = Eliminación';

ALTER TABLE tbPeddDVent ADD ventStd VARCHAR(1) NOT NULL DEFAULT 'F' CHECK (ventStd IN ('F','P','A','E'));
COMMENT ON COLUMN tbPeddDVent.ventStd IS 'Estado del pedido facturado => F: FACTURADO, P: FACTURADO Y PAGADO, A: ANULADO, E: ELIMINADO ';

ALTER TABLE tbIvtrDInvMt ADD peddScl VARCHAR(10);
ALTER TABLE tbIvtrDInvMt ADD invtScl INT;
ALTER TABLE tbIvtrDInvMt ADD itemAtv BOOLEAN DEFAULT TRUE;


update tbIvtrDInvMt
set InvtScl = a.numfield
FROM (
select ctid, prdcod, dInvfch, row_number() over (order by dInvfch) as numfield from tbIvtrDInvMt
) a
where tbIvtrDInvMt.ctid = a.ctid;

ALTER TABLE tbIvtrDInvMt ALTER COLUMN invtScl SET NOT NULL;
ALTER TABLE tbIvtrDInvMt ADD CONSTRAINT pk_IvtrDInvMt PRIMARY KEY (invtScl);

update tbIvtrDInvMt
set peddScl = b.peddScl
FROM ( 
	select invtScl, a.peddScl
	from (
	SELECT invtScl, substring(dInvDsc,33,length(dInvDsc)) as peddScl from tbIvtrDInvMt where dInvDsc like 'Preparación%'
	) a
	where length(a.peddscl) =10
) b
where tbIvtrDInvMt.invtScl = b.invtScl ;


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
	sclVent VARCHAR(10);
	isFact  BOOLEAN;
	cliCod  VARCHAR(20);
	cliNbr  VARCHAR(100);
	venSbt  DECIMAL(10,2);
	venDsc 	DECIMAL(10,2);
	venIva	DECIMAL(10,2);
	venTot 	DECIMAL(10,2);
	venRcb 	DECIMAL(10,2);
	venVto	DECIMAL(10,2);
	venStd  VARCHAR(1);
BEGIN
	SELECT 	v.ventScl, c.nidNid, c.nidNbr, v.ventsbt, v.ventdsc, v.ventIva, 
			v.ventTot, v.ventRcb, v.ventvto, v.ventFac, v.ventStd
			INTO 
			sclVent, cliCod, cliNbr, venSbt, venDsc, venIva, 
			venTot, venRcb, venVto, isFact, venStd
	FROM 	tbPeddDVent v
			INNER JOIN
			tbAdsiNid c ON c.nidNid = v.nidnid
	WHERE
			v.peddScl=codigo;
		
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
		            t.ventdsc, t.ventdpc, t.ventiva, t.venttot, t.ventrcb, t.ventvto, t.ventobv, t.ventatv, t.ventfac
					INTO 
					caja, usuario, clienid, tipo, carrier, mesa, letra, secPedd, subtotal, descuento, pdescuento, 
					iva, total, recibido, vuelto, observ, estado, facturado 
            FROM json_to_record(data::json) AS 
				  t(cajacod VARCHAR(4), empdnid VARCHAR(4), nidnid VARCHAR(20), venttip VARCHAR(1), ventccr VARCHAR(4),
					ventmsa VARCHAR(3), ventlta VARCHAR(1), peddscl VARCHAR(10), ventsbt NUMERIC(14,6),
					ventdsc NUMERIC(14,6), ventdpc INT, ventiva NUMERIC(14,6), venttot NUMERIC(14,6), 
					ventrcb NUMERIC(14,6), ventvto NUMERIC(14,6), ventobv TEXT, ventatv BOOLEAN, ventfac BOOLEAN);												

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
				 ventstd)
			VALUES(secVent, caja, usuario, fecha, clienid, tipo, carrier, pctcarrier, valcarrier, mesa, letra, secPedd, 
				   CURRENT_TIME, subtotal, descuento, pdescuento, iva, total, recibido, vuelto, '', true, true, 
				   CASE WHEN recibido > 0 THEN true ELSE false END,
				   CASE WHEN recibido > 0 THEN 'P' ELSE 'F' END);

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
						ventPgd = FALSE,
						ventFac = TRUE,
						ventObv = '',
						ventStd = 'F',
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

DROP FUNCTION IF EXISTS fnItemsInvent;
CREATE OR REPLACE FUNCTION fnItemsInvent(IN peddScl VARCHAR, IN data VARCHAR, IN detalle VARCHAR)
RETURNS INT
AS $$
DECLARE 
	fecha DATE;	
	reg RECORD;	
	invtScl INT;
	
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
		
		SELECT MAX(COALESCE(invtScl,0))+1 INTO invtScl FROM tbIvtrDInvMT;

		INSERT INTO tbIvtrDInvMT
		(invtScl, prdcod, dInvDsc, dInvTip, dInvFch, dInvCnt, dInvUnd, peddScl)
		VALUES 
		(invtScl, reg.codigo, 'Preparación de alimentos pedido '||peddScl, 'E', current_date, reg.cantidad, reg.unidad, peddScl);
		
		call fnKardex(reg.codigo,EXTRACT(YEAR FROM fecha)::INT,EXTRACT(MONTH FROM fecha)::INT,0,reg.cantidad);
	END LOOP;
	
	FOR reg IN cItemsNoRcta LOOP
		RAISE NOTICE 'COD = %, CANT=%, UND=%', reg.codigo, reg.cantidad, reg.unidad;

		SELECT MAX(COALESCE(invtScl,0))+1 INTO invtScl FROM tbIvtrDInvMT;
		
		INSERT INTO tbIvtrDInvMT
		(invtScl, prdcod, dInvDsc, dInvTip, dInvFch, dInvCnt, dInvUnd, peddScl)
		VALUES 
		(invtScl, reg.codigo, 'Venta de Producto pedido '||peddScl, 'E', current_date, reg.cantidad, reg.unidad, peddScl);
		
		call fnKardex(reg.codigo,EXTRACT(YEAR FROM fecha)::INT,EXTRACT(MONTH FROM fecha)::INT,0,reg.cantidad);
	END LOOP;
	RETURN 1;
END;
$$
LANGUAGE 'plpgsql';

DROP FUNCTION IF EXISTS fnItemsInventRev;

CREATE OR REPLACE FUNCTION fnItemsInventRev(IN peddScl VARCHAR, IN data VARCHAR, IN detalle VARCHAR)
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
	
	FOR reg IN cItemsNoRcta LOOP
		RAISE NOTICE 'Anulando movimiento inventario producto %', reg.codigo;
		
		UPDATE 	tbIvtrDInvMt
		SET		itemAtv = false
		WHERE	prdCod=reg.codigo AND peddScl = peddScl;
		
		call fnKardex(reg.codigo,EXTRACT(YEAR FROM fecha)::INT,EXTRACT(MONTH FROM fecha)::INT,0, -reg.cantidad);
	END LOOP;
	
	FOR reg IN cItems LOOP
		RAISE NOTICE 'Anulando movimiento inventario producto %', reg.codigo;
		
		UPDATE 	tbIvtrDInvMt
		SET		itemAtv = false
		WHERE	prdCod=reg.codigo AND peddScl = peddScl;
		
		call fnKardex(reg.codigo,EXTRACT(YEAR FROM fecha)::INT,EXTRACT(MONTH FROM fecha)::INT,0, -reg.cantidad);
	END LOOP;
END;
$$
LANGUAGE 'plpgsql';


----------------------------------------------------------------------------------------

DROP FUNCTION IF EXISTS fnLstControl;
CREATE OR REPLACE FUNCTION fnLstControl(IN fecha VARCHAR)
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
BEGIN
	SELECT SUM(vaegval) INTO _egresos FROM tbCajaVaEg WHERE vaegfch=fecha::DATE;
	SELECT SUM(vaigval) INTO _ingresos FROM tbCajaVaIg WHERE vaigfch=fecha::DATE;
	SELECT SUM(venttot) INTO _ventas FROM tbPeddDVent WHERE ventfch=fecha::DATE AND ventStd IN ('F','P');
	SELECT SUM(comptot) INTO _compras FROM tbCompDComp WHERE compfch=fecha::DATE AND compAtv=true;
	SELECT ctrlSec, ctrlVin, ctrlVcr INTO _secuencial, _inicio, _cierre FROM tbCtrlCaja WHERE ctrlFch=fecha::DATE;
	
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
							COALESCE((SELECT ctrldif FROM tbCtrlCaja WHERE ctrlFch=dia AND cajaCod=c.cajaCod),0)::NUMERIC(10,2) diferencia ,
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

DROP FUNCTION public.fnrptventas(integer, integer, boolean);

CREATE OR REPLACE FUNCTION public.fnrptventas(
	anio integer,
	mes integer,
	detalle boolean)
    RETURNS TABLE(fecha date, tipo character varying, subtotal numeric, descuento numeric, iva numeric, total numeric, efectivo numeric, tarjeta numeric, cheque numeric, vuelto numeric) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
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
	
	IF detalle = FALSE then
		RETURN QUERY
			SELECT 	v.ventfch AS fecha,
					CASE v.venttip
						WHEN 'L' THEN 'LOCAL'
						WHEN 'D' THEN 'DOMICILIO'
						WHEN 'C' THEN 'LLEVAR'
						WHEN 'E' THEN 'CARRIER'
						WHEN 'R' THEN 'RETIRO'
					END::VARCHAR(20) AS tipo,
					SUM(v.ventsbt)::numeric(10,2) AS subtotal, 
					SUM(v.ventdsc)::numeric(10,2) AS descuento, 
					SUM(v.ventiva)::numeric(10,2) AS iva, 
					SUM(v.venttot)::numeric(10,2) AS total,
					0::numeric(10,2) AS efectivo,
					0::numeric(10,2) AS tarjeta,
					0::numeric(10,2) AS cheque,
					SUM(v.ventVto)::numeric(10,2) AS vuelto
			FROM 	tbPeddDVent v
					INNER JOIN
					tbCajaCaja c On c.cajacod=v.cajacod
			WHERE 	v.ventfac=TRUE AND 
					--v.ventpgd=TRUE AND 
					--v.ventatv=TRUE AND
					v.ventstd IN ('F','P') AND
					v.ventfch BETWEEN _inicio AND _fin
			GROUP BY v.ventfch, v.venttip
			ORDER BY v.ventfch;	
		ELSE
			RETURN QUERY			
			SELECT 	v.ventfch AS fecha,
					CASE venttip
						WHEN 'L' THEN 'LOCAL'
						WHEN 'D' THEN 'DOMICILIO'
						WHEN 'C' THEN 'LLEVAR'
						WHEN 'E' THEN 'CARRIER'
						WHEN 'R' THEN 'RETIRO'
					END::VARCHAR(20) AS tipo,
					SUM(v.ventsbt)::numeric(10,2) AS subtotal, 
					SUM(v.ventdsc)::numeric(10,2) AS descuento, 
					SUM(v.ventiva)::numeric(10,2) AS iva, 
					SUM(v.venttot)::numeric(10,2) AS total,
					COALESCE((SELECT SUM(f.fpgoVal) FROM tbPeddDFpgo f WHERE f.ventScl=v.ventScl AND f.fpgoTip='E'),0)::numeric(10,2)  as Efectivo,
					COALESCE((SELECT SUM(f.fpgoVal) FROM tbPeddDFpgo f WHERE f.ventScl=v.ventScl AND f.fpgoTip='T'),0)::numeric(10,2) as Tarjeta,
					COALESCE((SELECT SUM(f.fpgoVal) FROM tbPeddDFpgo f WHERE f.ventScl=v.ventScl AND f.fpgoTip='C'),0)::numeric(10,2) as Cheque,
					SUM(v.ventVto)::numeric(10,2) AS vuelto
			FROM	tbPeddDVent v
					inner join
					tbCajaCaja c On c.cajacod=v.cajacod		
			WHERE 	v.ventfac=true AND 
					--v.ventpgd=true AND 
					--v.ventatv=true AND 
					v.ventstd IN ('F','P') AND
					v.ventfch BETWEEN _inicio AND _fin
			GROUP BY v.ventfch, v.ventScl, v.venttip
			ORDER BY v.ventfch;
		END IF;
END;
$BODY$;

ALTER FUNCTION public.fnrptventas(integer, integer, boolean)
    OWNER TO postgres;

DROP FUNCTION fnRptFactAnuEli;

CREATE OR REPLACE FUNCTION fnRptFactAnuEli(in anio int , in mes int)
RETURNS TABLE(
	peddscl VARCHAR(10),
	cajanbr VARCHAR(20),
	usuanbr VARCHAR(50),
	ventfch VARCHAR(10),
	ventnfc VARCHAR(20),
	venttot DECIMAL(10,2),
	ventobv VARCHAR(200),
	ventstd VARCHAR(10)
)
AS $$
DECLARE

BEGIN
	RETURN QUERY
	SELECT 	v.peddScl,
			c.cajaNbr,
			u.usuaNbr,
			TO_CHAR(h.ventfch,'YYYY-MM-DD')::VARCHAR(10) AS ventfch,
			h.ventnfc,
			ROUND(v.venttot,2) AS venttot,
			h.ventobv,
			CASE 
				WHEN v.ventstd='A' THEN 'ANULADO'
				ELSE 'ELIMINADO'
			END::VARCHAR(10) AS ventstd
	FROM	tbPeddDVent v
			JOIN
			tbPeddHVent h ON h.ventscl = v.ventscl
			JOIN
			tbCajaCaja c ON c.cajacod = v.cajacod
			JOIN
			tbAdsiusua u ON u.usuacod = h.usuacod AND u.usuaatv = true
	WHERE 	v.ventStd IN ('A','E') AND 
			EXTRACT( YEAR FROM v.ventfch) = anio AND 
			EXTRACT(MONTH FROM v.ventfch) = mes;
END;
$$
LANGUAGE 'plpgsql';

------------------------------------------------------------------------------------------
DROP FUNCTION IF EXISTS fnAjusInv;
CREATE OR REPLACE FUNCTION fnAjusInv(IN codProd VARCHAR, IN undProd VARCHAR, IN cntProd DOUBLE PRECISION, IN tipo VARCHAR)
RETURNS TABLE(
	codigo INT,
	descripcion TEXT
)
AS $$
DECLARE
	fecha DATE := CURRENT_DATE;
	invscl INT;
BEGIN
	BEGIN	
		RAISE NOTICE 'ANTES DE INSERT';
		RAISE NOTICE 'FECHA = %', fecha;
		
		SELECT COALESCE(MAX(invtscl)+1,1) INTO invscl FROM tbIvtrDInvMT;
		
		RAISE NOTICE 'EL SECUENCIAL = %', invscl;
		
		INSERT INTO tbIvtrDInvMT
			(invtscl, prdcod, dinvdsc, dinvtip, dinvfch, dinvcnt, dinvund, itematv)
			VALUES 
			(invscl, codProd, 'Ajuste de inventario', tipo, fecha, cntProd::NUMERIC(10,2), undProd, true);
		
		RAISE NOTICE 'ANTES DE KARDEX';
		IF tipo = 'I' THEN
			RAISE NOTICE 'KARDEX INGRESO';
			call fnKardex(codProd,EXTRACT(YEAR FROM fecha)::int,EXTRACT(MONTH FROM fecha)::int,cntProd::NUMERIC(10,2),0);
		ELSE
			RAISE NOTICE 'KARDEX EGRESO';
			call fnKardex(codProd,EXTRACT(YEAR FROM fecha)::int,EXTRACT(MONTH FROM fecha)::int,0,cntProd::NUMERIC(10,2));
		END IF;
						
		RAISE NOTICE 'ANTES DE RETURN';
		RETURN QUERY
			SELECT 1 AS codigo, 'Ajuste de inventario realizado correctamente' AS descripcion;			
	EXCEPTION WHEN OTHERS THEN
		RETURN QUERY
			SELECT 0 AS codigo, 'El ajuste de inventario no pudo ser realizado' AS descripcion;
	END;
END;
$$
LANGUAGE 'plpgsql';

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
BEGIN
	SELECT SUM(vaegval) INTO _egresos FROM tbCajaVaEg WHERE vaegfch=fecha::DATE AND cajacod=caja;
	SELECT SUM(vaigval) INTO _ingresos FROM tbCajaVaIg WHERE vaigfch=fecha::DATE AND cajacod=caja;
	SELECT SUM(venttot) INTO _ventas FROM tbPeddDVent WHERE ventfch=fecha::DATE AND cajacod=caja AND ventStd IN ('F','P');
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
BEGIN
	IF tipo = 'I' THEN
		BEGIN		
			RAISE NOTICE 'CREANDO INICIO DE CAJA';
			RAISE NOTICE 'OBTENIENDO SECUENCIAL';
			SELECT COALESCE(MAX(ctrlSec::int)+1,1) INTO _secuencial FROM tbCtrlCaja;

			RAISE NOTICE 'SECUENCIAL = %', _secuencial;			
			
			IF NOT EXISTS(SELECT * FROM tbCtrlCaja WHERE ctrlFch = fecha::DATE  AND cajaCod=caja) THEN
				INSERT INTO tbCtrlCaja (ctrlSec, ctrlFch, cajaCod, ctrlHin, ctrlVin)
						VALUES (LPAD(_secuencial::TEXT,10,'0'), fecha::DATE, caja, hora::TIME, valor::NUMERIC(10,2));

				RAISE NOTICE 'INGRESO CREADO CORRECTAMENTE';
				
				RETURN QUERY
				SELECT 1 AS codigo, 'Inicio de caja registrado correctamente' AS descripcion;
			ELSE
				RAISE NOTICE 'YA HABIA UN INGRESO DE CAJA REGISTRADO PREVIAMENTE';
				
				RETURN QUERY
				SELECT 0 AS codigo, 'Ya existe un inicio de caja registrado para el '|| fecha AS descripcion;
			END IF;

			
		EXCEPTION WHEN OTHERS THEN
			RAISE NOTICE 'Codigo de error = %', SQLSTATE;
			RETURN QUERY
				SELECT 0 AS codigo, 'Inicio de caja no pudo ser registrado' AS descripcion;
		END;
	ELSE
		BEGIN
			RAISE  NOTICE 'iniciando cierre de caja';
			IF NOT EXISTS(SELECT * FROM tbCtrlCaja WHERE ctrlFch = fecha::DATE AND ctrlHfn IS NOT NULL AND cajaCod=caja) THEN
				UPDATE tbCtrlCaja 
					SET ctrlHfn=hora::TIME,
						ctrlVcr=valor::NUMERIC(10,2),
						ctrlVsi=sistema::NUMERIC(10,2),
						ctrlDif=diferencia::NUMERIC(10,2)
				WHERE	ctrlSec=secuencial AND ctrlFch=fecha::DATE;

				RAISE NOTICE 'Actualización realizada';
				RETURN QUERY
					SELECT 1 AS codigo, 'Cierre de caja registrado correctamente' AS descripcion;
			ELSE
				RAISE NOTICE 'CIERRE DE CAJA YA FUE REGISTRADO';
				RETURN QUERY
					SELECT 0 AS codigo, 'El cierre de caja correspondiente al '|| fecha ||' fue registrado previamente' AS descripcion;
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
----------------------------------------------------------------------
INSERT INTO tbAdsiMtvos VALUES('0600','Cliente decidió eliminar el pedido','R',true);
INSERT INTO tbAdsiMtvos VALUES('0601','Cliente nunca llegó a retirar el pedido','R',true);
INSERT INTO tbAdsiMtvos VALUES('0602','Dueño(a) decidió eliminar el pedido','R',true);

ALTER TABLE tbPeddDVent DROP CONSTRAINT tbpedddvent_ventstd_check;
ALTER TABLE tbPeddDVent ADD CONSTRAINT tbpeddvent_ventstd_check CHECK (ventstd IN ('F','P','A','E','S'));

ALTER TABLE tbPeddDVent ADD ventsep VARCHAR(1) NOT NULL DEFAULT 'C' CHECK (ventsep IN ('C','S'));
COMMENT ON COLUMN tbPeddDVent.ventSep IS 'C:COMPLETO, S: SEPARADO';

-- Tabla Separacion de Ventas(Facturas)
CREATE TABLE tbPeddSVent(
	ventscl VARCHAR(10),
	peddscl VARCHAR(10),
	ventstd VARCHAR(1) NOT NULL DEFAULT 'F' CHECK (ventStd IN ('F','P','A','E')),
	ventsbt NUMERIC(14,6),
	ventdsc NUMERIC(14,6),
	ventiva NUMERIC(14,6),
	venttot NUMERIC(14,6),
	ventrcb NUMERIC(14,6),
	ventvto NUMERIC(14,6),
	ventobv VARCHAR(200),
	CONSTRAINT fk_peddsvent_pedddvent FOREIGN KEY (ventscl) REFERENCES tbPedddVent(ventscl),
	CONSTRAINT fk_peddsvent_pedddpedd FOREIGN KEY (peddscl) REFERENCES tbPedddPedd(peddscl)
)

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

-- FUNCTION: public.fnventas(character varying, character varying, character varying, character varying)

-- DROP FUNCTION public.fnventas(character varying, character varying, character varying, character varying);

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
						ventPgd = FALSE,
						ventFac = TRUE,
						ventObv = '',
						ventStd = 'F',
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
$BODY$;

ALTER FUNCTION public.fnventas(character varying, character varying, character varying, character varying)
    OWNER TO postgres;

CREATE TABLE tbPeddHVent(
	ventScl VARCHAR(10)	NOT NULL,
	histTip VARCHAR(1) NOT NULL DEFAULT 'A' CHECK (histTip IN ('A','E')),
	ventFch TIMESTAMP WITHOUT TIME ZONE NOT NULL,
	usuaCod VARCHAR(4) NOT NULL,
	ventNFc VARCHAR(20) NOT NULL,
	ventObv VARCHAR(200) NOT NULL,
	CONSTRAINT fk_PeddDHVentTip FOREIGN KEY (ventScl) REFERENCES tbPeddDVent(ventScl)	
);

COMMENT ON COLUMN tbPeddHVent.histTip IS 'Tipo de Histórico: A = Anulación, E = Eliminación';

--------------------------------------------------------2021-10-29 ---------------------------------------------
DROP FUNCTION IF EXISTS fnLstNid;
CREATE OR REPLACE FUNCTION fnLstNid( IN vapellido VARCHAR, IN vtipo VARCHAR)
RETURNS TABLE(
	codigo VARCHAR(25),
	tipo text,
	cliente text,
	email VARCHAR(100),
	direccion text
)
AS $$
BEGIN
    RETURN QUERY 
	SELECT a.nidNid AS codigo, 
			(CASE a.nidTip WHEN 'C' THEN 'CÉDULA' WHEN 'P' THEN 'PASAPORTE' ELSE 'RUC' END) AS tipo,
			UPPER(a.nidNbr)  As cliente, 
			a.nidEml AS email,
			UPPER(a.nidDir) AS direccion
	FROM tbAdsiNid a
	WHERE 
		('' = vtipo OR a.nidTip = vtipo) AND
		(('' = vapellido OR UPPER(a.nidNbr) like UPPER(vapellido)) OR
		('' = vapellido OR a.nidnid LIKE UPPER(vapellido)));
	
END;
$$
LANGUAGE 'plpgsql'

-------------------------------------2021-11-09----------------------------------------------------------------------
CREATE TABLE tbHistCocina(
	peddscl VARCHAR(10),
	pedddet jsonb,
	usuacod VARCHAR(4),
	CONSTRAINT uk_peddscl_cocina UNIQUE(peddscl),
	CONSTRAINT fk_peddscl_pedddped FOREIGN KEY (peddscl) REFERENCES tbPeddDPedd(peddscl),
	CONSTRAINT fk_usacod_adsiusua FOREIGN KEY (usuacod) REFERENCES tbAdsiUsua(usuacod)
);

DROP FUNCTION IF EXISTS fnCocnaSav;
CREATE OR REPLACE FUNCTION fnCocnaSav(IN vPeddScl VARCHAR, IN data VARCHAR, IN vUsuaCod VARCHAR)
RETURNS BOOLEAN
AS $$
DECLARE
	response BOOLEAN := FALSE;
BEGIN
	IF NOT EXISTS(SELECT 1 FROM tbHistCocina h WHERE h.peddscl = vPeddScl) THEN
		RAISE NOTICE 'GRABANDO EL PEDIDO DE COCINA';
		
		INSERT INTO tbHistCocina
			(peddscl, pedddet, usuacod)
		VALUES
			(vPeddScl, data::JSONB, vUsuaCod);
			
		RAISE NOTICE 'PEDIDO CREADO CORRECTAMENTE';
		response := TRUE;
	ELSE
		RAISE NOTICE 'GRABANDO LA MODIFICACION DEL PEDIDO DE COCINA';
		
		UPDATE tbHistCocina
		SET pedddet= data::JSONB
		WHERE peddscl = vPeddScl;
		
		RAISE NOTICE 'PEDIDO ACTUALIZADO CORRECTAMENTE';
		response := TRUE;
	END IF;

    RETURN response;
END;
$$
LANGUAGE 'plpgsql';

DROP FUNCTION IF EXISTS fnLstCocna;
CREATE OR REPLACE FUNCTION fnLstCocna(IN vPeddScl VARCHAR)
RETURNS TEXT
AS $$
DECLARE
	response TEXT := '';
BEGIN
	SELECT h.pedddet INTO response FROM tbHistCocina h WHERE h.peddscl = vPeddScl;

    RETURN response;
END;
$$
LANGUAGE 'plpgsql';

----------------------------------------------------------------------------
--------------------------- 19/02/2022 -------------------------------------
INSERT INTO tbConfParam VALUES( 39, 'Modo Ppedido-Facturación', encryptar('F-P'),true); 
INSERT INTO tbConfParam VALUES( 40, 'Tipo botones para productos a usar', encryptar(''),true);
INSERT INTO tbConfParam VALUES( 41, 'Usa Ventas por carrier en Cierre de caja', encryptar('0'),true);

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
------------------------------------------------------------------------------ 25/02/2022  -- DESDE ACA PARA SAMBO.
alter table tbIvtrItem add itemDelCoc bool default false;

------------------------------------------------------------------------------ 23/04/2022
INSERT INTO tbConfParam VALUES (42, 'Cierre de caja Acumulativo', Encryptar('1'), true)

alter table tbCtrlCaja rename column ctrlFch to ctrlFchIni;
alter table tbCtrlCaja add column ctrlFchFin DATE;

CREATE TABLE tbCtrlCierre(
	ctrlSec VARCHAR(10) NOT NULL,
	ctrlFchIni DATE NOT NULL,
	ctrlFchFin DATE,
	ctrlHin TIME WITHOUT TIME ZONE,
	ctrlVin NUMERIC(10,2),
	ctrlHfn TIME WITHOUT TIME ZONE,
	ctrlVcr NUMERIC(10,2),
	ctrlVsi NUMERIC(10,2),
	ctrlDif NUMERIC(10,2),
	CONSTRAINT pk_ctrlCajaCierre PRIMARY KEY(ctrlSec),
	CONSTRAINT uk_ctrlCajaFechaIni UNIQUE (ctrlFchIni)
);

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
				ELSE
					RAISE NOTICE 'YA HABIA UN INGRESO DE CAJA REGISTRADO PREVIAMENTE';

					RETURN QUERY
					SELECT 0 AS codigo, 'Ya existe un inicio de caja registrado para el '|| fecha AS descripcion;
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

/*
MIGRACION DE DATOS PARA EL CONTROL DE CIERRE UNIFICADO
*/
INSERT INTO tbCtrlCierre
SELECT *, 
		(b.ctrlvcr - b.ctrlvsi) AS ctrlDif
FROM (
	SELECT 	fa.numero AS ctrlSec,
			fa.dia AS ctrlfchIni,
			fa.dia AS ctrlfchFin,
			c.ctrlHin AS ctrlhin,
			c.ctrlVin AS ctrlVin,
			c.ctrlHfn AS ctrlhfn,
			COALESCE(c.ctrlvcr,0) AS ctrlvcr,
			SUM(p.venttot) + c.ctrlVin AS ctrlvsi
	FROM 	(
				SELECT A.dia, LPAD(COALESCE(MAX(A.numero),0)::TEXT,10,'0') AS numero
				FROM (
					SELECT 	f.dia, row_number() over() AS numero
					FROM 	(SELECT * FROM fnLstFechas('2021-09-01') 
							 UNION 
							 SELECT * FROM fnLstFechas('2021-10-01')
							 UNION 
							 SELECT * FROM fnLstFechas('2021-11-01')
							 UNION 
							 SELECT * FROM fnLstFechas('2021-12-01')
							 UNION 
							 SELECT * FROM fnLstFechas('2022-01-01')
							 UNION 
							 SELECT * FROM fnLstFechas('2022-02-01')
							 UNION 
							 SELECT * FROM fnLstFechas('2022-03-01')
							 UNION 
							 SELECT * FROM fnLstFechas('2022-04-01')
							 UNION 
							 SELECT * FROM fnLstFechas('2022-05-01')
							 UNION
							 SELECT * FROM fnLstFechas('2022-06-01')
							 ORDER BY dia
							) f
					ORDER BY f.dia
				) A
				GROUP BY a.dia
			) fa
			JOIN
			tbPeddDVent p ON p.ventfch = fa.dia
			JOIN
			tbCtrlCaja c ON c.ctrlFchIni = fa.dia
	WHERE	ventstd IN ('F', 'P')
	GROUP BY fa.dia, fa.numero, c.ctrlHin, c.ctrlVin, c.ctrlHfn, c.ctrlvcr
	ORDER BY fa.numero
) b;
/*
*/


alter table tbCompDComp add column useCaja boolean;
update tbCompDComp set useCaja = false;

DROP FUNCTION fncompras(character varying, character varying, character varying, character varying);
CREATE OR REPLACE FUNCTION fncompras(
	accion character varying,
	usaCajaChica boolean,
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
	
	compfac VARCHAR(10);
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

-- Falta corregir
CREATE OR REPLACE FUNCTION fnlstcontrol(
	fecha character varying,
	caja character varying)
    RETURNS TABLE(secuencial character varying, ingresos numeric, egresos numeric, compras numeric, ventas numeric, inicio numeric, cierre numeric) 
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
BEGIN
	SELECT desencryptar(confval)::boolean INTO _incVentCarrier FROM tbConfParam WHERE confnid=41;
	SELECT desencryptar(confval)::boolean INTO _acumulativo FROM tbConfParam WHERE confnid=42;
	
	IF _acumulativo = true THEN
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

		SELECT SUM(comptot) INTO _compras FROM tbCompDComp WHERE compfch=fecha::DATE AND compAtv=true;
		SELECT ctrlSec, ctrlVin, ctrlVcr INTO _secuencial, _inicio, _cierre FROM tbCtrlCierre WHERE ctrlFchIni=fecha::DATE;
	ELSE
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
	END IF;
	
	RETURN QUERY
		SELECT 	COALESCE(_secuencial, '') AS secuencial,
				COALESCE(_ingresos,0) AS ingresos, 
				COALESCE(_egresos,0) AS egresos, 
				COALESCE(_compras,0) AS compras, 
				COALESCE(_ventas,0) AS ventas,
				COALESCE(_inicio,0) AS inicio,
				COALESCE(_cierre,0) AS cierre;
END;
$BODY$;

CREATE OR REPLACE FUNCTION fnrptcontrolcaja(
	anio integer,
	mes integer,
	detalle boolean)
    RETURNS TABLE(fecha date, 
				  caja character varying, 
				  inicio numeric, 
				  ingreso numeric, 
				  egreso numeric, 
				  cierre numeric, 
				  sistema numeric, 
				  diferencia numeric) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
	_inicio 	DATE;
	_fin 		DATE;
	_CierrAcum	BOOLEAN;
BEGIN
	select desencryptar(confval)::BOOLEAN INTO _CierrAcum from tbConfParam where confNid = 42;

	_inicio :=  (anio ||'-'|| lpad(mes::text,2,'0') || '-01')::DATE;
	IF mes + 1 > 12 THEN
		anio := anio + 1;
		mes := 1;
	END IF;
	_fin 	:=  (anio ||'-'|| lpad((mes+1)::text,2,'0') || '-01')::DATE;

	IF detalle = false THEN
		IF _CierrAcum = TRUE THEN
			RETURN QUERY
			SELECT 	d.dia as fecha, 'Todas'::varchar AS caja, 	
					COALESCE(ctrlvin, 0)::NUMERIC(10,2) AS inicio, 
					0::NUMERIC(10,2) AS ingreso, 
					0::NUMERIC(10,2) AS egreso, 
					COALESCE(ctrlvcr, 0)::NUMERIC(10,2) AS cierre, 
					COALESCE(ctrlvsi, 0)::NUMERIC(10,2) AS sistema, 
					COALESCE(ctrldif, 0)::NUMERIC(10,2) diferencia 		
			FROM	fnLstFechas(_inicio::DATE) d
					LEFT JOIN
					tbCtrlCierre c ON c.ctrlFchIni = d.dia
			ORDER BY d.dia;
		ELSE
			RETURN QUERY	
			SELECT d.dia as fecha, d.cajaNbr AS caja, 	
					COALESCE(ctrlvin, 0)::NUMERIC(10,2) AS inicio, 
					0::NUMERIC(10,2) AS ingreso, 
					0::NUMERIC(10,2) AS egreso, 
					COALESCE(ctrlvcr, 0)::NUMERIC(10,2) AS cierre, 
					COALESCE(ctrlvsi, 0)::NUMERIC(10,2) AS sistema, 
					COALESCE(ctrldif, 0)::NUMERIC(10,2) diferencia 		
			FROM	(SELECT * 
					 FROM 	fnLstFechas(_inicio::DATE) d,
							tbCajaCaja cj
					) d
					LEFT JOIN
					tbCtrlCaja c ON c.ctrlFchIni = d.dia AND c.cajacod = d.cajacod
			ORDER BY d.cajaNbr, d.dia;	
		END IF;	
	ELSE
		IF _CierrAcum = TRUE THEN
			RETURN QUERY
			SELECT 	a.fecha, 'Todas'::varchar AS caja, a.inicio, a.ingreso, a.egreso, 
					a.cierre,a.sistema, a.diferencia
			FROM (
				SELECT d.dia as fecha, 
						COALESCE(c.ctrlvin, 0)::NUMERIC(10,2) AS inicio, 
						COALESCE(SUM(vi.vaigval),0)::NUMERIC(10,2) AS ingreso, 
						COALESCE(SUM(ve.vaegval),0)::NUMERIC(10,2) AS egreso, 
						COALESCE(c.ctrlvcr, 0)::NUMERIC(10,2) AS cierre, 
						COALESCE(c.ctrlvsi, 0)::NUMERIC(10,2) AS sistema, 
						COALESCE(c.ctrldif, 0)::NUMERIC(10,2) diferencia 		
				FROM	(SELECT * 
						 FROM 	fnLstFechas(_inicio::DATE) d,
								tbCajaCaja cj
						) d
						LEFT JOIN
						tbCtrlCierre c ON c.ctrlFchIni = d.dia 
						LEFT JOIN
						tbCajaVaIg vi ON vi.vaigFch = d.dia
						LEFT JOIN
						tbCajaVaEg ve ON ve.vaegFch = d.dia
				GROUP BY d.dia, c.ctrlvin, c.ctrlvcr, c.ctrlvsi, c.ctrldif
				ORDER BY d.dia
				) a;
		ELSE
			RETURN QUERY
			SELECT a.fecha, a.caja, a.inicio, a.ingreso, a.egreso, 
					a.cierre,a.sistema, 
					(a.cierre - sistema)::NUMERIC(10,2) AS diferencia
			FROM (
				SELECT d.dia as fecha, d.cajaNbr AS caja, 	
						COALESCE(c.ctrlvin, 0)::NUMERIC(10,2) AS inicio, 
						COALESCE(SUM(vi.vaigval),0)::NUMERIC(10,2) AS ingreso, 
						COALESCE(SUM(ve.vaegval),0)::NUMERIC(10,2) AS egreso, 
						COALESCE(c.ctrlvcr, 0)::NUMERIC(10,2) AS cierre, 
						COALESCE(c.ctrlvsi, 0)::NUMERIC(10,2) AS sistema, 
						COALESCE(c.ctrldif, 0)::NUMERIC(10,2) diferencia 		
				FROM	(SELECT * 
						 FROM 	fnLstFechas(_inicio::DATE) d,
								tbCajaCaja cj
						) d
						LEFT JOIN
						tbCtrlCaja c ON c.ctrlFchIni = d.dia AND c.cajacod = d.cajacod
						LEFT JOIN
						tbCajaVaIg vi ON vi.cajacod = c.cajacod AND vi.vaigFch = d.dia
						LEFT JOIN
						tbCajaVaEg ve ON ve.cajacod = c.cajacod AND ve.vaegFch = d.dia
				GROUP BY d.cajaNbr, d.dia, c.ctrlvin, c.ctrlvcr, c.ctrlvsi, c.ctrldif
				ORDER BY d.cajaNbr, d.dia
				) a;
		END IF;
	END IF;
END;
$BODY$;

DROP FUNCTION fnrptcontrolcajadiario(integer, integer, integer);
CREATE OR REPLACE FUNCTION fnrptcontrolcajadiario(
	anio integer,
	mes integer,
	dia integer)
    RETURNS TABLE(fecha date, 
				  caja character varying, 
				  resumen character varying,
				  inicio numeric, 
				  ingreso numeric, 
				  egreso numeric, 
				  cierre numeric, 
				  sistema numeric, 
				  diferencia numeric) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
	_inicio 	DATE;
	_fecha 		DATE;
	_CierrAcum	BOOLEAN;
BEGIN
	select desencryptar(confval)::BOOLEAN INTO _CierrAcum from tbConfParam where confNid = 42;
	
	_inicio :=  (anio ||'-'|| lpad(mes::text,2,'0')   || '-01')::DATE;
	_fecha  :=  (anio ||'-'|| lpad(mes::text,2,'0')   || '-' || lpad(dia::text,2,'0') )::DATE;
	
	IF _CierrAcum = true THEN
		RETURN QUERY
		SELECT b.fecha, b.caja, b.resumen::VARCHAR, b.inicio, b.ingreso, b.egreso, b.cierre, b.sistema, b.diferencia
		FROM (
				SELECT	* 
				FROM (
						SELECT 	d.dia AS fecha,'Todas'::VARCHAR AS caja, 'RESUMEN CAJA' AS resumen,
								COALESCE(SUM(ctrlvin), 0)::NUMERIC(10,2) AS inicio, 
								0::NUMERIC(10,2) AS ingreso, 
								0::NUMERIC(10,2) AS egreso, 
								COALESCE(SUM(ctrlvcr), 0)::NUMERIC(10,2) AS cierre, 
								COALESCE(SUM(ctrlvsi), 0)::NUMERIC(10,2) AS sistema, 
								COALESCE(SUM(ctrldif), 0)::NUMERIC(10,2) AS diferencia ,
								1 AS orden
						FROM	fnLstFechas(_inicio::DATE) d
								LEFT JOIN
								tbCtrlCierre c ON c.ctrlFchIni = d.dia
						WHERE 	d.dia = _fecha
						GROUP BY d.dia
						ORDER BY d.dia
			) a 
			UNION
			SELECT 	v.ventfch AS fecha, 'Todas'::VARCHAR AS caja, 
					'VENTAS' AS resumen, 0 AS inicio, 
					ROUND(SUM(v.venttot),2) AS ingreso, 
					0 AS egreso, 0 AS cierre, 0 AS sistema, 0 AS diferencia, 2 AS orden
			FROM 	tbPeddDVent v
					JOIN
					tbPeddDFpgo p ON p.ventscl = v.ventscl
			WHERE 	v.ventStd IN ('F','P') AND
					v.ventfch = _fecha
			GROUP BY v.ventfch
			UNION
			SELECT 	co.compfch AS fecha, 'Todas'::VARCHAR AS caja,  
					'COMPRAS' AS resumen, 0 AS inicio, 
					0 AS ingreso, ROUND(SUM(co.comptot),2) AS egreso, 
					0 AS cierre, 0 AS sistema, 0 AS diferencia, 3 AS orden
			FROM 	tbCompDComp co
					join
					tbCompFpgo p ON co.compNid = p.compNid
			WHERE	co.compatv = true AND
					co.compfch = _fecha AND
					co.useCaja = true
			GROUP BY co.compfch
			UNION
			SELECT 	v.vaigfch AS fecha, 'Todas'::VARCHAR AS caja, 
					'VALES INGRESO' AS resumen, 0 AS inicio, 
					ROUND(SUM(v.vaigval),2) AS ingreso, 
					0 AS egreso,0 AS cierre, 0 AS sistema, 0 AS diferencia, 4 AS orden
			FROM	tbcajavaig v
			WHERE	v.vaigfch = _fecha
			GROUP BY v.vaigfch
			UNION
			SELECT 	v.vaegfch AS fecha, 'Todas'::VARCHAR AS caja, 
					'VALES EGRESO' AS resumen, 0 AS inicio, 
					0 AS ingreso, ROUND(SUM(v.vaegval),2) AS egreso,
					0 AS cierre, 0 AS sistema, 0 AS diferencia, 5 AS orden
			FROM	tbcajavaeg v
			WHERE	v.vaegfch = _fecha
			GROUP BY v.vaegfch
		) B
		ORDER BY B.orden;
	ELSE
		RETURN QUERY
		SELECT fecha, caja, resumen, inicio, ingreso, egreso, cierre, sistema, diferencia
		FROM (
				SELECT	* 
				FROM (
						SELECT 	d.dia as fecha, c.cajaNbr AS caja, 'RESUMEN CAJA'	AS resumen,
								COALESCE((SELECT ctrlvin FROM tbCtrlCaja WHERE ctrlFchIni=dia AND cajaCod=c.cajaCod),0)::NUMERIC(10,2) AS inicio, 
								0::NUMERIC(10,2) AS ingreso, 
								0::NUMERIC(10,2) AS egreso, 
								COALESCE((SELECT ctrlvcr FROM tbCtrlCaja WHERE ctrlFchIni=dia AND cajaCod=c.cajaCod),0)::NUMERIC(10,2) AS cierre, 
								COALESCE((SELECT ctrlvsi FROM tbCtrlCaja WHERE ctrlFchIni=dia AND cajaCod=c.cajaCod),0)::NUMERIC(10,2) AS sistema, 
								COALESCE((SELECT ctrldif FROM tbCtrlCaja WHERE ctrlFchIni=dia AND cajaCod=c.cajaCod),0)::NUMERIC(10,2) diferencia ,
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
			WHERE 	v.ventStd IN ('F','P') AND
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
			WHERE	co.compatv = true AND
					co.compfch = _fecha AND
					co.useCaja = true
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
	END IF;
END;
$BODY$;

------------------------------------------------------------------- 2022-06-05
ALTER TABLE tbloggerALTER COLUMN logghra TYPE time without time zone ;

-- FUNCTION: fnlogger(character varying, character varying, character varying, character varying)

-- DROP FUNCTION fnlogger(character varying, character varying, character varying, character varying);

CREATE OR REPLACE FUNCTION fnlogger(
	tipo character varying,
	descrip character varying,
	usuario character varying,
	dispositivo character varying)
    RETURNS TABLE(codigo integer, descripcion text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
	loggScl INT;
BEGIN
	loggScl := 0;
	
	RAISE NOTICE 'Iniciando registro de actividad en el logger';
	BEGIN	
		RAISE NOTICE 'Obteniendo los datos ';
		
		SELECT COALESCE(MAX(loggNid) + 1,1) INTO loggScl FROM tbLogger; 
		RAISE NOTICE 'Obteniendo secuencial';
		
		INSERT INTO tbLogger 
		(loggNid, loggTip, loggDsc, loggFch, loggHra, loggUsr, loggOrg)
			VALUES 
		(loggScl, tipo, descrip, CURRENT_DATE, LOCALTIME, usuario, dispositivo);

		--COMMIT;			

		RAISE NOTICE 'Finalizó registro de actividad en el logger';
		RETURN QUERY
			SELECT 1 AS codigo, 'Actividad registrada correctamente ' AS descripcion;			
	EXCEPTION WHEN OTHERS THEN
		RAISE NOTICE 'HUBO un error y realizará un rollback';
		RAISE NOTICE 'Codio de error = %', SQLSTATE;
		--ROLLBACK;

		SELECT 0 AS codigo, 'Actividad no pudo ser registrada en el logger' AS descripcion;
	END;
	
END;
$BODY$;

ALTER FUNCTION public.fnlogger(character varying, character varying, character varying, character varying)
    OWNER TO postgres;

INSERT INTO tbConfParam values (43, 'Ejecutar Backup', encryptar('0'), true);
INSERT INTO tbConfParam values (44, 'Ubicación Pg_dump', encryptar(''), true);

CREATE OR REPLACE FUNCTION fnSetPgDumpLocation()
	RETURNS TABLE(descripcion text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
	_ubicacion VARCHAR(300)='';
	_actual VARCHAR(300)='';
BEGIN
	
	RAISE NOTICE 'Iniciando registro de ubicacion pg_dump';
	BEGIN	
		SELECT 	substring(setting,0,position('data' in setting))
				INTO _ubicacion
		FROM 	pg_settings
		WHERE name = 'data_directory';
		
		_ubicacion := _ubicacion || 'bin';
		
		SELECT desencryptar(confval) INTO _actual FROM tbConfParam WHERE confNid = 44;
		
		IF _ubicacion != _actual THEN
			RAISE NOTICE 'Actualizando ubicacion del pg_dump';
			
			UPDATE 	tbConfParam
			SET		confVal = encryptar(_ubicacion::text)
			WHERE	confNid = 44;
			
			RAISE NOTICE 'Ubicacion del pg_dump actualizada';
		END IF;

		SELECT desencryptar(confval) INTO _ubicacion FROM tbConfParam WHERE confNid = 44;
		_ubicacion = replace(_ubicacion,'/','\\');
		_ubicacion = replace(_ubicacion,'Program Files','\"Program Files\"');
		
		RAISE NOTICE 'Finalizó ubicacion del pg_dump';
		
		RETURN QUERY
			SELECT _ubicacion::text AS descripcion;	
	EXCEPTION WHEN OTHERS THEN
		RAISE NOTICE 'HUBO un error y realizará un rollback';
		RAISE NOTICE 'Codio de error = %', SQLSTATE;
		--ROLLBACK;

		RETURN QUERY
			SELECT 'No se pudo encontrar el pg_dump' AS descripcion;
	END;
	
END;
$BODY$;

-- PARA CREACION DE CARRIERS
ALTER TABLE tbAdsiCrrie ADD COLUMN crriImg BYTEA DEFAULT NULL;

DROP FUNCTION public.fnlstcarriers(character varying, character varying);

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

CREATE TABLE tbBkpRqsts(
	bkpNid INT NOT NULL,
	cajacod VARCHAR(4),
	bkpAtv BOOLEAN,
	bkpCmt VARCHAR(200),
	bkpCAc SMALLINT DEFAULT 3,
	bkpCPg SMALLINT DEFAULT 0,
	CONSTRAINT pkBkpRequest PRIMARY KEY (bkpNid)
);

COMMENT ON COLUMN tbBkpRqsts.bkpAtv IS 'FALSE: BACKUP iniciado pero sin finalizar, TRUE: backup finalizado';


---------------------------------------------------------------------- 2022-07-02
INSERT INTO tbConfParam VALUES (40, 'Tipo botones para productos a usar',encryptar('ST'),true);
--UPDATE tbConfParam set confVal = encryptar('ST') where confNid = 40
INSERT INTO tbConfParam VALUES (45, 'Organización de items en pedidos',encryptar('H'),true);



