using System.Collections.ObjectModel;

namespace WiseSwitchWpf.Entities
{
    public class ProductSeries
    {
        public string Name { get; set; }

        public ObservableCollection<SwitchModel> SwitchModels { get; set; }
    }
}