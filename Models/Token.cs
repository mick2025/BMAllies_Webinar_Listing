﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebinarRegistration.Models
{
    public class Token
    {
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string not_before { get; set; }
        public string expires_on { get; set; }
        public string resourse { get; set; }
        public string access_token { get; set; }

    }
}
