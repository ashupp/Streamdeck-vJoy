using BarRaider.SdTools;
using Newtonsoft.Json;

namespace Streamdeck_vJoy.PluginActions
{
    partial class ButtonToggle : ButtonBase
    {
        private class PluginSettings : ButtonBaseSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                Logger.Instance.LogMessage(TracingLevel.INFO, "CreateDefaultSettings ButtonToggle started");

                PluginSettings instance = new PluginSettings
                {
                    vJoyDeviceId = "",
                    vJoyButtonIds = "",
                    buttonProcessingOnPush = false,
                    buttonProcessingAfterReleased = true
                };

                return instance;
            }

            [JsonProperty(PropertyName = "buttonProcessingOnPush")]
            public bool buttonProcessingOnPush { get; set; }

            [JsonProperty(PropertyName = "buttonProcessingAfterReleased")]
            public bool buttonProcessingAfterReleased { get; set; }

        }
    }
}