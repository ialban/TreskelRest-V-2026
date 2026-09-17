--select length(encryptar('IAlban0603;-casañ998')),encryptar('IAlban0603;-casañ998'),desencryptar(encryptar('IAlban0603;-casañ998'))

DROP TABLE IF EXISTS tbCtrlCaja;
DROP TABLE IF EXISTS tbCajaVaEg;
DROP TABLE IF EXISTS tbCajaVaIg;
DROP TABLE IF EXISTS tbCompFpgo;
DROP TABLE IF EXISTS tbCompMComp;
DROP TABLE IF EXISTS tbCompDComp;
DROP TABLE IF EXISTS tbIvtrDInvMT;
DROP TABLE IF EXISTS tbIvtrKdrxMT;
DROP TABLE IF EXISTS tbAdsiMtvos;
DROP TABLE IF EXISTS tbConfParam;
DROP TABLE IF EXISTS tbPeddMVent;
DROP TABLE IF EXISTS tbPeddDFpgo;
DROP TABLE IF EXISTS tbPeddDVent;
DROP TABLE IF EXISTS tbPeddDatCli;
DROP TABLE IF EXISTS tbPeddMPedd;
DROP TABLE IF EXISTS tbPeddDPedd;
DROP TABLE IF EXISTS tbCajaCaja;
DROP TABLE IF EXISTS tbAdsiUsua;
DROP TABLE IF EXISTS tbAdsiEmpr;
DROP TABLE IF EXISTS tbIvtrProv;
DROP TABLE IF EXISTS tbAdsiNid;
DROP TABLE IF EXISTS tbIvtrPrd;
DROP TABLE IF EXISTS tbIvtrRcta;
DROP TABLE IF EXISTS tbIvtrItem;
DROP TABLE IF EXISTS tbIvtrSgrp;
DROP TABLE IF EXISTS tbIvtrGrp;
DROP TABLE IF EXISTS tbAdsiEmpd;
DROP TABLE IF EXISTS tbSystMdls;

CREATE TABLE tbSystMdls(
	mdlsCod VARCHAR(3) NOT NULL,
	mdlsNbr VARCHAR(100) NOT NULL,
	mdlsLcn VARCHAR(100) NOT NULL,
	mdlsVal VARCHAR(2) NOT NULL,
	mdlsAtv VARCHAR(1) NOT NULL CHECK (mdlsAtv IN ('A','I')),
	CONSTRAINT pk_SystMdls PRIMARY KEY (mdlsCod)
);

COMMENT ON COLUMN tbSystMdls.mdlsLcn IS 'Número de licencia de módulo';
COMMENT ON COLUMN tbSystMdls.mdlsVal IS 'Número validador de la licencia';

INSERT INTO tbSystMdls VALUES ('100','Generación de Pedidos','','','A');
INSERT INTO tbSystMdls VALUES ('200','Facturación de Pedidos','','','A');
INSERT INTO tbSystMdls VALUES ('300','Generación de Recetas','','','A');
INSERT INTO tbSystMdls VALUES ('400','Inventario de Productos','','','A');
INSERT INTO tbSystMdls VALUES ('500','Clientes','','','A');
INSERT INTO tbSystMdls VALUES ('510','Proveedores','','','A');
INSERT INTO tbSystMdls VALUES ('520','Caja','','','A');
INSERT INTO tbSystMdls VALUES ('410','Productos para Venta','','','A');

CREATE TABLE tbCajaCaja(
	cajaCod VARCHAR(4) NOT NULL,
	cajaNbr VARCHAR(20) NOT NULL,
	CONSTRAINT pk_CajaCaja PRIMARY KEY (cajaCod)
);

INSERT INTO tbCajaCaja VALUES ('0001','CAJA 1');

CREATE TABLE tbAdsiEmpd(
	empdNid VARCHAR(15) NOT NULL,
	empdTid VARCHAR(1) NOT NULL DEFAULT 'C' CHECK(empdTid IN ('C','P')),
	empdNbr VARCHAR(50) NOT NULL,
	empdApe VARCHAR(50) NOT NULL,
	empdDir VARCHAR(100) DEFAULT '',
	empdTlf VARCHAR(20) DEFAULT '',
	empdCel VARCHAR(20) DEFAULT '',
	empdFna DATE,
	empdChj SMALLINT NOT NULL DEFAULT 0,
	empdAtv BOOLEAN NOT NULL DEFAULT TRUE,
	CONSTRAINT pk_AdsiEmpd PRIMARY KEY (empdNid)
);

COMMENT ON COLUMN tbAdsiEmpd.empdTid IS 'Tipo de ID: C: cédula, P: pasaporte';
COMMENT ON COLUMN tbAdsiEmpd.empdChj IS 'Cantidad de hijos';

CREATE TABLE tbAdsiPmso(
	pmsoCod VARCHAR(4) NOT NULL,
	pmsoTag VARCHAR(50) NOT NULL,
	pmsoDsc VARCHAR(100) NOT NULL,
	pmsoAtv BOOLEAN NOT NULL DEFAULT TRUE,
	CONSTRAINT pk_AdsiPmso PRIMARY KEY (pmsoCod)
);

COMMENT ON COLUMN tbAdsiPmso.pmsoTag IS 'Acronimo con el cual se identificará a cada uno de los diferentes permisos';

INSERT INTO tbAdsiPmso VALUES ('0001', encryptar('VING'), 'Permite crear vales de ingreso',true);
INSERT INTO tbAdsiPmso VALUES ('0002', encryptar('VEGR'), 'Permite crear vales de egreso',true);
INSERT INTO tbAdsiPmso VALUES ('0003', encryptar('GSGR'), 'Permite acceder a las pantallas de Grupos y Subgrupos de productos',true);
INSERT INTO tbAdsiPmso VALUES ('0004', encryptar('USRE'), 'Permite acceder a las pantallas de Usuarios y Empleados',true);
INSERT INTO tbAdsiPmso VALUES ('0005', encryptar('CONF'), 'Permite acceder a la pantalla de Configuración',true);
INSERT INTO tbAdsiPmso VALUES ('0006', encryptar('PEDD'), 'Permite acceder a la pantalla de Pedidos',true);
INSERT INTO tbAdsiPmso VALUES ('0007', encryptar('VENT'), 'Permite acceder a la pantalla de Venta y Facturación',true);
INSERT INTO tbAdsiPmso VALUES ('0008', encryptar('COMP'), 'Permite acceder a la pantalla de Compras',true);
INSERT INTO tbAdsiPmso VALUES ('0009', encryptar('INVT'), 'Permite acceder a la pantalla de Inventario',true);
INSERT INTO tbAdsiPmso VALUES ('0010', encryptar('CAJA'), 'Permite acceder al Control de Caja',true);
INSERT INTO tbAdsiPmso VALUES ('0011', encryptar('RCTA'), 'Permite acceder a las pantalla de Productos y Recetas',true);
INSERT INTO tbAdsiPmso VALUES ('0012', encryptar('DSCT'), 'Permite otorgar descuentos',true);
INSERT INTO tbAdsiPmso VALUES ('0013', encryptar('GIFT'), 'Permite otorgar regalos',true);
INSERT INTO tbAdsiPmso VALUES ('0014', encryptar('DPED'), 'Permite eliminar un pedido',true);
INSERT INTO tbAdsiPmso VALUES ('0015', encryptar('RPTE'), 'Permite generar Reportes',true);
INSERT INTO tbAdsiPmso VALUES ('0016', encryptar('AFCT'), 'Permite anular una Factura',true);

INSERT INTO tbAdsiPmso VALUES ('9999', encryptar('BKPS'), 'Permite configurar y generar Backups',true);

