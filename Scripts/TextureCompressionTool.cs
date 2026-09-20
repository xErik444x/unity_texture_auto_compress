using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

#pragma warning disable CS0618

#pragma warning disable 0429

public class TextureCompressionTool : EditorWindow
{
    private Vector2 scrollPos;
    private int pcMaxSize = 1024;
    private int androidMaxSize = 512;
    private List<TextureInfo> textures = new List<TextureInfo>();
    private bool showCompressionSettings = true;
    private bool forceCompress = false;
    
    private SearchMode searchMode = SearchMode.Selection;
    private GameObject selectedGameObject;

    private enum SearchMode
    {
        Selection,
        AllProject
    }

    [MenuItem("Window/Texture Compression Tool")]
    public static void ShowWindow()
    {
        GetWindow<TextureCompressionTool>("Texture Compressor");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10);
        GUILayout.Label("🎯 Texture Compression Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        showCompressionSettings = EditorGUILayout.Foldout(showCompressionSettings, "Compression Settings", true);
        if (showCompressionSettings)
        {
            EditorGUI.indentLevel++;
            
            GUILayout.Label("📱 Android", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            androidMaxSize = EditorGUILayout.IntField("Max Size (px):", androidMaxSize);
            if (GUILayout.Button("256", GUILayout.Width(50))) androidMaxSize = 256;
            if (GUILayout.Button("512", GUILayout.Width(50))) androidMaxSize = 512;
            if (GUILayout.Button("1024", GUILayout.Width(50))) androidMaxSize = 1024;
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            GUILayout.Label("🖥️ PC", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            pcMaxSize = EditorGUILayout.IntField("Max Size (px):", pcMaxSize);
            if (GUILayout.Button("1024", GUILayout.Width(50))) pcMaxSize = 1024;
            if (GUILayout.Button("2048", GUILayout.Width(50))) pcMaxSize = 2048;
            if (GUILayout.Button("4096", GUILayout.Width(50))) pcMaxSize = 4096;
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            forceCompress = EditorGUILayout.Toggle("Force Compress", forceCompress);
            if (forceCompress)
            {
                EditorGUILayout.HelpBox("⚠️ Warning: Will overwrite existing compression settings", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox("✓ Only compress uncompressed textures", MessageType.Info);
            }
            
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space(10);

        GUILayout.Label("Search Mode", EditorStyles.boldLabel);
        searchMode = (SearchMode)EditorGUILayout.EnumPopup("Search in:", searchMode);

        if (searchMode == SearchMode.Selection)
        {
            EditorGUILayout.HelpBox("Select a GameObject in the Hierarchy", MessageType.Info);
            selectedGameObject = (GameObject)EditorGUILayout.ObjectField(
                "GameObject:", 
                selectedGameObject, 
                typeof(GameObject), 
                true
            );
        }

        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.cyan;
        
        string buttonText = searchMode == SearchMode.Selection ? 
            "🔍 SCAN GAMEOBJECT" : 
            "🔍 SCAN TEXTURES";
            
        if (GUILayout.Button(buttonText, GUILayout.Height(35)))
        {
            if (searchMode == SearchMode.Selection && selectedGameObject == null)
            {
                EditorUtility.DisplayDialog("Error", "Select a GameObject first", "OK");
            }
            else
            {
                ScanTextures();
            }
        }
        
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        string infoText = searchMode == SearchMode.Selection ? 
            $"Textures found: {textures.Count}" : 
            $"Textures found: {textures.Count}";
        EditorGUILayout.HelpBox(infoText, MessageType.Info);

        EditorGUILayout.Space(5);
        GUILayout.Label("Textures:", EditorStyles.boldLabel);
        
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.MinHeight(150));
        
        if (textures.Count == 0)
        {
            EditorGUILayout.HelpBox("Press the scan button to find textures", MessageType.Info);
        }
        else
        {
            float totalSizeMB = 0;
            foreach (var tex in textures)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(tex.name, EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Resolution: {tex.currentSize}px", EditorStyles.miniLabel);
                EditorGUILayout.LabelField($"Weight: {tex.currentWeightMB:F2} MB", EditorStyles.miniLabel);
                EditorGUILayout.LabelField($"Path: {tex.path}", EditorStyles.miniLabel);
                
                if (tex.isCompressed)
                {
                    EditorGUILayout.LabelField("Status: Already Compressed ✓", EditorStyles.miniLabel);
                }
                else
                {
                    EditorGUILayout.LabelField("Status: Uncompressed", EditorStyles.miniLabel);
                }
                
                EditorGUILayout.EndVertical();
                
                totalSizeMB += tex.currentWeightMB;
            }
            
            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox($"Total weight: {totalSizeMB:F2} MB", MessageType.Info);
        }
        
        GUILayout.EndScrollView();

        EditorGUILayout.Space(10);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("⚡ COMPRESS NOW", GUILayout.Height(45)))
        {
            if (textures.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "No textures to compress", "OK");
            }
            else if (EditorUtility.DisplayDialog("Confirm", 
                $"Compress {textures.Count} textures?\n\n" +
                $"Android: {androidMaxSize}px\n" +
                $"PC: {pcMaxSize}px",
                "Yes", "Cancel"))
            {
                CompressAllTextures();
            }
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(5);
        
        ShowGuide();
    }

    private void ShowGuide()
    {
        EditorGUILayout.Space(10);
        GUILayout.Label("📖 Quick Guide", EditorStyles.boldLabel);
        
        EditorGUILayout.HelpBox(
            "1. Select a GameObject in the Hierarchy or choose 'All Project'\n" +
            "2. Adjust Android and PC max sizes using buttons or input\n" +
            "3. Click 'SCAN GAMEOBJECT' or 'SCAN TEXTURES'\n" +
            "4. Review textures (✓ Already compressed ones won't be touched)\n" +
            "5. Enable 'Force Compress' to override existing settings\n" +
            "6. Click 'COMPRESS NOW' to apply compression\n" +
            "7. Check console for results and space saved",
            MessageType.Info);
    }

    private void ScanTextures()
    {
        textures.Clear();

        if (searchMode == SearchMode.Selection)
        {
            ScanGameObjectTextures(selectedGameObject);
        }
        else
        {
            ScanAllProjectTextures();
        }
    }

    private void ScanGameObjectTextures(GameObject gameObject)
    {
        EditorUtility.DisplayProgressBar("Scanning", "Finding textures in GameObject and children...", 0);

        textures.Clear();

        SkinnedMeshRenderer[] skinnedRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>(true);
        SpriteRenderer[] spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>(true);
        Image[] images = gameObject.GetComponentsInChildren<Image>(true);
        RawImage[] rawImages = gameObject.GetComponentsInChildren<RawImage>(true);

        int totalRenderers = skinnedRenderers.Length + meshRenderers.Length + spriteRenderers.Length + images.Length + rawImages.Length;
        int currentRenderer = 0;

        foreach (var renderer in skinnedRenderers)
        {
            if (renderer != null && renderer.sharedMaterials != null)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material != null)
                    {
                        ExtractTexturesFromMaterial(material);
                    }
                }
            }
            currentRenderer++;
            EditorUtility.DisplayProgressBar("Scanning", $"Scanning renderers... {currentRenderer}/{totalRenderers}", (float)currentRenderer / totalRenderers);
        }

