using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using DeviceDetectorNET;
using DeviceDetectorNET.Cache;
using DeviceDetectorNET.Parser;
using IPinfo;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AdeAuth.Services.Authentication
{
    internal class LocatorService : ILocatorService
    {
        // a constructor
        public LocatorService(IHttpContextAccessor contextAccessor,
            IpInfoConfiguration infoConfiguration, ILoggerFactory loggerFactory)
        {
            DeviceDetector
                .SetVersionTruncation(VersionTruncation.VERSION_TRUNCATION_NONE);

            _logger = loggerFactory.CreateLogger<LocatorService>();

            _context = contextAccessor.HttpContext;

            _apiKey = infoConfiguration?.APIKey;
        }

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
            IPinfoClient client = CreateClient(_apiKey);

            if (client == null)
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
            if (string.IsNullOrEmpty(_ipAddress))
            {
                return default;
            }
            return _ipAddress.Split(',')[0];
        }

        private IPinfoClient CreateClient(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogInformation("No API key found, locator client will not be created: {DateTime}", DateTime.UtcNow);
                return default;
            }

            IPinfoClient client = new IPinfoClient.Builder()
                .AccessToken(apiKey)
                .Build();

            _logger.LogInformation("User Locator client created: {DateTime}", DateTime.UtcNow);

            return client;
        }

        private readonly string _apiKey;
        private readonly HttpContext _context;
        private readonly ILogger<LocatorService> _logger;
    }
}
