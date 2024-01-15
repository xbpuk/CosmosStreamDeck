using BarRaider.SdTools;
using BarRaider.SdTools.Wrappers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace team.sparked.cosmos
{
    [PluginActionId("team.sparked.cosmos.pluginaction")]
    public class PluginAction : KeypadBase
    {
        private class PluginSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                PluginSettings instance = new PluginSettings();
                instance.apiKey = String.Empty;
                instance.userID = String.Empty;
                return instance;
            }

            [FilenameProperty]
            [JsonProperty(PropertyName = "apiKey")]
            public string apiKey { get; set; }

            [JsonProperty(PropertyName = "userID")]
            public string userID { get; set; }
        }

        private PluginSettings settings;

        public PluginAction(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                this.settings = PluginSettings.CreateDefaultSettings();
                SaveSettings();
            }
            else
            {
                this.settings = payload.Settings.ToObject<PluginSettings>();
            }
        }

        public override void Dispose()
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Destructor called");
        }

        public async override void KeyPressed(KeyPayload payload)
        {

            if (payload.State == 0)
            {
                string apiUrl = "https://api.sparked.team/time/clock_in";
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        if (!string.IsNullOrEmpty(settings.apiKey) && !string.IsNullOrEmpty(settings.userID))
                        {
                            client.DefaultRequestHeaders.Add("X-API-KEY", settings.apiKey);
                            client.DefaultRequestHeaders.Add("Cosmos-User-ID", settings.userID);
                        }
                        else
                        {
                            Logger.Instance.LogMessage(TracingLevel.ERROR, "[API] Api Key data is invalid.");
                        }

                        HttpResponseMessage response = await client.PostAsync(apiUrl, null);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseData = await response.Content.ReadAsStringAsync();
                            Logger.Instance.LogMessage(TracingLevel.INFO, "API response: " + responseData);
                        }
                        else
                        {
                            Logger.Instance.LogMessage(TracingLevel.INFO, "API response: " + response.StatusCode);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogMessage(TracingLevel.ERROR, "Error sending API request: " + ex.Message);
                }
            } else
            {
                string apiUrl = "https://api.sparked.team/time/clock_out";
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        if (!string.IsNullOrEmpty(settings.apiKey) && !string.IsNullOrEmpty(settings.userID))
                        {
                            client.DefaultRequestHeaders.Add("X-API-KEY", settings.apiKey);
                            client.DefaultRequestHeaders.Add("Cosmos-User-ID", settings.userID);
                        }
                        else
                        {
                            Logger.Instance.LogMessage(TracingLevel.ERROR, "[API] Api Key data is invalid.");
                        }

                        HttpResponseMessage response = await client.PostAsync(apiUrl, null);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseData = await response.Content.ReadAsStringAsync();
                            Logger.Instance.LogMessage(TracingLevel.INFO, "API response: " + responseData);
                        }
                        else
                        {
                            Logger.Instance.LogMessage(TracingLevel.INFO, "API response: " + response.StatusCode);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogMessage(TracingLevel.ERROR, "Error sending API request: " + ex.Message);
                }
            }
        }

        public override void KeyReleased(KeyPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Key Released");
        }

        public override void OnTick() { }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            Tools.AutoPopulateSettings(settings, payload.Settings);
            SaveSettings();
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
        {
            // Handle received global settings
        }

        private Task SaveSettings()
        {
            return Connection.SetSettingsAsync(JObject.FromObject(settings));
        }
    }
}
