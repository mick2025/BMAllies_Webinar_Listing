using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebinarRegistration.Models
{
    public class Webinar
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Active;
        public DateTime? EventDate { get; set; }
        public string Company { get; set; }
        public string RegistrationLink { get; set; }

    }
}
