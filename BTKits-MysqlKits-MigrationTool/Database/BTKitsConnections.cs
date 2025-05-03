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
    public class BTKitsConnections
    {
        private static string kitsTable = "BTKits_Kits";
        private static string playerCooldownsTable = "btkits_playercooldowns";
        private static string kitCooldowns = "BTKits_KitCooldowns";
        private static string kitItemsTable = "BTKits_Items";
        private static string kitVehiclesTable = "BTKits_Vehicles";
        private static bool ResetCooldownOnDeath = false;

        public static async Task AddKitsList(List<kits> mysqlKits)
        {
            
            await using MySqlConnection connection = new MySqlConnection(MigrationTool.CreateConnectionString().ConnectionString);
            await connection.OpenAsync();
            var sqlString = new StringBuilder($"INSERT INTO {kitsTable} (KitName, Price, ResetCooldownOnDeath, OneTimeUse) VALUES");
            var cooldownSqlString = new StringBuilder($"INSERT INTO {kitCooldowns} (KitName, Permission, Cooldown) VALUES");
            var values1 = new List<string>();
            var values2 = new List<string>();

            foreach (var kit in mysqlKits)
            {
                //Console.WriteLine(kit.Name);
                //Console.WriteLine(kit.Cost);
                var isOneTimeUse = kit.Cooldown.Equals(-1) ? "1" : "0";
                values1.Add(@$"(""{kit.Name}"", {kit.Cost}, {ResetCooldownOnDeath}, {isOneTimeUse})");
                values2.Add($@"(""{kit.Name}"", ""{kit.Name}"", ""{kit.Cooldown}"")");
            }
            sqlString.Append(string.Join(" , ", values1));
            cooldownSqlString.Append(string.Join(" , ", values2));

            // TODO - REMOVE THIS AFTER TESTING
            await using MySqlCommand deleteCommand = new MySqlCommand($"DELETE FROM {kitsTable}", connection);
            await deleteCommand.ExecuteNonQueryAsync();
            await using MySqlCommand deleteCommand2 = new MySqlCommand($"DELETE FROM {kitCooldowns}", connection);
            await deleteCommand2.ExecuteNonQueryAsync();

            await using MySqlCommand command = new MySqlCommand(sqlString.ToString(), connection);
            await command.ExecuteNonQueryAsync();
            await using MySqlCommand command2 = new MySqlCommand(cooldownSqlString.ToString(), connection);
            await command2.ExecuteNonQueryAsync();
        }
        public static async Task AddPlayerCooldownsList(List<cooldown> mysqlCooldown)
        {

            await using MySqlConnection connection = new MySqlConnection(MigrationTool.CreateConnectionString().ConnectionString);
            await connection.OpenAsync();
            var sqlString = new StringBuilder($"INSERT INTO {playerCooldownsTable} (PlayerID, KitName, Cooldown, StartDate, EndDate) VALUES");
            var values = new List<string>();

            foreach (var cooldown in mysqlCooldown)
            {
                //Console.WriteLine(kit.Name);
                //Console.WriteLine(kit.Cost);
                values.Add(@$"(""{cooldown.PlayerID}"", ""{cooldown.KitName}"", ""{cooldown.Cooldown}"", ""{cooldown.StartDate.ToString("yyyy-MM-dd HH:mm:ss")}"", ""{cooldown.EndDate.ToString("yyyy-MM-dd HH:mm:ss")}"")");
            }
            sqlString.Append(string.Join(" , ", values));


            // TODO - REMOVE THIS AFTER TESTING
            await using MySqlCommand deleteCommand = new MySqlCommand($"DELETE FROM {playerCooldownsTable}", connection);
            await deleteCommand.ExecuteNonQueryAsync();

            await using MySqlCommand command = new MySqlCommand(sqlString.ToString(), connection);
            await command.ExecuteNonQueryAsync();
        }

        public static async Task AddKitItems(List<Items> kitsItems)
        {
            await using MySqlConnection connection = new MySqlConnection(MigrationTool.CreateConnectionString().ConnectionString);
            await connection.OpenAsync();
            var sqlString = new StringBuilder($"INSERT INTO {kitItemsTable} (KitName, ItemID, SightID, TacticalID, BarrelID, MagazineID, MagazineAmount, GripID, X, Y, Page, Rotation) VALUES");
            var values = new List<string>();
            foreach(var item in kitsItems)
            {
                values.Add(@$"(""{item.KitName}"", ""{item.ItemID}"", ""{item.SightID}"", ""{item.TacticalID}"", ""{item.BarrelID}"", ""{item.MagazineID}"", ""{item.MagazineAmount}"", ""{item.GripID}"", 0,0,0,0)");
            }
            sqlString.Append(string.Join(" , ", values));

            // TODO - REMOVE THIS AFTER TESTING
            await using MySqlCommand deleteCommand = new MySqlCommand($"DELETE FROM {kitItemsTable}", connection);
            await deleteCommand.ExecuteNonQueryAsync();

            await using MySqlCommand command = new MySqlCommand(sqlString.ToString(), connection);
            await command.ExecuteNonQueryAsync();
        }
        public static async Task AddKitVehicles(List<Vehicles> kitVehicles)
        {
            await using MySqlConnection connection = new MySqlConnection(MigrationTool.CreateConnectionString().ConnectionString);
            await connection.OpenAsync();
            var sqlString = new StringBuilder($"INSERT INTO {kitVehiclesTable} (KitName, VehicleID) VALUES");
            var values = new List<string>();
            foreach (var vehicle in kitVehicles)
            {
                values.Add(@$"(""{vehicle.KitName}"", ""{vehicle.VehicleID}"")");
            }
            sqlString.Append(string.Join(" , ", values));

            // TODO - REMOVE THIS AFTER TESTING
            await using MySqlCommand deleteCommand = new MySqlCommand($"DELETE FROM {kitVehiclesTable}", connection);
            await deleteCommand.ExecuteNonQueryAsync();

            await using MySqlCommand command = new MySqlCommand(sqlString.ToString(), connection);
            await command.ExecuteNonQueryAsync();
        }
    }
}
