DROP FUNCTION IF EXISTS fnLstItem;
CREATE OR REPLACE FUNCTION fnLstItem( IN vnombre VARCHAR, IN vgrupo VARCHAR)
RETURNS TABLE(
	codigo 		VARCHAR(4),
	producto 	text,
	grupo 		text,	
	subgrupo 	text,	
	precio 		numeric(10,2),
	estado 		text
)
AS $$
BEGIN
    RETURN QUERY 
	SELECT i.itemCod, 
			UPPER(i.itemNbr) AS producto,
			UPPER(g.grpNbr) AS grupo,
			UPPER(s.sgrpNbr) AS subgrupo,
			i.itemPrc AS precio,
			i.itemCoc,
			CASE i.itemAtv WHEN TRUE THEN 'ACTIVO' ELSE 'INACTIVO' END AS estado
	FROM tbIvtrItem i
		INNER JOIN
		tbIvtrSgrp s ON i.sgrpCod = s.sgrpCod
		INNER JOIN
		tbIvtrGrp g ON g.grpCod = s.grpCod
	WHERE 
		('' = vnombre OR i.itemNbr like vnombre) AND
		('' = vgrupo OR g.grpCod = vgrupo);
	
END;
$$
LANGUAGE 'plpgsql';

DROP FUNCTION IF EXISTS fnLstRcta;
CREATE OR REPLACE FUNCTION fnLstRcta( IN vnombre VARCHAR, IN vestado VARCHAR)
RETURNS TABLE(
	codigo 	VARCHAR(4),
	item 	text,
	estado 	text
)
AS $$
BEGIN
    RETURN QUERY 
	SELECT i.itemCod AS codigo, i.itemNbr AS item, i.itemAtv AS estado 
	FROM tbIvtrItem i 
	    INNER JOIN 
	    tbIvtrRcta r ON r.itemCod = i.itemCod 
	WHERE 
		('' = vnombre OR i.itemNbr like vnombre) AND
		('' = vestado OR i.itemAtv = CASE vestado 
										WHEN 'A' THEN TRUE 
										ELSE FALSE 
									END 
		) AND
		i.itemAtv = true;
END;
$$
LANGUAGE 'plpgsql';

DROP FUNCTION IF EXISTS fnItemsToPedd;
CREATE OR REPLACE FUNCTION fnItemsToPedd(IN item VARCHAR, IN carrier VARCHAR)
RETURNS TABLE(
	codigo 		VARCHAR(4),
	producto 	VARCHAR(100),
	precio 		DECIMAL(10,6),
	iva 		DECIMAL(10,6),
	usaIva 		BOOLEAN,
	precioCarr	DECIMAL(10,6),
	carrPct		BOOLEAN,
	prdCod 		VARCHAR(6),
	itemCoc 	BOOLEAN
)
AS $$
DECLARE 
	prcCarr DECIMAL(10,6);
	ptjCarr BOOLEAN;
	aplCarr VARCHAR(1);
	
	prdNbr  VARCHAR(100);
	prdPrc  DECIMAL(10,6);
	prdCrr  DECIMAL(10,6);
	prdFin  DECIMAL(10,6);
	usaIva  BOOLEAN;
	prdCod  VARCHAR(6);
	prdCoc  BOOLEAN;
	
	valIva  DECIMAL(10,6);
	
BEGIN
	SELECT 	crriprc::DECIMAL(10,6), crripct, crriapi INTO prcCarr, ptjCarr, aplCarr FROM tbAdsiCrrie WHERE crriNid = carrier;
	
	SELECT 	(desencryptar(confval)::DECIMAL/100)::DECIMAL(10,6) 
			INTO valIva 
	FROM 	tbConfParam 
	WHERE 	confNid=2;
	
	SELECT 	i.itemNbr, 	i.itemPrc, 	i.itemIva, 	i.prdCod,  	i.itemCoc 
			INTO	
			prdNbr, 	prdPrc, 	usaIVA, 	prdCod, 	prdCoc
	FROM 	tbIvtrItem i 
	WHERE 	i.itemCod = item;	
	
	IF usaIVA = TRUE THEN
		prdPrc := prdPrc/(1 + valIva);
	END IF;
	
	IF aplCarr = 'I' THEN
		IF ptjCarr = TRUE THEN
			prdCrr := (PrdPrc + PrdPrc*(prcCarr/100));
		ELSE
			prdCrr := PrdPrc + prcCarr;
		END IF;
		
		PrdFin := prdCrr + prdCrr*valIva;
	
		RETURN QUERY
			SELECT 	item AS codigo, prdNbr AS producto, prdCrr AS precio, 
					PrdFin AS IVA, usaIva AS usaIVA, prcCarr, ptjCarr, prdCod, prdCoc;	
		/*
		* Se quitó esto para que realizar el cálculo de forma directa, y que incluya precios de carriers.
		*
		RETURN QUERY
			SELECT 	i.itemCod AS codigo, i.itemNbr AS producto, i.itemPrc AS precio, 
					i.itemPiv AS IVA, i.itemIva AS usaIVA, prcCarr::DECIMAL(10,6), ptjCarr, i.prdCod, i.itemCoc 
			FROM 	tbIvtrItem i 
			WHERE 	i.itemCod = item;*/
	ELSE
		/*
		* Se quitó esto para que realizar el cálculo de forma directa, y que incluya precios de carriers.
		*
		RETURN QUERY
			SELECT 	i.itemCod AS codigo, i.itemNbr AS producto, i.itemPrc AS precio, 
					i.itemPiv AS IVA, i.itemIva AS usaIVA, 0::DECIMAL(10,6), FALSE, i.prdCod, i.itemCoc 
			FROM 	tbIvtrItem i 
			WHERE 	i.itemCod = item;*/
		PrdFin := prdPrc + prdPrc*valIva;
			
		RETURN QUERY
			SELECT 	item AS codigo, prdNbr AS producto, prdPrc AS precio, 
					PrdFin AS IVA, usaIva AS usaIVA, 0, ptjCarr, prdCod, prdCoc;
	END IF;
	
END;
$$
LANGUAGE 'plpgsql';
/*
SELECT * FROM fnItemsToPedd('0001', '0003');
*/