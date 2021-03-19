﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtkBlazorApp.DataAccess.Entity
{
    public class PrikatReportTemplateEntity
    {
        public int template_id { get; set; }
        public int manufacturer_id { get; set; }        
        public int discount1 { get; set; }
        public int discount2 { get; set; }
        public int status { get; set; }

        public string manufacturer_name { get; set; }
        public string currency_code { get; set; }
    }
}
