using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfflictionComponent.Afflictions
{
    [RegisterTypeInIl2Cpp]
    internal class AfflictionManager : MonoBehaviour
    {

        public List<CustomAffliction> m_Afflictions = new List<CustomAffliction>();

        public void Start()
        {

        }

        public void Update()
        {
            //where the magic happens

            float hoursPlayedNotPaused = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();

            for (int num = m_Afflictions.Count - 1; num >= 0; num--)
            {
                CustomAffliction affliction = m_Afflictions[num];

                affliction.OnUpdate();

                if (hoursPlayedNotPaused > affliction.m_EndTime)
                {
                    affliction.Cure();
                    InterfaceManager.GetPanel<Panel_FirstAid>().UpdateDueToAfflictionHealed();
                }
            }
        }

        //so mod authors can check if the player has at least one CustomAffliction of their own type
        public bool HasAfflictionOfType(Type typeName)
        {
            return m_Afflictions.Any(obj => typeName.IsAssignableFrom(obj.GetType()));
        }

        public int GetCustomAfflictionCount()
        {
            return m_Afflictions.Count();
        }

    }
}
