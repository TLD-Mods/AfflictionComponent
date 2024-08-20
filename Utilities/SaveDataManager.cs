using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModData;
using Il2Cpp;
using AfflictionComponent.Components;
using Newtonsoft.Json;

namespace AfflictionComponent.Utilities
{
    internal class SaveDataManager
    {
        ModDataManager dm = new ModDataManager("AfflictionComponent", true);

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        public void Save(AfflictionManagerSaveDataProxy data)
        {
            Mod.Logger.Log("Saving data", ComplexLogger.FlaggedLoggingLevel.Debug);
            string? dataString;
            dataString = JsonConvert.SerializeObject(data, settings);
            dm.Save(dataString);
        }

        public AfflictionManagerSaveDataProxy Load()
        {
            string? dataString = dm.Load();
            if (dataString == null)
            {
                Mod.Logger.Log("Returning null from data load", ComplexLogger.FlaggedLoggingLevel.Debug);
                return null;
            }
            return JsonConvert.DeserializeObject<AfflictionManagerSaveDataProxy>(dataString, settings);
        }
    }
}
