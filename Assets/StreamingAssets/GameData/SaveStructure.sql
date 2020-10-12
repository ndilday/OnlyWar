--
-- File generated with SQLiteStudio v3.2.1 on Sat Oct 10 10:06:23 2020
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: Fleet
CREATE TABLE Fleet (Id INTEGER PRIMARY KEY UNIQUE NOT NULL, FactionId INTEGER NOT NULL, x REAL NOT NULL, y REAL NOT NULL, DestinationPlanetId INTEGER REFERENCES Planet (Id));

-- Table: HitLocation
CREATE TABLE HitLocation (SoldierId INTEGER NOT NULL REFERENCES Soldier (Id), HitLocationTemplateId INTEGER NOT NULL, WoundTotal INTEGER NOT NULL, WeeksOfHealing INTEGER);

-- Table: Planet
CREATE TABLE Planet (Id INTEGER PRIMARY KEY UNIQUE NOT NULL, Name STRING NOT NULL UNIQUE, x INTEGER NOT NULL, y INTEGER NOT NULL, PlanetType INTEGER NOT NULL, FactionId INTEGER);

-- Table: PlayerSoldier
CREATE TABLE PlayerSoldier (SoldierId INTEGER PRIMARY KEY REFERENCES Soldier (Id) UNIQUE NOT NULL, MeleeRating REAL, RangedRating REAL, LeadershipRating REAL, MedicalRating REAL, TechRating REAL, PietyRating REAL, AncientRating REAL, ImplantMillenium INTEGER NOT NULL, ImplantYear INTEGER NOT NULL, ImplanyWeek INTEGER NOT NULL);

-- Table: PlayerSoldierFactionCasualtyCount
CREATE TABLE PlayerSoldierFactionCasualtyCount (PlayerSoldierId INTEGER NOT NULL REFERENCES PlayerSoldier (SoldierId), FactionId INTEGER NOT NULL, Count INTEGER NOT NULL);

-- Table: PlayerSoldierHistory
CREATE TABLE PlayerSoldierHistory (PlayerSoldierId INTEGER NOT NULL REFERENCES PlayerSoldier, Entry STRING NOT NULL);

-- Table: PlayerSoldierWeaponCasualtyCount
CREATE TABLE PlayerSoldierWeaponCasualtyCount (PlayerSoldierId REFERENCES PlayerSoldier (SoldierId) NOT NULL, RangedWeaponTemplateId INTEGER, MeleeWeaponTemplateId INTEGER, Count INTEGER NOT NULL);

-- Table: Ship
CREATE TABLE Ship (Id INTEGER PRIMARY KEY UNIQUE NOT NULL, ShipTemplateId INTEGER NOT NULL, FleetId INTEGER REFERENCES Fleet (Id) NOT NULL, Name STRING NOT NULL);

-- Table: Soldier
CREATE TABLE Soldier (Id INTEGER PRIMARY KEY NOT NULL UNIQUE, SoldierTypeId INTEGER NOT NULL, SquadId INTEGER NOT NULL REFERENCES Squad (Id), Name STRING NOT NULL, Strength REAL NOT NULL, Dexterity REAL NOT NULL, Constitution REAL NOT NULL, Intelligence REAL NOT NULL, Perception REAL NOT NULL, Ego REAL NOT NULL, Charisma REAL NOT NULL, PsychicPower REAL NOT NULL, AttackSpeed REAL NOT NULL, Size REAL NOT NULL, MoveSpeed REAL NOT NULL);

-- Table: SoldierSkill
CREATE TABLE SoldierSkill (SoldierId INTEGER NOT NULL REFERENCES Soldiers (Id), BaseSkillId INTEGER NOT NULL, PointsInvested REAL NOT NULL);

-- Table: Squad
CREATE TABLE Squad (Id INTEGER PRIMARY KEY UNIQUE NOT NULL, SquadTemplateId INTEGER NOT NULL, ParentUnitId INTEGER NOT NULL REFERENCES Unit (Id), Name STRING NOT NULL, LoadedShipId INTEGER REFERENCES Ships (Id), LandedPlanetId INTEGER REFERENCES Planets (Id));

-- Table: Unit
CREATE TABLE Unit (Id INTEGER PRIMARY KEY UNIQUE NOT NULL, FactionId INTEGER NOT NULL, UnitTemplateId INTEGER NOT NULL, HQSquadId INTEGER REFERENCES Squads (Id), ParentUnitId INTEGER REFERENCES Unit (Id), Name STRING NOT NULL);

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
