using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MVCAbelStoreWeb.Models
{
    public class ContactUsViewModel
    {

        [Display(Name = "Adınız:")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public string Name { get; set; }

        [Display(Name = "E-Postanız:")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        [EmailAddress(ErrorMessage = "Lütfen geçerli bir e-posta adresi yazınız!")]
        public string Email { get; set; }

        [Display(Name = "İrtibat No:")]
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumner { get; set; }

        [Display(Name = "Mesaj Alanı:")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public string Message { get; set; }
    }
}
