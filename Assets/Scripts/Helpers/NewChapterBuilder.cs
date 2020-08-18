
using System.Collections.Generic;
using System.Linq;
using Iam.Scripts.Models;

namespace Iam.Scripts.Helpers
{
    public static class NewChapterBuilder
    {
        public static Unit AssignSoldiersToChapter(Soldier[] soldiers, UnitTemplate rootTemplate, string year)
        {
            Dictionary<int, Soldier> unassignedSoldierMap = soldiers.ToDictionary(s => s.Id);
            Unit chapter = BuildUnitTreeFromTemplate(rootTemplate);

            // first, assign the Librarians
            AssignLibrarians(unassignedSoldierMap, chapter, year);
            // then, assign up to the top 50 as Techmarines
            AssignTechMarines(unassignedSoldierMap, chapter, year);
            // then, assign the top leader as Chapter Master
            AssignChapterMaster(unassignedSoldierMap, chapter, year);
            // then, assign Captains
            AssignCaptains(unassignedSoldierMap, chapter, year);
            // then, assigned twenty apothecaries
            AssignApothecaries(unassignedSoldierMap, chapter, year);
            // then, assign twenty Chaplains
            AssignChaplains(unassignedSoldierMap, chapter, year);
            // any dual gold awards are assigned to the first company
            AssignVeterans(unassignedSoldierMap, chapter, year);
            // assign all marines who didn't get bronze in either melee or ranged to the tenth company
            AssignScouts(unassignedSoldierMap, chapter, year);
            // assign Champtions to the CM and each Company
            AssignChampions(unassignedSoldierMap, chapter, year);
            // assign Ancients to the CM and each Company
            AssignAncients(unassignedSoldierMap, chapter, year);
            // assign all other marines who got at least bronze in one skill, starting with the second company
            AssignMarines(unassignedSoldierMap, chapter, year);
            //Assign excess to scouts
            AssignExcessToScouts(unassignedSoldierMap, chapter, year);
            return chapter;
        }

        private static void AssignChapterMaster(Dictionary<int, Soldier> unassignedSoldierMap, Unit chapter, string year)
        {
            var master = unassignedSoldierMap.Values.OrderByDescending(s => s.LeadershipScore).First();
            master.Rank = TempSpecialtyRanks.ChapterMaster;
            chapter.Members.Add(master);
            master.AssignedUnit = chapter;
            master.SoldierHistory.Add(year + ": voted by the chapter to become the first Chapter Master");
            unassignedSoldierMap.Remove(master.Id);
        }

        private static Unit BuildUnitTreeFromTemplate(UnitTemplate rootTemplate)
        {
            int i = 1;
            Unit root = rootTemplate.GenerateUnitFromTemplateWithoutChildren(0, "Heart of the Emperor");
            BuildUnitTreeHelper(root, rootTemplate, ref i);
            return root;
        }

        private static void BuildUnitTreeHelper(Unit rootUnit, UnitTemplate rootTemplate, ref int nextId)
        {
            string[] companyStrings = { "First", "Second", "Third", "Fourth", "Fifth", "Sixth", "Seventh", "Eighth", "Ninth", "Tenth" };
            int stringIndex = 0;
            foreach(UnitTemplate child in rootTemplate.ChildUnits)
            {
                string name;
                if(child.Name.Contains("Company"))
                {
                    name = companyStrings[stringIndex] + " Company";
                    stringIndex++;
                }
                else
                {
                    name = child.Name;
                }
                Unit newUnit = child.GenerateUnitFromTemplateWithoutChildren(nextId, name);
                nextId++;
                rootUnit.ChildUnits.Add(newUnit);
                BuildUnitTreeHelper(newUnit, child, ref nextId);
            }
        }

