using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using DeviceDetectorNET;
using DeviceDetectorNET.Cache;
using DeviceDetectorNET.Parser;
using IPinfo;
using Microsoft.AspNetCore.Http;

namespace AdeAuth.Services.Authentication
{
    internal class LocatorService : ILocatorService
    {
        // a transient
        public LocatorService(IHttpContextAccessor contextAccessor, IpInfoConfiguration infoConfiguration)
        {
            DeviceDetector
                .SetVersionTruncation(VersionTruncation.VERSION_TRUNCATION_NONE);

            _context = contextAccessor.HttpContext;

            client = new IPinfoClient.Builder()
                .AccessToken(infoConfiguration.APIKey)
                .Build();
        }
        // ensure you manage the exceptions if the user uses a console instead of 
        public DeviceConfiguration FetchUserDevice()
        {
            var userAgent = _context?.Request?.Headers["User-Agent"];

            var headers = _context?.Request?.Headers?.ToDictionary(a => a.Key, a => a.Value.ToArray().FirstOrDefault());

            if(string.IsNullOrEmpty(userAgent) || headers == null)
            {
                return default;
            }
            var clientHints = ClientHints.Factory(headers);

            var deviceConfiguration = new DeviceDetector(userAgent, clientHints);

            deviceConfiguration.SetCache(new DictionaryCache());

            deviceConfiguration.DiscardBotInformation();

            deviceConfiguration.Parse();

            if (!deviceConfiguration.IsBot())
            {
                
                var osInfo = deviceConfiguration.GetOs();
                var device = deviceConfiguration.GetDeviceName();
                var brand = deviceConfiguration.GetBrandName();
                var model = deviceConfiguration.GetModel();

                return new DeviceConfiguration() 
                { 
                    Brand = brand,
                    Device = device,
                    Model = model,
                    OS = osInfo.ParserName
                };

            }

            return default;
        }

        public Location FetchUserLocation(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
            {
                return default;
            }

            var ipResponse = client.IPApi.GetDetails(ipAddress);

            return new Location
            {
                City = ipResponse.City,
                Country = ipResponse.Country
            };
        }

        public string FetchIpAddress()
        {
            string _ipAddress = _context?.Request?.Headers["X-Forwarded-For"];
            if (!string.IsNullOrEmpty(_ipAddress))
            {
                return _ipAddress.Split(',')[0];
            }

            return null;
        }
        private readonly IPinfoClient client;
        private readonly HttpContext _context;
    }
}
