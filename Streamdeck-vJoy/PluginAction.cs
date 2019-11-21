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
        private Dictionary<Axis, int> _previousValues;

        private class PluginSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                Logger.Instance.LogMessage(TracingLevel.INFO, "CreateDefaultSettings started");
                PluginSettings instance = new PluginSettings();

                instance.vJoyDeviceId = String.Empty;
                instance.vJoyButtonId = String.Empty;
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
        }

        #region Private Members

        private PluginSettings settings;

        #endregion
        public PluginAction(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "PluginAction constructor");
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
            if (_virtualJoystick != null && _virtualJoystick.Aquired)
            {
                _virtualJoystick.Release();
            }
            _virtualJoystick.Dispose();
        }


        private int getAxisValueDependingOnSetting(Axis axis)
        {
            if (settings.resetToMin)
            {
                return _virtualJoystick.GetJoystickAxisMinValue(axis);
            }

            if (settings.resetToCenter)
            {
                return _virtualJoystick.GetJoystickAxisCenter(axis);
            }

            if (settings.resetToMax)
            {
                return _virtualJoystick.GetJoystickAxisMaxValue(axis);
            }

            if (settings.resetToPrevious)
            {
                return _virtualJoystick.GetJoystickAxisValue(axis);
            }

            return 0;
        }

        private void setAxisValue(int axValue, Axis axis)
        {
            if (settings.resetToMin)
            {
                _previousValues[axis] = _virtualJoystick.GetJoystickAxisMinValue(axis);
            }

            if (settings.resetToCenter)
            {
                _previousValues[axis] = _virtualJoystick.GetJoystickAxisCenter(axis);
            }

            if (settings.resetToMax)
            {
                _previousValues[axis] = _virtualJoystick.GetJoystickAxisMaxValue(axis);
            }

            if (settings.resetToPrevious)
            {
                _previousValues[axis] = _virtualJoystick.GetJoystickAxisValue(axis);
            }

            _virtualJoystick.SetJoystickAxis(axValue, axis);
        }

        public override void KeyPressed(KeyPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Key Pressed");
            if (settings.triggerPush || settings.triggerPushAndRelease)
            {
                Logger.Instance.LogMessage(TracingLevel.INFO, "triggerPush or triggerPushAndRelease");
                if (_virtualJoystick == null || !_virtualJoystick.Aquired)
                {
                    Logger.Instance.LogMessage(TracingLevel.INFO, "setting virtualjoystick and previousvalues");
                    _virtualJoystick = new VirtualJoystick(Convert.ToUInt32(settings.vJoyDeviceId));
                    _virtualJoystick.Aquire();
                    _previousValues = new Dictionary<Axis, int>();
                }

                switch (settings.vJoyElementType)
                {
                    case "ax":
                        setAxisValue(_virtualJoystick.GetJoystickAxisMaxValue(Axis.HID_USAGE_X), Axis.HID_USAGE_X);
                        break;
                    case "ay":
                        setAxisValue(_virtualJoystick.GetJoystickAxisMaxValue(Axis.HID_USAGE_Y), Axis.HID_USAGE_Y);
                        break;
                    case "az":
                        setAxisValue(_virtualJoystick.GetJoystickAxisMaxValue(Axis.HID_USAGE_Z), Axis.HID_USAGE_Z);
                        break;
                    case "rx":
                        setAxisValue(_virtualJoystick.GetJoystickAxisMaxValue(Axis.HID_USAGE_RX), Axis.HID_USAGE_RX);
                        break;
                    case "ry":
                        setAxisValue(_virtualJoystick.GetJoystickAxisMaxValue(Axis.HID_USAGE_RY), Axis.HID_USAGE_RY);
                        break;
                    case "rz":
                        setAxisValue(_virtualJoystick.GetJoystickAxisMaxValue(Axis.HID_USAGE_RZ), Axis.HID_USAGE_RZ);
                        break;
                    case "sl1":
                        setAxisValue(_virtualJoystick.GetJoystickAxisMaxValue(Axis.HID_USAGE_SL0), Axis.HID_USAGE_SL0);
                        break;
                    case "sl2":
                        setAxisValue(_virtualJoystick.GetJoystickAxisMaxValue(Axis.HID_USAGE_SL1), Axis.HID_USAGE_SL1);
                        break;
                    case "btn":
                    default:
                        _virtualJoystick.SetJoystickButton(true, Convert.ToUInt32(settings.vJoyButtonId));
                        break;
                }
            }
            _virtualJoystick?.Release();
        }

        public override void KeyReleased(KeyPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Key released");
            if (settings.triggerPushAndRelease || settings.triggerRelease)
            {
                Logger.Instance.LogMessage(TracingLevel.INFO, "triggerRelease or triggerPushAndRelease");
                if (_virtualJoystick == null || !_virtualJoystick.Aquired)
                {
                    Logger.Instance.LogMessage(TracingLevel.INFO, "setting virtualjoystick and previousvalues");
                    _virtualJoystick = new VirtualJoystick(Convert.ToUInt32(settings.vJoyDeviceId));
                    _virtualJoystick.Aquire();
                    _previousValues = new Dictionary<Axis, int>();
                }

                switch (settings.vJoyElementType)
                {
                    case "ax":
                        if (_previousValues != null && _previousValues.ContainsKey(Axis.HID_USAGE_X))
                        {
                            Logger.Instance.LogMessage(TracingLevel.INFO, "X release value vorhanden");
                            setAxisValue(_previousValues[Axis.HID_USAGE_X], Axis.HID_USAGE_X);
                        }
                        else
                        {
                            Logger.Instance.LogMessage(TracingLevel.INFO, "X release ELSE");
                            setAxisValue(getAxisValueDependingOnSetting(Axis.HID_USAGE_X), Axis.HID_USAGE_X);
                        }
                            
                        break;
                    case "ay":
                        if (_previousValues != null && _previousValues[Axis.HID_USAGE_Y] != null)
                        {
                            setAxisValue(_previousValues[Axis.HID_USAGE_Y], Axis.HID_USAGE_Y);
                        }
                        else
                        {
                            setAxisValue(getAxisValueDependingOnSetting(Axis.HID_USAGE_Y), Axis.HID_USAGE_Y);
                        }
                            
                        break;
                    case "az":
                        setAxisValue(_previousValues[Axis.HID_USAGE_Z], Axis.HID_USAGE_Z);
                        break;
                    case "rx":
                        setAxisValue(_previousValues[Axis.HID_USAGE_RX], Axis.HID_USAGE_RX);
                        break;
                    case "ry":
                        setAxisValue(_previousValues[Axis.HID_USAGE_RY], Axis.HID_USAGE_RY);
                        break;
                    case "rz":
                        setAxisValue(_previousValues[Axis.HID_USAGE_RZ], Axis.HID_USAGE_RZ);
                        break;
                    case "sl1":
                        setAxisValue(_previousValues[Axis.HID_USAGE_SL0], Axis.HID_USAGE_SL0);
                        break;
                    case "sl2":
                        setAxisValue(_previousValues[Axis.HID_USAGE_SL1], Axis.HID_USAGE_SL1);
                        break;
                    case "btn":
                    default:
                        _virtualJoystick.SetJoystickButton(false, Convert.ToUInt32(settings.vJoyButtonId));
                        break;
                }
            }
            _virtualJoystick?.Release();
        }

        public override void OnTick()
        {
        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "ReceivedSettings");
            Tools.AutoPopulateSettings(settings, payload.Settings);
            SaveSettings();
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "ReceivedGlobalSettings");
        }

        #region Private Methods

        private Task SaveSettings()
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "SaveSettings");
            return Connection.SetSettingsAsync(JObject.FromObject(settings));
        }

        #endregion
    }
}