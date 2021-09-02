using BarRaider.SdTools;
using Newtonsoft.Json.Linq;
using System;

namespace Streamdeck_vJoy.PluginActions
{
    [PluginActionId("com.streamdeck.vjoy.buttontoggle")]
    partial class ButtonToggle : ButtonBase
    {
        #region Private Members

        private new PluginSettings settings => (PluginSettings)base.settings;
        private bool buttonIsCurrentlyPressed;

        #endregion

        public ButtonToggle(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            base.settings = payload.Settings == null || payload.Settings.Count == 0
                ? PluginSettings.CreateDefaultSettings()
                : payload.Settings.ToObject<PluginSettings>();

            ApplySettings();
        }


        public override void KeyPressed(KeyPayload payload)
        {
            if (settings.buttonProcessingOnPush)
            {
                ToggleJoystickButton();
            }
        }

        public override void KeyReleased(KeyPayload payload)
        {
            if (settings.buttonProcessingAfterReleased)
            {
                ToggleJoystickButton();
            }
        }

        #region Private Methods

        private void ToggleJoystickButton()
        {
            bool success = true;
            if (buttonIsCurrentlyPressed)
            {
                for (int i = 0; i < vJoyButtonIds.Length; i++)
                {
                    success &= joystick.SetBtn(false, vJoyDeviceId, vJoyButtonIds[i]);
                }

                if (success)
                {
                    Connection.SetStateAsync(0);
                    buttonIsCurrentlyPressed = false;
                }
            }
            else
            {
                for (int i = 0; i < vJoyButtonIds.Length; i++)
                {
                    success &= joystick.SetBtn(true, vJoyDeviceId, vJoyButtonIds[i]);
                }

                if (success)
                {
                    buttonIsCurrentlyPressed = true;
                    Connection.SetStateAsync(1);
                }
            }
        }

        #endregion
    }
}
