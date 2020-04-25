using BarRaider.SdTools;
using Newtonsoft.Json;

namespace Streamdeck_vJoy.PluginActions
{
    public class ButtonBaseSettings
    {
        [JsonProperty(PropertyName = "vJoyDeviceId")]
        public string vJoyDeviceId { get; set; }

        [JsonProperty(PropertyName = "vJoyButtonIds")]
        public string vJoyButtonIds { get; set; }
    }
}