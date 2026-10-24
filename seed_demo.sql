-- ============================================================
-- DATOS DEMO Y PRUEBAS INICIALES (SEED DEMO)
-- PROYECTO: Minimarket Chininin SAC
-- ============================================================

USE [ChinininDB];
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
IF NOT EXISTS (SELECT 1 FROM dbo.Proveedores WHERE Ruc = N'20100070970')
    INSERT INTO dbo.Proveedores (Ruc, RazonSocial, NombreComercial, Contacto, Telefono, Email, Direccion)
    VALUES (N'20100070970', N'Alicorp S.A.A.', N'Alicorp', N'Carlos Méndez', N'946112233', N'ventas@alicorp.com', N'Av. Argentina 4793, Callao');
IF NOT EXISTS (SELECT 1 FROM dbo.Proveedores WHERE Ruc = N'20462509236')
    INSERT INTO dbo.Proveedores (Ruc, RazonSocial, NombreComercial, Contacto, Telefono, Email, Direccion)
    VALUES (N'20462509236', N'Leche Gloria S.A.', N'Gloria', N'Rosa Huamán', N'951223344', N'pedidos@gloria.com', N'Av. República de Panamá 2461, Lima');
IF NOT EXISTS (SELECT 1 FROM dbo.Proveedores WHERE Ruc = N'20501523837')
    INSERT INTO dbo.Proveedores (Ruc, RazonSocial, NombreComercial, Contacto, Telefono, Email, Direccion)
    VALUES (N'20501523837', N'Molitalia S.A.', N'Molitalia', N'Pedro Castillo', N'987334455', N'compras@molitalia.com', N'Av. Venezuela 2850, Lima');
IF NOT EXISTS (SELECT 1 FROM dbo.Proveedores WHERE Ruc = N'20100182263')
    INSERT INTO dbo.Proveedores (Ruc, RazonSocial, NombreComercial, Contacto, Telefono, Email, Direccion)
    VALUES (N'20100182263', N'Procter & Gamble Perú S.R.L.', N'P&G', N'Ana Torres', N'965445566', N'distribucion@pg.com', N'Av. El Derby 254, Surco');
IF NOT EXISTS (SELECT 1 FROM dbo.Proveedores WHERE Ruc = N'20516704517')
    INSERT INTO dbo.Proveedores (Ruc, RazonSocial, NombreComercial, Contacto, Telefono, Email, Direccion)
    VALUES (N'20516704517', N'Distribuidora Jiménez S.A.C.', N'Dist. Jiménez', N'Luis Jiménez', N'943556677', N'pedidos@djimenez.com', N'Jr. Gamarra 438, Trujillo');
IF NOT EXISTS (SELECT 1 FROM dbo.Proveedores WHERE Ruc = N'20514727700')
    INSERT INTO dbo.Proveedores (Ruc, RazonSocial, NombreComercial, Contacto, Telefono, Email, Direccion)
    VALUES (N'20514727700', N'Laive S.A.', N'Laive', N'Martha Quispe', N'952667788', N'ventas@laive.com', N'Av. Nicolás de Piérola 601, Lima');
IF NOT EXISTS (SELECT 1 FROM dbo.Proveedores WHERE Ruc = N'20100127912')
    INSERT INTO dbo.Proveedores (Ruc, RazonSocial, NombreComercial, Contacto, Telefono, Email, Direccion)
    VALUES (N'20100127912', N'Intradevco Industrial S.A.', N'Intradevco', N'Jorge Salazar', N'974778899', N'pedidos@intradevco.com', N'Av. Producción Nacional 188, Chorrillos');
IF NOT EXISTS (SELECT 1 FROM dbo.Proveedores WHERE Ruc = N'20518870661')
    INSERT INTO dbo.Proveedores (Ruc, RazonSocial, NombreComercial, Contacto, Telefono, Email, Direccion)
    VALUES (N'20518870661', N'Panadería San Jorge S.A.C.', N'San Jorge', N'Carmen Flores', N'961889900', N'ventas@sanjorge.com', N'Calle Los Olivos 230, Trujillo');
GO

-- 4. CATEGORÍAS (14 Categorías Comerciales)
IF NOT EXISTS (SELECT 1 FROM dbo.Categorias WHERE Nombre = N'Abarrotes')
    INSERT INTO dbo.Categorias (Nombre, Descripcion) VALUES (N'Abarrotes', N'Arroz, azúcar, fideos y granos');
