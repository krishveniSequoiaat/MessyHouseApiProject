using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MessyHouseAPIProject.Models
{
    public class StorageBox
    {
        public int Id { get; set; }

        [NotNull]
        public required string Name { get; set; }
        [NotNull]
        public required string Location { get; set; }
        [NotNull]
        public required string Barcode { get; set; }
    }
}