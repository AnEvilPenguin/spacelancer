using System;
using System.Collections.Generic;
using Serilog;
using Spacelancer.Util;

namespace Spacelancer.Components.NPCs;

public class NonPlayerCharacter
{
    public string Name { get; private set; }
    public List<Dialog> Dialog = new List<Dialog>();

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
            Dialog.AddRange(converted);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error loading dialog for {Name}", Name);
        }
    }
}