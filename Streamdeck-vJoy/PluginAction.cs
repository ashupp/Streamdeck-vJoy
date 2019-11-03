using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vJoy.Wrapper;

namespace Streamdeck_vJoy
{
    [PluginActionId("streamdeck.vjoy.pluginaction")]
    public class PluginAction : PluginBase
    {
        private VirtualJoystick _virtualJoystick;

        private class PluginSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                PluginSettings instance = new PluginSettings();

                instance.vJoyDeviceId = String.Empty;
                instance.vJoyButtonId = String.Empty;
                return instance;
            }

            [FilenameProperty]
            [JsonProperty(PropertyName = "vJoyDeviceId")]
            public string vJoyDeviceId { get; set; }

            [JsonProperty(PropertyName = "vJoyButtonId")]
            public string vJoyButtonId { get; set; }
        }

        #region Private Members

        private PluginSettings settings;

        #endregion
        public PluginAction(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                this.settings = PluginSettings.CreateDefaultSettings();
            }
            else
            {
                this.settings = payload.Settings.ToObject<PluginSettings>();
            }
        }

        public override void Dispose()
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Destructor called");
            _virtualJoystick.Dispose();
        }

        public override void KeyPressed(KeyPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Key Pressed");
            _virtualJoystick = new VirtualJoystick(Convert.ToUInt32(settings.vJoyDeviceId));
            _virtualJoystick.Aquire();
            _virtualJoystick.SetJoystickButton(true, Convert.ToUInt32(settings.vJoyButtonId));

        }

        public override void KeyReleased(KeyPayload payload) { 
            _virtualJoystick.SetJoystickButton(false, Convert.ToUInt32(settings.vJoyButtonId));
            _virtualJoystick.Release();
        }

        public override void OnTick() { }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            Tools.AutoPopulateSettings(settings, payload.Settings);
            SaveSettings();
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }

        #region Private Methods

        private Task SaveSettings()
        {
            return Connection.SetSettingsAsync(JObject.FromObject(settings));
        }

        #endregion
    }
}