using System.Collections.Generic;
using System.Linq;
using _Core.Managers;
using Assets.Scripts.UI;
using Monster_System;
using MyBox;
using UnityEngine;

public class MythicaTabPage : TabPage
{
    [SerializeField] private MythicaButton[] _mythicaButtons;
    [ReadOnly] public List<Monster> _monsters;
    protected override void OnActive()
    {
        var monstersDiscovered = GameManager.instance.loadedSaveData.discoveredMonsters.Values.OrderBy(m => m.monsterNum).ToList();
        _monsters = monstersDiscovered;

        var buttonCount = _mythicaButtons.Length;
        var discoveredCount = monstersDiscovered.Count;

        for (var i = 0; i < buttonCount; i++)
        {
            _mythicaButtons[i].ChangeToBlank();
            for (var j = 0; j < discoveredCount; j++)
            {
                if (monstersDiscovered[j].monsterNum - 1 == i)
                {
                    _mythicaButtons[i].InitializeMonsterButton(monstersDiscovered[j]);
                }
            }
        }
        _mythicaButtons[0].ChangeInfoToBlank();
    }
}
