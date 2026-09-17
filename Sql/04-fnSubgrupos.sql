DROP FUNCTION IF EXISTS fnLstSgrp;
CREATE OR REPLACE FUNCTION fnLstSgrp( IN vnombre VARCHAR, IN vgrupo VARCHAR)
RETURNS TABLE(
	codigo VARCHAR(4),
	grupo text,
	subgrupo text
)
AS $$
BEGIN
    RETURN QUERY 
	SELECT a.sgrpCod AS codigo, 
			UPPER(b.grpNbr) AS grupo,
			UPPER(a.sgrpNbr) AS subgrupo
	FROM tbIvtrSgrp a
		INNER JOIN
		tbIvtrGrp b ON b.grpCod = a.grpCod
	WHERE 
		('' = vnombre OR a.sgrpNbr like vnombre) AND
		('' = vgrupo OR b.grpCod = vgrupo);
	
END;
$$
LANGUAGE 'plpgsql'
