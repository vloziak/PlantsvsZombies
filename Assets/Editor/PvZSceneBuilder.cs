using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Меню PvZ -> Create Game Scene: створює папки проєкту і базову сцену Game
// (камера, фон, газон із сіткою). Потрібно запустити один раз.
public static class PvZSceneBuilder
{
    private const string ScenePath = "Assets/Scenes/Game.unity";

    // Фон lawn.jpg (734x449 px). При 45 px на юніт картинка ~16.3 x 10 юнітів.
    // Газон на ній: x 185..704, y 52..399 px -> 9 x 5 клітинок по ~57.7 x 69.4 px.
    private const string BackgroundPath = "Assets/Art/Background/lawn.jpg";
    private const float BackgroundPixelsPerUnit = 45f;
    private static readonly Vector3 LawnTopLeft = new Vector3(-4.04f, 3.83f, 0f);
    private static readonly Vector2 LawnCellSize = new Vector2(1.282f, 1.542f);

    private static readonly string[] Folders =
    {
        "Assets/Art/Background",
        "Assets/Art/Plants",
        "Assets/Art/Zombies",
        "Assets/Art/Projectiles",
        "Assets/Art/UI",
        "Assets/Audio",
        "Assets/Prefabs",
        "Assets/Data",
        "Assets/Scenes",
    };

    [MenuItem("PvZ/Create Game Scene")]
    private static void CreateGameScene()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        if (File.Exists(ScenePath) &&
            !EditorUtility.DisplayDialog("PvZ", $"{ScenePath} вже існує. Перезаписати?", "Так", "Ні"))
            return;

        foreach (string folder in Folders)
            CreateFolder(folder);

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Камера: 2D, видно приблизно 17.8 x 10 юнітів (при 16:9).
        var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);
        var camera = cameraObject.GetComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.16f, 0.30f, 0.12f);

        // Фон: якщо lawn.jpg немає - поле Sprite лишається пустим, перетягни картинку сама.
        var background = new GameObject("Background", typeof(SpriteRenderer));
        var backgroundRenderer = background.GetComponent<SpriteRenderer>();
        backgroundRenderer.sortingOrder = -100;
        backgroundRenderer.sprite = LoadBackgroundSprite();

        // Газон: лівий верхній кут поля. Без фону - поле 9 x 1.4 на 5 x 1.5 з місцем зверху під UI.
        var lawn = new GameObject("Lawn", typeof(GridManager));
        lawn.transform.position = new Vector3(-6.3f, 3f, 0f);

        if (backgroundRenderer.sprite != null)
        {
            // Камера по ширині картинки (16:9), сітка - точно по клітинках газону з фону.
            camera.orthographicSize = 4.6f;
            lawn.transform.position = LawnTopLeft;

            var grid = new SerializedObject(lawn.GetComponent<GridManager>());
            grid.FindProperty("cellSize").vector2Value = LawnCellSize;
            grid.ApplyModifiedPropertiesWithoutUndo();
        }

        EditorSceneManager.SaveScene(scene, ScenePath);
        AddSceneToBuild(ScenePath);

        Selection.activeGameObject = background;
        Debug.Log("PvZ: сцену Game створено. Перетягни картинку фону в Background -> Sprite, потім підгони Lawn.");
    }

    // Налаштовує картинку фону як Sprite з потрібним масштабом і повертає її.
    private static Sprite LoadBackgroundSprite()
    {
        if (AssetImporter.GetAtPath(BackgroundPath) is not TextureImporter importer)
            return null;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = BackgroundPixelsPerUnit;
        importer.mipmapEnabled = false;
        importer.SaveAndReimport();

        return AssetDatabase.LoadAssetAtPath<Sprite>(BackgroundPath);
    }

    private static void CreateFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent = Path.GetDirectoryName(path).Replace('\\', '/');
        CreateFolder(parent);
        AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
    }

    // Game стає першою сценою в Build Profiles (з неї стартує гра).
    private static void AddSceneToBuild(string path)
    {
        var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        scenes.RemoveAll(s => s.path == path);
        scenes.Insert(0, new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
