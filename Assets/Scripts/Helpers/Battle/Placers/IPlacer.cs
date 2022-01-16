using System;
using System.Collections.Generic;
using UnityEngine;

namespace OnlyWar.Helpers.Battle.Placers
{
    interface IArmyPlacer
    {
        Dictionary<BattleSquad, Vector2> PlaceSquads(IEnumerable<BattleSquad> squads);
    }
}
