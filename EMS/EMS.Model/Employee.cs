namespace EMS.Model;

public class Employee
{
    public long id { get; set; }
    public string name { get; set; } = null!;
    public string email { get; set; } = null!;
    public string gender { get; set; } = null!;
    public string status { get; set; } = null!;
}