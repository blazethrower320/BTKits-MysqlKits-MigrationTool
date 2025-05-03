using BTKits_MysqlKits_MigrationTool.Modules;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTKits_MysqlKits_MigrationTool.Database
{
    public class MysqlKitsConnections
    {
        private static string kitsTable = "kits";
        private static string playerCooldownsTable = "kits_cooldowns";
        private static string moddedItemsTable = "kits_modified";

        public static async Task<List<kits>> GetKits()
        {
            List<kits> kits = new List<kits>() { };

            await using MySqlConnection connection = new MySqlConnection(MigrationTool.CreateConnectionString().ConnectionString);
            await connection.OpenAsync();
            await using MySqlCommand command = new MySqlCommand($"SELECT * FROM {kitsTable}", connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                string kitName = reader.GetString("Name");
                int kitCost = reader.GetInt32("Cost");
                int kitCooldown = reader.GetInt32("Cooldown");
                string content = reader.GetString("Content");
                kits.Add(new Modules.kits()
                {
                    Name = kitName,
                    Cooldown = kitCooldown,
                    Cost = kitCost,
                    KitContent = content
                    
                });
            }
            return kits;
        }
        public static async Task<List<cooldown>> GetPlayerCooldowns()
        {
            List<cooldown> playerCooldowns = new List<cooldown>() { };

            await using MySqlConnection connection = new MySqlConnection(MigrationTool.CreateConnectionString().ConnectionString);
            await connection.OpenAsync();

            var mysqlKits = await GetKits();

            await using MySqlCommand command = new MySqlCommand($"SELECT * FROM {playerCooldownsTable}", connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                ulong playerID = ulong.Parse(reader.GetString("PlayerID"));
                string KitName = reader.GetString("Kit");
                DateTime TimeUsed = reader.GetDateTime("TimeUsed");

                var kitCooldown = mysqlKits.FirstOrDefault(c => c.Name.Equals(KitName, StringComparison.OrdinalIgnoreCase));
                if (kitCooldown == null)
                    continue;

                playerCooldowns.Add(new Modules.cooldown()
                {
                    PlayerID = playerID,
                    KitName = KitName,
                    Cooldown = kitCooldown.Cooldown,
                    StartDate = TimeUsed,
                    EndDate = TimeUsed.AddSeconds(kitCooldown.Cooldown)
                });
            }
            return playerCooldowns;
        }
        public static async Task<(List<Items>, List<Vehicles>)> GetAllKitsItems()
        {
            await using MySqlConnection connection = new MySqlConnection(MigrationTool.CreateConnectionString().ConnectionString);
            await connection.OpenAsync();

            List<ModdedItem> moddedItems = new List<ModdedItem>();
            List<kits> kits = await GetKits();
            List<Items> KitItems = new List<Items>();
            List<Vehicles> KitVehicles = new List<Vehicles>();
            // Grab Modded Items before checking Items
            await using MySqlCommand command = new MySqlCommand($"SELECT * FROM {moddedItemsTable}", connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                string weapon = reader.GetString("Weapon");
                ushort weaponID = ushort.Parse(reader.GetInt32("ID").ToString());
                int magAmmo = reader.GetInt32("Clip");
                int sight = reader.GetInt32("Sight");
                int tactical = reader.GetInt32("Tactical");
                int grip = reader.GetInt32("Grip");
                int barrel = reader.GetInt32("Barrel");
                ushort magID = ushort.Parse(reader.GetInt32("Magazine").ToString());
                moddedItems.Add(new ModdedItem
                {
                    BarrelID = barrel,
                    GripID = grip,
                    ItemID = weaponID,
                    MagazineID = magID,
                    SightID = sight,
                    TacticalID = tactical,
                    MagazineAmount = magAmmo,
                    Weapon = weapon,
                });
            }

            // Kit Content
            foreach (var kit in kits)
            {
                string[] kitDetails = kit.KitContent.Split(" ");
                foreach(var detail in kitDetails)
                {
                    //Console.WriteLine(detail);
                    if(detail.StartsWith("m."))
                    {
                        var moddedItemName = detail.Substring(2);
                        var exists = moddedItems.FirstOrDefault(c => c.Weapon.Equals(moddedItemName, StringComparison.CurrentCultureIgnoreCase));
                        if (exists == null)
                        {
                            Console.WriteLine("Error - Modded Item Not Found: " +  moddedItemName);
                            continue;
                        }
                        KitItems.Add(new Items
                        {
                            BarrelID = exists.BarrelID,
                            GripID = exists.GripID,
                            ItemID = exists.ItemID,
                            MagazineID = exists.MagazineID,
                            SightID = exists.SightID,
                            TacticalID = exists.TacticalID,
                            KitName = kit.Name,
                            MagazineAmount = exists.MagazineAmount,
                        });
                    }
                    else if(detail.StartsWith("v."))
                    {
                        var vehicleKit = detail.Substring(2);
                        KitVehicles.Add(new Vehicles
                        {
                            VehicleID = ushort.Parse(vehicleKit),
                            KitName = kit.Name,
                        });
                    }
                    else if(detail.StartsWith("s."))
                    {
                        // A Set
                        continue;
                    }
                    else if(detail.StartsWith("c."))
                    {
                        // A Category
                        continue;
                    }
                    else if(detail.Contains("/"))
                    {
                        var ItemToAdd = detail.Split("/");
                        var itemID = ushort.Parse(ItemToAdd[0]);
                        var itemQuantity = ushort.Parse(ItemToAdd[1]);
                        for (int i = 1; i < itemQuantity; i++)
                        {
                            KitItems.Add(new Items
                            {
                                KitName = kit.Name,
                                ItemID = itemID,
                            });
                        }
                    }
                    else
                    {
                        // Just a ID without a /
                        KitItems.Add(new Items
                        {
                            KitName = kit.Name,
                            ItemID = ushort.Parse(detail),
                        });
                    }

                }
            }
            return (KitItems,  KitVehicles);
        }
    }
}
