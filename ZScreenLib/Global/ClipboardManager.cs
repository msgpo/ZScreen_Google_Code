﻿#region License Information (GPL v2)

/*
    ZScreen - A program that allows you to upload screenshots in one keystroke.
    Copyright (C) 2008-2011 ZScreen Developers

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v2)

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GraphicsMgrLib;
using UploadersLib;
using UploadersLib.HelperClasses;
using ZScreenLib.Properties;

// Last working class that supports multiple screenshots histories:
// http://code.google.com/p/zscreen/source/browse/trunk/ZScreen/Global/ClipboardManager.cs?spec=svn550&r=550

namespace ZScreenLib
{
    /// <summary>
    /// Class reponsible for Adding or Retrieving Clipboard Text and Setting Text to Clipboard
    /// </summary>
    public static class ClipboardManager
    {
        public static List<UploadInfo> UploadInfoList = new List<UploadInfo>();

        private static ImageFileManager LinkMgr = new ImageFileManager();
        private static int UniqueNumber = 0;

        /// <summary>
        /// Function to be called when a new Worker thread starts
        /// </summary>
        public static int Queue()
        {
            int number = UniqueNumber++;
            UploadInfoList.Add(new UploadInfo(number));
            return number;
        }

        public static void Clear()
        {
            UploadInfoList.Clear();
        }

        /// <summary>
        /// Remove Last Screenshot from Clipboard Manager after setting to Clipboard
        /// </summary>
        public static bool Commit(int number)
        {
            UploadInfo find = GetInfo(number);
            if (find != null)
            {
                return UploadInfoList.Remove(find);
            }
            return false;
        }

        public static UploadInfo GetInfo(int number)
        {
            return UploadInfoList.Find(x => x.UniqueID == number);
        }

        public static ImageFileManager GetLastImageUpload()
        {
            return LinkMgr;
        }

        public static int GetAverageProgress()
        {
            return UploadInfoList.Sum(x => x.UploadPercentage) / UploadInfoList.Count;
        }

        /// <summary>
        /// Sets Clipboard text and returns the content
        /// </summary>
        /// <returns></returns>
        public static void SetClipboard(WorkerTask task, bool showDialog)
        {
            string clipboardText = "";

            if (task.JobIsImageToClipboard())
            {
                Clipboard.SetImage(task.MyImage);
            }
            else if (task.WasImageToFile())
            {
                clipboardText = task.LocalFilePath;
            }
            else
            {
                switch (task.Job1)
                {
                    case JobLevel1.Images:
                        LinkMgr = task.LinkManager;
                        if (Engine.conf.ShowClipboardModeChooser || showDialog)
                        {
                            ClipboardOptions cmp = new ClipboardOptions(task);
                            cmp.Icon = Resources.zss_main;
                            if (showDialog) { cmp.ShowDialog(); } else { cmp.Show(); }
                        }
                        else
                        {
                            if (task.MakeTinyURL)
                            {
                                string tinyUrl = LinkMgr.GetUrlByType(ClipboardUriType.FULL_TINYURL);
                                if (!string.IsNullOrEmpty(tinyUrl))
                                {
                                    clipboardText = tinyUrl.Trim();
                                }
                            }
                            else
                            {
                                clipboardText = LinkMgr.GetUrlByType(Engine.conf.ClipboardUriMode).ToString().Trim();
                            }
                        }
                        break;

                    case JobLevel1.Binary:
                        clipboardText = task.RemoteFilePath;
                        break;

                    case JobLevel1.Text:
                        switch (task.Job2)
                        {
                            case WorkerTask.JobLevel2.LANGUAGE_TRANSLATOR:
                                if (task.TranslationInfo != null)
                                {
                                    clipboardText = task.TranslationInfo.Result;
                                }
                                break;
                            default:
                                if (!string.IsNullOrEmpty(task.RemoteFilePath))
                                {
                                    clipboardText = task.RemoteFilePath;
                                }
                                else if (null != task.MyText)
                                {
                                    clipboardText = task.MyText;
                                }
                                else
                                {
                                    clipboardText = task.LocalFilePath;
                                }
                                break;
                        }
                        break;
                }

                if (!string.IsNullOrEmpty(clipboardText))
                {
                    Engine.ClipboardUnhook();
                    FileSystem.AppendDebug("Setting Clipboard with URL: " + clipboardText);
                    Clipboard.SetText(clipboardText); // auto

                    // optional deletion link
                    string linkdel = LinkMgr.UploadResult.DeletionURL;
                    if (!string.IsNullOrEmpty(linkdel))
                    {
                        FileSystem.AppendDebug("Deletion Link: " + linkdel);
                    }

                    Engine.zClipboardText = clipboardText;
                    Engine.ClipboardHook(); // This is for Clipboard Monitoring - we resume monitoring the clipboard
                }
            }
        }
    }
}