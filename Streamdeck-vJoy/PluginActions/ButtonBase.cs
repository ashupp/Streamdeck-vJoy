using BarRaider.SdTools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Streamdeck_vJoy.PluginActions
{
    public abstract partial class ButtonBase : PluginBase
    {
        #region Internal Members

        internal readonly Dictionary<HID_USAGES, int> axisValues;
        internal readonly vJoyInterfaceWrap.vJoy joystick;

        internal ButtonBaseSettings settings;
        internal uint[] vJoyButtonIds;
        internal uint vJoyDeviceId;

        readonly char[] comma = new char[] { ',' };

        #endregion

        public ButtonBase(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            joystick = new vJoyInterfaceWrap.vJoy();

            axisValues = new Dictionary<HID_USAGES, int>();
        }

        private static readonly System.Reflection.MethodInfo AutoPopInfo = typeof(Tools).GetMethod("AutoPopulateSettings");

        public sealed override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            var genAutoPopInfo = AutoPopInfo.MakeGenericMethod(settings.GetType());
            genAutoPopInfo.Invoke(null, new object[] { settings, payload.Settings });
            ApplySettings();
            Connection.SetSettingsAsync(JObject.FromObject(settings));
        }

        public override void OnTick()
        {
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
        {
        }

        public override void Dispose()
        {
            joystick?.RelinquishVJD(vJoyDeviceId);
        }

        internal void ApplySettings()
        {
            if (settings.vJoyDeviceId != "")
            {
                uint updatedDeviceId = Convert.ToUInt32(settings.vJoyDeviceId);

                if (updatedDeviceId != vJoyDeviceId)
                {
                    joystick.RelinquishVJD(vJoyDeviceId);
                    vJoyDeviceId = updatedDeviceId;
                    joystick.AcquireVJD(vJoyDeviceId);
                }

                vJoyButtonIds = Array.ConvertAll(RemoveExtraText(settings.vJoyButtonIds).Split(comma, StringSplitOptions.RemoveEmptyEntries), Convert.ToUInt32);

                Connection.SetSettingsAsync(JObject.FromObject(settings));
            }
        }

        #region Private Methods

        private string RemoveExtraText(string value)
        {
            var allowedChars = "01234567890,";
            return new string(value.Where(c => allowedChars.Contains(c)).ToArray());
        }

        #endregion
    }
}