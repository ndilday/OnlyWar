
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Units;

namespace Iam.Scripts.Helpers
{
    public static class NewChapterBuilder
    {
        public static Chapter AssignSoldiersToChapter(IEnumerable<SpaceMarine> marines, UnitTemplate rootTemplate, string year)
        {
            Dictionary<int, SpaceMarine> unassignedSoldierMap = marines.ToDictionary(s => s.Id);
            Chapter chapter = BuildUnitTreeFromTemplate(rootTemplate);

            // first, assign the Librarians
            AssignLibrarians(unassignedSoldierMap, chapter.OrderOfBattle, year);
            // then, assign up to the top 50 as Techmarines
            AssignTechMarines(unassignedSoldierMap, chapter.OrderOfBattle, year);
            // then, assign the top leader as Chapter Master
            AssignChapterMaster(unassignedSoldierMap, chapter.OrderOfBattle, year);
            // then, assign Captains
            AssignCaptains(unassignedSoldierMap, chapter.OrderOfBattle, year);
            // then, assigned twenty apothecaries
            AssignApothecaries(unassignedSoldierMap, chapter.OrderOfBattle, year);
            // then, assign twenty Chaplains
            AssignChaplains(unassignedSoldierMap, chapter.OrderOfBattle, year);
            // any dual gold awards are assigned to the first company
            AssignVeterans(unassignedSoldierMap, chapter.OrderOfBattle, year);
            // assign Champtions to the CM and each Company
            AssignChampions(unassignedSoldierMap, chapter.OrderOfBattle, year);
            // assign Ancients to the CM and each Company
            AssignAncients(unassignedSoldierMap, chapter.OrderOfBattle, year);
            // assign all other marines who got at least bronze in one skill, starting with the second company
            AssignMarines(unassignedSoldierMap, chapter.OrderOfBattle, year);
            //Assign excess to scouts
            AssignExcessToScouts(unassignedSoldierMap, chapter.OrderOfBattle, year);
            return chapter;
        }

        private static Chapter BuildUnitTreeFromTemplate(UnitTemplate rootTemplate)
        {
            int i = 1;
            Chapter root = new Chapter(rootTemplate.GenerateUnitFromTemplateWithoutChildren(-1, "Heart of the Emperor"));
            BuildUnitTreeHelper(root.OrderOfBattle, rootTemplate, ref i);
            return root;
        }

        private static void BuildUnitTreeHelper(Unit rootUnit, UnitTemplate rootTemplate, ref int nextId)
        {
            string[] companyStrings = { "First", "Second", "Third", "Fourth", "Fifth", "Sixth", "Seventh", "Eighth", "Ninth", "Tenth" };
            int stringIndex = 0;
            foreach (UnitTemplate child in rootTemplate.GetChildUnits())
            {
                string name;
                if (child.Name.Contains("Company"))
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
                newUnit.ParentUnit = rootUnit;
                BuildUnitTreeHelper(newUnit, child, ref nextId);
            }
        }

        private static void AssignChapterMaster(Dictionary<int, SpaceMarine> unassignedSoldierMap, Unit chapter, string year)
        {
            var master = unassignedSoldierMap.Values.OrderByDescending(s => s.LeadershipScore).First();
            master.Rank = TempSpaceMarineRanks.ChapterMaster;
            chapter.HQSquad.SquadLeader = master;
            master.AssignedSquad = chapter.HQSquad;
            master.SoldierHistory.Add(year + ": voted by the chapter to become the first Chapter Master");
            unassignedSoldierMap.Remove(master.Id);

            master.AddSkillPoints(TempBaseSkillList.Instance.Leadership, 4);
            master.AddSkillPoints(TempBaseSkillList.Instance.Tactics, 4);
            master.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 4);
            master.AddSkillPoints(TempBaseSkillList.Instance.Leadership, 4);
            master.AddSkillPoints(TempBaseSkillList.Instance.Marine, 4);
            master.AddSkillPoints(TempBaseSkillList.Instance.Sword, 4);
        }

