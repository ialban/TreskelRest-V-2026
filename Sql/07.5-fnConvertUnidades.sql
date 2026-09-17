DROP FUNCTION IF EXISTS fnConvertUnidades;
CREATE OR REPLACE FUNCTION fnConvertUnidades(IN valor NUMERIC(10,2), IN unidDst VARCHAR(1), IN unidOri VARCHAR(1))
RETURNS NUMERIC(10,2)
AS $$
DECLARE 
	RETORNO NUMERIC(10,2) := valor;
BEGIN
	IF unidOri = 'U' THEN
		RETORNO := valor;
	END IF;
	IF unidOri = 'P' THEN
		RETORNO := valor;
	END IF;
	IF unidOri = 'K' THEN
		IF unidDst = 'K' THEN
			RETORNO := valor;
		END IF;
		IF unidDst = 'L' THEN
			RETORNO := valor*0.453592;
		END IF;
		IF unidDst = 'G' THEN
			RETORNO := valor*0.001;
		END IF;
		IF unidDst = 'O' THEN
			RETORNO := valor*0.0283495;
		END IF;
		IF unidDst = 'Q' THEN
			RETORNO := valor*100;
		END IF;
	END IF;
	IF unidOri = 'L' THEN
		IF unidDst = 'K' THEN
			RETORNO := valor*2.20462;
		END IF;
		IF unidDst = 'L' THEN
			RETORNO := valor;
		END IF;
		IF unidDst = 'G' THEN
			RETORNO := valor*0.00220462;
		END IF;
		IF unidDst = 'O' THEN
			RETORNO := valor*0.0625;
		END IF;
		IF unidDst = 'Q' THEN
			RETORNO := valor*220.462;
		END IF;
	END IF;
	IF unidOri = 'O' THEN					
		IF unidDst = 'K' THEN
			RETORNO := valor*35.274;
		END IF;
		IF unidDst = 'L' THEN
			RETORNO := valor*16;
		END IF;
		IF unidDst = 'G' THEN
			RETORNO := valor*0.035274;
		END IF;
		IF unidDst = 'O' THEN
			RETORNO := valor;
		END IF;
		IF unidDst = 'Q' THEN
			RETORNO := valor*3527.4;
		END IF;
	END IF;
	IF unidOri = 'G' THEN
		IF unidDst = 'K' THEN
			RETORNO := valor*1000;
		END IF;
		IF unidDst = 'L' THEN
			RETORNO := valor*453.592;
		END IF;
		IF unidDst = 'G' THEN
			RETORNO := valor;
		END IF;
		IF unidDst = 'O' THEN
			RETORNO := valor*28.3495;
		END IF;
		IF unidDst = 'Q' THEN
			RETORNO := valor*100000;
		END IF;
	END IF;
	IF unidOri = 'Q' THEN
		IF unidDst = 'K' THEN
			RETORNO := valor*0.01;
		END IF;
		IF unidDst = 'L' THEN
			RETORNO := valor*0.00453592;
		END IF;
		IF unidDst = 'G' THEN
			RETORNO := valor/100000;
		END IF;
		IF unidDst = 'O' THEN
			RETORNO := valor*0.000283495;
		END IF;
		IF unidDst = 'Q' THEN
			RETORNO := valor;
		END IF;
	END IF;
	
	RETURN RETORNO;
END;
$$
LANGUAGE plpgsql;