CREATE TABLE tbAdsiUsua(
	usuaCod VARCHAR(4) NOT NULL,
	usuaUsr VARCHAR(20) NOT NULL,
	usuaNbr VARCHAR(50) NOT NULL,
	usuaClv VARCHAR(100) NOT NULL,
	usuaCnm VARCHAR(100),
	usuaCcm VARCHAR(100),
	usuaCad VARCHAR(200) NOT NULL DEFAULT encryptar('N0V4L1D0'),
	usuaTip VARCHAR(1) NOT NULL DEFAULT '' CHECK (usuaTip IN ('M','A','C','S')),
	empdNid VARCHAR(15),
	usuaAtv BOOLEAN NOT NULL DEFAULT TRUE,
	CONSTRAINT pk_AdsiUsua PRIMARY KEY(usuaCod),
	CONSTRAINT ul_AdsiUsuaUsr UNIQUE (usuaUsr),
	CONSTRAINT fk_AdsiUsuaEmpd FOREIGN KEY(empdNid) REFERENCES tbAdsiEmpd(empdNid)
);

COMMENT ON COLUMN tbAdsiUsua.usuaTip IS 'Tipo de usuario M:MESERO, A:ADMINISTRADOR, C:CAJERO, S:SUPERVISOR';
COMMENT ON COLUMN tbAdsiUsua.usuaCad IS 'Clave de administrador';
COMMENT ON COLUMN tbAdsiUsua.usuaCcm IS 'Clave para cajero o mesero';
COMMENT ON COLUMN tbAdsiUsua.usuaCnm IS 'Clave numérico para el usuario admin';

INSERT INTO tbAdsiUsua VALUES('9999','admin','Administrador',encryptar('Tr3sk3lS0ft'),encryptar('6669'),encryptar(null),encryptar('Tr3sk3lS0ft'),'A',null,true);

-- TABLA PARA RELACIONAR USUARIO CON PERMISO
CREATE TABLE tbAdsiUsPer(
	usuaCod	VARCHAR(4),
	pmsoCod VARCHAR(4),
	uperAtv BOOLEAN,
	CONSTRAINT fk_UsPerUsua FOREIGN KEY (usuaCod) REFERENCES tbAdsiUsua (usuaCod),
	CONSTRAINT fk_UsPerPmso FOREIGN KEY (pmsoCod) REFERENCES tbAdsiPmso (pmsoCod)
);

CREATE TABLE tbConfParam(
    confNid INT NOT NULL,
	confDsc VARCHAR(200) NOT NULL,
	confVal VARCHAR(200) NOT NULL,
	confAtv BOOLEAN  NOT NULL DEFAULT TRUE,
	CONSTRAINT pk_ConfParam PRIMARY KEY(confNid)
);

INSERT INTO tbConfParam VALUES(  1, 'Precios con IVA incluido', encryptar('1'), true);
INSERT INTO tbConfParam VALUES(  2, 'Valor del IVA', encryptar('12'), true);
INSERT INTO tbConfParam VALUES(  3, 'Usa Touch Screen', encryptar('1'),true);
INSERT INTO tbConfParam VALUES(  4, 'Cantidad de mesas', encryptar('10'),true);
INSERT INTO tbConfParam VALUES(  5, 'Envia correo al propietario en caso de eliminación', encryptar('1'),true);
INSERT INTO tbConfParam VALUES(  6, 'Servidor de correo', encryptar(''),true);
INSERT INTO tbConfParam VALUES(  7, 'Puerto Smpt de correo', encryptar(''),true);
INSERT INTO tbConfParam VALUES(  8, 'Usuario de correo', encryptar(''),true);
INSERT INTO tbConfParam VALUES(  9, 'Contraseña de correo', encryptar(''),true);
INSERT INTO tbConfParam VALUES( 10, 'Correo usa autenticación', encryptar('0'),true);
INSERT INTO tbConfParam VALUES( 11, 'Correo usa TLS', encryptar('0'),true);
INSERT INTO tbConfParam VALUES( 12, 'Ejecuta control de inventario', encryptar('1'),true);
INSERT INTO tbConfParam VALUES( 13, 'Cantidad de caracteres para el password', encryptar('10'),true);
INSERT INTO tbConfParam VALUES( 14, 'Correo 1', encryptar(''),true);
INSERT INTO tbConfParam VALUES( 15, 'Copiar a correos', encryptar('0'),true);
INSERT INTO tbConfParam VALUES( 16, 'Correo 2', encryptar(''),true);
INSERT INTO tbConfParam VALUES( 17, 'Correo 3', encryptar(''),true);
INSERT INTO tbConfParam VALUES( 18, 'Correo 4', encryptar(''),true);
INSERT INTO tbConfParam VALUES( 19, 'Habilita envío de email', encryptar('0'),true);
INSERT INTO tbConfParam VALUES( 20, 'Usa password numérico', encryptar('1'),true);
INSERT INTO tbConfParam VALUES( 21, 'Usa descuentos manuales', encryptar('0'),true);
INSERT INTO tbConfParam VALUES( 22, 'Tema para reportes', encryptar('-1'),true);

INSERT INTO tbConfParam VALUES( 23, 'Cambio rápido entre facturación y pedidos', encryptar('0'),true);
INSERT INTO tbConfParam VALUES( 24, 'Inicio de secuencial de pedidos', encryptar('1'),true);
INSERT INTO tbConfParam VALUES( 25, 'Imprime hora en talonario de cocina', encryptar('0'),true);

INSERT INTO tbConfParam VALUES( 26, 'Cantidad de provincias', encryptar('24'),true);
INSERT INTO tbConfParam VALUES( 27, 'Ticket de cocina con 1=cantidad/producto, 0=producto/cantidad', encryptar('1'),true);
INSERT INTO tbConfParam VALUES( 28, 'Imprimir ticket para carrier automáticamente', encryptar('1'),true);

INSERT INTO tbConfParam VALUES( 29, 'Hora de inicio del Backup', encryptar('00:00'),true);
INSERT INTO tbConfParam VALUES( 30, 'Cantidad de Archivos de backup', encryptar('4'),true);
INSERT INTO tbConfParam VALUES( 31, 'Confirmación vía mail de realización del backup', encryptar('0'),true);
INSERT INTO tbConfParam VALUES( 32, 'Cantidad de lineas de alimentacion para impresión', encryptar('5'),true);

INSERT INTO tbConfParam VALUES( 33, 'Permitir upload archivos de backup', encryptar('0'),true);
INSERT INTO tbConfParam VALUES( 34, 'Url del servidor upload', encryptar(''),true);
INSERT INTO tbConfParam VALUES( 35, 'Usuario server upload', encryptar(''),true);
INSERT INTO tbConfParam VALUES( 36, 'Clave server upload', encryptar(''),true);

INSERT INTO tbConfParam VALUES( 37, 'Hora inicio backup 2', encryptar(''),true);
INSERT INTO tbConfParam VALUES( 38, 'Hora inicio backup 3', encryptar(''),true);

/*
  Estos campos funcionan en versión 3.0
*/
INSERT INTO tbConfParam VALUES( 39, 'Modo Ppedido-Facturación', encryptar(''),true); 
INSERT INTO tbConfParam VALUES( 40, 'Tipo botones para productos a usar', encryptar('ST'),true);
-- ST  = SOLO TEXTO,      SI  = SOLO IMAGEN,      IT = TEXTO E IMAGEN,
-- STE = SOLO TEXTO EDIT, SIT = SOLO IMAGEN EDIT, ITE = TEXTO E IMAGEN EDIT

-------------------------------------------------------------

INSERT INTO tbConfParam VALUES( 41, 'Usa Ventas por carrier en Cierre de caja', encryptar('0'),true);
INSERT INTO tbConfParam VALUES( 42, 'Cierre de caja Acumulativo', Encryptar('1'), true)

INSERT INTO tbConfParam values (43, 'Ejecutar Backup', encryptar('0'), true);
INSERT INTO tbConfParam values (44, 'Ubicación Pg_dump', encryptar(''), true);

-------------------------------------------------------------
INSERT INTO tbConfParam VALUES (45, 'Organización de items en pedidos',encryptar('H'),true);

-------------------------------------------------------------
insert into tbConfParam values (46, 'Permite usar Impresora para Proformar', encryptar('0'), true);
insert into tbConfParam values (47, 'Permite usar Impresora para Parrillas', encryptar('1'), true);

INSERT INTO tbConfParam VALUES (48,'Formato numérico Reportes Excel', encryptar('###,###.##'),true);

