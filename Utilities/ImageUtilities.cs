using ComplexLogger;

namespace AfflictionComponent.Utilities;

public class ImageUtilities
{
    /// <summary>
	/// Loads and converts a raw image
	/// </summary>
	/// <param name="FolderName">The name of the folder, without parents eg: "TEMPLATE". See: <see cref="MelonLoader.Utils.MelonEnvironment.ModsDirectory"/></param>
	/// <param name="FileName">The name of the image, without extension or foldername</param>
	/// <param name="ext">The extension of the file eg: "jpg". This is provided to allow extension methods to define this parameter</param>
	/// <returns>The image if all related functions work, otherwise null</returns>
	public static Texture2D? GetImage(string FolderName, string FileName, string ext)
	{
		byte[]? file = null;
		string AbsoluteFileName = Path.Combine(MelonLoader.Utils.MelonEnvironment.ModsDirectory, FolderName, $"{FileName}.{ext}");

		Mod.Logger.Log("GetImage", FlaggedLoggingLevel.Debug, LoggingSubType.IntraSeparator);
		if (!File.Exists(AbsoluteFileName))
		{
			Mod.Logger.Log($"The file requested was not found {AbsoluteFileName}", FlaggedLoggingLevel.Error);
			return null;
		}

		Texture2D texture = new(4096, 4096, TextureFormat.RGBA32, false) { name = FileName };

		try
		{
			file = File.ReadAllBytes(AbsoluteFileName);

			if (file == null)
			{
				Mod.Logger.Log($"Attempting to ReadAllBytes failed for image {FileName}.{ext}", FlaggedLoggingLevel.Warning);
				return null;
			}
		}
		catch (DirectoryNotFoundException dnfe)
		{
			Mod.Logger.Log($"Directory was not found {FolderName}", FlaggedLoggingLevel.Exception, dnfe);
		}
		catch (FileNotFoundException fnfe)
		{
			Mod.Logger.Log($"File was not found {FileName}", FlaggedLoggingLevel.Exception, fnfe);
		}
		catch (Exception e)
		{
			Mod.Logger.Log($"Attempting to load requested file failed", FlaggedLoggingLevel.Exception, e);
		}

		if (ImageConversion.LoadImage(texture, file))
		{
			Mod.Logger.Log($"Successfully loaded file {FileName}", FlaggedLoggingLevel.Debug);
			texture.DontUnload();

			return texture;
		}

		texture.LoadRawTextureData(file);
		texture.Apply();
		texture.DontUnload();
		Mod.Logger.Log($"Successfully loaded file {FileName}", FlaggedLoggingLevel.Debug);
		Mod.Logger.Log(FlaggedLoggingLevel.Debug, LoggingSubType.Separator);
		return texture ?? null;
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