
using System.Collections.Generic;
using Newtonsoft.Json;
using Assets.Scripts.Firebase;

namespace Assets.Scripts.Models {
    
    [JsonObject]
    public struct HackpointData
    {        
        [JsonProperty(FirebaseRefs.HackpointPlayerHighscoresRef)]
        public Dictionary<string,int> PlayerHighscores;

        [JsonProperty(FirebaseRefs.HackpointPositionRef)]
        public Position Position;

        [JsonProperty(FirebaseRefs.HackpointMinigameIndexRef)]
        public int MinigameIndex;

    }
}