INSERT INTO tbConfParam VALUES (49,'Permite Enviar Pedidos a Bar', encryptar('0'),true);
INSERT INTO tbConfParam VALUES (50,'Cantidad mesas Sala Alterna', encryptar('0'),true);
INSERT INTO tbConfParam VALUES (51,'Cantidad mesas Exterior', encryptar('0'),true);
INSERT INTO tbConfParam VALUES (52,'Cantidad mesas Terraza', encryptar('0'),true);
INSERT INTO tbConfParam VALUES (53,'Cantidad mesas 2do piso', encryptar('0'),true);

INSERT INTO tbConfParam VALUES (55,'Envio al SRI Automático',encryptar('0'), true);
INSERT INTO tbConfParam VALUES (56,'Envío automático',encryptar('0'), true);
INSERT INTO tbConfParam VALUES (57,'Ruta archivo firma',encryptar(''), true);
INSERT INTO tbConfParam VALUES (58,'Hora de inicio',encryptar(''), true);
INSERT INTO tbConfParam VALUES (59,'Minutos para reintento',encryptar('0'), true);
INSERT INTO tbConfParam VALUES (60, 'GUID de la empresa para Facturación Electronica', encryptar(''),true);
INSERT INTO tbConfParam VALUES (61, 'URL Treskel WebService', encryptar(''),true);
INSERT INTO tbConfParam VALUES (62, 'Usuario Treskel WebService', encryptar(''),true);
INSERT INTO tbConfParam VALUES (63, 'Clave Treskel WebService', encryptar(''),true);
INSERT INTO tbConfParam VALUES (64, 'Clave para la firma electrónica', encryptar(''), true);
INSERT INTO tbConfParam VALUES (65, 'Ruta Carpeta para grabar XML generados en Facturacion Electronica', encryptar(''), true);

INSERT INTO tbConfParam VALUES (66, 'Ruta imagen Logotipo Factura', encryptar(''),true);

CREATE TABLE tbAdsiEmpr(
	emprNid  VARCHAR(100) NOT NULL,
	emprNbr  VARCHAR(100) NOT NULL,
	emprImg  BYTEA,
	emprIco  BYTEA,
	emprCrd  VARCHAR(100) NOT NULL,
	emprAcr  INT NOT NULL DEFAULT DATE_PART('YEAR',CURRENT_DATE)::INT,
	emprVld  VARCHAR(1),
	emprrsoc VARCHAR(100),
	emprdirm VARCHAR(250),
	emprdirl VARCHAR(250),
	emprecod VARCHAR(100),
	emprcont VARCHAR(100),
	emprretc VARCHAR(100),
	emprlofc BYTEA
);

-- tabla para definir los carrier que llevarán la comida al cliente
CREATE TABLE tbAdsiCrrie(
	crriNid VARCHAR(4) NOT NULL,
	crriNbr VARCHAR(100) NOT NULL,
	crriIcn VARCHAR(3) NOT NULL,
	crriPrc NUMERIC(6,2) NOT NULL DEFAULT 0,
	crriApi VARCHAR(1) NOT NULL DEFAULT 'I',
	crriPct BOOLEAN NOT NULL DEFAULT TRUE,
	crriAtv BOOLEAN NOT NULL DEFAULT TRUE,
	crriImg BYTEA NULL,	
	CONSTRAINT pk_AdsiCrrie PRIMARY KEY (crriNid)
);

COMMENT ON COLUMN tbAdsiCrrie.crriApi IS 'Se aplica a T: Total, I: Items de Venta';

INSERT INTO tbAdsiCrrie VALUES('0001','UBER EATS','UBR',10,TRUE,TRUE);
INSERT INTO tbAdsiCrrie VALUES('0002','GLOVO','GLV',10,TRUE,TRUE);
INSERT INTO tbAdsiCrrie VALUES('0003','RAPPI','RPI',10,TRUE,TRUE);
INSERT INTO tbAdsiCrrie VALUES('0004','TIPTI','TPT',10,TRUE,TRUE);
INSERT INTO tbAdsiCrrie VALUES('0005','CABIFY','CBF',10,TRUE,TRUE);
INSERT INTO tbAdsiCrrie VALUES('0006','SUPER EASY','SES',10,TRUE,TRUE);
INSERT INTO tbAdsiCrrie VALUES('0007','PEDIDOS YA','PEY',10,TRUE,TRUE);
INSERT INTO tbAdsiCrrie VALUES('0008','DiDi','DID',10,true,true,'I');

CREATE TABLE tbPctRete(
	reteScl VARCHAR(3) NOT NULL,
	reteDsc VARCHAR(200) NOT NULL,
	reteVal NUMERIC(6,2) NOT NULL,
	reteApl VARCHAR(2) NOT NULL DEFAULT 'I',
	reteAct BOOLEAN NOT NULL DEFAULT TRUE,
	CONSTRAINT pk_PctRete PRIMARY KEY (reteScl)
);

COMMENT ON COLUMN tbPctRete.reteApl IS 'Se aplica a I: Iva, R: Renta';

CREATE TABLE tbIvtrProv(
	provNid VARCHAR(25) NOT NULL,
	provNbr VARCHAR(100) NOT NULL,
	provDir VARCHAR(200) NOT NULL,
	provTlf VARCHAR(50) NOT NULL,
	provEml VARCHAR(100),
	provCto VARCHAR(100),
	provAtv BOOLEAN NOT NULL DEFAULT TRUE,
	CONSTRAINT pk_IvtrProv PRIMARY KEY (provNid)
);

COMMENT ON COLUMN tbIvtrProv.provEml IS 'email de la empresa';
COMMENT ON COLUMN tbIvtrProv.provCto IS 'Nombre del contacto en la empresa';

CREATE TABLE tbLogger(
	loggNid	INT NOT NULL,
	loggTip VARCHAR(3) NOT NULL,
	loggDsc VARCHAR(200) NOT NULL,
	loggOrg VARCHAR(100) NOT NULL,
	loggFch DATE NOT NULL,
	loggHra TIME WITHOUT TIME ZONE,
	loggUsr VARCHAR(20),
	CONSTRAINT pk_Logger PRIMARY KEY (loggNid)
);

COMMENT ON COLUMN tbLogger.loggTip IS 'Tipo de Logger, BKP = backup';

CREATE TABLE tbBkpRqsts(
	bkpNid INT NOT NULL,
	cajacod VARCHAR(4),
	bkpAtv BOOLEAN,
	bkpCmt VARCHAR(200),
	CONSTRAINT pkBkpRequest PRIMARY KEY (bkpNid)
);

COMMENT ON COLUMN tbBkpRqsts.bkpAtv IS 'FALSE: BACKUP iniciado pero sin finalizar, TRUE: backup finalizado';

CREATE TABLE tbIvtrDInvMT(
    invtScl INT NOT NULL,
	prdCod  VARCHAR(6),
	dinvDsc VARCHAR(100),
	dinvTip VARCHAR(1) CHECK(dinvTip IN ('I','E','T','A')),
	dinvFch DATE,
	dinvCnt NUMERIC(10,2),
	dinvUnd VARCHAR(1),
	peddScl VARCHAR(10),
	itemAtv BOOLEAN DEFAULT TRUE,
	CONSTRAINT pk_IvtrDInvMt PRIMARY KEY (invtScl),
	CONSTRAINT fk_DinvMtProd FOREIGN KEY (prdCod) REFERENCES tbIvtrPrd(prdCod)
);

COMMENT ON COLUMN tbIvtrDINvMT.dinvTip IS 'Tipo de transacción de inventario I=Ingreso, E=Egreso, T=Transferencia, A=Ajuste';

