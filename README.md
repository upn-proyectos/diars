# Minimarket Chininin SAC — Sistema de Gestión de Inventario y Ventas

**Curso:** Diseño y Arquitectura de Software (DIARS) — UPN Trujillo (2026-2)  
**Docente:** Ing. Ronny Alexis Sanchez Vargas  
**Integrantes:**
- Cano Sanchez Carlos Fabrizio Benjamin
- Encomenderos Ruiz Wilbort Yeyyson
- Flores Chininnin Walter Junior
- Mendez Oviedo Nick Alonso
- Silva Diaz Cristian Jesús
- Villanueva Briceño Carlitos Maximo Ruben

---

## 1. Arquitectura de Solución

El sistema implementa una **Arquitectura Cliente-Servidor** desplegada en entorno local y organizada internamente bajo una **Arquitectura en Capas (Layered Architecture)** y el patrón **MVC**:

- **Cliente Web (`Minimarket.Front`):** Aplicación web desarrollada en **ASP.NET Core MVC (.NET 10)** con vistas Razor (`.cshtml`), Tailwind CSS (identidad corporativa verde `#16A34A`), controladores web y modelos de vista.
- **Servidor de Aplicación (`Back`):** Backend estructurado en **3 capas desacopladas** en C#:
  1. **Capa de Presentación / Servicios (`Minimarket.API` y Controladores MVC):** Exposición de endpoints HTTP/JSON y gestión de peticiones de usuario.
  2. **Capa de Lógica de Negocio (`Minimarket.Negocio`):** Reglas comerciales, validaciones de dominio, DTOs, servicios (`AuthService`, `ProductoService`, `ProveedorService`) y patrones de diseño GoF.
  3. **Capa de Acceso a Datos (`Minimarket.Datos`):** Persistencia relacional con **Entity Framework Core**, `MinimarketDbContext`, entidades de dominio y repositorios.
- **Servidor de Base de Datos (`ChinininDB`):** Motor relacional **Microsoft SQL Server** aprovisionado mediante enfoque *Database First*.

```text
[ Cliente Web (Navegador / Vistas MVC Razor) ]
                      │
                      │  Peticiones HTTP / HTTPS
                      ▼
[ Servidor de Aplicación C# (ASP.NET Core .NET 10) ]
  ├── Capa de Presentación    (Minimarket.Front / Minimarket.API)
  ├── Capa Lógica de Negocio  (Minimarket.Negocio: Servicios + Patrones GoF)
  └── Capa Acceso a Datos     (Minimarket.Datos: Repositorios + EF Core)
                      │
                      │  Consultas LINQ / Transacciones ACID (TCP/IP)
                      ▼
[ Servidor de Base de Datos (Microsoft SQL Server: ChinininDB) ]
```

---

## 2. Estructura de la Solución (`Minimarket.sln`)

```text
minimarket-diars/
├── Minimarket.sln                  # Solución principal de Visual Studio (.NET 10)
├── schema.sql                      # Script DDL transaccional (10 tablas relacionales)
├── seed_demo.sql                   # Datos semilla (3 roles, 3 usuarios, 8 proveedores, 14 categorías, 42 productos)
├── Back/
│   ├── Minimarket.API/             # API RESTful (AuthController, ProductosController, ProveedoresController)
│   ├── Minimarket.Negocio/         # Capa de Lógica de Negocio
│   │   ├── DTOs/                   # Objetos de transferencia (UsuarioSessionDto, ProductoDto, ProveedorDto)
│   │   ├── Interfaces/             # Contratos de negocio (IAuthService, IProductoService, IProveedorService)
│   │   ├── Patterns/Adapter/       # Patrón GoF Adapter (IValidadorDocumentoAdapter, ValidadorDocumentoPeruAdapter)
│   │   └── Services/               # Implementación de reglas de negocio
│   └── Minimarket.Datos/           # Capa de Acceso a Datos
│       ├── Context/                # MinimarketDbContext (Mapeo Fluent API)
│       ├── Entities/               # Entidades de dominio (Producto, Categoria, Proveedor, Kardex, Venta, etc.)
│       └── Repositories/           # Patrón Repository (IProductoRepository, IProveedorRepository)
└── Front/
    ├── Controllers/                # Controladores MVC (Account, Home, Productos, Categorias, Proveedores, etc.)
    ├── Models/                     # ViewModels de interfaz (DashboardViewModel, ProductosCatalogoViewModel)
    ├── Views/                      # Vistas Razor (.cshtml) con diseño responsivo Tailwind CSS
    └── wwwroot/                    # Recursos estáticos (site.css, iconos y scripts)
```

---

## 3. Patrones de Diseño Aplicados

