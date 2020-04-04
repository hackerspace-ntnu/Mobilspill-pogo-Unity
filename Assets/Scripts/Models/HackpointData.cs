
using System.Collections.Generic;
using Newtonsoft.Json;
using Assets.Scripts.Firebase;

namespace Assets.Scripts.Models {
    
    [JsonObject]
    public struct HackpointData
    {
        [JsonProperty(FirebaseRefs.HackpointTeamScoresRef)]
        public Dictionary<int,int> TeamScores;
        
        [JsonProperty(FirebaseRefs.HackpointPlayerHighscoresRef)]
        public Dictionary<string,int> PlayerHighscores;

        [JsonProperty(FirebaseRefs.HackpointPositionRef)]
        public Position Position;

        [JsonProperty(FirebaseRefs.HackpointMinigameIndexRef)]
        public int MinigameIndex;

    }
}