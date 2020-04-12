using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Streamdeck_vJoy
{
    [PluginActionId("com.streamdeck.vjoy.vjoybutton")]
    public class vJoyButton : PluginBase
    {
        static private vJoyInterfaceWrap.vJoy _virtualJoystick = new vJoyInterfaceWrap.vJoy();
        static private bool _virtualJoystickAcquired = false;
        public static int xAxisVal= 0;
        public static Dictionary<HID_USAGES,int> axisValues = new Dictionary<HID_USAGES, int>();


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


                instance.setToMin = false;
                instance.setToMax= true;
                instance.setToCenter = false;
                instance.setToCustom = false;
                instance.setToStepUp = false;
                instance.setToStepDown = false;
                instance.setToCustomValue = "";
                instance.setStepUp = "";
                instance.setStepDown = "";

                instance.resetToMin = true;
                instance.resetToMax = false;
                instance.resetToCenter = false;
                instance.resetToCustom= false;
                instance.resetToStepUp = false;
                instance.resetToStepDown = false;
                instance.resetToCustomValue = "";
                instance.resetStepUp = "";
                instance.resetStepDown = "";
                instance.resetDoNothing = false;

                    
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

        #region Private Members

        private PluginSettings settings;

        #endregion
        public vJoyButton(SDConnection connection, InitialPayload payload) : base(connection, payload)
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
            _virtualJoystick.SetAxis(axValue, Convert.ToUInt32(settings.vJoyDeviceId), axis);
        }

        public override void KeyPressed(KeyPayload payload)
        {
            if (settings.triggerPush || settings.triggerPushAndRelease)
            {
                if (_virtualJoystick == null || !_virtualJoystickAcquired)
                {
                    _virtualJoystick.AcquireVJD(Convert.ToUInt32(settings.vJoyDeviceId));
                }


                if (settings.vJoyElementType == "btn")
                {
                    // Es ist ein Button
                    _virtualJoystick.SetBtn(true, Convert.ToUInt32(settings.vJoyDeviceId), Convert.ToUInt32(settings.vJoyButtonId));
                }
                else
                {
                    // Es ist eine Achse
                    var axisValue = 0;
                    HID_USAGES theAxis = getAxisNameByString(settings.vJoyElementType);


                    if (settings.setToMin)
                    {
                        axisValue = GetJoystickAxisMinValue(theAxis);
                    }
                    else if (settings.setToMax)
                    {
                        axisValue = GetJoystickAxisMaxValue(theAxis);
                    }
                    else if (settings.setToCenter)
                    {
                        axisValue = GetJoystickAxisCenter(theAxis);
                    }
                    else if (settings.setToCustom)
                    {
                        axisValue = Convert.ToInt32(settings.setToCustomValue);
                    }
                    else if (settings.setToStepUp)
                    {
                        axisValue = stepUpAxisValue(theAxis, settings.setStepUp);
                    }
                    else if (settings.setToStepDown)
                    {
                        axisValue = stepDownAxisValue(theAxis, settings.setStepDown);
                    }

                    setAxisValue(axisValue, theAxis);
                }
                _virtualJoystick?.RelinquishVJD(Convert.ToUInt32(settings.vJoyDeviceId));
            }
            
            
        }

        private int stepDownAxisValue(HID_USAGES theAxis, string stepValue)
        {
            var axisVal = 0;
            if (axisValues.ContainsKey(theAxis))
            {
                axisVal = axisValues[theAxis];
            }
            axisVal -= Convert.ToInt32(stepValue);
            if (axisVal < GetJoystickAxisMinValue(theAxis))
            {
                axisVal = GetJoystickAxisMinValue(theAxis);
            }
            axisValues[theAxis] = axisVal;
            return axisVal;
        }

        private int stepUpAxisValue(HID_USAGES theAxis, string stepValue)
        {
            var axisVal = 0;
            if (axisValues.ContainsKey(theAxis))
            {
                axisVal = axisValues[theAxis];
            }
            axisVal += Convert.ToInt32(stepValue);
            if (axisVal > GetJoystickAxisMaxValue(theAxis))
            {
                axisVal = GetJoystickAxisMaxValue(theAxis);
            }
            axisValues[theAxis] = axisVal;
            return axisVal;
        }

        private HID_USAGES getAxisNameByString(string settingsVJoyElementType)
        {
            HID_USAGES namedAxis = HID_USAGES.HID_USAGE_X;
            switch (settings.vJoyElementType)
            {
                case "ax":
                    namedAxis = HID_USAGES.HID_USAGE_X;
                    break;
                case "ay":
                    namedAxis = HID_USAGES.HID_USAGE_Y;
                    break;
                case "az":
                    namedAxis = HID_USAGES.HID_USAGE_Z;
                    break;
                case "rx":
                    namedAxis = HID_USAGES.HID_USAGE_RX;
                    break;
                case "ry":
                    namedAxis = HID_USAGES.HID_USAGE_RY;
                    break;
                case "rz":
                    namedAxis = HID_USAGES.HID_USAGE_RZ;
                    break;
                case "sl1":
                    namedAxis = HID_USAGES.HID_USAGE_SL0;
                    break;
                case "sl2":
                    namedAxis = HID_USAGES.HID_USAGE_SL1;
                    break;
            }

            return namedAxis;
        }

        public override void KeyReleased(KeyPayload payload)
        {
            if (settings.resetDoNothing)
                return;
            if (settings.triggerPushAndRelease || settings.triggerRelease)
            {
                if (_virtualJoystick == null || !_virtualJoystickAcquired)
                {
                    _virtualJoystick.AcquireVJD(Convert.ToUInt32(settings.vJoyDeviceId));
                }
                if (settings.vJoyElementType == "btn")
                {
                    // Es ist ein Button
                    _virtualJoystick.SetBtn(false, Convert.ToUInt32(settings.vJoyDeviceId), Convert.ToUInt32(settings.vJoyButtonId));
                }
                else
                {
                    // Es ist eine Achse
                    var axisValue = 0;
                    HID_USAGES theAxis = getAxisNameByString(settings.vJoyElementType);


                    if (settings.resetToMin)
                    {
                        axisValue = GetJoystickAxisMinValue(theAxis);
                    }
                    else if (settings.resetToMax)
                    {
                        axisValue = GetJoystickAxisMaxValue(theAxis);
                    }
                    else if (settings.resetToCenter)
                    {
                        axisValue = GetJoystickAxisCenter(theAxis);
                    }
                    else if (settings.resetToCustom)
                    {
                        axisValue = Convert.ToInt32(settings.resetToCustomValue);
                    }
                    else if (settings.resetToStepUp)
                    {
                        axisValue = stepUpAxisValue(theAxis, settings.resetStepUp);
                    }
                    else if (settings.resetToStepDown)
                    {
                        axisValue = stepDownAxisValue(theAxis, settings.resetStepDown);
                    }

                    setAxisValue(axisValue, theAxis);
                }
                _virtualJoystick?.RelinquishVJD(Convert.ToUInt32(settings.vJoyDeviceId));



            }
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