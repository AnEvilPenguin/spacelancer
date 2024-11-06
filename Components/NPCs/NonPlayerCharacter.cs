using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using Spacelancer.Util;

namespace Spacelancer.Components.NPCs;

public class NonPlayerCharacter
{
    public string Name { get; private set; }
    private readonly List<Dialog> _dialogList = new List<Dialog>();

    public NonPlayerCharacter(string name)
    {
        Name = name;
    }

    public void LoadDialog()
    {
        var dialogs = JsonResource.LoadFromResource(Name.ToLower())?.GetValue("Dialog");
		
        if (dialogs == null)
            return;

        try
        {
            var converted = dialogs.ToObject<IEnumerable<Dialog>>();
            _dialogList.AddRange(converted);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error loading dialog for {Name}", Name);
        }
    }

    public Dialog GetDialog() =>
        _dialogList.FirstOrDefault((d) => d.Type == DialogType.Greeting);
    
    public Dialog GetDialog(int id) =>
        _dialogList.FirstOrDefault((d) => d.Id == id);

}