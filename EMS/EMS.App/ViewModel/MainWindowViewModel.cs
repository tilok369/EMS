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
using EMS.Model.Validators;

namespace EMS.App.ViewModel;

public class MainWindowViewModel : ViewModelBase
{
    #region [DI private properties]

    private readonly IEmployeeManagementService _employeeManagementService;
    private readonly IEmployeeValidator _employeeValidator;

    #endregion

    #region [Observale Properties]

    public ObservableCollection<Employee> Employees { get; set; }
    public ObservableCollection<string> Statuses { get; set; }
    public ObservableCollection<string> Genders { get; set; }

    #endregion


    public MainWindowViewModel(IEmployeeValidator employeeValidator, IEmployeeManagementService employeeManagementService)
    { 
        _employeeManagementService = employeeManagementService;
        _employeeValidator = employeeValidator;    
        Initialize();
    }

    private async void Initialize()
    {
        Statuses = MainWindowViewModelHelper.InitializeStatues();
        Genders = MainWindowViewModelHelper.InitializeGenders();
        ResetPage();
        Employees = new ObservableCollection<Employee>();
        SavableEmployee = new Employee();
        await LoadEmployees();
    }

    #region [Commands]
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
        async (execute) => await SearchEmployees(),
        canExecute => !string.IsNullOrEmpty(SearchTerm));

    #endregion

    #region [Full Properties]

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


    #endregion

    #region [Command Handlers]

    private async Task LoadEmployees(Employee newEmployee = null)
    {
        IsLoading = true;
        Employees.Clear();
        if (newEmployee != null)
            Employees.Add(Copy(newEmployee));
        var employees = await _employeeManagementService.GetAllAsync($"?page={PageNumber}&per_page=20");
        foreach (var employee in employees)
        {
            Employees.Add(Copy(employee));
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
            Employees.Add(Copy(employee));
        }
        employees = await _employeeManagementService.GetAllAsync($"?email={searchTerm}");
        foreach (var employee in employees)
        {
            Employees.Add(Copy(employee));
        }
        IsLoading = false;
        ResetPage();
        PageInfo = ChangePageInfo();
    }

    private async Task SaveEmployee()
    {
        var validateResult = _employeeValidator.Validate(SavableEmployee);
        if (!validateResult.Success)
        {
            MessageBox.Show(validateResult.Message, "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if ((SavableEmployee?.id ?? 0) == 0)
            await AddEmployee();
        else 
            await UpdateEmployee();
    }

    private async Task DeleteEmployee()
    {
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
            Employees.Add(Copy(result.Item1));            
        }
            
        else
            MessageBox.Show(MainWindowViewModelHelper.FormatMessage(result.Item2), "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
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
            MessageBox.Show(MainWindowViewModelHelper.FormatMessage(result.Item2), "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private async Task EditEmployee()
    {
        SavableEmployee = Copy(SelectedEmployee);
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

    #endregion

    #region [Utilities]

    private Employee Copy(Employee employee)
    {
        return new Employee
        {
            id = employee.id,
            name = employee.name,
            email = employee.email,
            gender = employee.gender,
            status = employee.status,
        };
    }

    private string ChangePageInfo()
    {
        return $"Showing {Employees.Count} records of page #{PageNumber}";
    }

    private void ResetPage()
    {
        PageNumber = 1;
    }

    #endregion
}
