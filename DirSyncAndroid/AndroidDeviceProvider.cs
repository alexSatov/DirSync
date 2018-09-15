using PortableDeviceApiLib;

namespace DirSyncAndroid
{
    public class AndroidDeviceProvider
    {
        public static string[] GetDeviceIds()
        {
            var deviceManager = new PortableDeviceManagerClass();
            uint deviceCount = 1;

            deviceManager.GetDevices(null, ref deviceCount);

            if (deviceCount < 1) return new string[0];

            var deviceIds = new string[deviceCount];

            deviceManager.GetDevices(ref deviceIds[0], ref deviceCount);

            return deviceIds;
        }
    }
}
