using System.Collections.Generic;
using _Core.Managers;
using _Core.Others;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionUI : MonoBehaviour
{
    [SerializeField] private Transform _parent;
    [SerializeField] private GameObject _male;
    [SerializeField] private GameObject _female;
    [SerializeField] private MonsterTamedUI _monsterTamedUi;
    public RawImage rawImage;

    private GameObject _thisObj;
    private GameObject _current;
    private Dictionary<string, GameObject> _selected = new Dictionary<string, GameObject>();
    private Dictionary<GameObject, Outline> _outlines = new Dictionary<GameObject, Outline>();
    private GameObject _monsterTamedObj;


    void OnEnable()
    {
        if (_monsterTamedObj == null)
        {
            _monsterTamedObj = _monsterTamedUi.gameObject;
        }

        _monsterTamedUi.enabled = false;
        _monsterTamedObj.SetActive(true);
    }

    void OnDisable()
    {
        if (_monsterTamedObj == null)
        {
            _monsterTamedObj = _monsterTamedUi.gameObject;
        }

        _monsterTamedObj.SetActive(false);
        _monsterTamedUi.enabled = true;
    }

    public void Disable()
    {
        if (_thisObj == null)
        {
            _thisObj = gameObject;
        }

        try
        {
            Destroy(_selected[Sex.Male.ToString()]);
            Destroy(_selected[Sex.Female.ToString()]);
            _selected.Clear();
        }
        catch
        {
            //ignored
        }
        _thisObj.SetActive(false);
    }

    public void Switch(string sex)
    {
        if (_thisObj == null)
        {
            _thisObj = gameObject;
        }

        _thisObj.SetActive(true);

        switch (sex)
        {
            case "Male":
                if (!_selected.TryGetValue(Sex.Male.ToString(), out var maleObj))
                {
                    maleObj = Instantiate(_male, _parent);
                    _selected.Add(Sex.Male.ToString(), maleObj);
                    _outlines.Add(maleObj, maleObj.GetComponent<Outline>());
                }

                if (_current != null)
                {
                    _current.SetActive(false);
                }

                maleObj.SetActive(true);
                _outlines[maleObj].enabled = false;
                _current = maleObj;
                break;


            case "Female":
                if (!_selected.TryGetValue(Sex.Female.ToString(), out var femaleObj))
                {
                    femaleObj = Instantiate(_female, _parent);
                    _selected.Add(Sex.Female.ToString(), femaleObj);
                    _outlines.Add(femaleObj, femaleObj.GetComponent<Outline>());
                }

                if (_current != null)
                {
                    _current.SetActive(false);
                }

                femaleObj.SetActive(true);
                _outlines[femaleObj].enabled = false;
                _current = femaleObj;
                break;
        }

        GameManager.instance.audioManager.PlaySFX("Button Click");
    }
}
