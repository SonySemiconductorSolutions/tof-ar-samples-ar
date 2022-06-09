/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Hand;
using UnityEngine;
using UnityEngine.UI;

namespace TofArARSamples.RockPaperScissors
{
    /// <summary>
    /// Progress management class of rock-paper-scissors app
    /// </summary>
    public class RpsAppProgressManager : MonoBehaviour
    {
        [SerializeField]
        private AppStatus appStatus = AppStatus.stay;

        [SerializeField]
        private AppLanguage appLanguage = AppLanguage.english;

        [SerializeField]
        private GameObject handModelLeft = null;

        [SerializeField]
        private GameObject handModelRight = null;

        [SerializeField]
        private Text playerText = null;

        [SerializeField]
        private Image playerCircle = null;

        [SerializeField]
        private Image playerHand = null;

        [SerializeField]
        private Text opponentText = null;

        [SerializeField]
        private Image opponentCircle = null;

        [SerializeField]
        private Image opponentHand = null;

        [SerializeField]
        private Text topText = null;

        [SerializeField]
        private Text bottomText = null;

        [SerializeField]
        private Sprite question = null;

        [SerializeField]
        private Sprite fist = null;

        [SerializeField]
        private Sprite peace = null;

        [SerializeField]
        private Sprite openPalm = null;

        private RpsAudioManager rpsAudioManager = null;

        private PoseIndex playerHandIndex = PoseIndex.Fist;

        private PoseIndex confirmPlayerHand = PoseIndex.Fist;

        private int cpuHandIndex = 9;

        private float remainingTime = 2.0f;

        private bool win = true;

        private ThumbsUpHand thumbsUpHand = ThumbsUpHand.right;

        /// <summary>
        /// Application status
        /// </summary>
        private enum AppStatus
        {
            stay,
            ready,
            rematch,
            game,
            result
        }

        /// <summary>
        /// Application language
        /// </summary>
        public enum AppLanguage
        {
            english,
            chinese,
            japanese
        }

        /// <summary>
        /// Thumbs up hands
        /// </summary>
        private enum ThumbsUpHand
        {
            right,
            left
        }

        void Awake()
        {
            rpsAudioManager = FindObjectOfType<RpsAudioManager>();
        }

        /// <summary>
        /// Pose reflection
        /// </summary>
        public void PoseReflection(PoseIndex poseLeft, PoseIndex poseRight)
        {
            if (appStatus == AppStatus.stay
                || appStatus == AppStatus.result)
            {
                if (poseLeft == PoseIndex.ThumbUp || poseRight == PoseIndex.ThumbUp)
                {
                    if (poseLeft == PoseIndex.ThumbUp)
                    {
                        thumbsUpHand = ThumbsUpHand.left;
                    }
                    else if (poseRight == PoseIndex.ThumbUp)
                    {
                        thumbsUpHand = ThumbsUpHand.right;
                    }
                    playerHandIndex = PoseIndex.Fist;
                    remainingTime = 2.0f;
                    appStatus = AppStatus.ready;
                }

            }
            else if (appStatus == AppStatus.game)
            {
                if (thumbsUpHand == ThumbsUpHand.right &&
                    (poseRight == PoseIndex.Fist || poseRight == PoseIndex.Peace || poseRight == PoseIndex.OpenPalm))
                {
                    playerHandIndex = poseRight;
                }
                else if (thumbsUpHand == ThumbsUpHand.left &&
                    (poseLeft == PoseIndex.Fist || poseLeft == PoseIndex.Peace || poseLeft == PoseIndex.OpenPalm))
                {
                    playerHandIndex = poseLeft;
                }
            }
        }

