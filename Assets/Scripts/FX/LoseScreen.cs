using System;
using System.Collections;
using System.Collections.Generic;
using UI.Dialogue;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FX
{
    public class LoseScreen : MonoBehaviour
    {
        [SerializeField] private FadeAbleImage _winImage;
        [SerializeField] private float fadeSpeed;

        public static LoseScreen Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            _winImage.SetAlpha(0);
        }

        public void Win()
        {
            StartCoroutine(FadeElements(0, 1, _winImage));
        }
        
        
        private IEnumerator FadeElements(float alphaStart, float alphaTarget, IFadeAble element)
        {
            float elapsed = 0f;

            while (elapsed < fadeSpeed)
            {
                elapsed += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(elapsed / fadeSpeed);
                float alpha = Mathf.Lerp(alphaStart, alphaTarget, t);
                element.SetAlpha(alpha);
                

                yield return null;
            }
            element.SetAlpha(alphaTarget);
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}