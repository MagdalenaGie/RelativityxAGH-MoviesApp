using System.ComponentModel.DataAnnotations;

namespace WebAppOpenIDConnectDotNet.Models
{
    public enum GenereType
    {
        [Display(Name = "Dramat")]
        Drama,
        [Display(Name = "Komedia")]
        Comedy,
        [Display(Name = "Romans")]
        Romance,
        [Display(Name = "Komedia Romantyczna")]
        RomCom,
        [Display(Name = "Kryminał")]
        Crime,
        [Display(Name = "Film Akcji")]
        Mystery,
        [Display(Name = "Fantasy")]
        Fantasy,
        [Display(Name = "Inny")]
        Other
    }
}
