using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BarRaider.SdTools.Payloads;

namespace Streamdeck_vJoy
{
    [PluginActionId("com.streamdeck.vjoy.vjoydialbuttons")]
    public class vJoyDialButtons : EncoderBase
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
                instance.vJoyButtonIdTurnLeft = "";
                instance.vJoyButtonIdTurnRight = "";
                instance.vJoyButtonIdTouch = "";
                instance.vJoyButtonName = "";
                instance.vJoyButtonNameTurnLeft = "";
                instance.vJoyButtonNameTurnRight = "";
                instance.vJoyButtonNameTouch = "";
                instance.vJoyElementType = "btn";
                instance.vJoyButtonDebounceLeftRight = "50";
                instance.chkResetAxisToCenterAfterButtonRelease = String.Empty;


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


                instance.triggerPushAndRelease = true;
                instance.triggerPush = false;
                instance.triggerRelease = false;
                instance.triggerToggle = false;


                instance.triggerPushAndReleaseTurnLeft = true;
                instance.triggerPushTurnLeft = false;
                instance.triggerReleaseTurnLeft = false;

                instance.triggerPushAndReleaseTurnRight = true;
                instance.triggerPushTurnRight = false;
                instance.triggerReleaseTurnRight = false;

                instance.showStateOnDisplay = false;
                instance.displayTextOn = "";
                instance.displayTextOff = "";

