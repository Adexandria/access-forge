﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Services.Interfaces
{
    /// <summary>
    /// Manages token provider service
    /// </summary>
    public interface ITokenProvider
    {
        /// <summary>
        /// Generate token based on claims
        /// </summary>
        /// <param name="claims">Claims information</param>
        /// <param name="timeInMinutes">Expiry time in minutes</param>
        /// <returns>Token</returns>
        string GenerateToken(Dictionary<string, object> claims = null, int timeInMinutes = 5);

        /// <summary>
        /// Generate refresh token based on size
        /// </summary>
        /// <param name="tokenSize">Token size</param>
        /// <returns>Refresh token</returns>
        string GenerateToken(int tokenSize = 10);

        /// <summary>
        /// Read token and extract information
        /// </summary>
        /// <param name="token">Token to extract</param>
        /// <param name="claimTypes">Claim types to extract</param>
        /// <returns>A list of claims</returns>
        Dictionary<string, object> ReadToken(string token, params string[] claimTypes);

        /// <summary>
        /// Read token and validate token
        /// </summary>
        /// <param name="token">Token to extract</param>
        /// <returns>Validation</returns>
        bool ReadToken(string token);

        /// <summary>
        /// Generated token based on the encoded string
        /// </summary>
        /// <param name="encodedString">Encoded string</param>
        /// <returns>Token</returns>
        string GenerateToken(byte[] encodedString);

        /// <summary>
        /// Generate one time password using encoded string
        /// </summary>
        /// <param name="encodedString">Encoded string</param>
        /// <returns>One time password</returns>
        int GenerateOTP(byte[] encodedString);

        /// <summary>
        /// Read token using delimiter
        /// </summary>
        /// <param name="token">Token to read</param>
        /// <param name="delimiter">Delimiter</param>
        /// <returns>A list of decoded token </returns>
        string[] ReadTokenUsingDelimiter(string token, string delimiter = null);

        /// <summary>
        /// Verify one time password using encoded string
        /// </summary>
        /// <param name="encodedString">Encoded string</param>
        /// <param name="otp">One time password</param>
        /// <returns>Boolean value</returns>
        bool VerifyOTP(byte[] encodedString, int otp);

        /// <summary>
        /// Verify one time password using encoded string
        /// </summary>
        /// <param name="encodedString">Encoded string</param>
        /// <param name="otp">One time password</param>
        /// <returns>Boolean value</returns>
        bool VerifyOTP(byte[] encodedString, string otp);
    }
}
