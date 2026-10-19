-- ============================================================
-- ESQUEMA DDL DE BASE DE DATOS (ESTRUCTURA PURA)
-- PROYECTO: Minimarket Chininin SAC
-- ARQUITECTURA: Cliente - Servidor (ASP.NET Core MVC + Web API)
-- MOTOR: Microsoft SQL Server 2019 / 2022
-- ============================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ChinininDB')
BEGIN
    CREATE DATABASE [ChinininDB] COLLATE Modern_Spanish_CI_AS;
END
GO

USE [ChinininDB];
GO

-- 1. TABLA ROLES
IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Roles (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(50) NOT NULL UNIQUE,
        Descripcion NVARCHAR(200) NULL
    );
END
GO

-- 2. TABLA USUARIOS
IF OBJECT_ID(N'dbo.Usuarios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuarios (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(150) NOT NULL,
        Usuario NVARCHAR(50) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        RolId INT NOT NULL FOREIGN KEY REFERENCES dbo.Roles(Id),
        Activo BIT NOT NULL DEFAULT 1,
        FechaCreacion DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END
GO

-- 3. TABLA CATEGORIAS
IF OBJECT_ID(N'dbo.Categorias', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categorias (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL UNIQUE,
        Descripcion NVARCHAR(255) NULL,
        Activo BIT NOT NULL DEFAULT 1
    );
END
GO

-- 4. TABLA PROVEEDORES
IF OBJECT_ID(N'dbo.Proveedores', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Proveedores (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Ruc NVARCHAR(20) NOT NULL UNIQUE,
        RazonSocial NVARCHAR(150) NOT NULL,
        NombreComercial NVARCHAR(150) NULL,
        Contacto NVARCHAR(100) NULL,
        Telefono NVARCHAR(30) NULL,
        Email NVARCHAR(100) NULL,
        Direccion NVARCHAR(200) NULL,
        Activo BIT NOT NULL DEFAULT 1
    );
END
GO

-- 5. TABLA PRODUCTOS
IF OBJECT_ID(N'dbo.Productos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Productos (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        CodigoBarras NVARCHAR(50) NULL,
        Nombre NVARCHAR(150) NOT NULL,
        CategoriaId INT NOT NULL FOREIGN KEY REFERENCES dbo.Categorias(Id),
        ProveedorId INT NULL FOREIGN KEY REFERENCES dbo.Proveedores(Id),
        PrecioVenta DECIMAL(18,2) NOT NULL,
        CostoUnitario DECIMAL(18,2) NOT NULL,
        Stock DECIMAL(10,3) NOT NULL DEFAULT 0,
        StockMinimo DECIMAL(10,3) NOT NULL DEFAULT 5,
        UnidadMedida NVARCHAR(20) NOT NULL DEFAULT 'UNIDAD', -- 'UNIDAD' o 'KG'
        FechaVencimiento DATE NULL,
        Activo BIT NOT NULL DEFAULT 1
    );
END
GO

-- 6. TABLA CLIENTES
IF OBJECT_ID(N'dbo.Clientes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Clientes (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        TipoDoc NVARCHAR(20) NOT NULL DEFAULT 'DNI',
        Documento NVARCHAR(20) NOT NULL UNIQUE,
        NombreCompleto NVARCHAR(150) NOT NULL,
        Telefono NVARCHAR(30) NULL,
        Direccion NVARCHAR(200) NULL,
        Activo BIT NOT NULL DEFAULT 1
    );
END
GO

-- 7. TABLA VENTAS
IF OBJECT_ID(N'dbo.Ventas', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Ventas (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        NumeroBoleta NVARCHAR(50) NOT NULL UNIQUE,
        Fecha DATETIME2 NOT NULL DEFAULT GETDATE(),
        ClienteId INT NOT NULL FOREIGN KEY REFERENCES dbo.Clientes(Id),
        UsuarioId INT NOT NULL FOREIGN KEY REFERENCES dbo.Usuarios(Id),
        MetodoPago NVARCHAR(50) NOT NULL, -- 'Efectivo', 'Yape', 'Plin', 'Tarjeta'
        Subtotal DECIMAL(18,2) NOT NULL,
        Igv DECIMAL(18,2) NOT NULL,
        Total DECIMAL(18,2) NOT NULL
    );
END
GO

-- 8. TABLA DETALLE_VENTAS
IF OBJECT_ID(N'dbo.DetalleVentas', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DetalleVentas (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        VentaId INT NOT NULL FOREIGN KEY REFERENCES dbo.Ventas(Id) ON DELETE CASCADE,
        ProductoId INT NULL FOREIGN KEY REFERENCES dbo.Productos(Id),
        NombreProducto NVARCHAR(150) NOT NULL,
        Cantidad DECIMAL(10,3) NOT NULL,
        PrecioUnitario DECIMAL(18,2) NOT NULL,
        CostoUnitario DECIMAL(18,2) NOT NULL,
        Subtotal DECIMAL(18,2) NOT NULL
    );
END
GO

-- 9. TABLA KARDEX (Auditoría de Movimientos)
IF OBJECT_ID(N'dbo.Kardex', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Kardex (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ProductoId INT NOT NULL FOREIGN KEY REFERENCES dbo.Productos(Id),
        Fecha DATETIME2 NOT NULL DEFAULT GETDATE(),
        TipoMovimiento NVARCHAR(20) NOT NULL, -- 'ENTRADA', 'SALIDA', 'AJUSTE'
        Cantidad DECIMAL(10,3) NOT NULL,
        Saldo DECIMAL(10,3) NOT NULL,
        Motivo NVARCHAR(200) NOT NULL,
        UsuarioId INT NOT NULL FOREIGN KEY REFERENCES dbo.Usuarios(Id)
    );
END
GO

-- 10. TABLA CONFIGURACION (Parámetros para Patrón Singleton)
IF OBJECT_ID(N'dbo.Configuracion', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Configuracion (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        NombreComercio NVARCHAR(150) NOT NULL,
        Ruc NVARCHAR(20) NOT NULL,
        Direccion NVARCHAR(200) NOT NULL,
        SerieBoleta NVARCHAR(10) NOT NULL,
        CorrelativoBoleta INT NOT NULL,
        TasaIgv DECIMAL(5,2) NOT NULL
    );
END
GO
