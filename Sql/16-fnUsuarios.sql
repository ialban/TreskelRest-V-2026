DROP FUNCTION IF EXISTS fnLogin;
CREATE OR REPLACE FUNCTION fnLogin(IN usuario VARCHAR, IN clave VARCHAR)
RETURNS TABLE(
	codigo VARCHAR(4),
	username VARCHAR(20)
)
AS $$
DECLARE
	_usaClvNum BOOLEAN;
BEGIN
	_usaClvNum := FALSE;
	
	IF usuario = 'admin' THEN
		RETURN QUERY
				SELECT 	usuaCod AS codigo, usuaUsr AS username
				FROM 	tbadsiusua 
				WHERE 	usuausr=usuario AND desencryptar(usuaclv)=clave;
	ELSE
		
		SELECT CASE desencryptar(confval) 
					WHEN '1' THEN TRUE 
					ELSE FALSE 
				END INTO _usaClvNum
		FROM 	tbConfParam 
		WHERE 	confNid=20;

		IF _usaClvNum = TRUE THEN
			RETURN QUERY
				SELECT 	usuaCod AS codigo, usuaUsr AS username
				FROM 	tbadsiusua 
				WHERE 	usuausr=usuario AND desencryptar(usuaCnm)=clave;
		ELSE
			RETURN QUERY
				SELECT 	usuaCod AS codigo, usuaUsr AS username
				FROM 	tbadsiusua 
				WHERE 	usuausr=usuario AND desencryptar(usuaclv)=clave;
		END IF;
	END IF;	
END;
$$
LANGUAGE 'plpgsql';

DROP FUNCTION IF EXISTS fnLstUsuarios;
CREATE OR REPLACE FUNCTION fnLstUsuarios(IN nombre VARCHAR, IN estatus VARCHAR)
RETURNS TABLE(	
	codigo VARCHAR(4),
	usuario VARCHAR(20),
	empleado VARCHAR(100),
	tipo VARCHAR(20),
	estado VARCHAR(10)
)
AS $$
DECLARE
	
BEGIN	
	RETURN QUERY
		SELECT u.usuaCod AS Codigo, u.usuaUsr AS usuario,
				(e.empdApe||' '||e.empdNbr)::varchar(100) AS empleado,
				CASE usuaTip
					WHEN 'M' THEN 'Mesero'
					WHEN 'A' THEN 'Administrador'
					WHEN 'C' THEN 'Cajero'
					WHEN 'S' THEN 'Supervisor'			
				END::varchar(20) AS tipo,
				CASE usuaAtv
					WHEN TRUE THEN 'Activo'
					WHEN FALSE THEN 'Inactivo'
				END::varchar(10) AS estado
		FROM 	tbAdsiUsua u
				INNER JOIN
				tbAdsiEmpd e ON u.empdNid = e.empdNid
		WHERE	(nombre='' OR (e.empdApe||' '||e.empdNbr) LIKE '%'||nombre||'%') AND
				(estatus='' OR u.usuaAtv=CASE estatus 
				 							WHEN 'A' THEN TRUE 
				 							WHEN 'I' THEN FALSE
				 							ELSE TRUE
										END);
END;
$$
LANGUAGE 'plpgsql';

DROP FUNCTION IF EXISTS fnUsuarios;
CREATE OR REPLACE FUNCTION fnUsuarios(IN accion VARCHAR, IN secuencial VARCHAR, IN creador VARCHAR, IN usuario VARCHAR,
										 IN clave VARCHAR, IN detalle VARCHAR, 
									  	 IN tipo VARCHAR, IN empleado VARCHAR, IN estado VARCHAR,
									 	 IN permisos VARCHAR)
RETURNS TABLE(
	codigo INT,
	descripcion TEXT
)
AS $$
DECLARE
	_secuencial INT;	
	_usaClvNum BOOLEAN;
