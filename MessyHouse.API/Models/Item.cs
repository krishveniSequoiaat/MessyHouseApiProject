namespace MessyHouseAPIProject.Models
{
    public class Item
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string ImageUrl { get; set; }
        public required string Tag { get; set; }
        public required string Barcode { get; set; }
    }
}