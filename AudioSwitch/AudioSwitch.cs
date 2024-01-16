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

    static void GetOptions(string[] args, ref string speeker, ref string microphone)
    {
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-s":
                    speeker = args[++i];
                    break;
                case "-m":
                    microphone = args[++i];
                    break;
                default:
                    Console.WriteLine($"ERROR: unknown option: {args[i]}");
                    break;
            }
        }
    }

    [DllImport("kernel32.dll")]
    static extern bool AllocConsole();


    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            string speeker = null;
            string mic = null;
            string id = null;

            GetOptions(args, ref speeker, ref mic);
            Console.WriteLine(speeker);
            Console.WriteLine(mic);
            id = GetDeviceIdByName(speeker);
            if (id != null)
            {
                Console.WriteLine(id);
                PolicyConfigClient.SetDefaultDevice(id);
            }
            id = GetDeviceIdByName(mic);
            if (id != null)
            {
                Console.WriteLine(id);
                PolicyConfigClient.SetDefaultDevice(id);
            }
        }
        else
        {
            AllocConsole();
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
            foreach (var (device, index) in devices.Select((device, index) => (device, index)))
            {
                Console.WriteLine(string.Join(" : ", index + 1, device.FriendlyName));
            }
            Console.Write("\nSelect speeker device : ");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int s_num) && s_num <= devices.Count)
            {
                PolicyConfigClient.SetDefaultDevice(devices[s_num - 1].ID);
                Console.WriteLine($"SUCCESS: Default speeker has been set. ({devices[s_num - 1].FriendlyName})");
            }
            else
            {
                Console.WriteLine("ERROR: Invalid parameter.");
            }

            Console.Write("\nSelect microphone device : ");
            input = Console.ReadLine();
            if (int.TryParse(input, out int m_num) && m_num <= devices.Count)
            {
                PolicyConfigClient.SetDefaultDevice(devices[m_num - 1].ID);
                Console.WriteLine($"SUCCESS: Default micriphone has been set.  ({devices[m_num - 1].FriendlyName})");
            }
            else
            {
                Console.WriteLine("ERROR: Invalid parameter.");
            }
            Console.WriteLine("prompt => AudioSwitch.exe -s \"" + devices[s_num - 1].FriendlyName + "\" -m \"" + devices[m_num - 1].FriendlyName + "\"");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
        return;
    }
}
