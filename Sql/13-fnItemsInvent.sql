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