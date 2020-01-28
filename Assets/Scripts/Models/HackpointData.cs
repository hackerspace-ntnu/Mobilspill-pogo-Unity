
using Newtonsoft.Json;

namespace Assets.Scripts.Models {
    
    [JsonObject]
    public struct HackpointData
    {
        [JsonIgnore]
        public string ID;
        public int Highscore;
        public string HighscoringTeam;
        public Position Position;
        //public string[] UsersAtHackpoint;
    }
}