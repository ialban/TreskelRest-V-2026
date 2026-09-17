CREATE OR REPLACE FUNCTION fnGrabaRetenciones(IN accion VARCHAR(1), 
											IN secuencia VARCHAR(3),
											IN codsri VARCHAR(10),
											IN nombre VARCHAR(200),
											IN valor NUMERIC(6,2),
											IN tipo VARCHAR(2),
											IN activo BOOLEAN)
RETURNS TABLE(codigo integer, descripcion text) 
AS $$
DECLARE
	sclRete VARCHAR(3);
BEGIN
	IF accion = 'N' THEN
		SELECT LPAD(COALESCE(MAX(retescl::int)+1,1)::TEXT,3,'0') INTO sclRete FROM tbPctRete;
		
		INSERT INTO tbPctRete VALUES (sclRete, codsri, nombre, valor, tipo, activo);
		RETURN QUERY
			SELECT 1 AS codigo, 'Retención creada correctamente con el secuencial '|| sclRete::VARCHAR AS descripcion;	
	ELSE
		UPDATE 	tbPctRete
		SET		reteSri = codsri,
				reteDsc = nombre,
				reteVal = valor,
				reteApl = tipo,
				reteAct = activo
		WHERE	retescl = secuencia;
		
		RETURN QUERY
			SELECT 1 AS codigo, 'Retención actualizada correctamente con el secuencial '|| secuencia::VARCHAR AS descripcion;	
	END IF;
	
END
$$
LANGUAGE 'plpgsql';