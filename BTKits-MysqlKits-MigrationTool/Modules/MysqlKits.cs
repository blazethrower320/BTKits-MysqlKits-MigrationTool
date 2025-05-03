using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BTKits_MysqlKits_MigrationTool.Modules
{
    public class MysqlKits
    {
        public kits mysqlKits { get; set; }
        public cooldown MysqlPlayersCooldowns {  get; set; }
    }
    public class kits
    {
        public string Name { get; set; }
        public int Cost { get; set; }
        public int Cooldown { get; set; }
        public string KitContent {  get; set; }
    }
    public class cooldown
    {
        public ulong PlayerID { get; set; }
        public string KitName { get; set; }
        public int Cooldown { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class ModdedItem
    {
        public string Weapon { get; set; }
        public ushort ItemID { get; set; }
        public int SightID { get; set; }
        public int TacticalID { get; set; }
        public int BarrelID { get; set; }
        public ushort MagazineID { get; set; }
        public int MagazineAmount { get; set; }
        public int GripID { get; set; }
    }
    public class Items
    {
        public string KitName { get; set; }
        public ushort ItemID { get; set; }
        public byte Durability { get; set; } = 100;
        public int SightID { get; set; } = 0;
        public int TacticalID { get; set; } = 0;
        public int BarrelID { get; set; } = 0;
        public ushort MagazineID { get; set; } = 0;
        public int MagazineAmount { get; set; }
        public int GripID { get; set; } = 0;
        public byte X { get; set; } = 0;
        public byte Y { get; set; } = 0;
        public byte Page { get; set; } = 0;
        public byte Rotation { get; set; } = 0;
    }
    public class Vehicles
    {
        public string KitName { get; set; }
        public ushort VehicleID {  get; set; }
    }
}
