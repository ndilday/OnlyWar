--
-- File generated with SQLiteStudio v3.2.1 on Mon Oct 12 11:20:58 2020
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: Character
CREATE TABLE Character (Id INTEGER PRIMARY KEY UNIQUE NOT NULL, Investigation REAL NOT NULL, Paranoia REAL NOT NULL, Neediness REAL NOT NULL, Patience REAL NOT NULL, Appreciation REAL NOT NULL, Influence REAL NOT NULL, LoyalFactionId INTEGER NOT NULL, OpinionOfPlayer REAL NOT NULL);

-- Table: Fleet
CREATE TABLE Fleet (Id INTEGER PRIMARY KEY UNIQUE NOT NULL, FactionId INTEGER NOT NULL, x REAL NOT NULL, y REAL NOT NULL, DestinationPlanetId INTEGER REFERENCES Planet (Id));

-- Table: GlobalData
CREATE TABLE GlobalData (Millenium INTEGER NOT NULL, Year INTEGER NOT NULL, Week INTEGER NOT NULL, SaveVersion INTEGER NOT NULL);

-- Table: HitLocation
CREATE TABLE HitLocation (SoldierId INTEGER NOT NULL REFERENCES Soldier (Id), HitLocationTemplateId INTEGER NOT NULL, IsCybernetic BOOLEAN NOT NULL, Armor REAL NOT NULL, WoundTotal INTEGER NOT NULL, WeeksOfHealing INTEGER);

-- Table: Planet
CREATE TABLE Planet (Id INTEGER PRIMARY KEY UNIQUE NOT NULL, PlanetTemplateId INTEGER NOT NULL, Name STRING NOT NULL UNIQUE, x INTEGER NOT NULL, y INTEGER NOT NULL, FactionId INTEGER NOT NULL, Importance INTEGER NOT NULL, TaxLevel INTEGER NOT NULL, IsUnderAssault BOOLEAN NOT NULL);

-- Table: PlanetFaction
CREATE TABLE PlanetFaction (PlanetId INTEGER REFERENCES Planet (Id) NOT NULL, FactionId INTEGER NOT NULL, IsPublic BOOLEAN NOT NULL, Population BIGINT NOT NULL, PDFMembers INTEGER NOT NULL, PlanetaryControl INTEGER NOT NULL, PlayerReputation REAL NOT NULL, LeaderId INTEGER REFERENCES Character (Id));

-- Table: PlayeFactionSubEvent
CREATE TABLE PlayerFactionSubEvent (PlayerFactionEventId INTEGER REFERENCES PlayerFactionEvent (Id) NOT NULL, Entry TEXT NOT NULL);

-- Table: PlayerFactionEvent
CREATE TABLE PlayerFactionEvent (Id INTEGER PRIMARY KEY UNIQUE NOT NULL, Millenium INTEGER NOT NULL, Year INTEGER NOT NULL, Week INTEGER NOT NULL, Title TEXT NOT NULL);

-- Table: PlayerSoldier
CREATE TABLE PlayerSoldier (SoldierId INTEGER PRIMARY KEY REFERENCES Soldier (Id) UNIQUE NOT NULL, MeleeRating REAL, RangedRating REAL, LeadershipRating REAL, MedicalRating REAL, TechRating REAL, PietyRating REAL, AncientRating REAL, ImplantMillenium INTEGER NOT NULL, ImplantYear INTEGER NOT NULL, ImplantWeek INTEGER NOT NULL);

-- Table: PlayerSoldierFactionCasualtyCount
CREATE TABLE PlayerSoldierFactionCasualtyCount (PlayerSoldierId INTEGER NOT NULL REFERENCES PlayerSoldier (SoldierId), FactionId INTEGER NOT NULL, Count INTEGER NOT NULL);

-- Table: PlayerSoldierHistory
CREATE TABLE PlayerSoldierHistory (PlayerSoldierId INTEGER NOT NULL REFERENCES PlayerSoldier (SoldierId), Entry STRING NOT NULL);

-- Table: PlayerSoldierMeleeWeaponCasualtyCount
CREATE TABLE PlayerSoldierMeleeWeaponCasualtyCount (PlayerSoldierId REFERENCES PlayerSoldier (SoldierId) NOT NULL, MeleeWeaponTemplateId INTEGER, Count INTEGER NOT NULL);

-- Table: PlayerSoldierRangedWeaponCasualtyCount
CREATE TABLE PlayerSoldierRangedWeaponCasualtyCount (PlayerSoldierId REFERENCES PlayerSoldier (SoldierId) NOT NULL, RangedWeaponTemplateId INTEGER, Count INTEGER NOT NULL);

-- Table: Request
CREATE TABLE Request (Id INTEGER PRIMARY KEY UNIQUE NOT NULL, CharacterId INTEGER REFERENCES Character (Id) NOT NULL, PlanetId INTEGER REFERENCES Planet (Id) NOT NULL, RequestDate     INTEGER NOT NULL, FulfillmentDate INTEGER);

-- Table: Ship
CREATE TABLE Ship (Id INTEGER PRIMARY KEY UNIQUE NOT NULL, ShipTemplateId INTEGER NOT NULL, FleetId INTEGER REFERENCES Fleet (Id) NOT NULL, Name STRING NOT NULL);

-- Table: Soldier
CREATE TABLE Soldier (Id INTEGER PRIMARY KEY NOT NULL UNIQUE, SoldierTemplateId INTEGER NOT NULL, SquadId INTEGER NOT NULL REFERENCES Squad (Id), Name STRING NOT NULL, Strength REAL NOT NULL, Dexterity REAL NOT NULL, Constitution REAL NOT NULL, Intelligence REAL NOT NULL, Perception REAL NOT NULL, Ego REAL NOT NULL, Charisma REAL NOT NULL, PsychicPower REAL NOT NULL, AttackSpeed REAL NOT NULL, Size REAL NOT NULL, MoveSpeed REAL NOT NULL);

-- Table: SoldierSkill
CREATE TABLE SoldierSkill (SoldierId INTEGER NOT NULL REFERENCES Soldiers (Id), BaseSkillId INTEGER NOT NULL, PointsInvested REAL NOT NULL);

-- Table: Squad
CREATE TABLE Squad (Id INTEGER PRIMARY KEY UNIQUE NOT NULL, SquadTemplateId INTEGER NOT NULL, ParentUnitId INTEGER NOT NULL REFERENCES Unit (Id), Name STRING NOT NULL, LoadedShipId INTEGER REFERENCES Ships (Id), LandedPlanetId INTEGER REFERENCES Planets (Id), IsInReserve BOOLEAN NOT NULL);

-- Table: SquadWeaponSet
CREATE TABLE SquadWeaponSet (SquadId INTEGER NOT NULL REFERENCES Squad (Id), WeaponSetId INTEGER NOT NULL);

-- Table: Unit
CREATE TABLE Unit (Id INTEGER PRIMARY KEY UNIQUE NOT NULL, FactionId INTEGER NOT NULL, UnitTemplateId INTEGER NOT NULL, ParentUnitId INTEGER REFERENCES Unit (Id), Name STRING NOT NULL);

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
