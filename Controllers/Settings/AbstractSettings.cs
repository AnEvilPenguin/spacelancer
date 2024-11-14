using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Spacelancer.Controllers.Settings;

/// <summary>
/// Implements common logic for all 'settings' classes
/// </summary>
internal abstract class AbstractSettings
{
    /// <summary>
    /// The name of the settings class. 
    /// Functions as a JSON key in the settings file.
    /// <br/>
    /// E.g. Settings under "Foo" would affect the "Foo" settings class:
    /// <br/>
    /// { "Foo": { "Bar": true } }
    /// </summary>
    internal abstract string Name { get; }

    /// <summary>
    /// Overrides default settings where appropriate.
    /// </summary>
    /// <param name="overrides">The settings object parsed from the settings file.</param>
    internal void OverrideSettings(JObject overrides)
    {
        if (!overrides.ContainsKey(Name))
            return;

        try
        {
            JsonConvert.PopulateObject(overrides.GetValue(Name).ToString(), this);
        }
        catch (JsonSerializationException ex)
        {
            Log.Warning("Failed to serialize {Name} settings. Check that the settings are all valid.\n\t{@JsonSerializationException}", Name, ex);
        }
        catch (Exception ex)
        {
            Log.Error("Failed to load {Name} settings.\n\t{@Exception}", Name, ex);
        }
    }
}