using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataProtection.Web.Models
{
    public partial class Product
    {
        [NotMapped]  // Veritabanında bir sütun veya property değil mapping yapma, eşleştirme demektir.
        public string EncryptedId { get; set; }
    }
}