        private static void AssignLibrarians(Dictionary<int, SpaceMarine> unassignedSoldierMap, Unit chapter, string year)
        {
            // assume for now that there's a single unit to hold all of the Librarians as a squad on the chapter
            var library = chapter.Squads.First(s => s.Name == "Librarius");
            var psychers = unassignedSoldierMap.Values.Where(s => s.PsychicPower > 0).OrderByDescending(s => s.Ego);
            // TODO: add 24 points
            foreach (SpaceMarine soldier in psychers)
            {
                if (soldier.Ego >= 18)
                {
                    if (library.SquadLeader == null)
                    {
                        soldier.Rank = TempSpaceMarineRanks.ChiefLibrarian;
                        library.SquadLeader = soldier;
                    }
                    else
                    {
                        soldier.Rank = TempSpaceMarineRanks.Codicier;
                        library.Members.Add(soldier);

                    }
                }
                else
                {
                    soldier.Rank = TempSpaceMarineRanks.Lexicanius;
                    library.Members.Add(soldier);
                }
                soldier.AssignedSquad = library;
                soldier.SoldierHistory.Add(year + ": Promoted to " + soldier.Rank.Name + " and assigned to " + soldier.AssignedSquad.Name);
                unassignedSoldierMap.Remove(soldier.Id);
            }
        }

