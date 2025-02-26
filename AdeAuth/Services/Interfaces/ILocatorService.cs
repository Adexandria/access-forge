using AdeAuth.Models;
using AdeAuth.Services.Utility;

namespace AdeAuth.Services.Interfaces
{
    public interface ILocatorService
    {
        Location FetchUserLocation(string ipAddress);

        DeviceConfiguration FetchUserDevice();

        string FetchIpAddress();
    }
}
