CREATE DATABASE [BingoWeb]
GO

USE [BingoWeb]

CREATE TABLE [dbo].[HistorialBolillero](
	[IdHistorialCarton] [int] NULL,
	[FechaHora] [datetime] NULL,
	[numeroBolilla] [int] NULL
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[HistorialCartones](
	[IdHistorialCarton] [int] IDENTITY(1,1) NOT NULL,
	[FechaHora] [datetime] NULL,
	[Carton1] [int] NULL,
	[Carton2] [int] NULL,
	[Carton3] [int] NULL,
	[Carton4] [int] NULL
) 
GO