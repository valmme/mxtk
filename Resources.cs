using Raylib_cs;
using static Raylib_cs.Raylib;

namespace mxtk;

public class Resources
{
    public Texture2D ADD_TEX;
    public Texture2D RMV_TEX;
    public Texture2D ALBUM_TEX;
    public Font font;

    public Resources()
    {
        ADD_TEX = LoadTexture("Assets/add.png");
        RMV_TEX = LoadTexture("Assets/remove.png");
        ALBUM_TEX = LoadTexture("Assets/album.png");

        font = LoadFontEx("Assets/font.ttf", 20, null, 250);
        SetTextureFilter(font.Texture, TextureFilter.Point);
    }

    public void Unload()
    {
        UnloadTexture(ADD_TEX);
        UnloadTexture(RMV_TEX);
        UnloadTexture(ALBUM_TEX);
        UnloadFont(font);
    }
}