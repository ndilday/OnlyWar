using System.Collections.Generic;
using UnityEngine;

namespace OnlyWar.Scripts.Helpers.Battle.Placers
{
    interface IBattleSquadPlacer
    {
        Dictionary<BattleSquad, Vector2> PlaceSquads(IEnumerable<BattleSquad> squads);
    }
}
