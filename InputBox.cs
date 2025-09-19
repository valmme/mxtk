using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace mxtk;

public class InputBox
{
    public char[] name;
    private int maxInputChars;
    public int letterCount = 0;
    private int framesCounter;
    private bool mouseOn = false;
    private Rectangle textBox;

    public InputBox(int maxChars, Rectangle box)
    {
        maxInputChars = maxChars;
        name = new char[maxInputChars];
        textBox = box;
    }

    public void Draw(Resources tex)
    {
        if (IsMouseButtonPressed(MouseButton.Left))
        {
            if (CheckCollisionPointRec(GetMousePosition(), textBox)) mouseOn = true;
            else mouseOn = false;
        }
        
        if (mouseOn)
        {

            SetMouseCursor(MouseCursor.IBeam);
            int key = GetCharPressed();

            while (key > 0)
            {
                if ((key >= 32) && (key <= 255) && (letterCount < maxInputChars))
                {
                    name[letterCount] = (char)key;
                    letterCount++;
                }

                key = GetCharPressed();
            }

            if (IsKeyPressed(KeyboardKey.Backspace))
            {
                letterCount -= 1;
                if (letterCount < 0) letterCount = 0;
                name[letterCount] = '\0';
            }
        }
        
        else SetMouseCursor(MouseCursor.Default);

        if (mouseOn) framesCounter += 1;
        else framesCounter = 0;
        
        DrawRectangleRec(textBox, new Color(200, 200, 200, 255));
        
        if (mouseOn)
        {
            DrawRectangleLinesEx(
                new Rectangle(
                    (int)textBox.X,
                    (int)textBox.Y,
                    (int)textBox.Width,
                    (int)textBox.Height),
                2,
                new Color(70, 70, 70, 255)
            );
        }
        
        else
        {
            DrawRectangleLinesEx(
                new Rectangle(
                    (int)textBox.X,
                    (int)textBox.Y,
                    (int)textBox.Width,
                    (int)textBox.Height),
                2,
                Color.Gray
            );
        }
        
        DrawTextEx(tex.FONT_20S, new string(name), new Vector2((int)textBox.X + 5, (int)textBox.Y + 6), 25, 1, Color.DarkGray);

        
        if (mouseOn)
        {
            if (letterCount < maxInputChars)
            {
                if ((framesCounter / 20 % 2) == 0)
                {
                    DrawTextEx(tex.FONT_20S,
                        "_",
                        new Vector2((int)textBox.X + 8 + MeasureText(new string(name), 25),
                        (int)textBox.Y + 7),
                        25,
                        1,
                        Color.DarkGray
                    );
                }
            }
        }

    }
}