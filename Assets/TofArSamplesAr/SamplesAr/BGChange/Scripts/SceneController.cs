/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using UnityEngine;
using TofAr.V0.Hand;
using TofAr.V0.Segmentation.Sky;
using TofArSettings.Segmentation;
using System.Threading;
using TofAr.V0.Color;

namespace TofArARSamples.BGChange
{
    /// <summary>
    /// BGChange Scene management
    /// </summary>
    public class SceneController : MonoBehaviour
    {

        [SerializeField]
        private Material maskMaterial = null;

        [SerializeField]
        private SkySegmentationDetector skyDetector = null;

        [SerializeField]
        private SkySegmentationController skySegmentationController;

        [SerializeField]
        private AudioSource poseSound = null;

        [SerializeField]
        private GameObject poseEffectObjectPrefab = null;

        [SerializeField]
        private HandModel leftHandModel = null;

        [SerializeField]
        private HandModel rightHandModel = null;

        [SerializeField]
        private int successPosePoint = 12;

        public int SuccessPosePoint { get { return successPosePoint; } set { successPosePoint = value; } }

        [SerializeField]
        private int failurePosePoint = -30;

        public int FailurePosePoint { get { return failurePosePoint; } set { failurePosePoint = value; } }

        private readonly PoseIndex targetPoseIndex = PoseIndex.ThumbUp;

        private Coroutine currentMaskAnimationCoroutine = null;

        private float currentMaskAlpha = 0.0f;

        private int currentPoseContinuCount = 0;

        private readonly int poseCountThreshhold = 30;

        private bool isPlaySound = false;

        private bool isContinuePose = false;

        private GameObject currentPoseEffectObject = null;

        private SynchronizationContext context;

        IEnumerator Start()
        {
            context = SynchronizationContext.Current;

            //Mask initialization
            this.maskMaterial.SetFloat("_Alpha", 0.0f);

            skySegmentationController.SkySegmentationEnabled = true;


            yield return DisableHumanSegmentationSettings();

        }

        IEnumerator DisableHumanSegmentationSettings()
        {
            // Wait for settings to be created
            yield return new WaitForEndOfFrame();

            // Disable interactability for human settings
            SegmentationSettings segmentationSettings = FindObjectOfType<SegmentationSettings>();

            if (segmentationSettings != null)
            {
                TofArSettings.UI.ItemToggle[] detectorToggles = segmentationSettings.transform.GetComponentsInChildren<TofArSettings.UI.ItemToggle>(true);

                if (detectorToggles != null)
                {
                    foreach (var toggle in detectorToggles)
                    {
                        if (toggle.gameObject.name.Contains("Human"))
                        {
                            toggle.Interactable = false;
                        }
                    }
                }
            }
        }

        private void OnEnable()
        {
            //Notification registration of new frame arrival of hand
            TofArHandManager.OnFrameArrived += OnHandNewFrameArrived;
            TofArColorManager.OnStreamStarted += OnColorStreamStarted;

            if (TofArColorManager.Instance.IsStreamActive)
            {
                OnColorStreamStarted(null, null);
            }
        }

        private void OnDisable()
        {
            TofArHandManager.OnFrameArrived -= OnHandNewFrameArrived;
            TofArColorManager.OnStreamStarted -= OnColorStreamStarted;
        }

        void Update()
        {
            //Pass the latest empty mask image to mask processing
            this.maskMaterial.SetTexture("_MaskTexSky", this.skyDetector.MaskTexture);

#if UNITY_IOS
            this.maskMaterial.SetInt("_invertUVXSky", 1);
#endif

#if UNITY_ANDROID
            this.maskMaterial.SetInt("_invertUVXSky", 0);
#endif

            this.maskMaterial.SetFloat("_invertSky", skySegmentationController.NotSkySegmentationEnabled ? 1 : 0);
            this.maskMaterial.SetFloat("_useSky", skySegmentationController.SkySegmentationEnabled ? 1 : 0);

            if (isPlaySound == true)
            {
                //
                //Sound playback when changing effects
                //Because it is necessary to call audio playback from the main thread
                //
                poseSound.time = 0.0f;
                poseSound.Play();

                isPlaySound = false;
            }

        }

        private void OnColorStreamStarted(object sender, Texture2D ColorTexture)
        {
            var resolutionProperty = TofAr.V0.Color.TofArColorManager.Instance.GetProperty<TofAr.V0.Color.ResolutionProperty>();

            int width = resolutionProperty.width;
            int height = resolutionProperty.height;

            int segWidth = skyDetector.MaskTexture.width;
            int segHeight = skyDetector.MaskTexture.height;

            float segRatio = (float)segWidth / (float)segHeight;
            float imgRatio = (float)width / (float)height;

            float segHeightScale = 1f;
            float segWidthScale = 1f;
            float vOffset = 0;
            float uOffset = 0;

            // add offset and scale to match segmentation ratio
            if (segRatio > imgRatio)
            {
                int colorHeightAdjusted = (int)(width / segRatio);
                int vOffset0 = ((height - colorHeightAdjusted) / 2);
                vOffset = -(float)vOffset0 / (float)colorHeightAdjusted;

                segHeightScale = (float)height / (float)colorHeightAdjusted;
            }
            else if (segRatio < imgRatio)
            {
                int colorWidthAdjusted = (int)(height * segRatio);
                int uOffset0 = ((width - colorWidthAdjusted) / 2);
                uOffset = -(float)uOffset0 / (float)colorWidthAdjusted;

                segWidthScale = (float)width / (float)colorWidthAdjusted;
            }

            this.maskMaterial.SetFloat("_ScaleV", segHeightScale);
            this.maskMaterial.SetFloat("_ScaleU", segWidthScale);
            this.maskMaterial.SetFloat("_OffsetV", vOffset);
            this.maskMaterial.SetFloat("_OffsetU", uOffset);
        }


