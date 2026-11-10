using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image characterImage;
        [SerializeField, Min(0.1f)] private float textSpeed;
        [SerializeField, Min(0.1f)] private float fadeSpeed;

        [Header("References")]
        [SerializeField] private GameObject dialogueParent;
        [SerializeField] private GameObject questionParent;
        [SerializeField] private GameObject optionPrefab;

        private List<IFadeAble> _dialogueLinesGuiElements = new();
        private List<IFadeAble> _questionLinesGuiElements = new();

        [Header("View Settings")]
        [SerializeField] private float optionOffset;
        
        [Header("Sprites")]
        [SerializeField] private Sprite playerSprite;
        [SerializeField] private Sprite girlSprite;
        [SerializeField] private Sprite oldManSprite;
        [SerializeField] private Sprite tringleSprite;
        [SerializeField] private Sprite oldManSpriteShady;
        [SerializeField] private Sprite girlSpriteShady;
            
        [Header("DEBUG")]
        [SerializeField] private TextAsset debugText;
        [SerializeField] private bool trigger;
        
        private bool _inDialogue = false;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            _dialogueLinesGuiElements = dialogueParent.GetComponentsInChildren<IFadeAble>().ToList();
            _questionLinesGuiElements =  questionParent.GetComponentsInChildren<IFadeAble>().ToList();
            foreach (var element in _dialogueLinesGuiElements)
            {
                element.SetAlpha(0);
            }

            foreach (var element in _questionLinesGuiElements)
            {
                element.SetAlpha(0);
            }
        }

        private void Update()
        {
            if (trigger)
            {
                trigger = false;
                ShowDialogue(debugText);
            }
        }


        public void ShowDialogue(TextAsset dialogueAsset)
        {
            if (!_inDialogue)
            {
                StartCoroutine(ShowDialogueEnumerator(dialogueAsset));
            }
        }
        
        public IEnumerator ShowDialogueEnumerator(TextAsset dialogueAsset)
        {
            _inDialogue = true;
            Debug.Log(dialogueAsset.name);
            Time.timeScale = 0f;
            dialogueText.text = "";
            nameText.text = "";
            StartCoroutine(FadeElements(0, 1, _dialogueLinesGuiElements));
            DialogueLoader.DialogueLine[] dialogue = DialogueLoader.DialogueFromFile(dialogueAsset);
            characterImage.sprite = CharacterImageFromCharacter(dialogue[0].Character, dialogue[0].Shady);
            
            yield return new WaitForEndOfFrame();
            
            List<string> choices = new List<string>();
            
            foreach (var line in dialogue)
            {
                if (!line.HasCondition || line.Condition.All(choices.Contains))
                {
                    if (line.PlayerQuestion)
                    {
                        var c = new ChoiceResult();
                        yield return ShowPlayerQuestion(line.PlayerOptions, c);
                        choices.Add(c.Chosen);
                    }
                    else
                    {
                        yield return ShowDialogueLine(line);
                    }
                }
            }
            ApplyChoices(choices);
            yield return FadeElements(1, 0, _dialogueLinesGuiElements);
            Time.timeScale = 1;
            _inDialogue = false;
        }

        private void ApplyChoices(List<string> choices)
        {
            foreach (var choice in choices)
            {
                if (choice == "I need to be alone [Find the monster]")
                {
                    StoryStateLoader.Instance.OnGoAlone();
                } else if (choice == "Do nothing")
                {
                    StoryStateLoader.Instance.OnStayWithGroup();
                } else if (choice == "Accept")
                {
                    StoryStateLoader.Instance.OnAcceptMonsterDeal();
                } else if (choice == "Decline")
                {
                    StoryStateLoader.Instance.OnDeclineMonsterDeal();
                } else if (choice == "Steal tiles")
                {
                    StoryStateLoader.Instance.OnWin();
                } else if (choice == "Go back")
                {
                    StoryStateLoader.Instance.OnLose();
                } else if (choice == "Give food")
                {
                    StoryStateLoader.Instance.OnGiveGirlFood();
                } else if (choice == "Deny")
                {
                    // nothing happens lol
                } else if (choice == "Intervene")
                {
                    StoryStateLoader.Instance.KillGirl();
                } else if (choice == "Let it happen")
                {
                    StoryStateLoader.Instance.KillOldMan();
                }
            }
        }

        private IEnumerator ShowPlayerQuestion(string[] options, ChoiceResult result)
        {
            for (var i = 0; i < options.Length; i++)
            {
                var option = options[i];
                var obj = Instantiate(optionPrefab, questionParent.transform);
                obj.GetComponentInChildren<TMP_Text>().text = option;
                obj.transform.localPosition = new Vector3(0f, optionOffset * ((i - (options.Length - 1)) / 2f), 0f);
                
                var button = obj.GetComponentInChildren<Button>();

                int index = i;
                
                button.onClick.AddListener(() =>
                {
                    result.Chosen = options[index];
                    result.HasChosen = true;
                });
            }
            _questionLinesGuiElements =  questionParent.GetComponentsInChildren<IFadeAble>().ToList();

            yield return StartCoroutine(FadeElements(0, 1, _questionLinesGuiElements));
            
            yield return new WaitUntil(() => result.HasChosen);
            
            yield return StartCoroutine(FadeElements(1, 0, _questionLinesGuiElements));
            
            foreach (Transform child in questionParent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        private Sprite CharacterImageFromCharacter(DialogueLoader.Character character, bool shady)
        {
            if (shady)
            {
                switch (character)
                {
                    case DialogueLoader.Character.Girl:
                        return girlSpriteShady;
                    case DialogueLoader.Character.OldMan:
                        return oldManSpriteShady;
                    case DialogueLoader.Character.Player:
                        return playerSprite;
                    case DialogueLoader.Character.Tringle:
                        return tringleSprite;
                }
            }
            else
            {
                switch (character)
                {
                    case DialogueLoader.Character.Girl:
                        return girlSprite;
                    case DialogueLoader.Character.OldMan:
                        return oldManSprite;
                    case DialogueLoader.Character.Player:
                        return playerSprite;
                    case DialogueLoader.Character.Tringle:
                        return tringleSprite;
                }
            }
            return null;
        }

        private IEnumerator ShowDialogueLine(DialogueLoader.DialogueLine line)
        {
            if (line.Character == DialogueLoader.Character.Girl && StoryStateLoader.Instance.storyState.girlDead ||
                line.Character == DialogueLoader.Character.OldMan && StoryStateLoader.Instance.storyState.oldManDead )
                yield break;

            characterImage.sprite = CharacterImageFromCharacter(line.Character, line.Shady);
            
            nameText.text = DialogueLoader.CharacterDisplayNames[line.Character];

            dialogueText.text = line.Text;
            dialogueText.maxVisibleCharacters = 0;
            dialogueText.ForceMeshUpdate();

            int characterCount = dialogueText.textInfo.characterCount;

            for (int i = 0; i < characterCount; i++)
            {
                dialogueText.maxVisibleCharacters = i + 1;

                if (IsAdvanceHeld())
                {
                    dialogueText.maxVisibleCharacters = characterCount;
                    yield return new WaitUntil(() => !IsAdvanceHeld());
                    break;
                }

                yield return new WaitForSecondsRealtime(1f / textSpeed);
            }

            yield return new WaitUntil(IsAdvancePressed);
            yield return new WaitUntil(() => !IsAdvanceHeld());
        }

        private bool IsAdvancePressed()
        {
            return Input.GetKeyDown(KeyCode.Space) ||
                   Input.GetMouseButtonDown(0) ||
                   Input.GetKeyDown(KeyCode.E);
        }

        private bool IsAdvanceHeld()
        {
            return Input.GetKey(KeyCode.Space) ||
                   Input.GetMouseButton(0) ||
                   Input.GetKey(KeyCode.E);
        }

        private IEnumerator FadeElements(float alphaStart, float alphaTarget, List<IFadeAble> elements)
        {
            float elapsed = 0f;

            while (elapsed < fadeSpeed)
            {
                elapsed += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(elapsed / fadeSpeed);
                float alpha = Mathf.Lerp(alphaStart, alphaTarget, t);

                foreach (var element in elements)
                {
                    element.SetAlpha(alpha);
                }

                yield return null;
            }

            foreach (var element in elements)
            {
                element.SetAlpha(alphaTarget);
            }
        }
    }
    
    public class ChoiceResult
    {
        public string Chosen;
        public bool HasChosen = false;
    }

}