        private static void AssignLibrarians(Dictionary<int, Soldier> unassignedSoldierMap, Unit chapter, string year)
        {
            bool chief = false;
            // assume for now that there's a single unit to hold all of the Librarians
            var library = chapter.ChildUnits.First(u => u.UnitTemplate.Members.Contains(TempSpecialtyRanks.Lexicanius));
            var psychers = unassignedSoldierMap.Values.Where(s => s.PsychicAbility > 0).OrderByDescending(s => s.Ego);
            foreach (Soldier soldier in psychers)
            {
                if (soldier.Ego >= 25)
                {
                    if (!chief)
                    {
                        soldier.Rank = TempSpecialtyRanks.ChiefLibrarian;
                        chief = true;
                    }
                    else
                    {
                        soldier.Rank = TempSpecialtyRanks.Codicier;
                    }
                }
                else
                {
                    soldier.Rank = TempSpecialtyRanks.Lexicanius;
                }
                soldier.AssignedUnit = library;
                soldier.SoldierHistory.Add(year + ": Promoted to " + soldier.Rank.Name + " and assigned to " + soldier.AssignedUnit.Name);
                library.Members.Add(soldier);
                unassignedSoldierMap.Remove(soldier.Id);
            }
        }

        private static void AssignTechMarines(Dictionary<int, Soldier> unassignedSoldierMap, Unit chapter, string year)
        {
            var techMarines = unassignedSoldierMap.Values.Where(s => s.TechScore > 125).OrderByDescending(s => s.TechScore).Take(50);
            bool chief = false;
            // assume for now that there's a single unit to hold all of the Techmarines
            var armory = chapter.ChildUnits.First(u => u.UnitTemplate.Members.Contains(TempSpecialtyRanks.TechMarine));
            foreach (Soldier soldier in techMarines)
            {
                if (soldier.TechScore > 150 && !chief)
                {
                    soldier.Rank = TempSpecialtyRanks.ForgeMaster;
                    chief = true;
                }
                else
                {
                    soldier.Rank = TempSpecialtyRanks.TechMarine;
                }
                soldier.AssignedUnit = armory;
                soldier.SoldierHistory.Add(year + ": Returned from Mars, promoted to " + soldier.Rank.Name + " and assigned to " + soldier.AssignedUnit.Name);
                armory.Members.Add(soldier);
                unassignedSoldierMap.Remove(soldier.Id);
            }
        }

        private static void AssignApothecaries(Dictionary<int, Soldier> unassignedSoldierMap, Unit chapter, string year)
        {
            var apothecaries = unassignedSoldierMap.Values.Where(s => s.MedicalScore > 125).OrderByDescending(s => s.MedicalScore).Take(20);
            bool chief = false;
            // assume for now that there's a single unit to hold all of the Techmarines
            var apo = chapter.ChildUnits.First(u => u.UnitTemplate.Members.Contains(TempSpecialtyRanks.Apothecary));
            foreach (Soldier soldier in apothecaries)
            {
                if (soldier.MedicalScore > 150 && !chief)
                {
                    soldier.Rank = TempSpecialtyRanks.ChiefApothecary;
                    chief = true;
                }
                else
                {
                    soldier.Rank = TempSpecialtyRanks.Apothecary;
                }
                soldier.AssignedUnit = apo;
                soldier.SoldierHistory.Add(year + ": finished medical and genetic training, promoted to " + soldier.Rank.Name + " and assigned to " + soldier.AssignedUnit.Name);
                apo.Members.Add(soldier);
                unassignedSoldierMap.Remove(soldier.Id);
            }
        }

        private static void AssignChaplains(Dictionary<int, Soldier> unassignedSoldierMap, Unit chapter, string year)
        {
            var chaplains = unassignedSoldierMap.Values.Where(s => s.PietyScore > 125).OrderByDescending(s => s.PietyScore).Take(20);
            bool chief = false;
            // assume for now that there's a single unit to hold all of the Techmarines
            var reclusium = chapter.ChildUnits.First(u => u.UnitTemplate.Members.Contains(TempSpecialtyRanks.Chaplain));
            foreach (Soldier soldier in chaplains)
            {
                if (soldier.PietyScore > 150 && !chief)
                {
                    soldier.Rank = TempSpecialtyRanks.ChiefChaplain;
                    chief = true;
                }
                else
                {
                    soldier.Rank = TempSpecialtyRanks.Chaplain;
                }
                soldier.AssignedUnit = reclusium;
                soldier.SoldierHistory.Add(year + ": promoted to " + soldier.Rank.Name + " and assigned to " + soldier.AssignedUnit.Name);
                reclusium.Members.Add(soldier);
                unassignedSoldierMap.Remove(soldier.Id);
            }
        }

