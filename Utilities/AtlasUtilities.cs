namespace AfflictionComponent.Utilities;

internal static class AtlasUtilities
{
    private static UIAtlas? baseCoverflowAtlas;

    internal static void AddCustomSpriteToAtlas(string spriteName)
    {
        for (var i = 0; i < Mod.allCustomAtlas.transform.childCount; i++)
        {
            if ($"CustomAtlas{spriteName}(Clone)" == Mod.allCustomAtlas.transform.GetChild(i).name) return;
        }
        
        GameObject customAtlas = new() { name = $"CustomAtlas{spriteName}", layer = vp_Layer.Default };
        GameObject iCustomAtlas = UnityEngine.Object.Instantiate(customAtlas, Mod.allCustomAtlas.transform);
        var customUIAtlas = iCustomAtlas.AddComponent<UIAtlas>();

        var customImage = ImageUtilities.GetImage(spriteName);
        if (customImage == null) return;
        
        var spriteData = new UISpriteData
        {
            name = spriteName,
            x = 0,
            y = 0,
            width = customImage.width,
            height = customImage.height
        };

        customUIAtlas.spriteList.Add(spriteData);
        customUIAtlas.material = new Material(Shader.Find("Unlit/Transparent Colored"))
        {
            mainTexture = customImage
        };
    }

    internal static bool IsCustomAtlas(UIAtlas? atlas)
    {
        if (atlas == null || Mod.allCustomAtlas == null) return false;

        return atlas.transform.parent == Mod.allCustomAtlas.transform;
    }

    /// <summary>
    /// The atlas an AfflictionCoverflow icon ships on. The affliction scroll list pools its objects, so a slot
    /// keeps whatever atlas it was last given, and one left on a single sprite custom atlas renders blank as soon
    /// as vanilla reuses it and sets only a sprite name. Resolved once and cached.
    /// </summary>
    internal static UIAtlas? GetBaseCoverflowAtlas(Panel_Affliction panel)
    {
        if (baseCoverflowAtlas != null) return baseCoverflowAtlas;
        if (panel == null || panel.m_ScrollList == null) return null;

        // The pool's template is never handed out to a slot, so its atlas is still the one the prefab shipped with.
        var prefabObject = panel.m_ScrollList.m_PrefabObject;
        if (prefabObject != null)
        {
            var template = prefabObject.GetComponentInChildren<AfflictionCoverflow>(true);
            if (template != null && template.m_SpriteEffect != null && !IsCustomAtlas(template.m_SpriteEffect.atlas))
            {
                baseCoverflowAtlas = template.m_SpriteEffect.atlas;
                return baseCoverflowAtlas;
            }
        }

        // Otherwise take it from any live slot that has not been moved onto a custom atlas.
        for (var i = 0; i < panel.m_ScrollList.m_ScrollObjects.Count; i++)
        {
            var coverflow = Utils.GetComponentInChildren<AfflictionCoverflow>(panel.m_ScrollList.m_ScrollObjects[i]);
            if (coverflow == null || coverflow.m_SpriteEffect == null) continue;
            if (IsCustomAtlas(coverflow.m_SpriteEffect.atlas)) continue;

            baseCoverflowAtlas = coverflow.m_SpriteEffect.atlas;
            return baseCoverflowAtlas;
        }

        return null;
    }

    internal static void ResetToBaseAtlas(UISprite? sprite, UIAtlas? baseAtlas)
    {
        if (sprite == null || baseAtlas == null) return;
        if (sprite.atlas == baseAtlas) return;

        sprite.atlas = baseAtlas;
    }
}