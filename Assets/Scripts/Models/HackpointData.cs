
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Assets.Scripts.Models {
    
    [JsonObject]
    public struct HackpointData
    {
        public Dictionary<int,int> TeamScores;
        public Dictionary<string,int> PlayerHighscores;
        public Position Position;

        public static string TeamScoresRef = "TeamScores";
        public static string PlayerHighscoresRef = "PlayerHighscores";
        public static string PositionRef = "Position";
    }
}