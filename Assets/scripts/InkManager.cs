using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryScript : MonoBehaviour
{
    public TextAsset inkJSON;
    private Story story;

    public Text textPrefab;
    public Button buttonPrefab;
    private Text storyText;

    private List<Button> choiceButtons = new List<Button>();

    void Start()
    {
        story = new Story(inkJSON.text);
        refreshUI();
    }

    void refreshUI()
    {
        eraseUI();

        storyText = Instantiate(textPrefab, transform, false);
        storyText.text = "";

        StartCoroutine(WriteTextSlowly());
    }

    void CreateButtons()
    {
        eraseButtons();

        if (story.currentChoices.Count > 0)
        {
            foreach (Choice choice in story.currentChoices)
            {
                Button choiceButton = Instantiate(buttonPrefab, transform, false);
                Text choiceText = choiceButton.GetComponentInChildren<Text>();

                if (choiceText != null)
                {
                    choiceText.text = choice.text;
                }
                else
                {
                    Debug.LogError("Button prefab is missing a Text component.");
                }

                choiceButtons.Add(choiceButton);
                choiceButton.onClick.AddListener(() => chooseStoryChoice(choice.index));
            }
        }
        else
        {
            if (!story.canContinue)
            {
                Button restartButton = Instantiate(buttonPrefab, transform, false);
                Text restartText = restartButton.GetComponentInChildren<Text>();
                restartText.text = "Restart";
                restartButton.onClick.AddListener(RestartStory);
                choiceButtons.Add(restartButton);
            }
        }
    }

    void eraseUI()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        choiceButtons.Clear();
    }

    void eraseButtons()
    {
        foreach (Button button in choiceButtons)
        {
            Destroy(button.gameObject);
        }
        choiceButtons.Clear();
    }

    void chooseStoryChoice(int choiceIndex)
    {
        story.ChooseChoiceIndex(choiceIndex);
        refreshUI();
    }

    IEnumerator WriteTextSlowly()
    {
        string text = "";
        while (story.canContinue)
        {
            text += story.Continue().Trim() + " ";
        }

        foreach (char letter in text)
        {
            storyText.text += letter;
            yield return new WaitForSeconds(0.002f);
        }

        CreateButtons();
    }

    void RestartStory()
    {
        story = new Story(inkJSON.text);
        refreshUI();
    }
}
