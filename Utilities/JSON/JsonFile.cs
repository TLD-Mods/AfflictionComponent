// ---------------------------------------------
// JsonFile - by The Illusion
// ---------------------------------------------
// Reusage Rights ------------------------------
// You are free to use this script or portions of it in your own mods, provided you give me credit in your description and maintain this section of comments in any released source code
//
// Warning !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// Ensure you change the namespace to whatever namespace your mod uses, so it doesnt conflict with other mods
// ---------------------------------------------

using System.Text.Json;

namespace AfflictionComponent.Utilities.JSON
{
    public class JsonFile
    {
        public static JsonSerializerOptions DefaultOptions { get; } = new JsonSerializerOptions()
        {
            WriteIndented = true, // pretty print
            IncludeFields = true // use [JsonInclude] on properties you want to include, otherwise it wont be
        };

        #region Syncronous
        public static void Save<T>(string configFileName, T Tinput, JsonSerializerOptions? options = null)
        {
            try
            {
                options ??= DefaultOptions;
                using FileStream file = File.Open(configFileName, FileMode.Create, FileAccess.Write, FileShare.None);
                JsonSerializer.Serialize<T>(file, Tinput, options);
                file.Dispose();
            }
            catch
            {
                throw;
            }
        }

        public static T? Load<T>(string configFileName, JsonSerializerOptions? options = null)
        {
            try
            {
                options ??= DefaultOptions;
                using FileStream file = File.Open(configFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                var output = JsonSerializer.Deserialize<T>(file, options);
                file.Dispose();
                return output;
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region Async
        /// <summary>
        /// Loads a given JSON file
        /// </summary>
        /// <typeparam name="T">The class to deserialize</typeparam>
        /// <param name="configFileName">absolute path to the file</param>
        /// <returns>new class based on file contents</returns>
        public static async Task<T?> LoadAsync<T>(string configFileName, JsonSerializerOptions? options = null)
        {
            try
            {
                options ??= DefaultOptions;
                await using FileStream file = File.Open(configFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                var output = await JsonSerializer.DeserializeAsync<T>(file, options);
                await file.DisposeAsync();
                return output;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Saves a new JSON file
        /// </summary>
        /// <typeparam name="T">The class to serialize</typeparam>
        /// <param name="configFileName">absolute path to the file</param>
        /// <param name="Tinput">an instance of the given class with information filled</param>
        public static async Task SaveAsync<T>(string configFileName, T Tinput, JsonSerializerOptions? options = null)
        {
            try
            {
                options ??= DefaultOptions;
                await using FileStream file = File.Open(configFileName, FileMode.Create, FileAccess.Write, FileShare.None);
                await JsonSerializer.SerializeAsync<T>(file, Tinput, options);
                await file.DisposeAsync();
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
