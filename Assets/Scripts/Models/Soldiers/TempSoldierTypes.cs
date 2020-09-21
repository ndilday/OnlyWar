﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iam.Scripts.Models.Soldiers
{
    public sealed class TempSoldierTypes
    {
        private static TempSoldierTypes _instance;

        public static TempSoldierTypes Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new TempSoldierTypes();
                }
                return _instance;
            }
        }

        public IReadOnlyDictionary<int, SoldierType> SpaceMarineSoldierTypes { get; }
        public IReadOnlyDictionary<int, SoldierType> TyranidSoldierTypes { get; }

        private TempSoldierTypes()
        {
            SpaceMarineSoldierTypes = new List<SoldierType>
            {
                new SoldierType(1, "Chapter Master", true, 6),
                new SoldierType(2, "Master of the Apothecarion", true, 19),
                new SoldierType(3, "Master of the Forge", true, 29),
                new SoldierType(4, "Master Techmarine", true, 28),
                new SoldierType(5, "Techmarine", true, 27),
                new SoldierType(6, "Master of the Librarium", true, 39),
                new SoldierType(7, "Epistolary", true, 38),
                new SoldierType(8, "Codiciers", true, 37),
                new SoldierType(9, "Lexicanium", true, 36),
                new SoldierType(10, "Master of Sanctity", true, 49),
                new SoldierType(11, "Reclusiarch", false, 48),
                new SoldierType(12, "Captain", true, 5),
                new SoldierType(13, "Sergeant", true, 4),
                new SoldierType(14, "Ancient", false, 4),
                new SoldierType(15, "Champion", false, 4),
                new SoldierType(16, "Veteran", false, 4),
                new SoldierType(17, "Chaplain", false, 47),
                new SoldierType(18, "Apothecary", false, 18),
                new SoldierType(19, "Tactical Marine", false, 3),
                new SoldierType(20, "Assault Marine", false, 2),
                new SoldierType(21, "Devastator Marine", false, 1),
                new SoldierType(22, "Scout Marine", false, 0),
            }.ToDictionary(st => st.Id);

            TyranidSoldierTypes = new List<SoldierType>
            {
                new SoldierType(101, "Hive Tyrant", true, 100),
                new SoldierType(102, "Tyranid Warrior", false, 50),
                new SoldierType(103, "Genestealer", false, 20),
                new SoldierType(104, "Termagaunt", false, 1),
                new SoldierType(105, "Hormagaunt", false, 3)
            }.ToDictionary(st => st.Id);
        }
    }
}
