using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WiDiD.UI
{
    /// <summary>
    /// Canvas group extension enabling smooth UI transition & effects
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupCustom : MonoBehaviour
    {
        #region Fields
        private CanvasGroup canvasGroup = null;
        private Canvas canvas = null;
        private IEnumerator fadeBehaviour = null;
        private float lerpValue = 0.0f;
        private sbyte factor = 1;
        private bool isForcingAlpha = false; // used in CanvasFadeOutWithDistance 

        #endregion

        #region Accessors
        [SerializeField] private float fadeTime = 1.0f;
        public CanvasGroup CanvasGroup { get => canvasGroup; set => canvasGroup = value; }
        public bool IsVisible { get => canvasGroup.alpha == 1; }
        public Canvas Canvas { get => canvas; set => canvas = value; }
        public float FadeTime { get => fadeTime; set => fadeTime = value; }
        public bool IsForcingAlpha { get => isForcingAlpha; set => isForcingAlpha = value; }
        #endregion

        #region Events
        public Action<bool> OnFade = null;
        #endregion

        #region Public Methods
        /// <summary>
        /// Lerp alpha of canvas group Unity UI component to 1 or 0
        /// </summary>
        /// <param name="fadeIn">true sets alpha to 1, false 0</param>
        public void Fade(bool fadeIn)
        {
            if (fadeIn == true)
            {
                factor = 1;
            }
            else
            {
                factor = -1;
            }
            if (fadeBehaviour == null)
            {
                fadeBehaviour = FadeBehaviour();
                StartCoroutine(fadeBehaviour);
            }
        }

        /// <summary>
        /// Instantly set the <see cref="CanvasGroup "/> alpha to fadeIn hashCode value; 
        /// </summary>
        /// <param name="fadeIn">Alpha = 1 for true, 0 for false </param>
        public void SetFadeIn(bool fadeIn)
        {
            if (!canvasGroup)
                canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = fadeIn.GetHashCode();
            canvasGroup.interactable = fadeIn;
            canvasGroup.blocksRaycasts = fadeIn;
            lerpValue = IsVisible.GetHashCode();
            OnFade?.Invoke(fadeIn);
        }
        #endregion

        #region Private Methods
        private void OnEnable()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvas = GetComponent<Canvas>();
            lerpValue = canvasGroup.alpha;

            if (lerpValue <= 0)
            {
                isForcingAlpha = true;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            }
        }
        private void DisableInteractions()
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        private IEnumerator FadeBehaviour()
        {
            isForcingAlpha = true;
            yield return new WaitForEndOfFrame();
            lerpValue += Time.deltaTime / fadeTime * factor;
            if (lerpValue<=0f) DisableInteractions();
            while (lerpValue > 0 && lerpValue < 1)
            {
                lerpValue += Time.deltaTime / fadeTime * factor;
                canvasGroup.alpha = Mathf.Lerp(0, 1, lerpValue);
                yield return null;
            }
            if (lerpValue <= 0)
            {
                SetFadeIn(false);
            }
            else if (lerpValue >= 1)
            {
                SetFadeIn(true);
                isForcingAlpha = false;
            }
            fadeBehaviour = null;
            yield break;
        }
        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CanvasGroupCustom))]
    public class CanvasGroupCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            CanvasGroupCustom canvasGroupCustomTarget = (CanvasGroupCustom)target;

            if (Application.isPlaying)
            {
                if (GUILayout.Button(new GUIContent("FadeIn")))
                {
                    canvasGroupCustomTarget.Fade(true);
                }

                if (GUILayout.Button(new GUIContent("FadeOut")))
                {
                    canvasGroupCustomTarget.Fade(false);
                }
            }
        }
    }
#endif
}
