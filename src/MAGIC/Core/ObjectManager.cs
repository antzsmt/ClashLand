using ClashLand.Logic;

namespace ClashLand.Core
{
    internal static class ObjectManager
    {
        private static readonly object s_sync = new object();

        public static long AllianceSeed;
        public static long AvatarSeed;
       // public static Random Random;

        public static void Initialize()
        {
            AvatarSeed = DatabaseManager.GetMaxPlayerId() + 1;
            AllianceSeed = DatabaseManager.GetMaxAllianceId() + 1;
         //   Random = new Random(DateTime.Now.ToString(CultureInfo.InvariantCulture).GetHashCode());
        }

        public static Clan CreateClan(Clan clan)
        {
            clan.Clan_ID = AllianceSeed++;

            DatabaseManager.CreateAlliance(clan);

            ResourcesManager.AddAllianceInMemory(clan);
            return clan;
        }

        public static Clan CreateClan()
        {
            var alliance = default(Clan);
            alliance = new Clan(AllianceSeed++);

            DatabaseManager.CreateAlliance(alliance);

            ResourcesManager.AddAllianceInMemory(alliance);
            return alliance;
        }

        public static Level CreateLevel(long seed, string token = "")
        {
            // Increment & manage AvatarSeed thread safely.
            lock (s_sync)
            {
                if (seed == 0 || AvatarSeed == seed)
                {
                    seed = AvatarSeed++;
                }
                else
                {
                    if (seed > AvatarSeed)
                        AvatarSeed = seed + 1;
                }
            }

            var level = new Level(seed);

            if (string.IsNullOrEmpty(token))
            {
                if (string.IsNullOrEmpty(level.Avatar.Token))
                    for (var i = 0; i < 20; i++)
                    {
                       // var Letter = (char)Random.Next('A', 'Z');
                      //  level.Avatar.Token += Letter;
                    }
            }
            else
            {
                level.Avatar.Token = token;
            }

            if (string.IsNullOrEmpty(level.Avatar.Password))
                for (var i = 0; i < 6; i++)
                {
                  //  var Letter = (char)Random.Next('A', 'Z');
                   // var Number = (char)Random.Next('1', '9');
                   // level.Avatar.Password += Letter;
                   // level.Avatar.Password += Number;
                }

            //level.Json = Home.Starting_Home;

            DatabaseManager.CreateLevel(level);

            return level;
        }

        public static Clan GetAlliance(long allianceId)
        {
            var alliance = default(Clan);

            // Try to get alliance from memory first then db.
            // Could be improved.
            if (ResourcesManager.InMemoryAlliancesContain(allianceId))
                return ResourcesManager.GetInMemoryAlliance(allianceId);

            alliance = DatabaseManager.GetClan(allianceId);

            if (alliance != null)
                ResourcesManager.AddAllianceInMemory(alliance);

            return alliance;
        }

      //  public static List<Clan> GetInMemoryAlliances()
       // {
         //   return ResourcesManager.GetInMemoryAlliances();
      //  }

        //public static Level GetRandomOnlinePlayer()
       // {
          //  var levels = ResourcesManager.GetInMemoryLevels();
            //int index = Random.Next(0, levels.Count);
           // return levels[index];
       // }


        public static void RemoveInMemoryAlliance(long id)
        {
            ResourcesManager.RemoveAllianceFromMemory(id);
        }
    }
}
