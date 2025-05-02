using BTKits_MysqlKits_MigrationTool.Database;
using BTKits_MysqlKits_MigrationTool.Modules;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BTKits_MysqlKits_MigrationTool
{
    public class MigrationTool
    {
        private static string databaseName = "";
        private static string databaseUsername = "";
        private static string databasePassword = "";
        private static string databaseAddress = "";
        private static uint databasePort = new uint();
        private static List<MysqlKits> MysqlKits = new List<MysqlKits>() { };

        // TODO - Remove this when release
        private static bool AutoLogin = true;
        static void Main(string[] args)
        {
            Console.WriteLine("Program Running");
            Login();
        }

        public static MySqlConnectionStringBuilder CreateConnectionString()
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder()
            {
                Server = databaseAddress,
                Database = databaseName,
                UserID = databaseUsername,
                Password = databasePassword,
                Port = databasePort,
            };
            return builder;
        }

        public static async Task SetupTransferKits()
        {
            try
            {
                List<kits> mysqlKits = await MysqlKitsConnections.GetKits();
                Console.WriteLine($"Adding {mysqlKits.Count} MysqlKits to BTKits...");
                await BTKitsConnections.AddKitsList(mysqlKits);
                Console.WriteLine($"Successfully Added {mysqlKits.Count} Kits!");

                List<cooldown> playerCooldowns = await MysqlKitsConnections.GetPlayerCooldowns();
                Console.WriteLine($"Adding {playerCooldowns.Count} Player Cooldowns to BTKits...");
                await BTKitsConnections.AddPlayerCooldownsList(playerCooldowns);
                Console.WriteLine($"Successfully Added {playerCooldowns.Count} Cooldowns!");

                /*
                int i = 1;
                foreach (var kit in mysqlKits)
                {
                    Console.WriteLine($"{i} - {kit.Name}");
                    i++;
                }
                */
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }


        private static void Login() // login method
        {
            if(!AutoLogin)
            {
                Console.WriteLine("Database Name: ");
                databaseName = Console.ReadLine();
                Console.WriteLine("Database Username: ");
                databaseUsername = Console.ReadLine();
                Console.WriteLine("Database Password: ");
                databasePassword = Console.ReadLine();
                Console.WriteLine("Database Address: ");
                databaseAddress = Console.ReadLine();
                Console.WriteLine("Database Port: ");
                databasePort = uint.Parse(Console.ReadLine());
            }
            else
            {
                databaseName = "unturned";
                databaseUsername = "root";
                databasePassword = "password";
                databaseAddress = "127.0.0.1";
                databasePort = 3306;
            }
            
            Console.WriteLine($"Database: {databaseName}\nUsername: {databaseUsername}\nPassword: {databasePassword}\nServer: {databaseAddress}\nPort: {databasePort} ");


            using (MySqlConnection connection = new MySqlConnection(CreateConnectionString().ConnectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Application Open");
                    Task.Run(async () =>
                    {
                        await SetupTransferKits();
                    });
                    Console.ReadLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine("------------------------");
                    Console.WriteLine("Database Connection Error");
                    Console.WriteLine("------------------------");
                    Console.WriteLine(e.ToString());
                    Console.ReadLine();
                }
            }
        }
    }
}
