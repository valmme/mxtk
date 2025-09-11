using System.Numerics;
using System.Runtime.InteropServices;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace mxtk.Scenes;

public class Menu
{
    private bool isCreateMenuOpen = false;
    private string path = null;
    
    private Texture2D albumTexture;
    
    Rectangle addBtn_source = new Rectangle(0, 0, 15, 15);
    Rectangle addBtn_dest = new Rectangle(15, 15, 120, 120);
    Color addBtn_color = Color.White;
    
    Rectangle albumBtn_source = new Rectangle(0, 0, 1080, 1080);
    Rectangle albumBtn_dest = new Rectangle(325, 125, 150, 150);
    Color albumBtn_color = Color.White;

    private InputBox input = new InputBox(15, new Rectangle(305, 285, 200, 35));

    private Rectangle confirmBtn = new Rectangle(210, 455, 185, 35);
    private Rectangle cancelBtn = new Rectangle(405, 455, 185, 35);
    
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
    
    public void Draw(Resources tex, ref int scene)
    {
        DrawTexturePro(tex.ADD_TEX, addBtn_source, addBtn_dest, Vector2.Zero, 0, addBtn_color);
        DrawTextEx(tex.font, "Add an album", new Vector2(18, 140), 20, 1, Color.White);
        
        if (!isCreateMenuOpen)
        {
            if (CheckCollisionPointRec(GetMousePosition(), addBtn_dest))
            {
                addBtn_color = Color.LightGray;
                if (IsMouseButtonPressed(MouseButton.Left))
                {
                    isCreateMenuOpen = true;
                    addBtn_color = Color.White;
                }
            }
            else addBtn_color = Color.White;
        }

        else
        {
            DrawContext(tex);
        }
    }

    public void DrawContext(Resources tex)
    {
        if (IsFileDropped())
        {
            path = GetDroppedFiles()[0];
            string[] parts = path.Split('.');
            string format = parts[parts.Length - 1];
            
            if (format != "png")
            {
                MessageBox(IntPtr.Zero, "File must be PNG!", "Error", 0x10);
                path = null;
            }

            albumTexture = LoadTexture(path);
        }

        
        DrawRectangle(0, 0, 800, 600, Color.Black with {A=150});
        DrawRectangle(200, 100, 400, 400, Color.DarkGray);
        DrawRectangleLinesEx(new Rectangle(200, 100, 400, 400), 2, Color.Gray);
        
        if (path == null) DrawTexturePro(tex.ALBUM_TEX, albumBtn_source, albumBtn_dest, Vector2.Zero, 0, albumBtn_color);
        else DrawTexturePro(albumTexture, albumBtn_source, albumBtn_dest, Vector2.Zero, 0, albumBtn_color);
        DrawRectangleLinesEx(albumBtn_dest, 2, albumBtn_color);
        
        DrawRectangleRec(confirmBtn, Color.Gray);
        DrawRectangleRec(cancelBtn, Color.Gray);
        DrawTextEx(tex.font, "Confirm", new Vector2(265, 462), 20, 1, Color.White);
        DrawTextEx(tex.font, "Cancel", new Vector2(460, 462), 20, 1, Color.White);

        if (CheckCollisionPointRec(GetMousePosition(), confirmBtn))
        {
            DrawRectangleLinesEx(confirmBtn, 2, Color.White);
        }
        if (CheckCollisionPointRec(GetMousePosition(), cancelBtn))
        {
            DrawRectangleLinesEx(cancelBtn, 2, Color.White);
            if (IsMouseButtonPressed(MouseButton.Left))
            {
                isCreateMenuOpen = false;
                Array.Clear(input.name, 0, 15);
                input.letterCount = 0;
                path = null;
            }
        }
        
        input.Draw(tex);

        if (CheckCollisionPointRec(GetMousePosition(), albumBtn_dest) || path == null)
        {
            albumBtn_color = Color.Gray;
            DrawTextEx(tex.font, "      Drop\npicture here", new Vector2(360, 185), 15, 1, Color.White);
        }

        if (path != null)
        {
            
        }

        else albumBtn_color = Color.White;
    }
}