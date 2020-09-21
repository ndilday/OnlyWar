
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Squads;
using Iam.Scripts.Models.Units;
using UnityEditor;

namespace Iam.Scripts.Helpers
{
    public static class NewChapterBuilder
    {
        private delegate void TrainingFunction(PlayerSoldier playerSoldier);
        public static Chapter AssignSoldiersToChapter(IEnumerable<PlayerSoldier> soldiers, UnitTemplate rootTemplate, string year)
        {
            Dictionary<int, PlayerSoldier> unassignedSoldierMap = soldiers.ToDictionary(s => s.Id);
            Chapter chapter = BuildChapterFromUnitTemplate(rootTemplate, soldiers);

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
            var champions = unassignedSoldierMap.Values.OrderByDescending(s => s.MeleeRating).ToList();
            SoldierType championType = TempSoldierTypes.Instance.SpaceMarineSoldierTypes[15];
            AssignSpecialistsToUnit(unassignedSoldierMap, chapter.OrderOfBattle, year, championType, champions, TrainChampion);

            // assign Ancients to the CM and each Company
            var ancients = unassignedSoldierMap.Values.OrderByDescending(s => s.AncientRating).ToList();
            SoldierType ancientType = TempSoldierTypes.Instance.SpaceMarineSoldierTypes[14];
            AssignSpecialistsToUnit(unassignedSoldierMap, chapter.OrderOfBattle, year, ancientType, ancients, TrainAncient);
            
            // assign all other soldiers who got at least bronze in one skill, starting with the second company
            AssignMarines(unassignedSoldierMap, chapter.OrderOfBattle, year);
            //Assign excess to scouts
            AssignExcessToScouts(unassignedSoldierMap, chapter.OrderOfBattle, year);
            return chapter;
        }

        private static Chapter BuildChapterFromUnitTemplate(UnitTemplate rootTemplate, IEnumerable<PlayerSoldier> soldiers)
        {
            int i = 1;
            Chapter chapter = new Chapter(rootTemplate.GenerateUnitFromTemplateWithoutChildren(-1, "Heart of the Emperor"), soldiers);
            BuildUnitTreeHelper(chapter.OrderOfBattle, rootTemplate, ref i);
            return chapter;
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

        private static void AssignChapterMaster(Dictionary<int, PlayerSoldier> unassignedSoldierMap, Unit chapter, string year)
        {
            var master = unassignedSoldierMap.Values.OrderByDescending(s => s.LeadershipRating).First();
            master.Type = TempSoldierTypes.Instance.SpaceMarineSoldierTypes[1];
            chapter.HQSquad.Members.Add(master);
            master.AssignToSquad(chapter.HQSquad);
            master.AddEntryToHistory(year + ": voted by the chapter to become the first Chapter Master");
            unassignedSoldierMap.Remove(master.Id);

            master.AddSkillPoints(TempBaseSkillList.Instance.Leadership, 4);
            master.AddSkillPoints(TempBaseSkillList.Instance.Tactics, 4);
            master.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 4);
            master.AddSkillPoints(TempBaseSkillList.Instance.Leadership, 4);
            master.AddSkillPoints(TempBaseSkillList.Instance.Marine, 4);
            master.AddSkillPoints(TempBaseSkillList.Instance.Sword, 4);
        }

        private static void AssignLibrarians(Dictionary<int, PlayerSoldier> unassignedSoldierMap, Unit chapter, string year)
        {
            // assume for now that there's a single unit to hold all of the Librarians as a squad on the chapter
            var library = chapter.Squads.First(s => s.Name == "Librarius");
            var psychers = unassignedSoldierMap.Values.Where(s => s.PsychicPower > 0).OrderByDescending(s => s.Ego);
            // TODO: add 24 points
            foreach (PlayerSoldier soldier in psychers)
            {
                if (soldier.Ego >= 18)
                {
                    if (library.SquadLeader == null)
                    {
                        soldier.Type = TempSoldierTypes.Instance.SpaceMarineSoldierTypes[6];
                    }
                    else
                    {
                        soldier.Type = TempSoldierTypes.Instance.SpaceMarineSoldierTypes[8];
                    }
                }
                else
                {
                    soldier.Type = TempSoldierTypes.Instance.SpaceMarineSoldierTypes[9];
                }
                library.Members.Add(soldier);
                soldier.AssignToSquad(library);
                soldier.AddEntryToHistory(year + ": Promoted to " + soldier.Type.Name + " and assigned to " + soldier.AssignedSquad.Name);
                unassignedSoldierMap.Remove(soldier.Id);
            }
        }

