using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// AssetsLoader hỗ trợ loading từ Resources:
/// - Sprites trong Resources/images
/// - JSON level files (TextAsset) trong Resources/levels
/// Cung cấp API sync và async, caching đơn giản, và unload.
/// </summary>
public class AssetsLoader : MonoBehaviour
{
    // Simple caches to avoid reloading
    private readonly Dictionary<string, UnityEngine.Object> cache = new Dictionary<string, UnityEngine.Object>();
    private const string ImagesFolder = "images";
    private const string LevelsFolder = "levels";
    public void Start()
    {
        StartCoroutine(PreloadAllCoroutine());
    }
    #region Sync API

    /// <summary>Load sprite synchronously from Resources/images/{path}</summary>
    public Sprite LoadSprite(string path)
    {
        string fullPath = GetImagePath(path);
        if (cache.TryGetValue(fullPath, out var cached))
            return cached as Sprite;

        Sprite sprite = Resources.Load<Sprite>(fullPath);
        if (sprite != null) cache[fullPath] = sprite;
        else Debug.LogWarning($"[AssetsLoader] Sprite not found at Resources/{fullPath}");
        return sprite;
    }

    /// <summary>Load level JSON synchronously and parse to T (using JsonUtility)</summary>
    public T LoadLevel<T>(string path) where T : class
    {
        string fullPath = GetLevelPath(path);
        if (cache.TryGetValue(fullPath, out var cached))
        {
            if (cached is TextAsset ta)
                return JsonUtility.FromJson<T>(ta.text);
        }

        TextAsset taLoaded = Resources.Load<TextAsset>(fullPath);
        if (taLoaded == null)
        {
            Debug.LogWarning($"[AssetsLoader] Level JSON not found at Resources/{fullPath}");
            return null;
        }
        cache[fullPath] = taLoaded;
        return JsonUtility.FromJson<T>(taLoaded.text);
    }

    #endregion

    #region Async API (Resources.LoadAsync)

    /// <summary>Load a sprite asynchronously (wraps Resources.LoadAsync). Returns null if not found or cancelled.</summary>
    public async Task<Sprite> LoadSpriteAsync(string path, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        string fullPath = GetImagePath(path);
        if (cache.TryGetValue(fullPath, out var cached))
            return cached as Sprite;

        ResourceRequest req = Resources.LoadAsync<Sprite>(fullPath);
        // Wait until done but respect cancellation
        while (!req.isDone)
        {
            if (ct.IsCancellationRequested)
            {
                // no direct cancel for ResourceRequest -> just return null
                return null;
            }
            await Task.Yield();
        }

        if (req.asset == null)
        {
            Debug.LogWarning($"[AssetsLoader] Async sprite not found at Resources/{fullPath}");
            return null;
        }

        Sprite s = req.asset as Sprite;
        cache[fullPath] = s;
        return s;
    }

    /// <summary>Load a level JSON file asynchronously and parse to T. Returns null if not found or cancelled.</summary>
    public async Task<T> LoadLevelAsync<T>(string path, CancellationToken ct = default) where T : class
    {
        ct.ThrowIfCancellationRequested();
        string fullPath = GetLevelPath(path);

        if (cache.TryGetValue(fullPath, out var cached) && cached is TextAsset ta)
            return JsonUtility.FromJson<T>(ta.text);

        ResourceRequest req = Resources.LoadAsync<TextAsset>(fullPath);
        while (!req.isDone)
        {
            if (ct.IsCancellationRequested) return null;
            await Task.Yield();
        }

        if (req.asset == null)
        {
            Debug.LogWarning($"[AssetsLoader] Async level JSON not found at Resources/{fullPath}");
            return null;
        }

        TextAsset textAsset = req.asset as TextAsset;
        Debug.Log($"[AssetsLoader] Loaded level JSON {fullPath}, size={textAsset.text.Length} chars");
        cache[fullPath] = textAsset;
        return JsonUtility.FromJson<T>(textAsset.text);
    }

    #endregion

    #region Preload / Load all utilities

    /// <summary>
    /// Preload tất cả ảnh (Sprite) nằm trong folder Resources/<folder>.
    /// - Nếu có manifest (Resources/AssetsLoaderResourceManifest) và manifest.images chứa các đường dẫn,
    ///   hàm sẽ load từng file bằng Resources.LoadAsync (không block).
    /// - Nếu không có manifest, hàm fallback sang Resources.LoadAll (synchronous) rồi cache.
    /// onProgress: callback nhận value 0..1
    /// </summary>
    public async Task PreloadImagesInFolderAsync(string folder = ImagesFolder, Action<float> onProgress = null, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(folder)) folder = ImagesFolder;
        folder = folder.Trim('/');
        
        Sprite[] sprites = Resources.LoadAll<Sprite>(folder);
        if (sprites == null || sprites.Length == 0)
        {
            onProgress?.Invoke(1f);
            Debug.Log($"[AssetsLoader] PreloadImagesInFolderAsync: no sprites found in Resources/{folder}");
            return;
        }

        int total2 = sprites.Length;
        int done2 = 0;
        const int batchSize = 20; // xử lý batch rồi yield để giảm hitch do xử lý cache
        for (int i = 0; i < sprites.Length; i++)
        {
            if (ct.IsCancellationRequested) return;
            var sp = sprites[i];
            if (sp != null)
            {
                string key = $"{folder}/{sp.name}";
                cache[key] = sp;
            }
            done2++;

            if (done2 % batchSize == 0)
            {
                onProgress?.Invoke((float)done2 / total2);
                await Task.Yield(); // give control back to main loop for a frame
            }
        }

        onProgress?.Invoke(1f);
        Debug.Log($"[AssetsLoader] PreloadImagesInFolderAsync: loaded {total2} sprites from Resources/{folder} (fallback LoadAll).");
    }

    /// <summary>
    /// Utility: trả về list tên tất cả sprites đã cache (key -> object)
    /// </summary>
    public List<string> GetCachedKeys()
    {
        return new List<string>(cache.Keys);
    }

    #endregion

    #region Utilities & unloading

    private string GetImagePath(string path)
    {
        // Allow passing "enemy" or "images/enemy" — normalize to images/...
        if (path.StartsWith("images/")) return path;
        return $"images/{path}";
    }

    private string GetLevelPath(string path)
    {
        if (path.StartsWith("levels/")) return path;
        return $"levels/{path}";
    }

    /// <summary>Unload a cached asset (Resources.UnloadAsset). If not cached, does nothing.</summary>
    public void Unload(string resourceRelativePath)
    {
        string p = resourceRelativePath;
        if (!p.StartsWith("images/") && !p.StartsWith("levels/"))
        {
            // try both prefixes
            string pi = "images/" + resourceRelativePath;
            string pl = "levels/" + resourceRelativePath;
            if (cache.TryGetValue(pi, out var a))
            {
                Resources.UnloadAsset(a);
                cache.Remove(pi);
                return;
            }
            if (cache.TryGetValue(pl, out var b))
            {
                Resources.UnloadAsset(b);
                cache.Remove(pl);
                return;
            }
            return;
        }

        if (cache.TryGetValue(p, out var asset))
        {
            Resources.UnloadAsset(asset);
            cache.Remove(p);
        }
    }

    /// <summary>Clear all caches. Note: use Resources.UnloadUnusedAssets() to free memory fully.</summary>
    public void ClearCache(bool runUnloadUnusedAssets = false)
    {
        cache.Clear();
        if (runUnloadUnusedAssets)
            Resources.UnloadUnusedAssets();
    }

    #endregion
}
