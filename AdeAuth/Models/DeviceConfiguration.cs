using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    /// <summary>
    /// Manages device configuration details
    /// </summary>
    public class DeviceConfiguration
    {
        /// <summary>
        /// Manages the device name
        /// </summary>
        public string Device {  get; set; }

        /// <summary>
        /// Manages trhe device brand
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// Manges the device model
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the operating system name of trhe device e.g andriod or windows.
        /// </summary>
        public string OS { get; set; }
    }
}
