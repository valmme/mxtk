using System.Numerics;
using mxtk;
using mxtk.Scenes;
using Raylib_cs;
using static Raylib_cs.Raylib;

class Program
{
    public static int scene = 0;
    
    const int screenWidth = 800;
    const int screenHeight = 600;
    public static Resources tex = new Resources();
    static Menu menu = new Menu();
    
    static void Main()
    {
        if (!Directory.Exists("albums")) Directory.CreateDirectory("albums");
        
        InitWindow(screenWidth, screenHeight, "mxtk");
        
        Image icon = LoadImage("Assets/logo.png");
        ImageFormat(ref icon, PixelFormat.UncompressedR8G8B8A8);
        Texture2D icon_tex = LoadTextureFromImage(icon);
        SetTextureFilter(icon_tex, TextureFilter.Point);
        SetWindowIcon(icon);
        
        SetTargetFPS(60);
        
        Camera2D camera = new Camera2D();
        camera.Offset = Vector2.Zero;
        camera.Rotation = 0;
        camera.Zoom = 1;
        camera.Target = Vector2.Zero;

        while (!WindowShouldClose())
        {
            
            BeginDrawing();
            ClearBackground(new Color(40, 40, 40, 255));
            
            BeginMode2D(camera);
            
            switch (scene)
            {
                case 0: camera.Offset = Vector2.Zero; menu.Draw(tex, ref scene); break;
                case 1: camera.Offset = new Vector2(800, 0); menu.Draw(tex, ref scene); break;
            }
            
            EndDrawing();
        }
        
        tex.Unload();
        CloseWindow();
    }
}