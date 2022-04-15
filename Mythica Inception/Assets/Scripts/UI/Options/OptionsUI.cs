using System;
using _Core.Managers;
using Assets.Scripts._Core.Player;
using Codice.Client.BaseCommands.Import;
using MyBox;
using SoundSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    [Foldout("Settings", true)]
    public Toggle autoSave;
    public TMP_Dropdown difficulty;
    public Toggle showConsole;
    public Slider masterVolume;
    public Slider sfxVolume;
    public Slider bgMusicVolume;
    public Slider ambienceVolume;

    public void ChangeUIValues()
    {
        if(GameManager.instance.loadedSaveData == null) return;
        var optionsSaveData = GameManager.instance.loadedSaveData.optionsSaveData;
        autoSave.isOn = optionsSaveData.autoSave;
        difficulty.value = (int) optionsSaveData.difficulty;
        showConsole.isOn = optionsSaveData.showConsole;
        masterVolume.value = optionsSaveData.masterVolume;
        sfxVolume.value = optionsSaveData.sfxVolume;
        bgMusicVolume.value = optionsSaveData.bgMusicVolume;
        ambienceVolume.value = optionsSaveData.ambienceVolume;
    }

    public void ChangeMaster()
    {
        GameManager.instance.audioManager.ChangeVolume(masterVolume.value, SoundType.All);
    }

    public void ChangeSFX()
    {
        GameManager.instance.audioManager.ChangeVolume(sfxVolume.value, SoundType.SFX);
    }

    public void ChangeBackground()
    {
        GameManager.instance.audioManager.ChangeVolume(bgMusicVolume.value, SoundType.Background);
    }

    public void ChangeAmbience()
    {
        GameManager.instance.audioManager.ChangeVolume(ambienceVolume.value, SoundType.Ambience);
    }

    public OptionsSaveData GetCurrentOptionsData()
    {
        var difficultyValue = (OptionsSaveData.DifficultyOptions)difficulty.value;
        return new OptionsSaveData
            (autoSave.isOn,
            difficultyValue, 
            showConsole.isOn, 
            masterVolume.value, 
            sfxVolume.value,
            bgMusicVolume.value, 
            ambienceVolume.value);
    }

    public void EnterGameplay()
    {
        GameManager.instance.inputHandler.EnterGameplay();
    }

    public void EnterOptions()
    {
        GameManager.instance.inputHandler.EnterOptions();
    }
}
