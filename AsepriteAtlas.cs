using Godot;
using Godot.Collections;

namespace nis.addons.AsepriteAtlasGenerator;

public partial class AsepriteAtlas : Resource
{
    
    [ExportGroup("Paths")]
    [Export] public string AtlasName = "TestAtlas";
    [Export] public string OutputPath = "res://textures/whatever";
    [Export] public string OutputFileNameFormat = "<atlasName>~<filename>.tres";
    [Export] public string TexturePath = "res://textures/whatever";
    [Export] public string JsonPath = "res://textures/whatever";

    [Export] public Array<AtlasTexture> AtlasTextures;
}