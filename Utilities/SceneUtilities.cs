// ---------------------------------------------
// ScenenUtilities - by The Illusion
// ---------------------------------------------
// Reusage Rights ------------------------------
// You are free to use this script or portions of it in your own mods, provided you give me credit in your description and maintain this section of comments in any released source code
//
// Warning !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// Ensure you change the namespace to whatever namespace your mod uses, so it doesnt conflict with other mods
// ---------------------------------------------

namespace AfflictionComponent.Utilities
{
    public class SceneUtilities
    {
        /// <summary>
        /// This contains a list of scenes that wont otherwise be caught in the checks
        /// </summary>
        public static List<string> BlacklistedScenes = new()
        {

        };

        /// <summary>
        /// Used to check if the current scene is EMPTY
        /// </summary>
        /// <param name="sceneName">The name of the scene to check, if null will use <c>GameManager.m_ActiveScene</c></param>
        /// <returns></returns>
        public static bool IsSceneEmpty(string? sceneName = null)
        {
            sceneName ??= GameManager.m_ActiveScene;

            if (sceneName != null && sceneName.Contains("Empty", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Used to check if the current scene is BOOT
        /// </summary>
        /// <param name="sceneName">The name of the scene to check, if null will use <c>GameManager.m_ActiveScene</c></param>
        /// <returns></returns>
        public static bool IsSceneBoot(string? sceneName = null)
        {
            sceneName ??= GameManager.m_ActiveScene;

            if (sceneName != null && sceneName.Contains("Boot", StringComparison.InvariantCultureIgnoreCase) )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Used to check if the current scene is a main menu scene
        /// </summary>
        /// <param name="sceneName">The name of the scene to check, if null will use <c>GameManager.m_ActiveScene</c></param>
        /// <returns></returns>
        public static bool IsSceneMenu(string? sceneName = null)
        {
            sceneName ??= GameManager.m_ActiveScene;

            if (sceneName != null && sceneName.StartsWith("MainMenu", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Used to check if the current scene is a playable scene. Use this for most needs
        /// </summary>
        /// <param name="sceneName">The name of the scene to check, if null will use <c>GameManager.m_ActiveScene</c></param>
        /// <returns></returns>
        public static bool IsScenePlayable(string? sceneName = null)
        {
            sceneName ??= GameManager.m_ActiveScene;

            if (sceneName != null && IsSceneEmpty(sceneName) || IsSceneBoot(sceneName) || IsSceneMenu(sceneName))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Used to check if the current scene is a base scene (Zone or Region)
        /// </summary>
        /// <param name="sceneName">The name of the scene to check, if null will use <c>GameManager.m_ActiveScene</c></param>
        /// <returns></returns>
        public static bool IsSceneBase(string? sceneName = null)
        {
            sceneName ??= GameManager.m_ActiveScene;

            // BlacklistedScenes MUST fail first
            if (sceneName != null)
            {
                if (!BlacklistedScenes.Contains(sceneName) && sceneName.Contains("Region", StringComparison.InvariantCultureIgnoreCase) || sceneName.Contains("Zone", StringComparison.InvariantCultureIgnoreCase)) return true;
            }

            return false;
        }

        /// <summary>
        /// Used to check if the current scene is a sandbox scene
        /// </summary>
        /// <param name="sceneName">The name of the scene to check, if null will use <c>GameManager.m_ActiveScene</c></param>
        /// <returns></returns>
        public static bool IsSceneSandbox(string? sceneName = null)
        {
            sceneName ??= GameManager.m_ActiveScene;

            if (sceneName != null && sceneName.Contains("SANDBOX", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Used to check if the current scene is a DLC01 scene
        /// </summary>
        /// <param name="sceneName">The name of the scene to check, if null will use <c>GameManager.m_ActiveScene</c></param>
        /// <returns></returns>
        public static bool IsSceneDLC01(string? sceneName = null)
        {
            sceneName ??= GameManager.m_ActiveScene;

            if (sceneName != null && sceneName.Contains("DLC01", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Used to check if the current scene is a DARKWALKER scene
        /// </summary>
        /// <param name="sceneName">The name of the scene to check, if null will use <c>GameManager.m_ActiveScene</c></param>
        /// <returns></returns>
        public static bool IsSceneDarkWalker(string? sceneName = null)
        {
            sceneName ??= GameManager.m_ActiveScene;

            if (sceneName != null && sceneName.Contains("DARKWALKER", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Used to check if the current scene is an additive scene, like sandbox or DLC scenes added to the base scene
        /// </summary>
        /// <param name="sceneName">The name of the scene to check, if null will use <c>GameManager.m_ActiveScene</c></param>
        /// <returns></returns>
        public static bool IsSceneAdditive(string? sceneName = null)
        {
            sceneName ??= GameManager.m_ActiveScene;

            if (sceneName != null && IsSceneSandbox(sceneName) || IsSceneDLC01(sceneName) || IsSceneDarkWalker(sceneName))
            {
                return false;
            }

            return true;
        }
    }
}
