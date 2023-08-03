using EMS.App.ViewModel;
using EMS.Model.Validators;
using EMS.Service.Contracts;
using EMS.Service.Services;
using System.Windows;
using System.Windows.Input;

namespace EMS.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IEmployeeValidator employeeValidator, IEmployeeManagementService employeeManagementService)
        {
            InitializeComponent();
            var vm = new MainWindowViewModel(employeeValidator, employeeManagementService);
            DataContext = vm;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if(this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
            Application.Current.Shutdown();
        }
    }
}
