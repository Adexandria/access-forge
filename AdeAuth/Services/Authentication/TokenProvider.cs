﻿using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AdeAuth.Services.Authentication
{
    /// <summary>
    /// Manages token service
    /// </summary>
    /// <remarks>
    /// A constructor
    /// </remarks>
    /// <param name="tokenConfiguration">Details of token</param>
    class TokenProvider(TokenConfiguration tokenConfiguration) : ITokenProvider
    {

        /// <summary>
        /// Generate refresh token based on size
        /// </summary>
        /// <param name="tokenSize">Token size</param>
        /// <returns>Refresh token</returns>
        /// 
        public string GenerateToken(int tokenSize)
        {
            var randomNumber = new byte[tokenSize];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            var refreshtoken = Convert.ToBase64String(randomNumber);
            return refreshtoken;
        }


        /// <summary>
        /// Generate token based on claims
        /// </summary>
        /// <param name="claims">Claims information</param>
        /// <param name="timeInMinutes">Expiry time in minutes</param>
        /// <returns>Token</returns>
        public string GenerateToken(Dictionary<string, object> claims, int timeInMinutes)
        {
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Claims = claims,
                Expires = DateTime.UtcNow.AddMinutes(timeInMinutes),
                SigningCredentials = new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(securityTokenDescriptor);
            string token = tokenHandler.WriteToken(securityToken);

            return token;
        }


        /// <summary>
        /// Read token and validate token
        /// </summary>
        /// <param name="token">Token to extract</param>
        /// <returns>Validation</returns>
        public bool ReadToken(string token)
        {
            TokenValidationParameters tokenValidationParameters = GetTokenValidationParameters();

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new();

            try
            {
                _ = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                if (validatedToken != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        /// <summary>
        /// Read token and extract information
        /// </summary>
        /// <param name="token">Token to extract</param>
        /// <param name="claimTypes">Claim types to extract</param>
        /// <returns>A list of claims</returns>
        public Dictionary<string, object> ReadToken(string token, params string[] claimTypes)
        {
            TokenValidationParameters tokenValidationParameters = GetTokenValidationParameters();

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
            Dictionary<string, object> claimValues = new();

            try
            {
                var claims = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                foreach (var claimType in claimTypes)
                {
                    var claimValue = claims.Claims.Where(s => s.Type == claimType)
                        .Select(s => s.Value).FirstOrDefault();

                    if (claimValue == null)
                        continue;

                    claimValues.Add(claimType, claimValue);
                }

                return claimValues;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        /// <summary>
        /// Generated token based on the encoded string
        /// </summary>
        /// <param name="encodedString">Encoded string</param>
        /// <returns>Token</returns>
        public string GenerateToken(byte[] encodedString)
        {
            return Convert.ToBase64String(encodedString);
        }

        /// <summary>
        /// Read token using delimiter
        /// </summary>
        /// <param name="token">Token to read</param>
        /// <param name="delimiter">Delimiter</param>
        /// <returns>A list of decoded token </returns>
        public string[] ReadTokenUsingDelimiter(string token, string delimiter)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(delimiter))
            {
                throw new NullReferenceException("Token can not be empty");
            }
            var decodedToken = Convert.FromBase64String(token);

            var details = Encoding.UTF8.GetString(decodedToken);

            if (delimiter == null)
            {
                return new string[] { details };
            }

            return details.Split(delimiter);
        }

        /// <summary>
        /// Generate one time password using encoded string
        /// </summary>
        /// <param name="encodedString">Encoded string</param>
        /// <returns>One time password</returns>
        public int GenerateOTP(byte[] encodedString)
        {
            return BitConverter.ToInt16(encodedString, 0);
        }


        /// <summary>
        /// Verify one time password using encoded string
        /// </summary>
        /// <param name="encodedString">Encoded string</param>
        /// <param name="otp">One time password</param>
        /// <returns>Boolean value</returns>
        public bool VerifyOTP(byte[] encodedString, int otp)
        {
            var generatedOtp = GenerateOTP(encodedString);
            return generatedOtp == otp;
        }

        /// <summary>
        /// Verify one time password using encoded string
        /// </summary>
        /// <param name="encodedString">Encoded string</param>
        /// <param name="otp">One time password</param>
        /// <returns>Boolean value</returns>
        public bool VerifyOTP(byte[] encodedString, string otp)
        {
            var generatedOtp = GenerateOTP(encodedString).ToString();
            return generatedOtp == otp;
        }


        private SecurityKey GetSymmetricSecurityKey()
        {
            byte[] symmetricKey = Encoding.UTF8.GetBytes(_tokenConfiguration.TokenSecret);
            return new SymmetricSecurityKey(symmetricKey);
        }


        private TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateIssuer = !string.IsNullOrEmpty(_tokenConfiguration.Issuer),
                ValidateAudience = !string.IsNullOrEmpty(_tokenConfiguration.Audience),
                IssuerSigningKey = GetSymmetricSecurityKey(),
                ValidAudience = _tokenConfiguration.Audience ?? null,
                ValidIssuer = _tokenConfiguration.Issuer ?? null,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true
            };
        }

        private readonly TokenConfiguration _tokenConfiguration = tokenConfiguration ?? throw new NullReferenceException(nameof(tokenConfiguration));
    }

}
