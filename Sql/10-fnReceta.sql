DROP FUNCTION IF EXISTS fnLstRcta;
CREATE OR REPLACE FUNCTION fnLstRcta( IN vnombre VARCHAR, IN vestado VARCHAR)
RETURNS TABLE(
	codigo VARCHAR(4),
	item text,
	estado text
)
AS $$
BEGIN
    RETURN QUERY 
	SELECT DISTINCT i.itemCod AS codigo, 
			UPPER(i.itemNbr) AS item, 
			CASE i.itemAtv 
				WHEN TRUE THEN 'ACTIVO'
				ELSE 'INACTIVO'				
			END AS estado 
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

DROP FUNCTION IF EXISTS fnReceta;
CREATE OR REPLACE FUNCTION fnReceta(IN accion VARCHAR, IN data VARCHAR, IN detalle VARCHAR)
RETURNS TABLE(
	codigo INT,
	descripcion TEXT
)
AS $$
DECLARE 
	itemScl VARCHAR(10);
	
BEGIN
    IF accion = 'N' THEN
		BEGIN
			RAISE NOTICE 'antes de ingresar los datos';
			RAISE NOTICE 'data = % ', data;
			
			IF NOT EXISTS(SELECT * FROM tbIvtrRcta WHERE itemCod = data) THEN
				INSERT INTO tbIvtrRcta 
				SELECT data, t.prdcod, t.rctacnt, t.rctaund, '', t.rctainv
				FROM json_to_recordset (detalle::json) AS
					t(prdcod VARCHAR(6), rctacnt NUMERIC(10,2), rctaund VARCHAR(1), rctainv BOOLEAN);
				RAISE NOTICE 'DESPUES DEL INSERT';
				RETURN QUERY
					SELECT 1 AS codigo, 'Receta creada correctamente' AS descripcion;			
			ELSE
				RETURN QUERY
					SELECT 0 AS codigo, 'La receta no fue creada por que ya existe' AS descripcion;
			END IF;					
		EXCEPTION WHEN OTHERS THEN
			RETURN QUERY
			SELECT 0 AS codigo, 'La receta no pudo ser creada' AS descripcion;
		END;
	END IF;
	
	IF accion = 'M' THEN
		BEGIN
			UPDATE tbIvtrRcta
			SET rctacnt= a.rctacnt, rctaund=a.rctaund, rctainv=a.rctainv
			FROM(
				SELECT data as codigo,t.prdcod, t.rctacnt, t.rctaund, '', t.rctainv
				FROM json_to_recordset (detalle::json) AS
					t(prdcod VARCHAR(6), rctacnt NUMERIC(10,2), rctaund VARCHAR(1), rctainv BOOLEAN)
				WHERE t.prdcod IN (SELECT prdcod FROM tbIvtrRcta WHERE itemCod=data)			
			) a
			WHERE tbIvtrRcta.itemcod=a.codigo AND tbIvtrRcta.prdcod=a.prdcod;
			
			INSERT INTO tbIvtrRcta 
			SELECT data as codigo,t.prdcod, t.rctacnt, t.rctaund, '', t.rctainv
			FROM json_to_recordset (detalle::json) AS
					t(prdcod VARCHAR(6), rctacnt NUMERIC(10,2), rctaund VARCHAR(1), rctainv BOOLEAN)
			WHERE t.prdcod NOT IN (SELECT prdcod FROM tbIvtrRcta WHERE itemCod=data);
			
			DELETE FROM tbIvtrRcta
			USING (
				SELECT itemCod, prdCod FROM tbIvtrRcta 
				WHERE itemCod=data AND prdCod NOT IN (
					SELECT t.prdcod 
					FROM json_to_recordset (detalle::json) AS
					t(prdcod VARCHAR(6), rctacnt NUMERIC(10,2), rctaund VARCHAR(1), rctainv BOOLEAN)
				)
			) a
			WHERE tbIvtrRcta.itemCod = a.itemCod AND tbIvtrRcta.prdCod = a.prdCod;
			
			RETURN QUERY
			SELECT 1 AS codigo, 'La receta fue modificada con éxito' AS descripcion;
		EXCEPTION WHEN OTHERS THEN
			RETURN QUERY
			SELECT 0 AS codigo, 'La receta no pudo ser modificada' AS descripcion;
		END;
	END IF;
END;
$$
LANGUAGE 'plpgsql';
