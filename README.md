
```markdown
<p align="center">
  <img src="https://img.icons8.com/color/96/000000/security-checked.png" alt="ShadowLink Logo"/>
</p>

# 🌑 ShadowLink

**ShadowLink** es una herramienta de administración remota diseñada para controlar un PC a través de comandos enviados por Discord. Permite ejecutar comandos del sistema, gestionar archivos y procesos, y monitorear la actividad del equipo en tiempo real, todo desde la comodidad de tu servidor de Discord.

## ✨ Características Principales

- **Control Total:** Ejecuta comandos CMD y PowerShell remotamente
- **Gestión de Sistema:** Apaga, reinicia, bloquea o suspende el PC
- **Archivos y Procesos:** Lista, envía, elimina archivos y administra procesos del sistema
- **Monitoreo:** Captura la pantalla y toma fotos con la webcam remotamente
- **Control de Entrada:** Simula escritura de teclado y clicks del mouse
- **Seguridad:** Detecta y alerta sobre la presencia de herramientas de análisis o debugging

## 🚀 Comandos Rápidos

| Tipo | Ejemplo | Función |
| :--- | :--- | :--- |
| **Básico** | `!test` | Verifica que el bot esté online |
| **Sistema** | `/apagar` | Apaga el equipo remotamente |
| **Archivos** | `/mpc enviar C:\archivo.txt` | Envía un archivo a Discord |
| **Captura** | `/screenshot` | Toma captura de pantalla |
| **Procesos** | `/mpc matar 1234` | Mata un proceso por PID |
| **Teclado** | `/mpc escribir Hola` | Escribe texto remotamente |

> Para la lista completa de comandos, usa `!help`, `/helppc` o `/mpchelp` dentro del canal de Discord.

## 🛠️ Tecnologías Utilizadas

- **.NET Framework 4.7.2** - Plataforma de desarrollo
- **C#** - Lenguaje de programación principal
- **Discord API** - Para la comunicación con el bot
- **Newtonsoft.Json** - Procesamiento de datos JSON
- **Windows Forms** - Interfaz oculta

## 📦 Paquetes NuGet Necesarios

Para compilar y ejecutar ShadowLink, necesitas instalar los siguientes paquetes NuGet:

### Instalación desde Consola NuGet

Abre la **Consola del Administrador de paquetes** en Visual Studio (`Herramientas` → `Administrador de paquetes NuGet` → `Consola del Administrador de paquetes`) y ejecuta:

```powershell
Install-Package Newtonsoft.Json -Version 13.0.2
Install-Package System.Text.Json -Version 10.0.8
Install-Package System.Management
Install-Package System.Drawing.Common -Version 9.0.0
Install-Package DirectShowLib -Version 1.0.0
```

### Instalación desde la GUI de NuGet

1. Haz clic derecho en el proyecto → `Administrar paquetes NuGet`
2. Busca e instala cada uno de estos paquetes:

| Paquete | Versión | Uso |
|---------|---------|-----|
| `Newtonsoft.Json` | 13.0.2 | Procesamiento de JSON |
| `System.Text.Json` | 10.0.8 | JSON alternativo |
| `System.Management` | Última estable | Información del sistema |
| `System.Drawing.Common` | 9.0.0 | Captura de pantalla |
| `DirectShowLib` | 1.0.0 | Acceso a cámara web |

### Dependencias incluidas automáticamente

Estos paquetes se instalarán automáticamente con los anteriores:

- `Microsoft.Bcl.AsyncInterfaces`
- `System.Buffers`
- `System.Memory`
- `System.Numerics.Vectors`
- `System.Runtime.CompilerServices.Unsafe`
- `System.Threading.Tasks.Extensions`

## ⚙️ Configuración y Uso

### 1. Requisitos Previos

- [Visual Studio 2019 o 2022](https://visualstudio.microsoft.com/es/downloads/) (con la carga de trabajo de ".NET desktop development")
- [.NET Framework 4.7.2](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472) o superior
- Una cuenta de Discord con permisos para crear un bot

### 2. Configurar el Bot de Discord

1. Ve a [Discord Developer Portal](https://discord.com/developers/applications) y crea una "New Application"
2. Ve a la sección **Bot** y haz clic en "Reset Token". **Copia y guarda este token**
3. Habilita los "Privileged Gateway Intents" (especialmente `MESSAGE CONTENT INTENT`)
4. Ve a la sección **OAuth2 → URL Generator**
5. Selecciona `bot` y los permisos necesarios:
   - `Send Messages`
   - `Read Messages` / `View Channels`
   - `Read Message History`
   - `Attach Files`
   - `Manage Messages`
6. Usa la URL generada para invitar al bot a tu servidor

### 3. Configurar la Aplicación

1. Clona este repositorio o descarga el código
2. Abre la solución en Visual Studio
3. Restaura los paquetes NuGet:
   - Haz clic derecho en la solución → `Restaurar paquetes NuGet`
4. En el archivo `Form1.cs`, localiza las líneas donde se definen las constantes:

```csharp
private static readonly string BotToken = "AQUI_VA_TU_TOKEN";
private static readonly ulong CanalID = 123456789012345678; // ID del canal
```

5. Reemplaza `"AQUI_VA_TU_TOKEN"` con el token que copiaste
6. Reemplaza el número con el **ID de tu canal de Discord**:
   - Activa el "Modo Desarrollador" en Discord (`Ajustes` → `Avanzado`)
   - Haz clic derecho en el canal → `Copiar ID`

### 4. Compilar y Ejecutar

1. Compila la solución (`Ctrl + Shift + B`)
2. Ejecuta la aplicación (`F5`)
3. Verás que se inicia en segundo plano (sin ventana visible)
4. Escribe `!test` en el canal que configuraste. ¡El bot debería responder!

### 5. Verificar que funciona

Prueba estos comandos básicos:

| Comando | Respuesta esperada |
|---------|-------------------|
| `!test` | "✅ Bot funcionando correctamente!" |
| `!info` | Información del sistema |
| `!help` | Lista de comandos disponibles |

## 📁 Estructura del Proyecto

```
ShadowLink/
├── Form1.cs              # Clase principal del bot
├── ManejoPC.cs           # Comandos avanzados de control
├── ControlPC.cs          # Comandos básicos de control
├── Procesos.cs           # Gestión y monitoreo de procesos
├── Files.cs              # Gestión de archivos
├── FotoCam.cs            # Captura de cámara
├── WebCam.cs             # Captura de pantalla
├── SistemaSeguridad.cs   # Detección de amenazas
├── Program.cs            # Punto de entrada
└── App.config            # Configuración de la aplicación
```

## 🔧 Solución de Problemas

### Error: "Token inválido o expirado"
- El token de Discord es incorrecto o expiró
- Regenera el token en Discord Developer Portal y actualízalo en `Form1.cs`

### Error: "No se encontró el paquete X"
- Ejecuta `Update-Package -reinstall` en la consola NuGet
- O reinstala los paquetes manualmente

### Error: "Referencia a System.Management no encontrada"
- Agrega la referencia manualmente:
  - Haz clic derecho en `Referencias` → `Agregar referencia`
  - Busca `System.Management` en `Ensamblados` → `Marco`

### El bot no responde a los comandos
- Verifica que el bot esté en el canal correcto
- Asegúrate de que el `CanalID` sea el correcto
- Revisa la consola de Visual Studio para ver errores

## 🛡️ Seguridad

**IMPORTANTE:** 
- **NUNCA** subas tu token de Discord a GitHub
- El archivo `App.config` debe estar en `.gitignore`
- Si subiste tu token por error, revócalo inmediatamente en Discord Developer Portal

Para proteger tu token, usa variables de entorno:
```csharp
private static readonly string BotToken = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN");
```

## ⚠️ Advertencia y Responsabilidad

Este proyecto se desarrolla con **fines estrictamente educativos y de administración de sistemas propios**.

- Su uso no autorizado en equipos ajenos puede violar leyes de privacidad y computación
- El autor no se hace responsable del mal uso que se le pueda dar a esta herramienta
- Es responsabilidad del usuario cumplir con las leyes locales aplicables

**Úsalo de manera ética y solo en equipos donde tengas permiso explícito para hacerlo.**

## 🤝 Contribuciones

Las contribuciones, reportes de errores y sugerencias son bienvenidas. Por favor:

1. Haz un Fork del proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Haz commit de tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo `LICENSE` para más detalles.

## 👨‍💻 Desarrollador

Creado y mantenido por [MEMECRAFT12126](https://github.com/MEMECRAFT12126).

---

<p align="center">
  ⭐ Si este proyecto te ha sido útil, ¡no olvides darle una estrella en GitHub! ⭐
</p>

<p align="center">
  <sub>ShadowLink - Control remoto total por Discord</sub>
</p>
```

Este `README.md` incluye:

1. ✅ **Instrucciones completas** de instalación de paquetes NuGet
2. ✅ **Comandos para la consola NuGet** (copiar y pegar)
3. ✅ **Tabla de paquetes** con versiones específicas
4. ✅ **Solución de problemas** común
5. ✅ **Seguridad** (cómo proteger el token)
6. ✅ **Estructura del proyecto** explicada
7. ✅ **Guía de configuración** paso a paso
8. ✅ **Licencia y contribuciones**
