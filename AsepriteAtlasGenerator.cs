#if TOOLS
using Godot;

[Tool]
public partial class AsepriteAtlasGenerator : EditorPlugin
{
    private Control dock;
    
    public override void _EnterTree()
    {
        // Initialization of the plugin goes here.
        dock = (Control)GD.Load<PackedScene>("addons/AsepriteAtlasGenerator/AsepriteAtlasGeneratorDock.tscn").Instantiate();
        AddControlToDock(DockSlot.LeftUl, dock);
    }

    public override void _ExitTree()
    {
        // Clean-up of the plugin goes here.
        // Remove the dock.
        RemoveControlFromDocks(dock);
        // Erase the control from the memory.
        dock.Free();
    }
}
#endif