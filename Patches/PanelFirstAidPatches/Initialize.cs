using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2Cpp;

namespace AfflictionComponent.Patches.PanelFirstAidPatches
{
    internal class Initialize
    {

        [HarmonyPatch(typeof(Panel_FirstAid), nameof(Panel_FirstAid.Initialize))]

        public class AddCustomRightPageLabelObject : MonoBehaviour
        {

            public static void Postfix(Panel_FirstAid __instance)
            {
                GameObject restWidgetTemplate = __instance.m_RightPageObject.transform.GetChild(7).gameObject;
                GameObject restNeededTemplate = restWidgetTemplate.transform.GetChild(1).gameObject; //template to build from
                GameObject customObjInstance = Instantiate(restNeededTemplate, __instance.m_RightPageObject.transform.parent);
                customObjInstance.name = "AFFLICTION_COMPONENT_CUSTOM_RIGHT_PANEL_OBJECT";
                Vector3 position = restNeededTemplate.transform.position;
                position.y -= 0.3f; //move it down by a bit
                customObjInstance.transform.position = position;

                customObjInstance.SetActive(false);

                GameObject centerContents = customObjInstance.transform.GetChild(2).GetChild(0).GetChild(0).gameObject;

                GameObject.Destroy(centerContents.transform.GetChild(2).gameObject);
                GameObject.Destroy(centerContents.transform.GetChild(3).gameObject);
                GameObject.Destroy(centerContents.transform.GetChild(4).gameObject);

                Mod.customRightSidePanelObject = customObjInstance;
            }
        }
    }
}
