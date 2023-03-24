namespace JumpsellerSync.BusinessLogic.Core.Dtos.Jumpseller
{
    public class JumpsellerProductWrapperDto
    {
        public JumpsellerProductDto Product { get; set; }
    }

    public class JumpsellerProductDto
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public double Stock { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }

        public double Weight { get; set; }

        public bool StockUnlimited { get; set; }

        public string Sku { get; set; }

        public bool ShippingRequired { get; set; }

        public string Status { get; set; }

        public double Length { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public string PackageFormat { get; set; }

        public string Brand { get; set; }
    }

    public class JumpsellerProductsCountDto
    {
        public int Count { get; set; }
    }
}
