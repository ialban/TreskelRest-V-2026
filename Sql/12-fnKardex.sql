DROP PROCEDURE IF EXISTS fnKardex;	
CREATE OR REPLACE PROCEDURE fnKardex(IN producto VARCHAR(6), IN anio INT, IN mes INT, IN invalue NUMERIC(10,2), IN ouvalue NUMERIC(10,2))
LANGUAGE plpgsql
AS $$
DECLARE
	mes_anterior INT := mes-1;
	anio_anterior INT :=anio;
	
	krdxBegin NUMERIC(10,2);
	KrdxInput NUMERIC(10,2);
	KrdxOutpt NUMERIC(10,2);
	KrdxSaldo NUMERIC(10,2);	
BEGIN
SELECT  
	CASE mes
		WHEN  1 THEN krdxBEn
		WHEN  2 THEN krdxBFe
		WHEN  3 THEN krdxBMa
		WHEN  4 THEN krdxBAb
		WHEN  5 THEN krdxBMy
		WHEN  6 THEN krdxBJu
		WHEN  7 THEN krdxBJl
		WHEN  8 THEN krdxBAg
		WHEN  9 THEN krdxBSp
		WHEN 10 THEN krdxBOc
		WHEN 11 THEN krdxBNv
		WHEN 12 THEN krdxBDi
	END,
	CASE mes
		WHEN  1 THEN krdxIEn
		WHEN  2 THEN krdxIFe
		WHEN  3 THEN krdxIMa
		WHEN  4 THEN krdxIAb
		WHEN  5 THEN krdxIMy
		WHEN  6 THEN krdxIJu
		WHEN  7 THEN krdxIJl
		WHEN  8 THEN krdxIAg
		WHEN  9 THEN krdxISp
		WHEN 10 THEN krdxIOc
		WHEN 11 THEN krdxINv
		WHEN 12 THEN krdxIDi
	END,
	CASE mes
		WHEN  1 THEN krdxEEn
		WHEN  2 THEN krdxEFe
		WHEN  3 THEN krdxEMa
		WHEN  4 THEN krdxEAb
		WHEN  5 THEN krdxEMy
		WHEN  6 THEN krdxEJu
		WHEN  7 THEN krdxEJl
		WHEN  8 THEN krdxEAg
		WHEN  9 THEN krdxESp
		WHEN 10 THEN krdxEOc
		WHEN 11 THEN krdxENv
		WHEN 12 THEN krdxEDi
	END INTO KrdxBegin, KrdxInput, KrdxOutpt
FROM tbIvtrKdrxMT where prdCod=producto and krdxani=anio;

RAISE NOTICE 'begin %', KrdxBegin;