        foreach (var renderer in meshRenderers)
        {
            if (renderer != null && renderer.sharedMaterials != null)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material != null)
                    {
                        ExtractTexturesFromMaterial(material);
                    }
                }
            }
            currentRenderer++;
            EditorUtility.DisplayProgressBar("Scanning", $"Scanning renderers... {currentRenderer}/{totalRenderers}", (float)currentRenderer / totalRenderers);
        }

        foreach (var renderer in spriteRenderers)
        {
            if (renderer != null && renderer.sprite != null)
            {
                AddTextureInfo(renderer.sprite.texture);
            }
            currentRenderer++;
            EditorUtility.DisplayProgressBar("Scanning", $"Scanning renderers... {currentRenderer}/{totalRenderers}", (float)currentRenderer / totalRenderers);
        }

        foreach (var img in images)
        {
            if (img != null && img.sprite != null)
            {
                AddTextureInfo(img.sprite.texture);
            }
            currentRenderer++;
            EditorUtility.DisplayProgressBar("Scanning", $"Scanning renderers... {currentRenderer}/{totalRenderers}", (float)currentRenderer / totalRenderers);
        }

        foreach (var rawImg in rawImages)
        {
            if (rawImg != null && rawImg.texture is Texture2D)
            {
                AddTextureInfo((Texture2D)rawImg.texture);
            }
            currentRenderer++;
            EditorUtility.DisplayProgressBar("Scanning", $"Scanning renderers... {currentRenderer}/{totalRenderers}", (float)currentRenderer / totalRenderers);
        }

        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("Done", $"Found {textures.Count} textures in {gameObject.name} and its children", "OK");
    }

    private void ExtractTexturesFromMaterial(Material material)
    {
        if (material == null || material.shader == null) return;

        // GetTexturePropertyNames devuelve SOLO propiedades de tipo textura,
        // asi nunca llamamos GetTexture() sobre floats/vectors (que es lo que
        // generaba los errores "doesn't have a texture property").
        string[] textureProps = material.GetTexturePropertyNames();

        foreach (string propertyName in textureProps)
        {
            if (string.IsNullOrEmpty(propertyName)) continue;

            Texture texture = material.GetTexture(propertyName);
            if (texture is Texture2D)
            {
                AddTextureInfo((Texture2D)texture);
            }
        }
    }


    private void AddTextureInfo(Texture2D texture)
    {
        if (texture == null) return;

        string assetPath = AssetDatabase.GetAssetPath(texture);
        if (string.IsNullOrEmpty(assetPath)) return;

        if (textures.Exists(t => t.path == assetPath)) return;

        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        bool isCompressed = false;

        if (importer != null)
        {
            TextureImporterPlatformSettings androidSettings = importer.GetPlatformTextureSettings("Android");
            TextureImporterPlatformSettings pcSettings = importer.GetPlatformTextureSettings("Standalone");

            isCompressed = (androidSettings.overridden || pcSettings.overridden);
        }

        FileInfo fileInfo = new FileInfo(assetPath);
        textures.Add(new TextureInfo
        {
            path = assetPath,
            name = texture.name,
            currentSize = Mathf.Max(texture.width, texture.height),
            currentWeightMB = fileInfo.Length / (1024f * 1024f),
            isCompressed = isCompressed
        });
    }

    private void ScanAllProjectTextures()
    {
        textures.Clear();
        string[] guids = AssetDatabase.FindAssets("t:Texture2D");
        
        EditorUtility.DisplayProgressBar("Scanning", "Finding textures...", 0);

        for (int i = 0; i < guids.Length; i++)
        {
            string guid = guids[i];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            
            if (tex != null && !path.Contains("Packages"))
            {
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                bool isCompressed = false;

                if (importer != null)
                {
                    TextureImporterPlatformSettings androidSettings = importer.GetPlatformTextureSettings("Android");
                    TextureImporterPlatformSettings pcSettings = importer.GetPlatformTextureSettings("Standalone");

                    isCompressed = (androidSettings.overridden || pcSettings.overridden);
                }

                FileInfo fileInfo = new FileInfo(path);
                textures.Add(new TextureInfo
                {
                    path = path,
                    name = tex.name,
                    currentSize = Mathf.Max(tex.width, tex.height),
                    currentWeightMB = fileInfo.Length / (1024f * 1024f),
                    isCompressed = isCompressed
                });
            }

            EditorUtility.DisplayProgressBar("Scanning", $"Processing {path}...", (float)i / guids.Length);
        }

        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("Done", $"Found {textures.Count} textures", "OK");
    }

    private void CompressAllTextures()
    {
        float totalBefore = 0;
        int compressedCount = 0;
        int skippedCount = 0;

        foreach (var tex in textures)
        {
            totalBefore += tex.currentWeightMB;
            if (tex.isCompressed && !forceCompress)
            {
                skippedCount++;
            }
            else
            {
                compressedCount++;
            }
        }

        for (int i = 0; i < textures.Count; i++)
        {
            var tex = textures[i];

            if (tex.isCompressed && !forceCompress)
            {
                continue;
            }

            TextureImporter importer = AssetImporter.GetAtPath(tex.path) as TextureImporter;
            
            if (importer == null) continue;

            EditorUtility.DisplayProgressBar("Compressing", $"{tex.name}...", (float)i / textures.Count);

            var androidSettings = new TextureImporterPlatformSettings
            {
                name = "Android",
                overridden = true,
                maxTextureSize = androidMaxSize,
                format = TextureImporterFormat.ASTC_6x6,
                compressionQuality = 100
            };
            importer.SetPlatformTextureSettings(androidSettings);

            var standaloneSettings = new TextureImporterPlatformSettings
            {
                name = "Standalone",
                overridden = true,
                maxTextureSize = pcMaxSize,
                format = TextureImporterFormat.DXT5,
                compressionQuality = 100
            };
            importer.SetPlatformTextureSettings(standaloneSettings);

            importer.SaveAndReimport();
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayProgressBar("Calculating", "Calculating space saved...", 0.5f);
        ScanTextures();
        
        float totalAfter = 0;
        foreach (var tex in textures) totalAfter += tex.currentWeightMB;
        
        float savedMB = totalBefore - totalAfter;
        float savedPercent = totalBefore > 0 ? (savedMB / totalBefore) * 100 : 0;

        float estimatedAndroidSize = CalculateEstimatedSize(androidMaxSize);
        float estimatedPCSize = CalculateEstimatedSize(pcMaxSize);
        
        EditorUtility.ClearProgressBar();

        string message = $"✅ Compression Complete\n\n" +
            $"Textures processed: {compressedCount}\n" +
            $"Textures skipped: {skippedCount}\n\n" +
            $"Original weight: {totalBefore:F2} MB\n" +
            $"After compression: {totalAfter:F2} MB\n" +
            $"Space saved: {savedMB:F2} MB ({savedPercent:F1}%)\n\n" +
            $"📱 Android ({androidMaxSize}px): ~{estimatedAndroidSize:F2} MB\n" +
            $"🖥️ PC ({pcMaxSize}px): ~{estimatedPCSize:F2} MB";

        EditorUtility.DisplayDialog("✅ Compression Complete", message, "OK");
    }

    private float CalculateEstimatedSize(int maxSize)
    {
        float totalEstimated = 0;
        
        foreach (var tex in textures)
        {
            float scale = (float)maxSize / tex.currentSize;
            if (scale > 1) scale = 1;

            float newPixelArea = (tex.currentSize * scale) * (tex.currentSize * scale);
            float originalPixelArea = tex.currentSize * tex.currentSize;
            
            float scaleFactor = newPixelArea / originalPixelArea;
            float estimatedWeight = tex.currentWeightMB * scaleFactor;
            
            totalEstimated += estimatedWeight;
        }
        
        return totalEstimated;
    }

    private class TextureInfo
    {
        public string path;
        public string name;
        public int currentSize;
        public float currentWeightMB;
        public bool isCompressed;
    }
}

#pragma warning restore CS0618
