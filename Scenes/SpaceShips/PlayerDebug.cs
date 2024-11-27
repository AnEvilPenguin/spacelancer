using System.Collections.Generic;
using Godot;
using Spacelancer.Components.Navigation.Software;

namespace Spacelancer.Scenes.SpaceShips;

public partial class Player
{
    private Label _rotationLabel;
    private Label _speedLabel;
    private Label _navLabel;

    private List<Label> _debugLabels;
    
    private Line2D _velocityLine;
    private Line2D _accelerationLine;

    protected override void DebugVelocity()
    {
        if (!OS.IsDebugBuild())
            return;
        
        PopulateDebugLine(ref _velocityLine, Colors.DarkGreen);
        
        DrawDebugLine(_velocityLine, GlobalPosition, GlobalPosition + Velocity);
    }
    
    protected override void DebugAcceleration(Vector2 acceleration)
    {
        if (!OS.IsDebugBuild())
            return;

        PopulateDebugLine(ref _accelerationLine, Colors.Blue, 1);

        // exaggerate the acceleration so it can be easily seen.
        var end = GlobalPosition + acceleration * 15;
        
        DrawDebugLine(_accelerationLine, GlobalPosition, end);
    }
    
    protected override void DebugRotation()
    {
        if (!OS.IsDebugBuild())
            return;
        
        PopulateDebugLabel(ref _rotationLabel);
        PositionDebugLabel(_rotationLabel, 40, 100);
        
        // interpolate float to 2dp (Fixed point), and utf degrees symbol
        // https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
        _rotationLabel.Text = $"{RotationDegrees:F2}\u00b0";
    }

    protected override void DebugSpeed(float speed)
    {
        if (!OS.IsDebugBuild())
            return;
        
        PopulateDebugLabel(ref _speedLabel);
        PositionDebugLabel(_speedLabel, 35, 125);
        
        _speedLabel.Text = $"{speed}";

        if (speed < MaxSpeed * 0.5)
            _speedLabel.AddThemeColorOverride("font_color", Colors.Green);
        else if (speed < MaxSpeed * 0.9)
            _speedLabel.AddThemeColorOverride("font_color", Colors.Yellow);
        else
            _speedLabel.AddThemeColorOverride("font_color", Colors.Red);
    }

    protected override void DebugNav()
    {
        if (!OS.IsDebugBuild())
            return;
        
        PopulateDebugLabel(ref _navLabel);
        PositionDebugLabel(_navLabel, 35, 125);
        
        _navLabel.Text = $"{NavComputer.Name ?? "No Nav Computer"}";
        
        if (NavSoftware is null)
            _navLabel.AddThemeColorOverride("font_color", Colors.Red);
        else if (NavSoftware.GetType() == typeof(PlayerNavigation))
            _navLabel.AddThemeColorOverride("font_color", Colors.Green);
        else
            _navLabel.AddThemeColorOverride("font_color", Colors.Orange);
    }

    private void PopulateDebugLabel(ref Label label)
    {
        if (label != null)
            return;
        
        if (_debugLabels == null)
            _debugLabels = new List<Label>();

        label = new Label();
        
        AddChild(label);
        _debugLabels.Add(label);
    }

    private void PopulateDebugLine(ref Line2D line, Color color, int zIncrement = 0)
    {
        if (line != null)
            return;
        
        line = new Line2D() {DefaultColor = color, Width = 2};
        line.ZIndex += zIncrement;
        
        // We don't want the line rotating with this node.
        // We may need to change this to explicitly use the scene (or possibly the GameController)
        //     if the parent becomes ambiguous.
        GetParent().AddChild(line);
    }

    private void PositionDebugLabel(Label label, int x, int invertedX)
    {
        bool shouldInvert = Mathf.Abs(RotationDegrees) > 90;
        var index = _debugLabels.IndexOf(label);

        label.Position = shouldInvert ? new Vector2(invertedX, 20 * index) : new Vector2(x, 20 * index);
        label.Rotation = shouldInvert ? Mathf.Pi : 0;
    }

    private void DrawDebugLine(Line2D line, Vector2 start, Vector2 end)
    {
        line.ClearPoints();
        
        line.AddPoint(start);
        line.AddPoint(end);
    }
    
    // TODO debug options
    // health
}