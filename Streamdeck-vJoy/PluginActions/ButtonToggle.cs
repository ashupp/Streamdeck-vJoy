using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Streamdeck_vJoy.PluginActions
{
    [PluginActionId("com.streamdeck.vjoy.buttontoggle")]
    class ButtonToggle : PluginBase
    {
        #region Private Members
        private PluginSettings settings;
        private static readonly vJoyInterfaceWrap.vJoy _virtualJoystick = new vJoyInterfaceWrap.vJoy();
        private static bool _virtualJoystickAcquired;
        public bool ButtonIsCurrentlyPressed;
        public bool LockedForInput;
        #endregion

        #region PluginSettings
        private class PluginSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                PluginSettings instance = new PluginSettings();
                Logger.Instance.LogMessage(TracingLevel.INFO, "CreateDefaultSettings ButtonToggle started");
                instance.vJoyDeviceId = "";
                instance.vJoyButtonId = "";
                instance.buttonProcessingOnPush = false;
                instance.buttonProcessingAfterReleased = true;

                return instance;
            }

            [JsonProperty(PropertyName = "vJoyDeviceId")]
            public string vJoyDeviceId { get; set; }

            [JsonProperty(PropertyName = "vJoyButtonId")]
            public string vJoyButtonId { get; set; }

            [JsonProperty(PropertyName = "buttonProcessingOnPush")]
            public bool buttonProcessingOnPush { get; set; }

            [JsonProperty(PropertyName = "buttonProcessingAfterReleased")]
            public bool buttonProcessingAfterReleased { get; set; }

        }
        #endregion


        private void EnsureAcquireJoystick()
        {
            if (settings.vJoyDeviceId != "")
            {
                 _virtualJoystick.AcquireVJD(Convert.ToUInt32(settings.vJoyDeviceId));
            }
        }

        private void ReleaseJoystick()
        {
            if (settings.vJoyDeviceId != "")
            {
                _virtualJoystick.RelinquishVJD(Convert.ToUInt32(settings.vJoyDeviceId));
            }
        }

        private void ToggleJoystickButton()
        {
            EnsureAcquireJoystick();
            if (ButtonIsCurrentlyPressed)
            {
                if (_virtualJoystick.SetBtn(false, Convert.ToUInt32(settings.vJoyDeviceId), Convert.ToUInt32(settings.vJoyButtonId)))
                {
                    Connection.SetStateAsync(0);
                    ButtonIsCurrentlyPressed = false;
                }
            }
            else
            {
                if (_virtualJoystick.SetBtn(true, Convert.ToUInt32(settings.vJoyDeviceId), Convert.ToUInt32(settings.vJoyButtonId)))
                {
                    ButtonIsCurrentlyPressed = true;
                    Connection.SetStateAsync(1);
                }
            }
        }

        public ButtonToggle(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                settings = PluginSettings.CreateDefaultSettings();
            }
            else
            {
                settings = payload.Settings.ToObject<PluginSettings>();
            }
        }


        public override void KeyPressed(KeyPayload payload)
        {
            if (!LockedForInput && settings.buttonProcessingOnPush)
            {
                LockedForInput = true;
                ToggleJoystickButton();
            }
        }

        public override void KeyReleased(KeyPayload payload)
        {
            if (settings.buttonProcessingAfterReleased)
            {
                ToggleJoystickButton();
            }
            LockedForInput = false;
            ReleaseJoystick();
        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            Tools.AutoPopulateSettings(settings, payload.Settings);
            SaveSettings();
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }

        public override void OnTick() { }

        public override void Dispose()
        {
            ReleaseJoystick();
        }

        private void SaveSettings()
        {
            Connection.SetSettingsAsync(JObject.FromObject(settings));
        }
    }
}
