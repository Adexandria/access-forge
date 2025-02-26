using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    public class PasswordRule
    {
        public bool HasNumber { get; set; }

        public bool HasCapitalLetter { get; set; }

        public bool HasSmallLetter { get; set; }

        public bool HasSpecialNumber { get; set; }

        public int MinimumPasswordLength { get; set; } = 3;

        public int MaximumPasswordLength { get; set; } = 5;
    }
}
