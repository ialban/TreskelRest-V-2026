DROP FUNCTION IF EXISTS fnLstProv;
CREATE OR REPLACE FUNCTION fnLstProv( IN vnombre VARCHAR, IN vestado VARCHAR)
RETURNS TABLE(
	codigo VARCHAR(25),
	proveedor text,
	email VARCHAR(100),
	telefono text,
	estado text
)
AS $$
BEGIN
    RETURN QUERY 
	SELECT provNid AS codigo, 
			UPPER(provNbr) AS producto,
			provEml AS email,
			UPPER(provTlf) AS telefono,
			CASE provAtv
			    WHEN true THEN 'ACTIVO'
				ELSE 'INACTIVO'
			END AS estado
	FROM tbIvtrProv
	WHERE 
		('' = vnombre OR provNbr like vnombre) AND
		('' = vestado OR provAtv = CASE vestado WHEN 'A' THEN TRUE ELSE FALSE END);
	
END;
$$
LANGUAGE 'plpgsql'
