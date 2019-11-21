using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Streamdeck_vJoy
{
    [PluginActionId("streamdeck.vjoy.pluginaction")]
    public class PluginAction : PluginBase
    {
        static private vJoyInterfaceWrap.vJoy _virtualJoystick = new vJoyInterfaceWrap.vJoy();
        static private bool _virtualJoystickAcquired = false;


        private class PluginSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                Logger.Instance.LogMessage(TracingLevel.INFO, "CreateDefaultSettings started");
                PluginSettings instance = new PluginSettings();

                instance.vJoyDeviceId = "";
                instance.vJoyButtonId = "";
                instance.vJoyElementType = String.Empty;
                instance.chkResetAxisToCenterAfterButtonRelease = String.Empty;
                instance.resetToMin = true;
                instance.resetToMax = false;
                instance.resetToCenter = false;
                instance.resetToPrevious = false;

                instance.triggerPushAndRelease = true;
                instance.triggerPush = true;
                instance.triggerRelease = true;
                return instance;
            }


            [FilenameProperty]
            [JsonProperty(PropertyName = "vJoyDeviceId")]
            public string vJoyDeviceId { get; set; }

            [JsonProperty(PropertyName = "vJoyButtonId")]
            public string vJoyButtonId { get; set; }

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

            [JsonProperty(PropertyName = "resetToPrevious")]
            public bool resetToPrevious { get; set; }


            [JsonProperty(PropertyName = "triggerPushAndRelease")]
            public bool triggerPushAndRelease { get; set; }

            [JsonProperty(PropertyName = "triggerPush")]
            public bool triggerPush { get; set; }

            [JsonProperty(PropertyName = "triggerRelease")]
            public bool triggerRelease { get; set; }


            [JsonProperty(PropertyName = "previousValues")]
            public Dictionary<HID_USAGES,int> previousValues { get; set; }
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
            if (_virtualJoystickAcquired)
                _virtualJoystick?.RelinquishVJD(Convert.ToUInt32(settings.vJoyDeviceId));
        }

        private int GetJoystickAxisMinValue(HID_USAGES axis)
        {
            long MinValue = 0;
            _virtualJoystick.GetVJDAxisMin(Convert.ToUInt32(settings.vJoyDeviceId), axis, ref MinValue);
            return (int)MinValue;
        }

        private int GetJoystickAxisMaxValue(HID_USAGES axis)
        {
            long MaxValue = 0;
            _virtualJoystick.GetVJDAxisMax(Convert.ToUInt32(settings.vJoyDeviceId), axis, ref MaxValue);
            return (int)MaxValue;
        }

        private int GetJoystickAxisCenter(HID_USAGES axis)
        {
            long MaxValue = 0;
            _virtualJoystick.GetVJDAxisMax(Convert.ToUInt32(settings.vJoyDeviceId), axis, ref MaxValue);
            return (int)MaxValue / 2;
        }


        private int getAxisValueDependingOnSetting(HID_USAGES axis)
        {
            if (settings.resetToMin)
            {
                return GetJoystickAxisMinValue(axis);
            }

            if (settings.resetToCenter)
            {
                return GetJoystickAxisCenter(axis);
            }

            if (settings.resetToMax)
            {
                return GetJoystickAxisMaxValue(axis);
            }

            return 0;
        }

        private void setAxisValue(int axValue, HID_USAGES axis)
        {
            if (settings.resetToMin)
            {
                settings.previousValues[axis] = GetJoystickAxisMaxValue(axis);
            }

            if (settings.resetToCenter)
            {
                settings.previousValues[axis] = GetJoystickAxisCenter(axis);
            }

            if (settings.resetToMax)
            {
                settings.previousValues[axis] = GetJoystickAxisMaxValue(axis);
            }

            _virtualJoystick.SetAxis(axValue, Convert.ToUInt32(settings.vJoyDeviceId), axis);
        }

        public override void KeyPressed(KeyPayload payload)
        {
            if (settings.triggerPush || settings.triggerPushAndRelease)
            {
                if (_virtualJoystick == null || !_virtualJoystickAcquired)
                {
                    _virtualJoystick.AcquireVJD(Convert.ToUInt32(settings.vJoyDeviceId));
                    settings.previousValues = new Dictionary<HID_USAGES, int>();
                }

                switch (settings.vJoyElementType)
                {
                    case "ax":
                        setAxisValue(GetJoystickAxisMaxValue(HID_USAGES.HID_USAGE_X), HID_USAGES.HID_USAGE_X);
                        break;
                    case "ay":
                        setAxisValue(GetJoystickAxisMaxValue(HID_USAGES.HID_USAGE_Y), HID_USAGES.HID_USAGE_Y);
                        break;
                    case "az":
                        setAxisValue(GetJoystickAxisMaxValue(HID_USAGES.HID_USAGE_Z), HID_USAGES.HID_USAGE_Z);
                        break;
                    case "rx":
                        setAxisValue(GetJoystickAxisMaxValue(HID_USAGES.HID_USAGE_RX), HID_USAGES.HID_USAGE_RX);
                        break;
                    case "ry":
                        setAxisValue(GetJoystickAxisMaxValue(HID_USAGES.HID_USAGE_RY), HID_USAGES.HID_USAGE_RY);
                        break;
                    case "rz":
                        setAxisValue(GetJoystickAxisMaxValue(HID_USAGES.HID_USAGE_RZ), HID_USAGES.HID_USAGE_RZ);
                        break;
                    case "sl1":
                        setAxisValue(GetJoystickAxisMaxValue(HID_USAGES.HID_USAGE_SL0), HID_USAGES.HID_USAGE_SL0);
                        break;
                    case "sl2":
                        setAxisValue(GetJoystickAxisMaxValue(HID_USAGES.HID_USAGE_SL1), HID_USAGES.HID_USAGE_SL1);
                        break;
                    case "btn":
                    default:
                        _virtualJoystick.SetBtn(true, Convert.ToUInt32(settings.vJoyDeviceId), Convert.ToUInt32(settings.vJoyButtonId));
                        break;
                }
            }
            _virtualJoystick?.RelinquishVJD(Convert.ToUInt32(settings.vJoyDeviceId));
            
        }

        public override void KeyReleased(KeyPayload payload)
        {
            if (settings.triggerPushAndRelease || settings.triggerRelease)
            {
                if (_virtualJoystick == null || !_virtualJoystickAcquired)
                {
                    _virtualJoystick.AcquireVJD(Convert.ToUInt32(settings.vJoyDeviceId));
                    settings.previousValues = new Dictionary<HID_USAGES, int>();
                }

                switch (settings.vJoyElementType)
                {
                    case "ax":
                        setAxisValue(getAxisValueDependingOnSetting(HID_USAGES.HID_USAGE_X), HID_USAGES.HID_USAGE_X);
                            break;
                    case "ay":
                        setAxisValue(getAxisValueDependingOnSetting(HID_USAGES.HID_USAGE_Y), HID_USAGES.HID_USAGE_Y);
                        break;
                    case "az":
                        setAxisValue(getAxisValueDependingOnSetting(HID_USAGES.HID_USAGE_Z), HID_USAGES.HID_USAGE_Z);
                        break;
                    case "rx":
                        setAxisValue(getAxisValueDependingOnSetting(HID_USAGES.HID_USAGE_RX), HID_USAGES.HID_USAGE_RX);
                        break;
                    case "ry":
                        setAxisValue(getAxisValueDependingOnSetting(HID_USAGES.HID_USAGE_RY), HID_USAGES.HID_USAGE_RY);
                        break;
                    case "rz":
                        setAxisValue(getAxisValueDependingOnSetting(HID_USAGES.HID_USAGE_RZ), HID_USAGES.HID_USAGE_RZ);
                        break;
                    case "sl1":
                        setAxisValue(getAxisValueDependingOnSetting(HID_USAGES.HID_USAGE_SL0), HID_USAGES.HID_USAGE_SL0);
                        break;
                    case "sl2":
                        setAxisValue(getAxisValueDependingOnSetting(HID_USAGES.HID_USAGE_SL1), HID_USAGES.HID_USAGE_SL1);
                        break;
                    case "btn":
                    default:
                        _virtualJoystick.SetBtn(false, Convert.ToUInt32(settings.vJoyDeviceId), Convert.ToUInt32(settings.vJoyButtonId));
                        break;
                }
            }
            _virtualJoystick?.RelinquishVJD(Convert.ToUInt32(settings.vJoyDeviceId));
        }

        public override void OnTick()
        {
        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            Tools.AutoPopulateSettings(settings, payload.Settings);
            SaveSettings();
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
        {
        }

        #region Private Methods

        private Task SaveSettings()
        {
            return Connection.SetSettingsAsync(JObject.FromObject(settings));
        }

        #endregion
    }
}