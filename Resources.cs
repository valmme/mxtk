using Raylib_cs;
using static Raylib_cs.Raylib;

namespace mxtk;

public class Resources
{
    public Texture2D ADD_TEX;
    public Texture2D RMV_TEX;
    public Texture2D ALBUM_TEX;
    
    public Font FONT_20S;
    public Font FONT_15S;

    public Resources()
    {
        ADD_TEX = LoadTexture("Assets/add.png");
        RMV_TEX = LoadTexture("Assets/remove.png");
        ALBUM_TEX = LoadTexture("Assets/album.png");

        FONT_20S = LoadFontEx("Assets/font.ttf", 20, null, 250);
        FONT_15S = LoadFontEx("Assets/font.ttf", 15, null, 250);
        SetTextureFilter(FONT_20S.Texture, TextureFilter.Point);
        SetTextureFilter(FONT_15S.Texture, TextureFilter.Point);
    }

    public void Unload()
    {
        UnloadTexture(ADD_TEX);
        UnloadTexture(RMV_TEX);
        UnloadTexture(ALBUM_TEX);
        
        UnloadFont(FONT_20S);
        UnloadFont(FONT_15S);
    }
}