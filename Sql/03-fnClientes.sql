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
