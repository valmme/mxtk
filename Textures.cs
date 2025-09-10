using Raylib_cs;
using static Raylib_cs.Raylib;

namespace mxtk;

public class Textures
{
    public Texture2D ADD_TEX = LoadTexture("Assets/add.png");

    public void Unload()
    {
        UnloadTexture(ADD_TEX);
    }
}