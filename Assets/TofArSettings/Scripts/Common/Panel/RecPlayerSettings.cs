/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using System.Linq;
using TofArSettings.Body;
using TofArSettings.Color;
using TofArSettings.Tof;
using TofArSettings.Face;
using UnityEngine;
using UnityEngine.Events;
using TofArSettings.Plane;
using TofArSettings.Segmentation;

namespace TofArSettings.UI
{
    public class RecPlayerSettings : SettingsBase
    {
        [Header("Use Component")]

        /// <summary>
        /// Use/do not use Color component
        /// </summary>
        [SerializeField]
        bool color = true;

        /// <summary>
        /// Use/do not use Tof component
        /// </summary>
        [SerializeField]
        bool tof = true;

        /// <summary>
        /// Use/do not use Hand component
        /// </summary>
        [SerializeField]
        bool hand = false;

        /// <summary>
        /// Use/do not use Segmentation component
        /// </summary>
        [SerializeField]
        bool segmentation = false;

        /// <summary>
        /// Use/do not use Body component
        /// </summary>
        [SerializeField]
        bool body = false;

        /// <summary>
        /// Use/do not use Face component
        /// </summary>
        [SerializeField]
        bool face = false;

        /// <summary>
        /// Use/do not use Plane component
        /// </summary>
        [SerializeField]
        bool plane = false;

        [Space]

        /// <summary>
        /// Match Tof and Color options
        /// </summary>
        [SerializeField]
        bool matchTofAndColor = true;

        /// <summary>
        /// Match Tof and Hand options
        /// </summary>
        [SerializeField]
        bool matchTofAndHand = true;

        /// <summary>
        /// Match Tof and Color options
        /// </summary>
        [SerializeField]
        bool matchTofAndBody = true;

        /// <summary>
        /// Match Tof and Color options
        /// </summary>
        [SerializeField]
        bool matchTofAndPlane = true;

        /// <summary>
        /// Match Color and Face options
        /// </summary>
        [SerializeField]
        bool matchColorAndFace = true;

        const string idleText = "Play";
        const string stopText = "Stop";
        const string pauseText = "Pause";
        const string unpauseText = "UnPause";

        public const string fnamePlayBackEstimate_tof = "Tof PlayBack Estimate";
        public const string fnamePlayBackEstimate_color = "Color PlayBack Estimate";

        UnityEvent addControllersAndPlayerSetsEvent = new UnityEvent();

        /// <summary>
        /// Data set required for playback
        /// </summary>
        struct PlayerSet
        {
            public RecPlayerController Controller;
            public ItemDropdown Item;
            public ItemDropdown.ChangeEvent ChangeFunc;
            public ItemDropdown.DeleteEvent DeleteFunc;
            public ItemDropdown.RenameEvent RenameFunc;

            public PlayerSet(RecPlayerController ctrl, ItemDropdown item,
                ItemDropdown.ChangeEvent changeFunc, ItemDropdown.DeleteEvent deleteFunc, ItemDropdown.RenameEvent renameFunc)
            {
                Controller = ctrl;
                Item = item;
                ChangeFunc = changeFunc;
                DeleteFunc = deleteFunc;
                RenameFunc = renameFunc;
            }
        }

        Dictionary<ComponentType, PlayerSet> playerSets =
            new Dictionary<ComponentType, PlayerSet>();
        ItemButton playBtn, stopBtn;