        private static void AssignTechMarines(Dictionary<int, SpaceMarine> unassignedSoldierMap, Unit chapter, string year)
        {
            var techMarines = unassignedSoldierMap.Values.Where(s => s.TechScore > 100).OrderByDescending(s => s.TechScore).Take(50);
            // assume for now that there's a single unit to hold all of the Techmarines
            var armory = chapter.Squads.First(s => s.Name == "Armory");
            foreach (SpaceMarine soldier in techMarines)
            {
                soldier.AddSkillPoints(TempBaseSkillList.Instance.MachineGod, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.ArmoryForceShield, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.ArmoryPowerArmor, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.ArmoryVehicle, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.ArmorySmallArms, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.RhinoMechanic, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.LandSpeederMechanic, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.LandRaiderMechanic, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.LandRaider, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.ServoArm, 2);
                if (soldier.TechScore > 150 && armory.SquadLeader == null)
                {
                    soldier.Rank = TempSpaceMarineRanks.ForgeMaster;
                    armory.SquadLeader = soldier;
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Leadership, 2);
                }
                else
                {
                    soldier.Rank = TempSpaceMarineRanks.TechMarine;
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.GunneryBeam, 2);
                    armory.Members.Add(soldier);
                }
                soldier.AssignedSquad = armory;
                soldier.SoldierHistory.Add(year + ": Returned from Mars, promoted to " + soldier.Rank.Name + " and assigned to " + soldier.AssignedSquad.Name);
                unassignedSoldierMap.Remove(soldier.Id);
            }
        }

        private static void AssignApothecaries(Dictionary<int, SpaceMarine> unassignedSoldierMap, Unit chapter, string year)
        {
            var apothecaries = unassignedSoldierMap.Values.Where(s => s.MedicalScore > 80).OrderByDescending(s => s.MedicalScore).Take(20);
            // assume for now that there's a single unit to hold all of the Techmarines
            var apo = chapter.Squads.First(s => s.Name == "Apothecarion");
            foreach (SpaceMarine soldier in apothecaries)
            {
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Diagnosis, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Pharmacy, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Physician, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Surgery, 4);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Bike, 2);
                if (soldier.MedicalScore > 150 && apo.SquadLeader == null)
                {
                    soldier.Rank = TempSpaceMarineRanks.ChiefApothecary;
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Leadership, 2);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 1);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, 1);
                    apo.SquadLeader = soldier;
                }
                else
                {
                    soldier.Rank = TempSpaceMarineRanks.Apothecary;
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 2);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, 2);
                    apo.Members.Add(soldier);
                }
                soldier.AssignedSquad = apo;
                soldier.SoldierHistory.Add(year + ": finished medical and genetic training, promoted to " + soldier.Rank.Name + " and assigned to " + soldier.AssignedSquad.Name);
                unassignedSoldierMap.Remove(soldier.Id);
            }
        }

        private static void AssignChaplains(Dictionary<int, SpaceMarine> unassignedSoldierMap, Unit chapter, string year)
        {
            var chaplains = unassignedSoldierMap.Values.Where(s => s.PietyScore > 110).OrderByDescending(s => s.PietyScore).Take(20);
            // assume for now that there's a single unit to hold all of the Techmarines
            var reclusium = chapter.Squads.First(s => s.Name == "Reclusium");
            foreach (SpaceMarine soldier in chaplains)
            {
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Piety, 4);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Ritual, 4);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Axe, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.JumpPack, 2);

                if (soldier.PietyScore > 11 && reclusium.SquadLeader == null)
                {
                    soldier.Rank = TempSpaceMarineRanks.ChiefChaplain;
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Leadership, 2);
                    reclusium.SquadLeader = soldier;
                }
                else
                {
                    soldier.Rank = TempSpaceMarineRanks.Chaplain;
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Axe, 2);
                    reclusium.Members.Add(soldier);
                }
                soldier.AssignedSquad = reclusium;
                soldier.SoldierHistory.Add(year + ": promoted to " + soldier.Rank.Name + " and assigned to " + soldier.AssignedSquad.Name);
                unassignedSoldierMap.Remove(soldier.Id);
            }
        }

        private static void AssignCaptains(Dictionary<int, SpaceMarine> unassignedSoldierMap, Unit chapter, string year)
        {
            SpaceMarine soldier;
            // see if there is an impressive enough leader to be the Veteran Captain
            var veteranLeaders = unassignedSoldierMap.Values.Where(s => s.LeadershipScore > 235 && s.MeleeScore > 600 && s.RangedScore > 75).OrderByDescending(s => s.LeadershipScore).ToList();
            if (veteranLeaders.Count > 0)
            {
                var firstCompany = chapter.ChildUnits.First(u => u.Name == "First Company");
                soldier = AssignSoldier(unassignedSoldierMap, veteranLeaders, firstCompany.HQSquad, TempSpaceMarineRanks.VeteranCaptain, true, year);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Leadership, 4);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Tactics, 4);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Bike, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.JumpPack, 2);
            }
            var leaders = unassignedSoldierMap.Values.OrderByDescending(s => s.LeadershipScore).Take(20).ToList();
            // assign Recruitment Captain next
            // assuming Tenth Company for now
            var tenthCompany = chapter.ChildUnits.First(u => u.Name == "Tenth Company");
            soldier = AssignSoldier(unassignedSoldierMap, leaders, tenthCompany.HQSquad, TempSpaceMarineRanks.RecruitmentCaptain, true, year);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Leadership, 4);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Persuade, 2);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Teaching, 4);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Tactics, 4);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 2);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 2);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, 2);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bike, 2);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.JumpPack, 2);

            foreach (Unit company in chapter.ChildUnits)
            {
                if (company.HQSquad.SquadLeader == null && company.Name != "First Company" )
                {
                    // is a true company, needs a captain
                    soldier = AssignSoldier(unassignedSoldierMap, leaders, company.HQSquad, TempSpaceMarineRanks.Captain, true, year);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Leadership, 4);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Tactics, 4);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 2);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, 2);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Bike, 2);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.JumpPack, 2);
                }
            }
        }
    
        private static void AssignVeterans(Dictionary<int, SpaceMarine> unassignedSoldierMap, Unit chapter, string year)
        {
            var veterans = unassignedSoldierMap.Values.Where(s => s.MeleeScore > 600 && s.RangedScore > 75);
            var veteranLeaders = veterans.Where(s => s.LeadershipScore > 235).OrderByDescending(s => s.LeadershipScore).ToList();
            // if there are no veteran sgts, leave First Company empty for now
            if (veteranLeaders.Count == 0) return;
            var vetList = veterans.Except(veteranLeaders).OrderByDescending(s => s.MeleeScore).ToList();
            int squadSize = (vetList.Count / veteranLeaders.Count) + 1;
            if(squadSize < 4)
            {
                // set squad size to five, and we'll use the other veteran sgts elsewhere
                squadSize = 4;
            }
            if(squadSize > 9)
            {
                // set squad size to ten, and we'll use the other veterans elsewhere
                squadSize = 9;
            }

            SpaceMarine soldier;
            // look for Veteran Squads
            foreach(Unit company in chapter.ChildUnits)
            {
                foreach (Squad squad in company.Squads)
                {
                    if (squad.SquadTemplate == TempSpaceMarineSquadTemplates.Instance.VeteranSquadTemplate)
                    {
                        if (vetList.Count == 0 || veteranLeaders.Count == 0)
                        {
                            break;
                        }
                        // assign sgt to squad
                        squad.Name = veteranLeaders[0].LastName + " Squad";
                        soldier = AssignSoldier(unassignedSoldierMap, veteranLeaders, squad, TempSpaceMarineRanks.VeteranSquadSergeant, true, year);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.Leadership, 4);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.Tactics, 2);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 2);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 2);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.Bike, 2);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.JumpPack, 2);
                        soldier.GetBestSkillByCategory(SkillCategory.Melee).AddPoints(2);

                        while (squad.Members.Count < squadSize && vetList.Count > 0)
                        {
                            soldier = AssignSoldier(unassignedSoldierMap, vetList, squad, TempSpaceMarineRanks.VeteranMarine, false, year);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 4);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 4);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bike, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.JumpPack, 2);
                            soldier.GetBestSkillByCategory(SkillCategory.Melee).AddPoints(4);
                        }
                    }
                }
            }
        }

        private static void AssignExcessToScouts(Dictionary<int, SpaceMarine> unassignedSoldierMap, Unit chapter, string year)
        {
            int sgtNeed = ((unassignedSoldierMap.Count - 1) / 10) + 1;
            var leaderList = unassignedSoldierMap.Values.OrderByDescending(s => s.LeadershipScore).Take(sgtNeed).ToList();
            var scoutList = unassignedSoldierMap.Values.Except(leaderList).ToList();
            Unit lastCompany = null;
            Squad lastSquad = null;
            SpaceMarine soldier = null;
            foreach (Unit company in chapter.ChildUnits)
            {
                lastCompany = company;
                foreach (Squad squad in company.Squads)
                {
                    if (squad.SquadTemplate == TempSpaceMarineSquadTemplates.Instance.ScoutSquadTemplate)
                    {
                        lastSquad = squad;
                        // assign sgt to squad
                        squad.Name = leaderList[0].LastName + " Squad";
                        soldier = AssignSoldier(unassignedSoldierMap, leaderList, squad, TempSpaceMarineRanks.ScoutSergeant, true, year);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.Leadership, 4);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.Teaching, 4);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.Tactics, 2);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 2);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 2);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.Sniper, 1);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.Shotgun, 1);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, 2);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 2);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.Bike, 2);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.Stealth, 2);

                        while (squad.Members.Count < 9 && scoutList.Count > 0)
                        {
                            soldier = AssignSoldier(unassignedSoldierMap, scoutList, squad, TempSpaceMarineRanks.Scout, false, year);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 4);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bike, 4);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Stealth, 4);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Sniper, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Shotgun, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.LandSpeeder, 2);
                        }
                    }
                }
            }
            while (scoutList.Count > 0)
            {
                int id = lastSquad.Id + 1;
                // add a new Scout Squad to the company
                Squad squad = new Squad(id, "Scout Squad", TempSpaceMarineSquadTemplates.Instance.ScoutSquadTemplate);
                lastCompany.Squads.Add(squad);
                id++;
                lastSquad = squad;
                // assign sgt to squad
                squad.Name = leaderList[0].LastName + " Squad";
                soldier = AssignSoldier(unassignedSoldierMap, leaderList, squad, TempSpaceMarineRanks.ScoutSergeant, true, year);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Leadership, 4);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Teaching, 4);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Tactics, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 4);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Bike, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Stealth, 2);

                //int squadSize = CalculateSquadSize(scoutList, leaderList);
                while (squad.Members.Count < 9 && scoutList.Count > 0)
                {
                    soldier = AssignSoldier(unassignedSoldierMap, scoutList, squad, TempSpaceMarineRanks.Scout, false, year);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 4);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 2);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, 2);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 2);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Bike, 4);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Stealth, 4);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Sniper, 2);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Shotgun, 2);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.LandSpeeder, 2);
                }
            }
            if (unassignedSoldierMap.Count > 0) Debug.WriteLine("Still did it wrong");
        }

        private static void AssignChampions(Dictionary<int, SpaceMarine> unassignedSoldierMap, Unit chapter, string year)
        {
            var champions = unassignedSoldierMap.Values.OrderByDescending(s => s.MeleeScore).Take(20).ToList();
            // assign top to HQ
            SpaceMarine soldier = AssignSoldier(unassignedSoldierMap, champions, chapter.HQSquad, TempSpaceMarineRanks.ChapterChampion, false, year);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 4);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 4);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, 4);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bike, 4);

            foreach (Unit company in chapter.ChildUnits)
            {
                if (company.Name != "Tenth Company" && company.Name != "First Company")
                {
                    // is a true company, needs a champion
                    soldier = AssignSoldier(unassignedSoldierMap, champions, company.HQSquad, TempSpaceMarineRanks.Champion, false, year);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 4);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 4);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, 4);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Bike, 4);
                }
            }
        }

        private static void AssignAncients(Dictionary<int, SpaceMarine> unassignedSoldierMap, Unit chapter, string year)
        {
            var ancients = unassignedSoldierMap.Values.OrderByDescending(s => s.AncientScore).Take(20).ToList();
            // assign top to HQ
            SpaceMarine soldier = AssignSoldier(unassignedSoldierMap, ancients, chapter.HQSquad, TempSpaceMarineRanks.ChapterAncient, false, year);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 8);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, 4);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Throwing, 4);
            foreach (Unit company in chapter.ChildUnits)
            {
                if (company.Name != "Tenth Company" && company.Name != "First Company")
                {
                    // is a true company, needs a champion
                    soldier = AssignSoldier(unassignedSoldierMap, ancients, company.HQSquad, TempSpaceMarineRanks.Ancient, false, year);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 8);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, 4);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Throwing, 4);
                }
            }
        }

        private static void AssignMarines(Dictionary<int, SpaceMarine> unassignedSoldierMap, Unit chapter, string year)
        {
            var assList = unassignedSoldierMap.Values.Where(s => s.MeleeScore > 400 && s.RangedScore <= 60 && s.LeadershipScore <= 160).OrderByDescending(s => s.MeleeScore).ToList();
            var devList = unassignedSoldierMap.Values.Where(s => s.MeleeScore <= 400 && s.RangedScore > 60 && s.LeadershipScore <= 160).OrderByDescending(s => s.RangedScore).ToList();
            var tactList = unassignedSoldierMap.Values.Where(s => s.MeleeScore > 400 && s.RangedScore > 60 && s.LeadershipScore <= 160).ToList();
            var assSgtList = unassignedSoldierMap.Values.Where(s => s.MeleeScore > 400 && s.RangedScore <= 60 && s.LeadershipScore > 160).OrderByDescending(s => s.LeadershipScore).ToList();
            var devSgtList = unassignedSoldierMap.Values.Where(s => s.MeleeScore <= 400 && s.RangedScore > 60 && s.LeadershipScore > 160).OrderByDescending(s => s.LeadershipScore).ToList();
            var tactSgtList = unassignedSoldierMap.Values.Where(s => s.MeleeScore > 400 && s.RangedScore > 60 && s.LeadershipScore > 160).OrderByDescending(s => s.LeadershipScore).ToList();

            BalanceLists(assList, devList, tactList, assSgtList, devSgtList, tactSgtList);
            AssignTacticalMarines(unassignedSoldierMap, chapter, year, tactList, tactSgtList);
            assList.AddRange(tactList);
            AssignAssaultMarines(unassignedSoldierMap, chapter, year, assList, assSgtList);
            if (assList.Count > 0)
            {
                devList.AddRange(assList.Where(a => a.RangedScore > 60));
                devList = devList.OrderByDescending(s => s.RangedScore).ToList();
            }
            AssignDevastatorMarines(unassignedSoldierMap, chapter, year, devList, devSgtList);
        }

        private static void AssignDevastatorMarines(Dictionary<int, SpaceMarine> unassignedSoldierMap, Unit chapter, string year, List<SpaceMarine> devList, List<SpaceMarine> devSgtList)
        {
            SpaceMarine soldier;
            // since Devastators are assigned last, make sure the dev to sgt list is reasonable
            while(devSgtList.Count() * 4 > devList.Count())
            {
                // turn the last Sgt into a dev
                var demote = devSgtList[devSgtList.Count - 1];
                devList.Add(demote);
                devSgtList.Remove(demote);
            }
            foreach (Unit company in chapter.ChildUnits)
            {
                foreach (Squad squad in company.Squads)
                {
                    if (squad.SquadTemplate == TempSpaceMarineSquadTemplates.Instance.DevastatorSquadTemplate)
                    {
                        if (devSgtList.Count > 0)
                        {
                            squad.Name = devSgtList[0].LastName + " Squad";
                            soldier = AssignSoldier(unassignedSoldierMap, devSgtList, squad, TempSpaceMarineRanks.DevastatorSergeant, true, year);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Leadership, 4);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Tactics, 4);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 4);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 4);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
                        }
                        int devSquadSize = CalculateSquadSize(devList, devSgtList);
                        while (devList.Count > 0 && squad.Members.Count < devSquadSize)
                        {
                            soldier = AssignSoldier(unassignedSoldierMap, devList, squad, TempSpaceMarineRanks.DevastatorMarine, false, year);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 4);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 3);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Lascannon, 3);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Plasma, 3);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.MissileLauncher, 3);
                        }
                    }
                }
            }
        }

        private static void AssignAssaultMarines(Dictionary<int, SpaceMarine> unassignedSoldierMap, Unit chapter, string year, List<SpaceMarine> assList, List<SpaceMarine> assSgtList)
        {
            foreach (Unit company in chapter.ChildUnits)
            {
                foreach (Squad squad in company.Squads)
                {
                    if (squad.SquadTemplate == TempSpaceMarineSquadTemplates.Instance.AssaultSquadTemplate)
                    {
                        if (assSgtList.Count > 0)
                        {
                            squad.Name = assSgtList[0].LastName + " Squad";
                            SpaceMarine soldier = AssignSoldier(unassignedSoldierMap, assSgtList, squad, TempSpaceMarineRanks.AssaultSergeant, true, year);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Leadership, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Tactics, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bike, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.JumpPack, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Explosives, 1);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Throwing, 1);
                        }
                        int assSquadSize = CalculateSquadSize(assList, assSgtList);
                        while (assList.Count > 0 && squad.Members.Count < assSquadSize)
                        {
                            SpaceMarine soldier = AssignSoldier(unassignedSoldierMap, assList, squad, TempSpaceMarineRanks.AssaultMarine, false, year);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Flamer, 1);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Plasma, 1);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bike, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.JumpPack, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Throwing, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Shield, 2);
                        }
                    }
                }
            }
        }

        private static void AssignTacticalMarines(Dictionary<int, SpaceMarine> unassignedSoldierMap, Unit chapter, string year, List<SpaceMarine> tactList, List<SpaceMarine> tactSgtList)
        {
            foreach (Unit company in chapter.ChildUnits)
            {
                foreach (Squad squad in company.Squads)
                {
                    if (squad.SquadTemplate == TempSpaceMarineSquadTemplates.Instance.TacticalSquadTemplate)
                    {

                        if (tactSgtList.Count > 0)
                        {
                            squad.Name = tactSgtList[0].LastName + " Squad";
                            SpaceMarine soldier = AssignSoldier(unassignedSoldierMap, tactSgtList, squad, TempSpaceMarineRanks.TacticalSergeant, true, year);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Leadership, 4);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Tactics, 4);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 4);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, 2);
                        }
                        int tactSquadSize = CalculateSquadSize(tactList, tactSgtList);
                        while (tactList.Count > 0 && squad.Members.Count < tactSquadSize)
                        {
                            SpaceMarine soldier = AssignSoldier(unassignedSoldierMap, tactList, squad, TempSpaceMarineRanks.TacticalMarine, false, year);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 4);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 4);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Plasma, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Lascannon, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.MissileLauncher, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Flamer, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
                        }
                    }
                }
            }
        }

        private static int CalculateSquadSize(List<SpaceMarine> soldierList, List<SpaceMarine> sgtList)
        {
            if ((soldierList.Count - 9) >= (sgtList.Count * 4))
            {
                return 9;
            }
            else if (sgtList.Count == 0)
            {
                return soldierList.Count;
            }
            else
            {
                return soldierList.Count - (sgtList.Count * 4);
            }
        }

        private static SpaceMarine AssignSoldier(Dictionary<int, SpaceMarine> unassignedSoldierMap, List<SpaceMarine> soldierList, Squad squad, SpaceMarineRank rank, bool isLeader, string year)
        {
            var soldier = soldierList[0];
            soldier.Rank = rank;
            soldier.AssignedSquad = squad;
            soldier.SoldierHistory.Add(year + ": promoted to " + soldier.Rank.Name + " and assigned to " + soldier.AssignedSquad.Name);
            if (isLeader && squad.SquadLeader == null)
            {
                squad.SquadLeader = soldier;
            }
            else if(isLeader)
            {
                throw new ArgumentException("Squad " + squad.Name + " already has a leader assigned");
            }
            else
            {
                squad.Members.Add(soldier);
            }
            unassignedSoldierMap.Remove(soldier.Id);
            soldierList.RemoveAt(0);
            return soldier;
        }

        private static void BalanceLists(List<SpaceMarine> assList, List<SpaceMarine> devList, List<SpaceMarine> tactList, 
            List<SpaceMarine> assSgtList, List<SpaceMarine> devSgtList, List<SpaceMarine> tactSgtList)
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