        private static void AssignTechMarines(Dictionary<int, PlayerSoldier> unassignedSoldierMap, Unit chapter, string year)
        {
            var techMarines = unassignedSoldierMap.Values.Where(s => s.TechRating > 100).OrderByDescending(s => s.TechRating).Take(50);
            // assume for now that there's a single unit to hold all of the Techmarines
            var armory = chapter.Squads.First(s => s.Name == "Armory");
            foreach (PlayerSoldier soldier in techMarines)
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
                if (soldier.TechRating > 150 && armory.SquadLeader == null)
                {
                    soldier.Type = TempSoldierTypes.Instance.SpaceMarineSoldierTypes[3];
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Leadership, 2);
                }
                else
                {
                    soldier.Type = TempSoldierTypes.Instance.SpaceMarineSoldierTypes[5];
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.GunneryBeam, 2);
                }
                armory.Members.Add(soldier);
                soldier.AssignToSquad(armory);
                soldier.AddEntryToHistory(year + ": Returned from Mars, promoted to " 
                    + soldier.Type.Name + " and assigned to " + soldier.AssignedSquad.Name);
                unassignedSoldierMap.Remove(soldier.Id);
            }
        }

        private static void AssignApothecaries(Dictionary<int, PlayerSoldier> unassignedSoldierMap, Unit chapter, string year)
        {
            var apothecaries = unassignedSoldierMap.Values.Where(s => s.MedicalRating > 80).OrderByDescending(s => s.MedicalRating).Take(20);
            // assume for now that there's a single unit to hold all of the Techmarines
            var apo = chapter.Squads.First(s => s.Name == "Apothecarion");
            foreach (PlayerSoldier soldier in apothecaries)
            {
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Diagnosis, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Pharmacy, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Physician, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Surgery, 4);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Bike, 2);
                if (soldier.MedicalRating > 150 && apo.SquadLeader == null)
                {
                    soldier.Type = TempSoldierTypes.Instance.SpaceMarineSoldierTypes[2];
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Leadership, 2);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 1);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, 1);
                }
                else
                {
                    soldier.Type = TempSoldierTypes.Instance.SpaceMarineSoldierTypes[18];
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 2);
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, 2);
                }
                apo.Members.Add(soldier);
                soldier.AssignToSquad(apo);
                soldier.AddEntryToHistory(year + ": finished medical and genetic training, promoted to " 
                    + soldier.Type.Name + " and assigned to " + soldier.AssignedSquad.Name);
                unassignedSoldierMap.Remove(soldier.Id);
            }
        }

        private static void AssignChaplains(Dictionary<int, PlayerSoldier> unassignedSoldierMap, Unit chapter, string year)
        {
            var chaplains = unassignedSoldierMap.Values.Where(s => s.PietyRating > 110).OrderByDescending(s => s.PietyRating).Take(20);
            // assume for now that there's a single unit to hold all of the Techmarines
            var reclusium = chapter.Squads.First(s => s.Name == "Reclusium");
            foreach (PlayerSoldier soldier in chaplains)
            {
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Piety, 4);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Ritual, 4);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Axe, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.JumpPack, 2);

                if (soldier.PietyRating > 11 && reclusium.SquadLeader == null)
                {
                    soldier.Type = TempSoldierTypes.Instance.SpaceMarineSoldierTypes[10];
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Leadership, 2);
                }
                else
                {
                    soldier.Type = TempSoldierTypes.Instance.SpaceMarineSoldierTypes[17];
                    soldier.AddSkillPoints(TempBaseSkillList.Instance.Axe, 2);
                }
                reclusium.Members.Add(soldier);
                soldier.AssignToSquad(reclusium);
                soldier.AddEntryToHistory(year + ": promoted to " + soldier.Type.Name 
                    + " and assigned to " + soldier.AssignedSquad.Name);
                unassignedSoldierMap.Remove(soldier.Id);
            }
        }

        private static void AssignCaptains(Dictionary<int, PlayerSoldier> unassignedSoldierMap, Unit chapter, string year)
        {
            PlayerSoldier soldier;
            // see if there is an impressive enough leader to be the Veteran Captain
            var veteranLeaders = unassignedSoldierMap.Values.Where(s => s.LeadershipRating > 235 && s.MeleeRating > 600 && s.RangedRating > 75).OrderByDescending(s => s.LeadershipRating).ToList();
            if (veteranLeaders.Count > 0)
            {
                var firstCompany = chapter.ChildUnits.First(u => u.Name == "First Company");
                soldier = AssignSoldier(unassignedSoldierMap, veteranLeaders, firstCompany.HQSquad, 
                    TempSoldierTypes.Instance.SpaceMarineSoldierTypes[12], year);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Leadership, 4);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Tactics, 4);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Bike, 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.JumpPack, 2);
            }
            var leaders = unassignedSoldierMap.Values.OrderByDescending(s => s.LeadershipRating).Take(20).ToList();
            // assign Recruitment Captain next
            // assuming Tenth Company for now
            var tenthCompany = chapter.ChildUnits.First(u => u.Name == "Tenth Company");
            soldier = AssignSoldier(unassignedSoldierMap, leaders, tenthCompany.HQSquad, 
                TempSoldierTypes.Instance.SpaceMarineSoldierTypes[12], year);
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
                    soldier = AssignSoldier(unassignedSoldierMap, leaders, company.HQSquad, 
                        TempSoldierTypes.Instance.SpaceMarineSoldierTypes[12], year);
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
    
        private static void AssignVeterans(Dictionary<int, PlayerSoldier> unassignedSoldierMap, Unit chapter, string year)
        {
            var veterans = unassignedSoldierMap.Values.Where(s => s.MeleeRating > 600 && s.RangedRating > 75);
            var veteranLeaders = veterans.Where(s => s.LeadershipRating > 235).OrderByDescending(s => s.LeadershipRating).ToList();
            // if there are no veteran sgts, leave First Company empty for now
            if (veteranLeaders.Count == 0) return;
            var vetList = veterans.Except(veteranLeaders).OrderByDescending(s => s.MeleeRating).ToList();
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

            PlayerSoldier soldier;
            // look for Veteran Squads
            foreach(Unit company in chapter.ChildUnits)
            {
                foreach (Squad squad in company.Squads)
                {
                    if ((squad.SquadTemplate.SquadType & SquadTypes.Elite) > 0)
                    {
                        if (vetList.Count == 0 || veteranLeaders.Count == 0)
                        {
                            break;
                        }
                        // assign sgt to squad
                        squad.Name = veteranLeaders[0].Name.Split(' ')[1] + " Squad";
                        soldier = AssignSoldier(unassignedSoldierMap, veteranLeaders, squad, 
                            TempSoldierTypes.Instance.SpaceMarineSoldierTypes[13], year);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.Leadership, 4);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.Tactics, 2);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 2);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 2);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.Bike, 2);
                        soldier.AddSkillPoints(TempBaseSkillList.Instance.JumpPack, 2);
                        soldier.GetBestSkillInCategory(SkillCategory.Melee).AddPoints(2);

                        while (squad.Members.Count < squadSize && vetList.Count > 0)
                        {
                            soldier = AssignSoldier(unassignedSoldierMap, vetList, squad,
                                TempSoldierTypes.Instance.SpaceMarineSoldierTypes[16], year);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 4);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 4);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bike, 2);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.JumpPack, 2);
                            soldier.GetBestSkillInCategory(SkillCategory.Melee).AddPoints(4);
                        }
                    }
                }
            }
        }

        private static void AssignExcessToScouts(Dictionary<int, PlayerSoldier> unassignedSoldierMap, Unit chapter, string year)
        {
            int sgtNeed = ((unassignedSoldierMap.Count - 1) / 10) + 1;
            var leaderList = unassignedSoldierMap.Values.OrderByDescending(s => s.LeadershipRating).Take(sgtNeed).ToList();
            var scoutList = unassignedSoldierMap.Values.Except(leaderList).ToList();
            Unit lastCompany = null;
            Squad lastSquad = null;
            PlayerSoldier soldier = null;
            foreach (Unit company in chapter.ChildUnits)
            {
                lastCompany = company;
                foreach (Squad squad in company.Squads)
                {
                    if ((squad.SquadTemplate.SquadType & SquadTypes.Scout) > 0)
                    {
                        lastSquad = squad;
                        // assign sgt to squad
                        squad.Name = leaderList[0].Name.Split(' ')[1] + " Squad";
                        soldier = AssignSoldier(unassignedSoldierMap, leaderList, squad,
                            TempSoldierTypes.Instance.SpaceMarineSoldierTypes[13], year);
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

                        while (squad.Members.Count < 10 && scoutList.Count > 0)
                        {
                            soldier = AssignSoldier(unassignedSoldierMap, scoutList, squad,
                                TempSoldierTypes.Instance.SpaceMarineSoldierTypes[22], year);
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
                Squad squad = new Squad(id, "Scout Squad", 
                    TempSpaceMarineSquadTemplates.Instance.SquadTemplates[22]);
                lastCompany.Squads.Add(squad);
                id++;
                lastSquad = squad;
                // assign sgt to squad
                squad.Name = leaderList[0].Name.Split(' ')[1] + " Squad";
                soldier = AssignSoldier(unassignedSoldierMap, leaderList, squad,
                   TempSoldierTypes.Instance.SpaceMarineSoldierTypes[13], year);
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
                while (squad.Members.Count < 10 && scoutList.Count > 0)
                {
                    soldier = AssignSoldier(unassignedSoldierMap, scoutList, squad,
                        TempSoldierTypes.Instance.SpaceMarineSoldierTypes[22], year);
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

        private static void AssignSpecialistsToUnit(Dictionary<int, PlayerSoldier> unassignedSoldierMap,
                                             Unit chapter,
                                             string year,
                                             SoldierType specialistType,
                                             List<PlayerSoldier> sortedCandidates, 
                                             TrainingFunction Train)
        {
            AssignSpecialistsToSquad(unassignedSoldierMap, chapter.HQSquad, year, specialistType, sortedCandidates, Train);
            foreach (Squad squad in chapter.Squads)
            {
                AssignSpecialistsToSquad(unassignedSoldierMap, squad, year, specialistType, sortedCandidates, Train);
            }
            foreach (Unit company in chapter.ChildUnits)
            {
                AssignSpecialistsToSquad(unassignedSoldierMap, company.HQSquad, year, specialistType, sortedCandidates, Train);
                foreach (Squad squad in chapter.Squads)
                {
                    AssignSpecialistsToSquad(unassignedSoldierMap, squad, year, specialistType, sortedCandidates, Train);
                }
            }
        }

        private static void AssignSpecialistsToSquad(Dictionary<int, PlayerSoldier> unassignedSoldierMap,
                                                     Squad squad,
                                                     string year,
                                                     SoldierType specialistType,
                                                     List<PlayerSoldier> sortedCandidates,
                                                     TrainingFunction Train)
        {
            // minor hack to avoid assigning non-veteran specialists to veteran HQ squads
            if (squad == null 
                || sortedCandidates.Count == 0 
                || (squad.SquadTemplate.SquadType & SquadTypes.Elite) > 0) return;
            IEnumerable<SquadTemplateElement> elements = squad.SquadTemplate.Elements
                .Where(e => e.AllowedSoldierTypes.Contains(specialistType));
            foreach (SquadTemplateElement element in elements)
            {
                if (sortedCandidates.Count == 0) return;
                for (int i = 0; i < element.MinimumNumber; i++)
                {
                    if (sortedCandidates.Count == 0) return;
                    // assign top to HQ
                    PlayerSoldier soldier = AssignSoldier(unassignedSoldierMap, sortedCandidates, squad,
                        specialistType, year);
                    Train(soldier);
                }
            }
        }

        private static void TrainChampion(PlayerSoldier soldier)
        {
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 4);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 4);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, 4);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bike, 4);
        }

        private static void TrainAncient(PlayerSoldier soldier)
        {
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 8);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, 4);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Throwing, 4);
        }

        private static void AssignMarines(Dictionary<int, PlayerSoldier> unassignedSoldierMap, Unit chapter, string year)
        {
            var assList = unassignedSoldierMap.Values.Where(s => s.MeleeRating > 400 && s.RangedRating <= 60 && s.LeadershipRating <= 160).OrderByDescending(s => s.MeleeRating).ToList();
            var devList = unassignedSoldierMap.Values.Where(s => s.MeleeRating <= 400 && s.RangedRating > 60 && s.LeadershipRating <= 160).OrderByDescending(s => s.RangedRating).ToList();
            var tactList = unassignedSoldierMap.Values.Where(s => s.MeleeRating > 400 && s.RangedRating > 60 && s.LeadershipRating <= 160).ToList();
            var assSgtList = unassignedSoldierMap.Values.Where(s => s.MeleeRating > 400 && s.RangedRating <= 60 && s.LeadershipRating > 160).OrderByDescending(s => s.LeadershipRating).ToList();
            var devSgtList = unassignedSoldierMap.Values.Where(s => s.MeleeRating <= 400 && s.RangedRating > 60 && s.LeadershipRating > 160).OrderByDescending(s => s.LeadershipRating).ToList();
            var tactSgtList = unassignedSoldierMap.Values.Where(s => s.MeleeRating > 400 && s.RangedRating > 60 && s.LeadershipRating > 160).OrderByDescending(s => s.LeadershipRating).ToList();

            BalanceLists(assList, devList, tactList, assSgtList, devSgtList, tactSgtList);
            AssignTacticalMarines(unassignedSoldierMap, chapter, year, tactList, tactSgtList);
            assList.AddRange(tactList);
            AssignAssaultMarines(unassignedSoldierMap, chapter, year, assList, assSgtList);
            if (assList.Count > 0)
            {
                devList.AddRange(assList.Where(a => a.RangedRating > 60));
                devList = devList.OrderByDescending(s => s.RangedRating).ToList();
            }
            AssignDevastatorMarines(unassignedSoldierMap, chapter, year, devList, devSgtList);
        }

        private static void AssignDevastatorMarines(Dictionary<int, PlayerSoldier> unassignedSoldierMap, Unit chapter, string year, List<PlayerSoldier> devList, List<PlayerSoldier> devSgtList)
        {
            PlayerSoldier soldier;
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
                    if (squad.SquadTemplate.Id == 21)
                    {
                        if (devSgtList.Count > 0)
                        {
                            squad.Name = devSgtList[0].Name.Split(' ')[1] + " Squad";
                            soldier = AssignSoldier(unassignedSoldierMap, devSgtList, squad, 
                                TempSoldierTypes.Instance.SpaceMarineSoldierTypes[13], year);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Leadership, 4);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Tactics, 4);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, 4);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, 4);
                            soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, 8);
                        }
                        int devSquadSize = CalculateSquadSize(devList, devSgtList);
                        while (devList.Count > 0 && squad.Members.Count < devSquadSize)
                        {
                            soldier = AssignSoldier(unassignedSoldierMap, devList, squad,
                                TempSoldierTypes.Instance.SpaceMarineSoldierTypes[21], year);
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

        private static void AssignAssaultMarines(Dictionary<int, PlayerSoldier> unassignedSoldierMap, Unit chapter, string year, List<PlayerSoldier> assList, List<PlayerSoldier> assSgtList)
        {
            foreach (Unit company in chapter.ChildUnits)
            {
                foreach (Squad squad in company.Squads)
                {
                    if (squad.SquadTemplate.Id == 20)
                    {
                        if (assSgtList.Count > 0)
                        {
                            squad.Name = assSgtList[0].Name.Split(' ')[1] + " Squad";
                            PlayerSoldier soldier = AssignSoldier(unassignedSoldierMap, assSgtList, squad, 
                                TempSoldierTypes.Instance.SpaceMarineSoldierTypes[13], year);
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
                            PlayerSoldier soldier = AssignSoldier(unassignedSoldierMap, assList, squad,
                                TempSoldierTypes.Instance.SpaceMarineSoldierTypes[20], year);
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

        private static void AssignTacticalMarines(Dictionary<int, PlayerSoldier> unassignedSoldierMap, Unit chapter, string year, List<PlayerSoldier> tactList, List<PlayerSoldier> tactSgtList)
        {
            foreach (Unit company in chapter.ChildUnits)
            {
                foreach (Squad squad in company.Squads)
                {
                    if (squad.SquadTemplate.Id == 19)
                    {

                        if (tactSgtList.Count > 0)
                        {
                            squad.Name = tactSgtList[0].Name.Split(' ')[1] + " Squad";
                            PlayerSoldier soldier = AssignSoldier(unassignedSoldierMap, tactSgtList, squad,
                                TempSoldierTypes.Instance.SpaceMarineSoldierTypes[13], year);
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
                            PlayerSoldier soldier = AssignSoldier(unassignedSoldierMap, tactList, squad,
                               TempSoldierTypes.Instance.SpaceMarineSoldierTypes[19], year);
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

        private static int CalculateSquadSize(List<PlayerSoldier> soldierList, List<PlayerSoldier> sgtList)
        {
            if ((soldierList.Count - 9) >= (sgtList.Count * 4))
            {
                return 10;
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

        private static PlayerSoldier AssignSoldier(Dictionary<int, PlayerSoldier> unassignedSoldierMap, 
                                                   List<PlayerSoldier> soldierList, 
                                                   Squad squad, 
                                                   SoldierType type, 
                                                   string year)
        {
            var soldier = soldierList[0];
            soldier.Type = type;
            soldier.AssignToSquad(squad);
            soldier.AddEntryToHistory(year + ": promoted to " + soldier.Type.Name 
                + " and assigned to " + soldier.AssignedSquad.Name);
            squad.Members.Add(soldier);
            unassignedSoldierMap.Remove(soldier.Id);
            soldierList.RemoveAt(0);
            return soldier;
        }

        private static void BalanceLists(List<PlayerSoldier> assList, List<PlayerSoldier> devList, List<PlayerSoldier> tactList, 
            List<PlayerSoldier> assSgtList, List<PlayerSoldier> devSgtList, List<PlayerSoldier> tactSgtList)
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
                // shift tact soldiers to be other soldiers
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
                    // move nine tactMarines to be devMarines
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
