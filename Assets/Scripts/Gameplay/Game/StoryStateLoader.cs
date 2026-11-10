using System.Collections;
using FX;
using Gameplay.Monster;
using Gameplay.Player;
using TMPro;
using UI.Dialogue;
using UnityEngine;

namespace Gameplay.Game
{
    public class StoryStateLoader : MonoBehaviour
    {
        public static StoryStateLoader Instance;
        
        [Header("Story State")]
        [SerializeField]public StoryState storyState;

        [Header("Gameplay Settings")]
        [SerializeField] private float insanityTimer = 130f;
        [SerializeField] private float girlTileTimer = 15f;

        [Header("Start")]
        [SerializeField] private TextAsset startDialogueAsset;

        [Header("Monster")]
        [SerializeField] private TextAsset firstMonsterDialogueAsset;
        [SerializeField] private TextAsset caughtByMonsterDialogueAsset;
        [SerializeField] private TextAsset caughtByMonsterTalkDialogueAsset;
        [SerializeField] private TextAsset monsterKillNPCDialogueAsset;
        [SerializeField] private TextAsset followMonsterDialogueAsset;

        [Header("NPC")]
        [SerializeField] private TextAsset talkToNPCWakingUpDialogueAsset;
        [SerializeField] private TextAsset talkToOneNPCDialogueAsset;
        [SerializeField] private TextAsset talkToOneNPCInsaneDialogueAsset;
        [SerializeField] private TextAsset leaveAloneDialogueAsset;

        [Header("Monster - Deal")]
        [SerializeField] private TextAsset monsterDealDialogueAsset;

        [Header("Exit")]
        [SerializeField] private TextAsset exitFoundDialogueAsset;

        [Header("Food")]
        [SerializeField] private TextAsset girlHungryDialogueAsset;

        [Header("Misc")]
        [SerializeField] private TextAsset firstTileDialogueAsset;
        [SerializeField] private TextAsset firstFoodDialogueAsset;

        [Header("References")]
        [SerializeField] private GameObject girl;
        [SerializeField] private GameObject oldMan;

        private float _insanityTimer;
        private float _girlTileTimer;

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
            storyState = new StoryState();
            _insanityTimer = 0f;

            StartCoroutine(StartBehaviour());
        }

        // START

        private IEnumerator StartBehaviour()
        {
            yield return new WaitForSecondsRealtime(1f);

            storyState.gameStarted = true;

            ShowDialogue(startDialogueAsset);
        }

        // NPC INTERACTION

        public void OnTalkToNPC()
        {
            StartCoroutine(TalkToNPCCoroutine());
        }

        private IEnumerator TalkToNPCCoroutine()
        {
            yield return null;

            // Both NPCs are alive.
            if (!storyState.girlDead && !storyState.oldManDead)
            {
                ShowDialogue(talkToNPCWakingUpDialogueAsset);
                yield break;
            }

            // Player has already sided with / spoken to monster.
            if (storyState.talkedToMonster)
            {
                ShowDialogue(leaveAloneDialogueAsset);

                storyState.alone = true;

                yield break;
            }

            // Player has gone insane.
            if (storyState.insane)
            {
                ShowDialogue(talkToOneNPCInsaneDialogueAsset);

                yield break;
            }

            ShowDialogue(talkToOneNPCDialogueAsset);
        }

        // MONSTER RUNNING AWAY

        public void OnMonsterRun()
        {
            StartCoroutine(MonsterRunCoroutine());
        }

        private IEnumerator MonsterRunCoroutine()
        {
            if (!storyState.seenMonster)
            {
                storyState.seenMonster = true;
                yield return new WaitForSecondsRealtime(10f);

                ShowDialogue(firstMonsterDialogueAsset);

                storyState.seenMonster = true;

                yield break;
            }

            if (storyState.insane &&
                storyState.alone)
            {
                OnMonsterOfferDeal();
            }
        }

        // MONSTER CATCH

        public void OnMonsterCatch()
        {
            StartCoroutine(MonsterCatchCoroutine());
        }
        
        

