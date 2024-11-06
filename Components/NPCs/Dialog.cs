using System.Collections.Generic;

namespace Spacelancer.Components.NPCs;

public class Dialog
{
    public int Id;
    public DialogType Type;
    public string Text;
    public List<Response> Responses = new List<Response>();
}