CREATE TABLE tbIvtrKdrxMT(
	prdCod VARCHAR(6),
	krdxAni INT,
	
	krdxBEn NUMERIC(10,2), -- Inicio Enero
	krdxIEn NUMERIC(10,2), -- Ingreso Enero
	krdxEEn NUMERIC(10,2), -- Egreso Enero
	krdxSEn NUMERIC(10,2), -- Saldo Enero
	
	krdxBFe NUMERIC(10,2),
	krdxIFe NUMERIC(10,2),
	krdxEFe NUMERIC(10,2),
	krdxSFe NUMERIC(10,2),
	
	krdxBMa NUMERIC(10,2),
	krdxIMa NUMERIC(10,2),
	krdxEMa NUMERIC(10,2),
	krdxSMa NUMERIC(10,2),
	
	krdxBAb NUMERIC(10,2),
	krdxIAb NUMERIC(10,2),
	krdxEAb NUMERIC(10,2),
	krdxSAb NUMERIC(10,2),
	
	krdxBMy NUMERIC(10,2),
	krdxIMy NUMERIC(10,2),
	krdxEMy NUMERIC(10,2),
	krdxSMy NUMERIC(10,2),
	
	krdxBJu NUMERIC(10,2),
	krdxIJu NUMERIC(10,2),
	krdxEJu NUMERIC(10,2),
	krdxSJu NUMERIC(10,2),
	
	krdxBJl NUMERIC(10,2),
	krdxIJl NUMERIC(10,2),
	krdxEJl NUMERIC(10,2),
	krdxSJl NUMERIC(10,2),
	
	krdxBAg NUMERIC(10,2),
	krdxIAg NUMERIC(10,2),
	krdxEAg NUMERIC(10,2),
	krdxSAg NUMERIC(10,2),
	
	krdxBSp NUMERIC(10,2),
	krdxISp NUMERIC(10,2),
	krdxESp NUMERIC(10,2),
	krdxSSp NUMERIC(10,2),
	
	krdxBOc NUMERIC(10,2),
	krdxIOc NUMERIC(10,2),
	krdxEOc NUMERIC(10,2),
	krdxSOc NUMERIC(10,2),
	
	krdxBNv NUMERIC(10,2),
	krdxINv NUMERIC(10,2),
	krdxENv NUMERIC(10,2),
	krdxSNv NUMERIC(10,2),
	
	krdxBDi NUMERIC(10,2),
	krdxIDi NUMERIC(10,2),
	krdxEDi NUMERIC(10,2),
	krdxSDi NUMERIC(10,2),
	
	CONSTRAINT fk_IvtrInvMTProd FOREIGN KEY (prdCod) REFERENCES tbIvtrPrd (prdCod)
);

COMMENT ON COLUMN tbIvtrKdrxMT.krdxAni IS 'Anio de realización del inventario a controlar';

CREATE TABLE tbAdsiNid(
	nidNid VARCHAR(25) NOT NULL,
	nidTip VARCHAR(1) NOT NULL DEFAULT 'C' CHECK(nidTip IN ('C','P','R')),
	nidNbr VARCHAR(100) NOT NULL CHECK(LENGTH(nidNbr)>2),
	nidDir VARCHAR(100) NOT NULL DEFAULT '',
	nidTlf VARCHAR(30) NOT NULL DEFAULT '',
	nidEml VARCHAR(100) NOT NULL DEFAULT '',
	CONSTRAINT pk_AdsiNid PRIMARY KEY(nidNid)
);

COMMENT ON COLUMN tbAdsiNid.nidTip IS 'C:CEDULA, P:PASAPORTE, R:RUC';
COMMENT ON COLUMN tbAdsiNid.nidEml IS 'CORREO ELECTRÓNICO';

INSERT INTO tbAdsiNid VALUES('9999999999999','R','CLIENTE FINAL','','','');

CREATE TABLE tbMoviRete(
	moviscl VARCHAR(10) NOT NULL,
	movinum VARCHAR(10) NOT NULL,
	movifau TIMESTAMP WITHOUT TIME ZONE,
	movifch TIMESTAMP WITHOUT TIME ZONE NOT NULL,
	movisau VARCHAR(10) NOT NULL,
	movicli VARCHAR(20) NOT NULL,
	movitcv VARCHAR(1) NOT NULL DEFAULT 'F' CHECK (movitcv IN ('F','N', 'L', 'T', '0')),
	movincv VARCHAR(20) NOT NULL DEFAULT '',
	ventscl VARCHAR(10),
	movistd VARCHAR(1) NOT NULL DEFAULT 'A' CHECK (movistd IN ('A','D','E')),
	CONSTRAINT fk_movirete_vent FOREIGN KEY (ventscl) REFERENCES tbPeddDVent (ventscl),
	CONSTRAINT fk_movirete_clie FOREIGN KEY (movicli) REFERENCES tbAdsiNid (nidNid)
);

COMMENT ON COLUMN tbMoviRete.movitcv IS 'TIPO DE COMPROBANTE DE VENTA(F: FACTURA, N: NOTA DE VENTA - RISE, L: LIQUIDACIONES EN COMPRAS Y PRESTACIONES DE SERVICIOS), T: TICKET, O: OTROS';
COMMENT ON COLUMN tbMoviRete.movifau IS 'FECHA DE LA AUTORIZACIÓN';
COMMENT ON COLUMN tbMoviRete.movisau IS 'NUMERO DE AUTORIZACIÓN DEL SRI';
COMMENT ON COLUMN tbMoviRete.movicli IS 'NUMERO DE IDENTIFICACIÓN DEL CLIENTE';
COMMENT ON COLUMN tbMoviRete.movistd IS 'ESTADO DE LA RETENCION: A = ACTIVO, D=ANULADO, E=ELIMINADO';

CREATE TABLE tbMoviReteDet(
	moviscl VARCHAR(10) NOT NULL,
	moviafs INT NOT NULL DEFAULT 0,
	retescl VARCHAR(3),
	retesri VARCHAR(20),
	retepct NUMERIC(6,2),
	retetip VARCHAR(3),
	movival NUMERIC(14,4),
	movifac NUMERIC(14,4),
	CONSTRAINT fk_movirete_rete FOREIGN KEY (retescl) REFERENCES tbPctRete (retescl)
);

COMMENT ON COLUMN tbMoviReteDet.moviafs IS 'ANIO FISCAL';
COMMENT ON COLUMN tbMoviReteDet.movival IS 'VALOR RETENIDO';
COMMENT ON COLUMN tbMoviReteDet.movifac IS 'VALOR DEL CUAL ES REALIZADA LA RETENCION (BASE IMPONIBLE)';
COMMENT ON COLUMN tbMoviReteDet.retepct IS 'PORCENTAJE DE LA RETENCION';
COMMENT ON COLUMN tbMoviReteDet.retesri IS 'CODIGO DEL SRI PARA LA RETENCION';


CREATE TABLE tbIvtrPrd(
	prdCod VARCHAR(6) NOT NULL,
	prdNbr VARCHAR(100) NOT NULL,
	prdUnd VARCHAR(1) NOT NULL DEFAULT 'U' CHECK(prdUnd IN ('U','Q','L','K','O','G','P')),
	prdGrp VARCHAR(1) NOT NULL DEFAULT '',
	prdImg BYTEA,
	prdFnl BOOLEAN NOT NULL DEFAULT FALSE,
	prdAtv BOOLEAN DEFAULT TRUE,
	CONSTRAINT pk_IvtrPrd PRIMARY KEY(prdCod)
);

COMMENT ON COLUMN tbIvtrPrd.prdFnl IS 'PERMITE INDICAR SI ES UN PRODUCTO TERMINADO O NO';
COMMENT ON COLUMN tbIvtrPrd.prdGrp IS 'V:VEGETAL, C:CARNES, W:VINOS, L:LACTEO, M:MARISCO, E:ESPECIAS, F:FRUTAS';
COMMENT ON COLUMN tbIvtrPrd.prdUnd IS 'Unidad para cálculo de inventario: U:UNIDAD, L:LIBRA, Q: QUINTAL, K:KILO, O:ONZA, G:GRAMO, P: PORCIÓN';

CREATE TABLE tbCompDComp(
	compNid VARCHAR(10) NOT NULL,
	compFac VARCHAR(20) NOT NULL,
	provNid VARCHAR(25) NOT NULL,
	compFch DATE NOT NULL,
	compSbt NUMERIC(10,2) DEFAULT 0,
	compIva NUMERIC(10,2) DEFAULT 0,
	compTot NUMERIC(10,2) DEFAULT 0,
	compAtv BOOLEAN DEFAULT TRUE,
	codCaja VARCHAR(4) NOT NULL,
	CONSTRAINT pk_CompComp PRIMARY KEY (compNid),
	CONSTRAINT fk_CompCompIvtrProv FOREIGN KEY (provNid) REFERENCES tbIvtrProv(provNid)
);

