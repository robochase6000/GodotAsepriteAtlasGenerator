using System;
using System.Collections.Generic;

namespace nis.addons.AsepriteAtlasGenerator.AsespriteJson;

public class AsepriteJsonData
{
    public List<FrameData> frames = new();
}

[Serializable]
public class FrameData
{
    public string filename;
    public Region frame;
    public bool rotated;
    public bool trimmed;
    public Region spriteSourceSize;
    public Region sourceSize;
    public int duration;
}

[Serializable]
public class Region
{
    public int x;
    public int y;
    public int w;
    public int h;
}