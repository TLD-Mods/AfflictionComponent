using ComplexLogger;

namespace AfflictionComponent.Utilities;

public static class ImageUtilities
{
    /// <summary>
    /// Loads and converts an embedded resource image
    /// </summary>
    /// <param name="resourceName">The full name of the embedded resource</param>
    /// <returns>The image if all related functions work, otherwise null</returns>
    public static Texture2D? GetImage(string resourceName)
    {
        byte[]? resourceData = null;
        Mod.Logger.Log("GetImage", FlaggedLoggingLevel.Debug, LoggingSubType.IntraSeparator);

        try
        {
            Assembly? assembly = Assembly.GetExecutingAssembly();
            using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    Mod.Logger.Log($"The embedded resource was not found: {resourceName}", FlaggedLoggingLevel.Error);
                    return null;
                }

                resourceData = new byte[stream.Length];
                stream.Read(resourceData, 0, (int)stream.Length);
            }
        }
        catch (Exception e)
        {
            Mod.Logger.Log($"Attempting to load embedded resource failed", FlaggedLoggingLevel.Exception, e);
            return null;
        }

        if (resourceData == null)
        {
            Mod.Logger.Log($"Failed to read embedded resource data: {resourceName}", FlaggedLoggingLevel.Warning);
            return null;
        }

        Texture2D texture = new(4096, 4096, TextureFormat.RGBA32, false) { name = Path.GetFileNameWithoutExtension(resourceName) };

        if (ImageConversion.LoadImage(texture, resourceData))
        {
            Mod.Logger.Log($"Successfully loaded embedded resource: {resourceName}", FlaggedLoggingLevel.Debug);
            texture.DontUnload();
            return texture;
        }

        texture.LoadRawTextureData(resourceData);
        texture.Apply();
        texture.DontUnload();
        Mod.Logger.Log($"Successfully loaded embedded resource: {resourceName}", FlaggedLoggingLevel.Debug);
        Mod.Logger.Log(FlaggedLoggingLevel.Debug, LoggingSubType.Separator);
        return texture;
    }
}

public static class Extensions
{
	//https://github.dev/NuclearPowered/Reactor/blob/6eb0bf19c30733b78532dada41db068b2b247742/Reactor/Utilities/DefaultBundle.cs#L17#L40
	/// <summary>
	/// Stops <paramref name="obj"/> from being unloaded.
	/// </summary>
	/// <param name="obj">The object to stop from being unloaded.</param>
	/// <typeparam name="T">The type of the object.</typeparam>
	/// <returns>Passed <paramref name="obj"/>.</returns>
	public static T DontUnload<T>(this T obj) where T : UnityEngine.Object
	{
		obj.hideFlags |= HideFlags.DontUnloadUnusedAsset;

		return obj;
	}
}