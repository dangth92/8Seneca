namespace WpfApp.Models;

public class MethodOption
{
    public string Title { get; set; }
    public string[] Options { get; set; }

    public MethodOption(string title, string[] options)
    {
        Title = title;
        Options = options;
    }
}
