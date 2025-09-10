using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace mxtk.Scenes;

public class Menu
{
    private bool isCreateMenuOpen = false;
    
    Rectangle addBtn_source = new Rectangle(0, 0, 15, 15);
    Rectangle addBtn_dest = new Rectangle(15, 15, 115, 115);
    Color addBtn_color = Color.White;
    
    public void Draw(Texture2D addTex, ref int scene)
    {
        if (!isCreateMenuOpen)
        {
            if (CheckCollisionPointRec(GetMousePosition(), addBtn_dest))
            {
                addBtn_color = Color.LightGray;
                if (IsMouseButtonPressed(MouseButton.Left)) scene = 1;
            }
            else addBtn_color = Color.White;
            DrawTexturePro(addTex, addBtn_source, addBtn_dest, Vector2.Zero, 0, addBtn_color);
        }
    }
}