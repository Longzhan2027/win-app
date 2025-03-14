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

using ProtonVPN.Core.Settings;
using ProtonVPN.Translations;

namespace ProtonVPN.Modals.Welcome
{
    public class PlusWelcomeModalViewModel : BaseModalViewModel
    {
        private readonly IAppSettings _appSettings;

        public PlusWelcomeModalViewModel(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public string Bullet1
        {
            get
            {
                int totalCountries = _appSettings.CountryCount;
                return string.Format(Translation.GetPlural("PlusWelcome_Bullet1", totalCountries), totalCountries);
            }
        }
    }
}