        protected override void Start()
        {
            // Set UI order
            uiOrder = new UnityAction[]
            {
                MakeUIFile,
                MakeUIPlay
            };

            if (color)
            {
                var colorCtrl = FindObjectOfType<ColorRecPlayerController>();
                controllers.Add(colorCtrl);
                playerSets.Add(ComponentType.Color, new PlayerSet(
                    colorCtrl, null, ChangeColorIndex, (idx) => DeleteEntry(idx, ComponentType.Color), (idx, newName) => RenameEntry(idx, newName, ComponentType.Color)));
            }

            if (tof)
            {
                var tofCtrl = FindObjectOfType<TofRecPlayerController>();
                controllers.Add(tofCtrl);
                playerSets.Add(ComponentType.Tof, new PlayerSet(
                    tofCtrl, null, ChangeTofIndex, (idx) => DeleteEntry(idx, ComponentType.Tof), (idx, newName) => RenameEntry(idx, newName, ComponentType.Tof)));
            }

            if (hand)
            {
                addControllersAndPlayerSetsEvent.Invoke();
            }

            if (segmentation)
            {
                var segmentationCtrl = FindObjectOfType<SegmentationRecPlayerController>();
                controllers.Add(segmentationCtrl);
                playerSets.Add(ComponentType.Segmentation, new PlayerSet(
                    segmentationCtrl, null, ChangeSegmentationIndex, (idx) => DeleteEntry(idx, ComponentType.Segmentation), (idx, newName) => RenameEntry(idx, newName, ComponentType.Segmentation)));
            }

            if (body)
            {
                var bodyCtrl = FindObjectOfType<BodyRecPlayerController>();
                controllers.Add(bodyCtrl);
                playerSets.Add(ComponentType.Body, new PlayerSet(
                    bodyCtrl, null, ChangeBodyIndex, (idx) => DeleteEntry(idx, ComponentType.Body), (idx, newName) => RenameEntry(idx, newName, ComponentType.Body)));
            }

            if (face)
            {
                var faceCtrl = FindObjectOfType<FaceRecPlayerController>();
                controllers.Add(faceCtrl);
                playerSets.Add(ComponentType.Face, new PlayerSet(
                    faceCtrl, null, ChangeFaceIndex, (idx) => DeleteEntry(idx, ComponentType.Face), (idx, newName) => RenameEntry(idx, newName, ComponentType.Face)));
            }

            if (plane)
            {
                var planeCtrl = FindObjectOfType<PlaneRecPlayerController>();
                controllers.Add(planeCtrl);
                playerSets.Add(ComponentType.Plane, new PlayerSet(
                    planeCtrl, null, ChangePlaneIndex, (idx) => DeleteEntry(idx, ComponentType.Plane), (idx, newName) => RenameEntry(idx, newName, ComponentType.Plane)));
            }

            for (int i = 0; i < controllers.Count; i++)
            {
                var ctrl = controllers[i] as RecPlayerController;
                if (ctrl != null)
                {
                    ctrl.OnChangeStatus += OnChangeRecStatus;
                }
            }

            base.Start();
        }

        /// <summary>
        /// Make file selection UI
        /// </summary>
        void MakeUIFile()
        {
            var keys = Enumerable.ToArray(playerSets.Keys);
            var runtimeMode = TofAr.V0.TofArManager.Instance.RuntimeSettings.runMode == TofAr.V0.RunMode.MultiNode;
            foreach (var key in keys)
            {
                var set = playerSets[key];
                var ctrl = set.Controller;
                if (ctrl.HasDropdown)
                {
                    string[] fileNames = ctrl.FileNames;

                    var fileNamesEditable = new KeyValuePair<string, EditFlags>[fileNames.Length];
                    for (int i = 0; i < fileNames.Length; i++)
                    {
                        fileNamesEditable[i] = new KeyValuePair<string, EditFlags>(fileNames[i], (i == 0) || runtimeMode || fileNames[i].Equals(fnamePlayBackEstimate_tof) || fileNames[i].Equals(fnamePlayBackEstimate_color) ? EditFlags.None : (EditFlags.Deletable | EditFlags.Renamable));
                    }

                    string titleName = key.ToString();

                    if (key == ComponentType.Segmentation)
                    {
                        titleName = "Seg";
                    }

                    var item = settings.AddItem(titleName, fileNamesEditable,
                        ctrl.Index, set.ChangeFunc, set.DeleteFunc, set.RenameFunc, -2, 100, 400);
                    set.Item = item;
                    playerSets[key] = set;

                    ctrl.OnChangeIndex += (index) =>
                    {
                        item.Index = index;
                    };

                    ctrl.OnUpdateFileNames += (list, index) =>
                    {
                        var optionsEditable = new KeyValuePair<string, EditFlags>[list.Length];
                        for (int i = 0; i < optionsEditable.Length; i++)
                        {
                            optionsEditable[i] = new KeyValuePair<string, EditFlags>(list[i], (i == 0) || runtimeMode || list[i].Equals(fnamePlayBackEstimate_tof) || list[i].Equals(fnamePlayBackEstimate_color) ? EditFlags.None : (EditFlags.Deletable | EditFlags.Renamable));
                        }
                        item.Index = index;
                        item.OptionsEditable = optionsEditable;
                    };
                }
            }
        }

        void DeleteEntry(int index, ComponentType componentType)
        {
            var fnames = playerSets[componentType].Item.Options;
            if (index < fnames.Length)
            {
                string fileName = fnames[index];

                playerSets[componentType].Controller.DeleteFile(fileName, index);
            }
        }

        void RenameEntry(int index, string newName, ComponentType componentType)
        {
            var fnames = playerSets[componentType].Item.Options;
            if (index < fnames.Length)
            {
                string fileName = fnames[index];

                playerSets[componentType].Controller.RenameFile(fileName, newName, index);
            }
        }

