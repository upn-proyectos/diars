-- ============================================================
-- DATOS DEMO Y PRUEBAS INICIALES (SEED DEMO)
-- PROYECTO: Minimarket Chininin SAC
-- ============================================================

USE [ChinininDB];
GO

-- 0. LIMPIEZA ORDENADA PARA REINICIO LIMPIO (En orden inverso de dependencias)
DELETE FROM dbo.DetalleVentas;
DELETE FROM dbo.Kardex;
DELETE FROM dbo.Ventas;
DELETE FROM dbo.Productos;
DELETE FROM dbo.Proveedores;
DELETE FROM dbo.Categorias;

DBCC CHECKIDENT ('dbo.Productos', RESEED, 0);
DBCC CHECKIDENT ('dbo.Proveedores', RESEED, 0);
DBCC CHECKIDENT ('dbo.Categorias', RESEED, 0);
GO

-- 1. ROLES OFICIALES (3 Roles del Informe Académico)
IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Nombre = N'Administrador')
    INSERT INTO dbo.Roles (Nombre, Descripcion) VALUES (N'Administrador', N'Acceso total y configuración del sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Nombre = N'Almacenero')
    INSERT INTO dbo.Roles (Nombre, Descripcion) VALUES (N'Almacenero', N'Control de stock, productos y kardex');
IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Nombre = N'Cajero')
    INSERT INTO dbo.Roles (Nombre, Descripcion) VALUES (N'Cajero', N'Registro de ventas y punto de venta POS');
GO

-- 2. USUARIOS DEMO (Contraseña pública de aula: admin123)
DECLARE @RolAdminId INT = (SELECT Id FROM dbo.Roles WHERE Nombre = N'Administrador');
DECLARE @RolAlmacenId INT = (SELECT Id FROM dbo.Roles WHERE Nombre = N'Almacenero');
DECLARE @RolCajeroId INT = (SELECT Id FROM dbo.Roles WHERE Nombre = N'Cajero');

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Usuario = N'admin')
    INSERT INTO dbo.Usuarios (Nombre, Usuario, PasswordHash, RolId) 
    VALUES (N'Administrador General', N'admin', N'admin123', @RolAdminId);

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Usuario = N'almacen')
    INSERT INTO dbo.Usuarios (Nombre, Usuario, PasswordHash, RolId) 
    VALUES (N'Luis Quispe Rojas', N'almacen', N'admin123', @RolAlmacenId);

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Usuario = N'cajero')
    INSERT INTO dbo.Usuarios (Nombre, Usuario, PasswordHash, RolId) 
    VALUES (N'María Fernanda Salazar', N'cajero', N'admin123', @RolCajeroId);
GO

-- 3. PROVEEDORES REALES PERUANOS (Con RUCs válidos de 11 dígitos de SUNAT)
INSERT INTO dbo.Proveedores (Ruc, RazonSocial, NombreComercial, Contacto, Telefono, Email, Direccion) VALUES
(N'20100070970', N'Alicorp S.A.A.', N'Alicorp', N'Carlos Méndez', N'946112233', N'ventas@alicorp.com', N'Av. Argentina 4793, Callao'),
(N'20462509236', N'Leche Gloria S.A.', N'Gloria', N'Rosa Huamán', N'951223344', N'pedidos@gloria.com', N'Av. República de Panamá 2461, Lima'),
(N'20501523837', N'Molitalia S.A.', N'Molitalia', N'Pedro Castillo', N'987334455', N'compras@molitalia.com', N'Av. Venezuela 2850, Lima'),
(N'20100182263', N'Procter & Gamble Perú S.R.L.', N'P&G', N'Ana Torres', N'965445566', N'distribucion@pg.com', N'Av. El Derby 254, Surco'),
(N'20516704517', N'Distribuidora Jiménez S.A.C.', N'Dist. Jiménez', N'Luis Jiménez', N'943556677', N'pedidos@djimenez.com', N'Jr. Gamarra 438, Trujillo'),
(N'20514727700', N'Laive S.A.', N'Laive', N'Martha Quispe', N'952667788', N'ventas@laive.com', N'Av. Nicolás de Piérola 601, Lima'),
(N'20100127912', N'Intradevco Industrial S.A.', N'Intradevco', N'Jorge Salazar', N'974778899', N'pedidos@intradevco.com', N'Av. Producción Nacional 188, Chorrillos'),
(N'20518870661', N'Panadería San Jorge S.A.C.', N'San Jorge', N'Carmen Flores', N'961889900', N'ventas@sanjorge.com', N'Calle Los Olivos 230, Trujillo');
GO

