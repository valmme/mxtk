using Raylib_cs;
using static Raylib_cs.Raylib;

namespace mxtk;

public class Resources
{
    public Texture2D ADD_TEX = LoadTexture("Assets/add.png");
    public Texture2D ALBUM_TEX = LoadTexture("Assets/album.png");
    public Font font = LoadFontEx("Assets/font.ttf", 32, null, 250);
    

    public void Unload()
    {
        UnloadTexture(ADD_TEX);
    }
}