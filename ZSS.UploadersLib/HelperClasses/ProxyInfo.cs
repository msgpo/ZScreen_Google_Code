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

using System;
using System.ComponentModel;

namespace UploadersLib.HelperClasses
{
    [Serializable]
    public class ProxyInfo
    {
        public string UserName { get; set; }
        [PasswordPropertyText(true)]
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public Proxy ProxyType { get; set; }

        public ProxyInfo() { }

        public ProxyInfo(string username, string password, string host, int port)
        {
            this.UserName = username;
            this.Password = password;
            this.Host = host;
            this.Port = port;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}:{2} ({3})", this.UserName, this.Host, this.Port, this.ProxyType.ToString());
        }

        public string GetAddress()
        {
            return string.Format("{0}:{1}", this.Host, this.Port);
        }
    }
}