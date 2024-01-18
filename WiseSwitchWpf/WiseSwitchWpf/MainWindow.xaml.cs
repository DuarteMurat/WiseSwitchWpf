using System.Collections.ObjectModel;
using System.Windows;
using WiseSwitchWpf.Entities;
using WiseSwitchWpf.Services.Api;
using WiseSwitchWpf.Services.Data;
using WiseSwitchWpf.ViewModels;

namespace WiseSwitchWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DataService _dataService;
        public ObservableCollection<Brand> Brands { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _dataService = new DataService(new ApiService());

            Brands = new ObservableCollection<Brand>();

            DataContext = new PopulateTreeViewModel();
        }

        private void ResetPage(object sender, RoutedEventArgs e)
        {
            // Check if the current content of mainFrame is of type FactoryResetSwitchPage
            if (mainFrame.Content is FactoryResetSwitchPage)
            {
                // Close the current page by setting content to null
                mainFrame.Content = null;
            }
            else
            {
                // Open the FactoryResetSwitchPage if it's not already open
                mainFrame.Navigate(new FactoryResetSwitchPage());
            }
        }

        private async Task PopulateTreeView()
        {
            try
            {
                var response = await _dataService.GetAsync<ObservableCollection<Brand>>("api/GetAllForDesktopApp");
                if (response.IsSuccess && response.Result is ObservableCollection<Brand> result)
                {
                    Brands = result;
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                MessageBox.Show($"An error occurred: {ex.Message}");
            }

            foreach (var brand in Brands)
            {
                SideTreeView.Items.Add(brand);
            }
        }

        private void SideTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is SwitchModel switchModel)
            {
                // Check if the current content of mainFrame is of type FactoryResetSwitchPage
                if (mainFrame.Content is FactoryResetSwitchPage)
                {
                    // Close the current page by setting content to null
                    mainFrame.Content = null;
                }
                else
                {
                    // Open the FactoryResetSwitchPage if it's not already open
                    mainFrame.Navigate(new FactoryResetSwitchPage());
                }
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await PopulateTreeView();
        }
    }
}