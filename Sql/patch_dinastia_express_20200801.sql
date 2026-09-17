


ALTER TABLE tbIvtrPrd ADD COLUMN prdAtv BOOLEAN DEFAULT TRUE;

alter table tbivtrprd drop constraint tbivtrprd_prdund_check;
alter table tbivtrprd add constraint tbivtrprd_prdund_check CHECK(prdUnd IN ('U','Q','L','K','O','G','P'));

CREATE TABLE tbAdsiCrrie(
	crriNid VARCHAR(4) NOT NULL,
	crriNbr VARCHAR(100) NOT NULL,
	crriIcn VARCHAR(3) NOT NULL,
	crriPrc NUMERIC(6,2) NOT NULL DEFAULT 0,
	crriPct BOOLEAN NOT NULL DEFAULT TRUE,
	crriAtv BOOLEAN NOT NULL DEFAULT TRUE,
	CONSTRAINT pk_AdsiCrrie PRIMARY KEY (crriNid)
);

insert into tbAdsiCrrie VALUES('0000','NINGUNO','',0,true,true);
INSERT INTO tbAdsiCrrie VALUES('0001','UBER EATS','UBR',10,TRUE,TRUE);
INSERT INTO tbAdsiCrrie VALUES('0002','GLOVO','GLV',10,TRUE,TRUE);
INSERT INTO tbAdsiCrrie VALUES('0003','RAPPI','RPI',10,TRUE,TRUE);
INSERT INTO tbAdsiCrrie VALUES('0004','TIPTI','TPT',10,TRUE,TRUE);
INSERT INTO tbAdsiCrrie VALUES('0005','CABIFY','CBF',10,TRUE,TRUE);
INSERT INTO tbAdsiCrrie VALUES('0006','SUPER EASY','SES',10,TRUE,TRUE);

INSERT INTO tbConfParam VALUES( 28, 'Imprimir ticket para carrier automáticamente', encryptar('1'),true);
INSERT INTO tbConfParam VALUES( 29, 'Hora de inicio del Backup', encryptar('00:00'),true);
INSERT INTO tbConfParam VALUES( 30, 'Cantidad de Archivos de backup', encryptar('4'),true);
INSERT INTO tbConfParam VALUES( 31, 'Confirmación vía mail de realización del backup', encryptar('0'),true);


alter table tbPeddDPedd drop constraint tbpedddpedd_peddtip_check;
alter table tbPeddDPedd add constraint tbpedddpedd_peddtip_check CHECK (peddTip IN ('L','D','C','E','R'));

alter table tbPeddDVent drop constraint tbpedddvent_venttip_check;
alter table tbPeddDVent add constraint tbpedddvent_venttip_check CHECK (ventTip IN ('L','D','C','E','R'));

alter table tbPeddDVent add ventCrr varchar(4) not null default '0000';
alter table tbPeddDVent add ventCPt numeric(10,2) not null default 0;
alter table tbPeddDVent add ventCVl numeric(10,2) not null default 0;

alter table tbIvtrGrp add grpAtv BOOLEAN DEFAULT true;

COMMENT ON COLUMN tbPeddDVent.ventCrr IS 'Codigo del carrier a utilizar';
COMMENT ON COLUMN tbPeddDVent.ventCpT IS 'Porcentaje a calcular por uso del carrier';
COMMENT ON COLUMN tbPeddDVent.ventCVl IS 'Valor a pagar por el carrier';

alter table tbPeddDVent add constraint fk_PeddVentAdsiCarrier FOREIGN KEY (ventCrr) REFERENCES tbAdsiCrrie(crriNid);

alter table tbPeddDPedd add peddCcr VARCHAR(4) NOT NULL default '0000';
alter table tbPeddDPedd add constraint fk_PeddDPeddAdsiCarrier FOREIGN KEY (peddCcr) REFERENCES tbAdsiCrrie(crriNid);
COMMENT ON COLUMN tbPeddDPedd.peddCcr IS 'Codigo del carrier a utilizar para enviar el pedido';

CREATE TABLE tbLogger(
	loggNid	INT NOT NULL,
	loggTip VARCHAR(3) NOT NULL,
	loggDsc VARCHAR(200) NOT NULL,
	loggOrg VARCHAR(100) NOT NULL,
	loggFch DATE NOT NULL,
	loggHra TIMESTAMP WITHOUT TIME ZONE,
	loggUsr VARCHAR(20),
	CONSTRAINT pk_Logger PRIMARY KEY (loggNid)
);

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
LANGUAGE 'plpgsql';

DROP FUNCTION IF EXISTS fnLogger;
CREATE OR REPLACE FUNCTION fnLogger( IN tipo VARCHAR, IN descrip VARCHAR, IN usuario VARCHAR, IN dispositivo VARCHAR)
RETURNS TABLE(
	codigo INT,
	descripcion TEXT
)
AS $$
DECLARE 
	loggScl INT;
BEGIN
	loggScl := 0;
	
	RAISE NOTICE 'Iniciando registro de actividad en el logger';
	BEGIN	
		RAISE NOTICE 'Obteniendo los datos ';
		
		SELECT COALESCE(MAX(loggNid),1) INTO loggScl FROM tbLogger; 
		
		INSERT INTO tbLogger 
		(loggNid, loggTip, loggDsc, loggFch, loggHra, loggUsr, loggOrg)
			VALUES 
		(loggScl, tipo, descrip, CURRENT_DATE, CURRENT_TIME, usuario, dispositivo);
		
		--COMMIT;			

		RAISE NOTICE 'Finalizó registro de actividad en el logger';
		RETURN QUERY
			SELECT 1 AS codigo, 'Actividad registrada correctamente ' AS descripcion;			
	EXCEPTION WHEN OTHERS THEN
		RAISE NOTICE 'HUBO un error y realizará un rollback';
		RAISE NOTICE 'Codio de error = %', SQLSTATE;
		--ROLLBACK;

		SELECT 0 AS codigo, 'Actividad no pudo ser registrada en el logger' AS descripcion;
	END;
	
END;
$$
LANGUAGE 'plpgsql';

