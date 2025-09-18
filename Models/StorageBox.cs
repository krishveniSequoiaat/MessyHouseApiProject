using System.ComponentModel.DataAnnotations;

namespace MessyHouseAPIProject.Models
{
    public class StorageBox
    {
        public int Id { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public string Barcode { get; set; }
    }
}