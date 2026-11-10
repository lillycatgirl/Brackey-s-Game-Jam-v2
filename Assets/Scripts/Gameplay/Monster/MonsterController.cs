using System;
using System.Collections.Generic;
using Gameplay.Game;
using Gameplay.Player;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Gameplay.Monster
{
    public class MonsterController : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private FlashlightController flashlight;
        
        private static int _mask;

        public static MonsterController Instance;
        
        private bool _kidnapped;
        [SerializeField] private GameObject kidnappedObject;

        [Header("Behaviour Settings")]
        [SerializeField] private float chaseSpeed;
        [SerializeField] private float stalkSpeed;
        [SerializeField] private float hideSpeed;
        
        [SerializeField] private int patienceWhenFound;
        [SerializeField] private float waitTimeWhenFound;
        
        [Header("Hiding Search Settings")]
        [SerializeField] private int searchBegin;
        [SerializeField] private int searchEnd;
        [SerializeField] [Min(1)] private int step;

        
        private NavMeshAgent _agent;

        [SerializeField] private float patience;


        private float _waiting;
        private void Awake()
        {
            _mask = 1 << NavMesh.GetAreaFromName("Walkable");
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private enum State
        {
            Stalk,
            Chase,
            Hide,
            Grab
        }
        
        [SerializeField] private State state;
        
        
        public void SetKidnapTarget(GameObject target)
        {
            if (_kidnapped) return;
            kidnappedObject =  target;
            _kidnapped = false;
        }
        
        private List<Vector2> HidePositions(Vector2 center, float distance, int count)
        {
            List<Vector2> positions = new List<Vector2>();

            for (int i = 0; i < count; i++)
            {
                float angle = i * Mathf.PI * 2f / count;

                Vector2 offset = new Vector2(
                    Mathf.Cos(angle) * distance,
                    Mathf.Sin(angle) * distance
                );

                positions.Add(center + offset);
            }

            return positions;
        }
        private void Start()
        {
            state = State.Hide;
            _agent = GetComponent<NavMeshAgent>();
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
        }

        private void Update()
        {
            UpdateBehaviour();
        }

        

        private void UpdateBehaviour()
        {
            if (Time.timeScale == 0) return;

            if (kidnappedObject != null && !_kidnapped)
            {
                state = State.Grab;
            }

            
            
            switch (state)
            {
                case State.Hide:
                    Hide();
                    break;
                case State.Chase:
                    Chase();
                    break;
                case State.Stalk:
                    Stalk();
                    break;
                case State.Grab:
                    Grab();
                    break;
            }
        }

        private void Grab()
        {
            _agent.stoppingDistance = 0.5f;
            _agent.autoBraking = false;
            _agent.speed = chaseSpeed;
            _agent.SetDestination(kidnappedObject.transform.position);

            if (_agent.remainingDistance < _agent.stoppingDistance)
            {
                _kidnapped = true;
                _waiting = 10f;
                patience = patienceWhenFound;
                state = State.Hide;
                _agent.SetDestination(TestHidingPositions());
                kidnappedObject.transform.SetParent(transform);
                kidnappedObject.transform.localPosition = Vector3.zero;
                kidnappedObject.transform.localRotation = Quaternion.identity;

                Hide();
            }
        }

        private void Hide()
        {
            if (kidnappedObject != null)
            {
                kidnappedObject.transform.localPosition = Vector3.zero;
            }
            
            _agent.stoppingDistance = 0.2f;
            _agent.autoBraking = true;
            _agent.speed = hideSpeed;

            bool pathCompleted = _agent.pathStatus == NavMeshPathStatus.PathComplete &&
                                 _agent.remainingDistance < _agent.stoppingDistance &&
                                 !_agent.pathPending;
            if (pathCompleted)
            {
                _waiting -= Time.deltaTime;
                if (_kidnapped)
                {
                    Destroy(kidnappedObject);
                    kidnappedObject = null;
                    _kidnapped = false;
                }
            }
                
            if (pathCompleted && _waiting < -1)
            {
                _waiting = waitTimeWhenFound;
            }
            
            

            if (_waiting < 0)
            {
                state = State.Stalk;
                _waiting = -100f;
            }
        }

        private Vector2 TestHidingPositions()
        {
            List<Vector2> positions = new List<Vector2>();
            for (int i = searchBegin; i < searchEnd; i += step)
            {
                foreach (var pos in HidePositions(player.position, i, 8))
                {
                    if (!flashlight.IsPointLOS(pos) && !flashlight.IsPointIlluminated(pos))
                    {
                        if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 0.1f, _mask))
                        {
                            var path = new NavMeshPath();

                            if (_agent.CalculatePath(hit.position, path) &&
                                path.status == NavMeshPathStatus.PathComplete)
                            {
                                positions.Add(hit.position);
                            }
                        }
                    }
                }
            }
            if (positions.Count > 0)
                return positions[Random.Range(0, positions.Count)];
            return Vector2.zero;
        }

        private void Chase()
        {
            _agent.stoppingDistance = 0.5f;
            _agent.autoBraking = false;
            _agent.speed = chaseSpeed;
            _agent.SetDestination(player.position);

            if (_agent.remainingDistance < _agent.stoppingDistance)
            {
                StoryStateLoader.Instance.OnMonsterCatch();
                patience = patienceWhenFound;
                state = State.Hide;
                _agent.SetDestination(TestHidingPositions());
                Hide();
            }
        }

        private void Stalk()
        {
            _agent.stoppingDistance = 5f;
            _agent.autoBraking = false;
            _agent.speed = stalkSpeed;
            _agent.SetDestination(player.position);

            if (_agent.remainingDistance < _agent.stoppingDistance)
            {
                patience -= Time.deltaTime;
            }
            else
            {
                patience -= Time.deltaTime / 3;
            }
            
            if (flashlight.IsPointIlluminated(transform.position))
            {
                if (Random.Range(0, Mathf.Max(Mathf.Round(patience), 0)) == 0 && !(StoryStateLoader.Instance.storyState.tileCount < 1))
                {
                    patience = 0;
                    state = State.Chase;
                    Chase();
                }
                else
                {
                    patience = Mathf.Min(patience, patienceWhenFound);
                    state = State.Hide;
                    _waiting = 10f;
                    _agent.SetDestination(TestHidingPositions());
                    Hide();
                    StoryStateLoader.Instance.OnMonsterRun();
                }
            }
        }
    }
}
