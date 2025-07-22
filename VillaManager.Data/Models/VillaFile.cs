using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VillaManager.Data.Models
{
    public class VillaFile
    {
         public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; } // image/jpeg, application/pdf etc.

        public int VillaId { get; set; }
        public Villa Villa { get; set; }
    }
}