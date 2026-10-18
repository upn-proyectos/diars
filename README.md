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
El sistema adopta una **Arquitectura Cliente-Servidor** con despliegue local:
- **Cliente:** Aplicación Web Frontend desarrollada bajo el patrón **MVC** (Models, Views, Controllers).
- **Servidor:** Backend en **C# (.NET)** estructurado en **Capas** con **ASP.NET Core Web API / MVC**.
- **Persistencia:** Base de datos relacional con acceso a través de **Entity Framework Core**.

```
[ Cliente Web (MVC) ]
         │  HTTP / HTTPS (JSON)
         ▼
[ Servidor C# (.NET) ]
  ├── Capa Presentación (API / Controllers MVC)
  ├── Capa Lógica de Negocio (Business / Core)
  └── Capa Acceso a Datos (Data / EF Core)
         │
         ▼
[ Base de Datos (SQL Server) ]
```

---

## 2. Módulos del Sistema
1. **Seguridad:** Autenticación de usuarios y autorización basada en 3 roles (`Administrador`, `Almacenero`, `Cajero`).
2. **Inventario:** Catálogo de productos, control de stock, movimientos de Kardex y alertas preventivas de stock mínimo y vencimientos.
3. **Compras:** Proveedores y registro de órdenes de compra para abastecimiento.
4. **Ventas:** Punto de venta (POS), cálculo de totales e IGV, emisión de comprobantes internos y métodos de pago.
5. **Reportes:** Consolidación de ingresos y reportes comerciales filtrados por rango de fechas.

---

## 3. Patrones de Diseño Aplicados
- **Arquitectónicos:** MVC (Model-View-Controller) y Arquitectura en Capas (Layered Architecture).
- **Creacional:** Singleton (Centralización de configuración global en memoria: IGV, serie de boletas).
- **Estructural:** Adapter (Adaptador para servicios externos de consulta de documentos DNI / RUC).
- **Comportamiento:** Strategy (Estrategias intercambiables para procesamiento de pagos: Efectivo, Yape/Plin, Tarjeta).
- **Empresariales / Framework:** Repository Pattern y Dependency Injection (nativo en ASP.NET Core).
