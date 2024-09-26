using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PowerPlantApp.Models;

public class PowerPlant
{
    [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "İsim  girilmesi zorunludur."),MaxLength(50)]
    [MinLength(3,ErrorMessage ="İsim minimum 3 karakter olmalıdır.")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Açıklama  girilmesi zorunludur."), MaxLength(50)]
    public string Description { get; set; }
    
    [Required(ErrorMessage = "Tip  girilmesi zorunludur."), MaxLength(50)]
    public string Type { get; set; }

}