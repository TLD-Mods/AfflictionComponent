// ---------------------------------------------
// Archive - by The Illusion
// ---------------------------------------------
// Reusage Rights ------------------------------
// You are free to use this script or portions of it in your own mods, provided you give me credit in your description and maintain this section of comments in any released source code
//
// Warning !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// Ensure you change the namespace to whatever namespace your mod uses, so it doesnt conflict with other mods
// ---------------------------------------------

using MelonLoader.Utils;
using AfflictionComponent.Utilities.JSON; //CHANGEME

namespace AfflictionComponent.Utilities
{
    /// <summary>
    /// A helper class for mods that make use of ModComponent
    /// <para>This class's intended use is to handle the PEBKAC issue where users will install multiple <c>modcomponent</c> archives instead of just one</para>
    /// <para>Use <see cref="Add(string)"/> to add a new archive to the list of used archives. It is best to keep a list of all archives, including any you are no longer using</para>
    /// <para>Use <see cref="Deactivate(string)"/> to set an archive as no longer used</para>
    /// <para>Use <see cref="Activate(string)"/> if you wish to reverse the above action</para>
    /// <para>Use <see cref="Verify"/> to check if the install is proper</para>
    /// <para>
    /// Use <see cref="GetInstalledArchives"/> to get all installed archives (the list is limited to matching any of the archive names in the list, regardless of if they are active or not)
    /// </para>
    /// <para>
    /// Ensure to use <see cref="Save"/> if you want this list to be persistant without code or accessable by other mods
    /// </para>
    /// </summary>
    public class Archive
    {
#nullable disable
        /// <summary>
        /// A list of archives this mod has ever used. Set to false if the mod no longer uses this archives name. DO NOT REMOVE OLD NAMES!
        /// </summary>
        private Dictionary<string, bool> Archives { get; set; }
        /// <summary>
        /// The fully qualified name of the config json file for the archives
        /// </summary>
        /// <value>The value should contain the full path, not a relative path</value>
        private string ArchiveConfigPath { get; set; }
#nullable enable

        /// <summary>
        /// This constructor initializes the new Archive with a default path
        /// </summary>
        public Archive() : this(Path.Combine(MelonEnvironment.ModsDirectory, Assembly.GetExecutingAssembly().GetName().Name + ".json")) { }

        /// <summary>
        /// This constructor initializes a new Archive with <paramref name="archiveConfigPath"/> instead of a default path
        /// </summary>
        /// <param name="archiveConfigPath">
        /// The absolute path to the archive. Use <see cref="Path.Combine(string[])"/> and <see cref="MelonEnvironment.ModsDirectory"/> to better get the actual mods directory
        /// </param>
        public Archive(string archiveConfigPath)
        {
            ArchiveConfigPath = archiveConfigPath;
            Load();
            Archives ??= new();
        }

        /// <summary>
        /// Used to load the json file containing the archive list. <see cref="JsonFile.Load{T}(string, bool, System.Text.Json.JsonSerializerOptions?)"/>
        /// </summary>
        public void Load()
        {
            Archives = JsonFile.Load<Dictionary<string, bool>>(ArchiveConfigPath); //CHANGEME
        }

        /// <summary>
        /// Used to save the json file containing the archive list. <see cref="JsonFile.Save{T}(string, T, System.Text.Json.JsonSerializerOptions?)"/>
        /// </summary>
        public void Save()
        {
            JsonFile.Save<Dictionary<string, bool>>(ArchiveConfigPath, Archives); //CHANGEME
        }

        /// <summary>
        /// Sets the given archive as inactive
        /// </summary>
        /// <param name="archive">The name of the archive to deactivate</param>
        public void Deactivate(string archive)
        {
            Archives[archive] = false;
        }

        /// <summary>
        /// Sets the given archive as active
        /// </summary>
        /// <param name="archive">The name of the archive to activate</param>
        public void Activate(string archive)
        {
            Archives[archive] = true;
        }

        /// <summary>
        /// Used to add an archive to the dictionary. Uses the value of true
        /// </summary>
        /// <param name="archive">the name of the archive</param>
        public void Add(string archive)
        {
            if (Archives.ContainsKey(archive)) return;

            Archives?.Add(archive, true);
        }

        /// <summary>
        /// Used to add an archive to the dictionary. Uses the value of true
        /// </summary>
        /// <param name="archive">the name of the archive</param>
        /// <param name="active">If the archive is active</param>
        public void Add(string archive, bool active)
        {
            if (Archives.ContainsKey(archive)) return;

            Archives?.Add(archive, active);
        }

        /// <summary>
        /// Copies a dictionary of archives to the base Archives dictionary
        /// </summary>
        /// <param name="archives">The dictionary to copy</param>
        public void Add(Dictionary<string, bool> archives)
        {
            foreach (KeyValuePair<string, bool> archive in archives)
            {
                if (Archives.ContainsKey(archive.Key)) continue;

                Archives.Add(archive.Key, archive.Value);
            }
        }

        /// <summary>
        /// Used to verify if the current install is proper
        /// </summary>
        /// <returns>True if the number of archives is 1 and there are no inactive archives installed, else false</returns>
        public bool Verify()
        {
            return GetNumArchivesPresent() == 1  && GetInActiveArchivesPresent()?.Count == 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetActiveArchives()
        {
            List<string> ActiveArchives = new();

            foreach (KeyValuePair<string, bool> archive in Archives)
            {
                if (archive.Value)
                {
                    ActiveArchives.Add(archive.Key);
                }
            }

            return ActiveArchives;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetInActiveArchives()
        {
            List<string> InActiveArchives = new();

            foreach (KeyValuePair<string, bool> archive in Archives)
            {
                if (!archive.Value)
                {
                    InActiveArchives.Add(archive.Key);
                }
            }

            return InActiveArchives;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetInstalledModComponents()
        {
            return Directory.GetFiles(MelonEnvironment.ModsDirectory, "*.modcomponent").ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetInstalledArchives()
        {
            List<string> temp = GetInstalledModComponents();
            List<string> InstalledArchives = new();

            if (temp.Count > 0)
            {
                foreach (var mod in temp)
                {
                    if (Archives.ContainsKey(mod)) InstalledArchives.Add(mod);
                }
            }

            return InstalledArchives;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetInstalledActiveArchives()
        {
            List<string> temp               = GetInstalledModComponents();
            List<string> ActiveArchives     = GetActiveArchives();
            List<string> InstalledArchives  = new();

            if (temp.Count > 0)
            {
                foreach (var mod in temp)
                {
                    if (ActiveArchives.Contains(mod))
                    {
                        InstalledArchives.Add(mod);
                    }
                }
            }

            return InstalledArchives;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetNumArchivesPresent()
        {
            List<string> PresentModComponents = GetInstalledArchives();
            List<string> ActiveArchives = GetActiveArchives();
            int archives = 0;

            if (ActiveArchives.Count > 0)
            {
                foreach (string mod in PresentModComponents)
                {
                    if (ActiveArchives.Contains(mod)) archives++;
                }
            }

            return archives;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string>? GetInActiveArchivesPresent()
        {
            List<string> InActiveArchivesInstalled = new();

            foreach (var archive in GetInstalledModComponents())
            {
                if (GetInActiveArchives().Contains(archive)) InActiveArchivesInstalled.Add(archive);
            }

            return InActiveArchivesInstalled;
        }
    }
}
