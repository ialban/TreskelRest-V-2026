-- Database: trsklrest

-- DROP DATABASE trsklrest;

CREATE DATABASE trsklrest
    WITH 
    OWNER = trsklusr
    ENCODING = 'UTF8'
    LC_COLLATE = 'Spanish_Spain.1252'
    LC_CTYPE = 'Spanish_Spain.1252'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;

CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE EXTENSION IF NOT EXISTS xml2;

CREATE OR REPLACE FUNCTION encryptar(mensaje text)
RETURNS VARCHAR AS 
$$
DECLARE
    KEY TEXT= 'Tr3sk3lS0ft';
	DATO TEXT;
BEGIN    
    SELECT encode(encrypt(mensaje::bytea, KEY::bytea,'aes'),'base64') into DATO;
	RETURN DATO;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION desencryptar(mensaje text)
RETURNS VARCHAR AS 
$$
DECLARE
    KEY TEXT= 'Tr3sk3lS0ft';
	DATO TEXT;
BEGIN    
	select convert_from(decrypt(decode(mensaje,'base64')::bytea,KEY::bytea,'aes'),'utf-8') INTO DATO;
	RETURN DATO;
END;
$$
LANGUAGE plpgsql;

