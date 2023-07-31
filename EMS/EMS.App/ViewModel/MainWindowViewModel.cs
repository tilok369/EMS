using EMS.App.MVVM;
using EMS.Model;
using System;
using System.Collections.ObjectModel;

namespace EMS.App.ViewModel;

public class MainWindowViewModel: ViewModelBase
{
    public ObservableCollection<Employee> Employees { get; set; }
    public ObservableCollection<string> Statuses { get; set; }
    public ObservableCollection<string> Genders { get; set; }


    public RelayCommand EditCommand => new RelayCommand(execute => EditEmployee());
    public RelayCommand SaveCommand => new RelayCommand(execute => SaveEmployee());
    public RelayCommand DeleteCommand => new RelayCommand(
        execute => DeleteEmployee(), 
        canExecute => SelectedEmployee != null);
    public RelayCommand ClearCommand => new RelayCommand(execute => ClearEmployee());

    public MainWindowViewModel()
    {
        Statuses = new ObservableCollection<string>();
        Statuses.Add("active");
        Statuses.Add("inactive");
        Genders = new ObservableCollection<string>();
        Genders.Add("male");
        Genders.Add("female");
        Employees = new ObservableCollection<Employee>();
        Employees.Add(new Employee
        {
            id = 100,
            name = "Tanjeer",
            email = "tanjeer@gmail.com",
            gender = "male",
            status = "active"
        });
        Employees.Add(new Employee
        {
            id = 101,
            name = "Sheema",
            email = "sheema@gmail.com",
            gender = "female",
            status = "active"
        });
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


    private void SaveEmployee()
    {
        if(SavableEmployee == null || string.IsNullOrEmpty(SavableEmployee.name))
            return;
        Employees.Add(new Employee
        {
            id = new Random(1000).NextInt64(),
            name = SavableEmployee.name,
            email = SavableEmployee.email,
            gender = SavableEmployee.gender,
            status = SavableEmployee.status
        });
    }

    private void DeleteEmployee()
    {
        Employees.Remove(SavableEmployee);
    }

    private void ClearEmployee()
    {
        SavableEmployee = new Employee();
    }

    private void EditEmployee()
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

}
