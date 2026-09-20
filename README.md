# 🎯 Texture Compression Tool for Unity

A powerful Unity Editor tool that automatically compresses textures for different platforms with customizable settings. Perfect for optimizing your project for mobile and PC builds.

## ✨ Features

- 📱 **Multi-platform compression** - Separate settings for Android and PC
- 🔍 **Smart texture scanning** - Find textures in specific GameObjects or entire project
- 🚫 **Skip already compressed** - Won't overwrite existing compression unless forced
- ⚡ **Real-time compression** - See space savings before and after
- 📊 **Detailed statistics** - Shows which textures were processed and skipped
- 🎮 **UI component support** - Works with Image, RawImage, Sprites, and Meshes
- 💾 **Space calculation** - Estimates storage usage per platform

## 🚀 Installation

1. Download the `TextureCompressionTool.unitypackage`
2. Import it into your Unity project (drag and drop or Assets > Import Package)
3. The tool will be installed in `Assets/Editor/TextureCompressionTool/`

## 📖 How to Use

### Opening the Tool

Go to **Window → Texture Compressor** in the Unity Editor menu bar.

<img width="327" height="405" alt="image" src="https://github.com/user-attachments/assets/bb7ede6f-3b44-4df6-aee0-543a40ac9965" />
<img width="763" height="786" alt="image" src="https://github.com/user-attachments/assets/b4ba0862-6ad8-4c24-b790-1d41fa73168e" />

### Step-by-Step Guide

#### 1. **Configure Compression Settings**
   - Click on "Compression Settings" to expand the options
   - Set **Android Max Size**: Choose from 256px, 512px, or 1024px (or enter custom value)
   - Set **PC Max Size**: Choose from 1024px, 2048px, or 4096px (or enter custom value)
   - Toggle **Force Compress** if you want to override existing compression settings

#### 2. **Select Search Mode**
   - **Selection**: Compress textures from a specific GameObject and all its children
     - Select a GameObject in the Hierarchy
     - Assign it in the tool's GameObject field
   - **All Project**: Compress all textures in your entire project

#### 3. **Scan for Textures**
   - Click **🔍 SCAN GAMEOBJECT** or **🔍 SCAN TEXTURES** depending on your selection
   - Wait for the scan to complete
   - Review the found textures in the list

#### 4. **Review Textures**
   - Each texture shows:
     - Resolution (e.g., 1024px)
     - Current file weight (MB)
     - File path in your project
     - Compression status (Already Compressed ✓ or Uncompressed)

#### 5. **Compress**
   - Click **⚡ COMPRESS NOW**
   - Confirm the compression in the dialog
   - Wait for the process to complete
   - View the results showing space saved

## ⚙️ Configuration Options

### Android Settings
- **256px** - Minimal quality, smallest file size (UI textures, icons)
- **512px** - Recommended for mobile (textures, sprites)
- **1024px** - High quality, larger file size

### PC Settings
- **1024px** - Standard quality for desktop
- **2048px** - High quality
- **4096px** - Ultra-high quality

### Force Compress Toggle
- **OFF** (default) - Only compresses uncompressed textures
- **ON** - Overwrites all existing compression settings (⚠️ Use with caution)

## 📊 Understanding the Results

After compression, you'll see:

```
✅ Compression Complete

Textures processed: 15
Textures skipped: 3

Original weight: 125.45 MB
After compression: 32.87 MB
Space saved: 92.58 MB (73.8%)

📱 Android (512px): ~8.23 MB
🖥️ PC (1024px): ~31.45 MB
```

- **Textures processed**: Number of textures that were compressed
- **Textures skipped**: Already compressed textures (when Force Compress is OFF)
- **Space saved**: Reduction in file size with percentage
- **Platform estimates**: Approximate build size per platform

## 🎮 Supported Components

The tool scans and compresses textures from:
- ✓ MeshRenderer (3D models)
- ✓ SkinnedMeshRenderer (Rigged models, avatars)
- ✓ SpriteRenderer (2D sprites)
- ✓ Image (UI elements)
- ✓ RawImage (Raw textures in UI)
- ✓ Materials with multiple texture properties

## 💡 Pro Tips

1. **Backup First** - Always backup your project before bulk compression
2. **Test on Device** - Test compressed assets on actual target devices
3. **Use Selection Mode** - Start with specific GameObjects to test compression
4. **Check Console** - Any warnings or errors will appear in the Console
5. **Platform Specific** - Compression is applied per-platform, not universal
6. **Custom Sizes** - You can enter any texture size, not just the presets

## 🔧 Advanced Usage

### VRChat/Custom Shaders
The tool automatically handles complex shaders with many properties. It will:
- Skip properties that don't exist
- Silently ignore missing texture references
- Continue processing all valid textures

### Large Projects
For very large projects:
1. Use **Selection Mode** to compress assets in batches
2. This prevents timeouts and makes issues easier to spot
3. Monitor the Console during processing

## 📦 What Gets Compressed

The tool looks for these common texture properties:
- `_MainTex` - Primary color texture
- `_NormalMap` / `_BumpMap` - Normal maps
- `_MetallicGlossMap` - Metallic/glossiness data
- `_OcclusionMap` - Ambient occlusion
- `_EmissionMap` - Self-illuminated areas
- And many more...

## ⚠️ Important Notes

- **Non-destructive**: Original texture files aren't deleted, only import settings changed
- **Reimport required**: Textures are automatically reimported after compression
- **Platform-specific**: Android and PC can have different compression formats
- **Reversible**: You can change settings back anytime in Texture Import settings

## 🐛 Troubleshooting

### No textures found
- Make sure you selected a GameObject that actually has renderers
- Check that textures exist in your project (not in Packages folder)
- Try the "All Project" mode to ensure textures are being detected

### Compression doesn't look right
- Verify the max size isn't too small for your texture resolution
- Check the Texture Import settings for any overrides
- Test with "Force Compress" OFF first

### Console errors about missing properties
- These are normal for custom shaders (VRChat, etc.)
- The tool ignores them automatically
- All valid textures will still be processed

## 📝 License

This tool is provided as-is for Unity project optimization.

## 🙌 Version

**Texture Compression Tool v1.0.0**
- Compatible with Unity 2019.4+
- Tested on Unity 2022 LTS

---

**Made with ❤️ for optimizing your Unity projects**

Questions or issues? Check the Console for detailed messages during operation.