        private static void AssignCaptains(Dictionary<int, Soldier> unassignedSoldierMap, Unit chapter, string year)
        {
            var veteranLeaders = unassignedSoldierMap.Values.OrderByDescending(s => s.LeadershipScore).Take(20).ToList();
            foreach (Unit company in chapter.ChildUnits)
            {
                if (company.ChildUnits != null && company.ChildUnits.Count > 0
                    && company.UnitTemplate.Members.Contains(TempSpecialtyRanks.Captain) )
                {
                    // is a true company, needs a captain
                    AssignSoldier(unassignedSoldierMap, veteranLeaders, company, TempSpecialtyRanks.Captain, year);
                }
            }
        }
    
        private static void AssignVeterans(Dictionary<int, Soldier> unassignedSoldierMap, Unit chapter, string year)
        {
            var veterans = unassignedSoldierMap.Values.Where(s => s.MeleeScore > 1350 && s.RangedScore > 120);
            var veteranLeaders = veterans.Where(s => s.LeadershipScore > 110).OrderByDescending(s => s.LeadershipScore).ToList();
            // if there are no veteran sgts, leave First Company empty for now
            if (veteranLeaders.Count == 0) return;
            var vetList = veterans.Except(veteranLeaders).OrderByDescending(s => s.MeleeScore).ToList();
            int squadSize = (vetList.Count / veteranLeaders.Count) + 1;
            if(squadSize < 5)
            {
                // set squad size to five, and we'll use the other veteran sgts elsewhere
                squadSize = 5;
            }
            if(squadSize > 10)
            {
                // set squad size to ten, and we'll use the other veterans elsewhere
                squadSize = 10;
            }

            // look for Veteran Squads
            foreach(Unit company in chapter.ChildUnits)
            {
                foreach (Unit squad in company.ChildUnits)
                {
                    if (squad.UnitTemplate.Name == "Veteran Squad")
                    {
                        if (vetList.Count == 0 || veteranLeaders.Count == 0)
                        {
                            break;
                        }
                        // assign sgt to squad
                        squad.Name = veteranLeaders[0].LastName + " Squad";
                        AssignSoldier(unassignedSoldierMap, veteranLeaders, squad, TempSpecialtyRanks.VeteranSquadSergeant, year);


                        while (squad.Members.Count < squadSize && vetList.Count > 0)
                        {
                            AssignSoldier(unassignedSoldierMap, vetList, squad, TempSpecialtyRanks.VeteranMarine, year);
                        }
                    }
                }
            }
        }

        private static void AssignScouts(Dictionary<int, Soldier> unassignedSoldierMap, Unit chapter, string year)
        {
            var scouts = unassignedSoldierMap.Values.Where(s => s.MeleeScore <= 950 && s.RangedScore <= 95);
            var leaderList = unassignedSoldierMap.Values.Where(s => s.LeadershipScore > 110).OrderByDescending(s => s.LeadershipScore).Take(10).ToList();
            var scoutList = scouts.Except(leaderList).ToList();
            Unit lastSquad = null;
            foreach (Unit company in chapter.ChildUnits)
            {
                foreach (Unit squad in company.ChildUnits)
                {
                    if (squad.UnitTemplate.Name == "Scout Squad")
                    {
                        if (leaderList.Count == 0 || scoutList.Count == 0)
                        {
                            break;
                        }
                        lastSquad = squad;
                        // assign sgt to squad
                        squad.Name = leaderList[0].LastName + " Squad";
                        AssignSoldier(unassignedSoldierMap, leaderList, squad, TempSpecialtyRanks.ScoutSergeant, year);
                        int squadSize = CalculateSquadSize(scoutList, leaderList);
                        while (squad.Members.Count < 10 && scoutList.Count > 0)
                        {
                            AssignSoldier(unassignedSoldierMap, scoutList, squad, TempSpecialtyRanks.Scout, year);
                        }
                    }
                }
            }
            while (scoutList.Count > 0)
            {
                AssignSoldier(unassignedSoldierMap, scoutList, lastSquad, TempSpecialtyRanks.Scout, year);
            }
        }

