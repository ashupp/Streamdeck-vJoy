using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BarRaider.SdTools.Payloads;

namespace Streamdeck_vJoy
{
    [PluginActionId("com.streamdeck.vjoy.vjoydialaxis")]
    public class vJoyDialAxis : EncoderBase
    {
        static private vJoyInterfaceWrap.vJoy _virtualJoystick = new vJoyInterfaceWrap.vJoy();
        static private bool _virtualJoystickAcquired = false;
        public static int xAxisVal = 0;
        public static Dictionary<HID_USAGES, int> axisValues = new Dictionary<HID_USAGES, int>();


        private class PluginSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                Logger.Instance.LogMessage(TracingLevel.INFO, "CreateDefaultSettings started");
                PluginSettings instance = new PluginSettings();

                instance.vJoyDeviceId = "";
                instance.vJoyButtonId = "";
                instance.vJoyElementType = String.Empty;
                instance.vJoyButtonDebounceLeftRight = "50";
                instance.chkResetAxisToCenterAfterButtonRelease = String.Empty;

                instance.setToMinLeft = true;
                instance.setToMaxLeft = false;
                instance.setToCenterLeft = false;
                instance.setToCustomLeft = false;
                instance.setToStepUpLeft = false;
                instance.setToStepDownLeft = false;
                instance.setToCustomValueLeft = "";
                instance.setStepUpLeft = "";
                instance.setStepDownLeft = "";

                instance.setToMinRight = false;
                instance.setToMaxRight = true;
                instance.setToCenterRight = false;
                instance.setToCustomRight = false;
                instance.setToStepUpRight = false;
                instance.setToStepDownRight = false;
                instance.setToCustomValueRight = "";
                instance.setStepUpRight = "";
                instance.setStepDownRight = "";


                instance.setToMin = false;
                instance.setToMax = true;
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
                instance.resetToCustom = false;
                instance.resetToStepUp = false;
                instance.resetToStepDown = false;
                instance.resetToCustomValue = "";
                instance.resetStepUp = "";
                instance.resetStepDown = "";
                instance.resetDoNothing = false;

                instance.showStateOnDisplay = false;
                instance.showValueAsPercent = false;
                instance.displayScaleMin = "";
                instance.displayScaleMax = "";

                return instance;
            }

            [FilenameProperty]
            [JsonProperty(PropertyName = "vJoyDeviceId")]
            public string vJoyDeviceId { get; set; }

            [JsonProperty(PropertyName = "vJoyButtonId")]
            public string vJoyButtonId { get; set; }

            [JsonProperty(PropertyName = "vJoyElementType")]
            public string vJoyElementType { get; set; }

            [JsonProperty(PropertyName = "vJoyButtonDebounceLeftRight")]
            public string vJoyButtonDebounceLeftRight { get; set; }

            [JsonProperty(PropertyName = "chkResetAxisToCenterAfterButtonRelease")]
            public string chkResetAxisToCenterAfterButtonRelease { get; set; }

            /* Turn left */
            [JsonProperty(PropertyName = "setToMinLeft")]
            public bool setToMinLeft { get; set; }

            [JsonProperty(PropertyName = "setToCenterLeft")]
            public bool setToCenterLeft { get; set; }

            [JsonProperty(PropertyName = "setToMaxLeft")]
            public bool setToMaxLeft { get; set; }

            [JsonProperty(PropertyName = "setToStepUpLeft")]
            public bool setToStepUpLeft { get; set; }

            [JsonProperty(PropertyName = "setToStepDownLeft")]
            public bool setToStepDownLeft { get; set; }

            [JsonProperty(PropertyName = "setToCustomLeft")]
            public bool setToCustomLeft { get; set; }

            [JsonProperty(PropertyName = "setToCustomValueLeft")]
            public string setToCustomValueLeft { get; set; }

            [JsonProperty(PropertyName = "setStepUpLeft")]
            public string setStepUpLeft { get; set; }

            [JsonProperty(PropertyName = "setStepDownLeft")]
            public string setStepDownLeft { get; set; }


            /* Turn right */
            [JsonProperty(PropertyName = "setToMinRight")]
            public bool setToMinRight { get; set; }

            [JsonProperty(PropertyName = "setToCenterRight")]
            public bool setToCenterRight { get; set; }

            [JsonProperty(PropertyName = "setToMaxRight")]
            public bool setToMaxRight { get; set; }

            [JsonProperty(PropertyName = "setToStepUpRight")]
            public bool setToStepUpRight { get; set; }

