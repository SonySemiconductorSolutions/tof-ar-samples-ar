/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace TofArSettings.UI
{
    public class RenamePanel : Panel
    {
        [SerializeField]
        InputField inputName;

        [SerializeField]
        Button btnOk;

        [SerializeField]
        Button btnCancel;

        [SerializeField]
        Text txtError;

        private const string msgFileNameEmpty = "New directory name is empty";
        private const string msgFileNameInvalid = "New directory name contains invalid characters";
        private const string msgFileNameDuplicate = "A directory with that name already exists";

        public UnityEngine.Events.UnityAction<string> OnConfirmRename;

        void OnEnable()
        {
            btnOk.onClick.AddListener(Apply);
            btnCancel.onClick.AddListener(Cancel);
        }

        void OnDisable()
        {
            btnOk.onClick.RemoveListener(Apply);
            btnCancel.onClick.RemoveListener(Cancel);
        }

        private bool IsNameValid(string fileName)
        {
            Regex regex = new Regex(@"^([a-zA-Z0-9_-]+)$");
            var matches = regex.Matches(fileName);

            return matches.Count > 0;
        }

        public void Apply()
        {
            // check for valid file names 
            string fileName = inputName.text;

            if (string.IsNullOrEmpty(fileName))
            {
                // output error
                txtError.enabled = true;
                txtError.text = msgFileNameEmpty;

                return;
            }

            if (!IsNameValid(fileName))
            {
                // output error
                txtError.enabled = true;
                txtError.text = msgFileNameInvalid;

                return;
            }

            var directoryListProperty = TofAr.V0.TofArManager.Instance.GetProperty<TofAr.V0.DirectoryListProperty>();
            var root = directoryListProperty.path;

            string fileNameAbsolute = System.IO.Path.Combine(root, fileName);

            // check if file already exists
            if (System.IO.Directory.Exists(fileNameAbsolute))
            {
                // output error
                txtError.enabled = true;
                txtError.text = msgFileNameDuplicate;

                return;
            }

            txtError.enabled = false;
            OnConfirmRename?.Invoke(inputName.text);
        }

        public void Cancel()
        {
            OnConfirmRename?.Invoke(null);
        }

        public override void OpenPanel(bool closeOther = true)
        {
            inputName.text = "";
            txtError.enabled = false;

            base.OpenPanel(closeOther);
        }

        protected override void OnOpen()
        {
            inputName.ActivateInputField();
            inputName.Select();

            base.OnOpen();
        }
    }
}
