/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;

public class SafeArea : MonoBehaviour
{
    private Rect currentRect;

    private void Start()
    {
        currentRect = Screen.safeArea;
        SetAnchor();
    }

    private void Update()
    {
        Check();
    }

    private void Check()
    {
        var safeArea = Screen.safeArea;

        if (currentRect != safeArea)
        {
            currentRect = safeArea;
            SetAnchor();
        }
    }

    private void SetAnchor()
    {
        var panel = GetComponent<RectTransform>();

        var anchorMin = currentRect.position;
        var anchorMax = currentRect.position + currentRect.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        panel.anchorMin = anchorMin;
        panel.anchorMax = anchorMax;
    }
}