        private static void AssignExcessToScouts(Dictionary<int, Soldier> unassignedSoldierMap, Unit chapter, string year)
        {
            Unit lastSquad = null;
            foreach (Unit company in chapter.ChildUnits)
            {
                foreach (Unit squad in company.ChildUnits)
                {
                    if (squad.UnitTemplate.Name == "Scout Squad")
                    {
                        lastSquad = squad;
                        while(squad.Members.Count <= 10 && unassignedSoldierMap.Count > 0)
                        {
                            AssignSoldier(unassignedSoldierMap, squad, TempSpecialtyRanks.Scout, year);
                        }
                    }
                }
            }
            while (unassignedSoldierMap.Count > 0)
            {
                AssignSoldier(unassignedSoldierMap, lastSquad, TempSpecialtyRanks.Scout, year);
            }
        }

        private static void AssignChampions(Dictionary<int, Soldier> unassignedSoldierMap, Unit chapter, string year)
        {
            var champions = unassignedSoldierMap.Values.OrderByDescending(s => s.MeleeScore).Take(20).ToList();
            // assign top to HQ
            AssignSoldier(unassignedSoldierMap, champions, chapter, TempSpecialtyRanks.ChapterChampion, year);
            foreach (Unit company in chapter.ChildUnits)
            {
                if (company.ChildUnits != null && company.ChildUnits.Count > 0
                    && company.UnitTemplate.Members.Contains(TempSpecialtyRanks.Champion) )
                {
                    // is a true company, needs a champion
                    AssignSoldier(unassignedSoldierMap, champions, company, TempSpecialtyRanks.Champion, year);
                }
            }
        }

        private static void AssignAncients(Dictionary<int, Soldier> unassignedSoldierMap, Unit chapter, string year)
        {
            var ancients = unassignedSoldierMap.Values.OrderByDescending(s => s.AncientScore).Take(20).ToList();
            // assign top to HQ
            AssignSoldier(unassignedSoldierMap, ancients, chapter, TempSpecialtyRanks.ChapterAncient, year);
            foreach (Unit company in chapter.ChildUnits)
            {
                if (company.ChildUnits != null && company.ChildUnits.Count > 0
                    && company.UnitTemplate.Members.Contains(TempSpecialtyRanks.Ancient) )
                {
                    // is a true company, needs a champion
                    AssignSoldier(unassignedSoldierMap, ancients, company, TempSpecialtyRanks.Ancient, year);
                }
            }
        }

