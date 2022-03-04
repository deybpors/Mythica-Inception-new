using UnityEngine;

namespace _Core.Others
{
    public enum Sex
    {
        Male,
        Female
    }
    [CreateAssetMenu(menuName = "Miscellaneous/Sex")]
    public class TamerSexGFX : ScriptableObject
    {
        public GameObject tamerMale;
        public GameObject tamerFemale;

        public GameObject GetTamerGFX(Sex sex)
        {
            if (sex == Sex.Male)
            {
                return tamerMale;
            }

            return tamerFemale;
        }
    }
}