                return instance;
            }

            [FilenameProperty]
            [JsonProperty(PropertyName = "vJoyDeviceId")]
            public string vJoyDeviceId { get; set; }

            [JsonProperty(PropertyName = "vJoyButtonId")]
            public string vJoyButtonId { get; set; }            
            
            [JsonProperty(PropertyName = "vJoyButtonDebounceLeftRight")]
            public string vJoyButtonDebounceLeftRight { get; set; }

            [JsonProperty(PropertyName = "vJoyButtonIdTurnLeft")]
            public string vJoyButtonIdTurnLeft { get; set; }

            [JsonProperty(PropertyName = "vJoyButtonIdTurnRight")]
            public string vJoyButtonIdTurnRight { get; set; }            
            
            [JsonProperty(PropertyName = "vJoyButtonIdTouch")]
            public string vJoyButtonIdTouch { get; set; }

            [JsonProperty(PropertyName = "vJoyButtonName")]
            public string vJoyButtonName { get; set; }

            [JsonProperty(PropertyName = "vJoyButtonNameTurnLeft")]
            public string vJoyButtonNameTurnLeft { get; set; }

            [JsonProperty(PropertyName = "vJoyButtonNameTurnRight")]
            public string vJoyButtonNameTurnRight { get; set; }

            [JsonProperty(PropertyName = "vJoyButtonNameTouch")]
            public string vJoyButtonNameTouch { get; set; }

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

            [JsonProperty(PropertyName = "triggerToggle")]
            public bool triggerToggle { get; set; }

            [JsonProperty(PropertyName = "triggerPushAndReleaseTurnLeft")]
            public bool triggerPushAndReleaseTurnLeft { get; set; }

            [JsonProperty(PropertyName = "triggerPushTurnLeft")]
            public bool triggerPushTurnLeft { get; set; }

            [JsonProperty(PropertyName = "triggerReleaseTurnLeft")]
            public bool triggerReleaseTurnLeft { get; set; }

            [JsonProperty(PropertyName = "triggerPushAndReleaseTurnRight")]
            public bool triggerPushAndReleaseTurnRight { get; set; }

            [JsonProperty(PropertyName = "triggerPushTurnRight")]
            public bool triggerPushTurnRight { get; set; }

            [JsonProperty(PropertyName = "triggerReleaseTurnRight")]
            public bool triggerReleaseTurnRight { get; set; }

            /* Display */
            [JsonProperty(PropertyName = "showStateOnDisplay")]
            public bool showStateOnDisplay { get; set; }

            [JsonProperty(PropertyName = "displayTextOn")]
            public string displayTextOn { get; set; }

            [JsonProperty(PropertyName = "displayTextOff")]
            public string displayTextOff { get; set; }

        }

        #region Private Members

        private PluginSettings settings;
        private SDConnection _connection;
        private bool _currentToggleStatus;
        private int percentage;

        #endregion
        public vJoyDialButtons(SDConnection connection, InitialPayload payload) : base(connection, payload)
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

        public override void OnTick()
        {
        }

        private string GetButtonStateText(uint buttonId, string buttonName, bool pressed)
        {
            string name = String.IsNullOrEmpty(buttonName) ? "B" + buttonId : buttonName;

            string customText = pressed ? settings.displayTextOn : settings.displayTextOff;
            if (!String.IsNullOrEmpty(customText))
            {
                // {id} = vJoy button id, {name} = configured button name (falls back to B<id>)
                return customText.Replace("{id}", buttonId.ToString()).Replace("{name}", name);
            }
            return name + (pressed ? " ON" : " OFF");
        }

        private void UpdateButtonDisplay(uint buttonId, string buttonName, bool pressed)
        {
            if (!settings.showStateOnDisplay)
                return;

            _ = _connection.SetFeedbackAsync(new Dictionary<string, string>
            {
                { "value", GetButtonStateText(buttonId, buttonName, pressed) },
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

        public override void Dispose()
        {
            if (_virtualJoystickAcquired)
                _virtualJoystick?.RelinquishVJD(Convert.ToUInt32(settings.vJoyDeviceId));
        }



        public override async void DialRotate(DialRotatePayload payload)
        {
            if (_virtualJoystick == null || !_virtualJoystickAcquired)
            {
                _virtualJoystick.AcquireVJD(Convert.ToUInt32(settings.vJoyDeviceId));
            }
            
            if (payload.Ticks > 0)
            {
                // Nach Rechts
                if (settings.triggerPushAndReleaseTurnRight)
                {
                    _virtualJoystick.SetBtn(true, Convert.ToUInt32(settings.vJoyDeviceId), Convert.ToUInt32(settings.vJoyButtonIdTurnRight));
                    UpdateButtonDisplay(Convert.ToUInt32(settings.vJoyButtonIdTurnRight), settings.vJoyButtonNameTurnRight, true);
                    await Task.Delay(Convert.ToInt32(this.settings.vJoyButtonDebounceLeftRight)).ContinueWith(_ =>
                    {
                        _virtualJoystick.SetBtn(false, Convert.ToUInt32(settings.vJoyDeviceId), Convert.ToUInt32(settings.vJoyButtonIdTurnRight));
                        UpdateButtonDisplay(Convert.ToUInt32(settings.vJoyButtonIdTurnRight), settings.vJoyButtonNameTurnRight, false);
                    });
                }

                if (settings.triggerPushTurnRight)
                {
                    _virtualJoystick.SetBtn(true, Convert.ToUInt32(settings.vJoyDeviceId), Convert.ToUInt32(settings.vJoyButtonIdTurnRight));
                    UpdateButtonDisplay(Convert.ToUInt32(settings.vJoyButtonIdTurnRight), settings.vJoyButtonNameTurnRight, true);
                }

                if (settings.triggerReleaseTurnRight)
                {
                    _virtualJoystick.SetBtn(false, Convert.ToUInt32(settings.vJoyDeviceId), Convert.ToUInt32(settings.vJoyButtonIdTurnRight));
                    UpdateButtonDisplay(Convert.ToUInt32(settings.vJoyButtonIdTurnRight), settings.vJoyButtonNameTurnRight, false);
                }
                
            }
            else
            {
                // Nach Links
                if (settings.triggerPushAndReleaseTurnLeft)
                {
                    _virtualJoystick.SetBtn(true, Convert.ToUInt32(settings.vJoyDeviceId), Convert.ToUInt32(settings.vJoyButtonIdTurnLeft));
                    UpdateButtonDisplay(Convert.ToUInt32(settings.vJoyButtonIdTurnLeft), settings.vJoyButtonNameTurnLeft, true);
                    await Task.Delay(Convert.ToInt32(this.settings.vJoyButtonDebounceLeftRight)).ContinueWith(_ =>
                    {
                        _virtualJoystick.SetBtn(false, Convert.ToUInt32(settings.vJoyDeviceId), Convert.ToUInt32(settings.vJoyButtonIdTurnLeft));
                        UpdateButtonDisplay(Convert.ToUInt32(settings.vJoyButtonIdTurnLeft), settings.vJoyButtonNameTurnLeft, false);
                    });
                }

                if (settings.triggerPushTurnLeft)
                {
                    _virtualJoystick.SetBtn(true, Convert.ToUInt32(settings.vJoyDeviceId), Convert.ToUInt32(settings.vJoyButtonIdTurnLeft));
                    UpdateButtonDisplay(Convert.ToUInt32(settings.vJoyButtonIdTurnLeft), settings.vJoyButtonNameTurnLeft, true);
                }

                if (settings.triggerReleaseTurnLeft)
                {
                    _virtualJoystick.SetBtn(false, Convert.ToUInt32(settings.vJoyDeviceId), Convert.ToUInt32(settings.vJoyButtonIdTurnLeft));
                    UpdateButtonDisplay(Convert.ToUInt32(settings.vJoyButtonIdTurnLeft), settings.vJoyButtonNameTurnLeft, false);
                }
            }


            _virtualJoystick?.RelinquishVJD(Convert.ToUInt32(settings.vJoyDeviceId));
 
        }



        public override void DialDown(DialPayload payload)
        {
            _ = ButtonPressAsync(false);
        }

        private async Task ButtonPressAsync(bool autoreset = false, bool touchscreen = false)
        {
            if (settings.triggerPush || settings.triggerPushAndRelease || settings.triggerToggle)
            {
                if (_virtualJoystick == null || !_virtualJoystickAcquired)
                {
                    _virtualJoystick.AcquireVJD(Convert.ToUInt32(settings.vJoyDeviceId));
                }

                var buttonId = Convert.ToUInt32(settings.vJoyButtonId);
                var buttonName = settings.vJoyButtonName;
                if(touchscreen)
                {
                    buttonId = Convert.ToUInt32(settings.vJoyButtonIdTouch);
                    buttonName = settings.vJoyButtonNameTouch;
                };

                
                if (_currentToggleStatus && settings.triggerToggle)
                {
                    // Es ist ein Button
                    _virtualJoystick.SetBtn(false, Convert.ToUInt32(settings.vJoyDeviceId), buttonId);
                    _currentToggleStatus = false;
                    UpdateButtonDisplay(buttonId, buttonName, false);

                }
                else
                {
                    // Es ist ein Button
                    _virtualJoystick.SetBtn(true, Convert.ToUInt32(settings.vJoyDeviceId), buttonId);
                    UpdateButtonDisplay(buttonId, buttonName, true);
                    if (!_currentToggleStatus && settings.triggerToggle)
                    {
                        _currentToggleStatus = true;
                    }
                }


                if (autoreset)
                {
                    await Task.Delay(Convert.ToInt32(this.settings.vJoyButtonDebounceLeftRight)).ContinueWith(_ =>
                    {
                        _virtualJoystick.SetBtn(false, Convert.ToUInt32(settings.vJoyDeviceId), buttonId);
                        UpdateButtonDisplay(buttonId, buttonName, false);
                    });
                }
                
                _virtualJoystick?.RelinquishVJD(Convert.ToUInt32(settings.vJoyDeviceId));
            }
        }

        public override void DialUp(DialPayload payload)
        {
            if (settings.resetDoNothing)
                return;
            if (settings.triggerPushAndRelease || settings.triggerRelease)
            {
                if (_virtualJoystick == null || !_virtualJoystickAcquired)
                {
                    _virtualJoystick.AcquireVJD(Convert.ToUInt32(settings.vJoyDeviceId));
                }

                _virtualJoystick.SetBtn(false, Convert.ToUInt32(settings.vJoyDeviceId), Convert.ToUInt32(settings.vJoyButtonId));
                UpdateButtonDisplay(Convert.ToUInt32(settings.vJoyButtonId), settings.vJoyButtonName, false);
                _virtualJoystick?.RelinquishVJD(Convert.ToUInt32(settings.vJoyDeviceId));
            }
        }

        public override void TouchPress(TouchpadPressPayload payload)
        {
            _ = ButtonPressAsync(true, true);
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