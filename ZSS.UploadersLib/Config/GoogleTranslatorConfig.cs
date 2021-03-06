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
using HelpersLib;
using UploadersLib.OtherServices;

namespace UploadersLib
{
    public class GoogleTranslatorConfig
    {
        #region I/O Methods

        public bool Write(string filePath)
        {
            return SettingsHelper.Save(this, filePath, SerializationType.Xml);
        }

        public static GoogleTranslatorConfig Read(string filePath)
        {
            return SettingsHelper.Load<GoogleTranslatorConfig>(filePath, SerializationType.Xml);
        }

        #endregion I/O Methods

        public string APIKey = "";
        public List<GoogleLanguage> GoogleLanguages;
        public string GoogleSourceLanguage = "en";
        public bool GoogleAutoDetectSource = true;
        public string GoogleTargetLanguage = "en";
        public string GoogleTargetLanguage2 = "?";
        public bool AutoTranslate = false;
        public int AutoTranslateLength = 20;
    }
}