namespace WiseSwitchWpf.Utils
{
    public static class EntityNames
    {
        public const string Brand = "Brand";
        public const string FirmwareVersion = "FirmwareVersion";
        public const string Manufacturer = "Manufacturer";
        public const string ProductLine = "ProductLine";
        public const string ProductSeries = "ProductSeries";
        public const string SwitchModel = "SwitchModel";

        public static string Spaced(string entityName)
        {
            return entityName switch
            {
                Brand => "Brand",
                FirmwareVersion => "Firmware Version",
                Manufacturer => "Manufacturer",
                ProductLine => "Product Line",
                ProductSeries => "Product Series",
                SwitchModel => "Switch Model",

                _ => throw new NotImplementedException()
            };
        }

        public static string Plural(string entityName)
        {
            return entityName switch
            {
                Brand => "Brands",
                FirmwareVersion => "FirmwareVersions",
                Manufacturer => "Manufacturers",
                ProductLine => "ProductLines",
                ProductSeries => "ProductSeries",
                SwitchModel => "SwitchModels",

                _ => throw new NotImplementedException()
            };
        }

        public static string PluralSpaced(string entityName)
        {
            return entityName switch
            {
                Brand => "Brands",
                FirmwareVersion => "Firmware Versions",
                Manufacturer => "Manufacturers",
                ProductLine => "Product Lines",
                ProductSeries => "Product Series",
                SwitchModel => "Switch Models",

                _ => throw new NotImplementedException()
            };
        }
    }
}
