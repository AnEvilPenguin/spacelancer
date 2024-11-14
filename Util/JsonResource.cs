using System;
using Godot;
using Newtonsoft.Json.Linq;
using Serilog;

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
        var path = $"{_basePath}/{resourceName}.json";
        
        Log.Debug("Loading resource {path}", path);
        
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);

        if (file == null)
        {
            Log.Debug("Failed to load resource {path}", path);
            return new JObject();
        }
        
        string content = file.GetAsText();
        
        var json = JObject.Parse(content);
        return json;
    }

    public JToken GetTokenFromResource(string resourceName, string tokenName)
    {
        Log.Debug("Getting token {tokenName} from {resourceName}", tokenName, resourceName);
        
        var content = LoadFromResource(resourceName);

        if (!content.TryGetValue(tokenName, out JToken token))
        {
            Log.Debug("Failed to get token {tokenName} from {resourceName}", tokenName, resourceName);
            return null;
        }
        
        return token;
    }
}