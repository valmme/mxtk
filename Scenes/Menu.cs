using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace mxtk.Scenes;

public class Menu
{
    private bool isCreateMenuOpen = false;
    private bool isRemoveMenuOpen = false;
    private bool newAlbum = true;
    private string path = null;
    private int btnSize = 120;
    private int spacing = 15;
    private string[] albums;
    private Dictionary<string, Texture2D> albumIcons = new Dictionary<string, Texture2D>();
    private HashSet<string> selectedForRemove = new HashSet<string>();
    private Texture2D albumTexture;
    private InputBox input = new InputBox(15, new Rectangle(300, 285, 200, 35));

    private Rectangle addBtn_source = new Rectangle(0, 0, 1080, 1080);
    private Rectangle addBtn_dest = new Rectangle(15, 15, 120, 120);
    private Color addBtn_color = Color.White;

    private Rectangle removeBtn_source = new Rectangle(0, 0, 1080, 1080);
    private Rectangle removeBtn_dest = new Rectangle(15, 170, 120, 120);
    private Color removeBtn_color = Color.White;

    private Rectangle albumBtn_source = new Rectangle(0, 0, 1080, 1080);
    private Rectangle albumBtn_dest = new Rectangle(325, 125, 150, 150);
    private Color albumBtn_color = Color.White;

    private bool showLimitMessage = false;
    private float limitMessageTimer = 0f;
    private float addBtnFlash = 0f;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    public void Draw(Resources tex, ref int scene)
    {
        if (newAlbum)
        {
            if (!Directory.Exists("albums")) Directory.CreateDirectory("albums");
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

        if (addBtnFlash > 0f)
        {
            addBtnFlash -= GetFrameTime() / 0.3f;
            if (addBtnFlash < 0f) addBtnFlash = 0f;
        }

        Color baseAddColor = Color.White;

        if (!isCreateMenuOpen)
        {
            if (CheckCollisionPointRec(GetMousePosition(), addBtn_dest))
            {
                baseAddColor = Color.LightGray;
                if (IsMouseButtonPressed(MouseButton.Left))
                {
                    if (Directory.GetDirectories("albums").Length == 12)
                    {
                        showLimitMessage = true;
                        limitMessageTimer = 0f;
                        addBtnFlash = 1f;
                    }
                    else
                    {
                        isCreateMenuOpen = true;
                    }
                }
            }

            if (CheckCollisionPointRec(GetMousePosition(), removeBtn_dest))
            {
                removeBtn_color = Color.LightGray;
                if (IsMouseButtonPressed(MouseButton.Left))
                {
                    isRemoveMenuOpen = !isRemoveMenuOpen;
                    if (isRemoveMenuOpen) selectedForRemove.Clear();
                }
            }
            else removeBtn_color = Color.White;
        }

        addBtn_color = LerpColor(baseAddColor, Color.Red, addBtnFlash);

        DrawTexturePro(tex.ADD_TEX, addBtn_source, addBtn_dest, Vector2.Zero, 0, addBtn_color);
        DrawTexturePro(tex.RMV_TEX, removeBtn_source, removeBtn_dest, Vector2.Zero, 0, removeBtn_color);
        DrawTextEx(tex.FONT_20S, "Add an album", new Vector2(18, 140), 20, 1, Color.White);
        DrawTextEx(tex.FONT_20S, "Remove album", new Vector2(15, 295), 20, 1, Color.White);

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

            if (CheckCollisionPointRec(GetMousePosition(), dest) && !(isCreateMenuOpen || isRemoveMenuOpen))
                color = Color.Gray;

            DrawTexturePro(texture, source, dest, Vector2.Zero, 0, color);
            DrawRectangleLinesEx(dest, 2, Color.White);
            Vector2 textSize = MeasureTextEx(tex.FONT_20S, albumName, 20, 1);
            DrawTextEx(tex.FONT_20S, albumName, new Vector2(x + btnSize / 2 - textSize.X / 2, y + btnSize + 5), 20, 1, Color.White);

            if (isRemoveMenuOpen)
            {
                Rectangle checkRect = new Rectangle(dest.X + dest.Width - 25, dest.Y + 5, 20, 20);
                DrawRectangleRec(checkRect, Color.Gray);
                DrawRectangleLinesEx(checkRect, 2, Color.White);
                if (selectedForRemove.Contains(albumName))
                    DrawRectangleRec(new Rectangle(checkRect.X + 4, checkRect.Y + 4, 12, 12), Color.White);

                if (IsMouseButtonPressed(MouseButton.Left) && CheckCollisionPointRec(GetMousePosition(), dest))
                {
                    if (selectedForRemove.Contains(albumName)) selectedForRemove.Remove(albumName);
                    else selectedForRemove.Add(albumName);
                }
            }

            x += btnSize + spacing;
            if (x + btnSize > 800)
            {
                x = startX;
                y += btnSize + spacing + 20;
            }
        }

        if (isRemoveMenuOpen)
        {
            Rectangle deleteBtn = new Rectangle(15, 335, 120, 35);
            bool canDelete = selectedForRemove.Count > 0;
            Color btnCol = canDelete ? Color.Red : Color.Gray;
            DrawRectangleRec(deleteBtn, btnCol);
            Vector2 txtSize = MeasureTextEx(tex.FONT_20S, "Remove", 20, 1);
            DrawTextEx(tex.FONT_20S, "Remove", 
                new Vector2(deleteBtn.X + deleteBtn.Width / 2 - txtSize.X / 2, deleteBtn.Y + 7), 
                20, 1, Color.White);
            
            DrawRectangleLinesEx(deleteBtn, 2, canDelete ? Color.White : Color.DarkGray);

            if (canDelete && CheckCollisionPointRec(GetMousePosition(), deleteBtn) && IsMouseButtonPressed(MouseButton.Left))
            {
                foreach (var name in selectedForRemove)
                {
                    string albumPath = Path.Combine("albums", name);
                    try
                    {
                        if (Directory.Exists(albumPath))
                            Directory.Delete(albumPath, true);
                    }
                    catch { }
                }
                selectedForRemove.Clear();
                newAlbum = true;
                isRemoveMenuOpen = false;
            }
        }

        if (isCreateMenuOpen) DrawCreateMenu(tex);

        if (showLimitMessage)
        {
            limitMessageTimer += GetFrameTime();
            float alpha = 0f;
            float fade = 1.0f;
            if (limitMessageTimer < fade)
                alpha = SmoothStep(limitMessageTimer / fade);
            else if (limitMessageTimer < fade + 1.0f)
                alpha = 1.0f;
            else if (limitMessageTimer < fade + 1.0f + fade)
                alpha = SmoothStep(1.0f - (limitMessageTimer - (fade + 1.0f)) / fade);
            else
            {
                showLimitMessage = false;
                alpha = 0f;
            }
            byte a = (byte)(alpha * 255f);
            var text = "You hit the max limit of albums";
            Vector2 size = MeasureTextEx(tex.FONT_20S, text, 20, 1);
            DrawTextEx(tex.FONT_20S, text, new Vector2(400 - size.X / 2, 560), 20, 1, new Color(255, 0, 0, (int)a));
        }
    }

