using Godot;
using Newtonsoft.Json.Linq;

namespace Spacelancer.Util;

public class JsonResource
{
    private readonly string _basePath;

    public JsonResource(string basePath)
    {
        _basePath = basePath;
    }
    
    public JObject LoadFromResource(string resourceName)
    {
        using var file = FileAccess.Open($"{_basePath}/{resourceName}.json", FileAccess.ModeFlags.Read);
        string content = file.GetAsText();
        
        var json = JObject.Parse(content);
        return json;
    }
    
    // Read in file from res:// path
    // Do newtonsofty things?
}