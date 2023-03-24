namespace JumpsellerSync.BusinessLogic.Provider.Intcomex.Dtos
{
    internal class IntcomexProductDto
    {
        public string Sku { get; set; }

        public string Mpn { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public IntcomexBrandDto Brand { get; set; }

        public int InStock { get; set; }

        public IntcomexPriceDto Price { get; set; }
    }
}
