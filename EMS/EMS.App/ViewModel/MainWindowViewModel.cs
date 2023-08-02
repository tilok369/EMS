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

    public MainWindowViewModel()
    {
        _employeeManagementService = new EmployeeManagementService("https://gorest.co.in/public/v2/users/", "0bf7fb56e6a27cbcadc402fc2fce8e3aa9ac2b40d4190698eb4e8df9284e2023");
        Statuses = new ObservableCollection<string>();
        Statuses.Add("active");
        Statuses.Add("inactive");
        Genders = new ObservableCollection<string>();
        Genders.Add("male");
        Genders.Add("female");
        Employees = new ObservableCollection<Employee>();
        //_initilize = new Lazy<Task>(() => LoadEmployees(null));
        Initialize();
        //LoadEmployees();
        //Employees.Add(new Employee
        //{
        //    id = 100,
        //    name = "Tanjeer",
        //    email = "tanjeer@gmail.com",
        //    gender = "male",
        //    status = "active"
        //});
        //Employees.Add(new Employee
        //{
        //    id = 101,
        //    name = "Sheema",
        //    email = "sheema@gmail.com",
        //    gender = "female",
        //    status = "active"
        //});
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

    private async void Initialize()
    {
        //await _initilize.Value;
        await LoadEmployees();
    }

    private async Task LoadEmployees(Employee newEmployee = null, int size = 20)
    {
        Employees.Clear();
        var employees = await _employeeManagementService.GetAllAsync($"?page=1&per_page={size}");
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
        if(newEmployee != null)
            Employees.Add(new Employee
            {
                id = newEmployee.id,
                name = newEmployee.name,
                email = newEmployee.email,
                gender = newEmployee.gender,
                status = newEmployee.status,
            });
    }

    private async Task SaveEmployee()
    {
        if (SavableEmployee == null || string.IsNullOrEmpty(SavableEmployee.name))
            MessageBox.Show("No employee selected for saving!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);

        var result = SavableEmployee.id == 0
            ? await _employeeManagementService.CreateAsync(SavableEmployee)
            : await _employeeManagementService.UpdateAsync(SavableEmployee.id, SavableEmployee);
        if (result.Item1 != null)
            await LoadEmployees(SavableEmployee.id == 0 ? result.Item1 : null, 20);
        else
            MessageBox.Show(FormatMessage(result.Item2), "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private async Task DeleteEmployee()
    {
        //Employees.Remove(SavableEmployee);
        if (SavableEmployee == null || SavableEmployee.id == 0)
            MessageBox.Show("No employee selected for deletion!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);

        var result = await _employeeManagementService.DeleteAsync(SavableEmployee.id);
        if (result)
        {
            var emp = Employees.FirstOrDefault(e => e.id == SavableEmployee.id);
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

    private string FormatMessage(IEnumerable<ValidationMessage> messages)
    {
        return string.Join(",", messages.Select(i => i.field + ": " + i.message));
    }
}
