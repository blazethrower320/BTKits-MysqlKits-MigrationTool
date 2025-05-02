using BTKits_MysqlKits_MigrationTool.Modules;
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

                kits.Add(new Modules.kits()
                {
                    Name = kitName,
                    Cooldown = kitCooldown,
                    Cost = kitCost
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
    }
}
