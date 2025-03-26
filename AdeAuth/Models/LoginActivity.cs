﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    /// <summary>
    /// Manages login activity for a user
    /// </summary>
    public class LoginActivity
    {

        protected LoginActivity()
        {
            Id = Guid.NewGuid();
        }


        public LoginActivity(string device, Guid userId)
        {
            Id = Guid.NewGuid();
            RecentLoginTime = DateTime.UtcNow;
            Device = device;
            UserId = userId;
        }

        public LoginActivity UpdateLoginActivity()
        {
            RecentLoginTime = DateTime.UtcNow;
            return this;
        }

        public LoginActivity AddLocation(string ipAddress, string city, string country)
        {
            IpAddress = ipAddress;
            City = city;
            Country = country;

            return this;
        }
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Device the user is using to connect
        /// </summary>

        public string? Device {  get; set; }

        /// <summary>
        /// IP address of the device
        /// </summary>

        public string? IpAddress { get; set; }

        /// <summary>
        /// City of the user
        /// </summary>

        public string? City { get; set; }


        /// <summary>
        /// Country of the user
        /// </summary>

        public string? Country { get; set; }

        /// <summary>
        /// Last login time
        /// </summary>

        public DateTime RecentLoginTime { get; set; }

        /// <summary>
        /// User id
        /// </summary>

        public Guid UserId { get; set; }
    }
}
