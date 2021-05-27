using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DualSenseSharp
{
    public class DS5
    {
        [DllImport("ds5w_x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern DS5W_ReturnValue enumDevices(IntPtr ptr, uint size, ref uint len);

        [DllImport("ds5w_x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern DS5W_ReturnValue initDeviceContext(IntPtr ptrEnumInfo, IntPtr ptrContext);
        [DllImport("ds5w_x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern DS5W_ReturnValue getDeviceInputState(IntPtr ptrContext, IntPtr ptrInputState);
        [DllImport("ds5w_x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern DS5W_ReturnValue setDeviceOutputState(IntPtr ptrContext, IntPtr ptrOutputState);
        [DllImport("ds5w_x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern DS5W_ReturnValue reconnectDevice(IntPtr ptrContext);
        [DllImport("ds5w_x64.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void freeDeviceContext(IntPtr ptrContext);
        public static DeviceEnumInfo[] EnumDevices()
        {
            IntPtr infoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DeviceEnumInfo)) * 16);
            uint deviceCount = 0;
            enumDevices(infoPtr, 16, ref deviceCount);

            DeviceEnumInfo[] infoArray = new DeviceEnumInfo[deviceCount];
            for (int i = 0; i < deviceCount; i++)
            {
                IntPtr ins = new IntPtr(infoPtr.ToInt64() + i * Marshal.SizeOf(typeof(DeviceEnumInfo)));
                infoArray[i] = Marshal.PtrToStructure<DeviceEnumInfo>(ins);
            }
            return infoArray;
        }

        public static DS5W_ReturnValue InitDeviceContext(DeviceEnumInfo enumInfo, ref DeviceContext ptrContext)
        {
            IntPtr infoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DeviceEnumInfo)));
            ptrContext.ContextHandle = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(_DeviceContext)));
            Marshal.StructureToPtr<DeviceEnumInfo>(enumInfo, infoPtr, false);
            var result = DS5.initDeviceContext(infoPtr, ptrContext.ContextHandle);
            return result;
        }

        public static DS5W_ReturnValue GetDeviceInputState(DeviceContext ptrContext, ref DS5InputState ptrInputState)
        {
            IntPtr inStatePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DS5InputState)));
            var result = DS5.getDeviceInputState(ptrContext.ContextHandle, inStatePtr);
            if (result == DS5W_ReturnValue.OK)
            {
                ptrInputState = Marshal.PtrToStructure<DS5InputState>(inStatePtr);
            }
            return result;
        }

        public static DS5W_ReturnValue SetDeviceOutputState(DeviceContext ptrContext, DS5OutputState ptrOutputState)
        {
            IntPtr outStatePtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DS5OutputState)));
            Marshal.StructureToPtr<DS5OutputState>(ptrOutputState, outStatePtr, false);
            return DS5.setDeviceOutputState(ptrContext.ContextHandle, outStatePtr);
        }

        public static DS5W_ReturnValue ReconnectDevice(DeviceContext ptrContext)
        {
            return reconnectDevice(ptrContext.ContextHandle);
        }

        public static void FreeDeviceContext(DeviceContext ptrContext)
        {
            freeDeviceContext(ptrContext.ContextHandle);
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 3)]
    public struct Battery
    {
        /// <summary>
        /// Charching state of the battery
        /// </summary>
        public bool chargin;

        /// <summary>
        /// Indicates that the battery is fully charged
        /// </summary>
        public bool fullyCharged;

        /// <summary>
        /// Battery charge level 0x0 to 
        /// </summary>
        public byte level;
    }

    /// <summary>
	/// Touchpad state
	/// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Touch
    {
        /// <summary>
        /// X positon of finger (~ 0 - 2000)
        /// </summary>
        public uint x;

        /// <summary>
        /// Y position of finger (~ 0 - 2048)
        /// </summary>
        public uint y;

        /// <summary>
        /// Touch is down
        /// </summary>
        public bool down;

        /// <summary>
        /// 7-bit ID for touch
        /// </summary>
        public byte id;
    }


    /// <summary>
	/// 3 Component vector
	/// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3
    {
        public short x;
        public short y;
        public short z;
    }

    /// <summary>
    /// Analog stick
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AnalogStick
    {
        /// <summary>
        /// X Position of stick (0 = Center)
        /// </summary>
        public sbyte x;

        /// <summary>
        /// Y Posistion of stick (0 = Center)
        /// </summary>
        public sbyte y;
    }

    /// <summary>
    /// Input state of the controler
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DS5InputState
    {
        /// <summary>
        /// Position of left stick
        /// </summary>
        public AnalogStick leftStick;

        /// <summary>
        /// Posisiton of right stick
        /// </summary>
        public AnalogStick rightStick;

        /// <summary>
        /// Left trigger position
        /// </summary>
        public byte leftTrigger;

        /// <summary>
        /// Right trigger position
        /// </summary>
        public byte rightTrigger;

        /// <summary>
        /// Buttons and dpad bitmask DS5W_ISTATE_BTX_?? and DS5W_ISTATE_DPAD_?? indices check with if(buttonsAndDpad & DS5W_ISTATE_DPAD_??)...
        /// </summary>
        public byte buttonsAndDpad;

        /// <summary>
        /// Button bitmask A (DS5W_ISTATE_BTN_A_??)
        /// </summary>
        public byte buttonsA;

        /// <summary>
        /// Button bitmask B (DS5W_ISTATE_BTN_B_??)
        /// </summary>
        public byte buttonsB;

        /// <summary>
        /// Accelerometer
        /// </summary>
        public Vector3 accelerometer;

        /// <summary>
        /// Gyroscope  (Currently only raw values will be dispayed! Probably needs calibration (Will be done within the lib in the future))
        /// </summary>
        public Vector3 gyroscope;

        /// <summary>
        /// First touch point
        /// </summary>
        public Touch touchPoint1;

        /// <summary>
        /// Second touch point
        /// </summary>
        public Touch touchPoint2;

        /// <summary>
        /// Battery information
        /// </summary>
        public Battery battery;

        /// <summary>
        /// Indicates the connection of headphone
        /// </summary>
        public bool headPhoneConnected;

        /// <summary>
        /// EXPERIMAENTAL: Feedback of the left adaptive trigger (only when trigger effect is active)
        /// </summary>
        public byte leftTriggerFeedback;

        /// <summary>
        /// EXPERIMAENTAL: Feedback of the right adaptive trigger (only when trigger effect is active)
        /// </summary>
        public byte rightTriggerFeedback;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct DS5OutputState
    {
        /// <summary>
        /// Left / Hard rumbel motor
        /// </summary>
        [FieldOffset(0)]
        public byte leftRumble;

        /// <summary>
        /// Right / Soft rumbel motor
        /// </summary>
        [FieldOffset(1)]
        public byte rightRumble;

        /// <summary>
        /// State of the microphone led
        /// </summary>
        [FieldOffset(2)]
        public MicLed microphoneLed;

        /// <summary>
        /// Diables all leds
        /// </summary>
        [FieldOffset(3)]
        public bool disableLeds;

        /// <summary>
        /// Player leds
        /// </summary>
        [FieldOffset(4)]
        public PlayerLeds playerLeds;

        /// <summary>
        /// Color of the lightbar
        /// </summary>
        [FieldOffset(7)]
        public Color lightbar;

        /// <summary>
        /// Effect of left trigger
        /// </summary>        
        [FieldOffset(10)]
        public TriggerEffect leftTriggerEffect;

        /// <summary>
        /// Effect of right trigger
        /// </summary>
        [FieldOffset(17)]
        public TriggerEffect rightTriggerEffect;

    }
    /// <summary>
	/// Type of trigger effect
	/// </summary>
	public enum TriggerEffectType : byte
    {
        /// <summary>
        /// No resistance is applied
        /// </summary>
        NoResitance = 0x00,

        /// <summary>
        /// Continuous Resitance is applied
        /// </summary>
        ContinuousResitance = 0x01,

        /// <summary>
        /// Seciton resistance is appleyed
        /// </summary>
        SectionResitance = 0x02,

        /// <summary>
        /// Extended trigger effect
        /// </summary>
        EffectEx = 0x26,

        /// <summary>
        /// Calibrate triggers
        /// </summary>
        Calibrate = 0xFC,
    }

    /// <summary>
    /// Trigger effect
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 7)]
    public struct TriggerEffect
    {
        /// <summary>
        /// Trigger effect type
        /// </summary>
        /// 
        [FieldOffset(0)]
        TriggerEffectType effectType;
        [FieldOffset(1)]
        byte _u1_raw0;
        [FieldOffset(2)]
        byte _u1_raw1;
        [FieldOffset(3)]
        byte _u1_raw2;
        [FieldOffset(4)]
        byte _u1_raw3;
        [FieldOffset(5)]
        byte _u1_raw4;
        [FieldOffset(6)]
        byte _u1_raw5;

    }

    public enum DS5W_ReturnValue : uint
    {
        /// <summary>
        /// Operation completed without an error
        /// </summary>
        OK = 0,

        /// <summary>
        /// Operation encountered an unknown error
        /// </summary>
        E_UNKNOWN = 1,

        /// <summary>
        /// The user supplied buffer is to small
        /// </summary>
        E_INSUFFICIENT_BUFFER = 2,

        /// <summary>
        /// External unexpected winapi error (please report as issue if you get this error!)
        /// </summary>
        E_EXTERNAL_WINAPI = 3,

        /// <summary>
        /// Not enought memroy on the stack
        /// </summary>
        E_STACK_OVERFLOW = 4,

        /// <summary>
        /// Invalid arguments
        /// </summary>
        E_INVALID_ARGS = 5,

        /// <summary>
        /// This feature is currently not supported
        /// </summary>
        E_CURRENTLY_NOT_SUPPORTED = 6,

        /// <summary>
        /// Device was disconnected
        /// </summary>
        E_DEVICE_REMOVED = 7,

        /// <summary>
        /// Bluetooth communication error
        /// </summary>
        E_BT_COM = 8,

    }


    public enum MicLed : byte
    {
        /// <summary>
        /// Lef is off
        /// </summary>
        OFF = 0x00,

        /// <summary>
        /// Led is on
        /// </summary>
        ON = 0x01,

        /// <summary>
        /// Led is pulsing
        /// </summary>
        PULSE = 0x02,
    }

    /// <summary>
	/// Player leds values
	/// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PlayerLeds
    {
        /// <summary>
        /// Player indication leds bitflag (You may used them for other features) DS5W_OSTATE_PLAYER_LED_???
        /// </summary>
        public byte bitmask;

        /// <summary>
        /// Indicates weather the player leds should fade in
        /// </summary>
        public bool playerLedFade;

        /// <summary>
        /// Brightness of the player leds
        /// </summary>
        public LedBrightness brightness;
    }

    /// <summary>
    /// Led brightness
    /// </summary>
    public enum LedBrightness : byte
    {
        /// <summary>
        /// Low led brightness
        /// </summary>
        LOW = 0x02,

        /// <summary>
        /// Medium led brightness
        /// </summary>
        MEDIUM = 0x01,

        /// <summary>
        /// High led brightness
        /// </summary>
        HIGH = 0x00,
    }

    /// <summary>
	/// RGB Color
	/// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 3)]
    public struct Color
    {
        public byte r;
        public byte g;
        public byte b;
    }


    /// <summary>
    /// Enum for device connection type
    /// </summary>
    public enum DeviceConnection : byte
    {
        /// <summary>
        /// Controler is connected via USB
        /// </summary>
        USB = 0,

        /// <summary>
        /// Controler is connected via bluetooth
        /// </summary>
        BT = 1,
    }

    /// <summary>
    /// Struckt for storing device enum info while device discovery
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 261, CharSet = CharSet.Unicode)]
    public struct DeviceEnumInfo
    {
        /// <summary>
        /// Path to the discovered device
        /// </summary>
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string path;
        /// <summary>
        /// Connection type of the discoverd device
        /// </summary>
        public DeviceConnection type;
    }


    public class DeviceContext
    {
        public IntPtr ContextHandle { get; set; } = IntPtr.Zero;
        private _DeviceContext _deviceContext;
        public string DevicePath
        {
            get
            {
                if (ContextHandle != IntPtr.Zero)
                {
                    _deviceContext = Marshal.PtrToStructure<_DeviceContext>(ContextHandle);
                    return _deviceContext.devicePath;
                }
                return string.Empty;
            }
        }

        public DeviceConnection Connection
        {
            get
            {
                if (ContextHandle != IntPtr.Zero)
                {
                    _deviceContext = Marshal.PtrToStructure<_DeviceContext>(ContextHandle);
                    return _deviceContext.connection;
                }
                return DeviceConnection.USB;
            }
        }

        public bool Connected
        {
            get
            {
                if (ContextHandle != IntPtr.Zero)
                {
                    _deviceContext = Marshal.PtrToStructure<_DeviceContext>(ContextHandle);
                    return _deviceContext.connected;
                }
                return false;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct _DeviceContext
    {
        /// <summary>
        /// Path to the device
        /// </summary>
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string devicePath;

        /// <summary>
        /// Handle to the open device
        /// </summary>
        public IntPtr deviceHandle;

        /// <summary>
        /// Connection of the device
        /// </summary>
        public DeviceConnection connection;

        /// <summary>
        /// Current state of connection
        /// </summary>
        public bool connected;

        /// <summary>
        /// HID Input buffer (will be allocated by the context init function)
        /// </summary>
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 547)]
        public byte[] hidBuffer;
    }
}