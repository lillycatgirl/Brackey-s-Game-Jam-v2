using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Monster;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FX
{
    public class MonsterIKController : MonoBehaviour
    {
        private List<Transform> _iKPositions;

        [Header("IK Settings")]
        [SerializeField] private float legRange;

        [SerializeField] private float xMult;
        
    
        private Vector3 _previousPosition;
        
        private int _wallMask;

        void Awake()
        {
            _wallMask = LayerMask.GetMask("Walls");
        }

        private void Start()
        {
            _iKPositions = GameObject.FindGameObjectsWithTag("LegIK").ToList().ConvertAll(x => x.transform);
        }

        private void FixedUpdate()
        {
            transform.localPosition = new (Mathf.Sin(Time.time/1.212f) / 3f, Mathf.Sin(Time.time / 1.102f) / 5f, 0);
        }

        private void Update()
        {
            if (transform.position == _previousPosition) return;
            
            var offset = transform.position - _previousPosition;
            float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg - 90;
            
            foreach (var iKPosition in _iKPositions)
            {
                iKPosition.transform.position -= offset;
                if ((iKPosition.position - transform.position).magnitude > legRange)
                {
                    Vector2 random = Random.insideUnitCircle;

                    Vector3 newPosition = new Vector3(
                        random.x * xMult,
                        Mathf.Abs(random.y),
                        0f
                    );
                    newPosition = Quaternion.Euler(0f, 0f, angle) * newPosition;
                    
                    var direction = newPosition.normalized;
                    
                    var hit = Physics2D.Raycast(transform.position, direction, legRange, _wallMask);

                    if (hit.collider != null)
                    {
                        newPosition = newPosition.normalized * hit.distance;
                    }
                    else
                    {
                        newPosition = newPosition.normalized * legRange;
                    }
                    

                    iKPosition.position = transform.position + newPosition;
                    iKPosition.gameObject.GetComponent<MonsterLeg>().PlayAudio();
                }
            }
            
            _previousPosition = transform.position;
        }
    }
}