IF NOT EXISTS(SELECT * FROM tbIvtrKdrxMT where prdCod=producto and krdxani=anio) THEN    
	SELECT  	
		CASE (mes_anterior)
			WHEN  1 THEN krdxBEn
			WHEN  2 THEN krdxBFe
			WHEN  3 THEN krdxBMa
			WHEN  4 THEN krdxBAb
			WHEN  5 THEN krdxBMy
			WHEN  6 THEN krdxBJu
			WHEN  7 THEN krdxBJl
			WHEN  8 THEN krdxBAg
			WHEN  9 THEN krdxBSp
			WHEN 10 THEN krdxBOc
			WHEN 11 THEN krdxBNv
			WHEN 12 THEN krdxBDi
		END INTO KrdxSaldo
	FROM tbIvtrKdrxMT where prdCod=producto and krdxani=anio;
	
	IF KrdxSaldo IS NULL THEN
		KrdxBegin := 0;
	ELSE
		KrdxBegin := KrdxSaldo;
	END IF;
	
	KrdxInput := COALESCE(KrdxInput, 0) + invalue;
	KrdxOutpt := COALESCE(KrdxOutpt, 0) + ouvalue;
	
	RAISE NOTICE 'CREANDO REGISTRO';
	
	IF mes = 1 THEN
		INSERT INTO tbIvtrKdrxMT (prdcod, krdxani, KrdxBEn, KrdxIEn, KrdxEEn, KrdxSEn)
	    VALUES(producto, anio, KrdxBegin,KrdxInput,KrdxOutpt,KrdxInput - KrdxOutpt);
	END IF;
	
	IF mes = 2 THEN
		INSERT INTO tbIvtrKdrxMT (prdcod, krdxani, KrdxBFe, KrdxIFe, KrdxEFe, KrdxSFe)
	    VALUES(producto, anio, KrdxBegin,KrdxInput,KrdxOutpt,KrdxInput - KrdxOutpt);
	END IF;
	
	IF mes = 3 THEN
		INSERT INTO tbIvtrKdrxMT (prdcod, krdxani, KrdxBMa, KrdxIMa, KrdxEMa, KrdxSMa)
	    VALUES(producto, anio, KrdxBegin,KrdxInput,KrdxOutpt,KrdxInput - KrdxOutpt);
	END IF;
	
	IF mes = 4 THEN
		INSERT INTO tbIvtrKdrxMT (prdcod, krdxani, KrdxBAb, KrdxIAb, KrdxEAb, KrdxSAb)
	    VALUES(producto, anio, KrdxBegin,KrdxInput,KrdxOutpt,KrdxInput - KrdxOutpt);
	END IF;
	
	IF mes = 5 THEN
		INSERT INTO tbIvtrKdrxMT (prdcod, krdxani, KrdxBMy, KrdxIMy, KrdxEMy, KrdxSMy)
	    VALUES(producto, anio, KrdxBegin,KrdxInput,KrdxOutpt,KrdxInput - KrdxOutpt);
	END IF;
	
	IF mes = 6 THEN
		INSERT INTO tbIvtrKdrxMT (prdcod, krdxani, KrdxBJu, KrdxJu, KrdxEJu, KrdxSJu)
	    VALUES(producto, anio, KrdxBegin,KrdxInput,KrdxOutpt,KrdxInput - KrdxOutpt);
	END IF;
	
	IF mes = 7 THEN
		INSERT INTO tbIvtrKdrxMT (prdcod, krdxani, KrdxBJl, KrdxIJl, KrdxEJl, KrdxSJl)
	    VALUES(producto, anio, KrdxBegin,KrdxInput,KrdxOutpt,KrdxInput - KrdxOutpt);
	END IF;
	
	IF mes = 8 THEN
		INSERT INTO tbIvtrKdrxMT (prdcod, krdxani, KrdxBAg, KrdxIAg, KrdxEAg, KrdxSAg)
	    VALUES(producto, anio, KrdxBegin,KrdxInput,KrdxOutpt,KrdxInput - KrdxOutpt);
	END IF;
	
	IF mes = 9 THEN
		INSERT INTO tbIvtrKdrxMT (prdcod, krdxani, KrdxBSp, KrdxISp, KrdxESp, KrdxSSp)
	    VALUES(producto, anio, KrdxBegin,KrdxInput,KrdxOutpt,KrdxInput - KrdxOutpt);
	END IF;
	
	IF mes = 10 THEN
		INSERT INTO tbIvtrKdrxMT (prdcod, krdxani, KrdxBOc, KrdxIOc, KrdxEOc, KrdxSOc)
	    VALUES(producto, anio, KrdxBegin,KrdxInput,KrdxOutpt,KrdxInput - KrdxOutpt);
	END IF;
		
	IF mes = 11 THEN
		INSERT INTO tbIvtrKdrxMT (prdcod, krdxani, KrdxBNv, KrdxINv, KrdxENv, KrdxSNv)
	    VALUES(producto, anio, KrdxBegin,KrdxInput,KrdxOutpt,KrdxInput - KrdxOutpt);
	END IF;
	
	IF mes = 12 THEN
		INSERT INTO tbIvtrKdrxMT (prdcod, krdxani, KrdxBEDi, KrdxIDi, KrdxEDi, KrdxSDi)
	    VALUES(producto, anio, KrdxBegin,KrdxInput,KrdxOutpt,KrdxInput - KrdxOutpt);
	END IF;
