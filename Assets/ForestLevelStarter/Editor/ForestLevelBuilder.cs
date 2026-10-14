// Unity 2022 LTS / 2023 / Unity 6
// Copy Assets/ForestLevelStarter into your Unity project's Assets directory.
// In Unity: Tools > 2D > Build Jungle Level 01
// This script adds a NEW root to the CURRENT scene; it does not delete existing objects.
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ForestLevelBuilder
{
    const string Art = "Assets/ForestLevelStarter/Art/environment";
    const float PPU = 16f;
    static readonly Dictionary<string, Sprite> SpriteCache = new Dictionary<string, Sprite>();
    static Transform root, background, ground, props, platforms, hazards, markers;

    [MenuItem("Tools/2D/Build Jungle Level 01")]
    public static void Build()
    {
        if (GameObject.Find("Jungle_Level_01") != null)
        {
            EditorUtility.DisplayDialog("Уровень уже создан", "В сцене уже есть Jungle_Level_01. Чтобы построить заново, сначала удали этот объект из Hierarchy.", "OK");
            return;
        }

        if (AssetDatabase.LoadAssetAtPath<Texture2D>(Art + "/tileset.png") == null)
        {
            EditorUtility.DisplayDialog("Нет ассетов", "Проверь путь Assets/ForestLevelStarter/Art/environment/tileset.png", "OK");
            return;
        }

        ConfigurePixelArt();
        SpriteCache.Clear();

        root = new GameObject("Jungle_Level_01").transform;
        Undo.RegisterCreatedObjectUndo(root.gameObject, "Create Jungle Level 01");
        background = Group("00_Background", root);
        ground = Group("01_Terrain", root);
        platforms = Group("02_Platforms", root);
        props = Group("03_Decorations", root);
        hazards = Group("04_Hazards", root);
        markers = Group("05_Gameplay", root);

        // Distant sky and soft jungle silhouettes.
        for (int x = -24; x < 144; x += 24)
            AddSprite("Sky", Art + "/Background/back.png", null, x, -6f, -200, background);
        for (int i = 0; i < 22; i++)
        {
            GameObject hill = AddSprite("Jungle_Parallax", Art + "/Background/middle.png", null,
                -5f + i * 7f, -8.5f, -150, background);
            hill.transform.localScale = Vector3.one * .64f;
        }

        // Six walkable areas separated by short, jumpable gaps.
        GroundSegment(0, 16, -3);
        GroundSegment(19, 37, -2);
        GroundSegment(40, 56, -3);
        GroundSegment(60, 78, -2);
        GroundSegment(82, 99, -2);
        GroundSegment(103, 121, -3);

        // Extra jumping routes, safe for a basic fox controller with speed = 5.
        Platform(13f, -1.9f, true);
        Platform(16.5f, -1.8f, false);
        Platform(33f, -0.9f, true);
        Platform(37f, -1.5f, false);
        Platform(53.5f, -1.7f, true);
        Platform(57.5f, -1.1f, true);
        Platform(77f, -0.9f, true);
        Platform(79.7f, -1.0f, false);
        Platform(98f, -1f, true);
        Platform(100.2f, -1.5f, true);

        // Scenic landmark props. They have NO colliders, so movement remains easy to test.
        Decor("tree-house", 4, -3, -12);
        Decor("bush", 9, -3, -4);
        Decor("palm", 21, -2, -12);
        Decor("rock-2", 30, -2, -10);
        Decor("pine", 35, -2, -12);
        Decor("straw-house", 43, -3, -12);
        Decor("bush", 51, -3, -4);
        Decor("tree", 61, -2, -12);
        Decor("plant-house", 68, -2, -12);
        Decor("rock-1", 76, -2, -10);
        Decor("pine", 84, -2, -12);
        Decor("wooden-house", 92, -2, -12);
        Decor("tree-house", 105, -3, -12);
        Decor("bush", 113, -3, -4);
        Decor("shrooms", 11, -3, -3);
        Decor("shrooms", 50, -3, -3);
        Decor("shrooms", 87, -2, -3);
        Decor("skulls", 73, -2, 4);
        Decor("crate", 27, -2, 4);
        Decor("big-crate", 48, -3, 4);
        Decor("sign", 2, -3, 5);

        // Touching a spike or falling into a pit resets the player to PlayerSpawn.
        foreach (int x in new[] { 10, 11 }) Spike(x, -3);
        foreach (int x in new[] { 26 }) Spike(x, -2);
        foreach (int x in new[] { 49, 50 }) Spike(x, -3);
        foreach (int x in new[] { 70, 71 }) Spike(x, -2);
        foreach (int x in new[] { 88, 89 }) Spike(x, -2);
        foreach (int x in new[] { 109, 110 }) Spike(x, -3);

        GameObject voidTrigger = new GameObject("DeathZone_BelowLevel");
        voidTrigger.transform.SetParent(hazards, false);
        voidTrigger.transform.position = new Vector3(60f, -11f, 0f);
        BoxCollider2D voidCollider = voidTrigger.AddComponent<BoxCollider2D>();
        voidCollider.isTrigger = true;
        voidCollider.size = new Vector2(155f, 2f);
        voidTrigger.AddComponent<ForestLevelHazard>();

        GameObject spawn = new GameObject("PlayerSpawn");
        spawn.transform.SetParent(markers, false);
        spawn.transform.position = new Vector3(2, -1.8f, 0);

        GameObject finish = AddSprite("FINISH_Door", Art + "/Props/door.png", null, 117f, -3f, 10, markers);
        BoxCollider2D finishTrigger = finish.AddComponent<BoxCollider2D>();
        finishTrigger.isTrigger = true;
        finishTrigger.offset = new Vector2(.65f, 1f);
        finishTrigger.size = new Vector2(1.2f, 1.9f);
        finish.AddComponent<ForestLevelGoal>();

        Selection.activeGameObject = root.gameObject;
        EditorUtility.DisplayDialog("Готово!", "Jungle_Level_01 добавлен в текущую сцену. Поставь своего лиса в PlayerSpawn и убедись, что у лиса Tag = Player. Сохрани сцену Ctrl+S.", "OK");
    }

    static void ConfigurePixelArt()
    {
        // Existing .meta files preserve the author's detailed sprite slicing.
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { Art });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) continue;
            if (importer.spritePixelsPerUnit == PPU && importer.filterMode == FilterMode.Point &&
                importer.textureCompression == TextureImporterCompression.Uncompressed && !importer.mipmapEnabled)
                continue;
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = PPU;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }
    }

    static Sprite LoadSprite(string path, string spriteName)
    {
        string key = path + ":" + (spriteName ?? "FIRST");
        if (SpriteCache.TryGetValue(key, out Sprite cached)) return cached;
        Sprite[] found = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();
        Sprite sprite = spriteName == null ? found.FirstOrDefault() : found.FirstOrDefault(s => s.name == spriteName);
        if (sprite == null) Debug.LogWarning("Не найден Sprite: " + key);
        SpriteCache[key] = sprite;
        return sprite;
    }

    static Transform Group(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        return obj.transform;
    }

    static GameObject AddSprite(string name, string path, string spriteName,
        float x, float y, int order, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        obj.transform.position = new Vector3(x, y, 0);
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = LoadSprite(path, spriteName);
        renderer.sortingOrder = order;
        return obj;
    }

    static void GroundSegment(int first, int last, int surfaceY)
    {
        Transform section = Group("Ground_" + first + "_" + last, ground);
        string path = Art + "/tileset.png";
        for (int x = first; x <= last; x++)
        {
            int topIndex = x == first ? 0 : x == last ? 2 : 1;
            AddSprite("Grass_" + x, path, "tileset_" + topIndex, x, surfaceY - 1, 0, section);
            for (int depth = 1; depth < 6; depth++)
            {
                int dirtIndex = x == first ? 9 : x == last ? 10 : 3;
                AddSprite("Dirt", path, "tileset_" + dirtIndex, x, surfaceY - 1 - depth, -1, section);
            }
        }

        // One collider per terrain island is much lighter than one per tile.
        GameObject solid = new GameObject("GroundCollider");
        solid.transform.SetParent(section, false);
        int width = last - first + 1;
        solid.transform.position = new Vector3(first + width * .5f, surfaceY - 3f, 0f);
        BoxCollider2D collider = solid.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(width, 6);
        // Create the Ground tag through Unity's Tags and Layers if you use the supplied Player.cs.
        // Unity does not allow creating new tags safely just by setting an unknown string.
        if (TagExists("Ground")) solid.tag = "Ground";
    }

    static bool TagExists(string tag)
    {
        return UnityEditorInternal.InternalEditorUtility.tags.Contains(tag);
    }

    static void Platform(float x, float y, bool longPlatform)
    {
        string name = longPlatform ? "platform-long" : "small-platform";
        GameObject platform = AddSprite("JumpPlatform", Art + "/Props/" + name + ".png", null,
            x, y, 5, platforms);
        BoxCollider2D collider = platform.AddComponent<BoxCollider2D>();
        float w = longPlatform ? 2f : 1f;
        collider.offset = new Vector2(w * .5f, .82f);
        collider.size = new Vector2(w * .97f, .26f);
        if (TagExists("Ground")) platform.tag = "Ground";
    }

    static void Decor(string asset, float x, float y, int order)
    {
        AddSprite("Decor_" + asset, Art + "/Props/" + asset + ".png", null, x, y, order, props);
    }

    static void Spike(float x, float y)
    {
        GameObject obj = AddSprite("Spike_Hazard", Art + "/Props/spikes.png", null, x, y, 8, hazards);
        BoxCollider2D collider = obj.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.offset = new Vector2(.47f, .23f);
        collider.size = new Vector2(.85f, .42f);
        obj.AddComponent<ForestLevelHazard>();
    }
}
