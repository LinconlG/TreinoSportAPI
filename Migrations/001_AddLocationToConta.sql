-- Migration 001: Add location fields to CONTA table
-- Run this script manually against the TreinoSport database

ALTER TABLE CONTA
    ADD Latitude  FLOAT NULL,
        Longitude FLOAT NULL,
        Cep       VARCHAR(9) NULL;
GO

-- Haversine formula function for distance calculation in km
CREATE OR ALTER FUNCTION dbo.fn_DistanciaKm (
    @lat1  FLOAT,
    @lon1  FLOAT,
    @lat2  FLOAT,
    @lon2  FLOAT
)
RETURNS FLOAT
AS
BEGIN
    DECLARE @R      FLOAT = 6371.0;  -- Earth radius in km
    DECLARE @dLat   FLOAT = RADIANS(@lat2 - @lat1);
    DECLARE @dLon   FLOAT = RADIANS(@lon2 - @lon1);
    DECLARE @a      FLOAT;
    DECLARE @c      FLOAT;
    DECLARE @dist   FLOAT;

    SET @a = SIN(@dLat / 2) * SIN(@dLat / 2)
           + COS(RADIANS(@lat1)) * COS(RADIANS(@lat2))
           * SIN(@dLon / 2) * SIN(@dLon / 2);

    SET @c = 2 * ATN2(SQRT(@a), SQRT(1 - @a));
    SET @dist = @R * @c;

    RETURN @dist;
END;
GO
