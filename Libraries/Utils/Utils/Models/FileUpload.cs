using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utils.Models
{
    public class FileUploadDo
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public byte[] BinaryFile { get; set; }
    }
    public class UploadObjectDo<T>
    {
        public T Parameter { get; set; }
        public List<FileUploadDo> Files { get; set; }

        public UploadObjectDo()
        {
            this.Files = new List<FileUploadDo>();
        }
    }
}