ELSE	
	-- SE BUSCA EN EL MES ANTERIOR
	IF KrdxBegin IS NULL THEN
		RAISE NOTICE 'BUSCA EN EL MES ANTERIOR';
		
		IF mes = 1 THEN
			mes_anterior = 12;
			anio_anterior = anio-1;
		ELSE
			mes_anterior = mes -1;
			anio_anterior = anio;
		END IF;
		
		SELECT  
			CASE (mes_anterior)
				WHEN  1 THEN krdxSEn
				WHEN  2 THEN krdxSFe
				WHEN  3 THEN krdxSMa
				WHEN  4 THEN krdxSAb
				WHEN  5 THEN krdxSMy
				WHEN  6 THEN krdxSJu
				WHEN  7 THEN krdxSJl
				WHEN  8 THEN krdxSAg
				WHEN  9 THEN krdxSSp
				WHEN 10 THEN krdxSOc
				WHEN 11 THEN krdxSNv
				WHEN 12 THEN krdxSDi
			END INTO KrdxSaldo
		FROM tbIvtrKdrxMT where prdCod=producto and krdxani=anio_anterior;

		IF KrdxSaldo IS NULL THEN
			KrdxBegin := 0;
		ELSE
			RAISE  NOTICE 'ACTUALIZANDO VALOR DE INICIO';
			KrdxBegin := KrdxSaldo;			
		END IF;	
	END IF;

	RAISE NOTICE 'IN % VAL %', KrdxInput, invalue;
	KrdxInput := COALESCE(KrdxInput, 0) + invalue;
	RAISE NOTICE 'IN % VAL %', KrdxInput, invalue;
	KrdxOutpt := COALESCE(KrdxOutpt, 0) + ouvalue;
	RAISE NOTICE 'OU % VAL %', KrdxOutpt, ouvalue;
	
	RAISE NOTICE 'ACTUALIZANDO REGISTRO';
	RAISE NOTICE 'B = % I = % O = % S = %', KrdxBegin, KrdxInput, KrdxOutpt, KrdxSaldo;
	
	IF mes = 1 THEN
		UPDATE tbIvtrKdrxMT 
		SET 
			KrdxBEn = KrdxBegin,
			KrdxIEn = KrdxInput,
			KrdxEEn = KrdxOutpt,
			KrdxSEn = KrdxBegin + KrdxInput - KrdxOutpt	    
		WHERE prdCod = producto AND krdxani=anio;	
	END IF;
	
	IF mes = 2 THEN
		UPDATE tbIvtrKdrxMT 
		SET 
			KrdxBFe = KrdxBegin,
			KrdxIFe = KrdxInput,
			KrdxEFe = KrdxOutpt,
			KrdxSFe = KrdxBegin + KrdxInput - KrdxOutpt	    
		WHERE prdCod = producto AND krdxani=anio;	
	END IF;
	
	IF mes = 3 THEN
		UPDATE tbIvtrKdrxMT 
		SET 
			KrdxBMa = KrdxBegin,
			KrdxIMa = KrdxInput,
			KrdxEMa = KrdxOutpt,
			KrdxSMa = KrdxBegin + KrdxInput - KrdxOutpt	    
		WHERE prdCod = producto AND krdxani=anio;	
	END IF;
	
	IF mes = 4 THEN
		UPDATE tbIvtrKdrxMT 
		SET 
			KrdxBAb = KrdxBegin,
			KrdxIAb = KrdxInput,
			KrdxEAb = KrdxOutpt,
			KrdxSAb = KrdxBegin + KrdxInput - KrdxOutpt	    
		WHERE prdCod = producto AND krdxani=anio;	
	END IF;
	
	IF mes = 5 THEN
		UPDATE tbIvtrKdrxMT 
		SET 
			KrdxBMy = KrdxBegin,
			KrdxIMy = KrdxInput,
			KrdxEMy = KrdxOutpt,
			KrdxSMy = KrdxBegin + KrdxInput - KrdxOutpt	    
		WHERE prdCod = producto AND krdxani=anio;	
	END IF;
	
	IF mes = 6 THEN
		UPDATE tbIvtrKdrxMT 
		SET 
			KrdxBJu = KrdxBegin,
			KrdxIJu = KrdxInput,
			KrdxEJu = KrdxOutpt,
			KrdxSJu = KrdxBegin + KrdxInput - KrdxOutpt	    
		WHERE prdCod = producto AND krdxani=anio;	
	END IF;
	
	IF mes = 7 THEN
		UPDATE tbIvtrKdrxMT 
		SET 
			KrdxBJl = KrdxBegin,
			KrdxIJl = KrdxInput,
			KrdxEJl = KrdxOutpt,
			KrdxSJl = KrdxBegin + KrdxInput - KrdxOutpt	    
		WHERE prdCod = producto AND krdxani=anio;	
	END IF;
	
	IF mes = 8 THEN
		UPDATE tbIvtrKdrxMT 
		SET 
			KrdxBAg = KrdxBegin,
			KrdxIAg = KrdxInput,
			KrdxEAg = KrdxOutpt,
			KrdxSAg = KrdxBegin + KrdxInput - KrdxOutpt	    
		WHERE prdCod = producto AND krdxani=anio;	
	END IF;
	
	IF mes = 9 THEN
		UPDATE tbIvtrKdrxMT 
		SET 
			KrdxBSp = KrdxBegin,
			KrdxISp = KrdxInput,
			KrdxESp = KrdxOutpt,
			KrdxSSp = KrdxBegin + KrdxInput - KrdxOutpt	    
		WHERE prdCod = producto AND krdxani=anio;	
	END IF;
	
	IF mes = 10 THEN
		UPDATE tbIvtrKdrxMT 
		SET 
			KrdxBOc = KrdxBegin,
			KrdxIOc = KrdxInput,
			KrdxEOc = KrdxOutpt,
			KrdxSOc = KrdxBegin + KrdxInput - KrdxOutpt	    
		WHERE prdCod = producto AND krdxani=anio;	
	END IF;
	
	IF mes = 11 THEN
		UPDATE tbIvtrKdrxMT 
		SET 
			KrdxBNv = KrdxBegin,
			KrdxINv = KrdxInput,
			KrdxENv = KrdxOutpt,
			KrdxSNv = KrdxBegin + KrdxInput - KrdxOutpt	    
		WHERE prdCod = producto AND krdxani=anio;	
	END IF;
	
	IF mes = 12 THEN
		UPDATE tbIvtrKdrxMT 
		SET 
			KrdxBDi = KrdxBegin,
			KrdxIDi = KrdxInput,
			KrdxEDi = KrdxOutpt,
			KrdxSDi = KrdxBegin + KrdxInput - KrdxOutpt	    
		WHERE prdCod = producto AND krdxani=anio;	
	END IF;
END IF;
END;
$$