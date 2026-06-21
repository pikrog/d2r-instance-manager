using System.Collections.Generic;
using System.Linq;
using AvaloniaApplication1.Engine.Platform;
using AvaloniaApplication1.Models;

namespace AvaloniaApplication1.Services;

public class DisplayService
{
    public List<DisplayOption> GetOptions()
    {
        return Display.GetAll()
            .Select(DisplayOption (d) => new DisplayOption.Specific(d.Index, d.Id, d.Description, d.Width, d.Height))
            .Prepend(new DisplayOption.Primary())
            .ToList();
    }
    
    public string GetDisplayName(int index) => (index + 1).ToString();
    
    public int GetDisplayIndex(string name) => int.Parse(name) - 1;
}