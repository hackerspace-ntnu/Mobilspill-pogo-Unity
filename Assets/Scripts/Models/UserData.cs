

using Newtonsoft.Json;

namespace Assets.Scripts.Models {
    public struct UserData
    {
        public string UserId;

        [JsonProperty(PropertyName = "username")]
        public string Username;
        

        [JsonProperty(PropertyName = "score")]
        public int Score;
    }
}