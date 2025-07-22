using AdeAuth.Models;
using AdeAuth.Services.Utility;

namespace AdeAuth.Services.Interfaces
{
    /// <summary>
    /// Provides methods to retrieve user location and device information.
    /// </summary>
    public interface ILocatorService
    {
        /// <summary>
        /// Retrieves the geographical location associated with the specified IP address.
        /// </summary>
        /// <param name="ipAddress">The IP address for which to fetch the location. Must be a valid IPv4 or IPv6 address.</param>
        /// <returns>A <see cref="Location"/> object representing the geographical location of the IP address. Returns <see
        /// langword="null"/> if the location cannot be determined.</returns>
        Location FetchUserLocation(string ipAddress);

        /// <summary>
        /// Retrieves the current device configuration for the user.
        /// </summary>
        /// <returns>A <see cref="DeviceConfiguration"/> object representing the user's device settings.</returns>
        DeviceConfiguration FetchUserDevice();
        
        /// <summary>
        /// Retrieves the current IP address of the local machine.
        /// </summary>
        /// <returns>A string representing the local machine's IP address. Returns an empty string if the IP address cannot be
        /// determined.</returns>
        string FetchIpAddress();
    }
}