        void Update()
        {
            if (appStatus == AppStatus.ready)
            {
                if (remainingTime == 2.0f)
                {
                    if (thumbsUpHand == ThumbsUpHand.right)
                    {
                        playerHand.transform.localEulerAngles = new Vector3(0, 180, 0);
                        handModelLeft.SetActive(false);
                    }
                    else if (thumbsUpHand == ThumbsUpHand.left)
                    {
                        playerHand.transform.localEulerAngles = new Vector3(0, 0, 0);
                        handModelRight.SetActive(false);
                    }

                    rpsAudioManager.PlaySoundEffect(RpsAudioManager.SoundEffect.ready, appLanguage);

                    switch (appLanguage)
                    {
                        case AppLanguage.english:
                            topText.text = "Are you ready?";
                            bottomText.text = "";
                            break;
                        case AppLanguage.chinese:
                            topText.text = "你准备好了吗？";
                            bottomText.text = "";
                            break;
                        case AppLanguage.japanese:
                            topText.text = "準備はよろしいですか？";
                            bottomText.text = "";
                            break;
                    }
                }

                remainingTime -= Time.deltaTime;
                playerCircle.fillAmount = remainingTime / 2;
                opponentCircle.fillAmount = remainingTime / 2;
                ImageChange((int)playerHandIndex, 9);

                if (remainingTime <= 0)
                {
                    remainingTime = 2.0f;
                    appStatus = AppStatus.game;
                }
            }
            else if (appStatus == AppStatus.rematch)
            {
                if (remainingTime == 2.0f)
                {
                    rpsAudioManager.PlaySoundEffect(RpsAudioManager.SoundEffect.again, appLanguage);
                    switch (appLanguage)
                    {
                        case AppLanguage.english:
                            topText.text = "One more time!";
                            bottomText.text = "draw";
                            break;
                        case AppLanguage.chinese:
                            topText.text = "再一次！";
                            bottomText.text = "画";
                            break;
                        case AppLanguage.japanese:
                            topText.text = "もう１回！";
                            bottomText.text = "あいこでした";
                            break;
                    }
                }

                remainingTime -= Time.deltaTime;
                playerCircle.fillAmount = remainingTime / 2;
                opponentCircle.fillAmount = remainingTime / 2;

                if (remainingTime <= 0)
                {
                    playerHandIndex = PoseIndex.Fist;
                    remainingTime = 2.0f;
                    appStatus = AppStatus.game;
                }
            }
            else if (appStatus == AppStatus.game)
            {
                if (remainingTime == 2.0f)
                {
                    rpsAudioManager.PlaySoundEffect(RpsAudioManager.SoundEffect.jankenpon, appLanguage);
                }

                remainingTime -= Time.deltaTime;

                if (remainingTime >= 0)
                {
                    playerCircle.fillAmount = remainingTime / 2;
                    opponentCircle.fillAmount = remainingTime / 2;
                    ImageChange((int)playerHandIndex, 9);
                }

                if (remainingTime >= 1.0F)
                {
                    switch (appLanguage)
                    {
                        case AppLanguage.english:
                            topText.text = "One          ";
                            bottomText.text = "";
                            break;
                        case AppLanguage.chinese:
                            topText.text = "剪刀　　　　　";
                            bottomText.text = "";
                            break;
                        case AppLanguage.japanese:
                            topText.text = "じゃん　　　　　　";
                            bottomText.text = "";
                            break;
                    }
                }
                else if (remainingTime >= 0.0F)
                {
                    switch (appLanguage)
                    {
                        case AppLanguage.english:
                            topText.text = "One two      ";
                            bottomText.text = "";
                            break;
                        case AppLanguage.chinese:
                            topText.text = "剪刀　石头　　";
                            bottomText.text = "";
                            break;
                        case AppLanguage.japanese:
                            topText.text = "じゃん　けん　　　";
                            bottomText.text = "";
                            break;
                    }
                }
                else if (remainingTime >= -1.0F)
                {
                    switch (appLanguage)
                    {
                        case AppLanguage.english:
                            topText.text = "One two three";
                            bottomText.text = "";
                            break;
                        case AppLanguage.chinese:
                            topText.text = "剪刀　石头　布";
                            bottomText.text = "";
                            break;
                        case AppLanguage.japanese:
                            topText.text = "じゃん　けん　ぽん";
                            bottomText.text = "";
                            break;
                    }

                    opponentCircle.fillAmount = 0;
                    playerCircle.fillAmount = 0;

                    if (cpuHandIndex == 9)
                    {
                        System.Random random = new System.Random();
                        cpuHandIndex = random.Next(0, 3);
                        confirmPlayerHand = playerHandIndex;
                        ImageChange((int)confirmPlayerHand, cpuHandIndex);
                    }
                }
                else if (remainingTime < -1.0f)
                {
                    if ((confirmPlayerHand == PoseIndex.Fist && cpuHandIndex == 1) ||
                        (confirmPlayerHand == PoseIndex.Peace && cpuHandIndex == 2) ||
                        (confirmPlayerHand == PoseIndex.OpenPalm && cpuHandIndex == 0))
                    {
                        win = true;
                        cpuHandIndex = 9;
                        rpsAudioManager.PlaySoundEffect(RpsAudioManager.SoundEffect.win, appLanguage);
                        appStatus = AppStatus.result;
                        MsgSetting();
                        handModelLeft.SetActive(true);
                        handModelRight.SetActive(true);
                    }
                    else if ((confirmPlayerHand == PoseIndex.Fist && cpuHandIndex == 2) ||
                        (confirmPlayerHand == PoseIndex.Peace && cpuHandIndex == 0) ||
                        (confirmPlayerHand == PoseIndex.OpenPalm && cpuHandIndex == 1))
                    {
                        win = false;
                        cpuHandIndex = 9;
                        rpsAudioManager.PlaySoundEffect(RpsAudioManager.SoundEffect.lose, appLanguage);
                        appStatus = AppStatus.result;
                        MsgSetting();
                        handModelLeft.SetActive(true);
                        handModelRight.SetActive(true);
                    }
                    else if ((confirmPlayerHand == PoseIndex.Fist && cpuHandIndex == 0) ||
                        (confirmPlayerHand == PoseIndex.Peace && cpuHandIndex == 1) ||
                        (confirmPlayerHand == PoseIndex.OpenPalm && cpuHandIndex == 2))
                    {
                        cpuHandIndex = 9;
                        remainingTime = 2.0f;
                        appStatus = AppStatus.rematch;
                    }
                }
            }
        }

