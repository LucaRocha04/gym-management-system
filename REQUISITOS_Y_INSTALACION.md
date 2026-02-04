# Sistema de Gestión de Gimnasio - Requisitos e Instalación

## 📋 Descripción del Programa

**GimnasioApp** es una aplicación integral de gestión para gimnasios que permite administrar:

- **Gestión de Socios**: Registro, modificación y control de estado de clientes
- **Planes de Membresía**: Creación y administración de diferentes planes (Mensual, Trimestral, Anual)
- **Control de Pagos**: Registro y seguimiento de pagos de cuotas con múltiples métodos (Efectivo, Tarjeta, Transferencia)
- **Control de Asistencia**: Registro rápido de entrada/salida de socios
- **Clases y Inscripciones**: Gestión de clases disponibles e inscripciones de socios
- **Reportes**: Análisis de socios morosos, activos, ingresos mensuales, etc.
- **Gestión de Usuarios**: Administración de cuentas con roles (Administrador, Recepcionista, Profesor)
- **Notificaciones por Email**: Envío de confirmaciones y notificaciones automáticas

La aplicación tiene dos interfaces:
- **Interfaz de Escritorio (WinForms)**: Aplicación con interfaz gráfica moderna
- **Interfaz de Consola**: Versión en línea de comandos

---

## 🔧 Requisitos del Sistema

### Hardware Mínimo
- **Procesador**: Intel Core i5 o equivalente (2 GHz)
- **RAM**: 4 GB mínimo (8 GB recomendado)
- **Disco**: 500 MB de espacio libre
- **OS**: Windows 10/11, macOS 11+, o Linux (Ubuntu 20.04+)

### Software Requerido

#### 1. **.NET 8 SDK** (Requerido)
- **Versión mínima**: .NET 8.0 o superior
- **Descarga**: https://dotnet.microsoft.com/download/dotnet/8.0

**Para verificar si está instalado:**
```powershell
dotnet --version
```

#### 2. **Git** (Opcional, para clonar el repositorio)
- **Descarga**: https://git-scm.com/download/win

#### 3. **SQLite3** (Incluido en la app, pero útil para consultas directas)
- **Descarga**: https://www.sqlite.org/download.html

#### 4. **Visual Studio Code** (Opcional, para editar código)
- **Descarga**: https://code.visualstudio.com/

---

## 📦 Dependencias del Proyecto

Las siguientes librerías se descargan automáticamente al compilar:

```
- Microsoft.Data.Sqlite (v8.0.0) - Acceso a base de datos SQLite
- Brevo (v2.x) - Servicio de email
- .NET Framework 8.0 - Framework base
```

---

## 🚀 Guía de Instalación y Ejecución

### Opción 1: Descargar desde archivo .zip (Más fácil)

1. **Descargar y extraer** el proyecto en tu computadora

2. **Abrir PowerShell** en la carpeta del proyecto:
   ```
   cd "C:\ruta\a\proyecto final"
   ```

3. **Compilar la aplicación**:
   ```powershell
   dotnet build
   ```

4. **Ejecutar la aplicación de escritorio**:
   ```powershell
   dotnet run --project "GimnasioApp.Desktop/GimnasioApp.Desktop.csproj"
   ```

   O **ejecutar la consola**:
   ```powershell
   dotnet run --project "GimnasioApp/GimnasioApp.csproj"
   ```

---

### Opción 2: Clonar desde Git (Si tienes repositorio)

1. **Clonar el repositorio**:
   ```powershell
   git clone <url-del-repositorio>
   cd "proyecto final"
   ```

2. **Restaurar dependencias**:
   ```powershell
   dotnet restore
   ```

3. **Compilar**:
   ```powershell
   dotnet build
   ```

4. **Ejecutar**:
   ```powershell
   dotnet run --project "GimnasioApp.Desktop/GimnasioApp.Desktop.csproj"
   ```

---

## 🔐 Credenciales por Defecto

Cuando la aplicación se inicia por primera vez, se crean automáticamente estos usuarios:

