using System.Collections.ObjectModel;
using System.Windows;
using WiseSwitchWpf.Entities;

namespace WiseSwitchWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Brand> Brands { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Brands = new ObservableCollection<Brand>();
            PopulateTreeView();
            DataContext = this; // Set the DataContext to the MainWindow instance
        }

        private void ResetPage(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new FactoryResetSwitchPage());
        }

        private void PopulateTreeView()
        {
            // Create Brand instances and add them to the Brands collection
            Brand brand1 = new Brand { Name = "Brand1", ProductLines = new() };
            brand1.ProductLines.Add(new ProductLine { Name = "ProductLine1", ProductSeries = new() });
            brand1.ProductLines.First().ProductSeries.Add(new ProductSeries { Name = "ProductSeries1", SwitchModels = new() });
            brand1.ProductLines.First().ProductSeries.First().SwitchModels.Add(new SwitchModel { Name = "SwitcModel1" });

            Brand brand2 = new Brand
            {
                Name = "Brand2",
                ProductLines = [
                    new ProductLine
                    {
                        Name = "ProductLine2",
                        ProductSeries = new()
                        {
                            new ProductSeries
                            {
                                Name = "ProductSeries2",
                                SwitchModels = new()
                                {
                                    new SwitchModel{Name = "SwitchModel2"}
                                }
                            }
                        }
                    }
                ]
            };

            Brands.Add(brand1);
            Brands.Add(brand2);
        }
    }
}