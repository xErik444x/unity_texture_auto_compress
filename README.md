# 🎯 Texture Compression Tool for Unity

A Unity Editor tool that compresses and downscales textures per platform (Android and PC) in one click. Scan a GameObject or the whole project, review what will change, and apply.

<img width="327" height="405" alt="Texture Compression Tool window" src="https://github.com/user-attachments/assets/bb7ede6f-3b44-4df6-aee0-543a40ac9965" />
<img width="763" height="786" alt="Texture list and results" src="https://github.com/user-attachments/assets/b4ba0862-6ad8-4c24-b790-1d41fa73168e" />

## ✨ Features

- 📱 **Per-platform settings**: separate max sizes for Android and PC
- 🔍 **Flexible scanning**: a GameObject and its children, or the entire project
- 🧠 **Smart Force mode**: recompresses oversized textures, leaves the rest untouched
- 👀 **Live preview**: every texture shows what will happen to it before you compress
- 📊 **Clear report**: textures processed vs. skipped, space saved, per-platform estimates
- 🎮 **Wide component support**: MeshRenderer, SkinnedMeshRenderer, SpriteRenderer, Image, RawImage
- 🧩 **Complex shader friendly**: works with shaders that have hundreds of properties (e.g. VRChat Toon Standard) without console errors

## 🚀 Installation

1. Import `TextureCompressionTool.unitypackage` (drag and drop, or **Assets → Import Package → Custom Package**)
2. The tool is installed in `Assets/Editor/TextureCompressionTool/`

Or copy `TextureCompressionTool.cs` into any `Editor` folder.

Requires **Unity 2019.4+** (tested on Unity 2022 LTS).

## 📖 How to Use

Open **Window → Texture Compression Tool**.

1. **Set max sizes**: Android (256 / 512 / 1024) and PC (1024 / 2048 / 4096), or type a custom value.
2. **Choose the search mode**:
   - **Selection**: assign a GameObject; its children are included.
   - **All Project**: every texture in `Assets` (packages are ignored).
3. **Scan**: click **SCAN GAMEOBJECT** or **SCAN TEXTURES**.
4. **Review the list**: each texture shows its resolution, file size, path, and what will happen to it.
5. **Compress**: click **COMPRESS NOW** and confirm.

## ⚙️ Compression Behavior

| Force Compress | Uncompressed texture | Compressed, **above** max size | Compressed, **at or below** max size |
|---|---|---|---|
| **OFF** (default) | Compressed | Skipped | Skipped |
| **ON** | Compressed | Recompressed to the selected max | Untouched |

- Android and PC are evaluated **independently**. A texture that exceeds the limit only on one platform is only recompressed for that platform.
- "Size" is the smaller of the importer's Max Size and the original file resolution, so a small source image is never treated as oversized.
- Applied settings: **Android** ASTC 6x6, **PC** DXT5, quality 100.

## 📊 Results

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

Platform figures are estimates based on the selected max size.

## 💡 Tips

- **Back up or commit first** before running on a whole project.
- Start with **Selection** mode on one object to check the result.
- Test compressed assets on a real target device.

## ⚠️ Important Notes

- **Non-destructive**: original texture files are never modified, only their import settings.
- Changes can be reverted anytime from each texture's **Import Settings**.
- The scan only reads materials. It does not instantiate or modify anything in your scene.

## 🐛 Troubleshooting

**No textures found**
Make sure the GameObject has renderers and that its textures live in `Assets` (not in Packages). Try **All Project** mode to confirm textures are detected.

**A texture wasn't changed**
With Force OFF, already-compressed textures are skipped. With Force ON, textures at or below the selected max are intentionally left as they are. Check the status line in the list.

**Compression doesn't look right**
Your max size may be too low for that texture. Adjust it and run again with Force ON, or review the texture's Import Settings.

## 📝 Changelog

### v1.0.1
- **Force Compress now downscales oversized textures** that are already compressed; compressed textures within the max size are left untouched.
- Per-platform evaluation and live per-texture status in the list.
- Fixed console errors ("doesn't have a texture property") on complex shaders by reading only real texture properties.
- Fixed a bug where scanning created temporary material instances, which could leave objects with missing materials after saving or building.

### v1.0.0
- Initial release.

## 📄 License

This tool is provided as-is for Unity project optimization.

---

Made with ❤️ for optimizing your Unity projects.
