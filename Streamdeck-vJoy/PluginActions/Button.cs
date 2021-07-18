using BarRaider.SdTools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Streamdeck_vJoy.PluginActions
{
    [PluginActionId("com.streamdeck.vjoy.button")]
    public partial class Button : ButtonBase
    {
        private new PluginSettings settings => (PluginSettings)base.settings;

        public Button(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            base.settings = payload.Settings == null || payload.Settings.Count == 0
                ? PluginSettings.CreateDefaultSettings()
                : payload.Settings.ToObject<PluginSettings>();

            ApplySettings();
        }

        public override void KeyPressed(KeyPayload payload)
        {
            if (settings.triggerPush || settings.triggerPushAndRelease)
            {
                if (settings.vJoyElementType == "btn")
                {
                    // Es ist ein Button
                    for (int i = 0; i < vJoyButtonIds.Length; i++)
                    {
                        joystick.SetBtn(true, vJoyDeviceId, vJoyButtonIds[i]);
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
                }
            }
        }

        public override void KeyReleased(KeyPayload payload)
        {
            if (!settings.resetDoNothing && (settings.triggerPushAndRelease || settings.triggerRelease))
            {
                if (settings.vJoyElementType == "btn")
                {
                    // Es ist ein Button
                    for (int i = 0; i < vJoyButtonIds.Length; i++)
                    {
                        joystick.SetBtn(false, vJoyDeviceId, vJoyButtonIds[i]);
                    }
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
            }
        }

        #region Private Methods

        private int GetJoystickAxisMinValue(HID_USAGES axis)
        {
            long MinValue = 0;
            joystick.GetVJDAxisMin(vJoyDeviceId, axis, ref MinValue);
            return (int)MinValue;
        }

        private int GetJoystickAxisMaxValue(HID_USAGES axis)
        {
            long MaxValue = 0;
            joystick.GetVJDAxisMax(vJoyDeviceId, axis, ref MaxValue);
            return (int)MaxValue;
        }

        private int GetJoystickAxisCenter(HID_USAGES axis)
        {
            long MaxValue = 0;
            joystick.GetVJDAxisMax(vJoyDeviceId, axis, ref MaxValue);
            return (int)MaxValue / 2;
        }


        private void setAxisValue(int axValue, HID_USAGES axis)
        {
            joystick.SetAxis(axValue, vJoyDeviceId, axis);
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
            axisValues.TryGetValue(theAxis, out int axisVal);

            axisVal = Math.Min(axisVal + Convert.ToInt32(stepValue), GetJoystickAxisMaxValue(theAxis));

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

        #endregion
    }
}