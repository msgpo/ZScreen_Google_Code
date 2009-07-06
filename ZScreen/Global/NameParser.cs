﻿#region License Information (GPL v2)
/*
    ZScreen - A program that allows you to upload screenshots in one keystroke.
    Copyright (C) 2008-2009  Brandon Zimmerman

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
#endregion

using System;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace ZSS
{
    public enum ReplacementVariables
    {
        [Description("Title of active window")]
        t,
        [Description("Gets the current year")]
        y,
        [Description("Gets the current month")]
        mo,
        [Description("Gets the current day")]
        d,
        [Description("Gets the current hour")]
        h,
        [Description("Gets the current minute")]
        mi,
        [Description("Gets the current second")]
        s,
        [Description("Auto increment")]
        i,
        [Description("Gets AM/PM")]
        pm,
        [Description("Gets image width")]
        width,
        [Description("Gets image height")]
        height
    }

    public enum NameParserType
    {
        EntireScreen,
        ActiveWindow,
        Watermark
    }

    public class NameParserInfo
    {
        public string Pattern;

        private NameParserType type;
        public NameParserType Type
        {
            get { return type; }
            set
            {
                type = value;
                switch (type)
                {
                    case NameParserType.ActiveWindow:
                        Pattern = Program.conf.NamingActiveWindow;
                        break;
                    case NameParserType.EntireScreen:
                        Pattern = Program.conf.NamingEntireScreen;
                        break;
                    case NameParserType.Watermark:
                        Pattern = Program.conf.WatermarkText;
                        break;
                }
            }
        }

        public bool IsPreview;
        public Image Picture;

        public NameParserInfo(string pattern)
        {
            Pattern = pattern;
        }

        public NameParserInfo(NameParserType nameType)
        {
            Type = nameType;
        }
    }

    public static class NameParser
    {
        public const string Prefix = "%";

        public static string Convert(NameParserInfo nameParser)
        {
            if (string.IsNullOrEmpty(nameParser.Pattern)) return "";

            StringBuilder sb = new StringBuilder(nameParser.Pattern);

            if (!nameParser.IsPreview && sb.ToString().Contains("%i"))
            {
                Program.conf.AutoIncrement++;
            }

            string width = "", height = "";

            if (nameParser.Picture != null)
            {
                width = nameParser.Picture.Width.ToString();
                height = nameParser.Picture.Height.ToString();
            }

            sb = sb.Replace(ToString(ReplacementVariables.width), width);
            sb = sb.Replace(ToString(ReplacementVariables.height), height);

            DateTime dt = DateTime.Now;

            if (sb.ToString().Contains(ToString(ReplacementVariables.pm)))
            {
                sb = sb.Replace(ToString(ReplacementVariables.h), dt.Hour == 0 ? "12" :
                    ((dt.Hour > 12 ? AddZeroes(dt.Hour - 12) : AddZeroes(dt.Hour))));
            }
            else
            {
                sb = sb.Replace(ToString(ReplacementVariables.h), AddZeroes(dt.Hour));
            }

            if (nameParser.Type == NameParserType.ActiveWindow || nameParser.Type == NameParserType.Watermark)
            {
                string activeWindow = User32.GetWindowLabel();
                if (string.IsNullOrEmpty(activeWindow))
                {
                    activeWindow = "ZScreen";
                }
                sb = sb.Replace(ToString(ReplacementVariables.t), activeWindow);
            }
            else
            {
                sb = sb.Replace(ToString(ReplacementVariables.t), "");
            }

            sb = sb.Replace(ToString(ReplacementVariables.y), dt.Year.ToString())
                .Replace(ToString(ReplacementVariables.mo), AddZeroes(dt.Month))
                .Replace(ToString(ReplacementVariables.d), AddZeroes(dt.Day))
                .Replace(ToString(ReplacementVariables.mi), AddZeroes(dt.Minute))
                .Replace(ToString(ReplacementVariables.s), AddZeroes(dt.Second))
                .Replace(ToString(ReplacementVariables.i), AddZeroes(Program.conf.AutoIncrement, 4))
                .Replace(ToString(ReplacementVariables.pm), (dt.Hour >= 12 ? "PM" : "AM"));

            if (nameParser.Type == NameParserType.Watermark)
            {
                sb = sb.Replace("\\n", "\n");
            }

            //normalize the entire thing, allow only characters and digits
            //spaces become underscores, prevents possible problems
            if (nameParser.Type != NameParserType.Watermark) sb = Normalize(sb);

            return sb.ToString();
        }

        private static string AddZeroes(int number)
        {
            return AddZeroes(number, 2);
        }

        private static string AddZeroes(int number, int digits)
        {
            return number.ToString("d" + digits);
        }

        public static StringBuilder Normalize(StringBuilder sb)
        {
            StringBuilder temp = new StringBuilder("");

            foreach (char c in sb.ToString())
            {
                if (char.IsLetterOrDigit(c) || c == '.' || c == '-' || c == '_')
                    temp.Append(c);
                if (c == ' ')
                    temp.Append('_');
            }

            return temp;
        }

        private static string ToString(ReplacementVariables replacement)
        {
            return Prefix + replacement.ToString();
        }
    }
}