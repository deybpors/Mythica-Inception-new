using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using _Core.Managers;
using Assets.Scripts._Core.Managers;
using Assets.Scripts._Core.Player;
using Ink.Runtime;
using TMPro;

public class DialogueManager : MonoBehaviour
{
	public static event Action<Story> OnCreateStory; 
	
	private Player _player;
	public void StartDialogue (TextAsset inkJSON, string name, Player player)
	{
		story = null;
		if (_canvas == null)
		{
			_canvas = GetComponent<Canvas>();
		}
		RemoveChildren();
		_player = player;
		_name.text = name;
		inkJSONAsset = inkJSON;
		story = new Story (inkJSONAsset.text);
		if(OnCreateStory != null) OnCreateStory(story);
		RefreshView();
	}

	private void EndDialogue()
	{
		RemoveChildren();
		_canvas.gameObject.SetActive(false);
		_player.inputHandler.activate = true;
		GameManager.instance.uiManager.gameplayUICanvas.SetActive(true);
	}

	// This is the main function called every time the story changes. It does a few things:
	// Set all the old content and choices to inactive and removes them as children.
	// Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!

	void RefreshView () {
		// Remove all the UI on screen
		RemoveChildren ();
		
		// Read all the content until we can't continue any more
		while (story.canContinue) {
			// Continue gets the next line of the story
			string text = story.Continue();
			// This removes any white space from the text.
			text = text.Trim();
			// Display the text on screen!
			CreateContentView(text);
		}

		// Display all the choices, if there are any!
		if(story.currentChoices.Count > 0) {
			for (var i = 0; i < story.currentChoices.Count; i++) {
				var choice = story.currentChoices[i];
				var button = CreateChoiceView(choice.text.Trim(), i);
				// Tell the button what to do when we press it
				button.onClick.AddListener (delegate {
					OnClickChoiceButton(choice);
				});
			}
		}
		// If we've read all the content and there's no choices, the story is finished!
		else {
			var choice = CreateChoiceView(">>>>>", 0);
			choice.onClick.AddListener(EndDialogue);
		}
	}

	// When we click the choice button, tell the story to choose that choice!

	private void OnClickChoiceButton (Choice choice) {
		story.ChooseChoiceIndex (choice.index);
		RefreshView();
	}

	// Creates a textbox showing the the line of text

	private void CreateContentView (string text)
	{
		if (txt == null)
		{
			txt = Instantiate(_textPrefab);
		}
		txt.gameObject.SetActive(true);
		var storyText = txt;
		storyText.text = text;
		storyText.transform.SetParent(textParent, false);
	}

	// Creates a button showing the choice text
	private Button CreateChoiceView (string text, int index) {
		// Creates the button from a prefab

		Button choice;
		if (index >= _buttons.Count)
		{
			choice = Instantiate(_buttonPrefab);
			_buttons.Add(choice);
		}
		else
		{
			choice = _buttons[index];
		}
		
		choice.gameObject.SetActive(true);
		choice.transform.SetParent(buttonParent, false);
		
		// Gets the text from the button prefab
		var choiceText = choice.GetComponentInChildren<TextMeshProUGUI> ();
		choiceText.text = text;

		// Make the button expand to fit the text
		var layoutGroup = choice.GetComponent<HorizontalLayoutGroup> ();
		layoutGroup.childForceExpandHeight = false;

		return choice;
	}

	// Set all the children inactive of this gameobject and removes them as children

	private void RemoveChildren () {
		var textChildCount = textParent.transform.childCount;
		for (var i = 0; i < textChildCount; ++i)
		{
			var child = textParent.transform.GetChild(i);
			child.gameObject.SetActive(false);
			child.SetParent(null, false);
		}

		for (var i = 0; i < _buttons.Count; i++)
		{
			var button = _buttons[i];
			button.transform.SetParent(null, false);
			button.gameObject.SetActive(false);
			button.onClick.RemoveAllListeners();
		}
	}

	private TextAsset inkJSONAsset = null;
	private Story story;
	[SerializeField]
	private TextMeshProUGUI _name;
	
	[SerializeField]
	private Canvas _canvas = null;
	
	[SerializeField]
	private TextMeshProUGUI _textPrefab = null;
	[SerializeField]
	private Button _buttonPrefab = null;

	[SerializeField] private Transform textParent;
	[SerializeField] private Transform buttonParent;

	private List<Button> _buttons = new List<Button>();
	private TextMeshProUGUI txt;
}