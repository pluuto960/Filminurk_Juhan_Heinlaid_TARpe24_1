using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filminurk.Core.Domain
{
    public class FiletoApi
    {
        public Guid ImageID { get; set; }
        public string? ExistingFilepath { get; set; }
        public Guid? MovieID { get; set; }
        public bool? IsPoster { get; set; }//näitab kas pilt on poster või mitte

    }
}