        /// <summary>
        /// Change Color file index
        /// </summary>
        /// <param name="index">Index</param>
        void ChangeColorIndex(int index)
        {
            var ctrl = playerSets[ComponentType.Color].Controller;

            if (matchTofAndColor && tof)
            {
                var fnames = playerSets[ComponentType.Color].Item.Options;
                if (index < fnames.Length)
                {
                    var splitName = fnames[index].Split('_');
                    var fname = splitName[0] + "_" + splitName[1];
                    var tofNames = playerSets[ComponentType.Tof].Item.Options;
                    for (int i = 0; i < tofNames.Length; i++)
                    {
                        if (tofNames[i].Contains(fname))
                        {
                            playerSets[ComponentType.Tof].Item.Index = i;
                            break;
                        }
                    }
                }
            }

            ctrl.Index = index;
        }

        /// <summary>
        /// Change Tof file index
        /// </summary>
        /// <param name="index">Index</param>
        void ChangeTofIndex(int index)
        {
            var ctrl = playerSets[ComponentType.Tof].Controller;

            if (matchTofAndColor && color)
            {
                var fnames = playerSets[ComponentType.Tof].Item.Options;
                if (index < fnames.Length)
                {
                    var splitName = fnames[index].Split('_');
                    var fname = splitName[0] + "_" + splitName[1];
                    var colorNames = playerSets[ComponentType.Color].Item.Options;
                    for (int i = 0; i < colorNames.Length; i++)
                    {
                        if (colorNames[i].Contains(fname))
                        {
                            playerSets[ComponentType.Color].Item.Index = i;
                            break;
                        }
                    }
                }
            }

            if (matchTofAndBody && body)
            {
                var fnames = playerSets[ComponentType.Tof].Item.Options;
                if (index < fnames.Length)
                {
                    var splitName = fnames[index].Split('_');
                    var fname = splitName[0] + "_" + splitName[1];
                    var bodyNames = playerSets[ComponentType.Body].Item.Options;
                    for (int i = 0; i < bodyNames.Length; i++)
                    {
                        if (bodyNames[i].Contains(fname))
                        {
                            playerSets[ComponentType.Body].Item.Index = i;
                            break;
                        }
                    }
                }
            }
            ctrl.Index = index;
        }

        /// <summary>
        /// Change Hand file index
        /// </summary>
        /// <param name="index">Index</param>
        void ChangeHandIndex(int index)
        {
            var ctrl = playerSets[ComponentType.Hand].Controller;

            if (matchTofAndHand && tof)
            {
                var fnames = playerSets[ComponentType.Hand].Item.Options;
                if (index < fnames.Length)
                {
                    var splitName = fnames[index].Split('_');
                    var fname = splitName[0] + "_" + splitName[1];
                    var tofNames = playerSets[ComponentType.Tof].Item.Options;
                    for (int i = 0; i < tofNames.Length; i++)
                    {
                        if (tofNames[i].Contains(fname))
                        {
                            playerSets[ComponentType.Tof].Item.Index = i;
                            break;
                        }
                    }
                }
            }
            ctrl.Index = index;
        }

        /// <summary>
        /// Change Segmentation file index
        /// </summary>
        /// <param name="index">Index</param>
        void ChangeSegmentationIndex(int index)
        {
            var ctrl = playerSets[ComponentType.Segmentation].Controller;
            ctrl.Index = index;
        }

        /// <summary>
        /// Change Body file index
        /// </summary>
        /// <param name="index">Index</param>
        void ChangeBodyIndex(int index)
        {
            var ctrl = playerSets[ComponentType.Body].Controller;

            if (matchTofAndBody && tof)
            {
                var fnames = playerSets[ComponentType.Body].Item.Options;
                if (index < fnames.Length)
                {
                    var splitName = fnames[index].Split('_');
                    var fname = splitName[0] + "_" + splitName[1];
                    var tofNames = playerSets[ComponentType.Tof].Item.Options;
                    for (int i = 0; i < tofNames.Length; i++)
                    {
                        if (tofNames[i].Contains(fname))
                        {
                            playerSets[ComponentType.Tof].Item.Index = i;
                            break;
                        }
                    }
                }
            }
            ctrl.Index = index;
        }

        /// <summary>
        /// Change Face file index
        /// </summary>
        /// <param name="index">Index</param>
        void ChangeFaceIndex(int index)
        {
            var ctrl = playerSets[ComponentType.Face].Controller;

            if (matchColorAndFace && face)
            {
                var fnames = playerSets[ComponentType.Face].Item.Options;
                if (index < fnames.Length)
                {
                    var splitName = fnames[index].Split('_');
                    var fname = splitName[0] + "_" + splitName[1];
                    var colorNames = playerSets[ComponentType.Color].Item.Options;
                    for (int i = 0; i < colorNames.Length; i++)
                    {
                        if (colorNames[i].Contains(fname))
                        {
                            playerSets[ComponentType.Color].Item.Index = i;
                            break;
                        }
                    }
                }
            }
            ctrl.Index = index;
        }