| Usuario | Contraseña | Rol |
|---------|-----------|-----|
| admin | admin123 | Administrador |
| recep | recep123 | Recepcionista |
| prof | prof123 | Profesor |

**⚠️ IMPORTANTE**: Cambiar estas contraseñas en producción

---

## 📊 Estructura de Carpetas

```
proyecto final/
├── GimnasioApp/                    # Aplicación de Consola
│   ├── Models/                     # Modelos de datos
│   ├── Managers/                   # Lógica de negocios
│   ├── Services/                   # Servicios (Email, etc)
│   ├── Connection/                 # Conexión a BD
│   ├── scripts/                    # Scripts SQL
│   └── Program.cs
│
├── GimnasioApp.Desktop/            # Aplicación de Escritorio (WinForms)
│   ├── Forms/                      # Formularios
│   ├── Resources/                  # Imágenes y recursos
│   ├── Theme/                      # Estilos visuales
│   └── Program.cs
│
├── ConsoleTest/                    # Pruebas de consola
├── TestDB/                         # Pruebas de base de datos
├── ManualTest/                     # Pruebas manuales
│
└── proyecto final.sln              # Solución principal
```

---

## 🗄️ Base de Datos

- **Tipo**: SQLite
- **Ubicación**: `bin/Debug/net8.0-windows/gimnasio.db`
- **Tablas**:
  - `usuarios` - Cuentas de usuario
  - `socios` - Información de miembros
  - `planes` - Planes de membresía
  - `pagos` - Registro de pagos
  - `asistencias` - Control de entrada/salida
  - `clases` - Clases disponibles
  - `clase_inscripciones` - Inscripciones a clases

**Nota**: La base de datos se crea automáticamente al ejecutar la aplicación por primera vez.

---

## 🔧 Configuración de Email (Opcional)

Para habilitar notificaciones por email:

1. Obtener una **API Key de Brevo** (https://www.brevo.com/)
2. En la aplicación → Configuración → Ingresar API Key y email remitente
3. La aplicación enviará confirmaciones de pagos y bienvenidas automáticamente

---

## ⚠️ Problemas Comunes y Soluciones

### Problema: "dotnet: El término no se reconoce"
**Solución**: Instalar .NET 8 SDK desde https://dotnet.microsoft.com/download/dotnet/8.0

### Problema: "No se puede conectar a la base de datos"
**Solución**: Eliminar el archivo `gimnasio.db` en la carpeta bin/ para que se recree

### Problema: "Error al compilar"
**Solución**: Ejecutar:
```powershell
dotnet clean
dotnet restore
dotnet build
```

### Problema: "El puerto está en uso"
**Solución**: La app de escritorio no usa puertos. Si usas consola, cambiar puerto en `Program.cs`

---

## 📱 Características Principales

✅ Interface moderna con diseño responsivo  
✅ Gestión completa de socios y planes  
✅ Control de asistencia en tiempo real  
✅ Reportes y estadísticas  
✅ Notificaciones por email automáticas  
✅ Sistema de roles y permisos  
✅ Base de datos SQLite (sin instalación adicional)  
✅ Respaldo automático de datos  

---

## 🛠️ Desarrollo y Compilación Avanzada

### Compilar en modo Release (optimizado):
```powershell
dotnet build -c Release
```

### Publicar como ejecutable:
```powershell
dotnet publish -c Release -o "./publish"
```

### Ejecutar pruebas:
```powershell
dotnet test
```

---

## 📝 Notas Finales

- Asegúrate de tener permisos de escritura en la carpeta del proyecto (para crear la BD)
- Para la aplicación de escritorio se requiere Windows con soporte para .NET 8
- Mantener las credenciales por defecto solo en desarrollo
- Hacer respaldos periódicos del archivo `gimnasio.db`

---

## 🆘 Soporte

Si encuentras problemas:
1. Verifica que .NET 8 esté instalado correctamente
2. Elimina la carpeta `bin/` y compila nuevamente
3. Revisa los logs en la consola para mensajes de error

---

**Última actualización**: 2 de diciembre de 2025  
**Versión**: 1.0
