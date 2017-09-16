/*
 *          Copyright (c) 2017 Rafael Almeida (ralms@ralms.net)
 *
 *                    EntityFrameworkCore.FirebirdSQL
 *
 * THIS MATERIAL IS PROVIDED AS IS, WITH ABSOLUTELY NO WARRANTY EXPRESSED
 * OR IMPLIED.  ANY USE IS AT YOUR OWN RISK.
 * 
 * Permission is hereby granted to use or copy this program
 * for any purpose,  provided the above notices are retained on all copies.
 * Permission to modify the code and to distribute modified code is granted,
 * provided the above notices are retained, and a notice that the code was
 * modified is included with the above copyright notice.
 *
 */

using System;
using System.Text.RegularExpressions;

namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class ServerVersion
    {
       
        public ServerVersion(string versionStr)
        {
            var version = ReVersion.Matches(versionStr);
            if (version.Count > 0)
                Version = Version.Parse(version[0].Value);
			else
			{
				throw new InvalidOperationException($"Unable to determine server version from version string '{versionStr}'." +
					$"Supported versions:{string.Join(", ", SupportedVersions)} ");
			}
        }

	    private static readonly string[] SupportedVersions = { "2.1", "2.5", "3.0", "4.0" };

		internal Regex ReVersion = new Regex(@"\d+\.\d+\.?(?:\d+)?");

		public readonly Version Version;

        public bool SupportIdentityIncrement => Version.Major >= 3;

		public int ObjectLengthName => Version.Major < 3 || Version.Major >= 4  ? 63 : 31;
    }

}
