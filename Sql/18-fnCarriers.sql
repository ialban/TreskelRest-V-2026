DROP FUNCTION IF EXISTS fnLstCarriers;
CREATE OR REPLACE FUNCTION fnLstCarriers(IN nombre VARCHAR, IN estatus VARCHAR)
RETURNS TABLE(	
	codigo VARCHAR(4),
	carrier VARCHAR(100),
	icono VARCHAR(3),
	valor NUMERIC(6,2),
	porcentaje VARCHAR(2),
	estado VARCHAR(10)
)
AS $$
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
				END::varchar(10) AS estado
		FROM 	tbAdsiCrrie c
		WHERE	(nombre='' OR c.crriNbr LIKE '%'||nombre||'%') AND
				(estatus='' OR c.crriAtv=CASE estatus 
				 							WHEN 'A' THEN TRUE 
				 							WHEN 'I' THEN FALSE
				 							ELSE TRUE
										END)
				AND c.crriNid <> '0000';
				
END;
$$
LANGUAGE 'plpgsql';
/*
select * from fnLstCarriers('','')
*/