| Categoría | Patrón | Ubicación en el Código | Propósito en el Sistema | Estado |
|---|---|---|---|:---:|
| **Arquitectónico** | **Capas (Layered) + MVC** | Solución `Minimarket.sln` | Separa responsabilidades entre Interfaz (`Front`), Reglas de Negocio (`Minimarket.Negocio`) y Persistencia (`Minimarket.Datos`). | ✅ Implementado |
| **Empresarial** | **Repository Pattern** | `Back/Minimarket.Datos/Repositories/` | Abstrae las consultas de Entity Framework Core mediante `IProductoRepository` e `IProveedorRepository`, incluyendo transacciones ACID para Kardex. | ✅ Implementado |
| **Estructural (GoF)** | **Adapter Pattern** | `Back/Minimarket.Negocio/Patterns/Adapter/` | `ValidadorDocumentoPeruAdapter` envuelve a `SunatReniecMotorExterno` para adaptar la validación tributaria de **RUC (11 dígitos SUNAT)** y **DNI (8 dígitos)** a `IValidadorDocumentoAdapter`. | ✅ Implementado |
| **Framework / IoC** | **Dependency Injection** | `Program.cs` (`Front` y `API`) | Inyección por constructor de contextos, repositorios, adaptadores y servicios con ciclo de vida controlado (`Scoped` / `Singleton`). | ✅ Implementado |
| **Creacional (GoF)** | **Singleton Pattern** | `Back/Minimarket.Negocio/Patterns/Singleton/` | Centralización en memoria de parámetros globales de tienda (IGV 18%, serie `B001` y correlativo de boletas). | ⏳ Fase 6 (T3) |
| **Comportamiento (GoF)** | **Strategy Pattern** | `Back/Minimarket.Negocio/Patterns/Strategy/` | Algoritmos intercambiables para métodos de pago en el POS (`Efectivo`, `Yape/Plin`, `Tarjeta`). | ⏳ Fase 6 (T3) |

---

## 4. Módulos del Sistema y Estado de Avance

1. **Dashboard Ejecutivo (`HomeController`):**
   - Indicadores KPI en tiempo real conectados a SQL Server (Total de productos activos, proveedores validados, categorías, valorización de almacén y tabla de alertas de stock crítico).
2. **Módulo de Seguridad y Control de Acceso (`AccountController` / `AuthService`):**
   - Autenticación con cookies seguras (`CookieAuthenticationDefaults`), protección de rutas con `[Authorize]` y navegación adaptativa según los **3 roles oficiales** (`Administrador`, `Almacenero`, `Cajero`).
3. **Módulo de Inventario y Catálogo (`ProductosController` / `ProductoService`):**
   - Listado general con buscador reactivo por nombre/código de barras, filtro por categoría y filtros rápidos (*Todos*, *Stock Bajo*, *Por Vencer*).
   - Semáforo automático de stock (`Stock <= StockMinimo`) y semáforo FEFO de vencimiento ($\le 7$ días crítico, $\le 30$ días próximo).
   - Creación y edición de productos con calculadora de margen comercial en vivo y creación rápida de categoría sin salir del modal.
   - Ajuste físico de existencias con registro transaccional automático en la tabla `Kardex`.
4. **Módulo de Categorías (`CategoriasController`):**
   - Gestión visual por tarjetas con conteo de productos asignados, barra de ocupación, edición y protección de integridad referencial al eliminar.
5. **Módulo de Proveedores y Abastecimiento (`ProveedoresController` / `ProveedorService`):**
   - Directorio de proveedores con validación en vivo de **RUC de 11 dígitos** e identificación automática del tipo de contribuyente (`Persona Jurídica RUC 20` / `Persona Natural con Negocio RUC 10`) mediante el **Patrón Adapter**.
6. **Punto de Venta (POS), Kardex y Reportes:**
   - Programados para el Bloque B (Entrega Final T3).

---

## 5. Requisitos e Instrucciones de Ejecución Local

### Requisitos previos
- **.NET SDK 10.0** (o Visual Studio 2022 / 2026 con soporte .NET).
- **Microsoft SQL Server** (instancia local `localhost`).

### Paso 1: Aprovisionar la Base de Datos en SQL Server
Ejecutar en **SQL Server Management Studio (SSMS)** o mediante `sqlcmd` los dos scripts incluidos en la raíz:
1. `schema.sql` — Crea la base de datos `ChinininDB` y sus 10 tablas relacionales.
2. `seed_demo.sql` — Inserta los 3 roles oficiales, 3 usuarios demo, 8 proveedores peruanos reales, 14 categorías y 42 productos de minimarket.

```powershell
sqlcmd -S localhost -E -f 65001 -i schema.sql
sqlcmd -S localhost -d ChinininDB -E -f 65001 -i seed_demo.sql
```

### Paso 2: Ejecutar la Solución
Abrir `Minimarket.sln` en Visual Studio y presionar **F5** (o ejecutar por consola):

```powershell
dotnet run --project Front/Minimarket.Front.csproj
```

---

## 6. Credenciales de Prueba (Entorno Académico)

La pantalla de Login incluye botones de **Acceso Rápido en 1 clic** para evaluar el comportamiento del sistema según cada rol:

| Rol Oficial | Usuario | Contraseña | Permisos y Módulos Habilitados |
|---|---|---|---|
| **Administrador** | `admin` | `admin123` | Acceso total: Dashboard, POS, Productos, Categorías, Proveedores, Clientes, Kardex y Reportes. |
| **Almacenero** | `almacen` | `admin123` | Gestión de Inventario: Dashboard, Productos, Ajustes de Stock, Categorías y Proveedores. |
| **Cajero** | `cajero` | `admin123` | Operación Comercial: Dashboard, Punto de Venta (POS) y Directorio de Clientes. |
