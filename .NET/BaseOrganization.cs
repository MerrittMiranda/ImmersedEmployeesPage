using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Domain.Employees
{
    public class BaseOrganization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public string BusinessPhone { get; set; }
        public string SiteUrl { get; set; }
        
    }
}
