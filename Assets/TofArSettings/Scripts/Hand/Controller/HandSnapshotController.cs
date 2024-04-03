/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Hand;
using UnityEngine;
using TofAr.V0.Tof;
using System;
using TofAr.V0;
using SensCord;
using System.Linq;

namespace TofArSettings.Hand
{
    public class HandSnapshotController : RecordController
    {
        private RecognizeResultProperty handDataCopy;
        private bool isSaveData = false;
        private string rawPath, imgPath, csvPath;

        public override bool IsMultiple => false;

        protected override string Output()
        {
            return rawPath;
        }

        protected override void StopRecording()
        {
            Debug.Log("Stop Hand");
        }

        private void OnEnable()
        {
            TofArHandManager.OnFrameArrived += HandFrameArrived;
        }

        private void OnDisable()
        {
            TofArHandManager.OnFrameArrived -= HandFrameArrived;
        }

        protected override bool IsRecord()
        {
            var instance_t = TofArTofManager.Instance;
            var instance_h = TofArHandManager.Instance;

            if (instance_t.IsStreamActive && instance_h.IsStreamActive)
            {
                return true;
            }

            return false;
        }

        protected override bool Record()
        {
            isSaveData = true;

            bool rawSaved = false;
            bool csvSaved = false;
            bool imgSaved = false;

            // Use timestamp
            string dataPath = Application.persistentDataPath + "/handdata/";
            string timeStamp = DateTime.Now.ToString("yyyyMMdd-HHmmssfff");


            if (!System.IO.Directory.Exists(dataPath))
            {
                System.IO.Directory.CreateDirectory(dataPath);
            }


            rawPath = dataPath + timeStamp + ".raw";
            imgPath = dataPath + timeStamp + ".png";
            csvPath = dataPath + "joints.csv";


            try
            {
                var depthData = TofArTofManager.Instance.DepthData;
                var rawDepthData = depthData.Data;

                using (var depthFile = System.IO.File.Open(rawPath, System.IO.FileMode.Create))
                {
                    {
                        byte[] outarr = new byte[rawDepthData.Length * 2];
                        System.Buffer.BlockCopy(rawDepthData, 0, outarr, 0, outarr.Length);

                        depthFile.Write(outarr, 0, outarr.Length);
                    }

                    rawSaved = true;
                }


                if (TofArManager.Instance.RuntimeSettings.runMode == RunMode.MultiNode)
                {
                    ScreenCapture.CaptureScreenshot(dataPath + timeStamp + ".png");
                }
                else
                {
                    ScreenCapture.CaptureScreenshot("/handdata/" + timeStamp + ".png");
                }
                imgSaved = true;

                // get joint data
                SaveCsv(dataPath, timeStamp);

                csvSaved = true;

            }
            catch (System.IO.IOException e)
            {
                TofArManager.Logger.WriteLog(LogLevel.Debug, TofAr.V0.Utils.FormatException(e));
            }
            catch (ArgumentException e)
            {
                TofArManager.Logger.WriteLog(LogLevel.Debug, TofAr.V0.Utils.FormatException(e));
            }
            finally
            {
                isSaveData = false;

                var sb = new System.Text.StringBuilder();

                if (imgSaved)
                {
                    sb.AppendLine("Screenshot saved to: " + imgPath + "\n");
                }
                else
                {
                    sb.AppendLine("Failed to save screenshot\n");
                }
                if (rawSaved)
                {
                    sb.AppendLine("Raw depth data saved to: " + rawPath + "\n");
                }
                else
                {
                    sb.AppendLine("Failed to save raw depth data\n");
                }

                if (csvSaved)
                {
                    sb.AppendLine("CSV joint data saved to: " + csvPath + "\n");
                }
                else
                {
                    sb.AppendLine("Failed to save CSV joint data\n");
                }
                TofArManager.Logger.WriteLog(LogLevel.Debug, sb.ToString());
            }
            return rawSaved && imgSaved && csvSaved;
        }

        private void SaveCsv(string dataPath, string timeStamp)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (!System.IO.File.Exists(dataPath + "/joints.csv"))
            {
                sb.Append("Timestamp,");

                // Left
                foreach (var j in Enum.GetNames(typeof(HandPointIndex)))
                {
                    sb.Append("L_" + j.ToString() + "_X,");
                    sb.Append("L_" + j.ToString() + "_Y,");
                    sb.Append("L_" + j.ToString() + "_Z,");
                }

                // Right
                foreach (var j in Enum.GetNames(typeof(HandPointIndex)))
                {
                    sb.Append("R_" + j.ToString() + "_X,");
                    sb.Append("R_" + j.ToString() + "_Y,");
                    sb.Append("R_" + j.ToString() + "_Z,");
                }

                sb.AppendLine();
            }

            sb.Append(timeStamp + ",");


            var pointsLeft = handDataCopy?.featurePointsLeft;
            var pointsRight = handDataCopy?.featurePointsRight;

            if (pointsLeft != null && pointsLeft.Length > 0)
            {
                foreach (var p in pointsLeft)
                {
                    if (p.z <= 0)
                    {
                        sb.Append("0,0,0,");
                    }
                    else
                    {
                        sb.Append(p.x + "," + p.y + "," + p.z + ",");
                    }
                }
            }


            if (pointsRight != null && pointsRight.Length > 0)
            {
                foreach (var p in pointsRight)
                {
                    if (p.z <= 0)
                    {
                        sb.Append("0,0,0,");
                    }
                    else
                    {
                        sb.Append(p.x + "," + p.y + "," + p.z + ",");
                    }
                }
            }

            sb.AppendLine();

            System.IO.File.AppendAllText(csvPath, sb.ToString());
        }

        private void HandFrameArrived(object sender)
        {
            var manager = sender as TofArHandManager;
            if (manager == null)
            {
                return;
            }

            if (isSaveData)
            {
                return;
            }

            handDataCopy = new RecognizeResultProperty();
            handDataCopy.handStatus = manager.HandData.Data.handStatus;
            handDataCopy.featurePointsLeft = manager.HandData.Data.featurePointsLeft;
            handDataCopy.featurePointsRight = manager.HandData.Data.featurePointsRight;
        }

        protected override string GetLastRecording()
        {
            return null;
        }

    }
}