        /// <summary>
        /// Image settings
        /// </summary>
        public void ImageChange(int player, int cpu)
        {
            switch (player)
            {
                case 0:
                    playerHand.sprite = fist;
                    break;
                case 2:
                    playerHand.sprite = peace;
                    break;
                case 5:
                    playerHand.sprite = openPalm;
                    break;
                default:
                    playerHand.sprite = question;
                    break;
            }
            switch (cpu)
            {
                case 0:
                    opponentHand.sprite = fist;
                    break;
                case 1:
                    opponentHand.sprite = peace;
                    break;
                case 2:
                    opponentHand.sprite = openPalm;
                    break;
                default:
                    opponentHand.sprite = question;
                    break;
            }
        }

        /// <summary>
        /// Language setting
        /// </summary>
        /// <param name="index"></param>
        public void SettingLanguage(int index)
        {
            appLanguage = (AppLanguage)index;
            MsgSetting();
        }

        /// <summary>
        /// Set a message
        /// </summary>
        private void MsgSetting()
        {
            if (appStatus == AppStatus.stay)
            {
                switch (appLanguage)
                {
                    case AppLanguage.english:
                        topText.text = "Start with thumbs up";
                        bottomText.text = "";
                        break;
                    case AppLanguage.chinese:
                        topText.text = "从竖起大拇指开始";
                        bottomText.text = "";
                        break;
                    case AppLanguage.japanese:
                        topText.text = "サムズアップで開始します";
                        bottomText.text = "";
                        break;
                }
            }
            else if(appStatus == AppStatus.result)
            {
                if (win)
                {
                    switch (appLanguage)
                    {
                        case AppLanguage.english:
                            topText.text = "Play again with thumbs up";
                            bottomText.text = "You win";
                            break;
                        case AppLanguage.chinese:
                            topText.text = "竖起大拇指再玩一次";
                            bottomText.text = "你的胜利";
                            break;
                        case AppLanguage.japanese:
                            topText.text = "サムズアップでもう1度プレイ";
                            bottomText.text = "あなたの勝ち";
                            break;
                    }
                }
                else
                {
                    switch (appLanguage)
                    {
                        case AppLanguage.english:
                            topText.text = "Play again with thumbs up";
                            bottomText.text = "You lose";
                            break;
                        case AppLanguage.chinese:
                            topText.text = "竖起大拇指再玩一次";
                            bottomText.text = "你的失败";
                            break;
                        case AppLanguage.japanese:
                            topText.text = "サムズアップでもう1度プレイ";
                            bottomText.text = "あなたの負け";
                            break;
                    }
                }
            }
            switch (appLanguage)
            {
                case AppLanguage.english:
                    opponentText.text = "Opponent";
                    playerText.text = "Player";
                    break;
                case AppLanguage.chinese:
                    opponentText.text = "对手";
                    playerText.text = "播放器";
                    break;
                case AppLanguage.japanese:
                    opponentText.text = "相手";
                    playerText.text = "あなた";
                    break;
            }
        }
    }
}
