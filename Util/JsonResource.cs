using Godot;
using Newtonsoft.Json.Linq;

namespace Spacelancer.Util;

public class JsonResource
{
    public static JObject LoadFromResource(string resourceName)
    {
        using var file = FileAccess.Open($"res://Configuration/Conversations/{resourceName}.json", FileAccess.ModeFlags.Read);
        string content = file.GetAsText();
        
        var json = JObject.Parse(content);
        return json;
    }
    
    // Read in file from res:// path
    // Do newtonsofty things?
}