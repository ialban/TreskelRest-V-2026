DROP FUNCTION IF EXISTS fnLstEmpd;
CREATE OR REPLACE FUNCTION fnLstEmpd( IN vapellido VARCHAR, IN vestado VARCHAR)
RETURNS TABLE(
	codigo VARCHAR(15),
	tipo text,
	empleado text,
	telefono text,
	estado text
)
AS $$
BEGIN
    RETURN QUERY 
	SELECT a.empdNid AS codigo, 
			(CASE a.empdTid WHEN 'C' THEN 'CÉDULA' ELSE 'PASAPORTE' END) AS tipo,
			(UPPER(a.empdApe) ||' '|| UPPER(a.empdNbr)) As nombre, 
			('CONV : '||a.empdTlf||' CEL : '||a.empdcel) AS telefono,
			(CASE a.empdAtv WHEN TRUE THEN 'ACTIVO' ELSE 'INACTIVO' END) AS estado
	FROM tbAdsiEmpd a
	WHERE 
		('' = UPPER(vapellido) OR UPPER(a.empdape) like UPPER(vapellido)) AND 
		('' = vestado OR a.empdAtv = CASE vestado WHEN 'A' THEN TRUE WHEN 'I' THEN FALSE END);
	
END;
$$
LANGUAGE 'plpgsql'