            [JsonProperty(PropertyName = "setToStepDownRight")]
            public bool setToStepDownRight { get; set; }

            [JsonProperty(PropertyName = "setToCustomRight")]
            public bool setToCustomRight { get; set; }

            [JsonProperty(PropertyName = "setToCustomValueRight")]
            public string setToCustomValueRight { get; set; }

            [JsonProperty(PropertyName = "setStepUpRight")]
            public string setStepUpRight { get; set; }

            [JsonProperty(PropertyName = "setStepDownRight")]
            public string setStepDownRight { get; set; }

            /* Knob release */

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

            /* Knob push */

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

            /* Display */
            [JsonProperty(PropertyName = "showStateOnDisplay")]
            public bool showStateOnDisplay { get; set; }

            [JsonProperty(PropertyName = "showValueAsPercent")]
            public bool showValueAsPercent { get; set; }

            [JsonProperty(PropertyName = "displayScaleMin")]
            public string displayScaleMin { get; set; }

            [JsonProperty(PropertyName = "displayScaleMax")]
            public string displayScaleMax { get; set; }

        }

        #region Private Members

        private PluginSettings settings;
        private SDConnection _connection;
        private int percentage;

        #endregion
        public vJoyDialAxis(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            _connection = connection;
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

        private void UpdateAxisDisplay(int axisValue, HID_USAGES axis)
        {
            if (!settings.showStateOnDisplay)
                return;

            int minValue = GetJoystickAxisMinValue(axis);
            int maxValue = GetJoystickAxisMaxValue(axis);
            int indicatorPercentage = 0;
            if (maxValue > minValue)
            {
                indicatorPercentage = (int)Math.Round((axisValue - minValue) * 100.0 / (maxValue - minValue));
            }
            if (indicatorPercentage < 0) indicatorPercentage = 0;
            if (indicatorPercentage > 100) indicatorPercentage = 100;

            string valueText;
            if (settings.showValueAsPercent)
            {
                valueText = indicatorPercentage + "%";
            }
            else
            {
                // Optionally map the axis range onto a custom display scale (e.g. 1-100)
                double displayValue = axisValue;
                if (maxValue > minValue && TryGetDisplayScale(out double scaleMin, out double scaleMax))
                {
                    displayValue = scaleMin + (axisValue - minValue) * (scaleMax - scaleMin) / (maxValue - minValue);
                }

                // Non-percent values are always rounded to whole numbers
                valueText = Math.Round(displayValue, MidpointRounding.AwayFromZero).ToString("0", System.Globalization.CultureInfo.InvariantCulture);
            }

            _ = _connection.SetFeedbackAsync(new Dictionary<string, string>
            {
                { "value", valueText },
                { "indicator", indicatorPercentage.ToString() }
            });
        }

        private bool TryGetDisplayScale(out double scaleMin, out double scaleMax)
        {
            scaleMin = 0;
            scaleMax = 0;

            if (String.IsNullOrEmpty(settings.displayScaleMin) || String.IsNullOrEmpty(settings.displayScaleMax))
                return false;

            // Accept both decimal comma and decimal point
            var style = System.Globalization.NumberStyles.Float;
            var culture = System.Globalization.CultureInfo.InvariantCulture;
            return double.TryParse(settings.displayScaleMin.Trim().Replace(",", "."), style, culture, out scaleMin)
                && double.TryParse(settings.displayScaleMax.Trim().Replace(",", "."), style, culture, out scaleMax)
                && scaleMin != scaleMax;
        }

        private void UpdateButtonDisplay(bool pressed)
        {
            if (!settings.showStateOnDisplay)
                return;

            _ = _connection.SetFeedbackAsync(new Dictionary<string, string>
            {
                { "value", "B" + settings.vJoyButtonId + (pressed ? " ON" : " OFF") },
                { "indicator", pressed ? "100" : "0" }
            });
        }

        private void ClearDisplay()
        {
            _ = _connection.SetFeedbackAsync(new Dictionary<string, string>
            {
                { "value", "" },
                { "indicator", "0" }
            });
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
        public override void OnTick()
        {
        }

        public override void DialRotate(DialRotatePayload payload)
        {
                if (_virtualJoystick == null || !_virtualJoystickAcquired)
                {
                    _virtualJoystick.AcquireVJD(Convert.ToUInt32(settings.vJoyDeviceId));
                }

                    // Es ist eine Achse
                    var axisValue = 0;
                    HID_USAGES theAxis = getAxisNameByString(settings.vJoyElementType);

                    
                    // Links oder rechts drehen:
                    if (payload.Ticks > 0)
                    {
                        if (settings.setToMinRight)
                        {
                            axisValue = GetJoystickAxisMinValue(theAxis);
                        }
                        else if (settings.setToMaxRight)
                        {
                            axisValue = GetJoystickAxisMaxValue(theAxis);
                        }
                        else if (settings.setToCenterRight)
                        {
                            axisValue = GetJoystickAxisCenter(theAxis);
                        }
                        else if (settings.setToCustomRight)
                        {
                            axisValue = Convert.ToInt32(settings.setToCustomValueRight);
                        }
                        else if (settings.setToStepUpRight)
                        {
                            axisValue = stepUpAxisValue(theAxis, settings.setStepUpRight);
                        }
                        else if (settings.setToStepDownRight)
                        {
                            axisValue = stepDownAxisValue(theAxis, settings.setStepDownRight);
                        }
                    }
                    if(payload.Ticks < 0)
                    {
                        if (settings.setToMinLeft)
                        {
                            axisValue = GetJoystickAxisMinValue(theAxis);
                        }
                        else if (settings.setToMaxLeft)
                        {
                            axisValue = GetJoystickAxisMaxValue(theAxis);
                        }
                        else if (settings.setToCenterLeft)
                        {
                            axisValue = GetJoystickAxisCenter(theAxis);
                        }
                        else if (settings.setToCustomLeft)
                        {
                            axisValue = Convert.ToInt32(settings.setToCustomValueLeft);
                        }
                        else if (settings.setToStepUpLeft)
                        {
                            axisValue = stepUpAxisValue(theAxis, settings.setStepUpLeft);
                        }
                        else if (settings.setToStepDownLeft)
                        {
                            axisValue = stepDownAxisValue(theAxis, settings.setStepDownLeft);
                        }
                    }

                setAxisValue(axisValue, theAxis);
                axisValues[theAxis] = axisValue;
                _virtualJoystick?.RelinquishVJD(Convert.ToUInt32(settings.vJoyDeviceId));
                UpdateAxisDisplay(axisValue, theAxis);

        }



        public override void DialDown(DialPayload payload)
        {
            _ = ButtonPressAsync(false);
        }

        private async Task ButtonPressAsync(bool autoreset = false)
        {
            if (_virtualJoystick == null || !_virtualJoystickAcquired)
            {
                _virtualJoystick.AcquireVJD(Convert.ToUInt32(settings.vJoyDeviceId));
            }


            if (settings.vJoyElementType == "btn")
            {
                // Es ist ein Button
                _virtualJoystick.SetBtn(true, Convert.ToUInt32(settings.vJoyDeviceId), Convert.ToUInt32(settings.vJoyButtonId));
                UpdateButtonDisplay(true);

                if (autoreset)
                {
                    await Task.Delay(60).ContinueWith(_ =>
                    {
                        _virtualJoystick.SetBtn(false, Convert.ToUInt32(settings.vJoyDeviceId), Convert.ToUInt32(settings.vJoyButtonId));
                        UpdateButtonDisplay(false);
                    });
                }
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
                axisValues[theAxis] = axisValue;
                UpdateAxisDisplay(axisValue, theAxis);
            }
            _virtualJoystick?.RelinquishVJD(Convert.ToUInt32(settings.vJoyDeviceId));
        }

        public override void DialUp(DialPayload payload)
        {
            if (settings.resetDoNothing)
                return;

            if (_virtualJoystick == null || !_virtualJoystickAcquired)
            {
                _virtualJoystick.AcquireVJD(Convert.ToUInt32(settings.vJoyDeviceId));
            }
            if (settings.vJoyElementType == "btn")
            {
                // Es ist ein Button
                _virtualJoystick.SetBtn(false, Convert.ToUInt32(settings.vJoyDeviceId), Convert.ToUInt32(settings.vJoyButtonId));
                UpdateButtonDisplay(false);
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
                axisValues[theAxis] = axisValue;
                UpdateAxisDisplay(axisValue, theAxis);
            }
            _virtualJoystick?.RelinquishVJD(Convert.ToUInt32(settings.vJoyDeviceId));
        }

        public override void TouchPress(TouchpadPressPayload payload)
        {
            _ = ButtonPressAsync(true);
        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            Tools.AutoPopulateSettings(settings, payload.Settings);
            SaveSettings();

            // Restore the default display when the state display gets disabled
            if (!settings.showStateOnDisplay)
            {
                ClearDisplay();
            }
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