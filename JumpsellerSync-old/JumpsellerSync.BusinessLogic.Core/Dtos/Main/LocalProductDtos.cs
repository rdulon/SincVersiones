namespace JumpsellerSync.BusinessLogic.Core.Dtos.Main
{
    public class CreateLocalProductDto
    {
        public string ProductId { get; set; }

        public double? Price { get; set; }

        public int? Stock { get; set; }
    }

    public class UpdateLocalProductDto
    {
        public string Id { get; set; }

        public double Price { get; set; }

        public int Stock { get; set; }
    }

    public class LocalProductDetailsDto
    {
        public string Id { get; set; }

        public string Sku { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public int Stock { get; set; }
    }
}
