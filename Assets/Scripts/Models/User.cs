using Boo.Lang;
using Firebase.Database;
using Newtonsoft.Json;

namespace Assets.Scripts.Models {

    public class User {

        private string _username;
        [JsonProperty(PropertyName = "username")]
        public string UserName {
            get { return _username; }
            set { _username = value.ToLower(); }
        }

        [JsonProperty(PropertyName = "displayname")]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "user_since")]
        public object UserSince {
            get { return ServerValue.Timestamp; }
        }

        [JsonProperty(PropertyName = "modified")]
        public object Modified {
            get { return ServerValue.Timestamp; }
        }

        [JsonProperty(PropertyName = "is_admin")]
        public bool isAdmin {
            get { return false; }
        }

        [JsonProperty(PropertyName = "groups")]
        public List<string> Groups { get; set; }

        [JsonIgnore] //not necessary to push with the rest of the data
        public string UserId { get; set; }

        public User(string userName, string userId) : 
            this(userName, userId, new List<string>()) { } 


        public User(string userName, string userId, List<string> groups) {
            UserName = userName;
            DisplayName = userName;
            UserId = userId;
            Groups = groups;
        }   

    }
}