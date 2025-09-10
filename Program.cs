using Raylib_cs;
using static Raylib_cs.Raylib;

class Program
{
    const int screenWidth = 800;
    const int screenHeight = 600;
    
    static void Main()
    {
        if (!Directory.Exists("albums")) Directory.CreateDirectory("albums");
        
        InitWindow(screenWidth, screenHeight, "mxtk");
        SetTargetFPS(60);

        while (!WindowShouldClose())
        {
            BeginDrawing();
            ClearBackground(new Color(40, 40, 40, 255));
            
            EndDrawing();
        }
        
        CloseWindow();
    }
}