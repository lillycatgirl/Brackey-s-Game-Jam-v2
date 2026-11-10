using TMPro;
using UnityEngine;

namespace UI.Dialogue
{
    public class FadeAbleText : MonoBehaviour, IFadeAble
    {
        private TMP_Text _text;

        void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        public void SetAlpha(float alpha)
        {
            _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, alpha);
        }
    }
}