        private static void AssignMarines(Dictionary<int, Soldier> unassignedSoldierMap, Unit chapter, string year)
        {
            var assList = unassignedSoldierMap.Values.Where(s => s.MeleeScore > 950 && s.RangedScore <= 95 && s.LeadershipScore <= 110).OrderByDescending(s => s.MeleeScore).ToList();
            var devList = unassignedSoldierMap.Values.Where(s => s.MeleeScore <= 950 && s.RangedScore > 95 && s.LeadershipScore <= 110).OrderByDescending(s => s.RangedScore).ToList();
            var tactList = unassignedSoldierMap.Values.Where(s => s.MeleeScore > 950 && s.RangedScore > 95 && s.LeadershipScore <= 110).ToList();
            var assSgtList = unassignedSoldierMap.Values.Where(s => s.MeleeScore > 950 && s.RangedScore <= 95 && s.LeadershipScore > 110).OrderByDescending(s => s.LeadershipScore).ToList();
            var devSgtList = unassignedSoldierMap.Values.Where(s => s.MeleeScore <= 950 && s.RangedScore > 95 && s.LeadershipScore > 110).OrderByDescending(s => s.LeadershipScore).ToList();
            var tactSgtList = unassignedSoldierMap.Values.Where(s => s.MeleeScore > 950 && s.RangedScore > 95 && s.LeadershipScore > 110).OrderByDescending(s => s.LeadershipScore).ToList();
            
            BalanceLists(assList, devList, tactList, assSgtList, devSgtList, tactSgtList);

            foreach (Unit company in chapter.ChildUnits)
            {
                foreach (Unit squad in company.ChildUnits)
                {
                    if (squad.UnitTemplate == TempUnitTemplates.Instance.TacticalSquadTemplate)
                    {

                        if (tactSgtList.Count > 0)
                        {
                            squad.Name = tactSgtList[0].LastName + " Squad";
                            AssignSoldier(unassignedSoldierMap, tactSgtList, squad, TempSpecialtyRanks.TacticalSergeant, year);
                        }
                        int tactSquadSize = CalculateSquadSize(tactList, tactSgtList);
                        while (tactList.Count > 0 && squad.Members.Count < tactSquadSize)
                        {
                            AssignSoldier(unassignedSoldierMap, tactList, squad, TempSpecialtyRanks.TacticalMarine, year);
                        }
                    }
                }
            }
            assList.AddRange(tactList);
            foreach (Unit company in chapter.ChildUnits)
            {
                foreach (Unit squad in company.ChildUnits)
                {
                    if (squad.UnitTemplate == TempUnitTemplates.Instance.AssaultSquadTemplate)
                    {
                        if (assSgtList.Count > 0)
                        {
                            squad.Name = assSgtList[0].LastName + " Squad";
                            AssignSoldier(unassignedSoldierMap, assSgtList, squad, TempSpecialtyRanks.AssaultSergeant, year);
                        }
                        int assSquadSize = CalculateSquadSize(assList, assSgtList);
                        while (assList.Count > 0 && squad.Members.Count < assSquadSize)
                        {
                            AssignSoldier(unassignedSoldierMap, assList, squad, TempSpecialtyRanks.AssaultMarine, year);
                        }
                    }
                }
            }
            if(assList.Count > 0)
            {
                devList.AddRange(assList.Where(a => a.RangedScore > 95));
                devList = devList.OrderByDescending(s => s.RangedScore).ToList();
            }
            foreach (Unit company in chapter.ChildUnits)
            {
                foreach (Unit squad in company.ChildUnits)
                {
                    if (squad.UnitTemplate == TempUnitTemplates.Instance.DevestatorSquadTemplate)
                    {
                        if (devSgtList.Count > 0)
                        {
                            squad.Name = devSgtList[0].LastName + " Squad";
                            AssignSoldier(unassignedSoldierMap, devSgtList, squad, TempSpecialtyRanks.DevestatorSergeant, year);
                        }
                        int devSquadSize = CalculateSquadSize(devList, devSgtList);
                        while (devList.Count > 0 && squad.Members.Count < devSquadSize)
                        {
                            AssignSoldier(unassignedSoldierMap, devList, squad, TempSpecialtyRanks.DevestatorMarine, year);
                        }
                    }
                }
            }
        }

        private static int CalculateSquadSize(List<Soldier> soldierList, List<Soldier> sgtList)
        {
            if ((soldierList.Count - 9) >= (sgtList.Count * 4))
            {
                return 10;
            }
            else if (sgtList.Count == 0)
            {
                return soldierList.Count + 1;
            }
            else
            {
                return soldierList.Count - (sgtList.Count * 4) + 1;
            }
        }

