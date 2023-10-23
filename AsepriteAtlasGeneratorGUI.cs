#if TOOLS
using System.IO;
using Godot;
using Godot.Collections;
using nis.addons.AsepriteAtlasGenerator;
using nis.addons.AsepriteAtlasGenerator.AsespriteJson;
using FileAccess = Godot.FileAccess;

[Tool]
public partial class AsepriteAtlasGeneratorGUI : Control
{
    [Export] public Button SelectAtlasButton;
    [Export] public FileDialog FileDialog;

    [ExportGroup("runtime settings")]
    [Export] public Vector2I RuntimeWindowSize = new Vector2I(640, 480);
    [Export] public Window.ContentScaleModeEnum ContentScaleModeEnum = Window.ContentScaleModeEnum.CanvasItems;
    [Export] public float ContentScaleFactor = 0.3f;
    [Export] public Window.ContentScaleAspectEnum ContentScaleAspect = Window.ContentScaleAspectEnum.Ignore;
    
    public override void _Ready()
    {
        base._Ready();
        SelectAtlasButton.Pressed += OnSelectAtlasButtonPressed;
        FileDialog.FileSelected += OnFileSelected;
        FileDialog.FilesSelected += OnMultipleFilesSelected;

        var window = GetWindow();
        window.Size = RuntimeWindowSize;
        window.ContentScaleMode = ContentScaleModeEnum;
        window.ContentScaleFactor = ContentScaleFactor;
        window.ContentScaleAspect = ContentScaleAspect;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        SelectAtlasButton.Pressed -= OnSelectAtlasButtonPressed;
        FileDialog.FileSelected -= OnFileSelected;
        FileDialog.FilesSelected -= OnMultipleFilesSelected;
    }

    private void OnSelectAtlasButtonPressed()
    {
        FileDialog.Show();
    }

    private void OnFileSelected(string path)
    {
        HandleFileSelected(path);
    }

    private void OnMultipleFilesSelected(string[] paths)
    {
        foreach (var path in paths)
        {
            HandleFileSelected(path);
        }
    }
    
    private void HandleFileSelected(string path)
    {
        AsepriteAtlas asepriteAtlas = ResourceLoader.Load<AsepriteAtlas>(path, cacheMode:ResourceLoader.CacheMode.Ignore);
        if (asepriteAtlas == null)
        {
            GD.PrintErr($"failed to open AsepriteAtlas .tres file at path {path}");
        }
        
        var asepriteData = LoadJson(asepriteAtlas.JsonPath);
        Texture2D atlasImage = ResourceLoader.Load<Texture2D>(asepriteAtlas.TexturePath);
        
        GD.Print($"frames loaded:{asepriteData.frames.Count}");

        System.Collections.Generic.Dictionary<string, AtlasTexture> existingAtlasTextures = new System.Collections.Generic.Dictionary<string, AtlasTexture>();
        foreach (var existingAtlasTexture in asepriteAtlas.AtlasTextures)
        {
            var filename = GetFilenameFor(existingAtlasTexture);
            if (!string.IsNullOrEmpty(filename))
            {
                existingAtlasTextures.Add(filename, existingAtlasTexture);
            }
        }
        
        for (int i = 0; i < asepriteData.frames.Count; i++)
        {
            var frameData = asepriteData.frames[i];

            AtlasTexture atlasTexture = null;
            if (!existingAtlasTextures.TryGetValue(frameData.filename, out atlasTexture))
            {
                atlasTexture = new();
                atlasTexture.SetMeta("filename", Variant.CreateFrom(frameData.filename));
                existingAtlasTextures.Add(frameData.filename, atlasTexture);
            }
            
            atlasTexture.Atlas = atlasImage;
            atlasTexture.Region = new Rect2(
                frameData.frame.x, 
                frameData.frame.y, 
                frameData.frame.w, 
                frameData.frame.h
            );

            //[Export] public string OutputFileNameFormat = "<atlasName>~<filename>.tres";
            var atlasFileName = asepriteAtlas.OutputFileNameFormat;
            atlasFileName = atlasFileName.Replace("<atlasName>", asepriteAtlas.AtlasName);
            atlasFileName = atlasFileName.Replace("<filename>", frameData.filename);
            var atlasSavePath = Path.Combine(asepriteAtlas.OutputPath, $"{atlasFileName}");
            Directory.CreateDirectory(Path.GetDirectoryName(ProjectSettings.GlobalizePath(atlasSavePath)));

            var atlasTextureSaveResult = ResourceSaver.Save(atlasTexture, atlasSavePath);
            
            GD.Print($"saving AtlasTexture result:{atlasTextureSaveResult}: {atlasSavePath}");
        }

        asepriteAtlas.AtlasTextures = new Array<AtlasTexture>(existingAtlasTextures.Values);
        var asepriteAtlasSaveResult = ResourceSaver.Save(asepriteAtlas, path);
        GD.Print($"saving asepriteAtlas result:{asepriteAtlasSaveResult}: {path}");
    }

    private string GetFilenameFor(AtlasTexture atlasTexture)
    {
        if (atlasTexture.HasMeta("filename"))
        {
            return atlasTexture.GetMeta("filename").AsString();
        }
        return null;
    }
    
    private void TestRun()
    {
        GD.Print("OnTestButtonPressed!");
        var asepriteData = LoadJson("res://textures/nis-sprites.json");
        GD.Print($"frames loaded:{asepriteData.frames.Count}");

        // res://.godot/imported/nis-sprites.png-f28c5295eaeed7a6b4ebb6ec8e46666a.ctex
        Texture2D atlasImage = ResourceLoader.Load<Texture2D>("res://textures/nis-sprites.png");
        //var image = ResourceLoader.Load("res://textures/nis-sprites.png");
        GD.Print($"image:{(atlasImage != null ? atlasImage.ResourcePath : "null")}");

        for (int i = 0; i < asepriteData.frames.Count; i++)
        {
            var frameData = asepriteData.frames[i];
            
            AtlasTexture atlasTexture = new();
            atlasTexture.Atlas = atlasImage;
            atlasTexture.Region = new Rect2(
                frameData.frame.x, 
                frameData.frame.y, 
                frameData.frame.w, 
                frameData.frame.h
            );

            var result = ResourceSaver.Save(atlasTexture, $"res://textures/test-atlas-{frameData.filename}.tres");
            
            GD.Print($"result:{result}");
        }
        
    }

    private AsepriteJsonData LoadJson(string path)
    {
        using var file = FileAccess.Open("res://textures/nis-sprites.json", FileAccess.ModeFlags.Read);
        string content = file.GetAsText();
        
        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<AsepriteJsonData>(content);
        return data;
    }
}
#endif