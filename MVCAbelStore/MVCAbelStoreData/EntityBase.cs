using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MVCAbelStoreData
{
    public abstract class EntityBase
    {

        public Guid Id { get; set; }
        [Display(Name = "Kayıt Tarihi")]
        public DateTime DateCreated { get; set; }

        [Display(Name ="Durum")]
        public bool Enabled { get; set; }

    }
}
