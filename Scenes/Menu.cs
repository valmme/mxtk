using System.Numerics;
using System.Runtime.InteropServices;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace mxtk.Scenes;

public class Menu
{
    private bool isCreateMenuOpen = false;
    private bool newAlbum = true;
    private string path = null;
    private int btnSize = 120;
    private int spacing = 15;
    
    private string[] albums;
    private Dictionary<string, Texture2D> albumIcons = new Dictionary<string, Texture2D>();
    
    private int row = 1;
    private int column = 0;
    
    private Texture2D albumTexture;
    
    Rectangle addBtn_source = new Rectangle(0, 0, 1080, 1080);
    Rectangle addBtn_dest = new Rectangle(15, 15, 120, 120);
    Color addBtn_color = Color.White;
    
    Rectangle removeBtn_source = new Rectangle(0, 0, 1080, 1080);
    Rectangle removeBtn_dest = new Rectangle(15, 170, 120, 120);
    Color removeBtn_color = Color.White;
    
    Rectangle albumBtn_source = new Rectangle(0, 0, 1080, 1080);
    Rectangle albumBtn_dest = new Rectangle(325, 125, 150, 150);
    Color albumBtn_color = Color.White;

    private InputBox input = new InputBox(15, new Rectangle(300, 285, 200, 35));

    private Rectangle confirmBtn = new Rectangle(210, 455, 185, 35);
    private Rectangle cancelBtn = new Rectangle(405, 455, 185, 35);
    
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
    
    public void Draw(Resources tex, ref int scene)
    {
        if (newAlbum)
        {
            albums = Directory.GetDirectories("albums");
            albumIcons.Clear();
            foreach (var album in albums)
            {
                string albumName = Path.GetFileName(album);
                string iconPath = Path.Combine(album, "icon.png");
                if (File.Exists(iconPath))
                    albumIcons[albumName] = LoadTexture(iconPath);
                else
                    albumIcons[albumName] = tex.ALBUM_TEX;
            }
            newAlbum = false;
        }
        
        DrawTexturePro(tex.ADD_TEX, addBtn_source, addBtn_dest, Vector2.Zero, 0, addBtn_color);
        DrawTexturePro(tex.RMV_TEX, removeBtn_source, removeBtn_dest, Vector2.Zero, 0, removeBtn_color);
        DrawTextEx(tex.font, "Add an album", new Vector2(18, 140), 20, 1, Color.White);
        DrawTextEx(tex.font, "Remove album", new Vector2(18, 295), 20, 1, Color.White);

        int startX = 150;
        int startY = 15;

        int x = startX;
        int y = startY;

        foreach (string album in albums)
        {
            string albumName = Path.GetFileName(album);
            Texture2D texture = albumIcons[albumName];
            Color color = Color.White;
            Rectangle dest = new Rectangle(x, y, btnSize, btnSize);
            Rectangle source = new Rectangle(0, 0, texture.Width, texture.Height);
            
            if (CheckCollisionPointRec(GetMousePosition(), dest))
            {
                color = Color.Gray;
            }
            
            DrawTexturePro(texture, source, dest, Vector2.Zero, 0, color);
            DrawRectangleLinesEx(dest, 2, color);
            
            Vector2 textSize = MeasureTextEx(tex.font, albumName, 20, 1);
            DrawTextEx(tex.font, albumName, new Vector2(x + btnSize / 2 - textSize.X / 2, y + btnSize + 5), 20, 1, Color.White);
            
            x += btnSize + spacing;
            
            if (x + btnSize > 800)
            {
                x = startX;
                y += btnSize + spacing + 20;
            }
            
            
        }
        
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

            if (CheckCollisionPointRec(GetMousePosition(), removeBtn_dest))
            {
                removeBtn_color = Color.LightGray;
                if (IsMouseButtonPressed(MouseButton.Left))
                {
                    removeBtn_color = Color.White;
                }
            }

            else removeBtn_color = Color.White; 
        }

        else
        {
            DrawContext(tex);
        }

        row = 0;
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

            if (path != null)
            {
                UnloadTexture(albumTexture);
            }

            albumTexture = LoadTexture(path);
            albumBtn_source.Width = albumTexture.Width;
            albumBtn_source.Height = albumTexture.Height;
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
            
            if (IsMouseButtonPressed(MouseButton.Left))
            {
                string name = new string(input.name, 0, input.letterCount);
                
                if (name.Length != 0)
                {
                    input.letterCount = 0;
                    isCreateMenuOpen = false;
                    if (!Directory.Exists("albums")) Directory.CreateDirectory("albums");
                    if (!Directory.Exists($"albums/{name}")) Directory.CreateDirectory($"albums/{name}");
                    Array.Clear(input.name, 0, 15);
                    if (path != null)
                    {
                        File.Copy(path, $"albums/{name}/icon.png");
                        UnloadTexture(albumTexture);
                    }
                    newAlbum = true;
                }
                else
                {
                    MessageBox(IntPtr.Zero, "Please enter name for album.", "Error", 0x10);
                }
            }
        }
        
        if (CheckCollisionPointRec(GetMousePosition(), cancelBtn))
        {
            DrawRectangleLinesEx(cancelBtn, 2, Color.White);
            if (IsMouseButtonPressed(MouseButton.Left))
            {
                isCreateMenuOpen = false;
                Array.Clear(input.name, 0, 15);
                input.letterCount = 0;
                if (path != null) UnloadTexture(albumTexture);
                path = null;
            }
        }
        
        input.Draw(tex);

        if (CheckCollisionPointRec(GetMousePosition(), albumBtn_dest) || path == null)
        {
            albumBtn_color = Color.Gray;
            DrawTextEx(tex.font, "      Drop\npicture here", new Vector2(360, 185), 15, 1, Color.White);
        }

        else
        {
            albumBtn_color = Color.White;
        }
    }

    public void UnloadAlbumIcons(Texture2D defaultTex)
    {
        foreach (var tex in albumIcons.Values)
        {
            if (tex.Id != defaultTex.Id)
                UnloadTexture(tex);
        }
        albumIcons.Clear();
    }
}
