using System;


namespace AdeAuth.Services.Utility
{
    /// <summary>
    /// Manages the response
    /// </summary>
    /// <remarks>
    /// Constructor
    /// </remarks>
    /// <param name="qrCodeImage">Qr code </param>
    /// <param name="manualKey">Manual key</param>
    public class Authenticator(string qrCodeImage, string manualKey) : IAuthenticator
    {

        /// <summary>
        /// Qr code
        /// </summary>
        public string QrCodeImage { get; set; } = qrCodeImage;

        /// <summary>
        /// Manual key from google authenticator
        /// </summary>
        public string ManualKey { get; set; } = manualKey;
    }
}
