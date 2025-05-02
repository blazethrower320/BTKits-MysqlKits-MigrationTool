using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
    public class cooldown
    {
        public ulong PlayerID { get; set; }
        public string KitName { get; set; }
        public int Cooldown { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
