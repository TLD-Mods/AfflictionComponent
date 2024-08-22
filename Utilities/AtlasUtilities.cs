namespace AfflictionComponent.Utilities;

internal static class AtlasUtilities
{
    // TODO: Somehow figure out a way to dynamically space the images in the atlas if multiple are added.
    internal static void AddCustomSpriteToAtlas(string spriteName)
    {
        foreach (var uiSpriteData in Mod.customAtlas.spriteList)
        {
            if (uiSpriteData.name == spriteName) return;
        }
     
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
        
        Mod.customAtlas.spriteList.Add(spriteData);
        Mod.customAtlas.material = new Material(Shader.Find("Unlit/Transparent Colored"))
        {
            mainTexture = customImage
        };
    }
}