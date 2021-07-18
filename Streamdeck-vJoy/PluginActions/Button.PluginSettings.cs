using BarRaider.SdTools;
using Newtonsoft.Json;
using System;

namespace Streamdeck_vJoy.PluginActions
{
    public partial class Button
    {
        private class PluginSettings : ButtonBaseSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                Logger.Instance.LogMessage(TracingLevel.INFO, "CreateDefaultSettings started");

                var instance = new PluginSettings
                {
                    vJoyDeviceId = "",
                    vJoyButtonIds = "",
                    vJoyElementType = String.Empty,
                    chkResetAxisToCenterAfterButtonRelease = String.Empty,


                    setToMin = false,
                    setToMax = true,
                    setToCenter = false,
                    setToCustom = false,
                    setToStepUp = false,
                    setToStepDown = false,
                    setToCustomValue = "",
                    setStepUp = "",
                    setStepDown = "",

                    resetToMin = true,
                    resetToMax = false,
                    resetToCenter = false,
                    resetToCustom = false,
                    resetToStepUp = false,
                    resetToStepDown = false,
                    resetToCustomValue = "",
                    resetStepUp = "",
                    resetStepDown = "",
                    resetDoNothing = false,


                    triggerPushAndRelease = true,
                    triggerPush = true,
                    triggerRelease = true
                };

                return instance;
            }

            [JsonProperty(PropertyName = "vJoyElementType")]
            public string vJoyElementType { get; set; }

            [JsonProperty(PropertyName = "chkResetAxisToCenterAfterButtonRelease")]
            public string chkResetAxisToCenterAfterButtonRelease { get; set; }

            [JsonProperty(PropertyName = "resetToMin")]
            public bool resetToMin { get; set; }

            [JsonProperty(PropertyName = "resetToCenter")]
            public bool resetToCenter { get; set; }

            [JsonProperty(PropertyName = "resetToMax")]
            public bool resetToMax { get; set; }

            [JsonProperty(PropertyName = "resetToStepUp")]
            public bool resetToStepUp { get; set; }

            [JsonProperty(PropertyName = "resetToStepDown")]
            public bool resetToStepDown { get; set; }

            [JsonProperty(PropertyName = "resetToCustom")]
            public bool resetToCustom { get; set; }

            [JsonProperty(PropertyName = "resetToCustomValue")]
            public string resetToCustomValue { get; set; }

            [JsonProperty(PropertyName = "resetStepUp")]
            public string resetStepUp { get; set; }

            [JsonProperty(PropertyName = "resetStepDown")]
            public string resetStepDown { get; set; }

            [JsonProperty(PropertyName = "resetDoNothing")]
            public bool resetDoNothing { get; set; }

            [JsonProperty(PropertyName = "setToMin")]
            public bool setToMin { get; set; }

            [JsonProperty(PropertyName = "setToCenter")]
            public bool setToCenter { get; set; }

            [JsonProperty(PropertyName = "setToMax")]
            public bool setToMax { get; set; }

            [JsonProperty(PropertyName = "setToStepUp")]
            public bool setToStepUp { get; set; }

            [JsonProperty(PropertyName = "setToStepDown")]
            public bool setToStepDown { get; set; }

            [JsonProperty(PropertyName = "setToCustom")]
            public bool setToCustom { get; set; }

            [JsonProperty(PropertyName = "setToCustomValue")]
            public string setToCustomValue { get; set; }

            [JsonProperty(PropertyName = "setStepUp")]
            public string setStepUp { get; set; }

            [JsonProperty(PropertyName = "setStepDown")]
            public string setStepDown { get; set; }

            [JsonProperty(PropertyName = "triggerPushAndRelease")]
            public bool triggerPushAndRelease { get; set; }

            [JsonProperty(PropertyName = "triggerPush")]
            public bool triggerPush { get; set; }

            [JsonProperty(PropertyName = "triggerRelease")]
            public bool triggerRelease { get; set; }

        }
    }
}