using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CRUD_Image.Models
{
    public class CustomPostedFile: HttpPostedFileBase
    {
        private readonly byte[] fileBytes;
        public CustomPostedFile(byte[] fileBytes, string fileName)
        {
            this.fileBytes = fileBytes;
            this.FileName = fileName;
            this.InputStream = new MemoryStream(fileBytes);
        }
        public override int ContentLength => fileBytes.Length;
        public override string FileName { get; }
        public override Stream InputStream { get; }

        public override void SaveAs(string filename)
        {
            using (var file = File.Open(filename, FileMode.CreateNew)) InputStream.CopyTo(file);
        }
    }
}