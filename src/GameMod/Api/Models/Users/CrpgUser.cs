﻿using System;
using System.Collections.Generic;
using Crpg.GameMod.Api.Models.Characters;

namespace Crpg.GameMod.Api.Models.Users
{
    // Copy of Crpg.Application.Games.Models.GameUser
    internal class CrpgUser
    {
        public int Id { get; set; }
        public string PlatformUserId { get; set; } = string.Empty;
        public int Gold { get; set; }
        public CrpgCharacter Character { get; set; } = default!;
        public IList<CrpgUserBrokenItem> BrokenItems { get; set; } = Array.Empty<CrpgUserBrokenItem>();
        public CrpgBan? Ban { get; set; }
    }
}