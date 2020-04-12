using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Streamdeck_vJoy.PluginActions
{
    [PluginActionId("streamdeck.vjoy.buttontoggle")]
    class ButtonToggle : PluginBase
    {
        static private vJoyInterfaceWrap.vJoy _virtualJoystick = new vJoyInterfaceWrap.vJoy();
        static private bool _virtualJoystickAcquired = false;
        public static int xAxisVal = 0;
        public static Dictionary<HID_USAGES, int> axisValues = new Dictionary<HID_USAGES, int>();
        public bool buttonPressed = false;


        private class PluginSettings
        {
            public static ButtonToggle.PluginSettings CreateDefaultSettings()
            {
                Logger.Instance.LogMessage(TracingLevel.INFO, "CreateDefaultSettings started");
                ButtonToggle.PluginSettings instance = new ButtonToggle.PluginSettings();

                instance.vJoyDeviceId = "";
                instance.vJoyButtonId = "";
                instance.triggerToggle = true;

                return instance;
            }

            [FilenameProperty]
            [JsonProperty(PropertyName = "vJoyDeviceId")]
            public string vJoyDeviceId { get; set; }

            [JsonProperty(PropertyName = "vJoyButtonId")]
            public string vJoyButtonId { get; set; }


            [JsonProperty(PropertyName = "triggerToggle")]
            public bool triggerToggle { get; set; }

        }

        #region Private Members

        private ButtonToggle.PluginSettings settings;

        #endregion

        public ButtonToggle(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
        }

        public override void KeyPressed(KeyPayload payload)
        {
            if (_virtualJoystick == null || !_virtualJoystickAcquired)
            {
                _virtualJoystick.AcquireVJD(Convert.ToUInt32(settings.vJoyDeviceId));
            }

            // Es ist ein Button
            if (buttonPressed)
            {
                _virtualJoystick.SetBtn(false, Convert.ToUInt32(settings.vJoyDeviceId), Convert.ToUInt32(settings.vJoyButtonId));
                buttonPressed = false;
                return;
            }

            buttonPressed = true;
            _virtualJoystick.SetBtn(true, Convert.ToUInt32(settings.vJoyDeviceId), Convert.ToUInt32(settings.vJoyButtonId));
        }

        public override void KeyReleased(KeyPayload payload)
        {
            //throw new NotImplementedException();
        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            Tools.AutoPopulateSettings(settings, payload.Settings);
            SaveSettings();
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
        {
            //throw new NotImplementedException();
        }

        public override void OnTick()
        {
            //throw new NotImplementedException();
        }

        public override void Dispose()
        {
            //throw new NotImplementedException();
        }

        private Task SaveSettings()
        {
            return Connection.SetSettingsAsync(JObject.FromObject(settings));
        }
    }
}
