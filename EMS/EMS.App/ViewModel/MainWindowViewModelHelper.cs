using EMS.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EMS.App.ViewModel;

public class MainWindowViewModelHelper
{
    public static ObservableCollection<string> InitializeStatues()
    {
        var statuses = new ObservableCollection<string>();
        statuses.Add("active");
        statuses.Add("inactive");
        return statuses;
    }

    public static ObservableCollection<string> InitializeGenders()
    {
        var genders = new ObservableCollection<string>();
        genders.Add("male");
        genders.Add("female");

        return genders;
    }

    public static string FormatMessage(IEnumerable<ValidationMessage> messages)
    {
        return string.Join(",", messages.Select(i => i.field + ": " + i.message));
    }
}