-- 4. CATEGORÍAS
INSERT INTO dbo.Categorias (Nombre, Descripcion) VALUES
(N'Abarrotes', N'Arroz, azúcar, fideos y granos'),
(N'Bebidas y Licores', N'Gaseosas, aguas, jugos y cervezas'),
(N'Lácteos y Huevos', N'Leche, yogures, quesos y mantequilla'),
(N'Carnes y Embutidos', N'Pollo, jamonada, salchichas'),
(N'Frutas y Verduras', N'Productos frescos pesados por kilogramo'),
(N'Limpieza y Aseo', N'Detergentes, jabones y desinfectantes');
GO

-- 5. PRODUCTOS DE PRUEBA REALES (Con tildes, códigos y proveedores asociados)
DECLARE @CatAbarrotes INT = (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Abarrotes');
DECLARE @CatBebidas INT = (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Bebidas y Licores');
DECLARE @CatLacteos INT = (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Lácteos y Huevos');
DECLARE @CatVerduras INT = (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Frutas y Verduras');
DECLARE @CatLimpieza INT = (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Limpieza y Aseo');

DECLARE @ProvAlicorp INT = (SELECT Id FROM dbo.Proveedores WHERE Ruc = N'20100070970');
DECLARE @ProvGloria INT = (SELECT Id FROM dbo.Proveedores WHERE Ruc = N'20462509236');
DECLARE @ProvJimenez INT = (SELECT Id FROM dbo.Proveedores WHERE Ruc = N'20516704517');
DECLARE @ProvPAndG INT = (SELECT Id FROM dbo.Proveedores WHERE Ruc = N'20100182263');

INSERT INTO dbo.Productos (CodigoBarras, Nombre, CategoriaId, ProveedorId, PrecioVenta, CostoUnitario, Stock, StockMinimo, UnidadMedida, FechaVencimiento) VALUES
(N'7750123456789', N'Arroz Costeño Extra 1kg', @CatAbarrotes, @ProvAlicorp, 4.80, 3.80, 50, 10, N'UNIDAD', DATEADD(MONTH, 6, GETDATE())),
(N'7750123456790', N'Azúcar Rubia Cartavio 1kg', @CatAbarrotes, @ProvJimenez, 3.90, 3.10, 45, 10, N'UNIDAD', DATEADD(MONTH, 8, GETDATE())),
(N'7750123456791', N'Aceite Primor Premium 900ml', @CatAbarrotes, @ProvAlicorp, 8.50, 7.00, 25, 8, N'UNIDAD', DATEADD(MONTH, 10, GETDATE())),
(N'7750236510042', N'Inca Kola 1.5L No Retornable', @CatBebidas, @ProvJimenez, 6.50, 4.80, 24, 8, N'UNIDAD', DATEADD(MONTH, 4, GETDATE())),
(N'7750236510059', N'Coca-Cola 1.5L No Retornable', @CatBebidas, @ProvJimenez, 6.50, 4.80, 20, 8, N'UNIDAD', DATEADD(MONTH, 4, GETDATE())),
(N'7750123456794', N'Leche Gloria Azul Entera 400g', @CatLacteos, @ProvGloria, 4.20, 3.40, 60, 15, N'UNIDAD', DATEADD(MONTH, 5, GETDATE())),
(N'7750123456795', N'Yogurt Gloria Fresa 1L', @CatLacteos, @ProvGloria, 6.80, 5.20, 18, 5, N'UNIDAD', DATEADD(DAY, 20, GETDATE())),
(N'7750123456796', N'Tomate Italiano (Granel)', @CatVerduras, NULL, 3.50, 2.30, 25.500, 5.000, N'KG', DATEADD(DAY, 7, GETDATE())),
(N'7750123456797', N'Papa Blanca (Granel)', @CatVerduras, NULL, 2.20, 1.50, 40.000, 10.000, N'KG', DATEADD(DAY, 14, GETDATE())),
(N'7750123456798', N'Detergente Ace 850g', @CatLimpieza, @ProvPAndG, 7.90, 6.20, 20, 5, N'UNIDAD', DATEADD(YEAR, 2, GETDATE()));
GO

-- 6. CLIENTE GENÉRICO (Público General)
IF NOT EXISTS (SELECT 1 FROM dbo.Clientes WHERE Documento = N'00000000')
    INSERT INTO dbo.Clientes (TipoDoc, Documento, NombreCompleto, Telefono, Direccion)
    VALUES (N'DNI', N'00000000', N'Clientes Varios / Público General', N'-', N'-');
GO

-- 7. CONFIGURACIÓN OFICIAL DE TIENDA (Para Patrón Singleton)
IF NOT EXISTS (SELECT 1 FROM dbo.Configuracion WHERE Id = 1)
    INSERT INTO dbo.Configuracion (NombreComercio, Ruc, Direccion, SerieBoleta, CorrelativoBoleta, TasaIgv)
    VALUES (N'Minimarket Chininin SAC', N'20601234567', N'Av. América Norte 1234 - Trujillo', N'B001', 1, 0.18);
GO