CREATE TABLE tbCompMComp(
	compNid VARCHAR(10),
	prdCod  VARCHAR(6),
	compCnt DECIMAL(10,2) DEFAULT 1,
	compUnd VARCHAR(1) DEFAULT 'U' CHECK(compUnd IN ('U','Q','L','K','O','G')),
	compPrc DECIMAL(10,2) DEFAULT 1,
	compPto DECIMAL(10,2) DEFAULT 1,
	CONSTRAINT fk_CompMCompPrd FOREIGN KEY (prdCod) REFERENCES tbIvtrPrd(prdCod),
	CONSTRAINT fk_CompMCompDComp FOREIGN KEY (compNid) REFERENCES tbCompDComp(compNid)
);

COMMENT ON COLUMN tbCompMComp.compUnd IS 'Unidad para cálculo de inventario: U:UNIDAD, L:LIBRA, Q: QUINTAL, K:KILO, O:ONZA, G:GRAMO';

CREATE TABLE tbCompFpgo(
	compNid VARCHAR(10),
	fpgoTip VARCHAR(1) NOT NULL DEFAULT 'E' CHECK(fpgoTip IN ('E','T','C')),
	fpgoVal DECIMAL(10,2) NOT NULL DEFAULT 0,
	fpgoTjc VARCHAR(1) DEFAULT 'N' NOT NULL CHECK(fpgoTjc IN ('N','V','A','M','D','C')),
	fpgoTpt VARCHAR(1) DEFAULT 'N' NOT NULL CHECK(fpgoTpt IN ('N','C','D')),
	fpgoMss INT DEFAULT 0 CHECK (fpgoMss IN (0, 3, 6, 9)),
	fpgoBco VARCHAR(2) DEFAULT '',
	fpgoChe VARCHAR(6) DEFAULT ''
);

COMMENT ON COLUMN tbCompFpgo.fpgoTip IS 'Tipo de forma de pago E: EFECTIVO, T: TARJETA DE CRÉDITO, C:CHEQUE';
COMMENT ON COLUMN tbCompFpgo.fpgoTpt IS 'Tipo de pago con tarjeta N: NINGUNO, C: CORRIENTE, D: DIFERIDO';
COMMENT ON COLUMN tbCompFpgo.fpgoMss IS 'Meses a diferir pagos, 0: NINGUNO, 3 o 6 o 9 MESES';
COMMENT ON COLUMN tbCompFpgo.fpgoTip IS 'Tarjetas de crédito N: NINGUNA, V: VISA, A: AMERICAN, M: MASTERCARD, D:DISCOVER, C: DINNERS CLUB';
COMMENT ON COLUMN tbCompFpgo.fpgoBco IS 'Banco que emite el cheque BP: PICHINCHA, BB: BOLIVARIANO, BG:GUAYAQUIL, BD: PRODUCBANCO, BM:MACHALA, BA:AZUAY, PB:PACIFICO, BR:RUMIÑAHUI, BS:SOLIDARIO';
COMMENT ON COLUMN tbCompFpgo.fpgoChe IS 'Número de cheque';


CREATE TABLE tbIvtrGrp(
	grpCod VARCHAR(4) NOT NULL CHECK(LENGTH(grpCod)=4),
	grpNbr VARCHAR(100) NOT NULL CHECK(LENGTH(grpNbr)>2),
	grpAtv BOOLEAN NOT NULL DEFAULT TRUE,
	CONSTRAINT pk_IvtrGrp PRIMARY KEY(grpCod)
);

INSERT INTO tbIvtrGrp VALUES('0001','CALDOS Y SOPAS', TRUE);
INSERT INTO tbIvtrGrp VALUES('0002','ARROCES', TRUE);
INSERT INTO tbIvtrGrp VALUES('0003','BEBIDAS', TRUE);

CREATE TABLE tbIvtrSgrp(
	sgrpCod VARCHAR(4) NOT NULL CHECK(LENGTH(sgrpCod)=4),
	sgrpNbr VARCHAR(100) NOT NULL DEFAULT '' CHECK(LENGTH(sgrpNbr)>2),
	grpCod VARCHAR(4),
	CONSTRAINT pk_IvtrSgrp PRIMARY KEY(sgrpCod),
	CONSTRAINT fk_IvtrSgrpIvtrGrp FOREIGN KEY (grpCod) REFERENCES tbIvtrGrp(grpCod)
);

CREATE TABLE tbIvtrItem(
	itemCod VARCHAR(4) NOT NULL,
	sgrpCod VARCHAR(4) NOT NULL,
	itemNbr VARCHAR(100) NOT NULL,
	itemIva BOOLEAN NOT NULL DEFAULT FALSE,
	itemPrc DECIMAL(10, 2) NOT NULL,
	itemPiv DECIMAL(12, 6),
	itemImg BYTEA,
	itemAtv BOOLEAN DEFAULT TRUE,	
	prdCod  VARCHAR(6), 
	itemCoc BOOLEAN DEFAULT TRUE,
	itemSbg BOOLEAN DEFAULT FALSE,
	itemDelCoc BOOLEAN DEFAULT FALSE,
	itemParr BOOLEAN DEFAULT FALSE,
	itemBar BOOLEAN DEFAULT FALSE,
	itemGIva BOOLEAN DEFAULT TRUE,
	CONSTRAINT pk_IvtrItem PRIMARY KEY(itemCod),
	CONSTRAINT fk_IvtrItemIvtrSgrp FOREIGN KEY(sgrpCod) REFERENCES tbIvtrSgrp(sgrpCod),
	CONSTRAINT fk_IvtrItemIvtrPrd FOREIGN KEY(prdCod) REFERENCES tbIvtrPrd(prdCod)
);

COMMENT ON COLUMN tbIvtrItem.itemIva IS 'Indica si el precio del producto ya contiene IVA';
COMMENT ON COLUMN tbIvtrItem.itemPiv IS 'Precio del producto sin IVA, se usa cuando el campo itemIva es true';
COMMENT ON COLUMN tbIvtrItem.itemSbg IS 'Permite indicar si usa el dato de subgrupo para presentar en factura';

CREATE TABLE tbIvtrRcta(
	itemCod VARCHAR(4),
	prdCod  VARCHAR(6),
	rctaCnt DECIMAL(10,2),
	rctaUnd VARCHAR(1) DEFAULT 'U',
	rctaObv VARCHAR(100) DEFAULT '',
	rctaInv BOOLEAN DEFAULT FALSE,
	CONSTRAINT fk_IvtrRctaIvtrItem FOREIGN KEY (itemCod) REFERENCES tbIvtrItem(itemCod)
);

COMMENT ON COLUMN tbIvtrRcta.rctaInv IS 'Define si el item afecta al inventario';
COMMENT ON COLUMN tbIvtrRcta.rctaUnd IS 'Unidad para cálculo de inventario: U:UNIDAD, Q:QUINTAL, L:LIBRA, K:KILO, O:ONZA, G:GRAMO';

CREATE TABLE tbPeddDPedd(
	peddScl VARCHAR(10) NOT NULL,
	peddLta VARCHAR(1) NOT NULL DEFAULT 'A',
	usuaCod VARCHAR(4) NOT NULL,
	peddFec DATE NOT NULL,
	peddTip VARCHAR(1) NOT NULL DEFAULT 'L' CHECK(peddTip IN ('L','D','C','E','R')),
	peddCcr VARCHAR(4) NOT NULL DEFAULT '0000',
	peddMsa VARCHAR(3) NOT NULL DEFAULT '',		
	peddObv VARCHAR(100),
	peddHin TIME,
	peddHFn TIME,
	peddAtv BOOLEAN NOT NULL DEFAULT TRUE,
	peddEli BOOLEAN NOT NULL DEFAULT FALSE,
	CONSTRAINT pk_PeddDPedd PRIMARY KEY(peddScl),
	CONSTRAINT fk_PeddDPeddAdsiUsua FOREIGN KEY (usuaCod) REFERENCES tbAdsiUsua(usuaCod),
	CONSTRAINT fk_PeddDPeddAdsiCarrier FOREIGN KEY (peddCcr) REFERENCES tbAdsiCrrie(crriNid)
);