BEGIN
	_usaClvNum := FALSE;
	
	SELECT CASE desencryptar(confval) 
				WHEN '1' THEN TRUE 
				ELSE FALSE 
			END INTO _usaClvNum
	FROM 	tbConfParam 
	WHERE 	confNid=20;
	
	IF accion = 'N' THEN
		IF NOT EXISTS(SELECT usuaUsr FROM tbAdsiUsua WHERE usuaUsr=usuario) THEN
			BEGIN
				RAISE NOTICE 'CREANDO USUARIO';
				RAISE NOTICE 'OBTENIENDO SECUENCIAL';
				SELECT COALESCE(MAX(usuaCod::int)+1,1) INTO _secuencial FROM tbAdsiUsua;
				
				RAISE NOTICE 'SECUENCIAL = %', _secuencial;			
				
				IF _usaClvNum = FALSE THEN
					RAISE NOTICE 'INGRESANDO CON CLAVE NO NUMERICA';
					INSERT INTO tbAdsiUsua 
							(usuaCod, usuaNbr, usuaUsr, usuaClv, usuaTip, empdNid, usuaCad, usuaAtv)
							VALUES (LPAD(_secuencial::TEXT,4,'0'), 
									detalle, usuario, encryptar(clave), 
									tipo, empleado, encryptar(creador),
									CASE estado WHEN 'A' THEN TRUE ELSE FALSE END);
				ELSE
					RAISE NOTICE 'INGRESANDO CON CLAVE NUMERICA';
					INSERT INTO tbAdsiUsua 
							(usuaCod, usuaNbr, usuaUsr, usuaClv, usuaCnm, usuaTip, empdNid, usuaCad, usuaAtv)
							VALUES (LPAD(_secuencial::TEXT,4,'0'), 
									detalle, usuario, encryptar(''),encryptar(clave), 
									tipo, empleado, encryptar(creador),
									CASE estado WHEN 'A' THEN TRUE ELSE FALSE END);
				END IF;
								
				RAISE NOTICE 'INGRESANDO LOS PERMISOS DEL USUARIO';
				INSERT INTO tbAdsiUsPer
					SELECT LPAD(_secuencial::TEXT,4,'0'), t.codigo, t.estado
					FROM JSON_TO_RECORDSET(permisos::json) AS
							t(codigo VARCHAR(4), estado BOOLEAN);
								
				RAISE NOTICE 'INGRESO CREADO CORRECTAMENTE';

				RETURN QUERY
					SELECT 1 AS codigo, 'El usuario fue registrado correctamente' AS descripcion;
			EXCEPTION WHEN OTHERS THEN
				RAISE NOTICE 'Codigo de error = %', SQLSTATE;
				RETURN QUERY
					SELECT 0 AS codigo, 'El usuario no pudo ser registrado correctamente' AS descripcion;
			END;
		END IF;
	ELSE
		BEGIN
			RAISE  NOTICE 'Modificando un usuario';
			IF _usaClvNum = FALSE THEN
				RAISE NOTICE 'ACTUALIZANDO CON CLAVE NO NUMERICA';
				UPDATE tbAdsiUsua
					SET usuaClv=encryptar(clave),
						usuaNbr=detalle,
						usuaTip=tipo,
						empdNid=empleado,
						usuaAtv=CASE estado WHEN 'A' THEN TRUE ELSE FALSE END
				WHERE	usuaCod=secuencial;
			ELSE
				RAISE NOTICE 'ACTUALIZANDO CON CLAVE NUMERICA';
				UPDATE tbAdsiUsua
				SET usuaCnm=encryptar(clave),
					usuaNbr=detalle,
					usuaTip=tipo,
					empdNid=empleado,
					usuaAtv=CASE estado WHEN 'A' THEN TRUE ELSE FALSE END
			WHERE	usuaCod=secuencial;
			END IF;
			RAISE NOTICE 'VERIFICANDO SI EL USUARIO TIENE PERMISOS DEFINIDOS';
			IF NOT EXISTS(SELECT * FROM tbAdsiUsPer WHERE usuaCod=secuencial) THEN
				RAISE NOTICE 'CREANDO PERMISOS PORQUE USUARIO NO TENIA ASIGNADO';
				INSERT INTO tbAdsiUsPer
					SELECT LPAD(secuencial::TEXT,4,'0'), t.codigo, t.estado
					FROM JSON_TO_RECORDSET(permisos::json) AS
							t(codigo VARCHAR(4), estado BOOLEAN);
			ELSE
				RAISE NOTICE 'ACTUALIZANDO LOS PERMISOS DEL USUARIO';								
				UPDATE tbAdsiUsPer
				SET uperAtv = a.estado
				FROM(
					SELECT t.codigo, t.estado
					FROM 	JSON_TO_RECORDSET(permisos::json) AS
							t(codigo VARCHAR(4), estado BOOLEAN)
				) a
				WHERE tbAdsiUsPer.usuaCod = secuencial AND tbAdsiUsPer.pmsoCod=a.codigo;
			END IF;									
			
			RAISE NOTICE 'Actualización realizada';
			RETURN QUERY
				SELECT 1 AS codigo, 'Los datos del usuario fueron modificados correctamente' AS descripcion;
		EXCEPTION WHEN OTHERS THEN
			RAISE NOTICE 'Codigo de error = %', SQLSTATE;
			RETURN QUERY
				SELECT 0 AS codigo, 'Los datos del usuario no pudieron ser modificados' AS descripcion;
		END;
	END IF;
END;
$$
LANGUAGE 'plpgsql';

DROP FUNCTION IF EXISTS fnLstPermisos;
CREATE OR REPLACE FUNCTION fnLstPermisos()
RETURNS TABLE(	
	codigo VARCHAR(4),
	tag VARCHAR(4),
	descripcion VARCHAR(100)
)
AS $$
DECLARE
	
BEGIN	
	RETURN QUERY
		SELECT	pmsoCod, desencryptar(pmsoTag) AS tag, pmsoDsc
		FROM 	tbAdsiPmso
		WHERE	pmsoAtv = true;
END;
$$
LANGUAGE 'plpgsql';