    public void DrawCreateMenu(Resources tex)
    {
        Rectangle confirmBtn = new Rectangle(210, 455, 185, 35);
        Rectangle cancelBtn = new Rectangle(405, 455, 185, 35);

        if (IsFileDropped())
        {
            path = GetDroppedFiles()[0];
            string[] parts = path.Split('.');
            string format = parts[parts.Length - 1].ToLowerInvariant();
            if (format != "png")
            {
                MessageBox(IntPtr.Zero, "File must be PNG!", "Error", 0x10);
                path = null;
            }
            if (path != null) UnloadTexture(albumTexture);
            if (path != null)
            {
                albumTexture = LoadTexture(path);
                albumBtn_source.Width = albumTexture.Width;
                albumBtn_source.Height = albumTexture.Height;
            }
        }

        DrawRectangle(0, 0, 800, 600, Color.Black with { A = 150 });
        DrawRectangle(200, 100, 400, 400, Color.DarkGray);
        DrawRectangleLinesEx(new Rectangle(200, 100, 400, 400), 2, Color.Gray);

        if (path == null) DrawTexturePro(tex.ALBUM_TEX, albumBtn_source, albumBtn_dest, Vector2.Zero, 0, albumBtn_color);
        else DrawTexturePro(albumTexture, albumBtn_source, albumBtn_dest, Vector2.Zero, 0, albumBtn_color);
        DrawRectangleLinesEx(albumBtn_dest, 2, albumBtn_color);

        DrawRectangleRec(confirmBtn, Color.Gray);
        DrawRectangleRec(cancelBtn, Color.Gray);

        DrawTextEx(tex.FONT_20S, "Confirm", new Vector2(265, 462), 20, 1, Color.White);
        DrawTextEx(tex.FONT_20S, "Cancel", new Vector2(460, 462), 20, 1, Color.White);

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
                        try { File.Copy(path, $"albums/{name}/icon.png"); } catch { }
                        UnloadTexture(albumTexture);
                        albumTexture = default;
                        path = null;
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
                albumTexture = default;
                path = null;
            }
        }

        input.Draw(tex);

        if (CheckCollisionPointRec(GetMousePosition(), albumBtn_dest) || path == null)
        {
            albumBtn_color = Color.Gray;
            DrawTextEx(tex.FONT_15S, "      Drop\npicture here", new Vector2(360, 185), 15, 1, Color.White);
        }
        else albumBtn_color = Color.White;
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

    private Color LerpColor(Color a, Color b, float t)
    {
        if (t < 0f) t = 0f;
        if (t > 1f) t = 1f;
        return new Color(
            (byte)(a.R + (b.R - a.R) * t),
            (byte)(a.G + (b.G - a.G) * t),
            (byte)(a.B + (b.B - a.B) * t),
            (byte)(a.A + (b.A - a.A) * t)
        );
    }

    private float SmoothStep(float t)
    {
        if (t <= 0f) return 0f;
        if (t >= 1f) return 1f;
        return t * t * (3f - 2f * t);
    }
}