COMMENT ON COLUMN tbPeddDPedd.peddTip IS 'L:LOCAL, C:LLEVAR, D:DOMICILIO, E: ENTREGA, R: CARRIER';
COMMENT ON COLUMN tbPeddDPedd.peddCcr IS 'Codigo del carrier a utilizar para enviar el pedido';


CREATE TABLE tbPeddMPedd(
	peddScl VARCHAR(10),
	itemCod VARCHAR(4),
	itemCnt INT,
	itemIva BOOLEAN,
	itemPrc DECIMAL(10,6),
	itemPiv DECIMAL(10,6),
	itemPto DECIMAL(10,6),
	peddAtv BOOLEAN,
	CONSTRAINT fk_PeddMPeddDPedd FOREIGN KEY (peddScl) REFERENCES tbPeddDPedd(peddScl),
	CONSTRAINT fk_PeddMPeddIvtrItem FOREIGN KEY (itemCod) REFERENCES tbIvtrItem(itemCod)
);

-- TABLA PARA Peddidos con Carrier
CREATE TABLE tbPeddCPedd(
	peddScl VARCHAR(10),
	itemCod VARCHAR(4),
	carrPrc DECIMAL(10,6),
	itemPrr DECIMAL(10,6),
	carrVal DECIMAL(10,6),
	CONSTRAINT fk_PeddCPeddDPedd FOREIGN KEY (peddScl) REFERENCES tbPeddDPedd(peddScl),
	CONSTRAINT fk_PeddCPeddIvtrItem FOREIGN KEY (itemCod) REFERENCES tbIvtrItem(itemCod)
);

COMMENT ON COLUMN tbPeddCPedd.itemPrc IS 'Precio real del item';

CREATE TABLE tbPeddDatCarr(
	peddScl varchar(10),
	peddCrr varchar(4),
	peddCod varchar(10),
	peddObv varchar(100),
	CONSTRAINT fk_PeddDatCliPedd FOREIGN KEY (peddScl) REFERENCES tbPeddDPedd(peddScl)
); 

CREATE TABLE tbPeddDatCli(
	peddScl VARCHAR(10),
	clieNid VARCHAR(20),
	clieNbr VARCHAR(50),
	clieDir VARCHAR(100),
	clieTlf VARCHAR(50),
	clieRef VARCHAR(100),
	CONSTRAINT fk_PeddDatCliPedd FOREIGN KEY (peddScl) REFERENCES tbPeddDPedd(peddScl)
);

CREATE TABLE tbAdsiMtvos(
	mtvoScl VARCHAR(4) NOT NULL,
	mtvoNbr VARCHAR(200) NOT NULL,
	mtvoTip VARCHAR(1) NOT NULL,
	mtvoAtv BOOLEAN NOT NULL DEFAULT TRUE,
	CONSTRAINT pk_AdsiMtvo PRIMARY KEY (mtvoScl)
);

INSERT INTO tbAdsiMtvos VALUES ('0001','El cliente se arrepintió y decidió cancelar','P',true);
INSERT INTO tbAdsiMtvos VALUES ('0002','El usuario cometió un error','P',true);
INSERT INTO tbAdsiMtvos VALUES ('0003','El propietario decidió cancelarlo','P',true);
INSERT INTO tbAdsiMtvos VALUES ('0099','Era un pedido generado por prueba del sistema','P',true);

INSERT INTO tbAdsiMtvos VALUES ('0100','SIN DESCUENTO','D',true);
INSERT INTO tbAdsiMtvos VALUES ('0101','Descuento del  5%','D',true);
INSERT INTO tbAdsiMtvos VALUES ('0102','Descuento del 10%','D',true);
INSERT INTO tbAdsiMtvos VALUES ('0103','Descuento del 15%','D',true);
INSERT INTO tbAdsiMtvos VALUES ('0104','Descuento del 20%','D',true);
INSERT INTO tbAdsiMtvos VALUES ('0105','Descuento del 25%','D',true);
INSERT INTO tbAdsiMtvos VALUES ('0106','Descuento del 30%','D',true);
INSERT INTO tbAdsiMtvos VALUES ('0107','Descuento del 50%','D',true);

INSERT INTO tbAdsiMtvos VALUES('0200','Factura con errores','A',true);
INSERT INTO tbAdsiMtvos VALUES('0201','Anulación autorizada por el propietario','A',true);
INSERT INTO tbAdsiMtvos VALUES('0202','Factura por prueba, MANTENIMIENTO','A',true);
INSERT INTO tbAdsiMtvos VALUES('0203','Anulación solicitada por cliente','A',true);

INSERT INTO tbAdsiMtvos VALUES('0300','SIN REGALO','G',true);
INSERT INTO tbAdsiMtvos VALUES('0301','Familiar del propietario','G',true);
INSERT INTO tbAdsiMtvos VALUES('0302','Amistad del propietario','G',true);
INSERT INTO tbAdsiMtvos VALUES('0303','Aprobado por el propietario','G',true);

INSERT INTO tbAdsiMtvos VALUES('0400','Reposición realizada por ','I',true);
INSERT INTO tbAdsiMtvos VALUES('0401','Dueño puso dinero','I',true);
INSERT INTO tbAdsiMtvos VALUES('0402','Pago de prestamo a ','I',true);

INSERT INTO tbAdsiMtvos VALUES('0500','Dueño hizo retiro','E',true);
INSERT INTO tbAdsiMtvos VALUES('0501','Pago de servicios básicos','E',true);
INSERT INTO tbAdsiMtvos VALUES('0502','Por prestamo a ','E',true);
INSERT INTO tbAdsiMtvos VALUES('0503','Para realizar compras varias...','E',true);
INSERT INTO tbAdsiMtvos VALUES('0504','Compra de gas','E',true);
INSERT INTO tbAdsiMtvos VALUES('0505','Pago de materiales de papelería','E',true);
INSERT INTO tbAdsiMtvos VALUES('0506','Pago de combustible','E',true);

INSERT INTO tbAdsiMtvos VALUES('0600','Cliente decidió eliminar el pedido','R',true);
INSERT INTO tbAdsiMtvos VALUES('0601','Cliente nunca llegó a retirar el pedido','R',true);
INSERT INTO tbAdsiMtvos VALUES('0602','Dueño(a) decidió eliminar el pedido','R',true);

	
CREATE TABLE tbPeddDVent(
	ventScl VARCHAR(10) NOT NULL,
	cajaCod VARCHAR(4) NOT NULL,
	usuaCod VARCHAR(4),
	ventFch DATE DEFAULT CURRENT_DATE,
	nidNid  VARCHAR(25), 
	ventTip VARCHAR(1) NOT NULL DEFAULT 'L' CHECK (ventTip IN ('L','D','C','E','R')),
	ventCrr VARCHAR(4) NOT NULL DEFAULT '0000',	
	ventCPt DOUBLE NOT NULL DEFAULT 0,
	ventCVl DOUBLE NOT NULL DEFAULT 0, 
	ventMsa VARCHAR(3),
	ventLta VARCHAR(1) NOT NULL DEFAULT 'A',
	peddScl VARCHAR(10) NOT NULL,
	ventHra TIME,
	ventSbt NUMERIC(14,6),
	ventDpc INT,
	ventDsc NUMERIC(14,6),
	ventIva NUMERIC(14,6),
	ventTot NUMERIC(14,6),
	ventRcb NUMERIC(14,6),
	ventVto NUMERIC(14,6),
	ventobv VARCHAR(200),
	ventAtv BOOLEAN NOT NULL DEFAULT TRUE,
	ventFac BOOLEAN NOT NULL DEFAULT FALSE,
	ventPgd BOOLEAN NOT NULL DEFAULT FALSE,
	ventStd VARCHAR(1) NOT NULL DEFAULT 'F' CHECK (ventStd IN ('F','P','A','E','S')),
	ventSep VARCHAR(1) NOT NULL DEFAULT 'C' CHECK (ventSep) IN ('C','S')),
	CONSTRAINT pk_PeddDVent PRIMARY KEY (ventScl),	
	CONSTRAINT fk_PeddDPeddAdsiUsua FOREIGN KEY (usuaCod) REFERENCES tbAdsiUsua(usuaCod),
	CONSTRAINT fk_PeddVentAdsiNid FOREIGN KEY (nidNid) REFERENCES tbAdsiNid(nidNid),
	CONSTRAINT fk_PeddVentAdsiCarrier FOREIGN KEY (ventCrr) REFERENCES tbAdsiCrrie(crriNid)
);	