IF NOT EXISTS (SELECT 1 FROM dbo.Categorias WHERE Nombre = N'Bebidas y Licores')
    INSERT INTO dbo.Categorias (Nombre, Descripcion) VALUES (N'Bebidas y Licores', N'Gaseosas, aguas, jugos y cervezas');
IF NOT EXISTS (SELECT 1 FROM dbo.Categorias WHERE Nombre = N'Lácteos y Huevos')
    INSERT INTO dbo.Categorias (Nombre, Descripcion) VALUES (N'Lácteos y Huevos', N'Leche, yogures, quesos y mantequilla');
IF NOT EXISTS (SELECT 1 FROM dbo.Categorias WHERE Nombre = N'Carnes y Embutidos')
    INSERT INTO dbo.Categorias (Nombre, Descripcion) VALUES (N'Carnes y Embutidos', N'Pollo, jamonada, salchichas');
IF NOT EXISTS (SELECT 1 FROM dbo.Categorias WHERE Nombre = N'Frutas y Verduras')
    INSERT INTO dbo.Categorias (Nombre, Descripcion) VALUES (N'Frutas y Verduras', N'Productos frescos pesados por kilogramo');
IF NOT EXISTS (SELECT 1 FROM dbo.Categorias WHERE Nombre = N'Limpieza y Aseo')
    INSERT INTO dbo.Categorias (Nombre, Descripcion) VALUES (N'Limpieza y Aseo', N'Detergentes, jabones y desinfectantes');
IF NOT EXISTS (SELECT 1 FROM dbo.Categorias WHERE Nombre = N'Snacks y Golosinas')
    INSERT INTO dbo.Categorias (Nombre, Descripcion) VALUES (N'Snacks y Golosinas', N'Papas fritas, galletas, chocolates, caramelos y piqueos');
IF NOT EXISTS (SELECT 1 FROM dbo.Categorias WHERE Nombre = N'Panadería y Pastelería')
    INSERT INTO dbo.Categorias (Nombre, Descripcion) VALUES (N'Panadería y Pastelería', N'Pan de molde, queques, biscochos, tostadas y panetones');
IF NOT EXISTS (SELECT 1 FROM dbo.Categorias WHERE Nombre = N'Cuidado Personal')
    INSERT INTO dbo.Categorias (Nombre, Descripcion) VALUES (N'Cuidado Personal', N'Shampoo, jabón de tocador, pasta dental, desodorantes y pañales');
IF NOT EXISTS (SELECT 1 FROM dbo.Categorias WHERE Nombre = N'Conservas y Enlatados')
    INSERT INTO dbo.Categorias (Nombre, Descripcion) VALUES (N'Conservas y Enlatados', N'Atún, sardinas, duraznos en almíbar, leche condensada y menestras');
IF NOT EXISTS (SELECT 1 FROM dbo.Categorias WHERE Nombre = N'Salsas y Condimentos')
    INSERT INTO dbo.Categorias (Nombre, Descripcion) VALUES (N'Salsas y Condimentos', N'Mayonesa, kétchup, mostaza, ají, vinagre, sal y especias');
IF NOT EXISTS (SELECT 1 FROM dbo.Categorias WHERE Nombre = N'Desayunos y Cereales')
    INSERT INTO dbo.Categorias (Nombre, Descripcion) VALUES (N'Desayunos y Cereales', N'Avena, café instantáneo, cocoa, infusiones, mermeladas y cereales');
IF NOT EXISTS (SELECT 1 FROM dbo.Categorias WHERE Nombre = N'Congelados y Helados')
    INSERT INTO dbo.Categorias (Nombre, Descripcion) VALUES (N'Congelados y Helados', N'Nuggets, hamburguesas congeladas, papas prefritas y helados');
IF NOT EXISTS (SELECT 1 FROM dbo.Categorias WHERE Nombre = N'Mascotas')
    INSERT INTO dbo.Categorias (Nombre, Descripcion) VALUES (N'Mascotas', N'Alimento balanceado para perros y gatos, sobres húmedos y premios');
GO

