using UnityEngine;

namespace Rekkuzan.UI.iOS
{
    public class ToggleTransition : MonoBehaviour
    {
        #region SerializeField
        [SerializeField] UnityEngine.UI.Toggle ToggleUnityComponent;
        [SerializeField] UnityEngine.UI.Image BackgroundColor;
        [SerializeField] bool isOn = false;

        [SerializeField] GameObject On;
        [SerializeField] GameObject Off;


        [SerializeField] RectTransform BackgroundScale;
        [SerializeField] RectTransform CircleMovement;

        [SerializeField] Color ColorBackgroundOff;
        [SerializeField] Color ColorBackgroundOn;
        [SerializeField] float Duration = 0.3f;
        [SerializeField] Vector3 OffPosition = new Vector3(31, -5, 0);
        [SerializeField] Vector3 OnPosition = new Vector3(71, -5, 0);

        [SerializeField] AnimationCurve animationCurveXPosition;
        [SerializeField] AnimationCurve animationBackgroundScale;
        #endregion

        #region Private Attributes
        private bool inTransition = false;
        private float timeElapsed = 0.0f;
        #endregion

        #region Monobehaviour methods
        private void Awake()
        {
            if (BackgroundColor == null || BackgroundScale == null || CircleMovement == null || ToggleUnityComponent == null)
            {
                Debug.LogWarning(string.Format("[IOSToggleTransition] {0} is missing components", gameObject.name));
                return;
            }
        }

        private void Start()
        {
            SetState(ToggleUnityComponent.isOn);
            if (!isOn)
            {
                On.SetActive(true);
                Off.SetActive(false);

            }
            else
            {
                On.SetActive(false);
                Off.SetActive(true);
            }

        }

        private void Update()
        {
            // Do not perform any update if there is no transition ingoing
            if (!inTransition)
                return;

            timeElapsed += Time.deltaTime;
            float timeProgress = timeElapsed / Duration;

            // Perform all modifications
            MoveCircle(timeProgress);
            ColorBackground(timeProgress);
            ScaleBackground(timeProgress);

            // Make sure the toggle is not interactable during transition
            if (ToggleUnityComponent.interactable)
            {
                ToggleUnityComponent.interactable = false;
            }

            if (timeElapsed >= Duration)
            {
                SetState(!isOn);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Will move Circle from OffPosition to OnPosition regarding the t progress time
        /// </summary>
        /// <param name="t"></param>
        private void MoveCircle(float t)
        {
            float evaluatedValue = animationCurveXPosition.Evaluate(t);
            Vector3 valueAdded;

            if (isOn)
            {
                valueAdded = (OffPosition - OnPosition) * evaluatedValue;
                valueAdded += OnPosition;
                On.SetActive(true);
                Off.SetActive(false);
            }
            else
            {
                valueAdded = (OnPosition - OffPosition) * evaluatedValue;
                valueAdded += OffPosition;
                On.SetActive(false);
                Off.SetActive(true);
            }

            CircleMovement.anchoredPosition = valueAdded;
        }

        /// <summary>
        /// Will scale the background from 0 to 1 regarding the t progress time
        /// </summary>
        /// <param name="t"></param>
        private void ScaleBackground(float t)
        {
            float evaluatedValue = animationBackgroundScale.Evaluate(t);
            Vector3 valueAdded;

            if (isOn)
            {
                valueAdded = (Vector3.one - Vector3.zero) * evaluatedValue;
                valueAdded += Vector3.zero;
            }
            else
            {
                valueAdded = (Vector3.zero - Vector3.one) * ((evaluatedValue * 2.0f) > 1.0f ? 1.0f : evaluatedValue * 2.0f);
                valueAdded += Vector3.one;
            }

            BackgroundScale.localScale = valueAdded;
        }

        /// <summary>
        /// Will lerp the color of the background ColorBackgroundOn to ColorBackgroundOff regarding the t progress time
        /// </summary>
        /// <param name="t"></param>
        private void ColorBackground(float t)
        {
            if (isOn)
            {
                BackgroundColor.color = Color.Lerp(ColorBackgroundOn, ColorBackgroundOff, t);
            }
            else
            {
                BackgroundColor.color = Color.Lerp(ColorBackgroundOff, ColorBackgroundOn, t);
            }
        }


        /// <summary>
        /// Set the state of the toggle. This function does not perform transition. Usually called when setting at the beginning/end of the transition
        /// </summary>
        /// <param name="on"></param>
        private void SetState(bool on)
        {
            BackgroundColor.color = !on ? ColorBackgroundOff : ColorBackgroundOn;
            BackgroundScale.localScale = !on ? Vector3.one : Vector3.zero;
            CircleMovement.anchoredPosition = !on ? OffPosition : OnPosition;
            inTransition = false;
            timeElapsed = 0.0f;
            isOn = on;
            ToggleUnityComponent.interactable = true;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Toggle the toggle if not already in transition
        /// </summary>
        public void Toggle()
        {
            // if in transition do nothing
            if (inTransition)
                return;

            inTransition = true;
            ToggleUnityComponent.interactable = false;
        }
        #endregion
    }
}