COMMENT ON COLUMN tbPeddDVent.ventTip IS 'L:LOCAL, C:LLEVAR, D:DOMICILIO';
COMMENT ON COLUMN tbPeddDVent.ventDpc IS 'PORCENTAJE DEL DESCUENTO';
COMMENT ON COLUMN tbPeddDVent.ventCrr IS 'Codigo del carrier a utilizar';
COMMENT ON COLUMN tbPeddDVent.ventCpT IS 'Porcentaje a calcular por uso del carrier';
COMMENT ON COLUMN tbPeddDVent.ventCVl IS 'Valor a pagar por el carrier';
COMMENT ON COLUMN tbPeddDVent.ventStd IS 'Estado del pedido facturado => F: FACTURADO, P: FACTURADO Y PAGADO, A: ANULADO, E: ELIMINADO, S: SEPARADO ';
COMMENT ON COLUMN tbPeddDVent.ventSep IS 'C:COMPLETO, S: SEPARADO';

CREATE TABLE tbPeddMVent(
	ventScl VARCHAR(10) NOT NULL,
	itemCod VARCHAR(4) NOT NULL,
	itemCnt INT,
	itemPrc NUMERIC(10,6),
	itemIva NUMERIC(10,6),
	itemPto NUMERIC(10,6),
	CONSTRAINT fk_PeddDVentIvtrItem FOREIGN KEY (ventScl) REFERENCES tbPeddDVent(ventScl)	
);

CREATE TABLE tbPeddDFpgo(
	ventScl VARCHAR(10),
	fpgoTip VARCHAR(1) NOT NULL CHECK(fpgoTip IN ('E','T','C')),
	fpgoVal NUMERIC(14,6) NOT NULL,
	fpgoVto NUMERIC(14,6) NOT NULL,
	fpgoTjc VARCHAR(1) DEFAULT 'N' NOT NULL CHECK(fpgoTjc IN ('N','V','A','M','D','C')),
	fpgoTpt VARCHAR(1) DEFAULT 'N' NOT NULL CHECK(fpgoTpt IN ('N','C','D')),
	fpgoMss INT DEFAULT 0 CHECK (fpgoMss IN (0, 3, 6, 9)),
	fpgoBco VARCHAR(2) DEFAULT '',
	fpgoChe VARCHAR(6) DEFAULT ''
);

COMMENT ON COLUMN tbPeddDFpgo.fpgoTip IS 'Tipo de forma de pago E: EFECTIVO, T: TARJETA DE CRÉDITO, C:CHEQUE';
COMMENT ON COLUMN tbPeddDFpgo.fpgoTpt IS 'Tipo de pago con tarjeta N: NINGUNO, C: CORRIENTE, D: DIFERIDO';
COMMENT ON COLUMN tbPeddDFpgo.fpgoMss IS 'Meses a diferir pagos, 0: NINGUNO, 3 o 6 o 9 MESES';
COMMENT ON COLUMN tbPeddDFpgo.fpgoTip IS 'Tarjetas de crédito N: NINGUNA, V: VISA, A: AMERICAN, M: MASTERCARD, D:DISCOVER, C: DINNERS CLUB';
COMMENT ON COLUMN tbPeddDFpgo.fpgoBco IS 'Banco que emite el cheque BP: PICHINCHA, BB: BOLIVARIANO, BG:GUAYAQUIL, BD: PRODUCBANCO, BM:MACHALA, BA:AZUAY, PB:PACIFICO, BR:RUMIÑAHUI, BS:SOLIDARIO';
COMMENT ON COLUMN tbPeddDFpgo.fpgoChe IS 'Número de cheque';


CREATE TABLE tbPeddDGift(
	giftScl VARCHAR(10) NOT NULL,
	cajaCod VARCHAR(4) NOT NULL,
	usuaCod VARCHAR(4),
	giftFch DATE DEFAULT CURRENT_DATE,
	nidNid  VARCHAR(25), 
	giftTip VARCHAR(1) NOT NULL DEFAULT 'L' CHECK (giftTip IN ('L','D','C','E','R')),	
	giftMsa VARCHAR(3),
	giftLta VARCHAR(1) NOT NULL DEFAULT 'A',
	peddScl VARCHAR(10) NOT NULL,
	giftHra TIME,
	giftSbt NUMERIC(14,6),
	giftIva NUMERIC(14,6),
	giftTot NUMERIC(14,6),
	giftobv VARCHAR(200),
	giftAtv BOOLEAN NOT NULL DEFAULT TRUE,
	CONSTRAINT pk_PeddDGift PRIMARY KEY (giftScl),	
	CONSTRAINT fk_PeddDGiftAdsiUsua FOREIGN KEY (usuaCod) REFERENCES tbAdsiUsua(usuaCod),
	CONSTRAINT fk_PeddDGiftAdsiNid FOREIGN KEY (nidNid) REFERENCES tbAdsiNid(nidNid)
);

COMMENT ON COLUMN tbPeddDGift.giftTip IS 'L:LOCAL, C:LLEVAR, D:DOMICILIO, E: ENTREGA, R: CARRIER';

CREATE TABLE tbPeddMGift(
	giftScl VARCHAR(10) NOT NULL,
	itemCod VARCHAR(4) NOT NULL,
	itemCnt INT,
	itemPrc NUMERIC(10,6),
	itemIva NUMERIC(10,6),
	itemPto NUMERIC(10,6),
	CONSTRAINT fk_PeddDGiftIvtrItem FOREIGN KEY (giftScl) REFERENCES tbPeddDGift(giftScl)	
);

-- Tabla Historico de Anulaciones/Eliminacion de Facturas
CREATE TABLE tbPeddHVent(
	ventScl VARCHAR(10)	NOT NULL,
	histTip VARCHAR(1) NOT NULL DEFAULT 'A' CHECK (histTip IN ('A','E')),
	ventFch TIMESTAMP WITHOUT TIME ZONE NOT NULL,
	usuaCod VARCHAR(4) NOT NULL,
	ventNFc VARCHAR(20) NOT NULL,
	ventObv VARCHAR(200) NOT NULL,
	CONSTRAINT fk_PeddDHVentTip FOREIGN KEY (ventScl) REFERENCES tbPeddDVent(ventScl)	
);

COMMENT ON COLUMN tbPeddHVent.histTip IS 'Tipo de Histórico: A = Anulación, E = Eliminación';

CREATE TABLE tbHistCocina(
	peddscl VARCHAR(10),
	pedddet jsonb,
	usuacod VARCHAR(4),
	CONSTRAINT uk_peddscl_cocina UNIQUE(peddscl),
	CONSTRAINT fk_peddscl_pedddped FOREIGN KEY (peddscl) REFERENCES tbPeddDPedd(peddscl),
	CONSTRAINT fk_usacod_adsiusua FOREIGN KEY (usuacod) REFERENCES tbAdsiUsua(usuacod)
)

-- Tabla Separacion de Ventas(Facturas)
CREATE TABLE tbPeddSVent(
	ventscl VARCHAR(10),
	peddscl VARCHAR(10),
	ventstd VARCHAR(1) NOT NULL DEFAULT 'F' CHECK (ventStd IN ('F','P','A','E')),
	ventsbt NUMERIC(14,6),
	ventdsc NUMERIC(14,6),
	ventiva NUMERIC(14,6),
	venttot NUMERIC(14,6),
	ventrcb NUMERIC(14,6),
	ventvto NUMERIC(14,6),
	ventobv VARCHAR(200),
	CONSTRAINT fk_peddsvent_pedddvent FOREIGN KEY (ventscl) REFERENCES tbPedddVent(ventscl),
	CONSTRAINT fk_peddsvent_pedddpedd FOREIGN KEY (peddscl) REFERENCES tbPedddPedd(peddscl)
)