        #region Effect system

        /// <summary>
        /// Called when a new frame of the hand arrives
        /// </summary>
        /// <param name="sender"></param>
        private void OnHandNewFrameArrived(object sender)
        {
            //Obtaining information such as hand poses
            HandData handData = TofArHandManager.Instance.HandData;
            handData.GetPoseIndex(out PoseIndex left, out PoseIndex right);


            if (left == targetPoseIndex || right == targetPoseIndex)
            {
                /// If you were in the intended pose

                //Add points to the pause continuation count
                currentPoseContinuCount += successPosePoint;

                if (currentPoseContinuCount >= poseCountThreshhold)
                {
                    currentPoseContinuCount = poseCountThreshhold;
                }

            }
            else
            {
                //If you were in the wrong pose

                //Pose continuation count is deducted
                currentPoseContinuCount += failurePosePoint;

                if (isContinuePose == true)
                {
                    //When you stop the specified pose, the score is reset
                    currentPoseContinuCount = 0;
                }


                if (currentPoseContinuCount < 0)
                {
                    currentPoseContinuCount = 0;
                }
            }

            context.Post((d) =>
            {
                Vector3 poseHandWorldPosition = Vector3.zero;
                //Get the world coordinates of a part of each hand
                if (left == targetPoseIndex)
                {
                    poseHandWorldPosition = leftHandModel.WorldHandPoints[(int)HandPointIndex.ThumbJoint];


                }
                else if (right == targetPoseIndex)
                {
                    poseHandWorldPosition = rightHandModel.WorldHandPoints[(int)HandPointIndex.ThumbJoint];
                }


                if (currentPoseContinuCount < poseCountThreshhold)
                {
                    //Do not continue the specified pose for a certain period of time
                    return;
                }

                if (isContinuePose == true)
                {
                    //If you continue the specified pose
                    //Update the coordinates of the pose recognition effect
                    UpdatePoseEffectPosition(poseHandWorldPosition);
                    return;
                }

                //
                // When the specified pose is continued for a certain period of time
                //
                Debug.Log("The specified pose was continued for a certain period of time.");

                currentPoseContinuCount = 0;
                isContinuePose = true;
                this.maskMaterial.SetFloat("_Alpha", 0.0f);


                // When recognizing a pose, the effect starts at the position of the hand
                StartPoseEffect(poseHandWorldPosition);

                //The change of the empty part starts
                StartSkyMaskObjectAnimation();
            }, null);



        }

        /// <summary>
        /// Start the effect when recognizing a pose
        /// </summary>
        /// <param name="effectWorldPosition"></param>
        private void StartPoseEffect(Vector3 effectWorldPosition)
        {
            if (currentPoseEffectObject != null)
            {
                Destroy(currentPoseEffectObject);
                currentPoseEffectObject = null;
            }

            //Effect start at the specified position
            currentPoseEffectObject = Instantiate(poseEffectObjectPrefab, effectWorldPosition, Quaternion.identity);

        }

        /// <summary>
        /// Updates the effect coordinates for pose recognition
        /// </summary>
        /// <param name="effectWorldPosition"></param>
        private void UpdatePoseEffectPosition(Vector3 effectWorldPosition)
        {
            if (currentPoseEffectObject != null)
            {
                currentPoseEffectObject.transform.position = effectWorldPosition;
            }
        }

        /// <summary>
        /// Start processing to change the part of the sky
        /// </summary>
        public void StartSkyMaskObjectAnimation()
        {

            if (currentMaskAnimationCoroutine != null)
            {
                StopCoroutine(currentMaskAnimationCoroutine);
            }
            currentMaskAnimationCoroutine = StartCoroutine(MaskSkyObjectAnimtionProcess());
        }



        private IEnumerator MaskSkyObjectAnimtionProcess()
        {


            //Delay more than other effects
            yield return new WaitForSeconds(0.15f);

            //Sound effect
            isPlaySound = true;

            yield return new WaitForSeconds(0.16f);

            //Display the image as it is
            currentMaskAlpha = 1.0f;
            this.maskMaterial.SetFloat("_Alpha", currentMaskAlpha);

            yield return new WaitForSeconds(1.5f);

            //Stop the pause effect
            if (currentPoseEffectObject != null)
            {
                Destroy(currentPoseEffectObject);
                currentPoseEffectObject = null;
            }


            //Display as it is for a certain period of time
            yield return new WaitForSeconds(4.5f);


            // Start processing to return to the original video
            int restoreAnimationCount = 100;//Number of stages of transmission change
            float restoreAnimationTime = 5.0f;//Time to completely penetrate

            for (int i = 0; i < restoreAnimationCount; i++)
            {
                //Processing to make it transparent little by little
                currentMaskAlpha = currentMaskAlpha - 1.0f / restoreAnimationCount;
                this.maskMaterial.SetFloat("_Alpha", currentMaskAlpha);

                yield return new WaitForSeconds(restoreAnimationTime / restoreAnimationCount);
            }

            //Decide that you have stopped continuing the specified pose
            isContinuePose = false;
            currentPoseContinuCount = 0;

            yield return null;
        }
    }
    #endregion
}