-- 5. CATÁLOGO DE 42 PRODUCTOS REALES (10 base + 12 de lista + 20 adicionales)
DECLARE @CatAbarrotes   INT = (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Abarrotes');
DECLARE @CatBebidas     INT = (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Bebidas y Licores');
DECLARE @CatLacteos     INT = (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Lácteos y Huevos');
DECLARE @CatCarnes      INT = (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Carnes y Embutidos');
DECLARE @CatVerduras    INT = (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Frutas y Verduras');
DECLARE @CatLimpieza    INT = (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Limpieza y Aseo');
DECLARE @CatSnacks      INT = (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Snacks y Golosinas');
DECLARE @CatPanaderia   INT = (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Panadería y Pastelería');
DECLARE @CatCuidado     INT = (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Cuidado Personal');
DECLARE @CatConservas   INT = (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Conservas y Enlatados');
DECLARE @CatSalsas      INT = (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Salsas y Condimentos');
DECLARE @CatDesayunos   INT = (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Desayunos y Cereales');
DECLARE @CatCongelados  INT = (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Congelados y Helados');
DECLARE @CatMascotas    INT = (SELECT Id FROM dbo.Categorias WHERE Nombre = N'Mascotas');

DECLARE @ProvAlicorp    INT = (SELECT Id FROM dbo.Proveedores WHERE Ruc = N'20100070970');
DECLARE @ProvGloria     INT = (SELECT Id FROM dbo.Proveedores WHERE Ruc = N'20462509236');
DECLARE @ProvMolitalia  INT = (SELECT Id FROM dbo.Proveedores WHERE Ruc = N'20501523837');
DECLARE @ProvPAndG      INT = (SELECT Id FROM dbo.Proveedores WHERE Ruc = N'20100182263');
DECLARE @ProvJimenez    INT = (SELECT Id FROM dbo.Proveedores WHERE Ruc = N'20516704517');
DECLARE @ProvLaive      INT = (SELECT Id FROM dbo.Proveedores WHERE Ruc = N'20514727700');
DECLARE @ProvIntradevco INT = (SELECT Id FROM dbo.Proveedores WHERE Ruc = N'20100127912');
DECLARE @ProvSanJorge   INT = (SELECT Id FROM dbo.Proveedores WHERE Ruc = N'20518870661');

-- Tabla temporal de carga idempotente
DECLARE @NuevosProductos TABLE (
    CodigoBarras NVARCHAR(50),
    Nombre NVARCHAR(150),
    CategoriaId INT,
    ProveedorId INT NULL,
    PrecioVenta DECIMAL(18,2),
    CostoUnitario DECIMAL(18,2),
    Stock DECIMAL(10,3),
    StockMinimo DECIMAL(10,3),
    UnidadMedida NVARCHAR(20),
    FechaVencimiento DATE NULL
);

INSERT INTO @NuevosProductos VALUES
-- 10 Productos Base
(N'7750123456789', N'Arroz Costeño Extra 1kg', @CatAbarrotes, @ProvAlicorp, 4.80, 3.80, 50, 10, N'UNIDAD', DATEADD(MONTH, 6, GETDATE())),
(N'7750123456790', N'Azúcar Rubia Cartavio 1kg', @CatAbarrotes, @ProvJimenez, 3.90, 3.10, 45, 10, N'UNIDAD', DATEADD(MONTH, 8, GETDATE())),
(N'7750123456791', N'Aceite Primor Premium 900ml', @CatAbarrotes, @ProvAlicorp, 8.50, 7.00, 25, 8, N'UNIDAD', DATEADD(MONTH, 10, GETDATE())),
(N'7750236510042', N'Inca Kola 1.5L No Retornable', @CatBebidas, @ProvJimenez, 6.50, 4.80, 24, 8, N'UNIDAD', DATEADD(MONTH, 4, GETDATE())),
(N'7750236510059', N'Coca-Cola 1.5L No Retornable', @CatBebidas, @ProvJimenez, 6.50, 4.80, 20, 8, N'UNIDAD', DATEADD(MONTH, 4, GETDATE())),
(N'7750123456794', N'Leche Gloria Azul Entera 400g', @CatLacteos, @ProvGloria, 4.20, 3.40, 60, 15, N'UNIDAD', DATEADD(MONTH, 5, GETDATE())),
(N'7750123456795', N'Yogurt Gloria Fresa 1L', @CatLacteos, @ProvGloria, 6.80, 5.20, 18, 5, N'UNIDAD', DATEADD(DAY, 20, GETDATE())),
(N'7750123456796', N'Tomate Italiano (Granel)', @CatVerduras, NULL, 3.50, 2.30, 25.500, 5.000, N'KG', DATEADD(DAY, 7, GETDATE())),
(N'7750123456797', N'Papa Blanca (Granel)', @CatVerduras, NULL, 2.20, 1.50, 40.000, 10.000, N'KG', DATEADD(DAY, 14, GETDATE())),
(N'7750123456798', N'Detergente Ace 850g', @CatLimpieza, @ProvPAndG, 7.90, 6.20, 20, 5, N'UNIDAD', DATEADD(YEAR, 2, GETDATE())),

-- 12 Productos de la lista previa
(N'7750100112231', N'Piqueo Snax Original 200g', @CatSnacks, @ProvJimenez, 6.50, 4.80, 30, 8, N'UNIDAD', DATEADD(MONTH, 5, GETDATE())),
(N'7750100112232', N'Galleta Casino Menta Paquete x6', @CatSnacks, @ProvAlicorp, 4.50, 3.20, 40, 10, N'UNIDAD', DATEADD(MONTH, 6, GETDATE())),
(N'7750100112233', N'Chocolate Sublime Clásico 30g', @CatSnacks, @ProvJimenez, 2.00, 1.30, 60, 15, N'UNIDAD', DATEADD(MONTH, 7, GETDATE())),
(N'7750100112234', N'Pan de Molde Blanco Bimbo 500g', @CatPanaderia, @ProvSanJorge, 7.50, 5.80, 15, 5, N'UNIDAD', DATEADD(DAY, 12, GETDATE())),
(N'7750100112235', N'Panetón Gloria Caja 900g', @CatPanaderia, @ProvGloria, 23.90, 18.50, 12, 4, N'UNIDAD', DATEADD(MONTH, 4, GETDATE())),
(N'7750100112236', N'Shampoo Head & Shoulders 375ml', @CatCuidado, @ProvPAndG, 17.90, 13.50, 18, 5, N'UNIDAD', DATEADD(YEAR, 2, GETDATE())),
(N'7750100112237', N'Pasta Dental Colgate Triple Acción 150ml', @CatCuidado, @ProvPAndG, 5.90, 4.20, 35, 8, N'UNIDAD', DATEADD(YEAR, 1, GETDATE())),
(N'7750100112238', N'Filete de Atún Florida en Aceite 170g', @CatConservas, @ProvAlicorp, 5.80, 4.50, 48, 12, N'UNIDAD', DATEADD(YEAR, 2, GETDATE())),
(N'7750100112239', N'Mayonesa Alacena Doypack 475g', @CatSalsas, @ProvAlicorp, 10.20, 7.80, 22, 6, N'UNIDAD', DATEADD(MONTH, 5, GETDATE())),
(N'7750100112240', N'Café Nescafé Tradición Frasco 170g', @CatDesayunos, @ProvJimenez, 20.50, 16.00, 14, 4, N'UNIDAD', DATEADD(YEAR, 1, GETDATE())),
(N'7750100112241', N'Avena 3 Ositos Clásica Bolsa 300g', @CatDesayunos, @ProvMolitalia, 3.80, 2.80, 5, 10, N'UNIDAD', DATEADD(MONTH, 8, GETDATE())),
(N'7750100112242', N'Ricocan Adultos Cordero y Cereales', @CatMascotas, @ProvJimenez, 8.90, 6.50, 25.500, 8.000, N'KG', DATEADD(MONTH, 9, GETDATE())),

-- 20 Productos Adicionales distribuidos en las 14 categorías
(N'7750100223301', N'Fideos Spaghetti Don Vittorio 500g', @CatAbarrotes, @ProvAlicorp, 3.80, 2.90, 55, 12, N'UNIDAD', DATEADD(YEAR, 1, GETDATE())),
(N'7750100223302', N'Lenteja Costeño Bolsa 500g', @CatAbarrotes, @ProvAlicorp, 5.20, 3.90, 28, 8, N'UNIDAD', DATEADD(MONTH, 10, GETDATE())),
(N'7750100223303', N'Agua Mineral San Mateo Sin Gas 600ml', @CatBebidas, @ProvJimenez, 2.00, 1.20, 72, 18, N'UNIDAD', DATEADD(MONTH, 8, GETDATE())),
(N'7750100223304', N'Jugo Frugos del Valle Durazno 1L', @CatBebidas, @ProvJimenez, 5.50, 4.10, 6, 8, N'UNIDAD', DATEADD(MONTH, 4, GETDATE())),
(N'7750100223305', N'Mantequilla Gloria con Sal Barra 200g', @CatLacteos, @ProvGloria, 9.50, 7.40, 16, 5, N'UNIDAD', DATEADD(DAY, 25, GETDATE())),
(N'7750100223306', N'Queso Edam Laive Laminado 180g', @CatLacteos, @ProvLaive, 10.90, 8.40, 12, 4, N'UNIDAD', DATEADD(DAY, 18, GETDATE())),
(N'7750100223307', N'Huevos Pardos de Corral (Granel)', @CatLacteos, @ProvJimenez, 8.80, 7.10, 18.500, 6.000, N'KG', DATEADD(DAY, 15, GETDATE())),
(N'7750100223308', N'Hot Dog de Pollo San Fernando 440g', @CatCarnes, @ProvLaive, 7.20, 5.50, 14, 5, N'UNIDAD', DATEADD(DAY, 10, GETDATE())),
(N'7750100223309', N'Jamonada Especial Laive 200g', @CatCarnes, @ProvLaive, 5.90, 4.30, 4, 6, N'UNIDAD', DATEADD(DAY, 6, GETDATE())),
(N'7750100223310', N'Pechuga de Pollo Fresca (Granel)', @CatCarnes, NULL, 14.50, 11.20, 12.000, 4.000, N'KG', DATEADD(DAY, 4, GETDATE())),
(N'7750100223311', N'Cebolla Roja Arequipeña (Granel)', @CatVerduras, NULL, 3.20, 2.10, 32.000, 8.000, N'KG', DATEADD(DAY, 16, GETDATE())),
(N'7750100223312', N'Plátano de Seda Orgánico (Granel)', @CatVerduras, NULL, 3.80, 2.50, 22.500, 6.000, N'KG', DATEADD(DAY, 5, GETDATE())),
(N'7750100223313', N'Lavavajilla Ayudín Limón Pasta 550g', @CatLimpieza, @ProvPAndG, 5.50, 4.10, 25, 6, N'UNIDAD', DATEADD(YEAR, 2, GETDATE())),
(N'7750100223314', N'Lejía Sapolio Tradicional 1L', @CatLimpieza, @ProvIntradevco, 3.50, 2.40, 30, 8, N'UNIDAD', DATEADD(YEAR, 1, GETDATE())),
(N'7750100223315', N'Galleta Soda San Jorge Paquete x6', @CatPanaderia, @ProvSanJorge, 3.20, 2.20, 36, 10, N'UNIDAD', DATEADD(MONTH, 5, GETDATE())),
(N'7750100223316', N'Jabón Antibacterial Bolívar Tocador Pack x3', @CatCuidado, @ProvAlicorp, 7.80, 5.90, 20, 5, N'UNIDAD', DATEADD(YEAR, 2, GETDATE())),
(N'7750100223317', N'Duraznos en Almíbar Arica Lata 820g', @CatConservas, @ProvJimenez, 11.50, 8.90, 15, 4, N'UNIDAD', DATEADD(YEAR, 2, GETDATE())),
(N'7750100223318', N'Crema de Ají Tarí Alacena 400g', @CatSalsas, @ProvAlicorp, 9.20, 7.00, 19, 5, N'UNIDAD', DATEADD(MONTH, 6, GETDATE())),
(N'7750100223319', N'Helado Peziduri Tricolor 1L', @CatCongelados, @ProvGloria, 13.90, 10.50, 10, 3, N'UNIDAD', DATEADD(MONTH, 6, GETDATE())),
(N'7750100223320', N'Ricocat Adultos Pescado y Pollo', @CatMascotas, @ProvJimenez, 10.50, 7.80, 16.000, 5.000, N'KG', DATEADD(MONTH, 10, GETDATE()));

INSERT INTO dbo.Productos (CodigoBarras, Nombre, CategoriaId, ProveedorId, PrecioVenta, CostoUnitario, Stock, StockMinimo, UnidadMedida, FechaVencimiento)
SELECT n.CodigoBarras, n.Nombre, n.CategoriaId, n.ProveedorId, n.PrecioVenta, n.CostoUnitario, n.Stock, n.StockMinimo, n.UnidadMedida, n.FechaVencimiento
FROM @NuevosProductos n
WHERE NOT EXISTS (SELECT 1 FROM dbo.Productos p WHERE p.CodigoBarras = n.CodigoBarras);
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
