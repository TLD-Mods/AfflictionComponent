// ---------------------------------------------
// ConversionUtilities - by The Illusion
// ---------------------------------------------
// Reusage Rights ------------------------------
// You are free to use this script or portions of it in your own mods, provided you give me credit in your description and maintain this section of comments in any released source code
//
// Warning !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// Ensure you change the namespace to whatever namespace your mod uses, so it doesnt conflict with other mods
// ---------------------------------------------

namespace AfflictionComponent.Utilities
{
    public class ConversionUtilities
    {
        /// <summary>
        /// Converts MPH to KM/H
        /// </summary>
        /// <param name="mph">The MPH to convert</param>
        /// <returns></returns>
		public static float ConvertMilesKilometerHour(float mph)
		{
            return mph * 1.609344f;
        }

        /// <summary>
        /// Converts MPH to M/S
        /// </summary>
        /// <param name="mph">The MPH to convert</param>
        /// <returns></returns>
		public static float ConvertMilesMetersSecond(float mph)
		{
			return mph * 0.44704f;
        }

        /// <summary>
        /// Rounds the input up to the nearest int
        /// </summary>
        /// <param name="input">The float to convert</param>
        /// <returns></returns>
        public static int GetNormalizedSpeed(float input)
        {
            float num = Mathf.Ceil(input);
            return (int)num;
        }
    }
}
