namespace JumpsellerSync.BusinessLogic.Provider.Linkstore.Options
{
    public class LinkstoreOptions
    {
        internal const string CONFIG_SECTION = "Linkstore";

        public AccessOptions Access { get; set; }

        public string BaseUrl { get; set; }

        public EndPointOptions Brands { get; set; }

        public EndPointOptions Categories { get; set; }

        public EndPointOptions Products { get; set; }

        public EndPointOptions Subcategories { get; set; }
    }

}
