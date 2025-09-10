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
    public static Textures tex = new Textures();
    static Menu menu = new Menu();
    
    static void Main()
    {
        if (!Directory.Exists("albums")) Directory.CreateDirectory("albums");
        
        InitWindow(screenWidth, screenHeight, "mxtk");
        Image icon = LoadImage("Assets/logo.png");
        unsafe {Raylib.ImageFormat(ref icon, PixelFormat.UncompressedR8G8B8A8);}
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
            if (scene == 0)
            {
                camera.Offset = Vector2.Zero;
                menu.Draw(tex.ADD_TEX, ref scene);
            }

            switch (scene)
            {
                case 0: camera.Offset = Vector2.Zero; menu.Draw(tex.ADD_TEX, ref scene); break;
                case 1: camera.Offset = new Vector2(800, 0); menu.Draw(tex.ADD_TEX, ref scene); break;
            }
            
            EndDrawing();
        }
        
        tex.Unload();
        CloseWindow();
    }
}