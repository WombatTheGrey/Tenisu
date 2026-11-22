CREATE USER TenisuWebApp
FROM EXTERNAL PROVIDER;


-- Allow reading
ALTER ROLE db_datareader
ADD MEMBER TenisuWebApp;

-- Allow writing
ALTER ROLE db_datawriter
ADD MEMBER TenisuWebApp;


GRANT CREATE TABLE TO TenisuWebApp;
GRANT ALTER ON SCHEMA::dbo TO TenisuWebApp;
GRANT REFERENCES ON SCHEMA::dbo TO TenisuWebApp;