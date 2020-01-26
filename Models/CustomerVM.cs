using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRUD_Image.Models
{
    public class CustomerVM : Customer
    {
        public byte[] Files { get; set; }
        public string FileName { get; set; }
    }
}