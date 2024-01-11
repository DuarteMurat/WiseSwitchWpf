using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseSwitchWpf.Entities
{
    public class Brand
    {
        public string Name { get; set; }

        public ObservableCollection<ProductLine> ProductLines { get; set; }
    }
}
