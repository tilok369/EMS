﻿using EMS.App.MVVM;
using EMS.Model;
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

    private void SaveEmployee()
    {
        Employees.Add(new Employee
        {
            id = 102,
            name = "Another Person",
            email = "another@gmail.com",
            gender = "female",
            status = "active"
        });
    }

    private void DeleteEmployee()
    {
        Employees.Remove(SelectedEmployee);
    }

    private void ClearEmployee()
    {
        SelectedEmployee = null;
    }

    private void EditEmployee()
    {
        
    }

}
