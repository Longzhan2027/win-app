﻿/*
 * Copyright (c) 2023 Proton AG
 *
 * This file is part of ProtonVPN.
 *
 * ProtonVPN is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * ProtonVPN is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with ProtonVPN.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Text;
using ProtonVPN.Common.Configuration;
using ProtonVPN.Common.OS.Architecture;

namespace ProtonVPN.Api
{
    public class ApiAppVersion : IApiAppVersion
    {
        private const string DEVELOPMENT_SUFFIX = "-dev";

        private readonly IConfiguration _appConfig;

        public ApiAppVersion(IConfiguration appConfig)
        {
            _appConfig = appConfig;
        }

        public string Value()
        {
            StringBuilder sb = new();
            sb.Append($"{_appConfig.ApiClientId}@{GetVersion()}");
#if DEBUG
            sb.Append(DEVELOPMENT_SUFFIX);
#endif
            sb.Append($"+{OsArchitecture.Value}");
            return sb.ToString();
        }

        public string UserAgent()
        {
            return $"{_appConfig.UserAgent}/{GetVersion()} ({Environment.OSVersion}; {OsArchitecture.Value})";
        }

        private string GetVersion()
        {
            return _appConfig.AppVersion;
        }
    }
}