CREATE TABLE tbCajaVaIg(
	vaigSec VARCHAR(10) NOT NULL,
	vaigFch DATE NOT NULL,
	cajaCod VARCHAR(4),
	vaigDet VARCHAR(100),
	vaigVal NUMERIC(10,2) NOT NULL,
	usuaCod VARCHAR(4),
	vaigFgr DATE,
	usuaEli VARCHAR(4),
	vaigFel DATE,
	CONSTRAINT pk_cajavaig PRIMARY KEY(vaigSec),
	CONSTRAINT fk_cajavaigcaja FOREIGN KEY(cajacod) REFERENCES tbCajaCaja(cajaCod),
	CONSTRAINT fk_cajavaigusua FOREIGN KEY(usuaCod) REFERENCES tbAdsiUsua(usuaCod),
	CONSTRAINT fk_cajavaigusel FOREIGN KEY(usuaEli) REFERENCES tbAdsiUsua(usuaCod)
);

CREATE TABLE tbCajaVaEg(
	vaegSec VARCHAR(10) NOT NULL,
	vaegFch DATE NOT NULL,
	cajaCod VARCHAR(4),
	vaegDet VARCHAR(100),
	vaegVal NUMERIC(10,2) NOT NULL,
	usuaCod VARCHAR(4),
	vaegFgr DATE,
	usuaEli VARCHAR(4),
	vaegFel DATE,
	CONSTRAINT pk_cajavaeg PRIMARY KEY(vaegSec),
	CONSTRAINT fk_cajavaegcaja FOREIGN KEY(cajacod) REFERENCES tbCajaCaja(cajaCod),
	CONSTRAINT fk_cajavaegusua FOREIGN KEY(usuaCod) REFERENCES tbAdsiUsua(usuaCod),
	CONSTRAINT fk_cajavaegusel FOREIGN KEY(usuaEli) REFERENCES tbAdsiUsua(usuaCod)
);

CREATE TABLE tbCtrlCaja(
	ctrlSec VARCHAR(10) NOT NULL,
	ctrlFchIni DATE NOT NULL,
	ctrlFchFin DATE NOT NULL,
	cajaCod VARCHAR(4),
	ctrlHin TIME WITHOUT TIME ZONE,
	ctrlVin NUMERIC(10,2),
	ctrlHfn TIME WITHOUT TIME ZONE,
	ctrlVcr NUMERIC(10,2),
	ctrlVsi NUMERIC(10,2),
	ctrlDif NUMERIC(10,2),
	CONSTRAINT pk_ctrlCaja PRIMARY KEY(ctrlSec),
	CONSTRAINT fk_ctrlCajaCaja FOREIGN KEY(cajacod) REFERENCES tbCajaCaja(cajaCod),
	CONSTRAINT uk_ctrlCajaFecha UNIQUE (ctrlFchIni)
);

CREATE TABLE tbCtrlCierre(
	ctrlSec VARCHAR(10) NOT NULL,
	ctrlFchIni DATE NOT NULL,
	ctrlFchFin DATE,
	ctrlHin TIME WITHOUT TIME ZONE,
	ctrlVin NUMERIC(10,2),
	ctrlHfn TIME WITHOUT TIME ZONE,
	ctrlVcr NUMERIC(10,2),
	ctrlVsi NUMERIC(10,2),
	ctrlDif NUMERIC(10,2),
	CONSTRAINT pk_ctrlCajaCierre PRIMARY KEY(ctrlSec),
	CONSTRAINT uk_ctrlCajaFechaIni UNIQUE (ctrlFchIni)
);

CREATE TABLE tbPeddDRete(
	reteScl VARCHAR(10) NOT NULL,
	cajaCod VARCHAR(3) NOT NULL,
	usuaCod VARCHAR(3) NOT NULL,
	reteTip VARCHAR(1) NOT NULL DEFAULT 'C' CHECK (reteTip IN ('C','V')),
	reteFch DATE NOT NULL,
	nidNid VARCHAR(25) NOT NULL,
	docuTip VARCHAR(1) NOT NULL DEFAULT 'F',
	docuSri VARCHAR(7),
	docuNid VARCHAR(20),
	docuSer VARCHAR(10),
	reteTot NUMERIC(14,6) NOT NULL,
	CONSTRAINT pk_PeddDRete PRIMARY KEY (reteScl),
	CONSTRAINT fk_PeddDReteCaja FOREIGN KEY (cajaCod) REFERENCES tbCajaCaja (cajaCod),
	CONSTRAINT fk_PeddDReteUsua FOREIGN KEY (usuaCod) REFERENCES tbAdsiUsua (usuaCod)
);

COMMENT ON COLUMN tbPeddDRete.reteTip IS 'Tipo de retencion: C = COMPRA, V = VENTA';
COMMENT ON COLUMN tbPeddDRete.docuTip IS 'Tipo de Documento: F = Factura, N = Nota de Venta, L = Liquidaciones y prestación de servicios, T = Tickets, O = Otros';
COMMENT ON COLUMN tbPeddDRete.docuSri IS 'Serial del número del documento ###-###-########## son los primeros 6 dígitos, ejemplo 001-001';

CREATE TABLE tbPeddMRete(
	reteScl VARCHAR(10),
	anioFis INT,
	reteTip VARCHAR(1) NOT NULL DEFAULT 'I',
	reteSri VARCHAR(5),
	reteBas NUMERIC(10,2) NOT NULL,
	retePct DECIMAL(10,2) NOT NULL,
	reteVal DECIMAL(10,2) NOT NULL,
	CONSTRAINT fk_PeddMReteDRete FOREIGN KEY (reteScl) REFERENCES tbPeddDRete (reteScl)
);

CREATE TABLE tbReserv(
	reseNid VARCHAR(10) NOT NULL,
	nidNid  VARCHAR(20) NOT NULL,
	reseCnt INT NOT NULL,
	reseDes TIMESTAMP WITHOUT TIME ZONE NOT NULL,
	resehas TIMESTAMP WITHOUT TIME ZONE NOT NULL,
	reseMot VARCHAR(1) NOT NULL DEFAULT 'N',
	reseNot TEXT,
	reseMes TEXT NOT NULL,
	CONSTRAINT pk_Reserv PRIMARY KEY (reseNid),
	CONSTRAINT fk_ReservCliente FOREIGN KEY (nidNid) REFERENCES tbAdsiNid(nidNid)
);

COMMENT ON COLUMN tbReserv.reseMot IS 'Motivo de la reserva N=Ninguno, C=Cumpleaños, R=Romántico, L=Celebración, F=Festividad, P=Propuesta de matrimonio, M=Matrimonio';

CREATE TABLE TbFactElect(
	ventscl VARCHAR(10) NOT NULL,
	autosri VARCHAR(100),
	secuencial VARCHAR(9) NOT NULL,
	numefac VARCHAR(50),
	etdofac VARCHAR(1) NOT NULL DEFAULT 'N', -- N = NO PROCESADO, P = PROCESADO, A = AUTORIZADO, R = RECHAZADO
	etdoeml VARCHAR(1), -- Estado del envio de correo al cliente
	fechenv TIMESTAMP WITHOUT TIME ZONE,
	fechpro TIMESTAMP WITHOUT TIME ZONE,
	CONSTRAINT fk_FactElect_PeddDVent FOREIGN KEY (ventscl) REFERENCES tbPeddDVent(ventscl)
);

COMMENT ON COLUMN TbFactElect.etdofac IS 'Estados de la factura: N = NO PROCESADO, P = PROCESADO, A = AUTORIZADO, R = RECHAZADO';
COMMENT ON COLUMN TbFactElect.etdoeml IS 'Estados del envio de correo al cliente: N = NO ENVIADO, E = ENVIADO, I = DIRECCION EMAIL NO VALIDA';