        private static void AssignSoldier(Dictionary<int, Soldier> unassignedSoldierMap, List<Soldier> soldierList, Unit squad, SpecialtyRank rank, string year)
        {
            var soldier = soldierList[0];
            soldier.Rank = rank;
            soldier.AssignedUnit = squad;
            soldier.SoldierHistory.Add(year + ": promoted to " + soldier.Rank.Name + " and assigned to " + soldier.AssignedUnit.Name);
            squad.Members.Add(soldier);
            unassignedSoldierMap.Remove(soldier.Id);
            soldierList.RemoveAt(0);
        }

        private static void AssignSoldier(Dictionary<int, Soldier> unassignedSoldierMap, Unit squad, SpecialtyRank rank, string year)
        {
            var soldier = unassignedSoldierMap.Values.First();
            soldier.Rank = rank;
            soldier.AssignedUnit = squad;
            soldier.SoldierHistory.Add(year + ": promoted to " + soldier.Rank.Name + " and assigned to " + soldier.AssignedUnit.Name);
            squad.Members.Add(soldier);
            unassignedSoldierMap.Remove(soldier.Id);
        }

        private static void BalanceLists(List<Soldier> assList, List<Soldier> devList, List<Soldier> tactList, List<Soldier> assSgtList, List<Soldier> devSgtList, List<Soldier> tactSgtList)
        {
            // we want 18 squads of 9 devs each, 18 squads of 9 ass each, and 52 squads of 9 tact each, so 162, 162, and 468
            while(assSgtList.Count > 18)
            {
                var soldier = assSgtList[assSgtList.Count - 1];
                assList.Add(soldier);
                assSgtList.RemoveAt(assSgtList.Count - 1);
            }
            while(devSgtList.Count > 18)
            {
                var soldier = devSgtList[devSgtList.Count - 1];
                devList.Add(soldier);
                devSgtList.RemoveAt(devSgtList.Count - 1);
            }
            if(assSgtList.Count == 18 && devSgtList.Count == 18)
            {
                while(tactSgtList.Count > 52)
                {
                    var soldier = tactSgtList[tactSgtList.Count - 1];
                    tactList.Add(soldier);
                    tactSgtList.RemoveAt(tactSgtList.Count - 1);
                }
            }
            int minAssSgtNeeded = (assList.Count - 1) + 1;
            int minDevSgtNeeded = (devList.Count - 1) + 1;
            int minTactSgtNeeded = (tactList.Count - 1) + 1;

            int spareTactSgt = tactSgtList.Count - minTactSgtNeeded;
            int spareAssSgt = assSgtList.Count - minAssSgtNeeded;
            int spareDevSgt = devSgtList.Count - minDevSgtNeeded;
            while (spareTactSgt > 0 && spareDevSgt < 0)
            {
                // shift a Tact Sgt to be a Dev Sgt
                devSgtList.Add(tactSgtList[0]);
                spareDevSgt++;
                tactSgtList.RemoveAt(0);
                spareTactSgt--;
            }
            while (spareTactSgt > 0 && spareAssSgt < 0)
            {
                // shift a Tact Sgt to be an Ass Sgt
                assSgtList.Add(tactSgtList[0]);
                spareAssSgt++;
                tactSgtList.RemoveAt(0);
                spareTactSgt--;
            }
            if (spareTactSgt < 0 && (spareAssSgt > 0 || spareDevSgt > 0))
            {
                // shift tact marines to be other marines
                while (spareTactSgt < 0 && spareAssSgt > 0)
                {
                    // move nine tactMarines to be assMarines
                    for (int i = 0; i < 9; i++)
                    {
                        assList.Add(tactList[0]);
                        tactList.RemoveAt(0);
                    }
                    spareTactSgt++;
                    spareAssSgt--;
                }
                while (spareTactSgt < 0 && spareDevSgt > 0)
                {
                    // move nine tactMarines to be assMarines
                    for (int i = 0; i < 9; i++)
                    {
                        devList.Add(tactList[0]);
                        tactList.RemoveAt(0);
                    }
                    spareTactSgt++;
                    spareDevSgt--;
                }
            }
        }
    }
}
