using System.Collections.ObjectModel;

namespace WiseSwitchWpf.Entities
{
    public class ProductLine
    {
        public string Name { get; set; }

        public ObservableCollection<ProductSeries> ProductSeries { get; set; }
    }
}