/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

namespace TofArSettings.MarkRecog
{
    public class InfoMarkRecogStatus : MarkRecogInfo
    {
        UI.Info uiInfo;

        void Start()
        {
            uiInfo = GetComponent<UI.Info>();
        }

        void Update()
        {
            // Show
            uiInfo.InfoText = managerController.IsDrawing ? "Drawing" : "None";
        }
    }
}
