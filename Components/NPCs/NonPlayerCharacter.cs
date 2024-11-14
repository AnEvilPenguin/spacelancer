using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using Spacelancer.Universe;
using Spacelancer.Util;

namespace Spacelancer.Components.NPCs;

public class NonPlayerCharacter : IEntity
{
    public string Id { get; }
    public string Name { get; }
    public string Summary { get; }
    public string Affiliation { get; }
    
    private readonly List<Dialog> _dialogList = new();
    
    public NonPlayerCharacter(string id, string name, string summary, string affiliation)
    {
        Id = id;
        Name = name;
        Summary = summary;
        Affiliation = affiliation;
    }

    public void LoadDialog(string locationId)
    {
        _dialogList.Clear();
        
        JsonResource conversationLoader = new($"res://Configuration/Conversations/{locationId}");
        var dialogs = conversationLoader.LoadFromResource(Id)?.GetValue("Dialog");
		
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