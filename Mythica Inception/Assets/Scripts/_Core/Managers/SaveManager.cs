using _Core.Managers;
using Assets.Scripts._Core.Player;
using MyBox;
using ToolBox.Serialization;
using UnityEngine;

namespace Assets.Scripts._Core.Managers
{
    
    public class SaveManager : MonoBehaviour
    {
        public string playerSaveKey = "playerData";
        public int defaultPlayerHealth = 20;
        public int profileIndex = 0;
        public float saveSeconds = 3f;
        [HideInInspector] public bool activated = true;
        [ReadOnly] public string optionsSaveKey = "SaveOptions";
        public void SavePlayerData(PlayerSaveData savedData)
        {
            DataSerializer.SaveToProfileIndex(profileIndex, playerSaveKey, savedData);
        }

        public bool VerifySaving()
        {
            if (!GameManager.instance.saveManager.activated) return false;
            return !GameManager.instance.player.SamePositionFromSaved();
        }

        public void SaveOtherData<T>(string saveKey, T objectToSave)
        {
            DataSerializer.SaveToProfileIndex(profileIndex, saveKey, objectToSave);
        }

        public bool LoadDataObject<T>(string saveKey, out T loadedObject)
        {
            return DataSerializer.TryLoadProfile(profileIndex, saveKey, out loadedObject);
        }
    }
}
