using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
     public class Language : BaseEntity<Guid>
    {
        //public int LanguageId { get; set; }
        public string LanguageName { get; set; }


        public ICollection<MovieLanguage> MovieLanguage { get; set; }



    }
}
