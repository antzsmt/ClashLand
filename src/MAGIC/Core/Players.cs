using System;
using System.Collections.Concurrent;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ClashLand.Core.Database;
using ClashLand.Extensions;
using ClashLand.Logic;
using Newtonsoft.Json;
using ClashLand.Logic.Enums;

namespace ClashLand.Core
{
    internal static class Players
    {
        internal static JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,                    MissingMemberHandling = MissingMemberHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Include,         NullValueHandling = NullValueHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented,                            Converters = { new Utils.ArrayReferencePreservngConverter() },
        };

        internal static long Seed;
        internal static readonly object s_sync = new object();
        internal static ConcurrentDictionary<long, Level> Levels;

        internal static void Initialize()
        {
            Levels = new ConcurrentDictionary<long, Level>();
        }

        internal static void Add(Level Player)
        {
            if (Device.m_vOnlinePlayers.Contains(Player) == false)
            {
                Device.m_vOnlinePlayers.Add(Player);
            }

            if (Levels.ContainsKey(Player.Avatar.UserId))
            {
                Levels[Player.Avatar.UserId] = Player;
            }
            else
            {
                Levels.TryAdd(Player.Avatar.UserId, Player);
            }
        }

        internal static void Remove(Level Player)
        {
            if (Player != null)
            {
                Player.Tick();
                Save(Player);

                Levels.TryRemove(Player.Avatar.UserId);

                if (Player.Device != null)
                {
                    if (Devices._Devices.ContainsKey(Player.Device.SocketHandle))
                    {
                        Devices.Remove(Player.Device.SocketHandle);
                    }
                    Resources.GChat.Remove(Player.Device);
                }
            }
        }

        internal static Level Get(long UserId, bool Store = true, bool AvatarOnly = false)
        {
            if (!Levels.ContainsKey(UserId))
            {
                Level Player = null;

                using (MysqlEntities Database = new MysqlEntities())
                {
                    var Data = Database.Player.Find(UserId);

                    if (!string.IsNullOrEmpty(Data?.Avatar) && !string.IsNullOrEmpty(Data?.Village))
                    {
                        Player = new Level
                        {
                            Avatar = JsonConvert.DeserializeObject<Logic.Player>(Data.Avatar, Settings)
                        };
                        if (!AvatarOnly)
                            Player.JSON = Data.Village;

                        if (Store)
                        {
                            Add(Player);
                        }
                    }
                }
                return Player;
            }
            return Levels[UserId];
        }

        internal static Level New(long UserId = 0, string token = "", bool Store = true)
        {
            lock (s_sync)
            {
                if (UserId == 0 || Seed == UserId)
                {
                    UserId = Seed++;
                }
                else
                {
                    if (UserId > Seed)
                        Seed = UserId + 1;
                }
            }

            var Player = new Level(UserId);


            if (string.IsNullOrEmpty(token))
            {
                if (string.IsNullOrEmpty(Player.Avatar.Token))
                {
                    for (int i = 0; i < 20; i++)
                    {
                        char Letter = (char) Resources.Random.Next('A', 'Z');
                        Player.Avatar.Token += Letter;
                    }
                }
            }
            else
            {
                Player.Avatar.Token = token;
            }

            if (string.IsNullOrEmpty(Player.Avatar.Password))
            {
                for (int i = 0; i < 6; i++)
                {
                    char Letter = (char) Resources.Random.Next('A', 'Z');
                    char Number = (char) Resources.Random.Next('1', '9');
                    Player.Avatar.Password += Letter;
                    Player.Avatar.Password += Number;
                }
            }

            Player.JSON = Files.Home.Starting_Home;

            if (Store)
            {
                Add(Player);
            }

            using (MysqlEntities Database = new MysqlEntities())
            {
                Database.Player.Add(new Database.Player
                {
                    ID = Player.Avatar.UserId,
                    Avatar = JsonConvert.SerializeObject(Player.Avatar, Settings),
                    Village =  Player.JSON,
                    FacebookID = "#:#:#:#",
                });

                Database.SaveChanges();
            }
            return Player;
        }

        internal static void Save(Level Player)
        {
            Player.Avatar.LastSave = DateTime.UtcNow;

            using (MysqlEntities Database = new MysqlEntities())
            {
                Database.Configuration.AutoDetectChangesEnabled = false;
                var Data = Database.Player.Find(Player.Avatar.UserId);

                if (Data != null)
                {
                    Data.Avatar = JsonConvert.SerializeObject(Player.Avatar, Settings);
                    Data.Village = Player.JSON;
                    Data.Trophies = Player.Avatar.Trophies;
                    Data.FacebookID = Player.Avatar.Facebook.Identifier ?? "#:#:#:#";
                    Database.Entry(Data).State = EntityState.Modified;
                }
                Database.SaveChanges();
            }
        }

        internal static async Task Save()
        {
            using (MysqlEntities Database = new MysqlEntities())
            {
                foreach (var Player in Levels.Values.ToList())
                {
                    lock (Player)
                    {
                        Player.Avatar.LastSave = DateTime.UtcNow;
                        var Data = Database.Player.Find(Player.Avatar.UserId);

                        if (Data != null)
                        {
                            Data.Avatar = JsonConvert.SerializeObject(Player.Avatar, Settings);
                            Data.Village = Player.JSON;
                            Data.Trophies = Player.Avatar.Trophies;
                            Data.FacebookID = Player.Avatar.Facebook.Identifier ?? "#:#:#:#";
                        }
                        Database.SaveChanges();
                    }
                }

                await Database.BulkSaveChangesAsync();
            }
        }
    }
}