        /// <summary>
        /// Change Plane file index
        /// </summary>
        /// <param name="index">Index</param>
        void ChangePlaneIndex(int index)
        {
            var ctrl = playerSets[ComponentType.Plane].Controller;

            if (matchTofAndPlane && plane)
            {
                var fnames = playerSets[ComponentType.Plane].Item.Options;
                if (index < fnames.Length)
                {
                    var splitName = fnames[index].Split('_');
                    var fname = splitName[0] + "_" + splitName[1];
                    var tofNames = playerSets[ComponentType.Plane].Item.Options;
                    for (int i = 0; i < tofNames.Length; i++)
                    {
                        if (tofNames[i].Contains(fname))
                        {
                            playerSets[ComponentType.Plane].Item.Index = i;
                            break;
                        }
                    }
                }
            }
            ctrl.Index = index;
        }

        /// <summary>
        /// Event that is called when playback status is changed
        /// </summary>
        /// <param name="status">Playback status</param>
        void OnChangeRecStatus(RecPlayerController.PlayStatus status)
        {
            string text = string.Empty;

            switch (status)
            {
                case RecPlayerController.PlayStatus.Idle:
                    ChangeInteractable(true);
                    ToolbarNotSelect(false);
                    text = idleText;
                    break;

                case RecPlayerController.PlayStatus.Playing:
                    ChangeInteractable(false);
                    ToolbarNotSelect(true);
                    text = stopText;
                    break;

                case RecPlayerController.PlayStatus.Pause:
                    ChangeInteractable(true);
                    ToolbarNotSelect(false);
                    text = unpauseText;
                    break;
            }

            if (playBtn != null && !string.IsNullOrEmpty(text))
            {
                playBtn.Title = text;
            }
        }

        private void ToolbarNotSelect(bool state)
        {
            Toolbar toolbar = FindObjectOfType<Toolbar>();
            if (toolbar)
            {
                toolbar.SetNotSelect(state);
            }
        }

        /// <summary>
        /// Change interactability of file selection UI
        /// </summary>
        /// <param name="onOff">On/Off</param>
        void ChangeInteractable(bool onOff)
        {
            foreach (var playerSet in playerSets.Values)
            {
                if (playerSet.Item != null)
                {
                    playerSet.Item.Interactable = onOff;
                }
            }
        }

        /// <summary>
        /// Make play button
        /// </summary>
        void MakeUIPlay()
        {
            playBtn = settings.AddItem(idleText, PlayPause);
        }

        /// <summary>
        /// Play
        /// </summary>
        void PlayPause()
        {
            bool selectIndexCheck = false;

            foreach (var playerSet in playerSets.Values)
            {
                if (playerSet.Item != null && playerSet.Item.Index > 0)
                {
                    selectIndexCheck = true;
                    break;
                }
            }

            if (selectIndexCheck)
            {
                foreach (var playerSet in playerSets.Values)
                {
                    playerSet.Controller.PlayPrep();
                }
                foreach (var playerSet in playerSets.Values)
                {
                    if (playerSet.Controller.IsPriority)
                    {
                        playerSet.Controller.PlayPause();
                    }
                }
                foreach (var playerSet in playerSets.Values)
                {
                    if (!playerSet.Controller.IsPriority)
                    {
                        playerSet.Controller.PlayPause();
                    }
                }
                foreach (var playerSet in playerSets.Values)
                {
                    if (playerSet.Controller.IsPriority)
                    {
                        playerSet.Controller.StopCleanup();
                    }
                }
                foreach (var playerSet in playerSets.Values)
                {
                    if (!playerSet.Controller.IsPriority)
                    {
                        playerSet.Controller.StopCleanup();
                    }
                }
            }
        }

        /// <summary>
        /// Controller registration event
        /// </summary>
        /// <param name="unityAction">Argumentless delegate</param>
        public void AddListenerAddControllersAndPlayerSetsEvent(UnityAction unityAction)
        {
            addControllersAndPlayerSetsEvent.AddListener(unityAction);
        }

        /// <summary>
        /// Register controller
        /// </summary>
        /// <param name="controller">RecordController</param>
        public void SetController(ControllerBase controller)
        {
            controllers.Add(controller);
        }

        /// <summary>
        /// Register playerSets
        /// </summary>
        /// <param name="controller">RecordController</param>
        public void SetPlayerSets(ComponentType type, RecPlayerController ctrl)
        {
            if (type == ComponentType.Hand)
            {
                playerSets.Add(ComponentType.Hand, new PlayerSet(
                                    ctrl, null, ChangeHandIndex, (idx) => DeleteEntry(idx, ComponentType.Hand), (idx, newName) => RenameEntry(idx, newName, ComponentType.Hand)));
            }
        }
    }
}
