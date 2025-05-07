using Newtonsoft.Json;
using Plugin.Firebase.CloudMessaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteoApp.Services
{
    public class NotificationService
    {
        const string NOTIFICATION_SERVER = "https://notificationserver-production-7580.up.railway.app";

        private async Task<string> GetToken()
        {
            var token = string.Empty;

#if ANDROID 
            // Ask to enable Notifications
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.PostNotifications>();
            }

            await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
            token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();

            // Condividi il token
            /*
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = token,
                Title = "Firebase Token"
            });
            */
#endif
            return token;
        }

        public async Task RegisterTokenAsync()
        {
            var token = await GetToken();

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Token non disponibile.");
                return;
            }

            var serverUrl = NOTIFICATION_SERVER + "/register";

            using var httpClient = new HttpClient();

            var payload = new
            {
                token = token
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync(serverUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Token registrato con successo.");
                }
                else
                {
                    Console.WriteLine($"Errore nella registrazione del token: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eccezione durante la registrazione del token: {ex.Message}");
            }
        }

        public static string getNotificationServerAddress()
        {
            return NOTIFICATION_SERVER;
        }
    }
}
