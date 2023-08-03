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

using System.Threading.Tasks;
using ProtonVPN.Core.Profiles;
using ProtonVPN.Core.Servers;
using ProtonVPN.Core.Service.Vpn;

namespace ProtonVPN.Vpn.Connectors
{
    public class GatewayConnector : BaseConnector
    {
        private readonly IProfileFactory _profileFactory;

        public GatewayConnector(IVpnManager vpnManager, IProfileFactory profileFactory)
            : base(vpnManager)
        {
            _profileFactory = profileFactory;
        }

        public async Task ConnectAsync(string gatewayName)
        {
            Profile profile = CreateProfile(gatewayName);
            await VpnManager.ConnectAsync(profile);
        }

        private Profile CreateProfile(string gatewayName)
        {
            Profile profile = _profileFactory.Create();
            profile.IsTemporary = true;
            profile.ProfileType = ProfileType.Fastest;
            profile.Features = Features.B2B;
            profile.GatewayName = gatewayName;
            return profile;
        }
    }
}
