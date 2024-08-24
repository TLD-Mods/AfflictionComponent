namespace AfflictionComponent.Utilities;

internal static class AtlasUtilities
{
    internal static void AddCustomSpriteToAtlas(string spriteName)
    {
        for (var i = 0; i < Mod.allCustomAtlas.transform.childCount; i++)
        {
            if ($"CustomAtlas{spriteName}(Clone)" == Mod.allCustomAtlas.transform.GetChild(i).name) return;
        }
        
        GameObject customAtlas = new() { name = $"CustomAtlas{spriteName}", layer = vp_Layer.Default };
        UnityEngine.Object.Instantiate(customAtlas, Mod.allCustomAtlas.transform);
        var customUIAtlas = customAtlas.AddComponent<UIAtlas>(); // TODO: This isn't adding the component to the game object for some reason?
        
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
}