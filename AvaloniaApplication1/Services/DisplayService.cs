using System.Collections.Generic;
using System.Linq;
using AvaloniaApplication1.Engine.Platform;
using AvaloniaApplication1.Models;

namespace AvaloniaApplication1.Services;

public class DisplayService
{
    public DisplayOption GetOption(int index) => new DisplayOption(index, GetDisplayName(index));
    
    public List<DisplayOption> GetOptions()
    {
        return Display.GetAll().Select(d =>
        {
            var name = GetDisplayName(d.Index);
            return new DisplayOption(d.Index, name);
        }).ToList();
    }
    
    public string GetDisplayName(int index) => (index + 1).ToString();
    
    public int GetDisplayIndex(string name) => int.Parse(name) - 1;
}