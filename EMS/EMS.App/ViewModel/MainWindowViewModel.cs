using EMS.App.MVVM;
using EMS.Model;
using EMS.Service.Contracts;
using EMS.Service.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;

namespace EMS.App.ViewModel;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IEmployeeManagementService _employeeManagementService;
    //private readonly Lazy<Task> _initilize;

    public ObservableCollection<Employee> Employees { get; set; }
    public ObservableCollection<string> Statuses { get; set; }
    public ObservableCollection<string> Genders { get; set; }


    public RelayCommand EditCommand => new RelayCommand(async (execute) => await EditEmployee());
    public RelayCommand SaveCommand => new RelayCommand(async (execute) => await SaveEmployee());
    public RelayCommand DeleteCommand => new RelayCommand(
        async (execute) => await DeleteEmployee(),
        canExecute => SelectedEmployee != null);
    public RelayCommand ClearCommand => new RelayCommand(execute => ClearEmployee());
    public RelayCommand PreviousPageCommand => new RelayCommand(
        async (execute) => await GetPreviousPage(),
        canExecute => PageNumber > 1);
    public RelayCommand NextPageCommand => new RelayCommand(async (execute) => await GetNextPage());
    public RelayCommand SearchCommand => new RelayCommand(
        async (execute) => SearchEmployees(),
        canExecute => !string.IsNullOrEmpty(SearchTerm));

    public MainWindowViewModel(IEmployeeManagementService employeeManagementService)
    {
        PageNumber = 1;      
        //_employeeManagementService = new EmployeeManagementService("https://gorest.co.in/public/v2/users/", "0bf7fb56e6a27cbcadc402fc2fce8e3aa9ac2b40d4190698eb4e8df9284e2023");
        _employeeManagementService = employeeManagementService;
        Statuses = new ObservableCollection<string>();
        Statuses.Add("active");
        Statuses.Add("inactive");
        Genders = new ObservableCollection<string>();
        Genders.Add("male");
        Genders.Add("female");
        Employees = new ObservableCollection<Employee>();
        Initialize();
        SavableEmployee = new Employee();
    }

    private Employee selectedEmployee;

    public Employee SelectedEmployee
    {
        get { return selectedEmployee; }
        set
        {
            selectedEmployee = value;
            OnPropertyChanged();
        }
    }

    private Employee savableEmployee;

    public Employee SavableEmployee
    {
        get { return savableEmployee; }
        set
        {
            savableEmployee = value;
            OnPropertyChanged();
        }
    }

    private int pageNumber;

    public int PageNumber
    {
        get { return pageNumber; }
        set 
        { 
            pageNumber = value;
            OnPropertyChanged();
        }
    }

    private string pageInfo;

    public string PageInfo
    {
        get { return pageInfo; }
        set 
        { 
            pageInfo = value;
            OnPropertyChanged();
        }
    }

    private bool isLoading;

    public bool IsLoading
    {
        get { return isLoading; }
        set 
        { 
            isLoading = value;
            OnPropertyChanged();
        }
    }

    private string searchTerm;

    public string SearchTerm
    {
        get { return searchTerm; }
        set 
        { 
            searchTerm = value;
            OnPropertyChanged();
        }
    }





    private async void Initialize()
    {
        //await _initilize.Value;
        await LoadEmployees();
    }

    private async Task LoadEmployees(Employee newEmployee = null)
    {
        IsLoading = true;
        Employees.Clear();
        if (newEmployee != null)
            Employees.Add(new Employee
            {
                id = newEmployee.id,
                name = newEmployee.name,
                email = newEmployee.email,
                gender = newEmployee.gender,
                status = newEmployee.status,
            });
        var employees = await _employeeManagementService.GetAllAsync($"?page={PageNumber}&per_page=20");
        foreach (var employee in employees)
        {
            Employees.Add(new Employee
            {
                id = employee.id,
                name = employee.name,
                email = employee.email,
                gender = employee.gender,
                status = employee.status,
            });
        }
        IsLoading = false;
        PageInfo = ChangePageInfo();
    }

    private async Task SearchEmployees()
    {
        IsLoading = true;
        Employees.Clear();
        var employees = await _employeeManagementService.GetAllAsync($"?name={searchTerm}");
        foreach (var employee in employees)
        {
            Employees.Add(new Employee
            {
                id = employee.id,
                name = employee.name,
                email = employee.email,
                gender = employee.gender,
                status = employee.status,
            });
        }
        employees = await _employeeManagementService.GetAllAsync($"?email={searchTerm}");
        foreach (var employee in employees)
        {
            Employees.Add(new Employee
            {
                id = employee.id,
                name = employee.name,
                email = employee.email,
                gender = employee.gender,
                status = employee.status,
            });
        }
        IsLoading = false;
        PageInfo = ChangePageInfo();
    }

    private async Task SaveEmployee()
    {
        if (SavableEmployee == null || string.IsNullOrEmpty(SavableEmployee.name))
            MessageBox.Show("No employee selected for saving!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);

        if ((SavableEmployee?.id ?? 0) == 0)
            await AddEmployee();
        else 
            await UpdateEmployee();
    }

    private async Task DeleteEmployee()
    {
        //Employees.Remove(SavableEmployee);
        if (SavableEmployee == null || SavableEmployee.id == 0)
            MessageBox.Show("No employee selected for deletion!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);

        var result = await _employeeManagementService.DeleteAsync(SavableEmployee?.id ?? 0);
        if (result)
        {
            MessageBox.Show("Employee deleted!", "Information!", MessageBoxButton.OK, MessageBoxImage.Information);
            var emp = Employees.FirstOrDefault(e => e.id == (SavableEmployee?.id ?? 0));
            if(emp != null)
                Employees.Remove(emp);
            ClearEmployee();
        }
        else
            MessageBox.Show("An error occurred while deleting", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void ClearEmployee()
    {
        SavableEmployee = new Employee();
        SearchTerm = string.Empty;
    }

    private async Task AddEmployee()
    {
        var result = await _employeeManagementService.CreateAsync(SavableEmployee);
        if (result.Item1 != null)
        {
            MessageBox.Show("Employee created!", "Information!", MessageBoxButton.OK, MessageBoxImage.Information);
            ClearEmployee();
            Employees.Add(new Employee
            {
                id = result.Item1.id,
                name = result.Item1.name,
                email = result.Item1.email,
                gender = result.Item1.gender,
                status = result.Item1.status
            });            
        }
            
        else
            MessageBox.Show(FormatMessage(result.Item2), "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private async Task UpdateEmployee()
    {
        var result = await _employeeManagementService.UpdateAsync(SavableEmployee.id, SavableEmployee);
        if (result.Item1 != null)
        {
            MessageBox.Show("Employee updated!", "Information!", MessageBoxButton.OK, MessageBoxImage.Information);
            ClearEmployee();
            var emp = Employees.FirstOrDefault(e => e.id == result.Item1.id);
            if (emp == null) return;

            Employees.Remove(emp);
            Employees.Add(result.Item1);
            
        }

        else
            MessageBox.Show(FormatMessage(result.Item2), "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private async Task EditEmployee()
    {
        SavableEmployee = new Employee
        {
            id = SelectedEmployee.id,
            email = SelectedEmployee.email,
            gender = SelectedEmployee.gender,
            name = SelectedEmployee.name,
            status = SelectedEmployee.status
        };
    }

    private async Task GetPreviousPage()
    {
        PageNumber--;        
        await LoadEmployees();
        PageInfo = ChangePageInfo();
    }

    private async Task GetNextPage()
    {
        PageNumber++;        
        await LoadEmployees();
        PageInfo = ChangePageInfo();
    }

    private string ChangePageInfo()
    {
        return $"Showing {Employees.Count} records of page #{PageNumber}";
    }

    private string FormatMessage(IEnumerable<ValidationMessage> messages)
    {
        return string.Join(",", messages.Select(i => i.field + ": " + i.message));
    }
}
