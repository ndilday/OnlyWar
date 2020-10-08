using UnityEngine;

namespace OnlyWar.Scripts.Models.Soldiers
{
    public enum Attribute
    {
        Strength = 1,
        Dexterity = 2,
        Constitution = 3,
        Intelligence = 4,
        Presence = 5,
        Ego = 6
    }

    public enum SkillCategory
    {
        Ranged = 1,
        Gunnery = 2,
        Melee = 3,
        Vehicle = 4,
        Military = 5,
        Professional = 6,
        Tech = 7,
        Apothecary = 8
    }

    public class BaseSkill
    {
        public int Id;
        public SkillCategory Category;
        public string Name;
        public Attribute BaseAttribute;
        public float Difficulty;
        public BaseSkill(int id, SkillCategory category, string name, Attribute baseAttribute, float difficulty)
        {
            Id = id;
            Category = category;
            Name = name;
            BaseAttribute = baseAttribute;
            Difficulty = difficulty;
        }
    }

    public class Skill
    {
        public BaseSkill BaseSkill { get; private set; }
        public float PointsInvested { get; private set; }

        public float SkillBonus
        {
            get
            {
                return (PointsInvested == 0 ? -4 : Mathf.Log(PointsInvested, 2)) - BaseSkill.Difficulty;
            }
        }
        public Skill(BaseSkill baseSkill, float points = 0)
        {
            BaseSkill = baseSkill;
            if (points < 0) PointsInvested = 0;
            else PointsInvested = points;
        }
        public void AddPoints(float points)
        {
            if(points > 0)
            {
                PointsInvested += points;
            }
        }
    }

