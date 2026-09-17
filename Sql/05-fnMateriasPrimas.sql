DROP FUNCTION IF EXISTS fnLstPrd;
CREATE OR REPLACE FUNCTION fnLstPrd( IN vnombre VARCHAR, IN vunidad VARCHAR)
RETURNS TABLE(
	codigo VARCHAR(4),
	producto text,
	unidad text,
	estado text
)
AS $$
BEGIN
    RETURN QUERY 
	SELECT prdCod AS codigo, 
			UPPER(prdNbr) AS producto,
			CASE prdUnd 
			    WHEN 'U' THEN 'UNIDAD' 
				WHEN 'K' THEN 'KILO' 
				WHEN 'L' THEN 'LIBRA' 
				WHEN 'O' THEN 'ONZA' 
				WHEN 'G' THEN 'GRAMO' 
				WHEN 'P' THEN 'PORCIÓN' 
			END AS unidad,
			CASE prdAtv
				WHEN true THEN 'ACTIVO'
				ELSE 'INACTIVO'
			END AS estado
	FROM tbIvtrPrd 
	WHERE 
		('' = vnombre OR prdNbr like vnombre) AND
		('' = vunidad OR prdUnd = vunidad);
	
END;
$$
LANGUAGE 'plpgsql'
