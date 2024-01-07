using NAudio.CoreAudioApi;
using System.Runtime.InteropServices;

class AudioSwitch
{
    internal enum ERole : uint
    {
        eConsole = 0,
        eMultimedia = 1,
        eCommunications = 2,
        ERole_enum_count = 3
    }

    [Guid("F8679F50-850A-41CF-9C72-430F290290C8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPolicyConfig
    {
        [PreserveSig]
        int GetMixFormat();
        [PreserveSig]
        int GetDeviceFormat();
        [PreserveSig]
        int ResetDeviceFormat();
        [PreserveSig]
        int SetDeviceFormat();
        [PreserveSig]
        int GetProcessingPeriod();
        [PreserveSig]
        int SetProcessingPeriod();
        [PreserveSig]
        int GetShareMode();
        [PreserveSig]
        int SetShareMode();
        [PreserveSig]
        int GetPropertyValue();
        [PreserveSig]
        int SetPropertyValue();
        [PreserveSig]
        int SetDefaultEndpoint(
            [In][MarshalAs(UnmanagedType.LPWStr)] string deviceId,
            [In][MarshalAs(UnmanagedType.U4)] ERole role);
        [PreserveSig]
        int SetEndpointVisibility();
    }
    [ComImport, Guid("870AF99C-171D-4F9E-AF0D-E63DF40C2BC9")]
    internal class _CPolicyConfigClient
    {
    }
    public class PolicyConfigClient
    {
        public static int SetDefaultDevice(string deviceID)
        {
            IPolicyConfig _policyConfigClient = (new _CPolicyConfigClient() as IPolicyConfig);
            try
            {
                Marshal.ThrowExceptionForHR(_policyConfigClient.SetDefaultEndpoint(deviceID, ERole.eConsole));
                Marshal.ThrowExceptionForHR(_policyConfigClient.SetDefaultEndpoint(deviceID, ERole.eMultimedia));
                Marshal.ThrowExceptionForHR(_policyConfigClient.SetDefaultEndpoint(deviceID, ERole.eCommunications));
                return 0;
            }
            catch
            {
                return 1;
            }
        }
    }

    static string GetDeviceIdByName(string targetDeviceName)
    {
        MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
        var devices = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);

        foreach (var device in devices)
        {
            if (device.FriendlyName == targetDeviceName)
            {
                return device.ID;
            }
        }

        return null;
    }

    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            PolicyConfigClient.SetDefaultDevice(GetDeviceIdByName(args[0]));
            PolicyConfigClient.SetDefaultDevice(GetDeviceIdByName(args[1]));
        }
        else
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
            foreach (var (device, index) in devices.Select((device, index) => (device, index)))
            {
                Console.WriteLine(string.Join(" : ", index + 1, device.FriendlyName));
            }
            Console.Write("Select speeker device : ");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int num) && num <= devices.Count)
            {
                PolicyConfigClient.SetDefaultDevice(devices[num - 1].ID);
            }
            else
            {
                Console.WriteLine("invalid value.");
            }
        }
        return;
    }
}
