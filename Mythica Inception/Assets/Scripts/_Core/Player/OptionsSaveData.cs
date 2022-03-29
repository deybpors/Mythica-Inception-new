using UnityEngine;

namespace Assets.Scripts._Core.Player
{
    public class OptionsSaveData
    {
        public enum DifficultyOptions
        {
            Dynamic,
            Easy,
            Medium,
            Hard
        }

        [SerializeField] private bool _autoSave;
        [SerializeField] private DifficultyOptions _difficulty;
        [SerializeField] private bool _showConsole;
        [SerializeField] private float _masterVolume;
        [SerializeField] private float _sfxVolume;
        [SerializeField] private float _bgMusicVolume;
        [SerializeField] private float _ambienceVolume;

        public bool autoSave => _autoSave;
        public DifficultyOptions difficulty => _difficulty;
        public bool showConsole => _showConsole;
        public float masterVolume => _masterVolume;
        public float sfxVolume => _sfxVolume;
        public float bgMusicVolume => _bgMusicVolume;
        public float ambienceVolume => _ambienceVolume;

        public OptionsSaveData(bool autoSave, DifficultyOptions difficulty, bool showConsole, float masterVolume, float sfxVolume, float bgMusicVolume, float ambienceVolume)
        {
            _autoSave = autoSave;
            _difficulty = difficulty;
            _showConsole = showConsole;
            _masterVolume = masterVolume;
            _sfxVolume = sfxVolume;
            _bgMusicVolume = bgMusicVolume;
            _ambienceVolume = ambienceVolume;
        }
    }
}
