-- Crea la base BDInmobiliaria con las tablas y los datos de ejemplo de la cátedra.
-- Cómo correrlo: ver BD.md en esta misma carpeta.
-- Para regenerarlo desde una base actualizada, ver BD.md.
:on error exit
SET NOCOUNT ON;
GO
IF DB_ID(N'BDInmobiliaria') IS NOT NULL
	RAISERROR(N'La base BDInmobiliaria ya existe. Si querés recrearla, primero borrala (ver BD.md).', 16, 1);
GO
CREATE DATABASE [BDInmobiliaria];
GO
USE [BDInmobiliaria];
GO
CREATE TABLE [dbo].[Conectados](
	[Usuario] [varchar](50) NOT NULL,
	[Nombre] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)
GO
CREATE TABLE [dbo].[Pasatiempos](
	[Id] [int] IDENTITY(10,1) NOT NULL,
	[Nombre] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)
GO
CREATE TABLE [dbo].[Inquilinos](
	[IdInquilino] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](50) NULL,
	[Apellido] [varchar](50) NULL,
	[Dni] [varchar](10) NULL,
	[Telefono] [varchar](15) NULL,
	[Email] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[IdInquilino] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)
GO
CREATE TABLE [dbo].[Usuarios](
	[Id] [int] IDENTITY(10,1) NOT NULL,
	[Nombre] [varchar](50) NOT NULL,
	[Apellido] [varchar](50) NOT NULL,
	[Avatar] [varchar](50) NULL,
	[Email] [varchar](50) NOT NULL,
	[Clave] [varchar](200) NOT NULL,
	[Rol] [int] NOT NULL
)
GO
CREATE TABLE [dbo].[Propietarios](
	[IdPropietario] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](50) NULL,
	[Apellido] [varchar](50) NULL,
	[Dni] [varchar](10) NULL,
	[Telefono] [varchar](15) NULL,
	[Email] [varchar](50) NULL,
	[Clave] [varchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[IdPropietario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)
GO
CREATE TABLE [dbo].[Personas](
	[Id] [int] IDENTITY(10,1) NOT NULL,
	[Nombre] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)
GO
CREATE TABLE [dbo].[PersonaPasatiempos](
	[Id] [int] IDENTITY(10,1) NOT NULL,
	[PersonaId] [int] NOT NULL,
	[PasatiempoId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)
GO
CREATE TABLE [dbo].[Inmuebles](
	[Id] [int] IDENTITY(10,1) NOT NULL,
	[Direccion] [varchar](50) NULL,
	[Cupo] [int] NOT NULL,
	[Latitud] [decimal](11, 8) NULL,
	[Longitud] [decimal](12, 8) NULL,
	[PropietarioId] [int] NULL,
	[Portada] [varchar](100) NULL,
	[Habilitado] [bit] NOT NULL,
	[PrecioPorDia] [decimal](12, 2) NOT NULL,
	[PorcentajeReserva] [decimal](5, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)
GO
CREATE TABLE [dbo].[Imagenes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InmuebleId] [int] NOT NULL,
	[Url] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
)
GO
SET IDENTITY_INSERT [dbo].[Pasatiempos] ON
GO
INSERT [dbo].[Pasatiempos] ([Id], [Nombre]) VALUES (10, N'Programar')
GO
INSERT [dbo].[Pasatiempos] ([Id], [Nombre]) VALUES (11, N'Correr')
GO
SET IDENTITY_INSERT [dbo].[Pasatiempos] OFF
GO
SET IDENTITY_INSERT [dbo].[Inquilinos] ON
GO
INSERT [dbo].[Inquilinos] ([IdInquilino], [Nombre], [Apellido], [Dni], [Telefono], [Email]) VALUES (1, N'Carla', N'Medina', N'35218844', N'2664900112', N'carla.medina@mail.com')
GO
INSERT [dbo].[Inquilinos] ([IdInquilino], [Nombre], [Apellido], [Dni], [Telefono], [Email]) VALUES (2, N'Nicolas', N'Arrieta', N'29876123', N'2664455780', N'nicolas.arrieta@mail.com')
GO
INSERT [dbo].[Inquilinos] ([IdInquilino], [Nombre], [Apellido], [Dni], [Telefono], [Email]) VALUES (3, N'Sofia', N'Quiroga', N'37654201', N'2665331209', N'sofia.quiroga@mail.com')
GO
SET IDENTITY_INSERT [dbo].[Inquilinos] OFF
GO
SET IDENTITY_INSERT [dbo].[Usuarios] ON
GO
INSERT [dbo].[Usuarios] ([Id], [Nombre], [Apellido], [Avatar], [Email], [Clave], [Rol]) VALUES (10, N'Mariano', N'Luzza', N'/Uploads/avatar_10.png', N'mluzza@ulp.edu.ar', N'GAKKw6Co5EiIGNiZC1OfQC6offL+e8CoEs3SX0LIrHA=', 1)
GO
INSERT [dbo].[Usuarios] ([Id], [Nombre], [Apellido], [Avatar], [Email], [Clave], [Rol]) VALUES (11, N'Pepe', N'Perez', N'/Uploads\avatar_11.png', N'a@a.com', N'GAKKw6Co5EiIGNiZC1OfQC6offL+e8CoEs3SX0LIrHA=', 2)
GO
INSERT [dbo].[Usuarios] ([Id], [Nombre], [Apellido], [Avatar], [Email], [Clave], [Rol]) VALUES (1011, N'Ana', N'Gimenez', NULL, N'123@123.com', N'GAKKw6Co5EiIGNiZC1OfQC6offL+e8CoEs3SX0LIrHA=', 2)
GO
INSERT [dbo].[Usuarios] ([Id], [Nombre], [Apellido], [Avatar], [Email], [Clave], [Rol]) VALUES (2011, N'Diego', N'Roldan', N'/Uploads\avatar_2011.png', N'b@a.com', N'GAKKw6Co5EiIGNiZC1OfQC6offL+e8CoEs3SX0LIrHA=', 2)
GO
SET IDENTITY_INSERT [dbo].[Usuarios] OFF
GO
SET IDENTITY_INSERT [dbo].[Propietarios] ON
GO
INSERT [dbo].[Propietarios] ([IdPropietario], [Nombre], [Apellido], [Dni], [Telefono], [Email], [Clave]) VALUES (1, N'Mariano', N'Luzza', N'31047158', N'2664501122', N'mluzza@ulp.edu.ar', N'GAKKw6Co5EiIGNiZC1OfQC6offL+e8CoEs3SX0LIrHA=')
GO
INSERT [dbo].[Propietarios] ([IdPropietario], [Nombre], [Apellido], [Dni], [Telefono], [Email], [Clave]) VALUES (2, N'Lucia', N'Ferreyra', N'28114390', N'2664778811', N'lucia.ferreyra@mail.com', N'GAKKw6Co5EiIGNiZC1OfQC6offL+e8CoEs3SX0LIrHA=')
GO
INSERT [dbo].[Propietarios] ([IdPropietario], [Nombre], [Apellido], [Dni], [Telefono], [Email], [Clave]) VALUES (3, N'Ramiro', N'Ojeda', N'33902145', N'2664336699', N'ramiro.ojeda@mail.com', N'GAKKw6Co5EiIGNiZC1OfQC6offL+e8CoEs3SX0LIrHA=')
GO
INSERT [dbo].[Propietarios] ([IdPropietario], [Nombre], [Apellido], [Dni], [Telefono], [Email], [Clave]) VALUES (4, N'Valeria', N'Sosa', N'30455712', N'2665120044', N'valeria.sosa@mail.com', N'GAKKw6Co5EiIGNiZC1OfQC6offL+e8CoEs3SX0LIrHA=')
GO
INSERT [dbo].[Propietarios] ([IdPropietario], [Nombre], [Apellido], [Dni], [Telefono], [Email], [Clave]) VALUES (5, N'Señor', N'Propietario', N'11222333', N'542664001122', N'senior@gmail.com', N'GAKKw6Co5EiIGNiZC1OfQC6offL+e8CoEs3SX0LIrHA=')
GO
INSERT [dbo].[Propietarios] ([IdPropietario], [Nombre], [Apellido], [Dni], [Telefono], [Email], [Clave]) VALUES (6, N'Borrame', N'Borrado', N'10123456', N'541155004400', N'delete@email.com', N'GAKKw6Co5EiIGNiZC1OfQC6offL+e8CoEs3SX0LIrHA=')
GO
SET IDENTITY_INSERT [dbo].[Propietarios] OFF
GO
SET IDENTITY_INSERT [dbo].[Personas] ON
GO
INSERT [dbo].[Personas] ([Id], [Nombre]) VALUES (10, N'Juan')
GO
INSERT [dbo].[Personas] ([Id], [Nombre]) VALUES (11, N'Juancito')
GO
INSERT [dbo].[Personas] ([Id], [Nombre]) VALUES (12, N'Juanon')
GO
SET IDENTITY_INSERT [dbo].[Personas] OFF
GO
SET IDENTITY_INSERT [dbo].[PersonaPasatiempos] ON
GO
INSERT [dbo].[PersonaPasatiempos] ([Id], [PersonaId], [PasatiempoId]) VALUES (10, 10, 10)
GO
INSERT [dbo].[PersonaPasatiempos] ([Id], [PersonaId], [PasatiempoId]) VALUES (11, 10, 11)
GO
SET IDENTITY_INSERT [dbo].[PersonaPasatiempos] OFF
GO
SET IDENTITY_INSERT [dbo].[Inmuebles] ON
GO
INSERT [dbo].[Inmuebles] ([Id], [Direccion], [Cupo], [Latitud], [Longitud], [PropietarioId], [Portada], [Habilitado], [PrecioPorDia], [PorcentajeReserva]) VALUES (10, N'Av. Illia 455, San Luis', 3, CAST(-33.30000000 AS Decimal(11, 8)), CAST(-66.34000000 AS Decimal(12, 8)), 1, N'/Uploads/Inmuebles/portada_10.jpg', 1, CAST(550.00 AS Decimal(12, 2)), CAST(30.00 AS Decimal(5, 2)))
GO
INSERT [dbo].[Inmuebles] ([Id], [Direccion], [Cupo], [Latitud], [Longitud], [PropietarioId], [Portada], [Habilitado], [PrecioPorDia], [PorcentajeReserva]) VALUES (11, N'Junin 1240, San Luis', 2, CAST(-33.30210000 AS Decimal(11, 8)), CAST(-66.34120000 AS Decimal(12, 8)), 1, NULL, 1, CAST(19500.00 AS Decimal(12, 2)), CAST(25.00 AS Decimal(5, 2)))
GO
INSERT [dbo].[Inmuebles] ([Id], [Direccion], [Cupo], [Latitud], [Longitud], [PropietarioId], [Portada], [Habilitado], [PrecioPorDia], [PorcentajeReserva]) VALUES (12, N'Los Alamos 87, Potrero de los Funes', 6, CAST(-33.20450000 AS Decimal(11, 8)), CAST(-66.24380000 AS Decimal(12, 8)), 2, NULL, 1, CAST(46000.00 AS Decimal(12, 2)), CAST(40.00 AS Decimal(5, 2)))
GO
INSERT [dbo].[Inmuebles] ([Id], [Direccion], [Cupo], [Latitud], [Longitud], [PropietarioId], [Portada], [Habilitado], [PrecioPorDia], [PorcentajeReserva]) VALUES (13, N'Belgrano 723, Villa Mercedes', 3, CAST(-33.67510000 AS Decimal(11, 8)), CAST(-65.46020000 AS Decimal(12, 8)), 3, NULL, 1, CAST(22000.00 AS Decimal(12, 2)), CAST(30.00 AS Decimal(5, 2)))
GO
INSERT [dbo].[Inmuebles] ([Id], [Direccion], [Cupo], [Latitud], [Longitud], [PropietarioId], [Portada], [Habilitado], [PrecioPorDia], [PorcentajeReserva]) VALUES (14, N'Ruta 20 km 12, El Trapiche', 8, CAST(-33.10870000 AS Decimal(11, 8)), CAST(-66.06340000 AS Decimal(12, 8)), 4, NULL, 0, CAST(61000.00 AS Decimal(12, 2)), CAST(50.00 AS Decimal(5, 2)))
GO
INSERT [dbo].[Inmuebles] ([Id], [Direccion], [Cupo], [Latitud], [Longitud], [PropietarioId], [Portada], [Habilitado], [PrecioPorDia], [PorcentajeReserva]) VALUES (15, N'Calle Falsa 123', 5, CAST(1.00000000 AS Decimal(11, 8)), CAST(2.00000000 AS Decimal(12, 8)), 1, NULL, 1, CAST(1500.00 AS Decimal(12, 2)), CAST(10.00 AS Decimal(5, 2)))
GO
SET IDENTITY_INSERT [dbo].[Inmuebles] OFF
GO
SET IDENTITY_INSERT [dbo].[Imagenes] ON
GO
INSERT [dbo].[Imagenes] ([Id], [InmuebleId], [Url]) VALUES (1, 10, N'/Uploads/Inmuebles/10/c6ba6250-1774-45bb-97cd-13acff7e7429.jpg')
GO
INSERT [dbo].[Imagenes] ([Id], [InmuebleId], [Url]) VALUES (2, 10, N'/Uploads/Inmuebles/10/0537b870-0160-428e-b7ae-a2461947072b.jpg')
GO
INSERT [dbo].[Imagenes] ([Id], [InmuebleId], [Url]) VALUES (3, 10, N'/Uploads/Inmuebles/10/7133305d-5d31-411e-a125-4b1763e028b4.jpg')
GO
INSERT [dbo].[Imagenes] ([Id], [InmuebleId], [Url]) VALUES (6, 10, N'/Uploads/Inmuebles/10/67790a2b-890f-4d60-bb04-f57e6f1b44ca.jpg')
GO
INSERT [dbo].[Imagenes] ([Id], [InmuebleId], [Url]) VALUES (7, 10, N'/Uploads/Inmuebles/10/d36811a3-676e-43a6-bb79-660d8116e613.jpg')
GO
SET IDENTITY_INSERT [dbo].[Imagenes] OFF
GO
ALTER TABLE [dbo].[Inmuebles] ADD  DEFAULT ((1)) FOR [Habilitado]
GO
ALTER TABLE [dbo].[Inmuebles] ADD  CONSTRAINT [DF_Inmuebles_PrecioPorDia]  DEFAULT ((0)) FOR [PrecioPorDia]
GO
ALTER TABLE [dbo].[Inmuebles] ADD  CONSTRAINT [DF_Inmuebles_PorcentajeReserva]  DEFAULT ((0)) FOR [PorcentajeReserva]
GO
ALTER TABLE [dbo].[PersonaPasatiempos]  WITH CHECK ADD  CONSTRAINT [FK_PersonaPasatiempos_PasatiempoId] FOREIGN KEY([PasatiempoId])
REFERENCES [dbo].[Pasatiempos] ([Id])
GO
ALTER TABLE [dbo].[PersonaPasatiempos] CHECK CONSTRAINT [FK_PersonaPasatiempos_PasatiempoId]
GO
ALTER TABLE [dbo].[PersonaPasatiempos]  WITH CHECK ADD  CONSTRAINT [FK_PersonaPasatiempos_PersonaId] FOREIGN KEY([PersonaId])
REFERENCES [dbo].[Personas] ([Id])
GO
ALTER TABLE [dbo].[PersonaPasatiempos] CHECK CONSTRAINT [FK_PersonaPasatiempos_PersonaId]
GO
ALTER TABLE [dbo].[Inmuebles]  WITH CHECK ADD  CONSTRAINT [FK_Inmubles_Propietario] FOREIGN KEY([PropietarioId])
REFERENCES [dbo].[Propietarios] ([IdPropietario])
GO
ALTER TABLE [dbo].[Inmuebles] CHECK CONSTRAINT [FK_Inmubles_Propietario]
GO
ALTER TABLE [dbo].[Imagenes]  WITH CHECK ADD  CONSTRAINT [FK_Imagenes_Inmueble_Id] FOREIGN KEY([InmuebleId])
REFERENCES [dbo].[Inmuebles] ([Id])
GO
ALTER TABLE [dbo].[Imagenes] CHECK CONSTRAINT [FK_Imagenes_Inmueble_Id]
GO
PRINT N'Base BDInmobiliaria creada.';
GO
