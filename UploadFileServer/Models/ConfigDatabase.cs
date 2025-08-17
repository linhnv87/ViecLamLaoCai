using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UploadFileServer.Models
{
    public class ConfigDatabase
    {
        public int? ID { set; get; }
        public string Database { set; get; }
        public int? OrganizationID { set; get; }
        public string OrganizationCode { set; get; }
        public string ConnectString { set; get; }
        public virtual ESOrganization ESOrganization { set; get; }
    }
}