    public class TempBaseSkillList
    {
        private static TempBaseSkillList _instance;
        public static TempBaseSkillList Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new TempBaseSkillList();
                }
                return _instance;
            }
        }

        private TempBaseSkillList()
        {
            Piety = new BaseSkill(1, SkillCategory.Professional, "Piety", Attribute.Presence, -2);
            Ritual = new BaseSkill(2, SkillCategory.Professional, "Ritual", Attribute.Presence, -2);
            Leadership = new BaseSkill(3, SkillCategory.Professional, "Leadership", Attribute.Presence, -1);
            Persuade = new BaseSkill(4, SkillCategory.Professional, "Persuade", Attribute.Presence, -2);
            FirstAid = new BaseSkill(14, SkillCategory.Professional, "First Aid", Attribute.Intelligence, 0);
            Teaching = new BaseSkill(16, SkillCategory.Professional, "Teaching", Attribute.Intelligence, -1);

            Tactics = new BaseSkill(10, SkillCategory.Military, "Tactics", Attribute.Intelligence, -2);
            Marine = new BaseSkill(11, SkillCategory.Military, "Marine", Attribute.Intelligence, -1);
            ArmorySmallArms = new BaseSkill(12, SkillCategory.Military, "Armory (Small Arms)", Attribute.Intelligence, -1);
            Explosives = new BaseSkill(17, SkillCategory.Military, "Explosives", Attribute.Intelligence, -1);
            Stealth = new BaseSkill(29, SkillCategory.Military, "Stealth", Attribute.Dexterity, -1);

            PowerArmor = new BaseSkill(20, SkillCategory.Military, "Power Armor", Attribute.Dexterity, -1);

            Rhino = new BaseSkill(21, SkillCategory.Vehicle, "Drive (Rhino)", Attribute.Dexterity, -1);
            LandSpeeder = new BaseSkill(22, SkillCategory.Vehicle, "Pilot (Land Speeder)", Attribute.Dexterity, -1);
            JumpPack = new BaseSkill(27, SkillCategory.Vehicle, "Jump Pack", Attribute.Dexterity, 0);
            Bike = new BaseSkill(28, SkillCategory.Vehicle, "Drive (Bike)", Attribute.Dexterity, 0);

            GunneryBeam = new BaseSkill(23, SkillCategory.Gunnery, "Gunnery (Beam)", Attribute.Dexterity, 0);
            GunneryBolter = new BaseSkill(24, SkillCategory.Gunnery, "Gunnery (Bolter)", Attribute.Dexterity, 0);
            GunneryCannon = new BaseSkill(25, SkillCategory.Gunnery, "Gunnery (Cannon)", Attribute.Dexterity, 0);
            GunneryRocket = new BaseSkill(26, SkillCategory.Gunnery, "Gunnery (Rocket)", Attribute.Dexterity, 0);
            
            Bolter = new BaseSkill(30, SkillCategory.Ranged, "Bolter", Attribute.Dexterity, 0);
            Lascannon = new BaseSkill(31, SkillCategory.Ranged, "Lascannon", Attribute.Dexterity, 0);
            Plasma = new BaseSkill(32, SkillCategory.Ranged, "Plasma", Attribute.Dexterity, 0);
            MissileLauncher = new BaseSkill(33, SkillCategory.Ranged, "Missile Launcher", Attribute.Dexterity, 0);
            Flamer = new BaseSkill(34, SkillCategory.Ranged, "Flamer", Attribute.Dexterity, 0);
            Sniper = new BaseSkill(35, SkillCategory.Ranged, "Sniper", Attribute.Dexterity, 0);
            Shotgun = new BaseSkill(36, SkillCategory.Ranged, "Shotgun", Attribute.Dexterity, 0);

            Throwing = new BaseSkill(40, SkillCategory.Melee, "Throwing", Attribute.Dexterity, -1);
            Shield = new BaseSkill(41, SkillCategory.Melee, "Shield", Attribute.Dexterity, 0);
            Sword = new BaseSkill(42, SkillCategory.Melee, "Sword", Attribute.Dexterity, -1);
            Axe = new BaseSkill(43, SkillCategory.Melee, "Axe", Attribute.Dexterity, -1);
            Fist = new BaseSkill(44, SkillCategory.Melee, "Fist", Attribute.Dexterity, -1);
            ServoArm = new BaseSkill(45, SkillCategory.Melee, "Servo-Arm", Attribute.Dexterity, -1);

            CyberEngineering = new BaseSkill(50, SkillCategory.Tech, "Engineering (Cybernetics)", Attribute.Intelligence, -2);
            MachineGod = new BaseSkill(51, SkillCategory.Tech, "Church of the Machine God", Attribute.Presence, -2);
            ArmoryForceShield = new BaseSkill(52, SkillCategory.Tech, "Armory (Force Shield)", Attribute.Intelligence, -1);
            ArmoryPowerArmor = new BaseSkill(53, SkillCategory.Tech, "Armory (Power Armor)", Attribute.Intelligence, -1);
            ArmoryVehicle = new BaseSkill(54, SkillCategory.Tech, "Armory (Vehicle)", Attribute.Intelligence, -1);
            RhinoMechanic = new BaseSkill(55, SkillCategory.Tech, "Mechanic (Rhino)", Attribute.Intelligence, -1);
            LandSpeederMechanic = new BaseSkill(56, SkillCategory.Tech, "Mechanic (Land Speeder)", Attribute.Intelligence, -1);
            LandRaiderMechanic = new BaseSkill(57, SkillCategory.Tech, "Mechanic (Land Raider)", Attribute.Intelligence, -1);
            LandRaider = new BaseSkill(58, SkillCategory.Tech, "Drive (Land Raider)", Attribute.Dexterity, -1);

            Surgery = new BaseSkill(60, SkillCategory.Apothecary, "Surgery", Attribute.Intelligence, -3);
            Pharmacy = new BaseSkill(61, SkillCategory.Apothecary, "Pharmacy", Attribute.Intelligence, -2);
            Physician = new BaseSkill(62, SkillCategory.Apothecary, "Physician", Attribute.Intelligence, -2);
            Diagnosis = new BaseSkill(63, SkillCategory.Apothecary, "Diagnosis", Attribute.Intelligence, -2);

            OpponentMelee = new BaseSkill(100, SkillCategory.Melee, "Melee", Attribute.Dexterity, 0);
            OpponentRanged = new BaseSkill(100, SkillCategory.Ranged, "Ranged", Attribute.Dexterity, 0);
        }

        public BaseSkill Piety { get; private set; }
        public BaseSkill Ritual { get; private set; }
        public BaseSkill Leadership { get; private set; }
        public BaseSkill Persuade { get; private set; }

        public BaseSkill Tactics { get; private set; }
        public BaseSkill Marine { get; private set; }
        public BaseSkill ArmorySmallArms { get; private set; }
        public BaseSkill ArmoryVehicle { get; private set; }
        public BaseSkill FirstAid { get; private set; }
        public BaseSkill Teaching { get; private set; }

        public BaseSkill PowerArmor { get; private set; }
        public BaseSkill Rhino { get; private set; }
        public BaseSkill LandSpeeder { get; private set; }
        public BaseSkill GunneryBeam { get; private set; }
        public BaseSkill GunneryCannon { get; private set; }
        public BaseSkill GunneryBolter { get; private set; }
        public BaseSkill GunneryRocket { get; private set; }
        public BaseSkill JumpPack { get; private set; }
        public BaseSkill Bike { get; private set; }
        public BaseSkill Stealth { get; private set; }
        public BaseSkill Explosives { get; private set; }

        public BaseSkill Bolter { get; private set; }
        public BaseSkill Lascannon { get; private set; }
        public BaseSkill Plasma { get; private set; }
        public BaseSkill MissileLauncher { get; private set; }
        public BaseSkill Flamer { get; private set; }
        public BaseSkill Sniper { get; private set; }
        public BaseSkill Shotgun { get; private set; }

        public BaseSkill Throwing { get; private set; }
        public BaseSkill Shield { get; private set; }
        public BaseSkill Sword { get; private set; }
        public BaseSkill Axe { get; private set; }
        public BaseSkill Fist { get; private set; }
        public BaseSkill ServoArm { get; private set; }

        public BaseSkill CyberEngineering { get; private set; }
        public BaseSkill MachineGod { get; private set; }
        public BaseSkill ArmoryPowerArmor { get; private set; }
        public BaseSkill RhinoMechanic { get; private set; }
        public BaseSkill LandSpeederMechanic { get; private set; }
        public BaseSkill LandRaiderMechanic { get; private set; }
        public BaseSkill LandRaider { get; private set; }
        public BaseSkill ArmoryForceShield { get; private set; }

        public BaseSkill Pharmacy { get; private set; }
        public BaseSkill Physician { get; private set; }
        public BaseSkill Surgery { get; private set; }
        public BaseSkill Diagnosis { get; private set; }

        public BaseSkill OpponentMelee { get; private set; }
        public BaseSkill OpponentRanged { get; private set; }
    }
}