        private IEnumerator MonsterCatchCoroutine()
        {
            if (storyState.tileCount < 1)
            {
                MonsterController.Instance.SetKidnapTarget(PlayerController.Instance.gameObject);
                yield break;
            }
            if (!storyState.hasBeenCaught)
            {
                ShowDialogue(caughtByMonsterDialogueAsset);

                storyState.hasBeenCaught = true;

                yield break;
            }

            if (storyState.insane &&
                !storyState.talkedToMonster)
            {
                storyState.talkedToMonster = true;

                ShowDialogue(caughtByMonsterTalkDialogueAsset);

                yield break;
            }

            
            if (!storyState.girlDead &&
                !storyState.oldManDead)
            {
                ShowDialogue(monsterKillNPCDialogueAsset);

                yield break;
            }

            if (!storyState.insane)
            {
                ShowDialogue(caughtByMonsterDialogueAsset);

                yield break;
            }

            yield return new WaitForSecondsRealtime(1f);
        }
        
        // ALONE

        public void OnGoAlone()
        {
            storyState.alone = true;

            ShowDialogue(leaveAloneDialogueAsset);
        }

        public void OnStayWithGroup()
        {
            storyState.alone = false;
        }

        // MONSTER DEAL

        public void OnMonsterOfferDeal()
        {
            if (storyState.monsterDealOffered)
                return;

            storyState.monsterDealOffered = true;

            ShowDialogue(monsterDealDialogueAsset);
        }

        public void OnAcceptMonsterDeal()
        {
            storyState.acceptedMonsterDeal = true;
            storyState.followingMonster = true;

            StartCoroutine(FollowMonster());
        }

        public void OnDeclineMonsterDeal()
        {
            storyState.declinedMonsterDeal = true;
            storyState.followingMonster = false;
        }

        private IEnumerator FollowMonster()
        {
            yield return null;
        }

        // EXIT

        public void OnExitFound()
        {
            if (storyState.foundExit)
                return;

            storyState.foundExit = true;

            ShowDialogue(exitFoundDialogueAsset);
        }
        

        // TILES

        public void OnCollectTile()
        {
            storyState.tileCount++;

            if (!storyState.seenFirstTile)
            {
                storyState.seenFirstTile = true;

                ShowDialogue(firstTileDialogueAsset);
            }
        }

        // FOOD

        public void OnCollectFood()
        {
            storyState.foodCount++;

            if (!storyState.seenFirstFood)
            {
                storyState.seenFirstFood = true;

                ShowDialogue(firstFoodDialogueAsset);
            }
        }

        public void OnGirlAskForFood()
        {
            if (storyState.girlAskedForFood)
                return;

            storyState.girlAskedForFood = true;

            ShowDialogue(girlHungryDialogueAsset);
        }

        public void OnGiveGirlFood()
        {
            if (storyState.foodCount <= 0)
            {
                return;
            }

            storyState.foodCount--;
            storyState.gaveGirlFood = true;
        }
        

        // NPC DEATH

        public void KillGirl()
        {
            if (storyState.girlDead)
                return;

            storyState.girlDead = true;
            MonsterController.Instance.SetKidnapTarget(girl);

            Debug.Log("Girl has died.");
        }

        public void KillOldMan()
        {
            if (storyState.oldManDead)
                return;

            storyState.oldManDead = true;
            MonsterController.Instance.SetKidnapTarget(oldMan);

            Debug.Log("Old man has died.");
        }


        private void Update()
        {
            if (storyState == null)
                return;

            _girlTileTimer += Time.deltaTime;
            if (_girlTileTimer >= girlTileTimer)
            {
                _girlTileTimer = 0f;
                if (!storyState.girlDead && storyState.hasBeenCaught)
                {
                    OnGirlAskForFood();
                }
            }
            
            if (storyState.insane)
                return;

            _insanityTimer += Time.deltaTime;

            if (_insanityTimer >= insanityTimer)
            {
                storyState.insane = true;

                Debug.Log("Player has become insane.");
            }
        }


        private void ShowDialogue(TextAsset dialogue)
        {
            if (dialogue == null)
            {
                return;
            }

            if (DialogueManager.Instance == null)
            {
                return;
            }

            DialogueManager.Instance.ShowDialogue(dialogue);
        }

        public void OnWin()
        {
            Time.timeScale = 0;
            WinScreen.Instance.Win();
        }

        public void OnLose()
        {
            Time.timeScale = 0;
            LoseScreen.Instance.Win();
        }
    }
}
