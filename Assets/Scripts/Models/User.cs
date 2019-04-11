using System.Collections.Generic;
using Assets.Scripts.Firebase;
using Firebase.Database;
using GoShared;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.Models {

    public class User : IDictionaryObject {

        private string _username;
        [JsonProperty(PropertyName = "username")]
        public string UserName {
            get { return _username; }
            set { _username = value.ToLower(); }
        }

        [JsonProperty(PropertyName = "displayname")]
        public string DisplayName { get; set; }

        private object _userSince;
        [JsonProperty(PropertyName = "user_since")]
        public object UserSince {
            get { return _userSince ?? ServerValue.Timestamp; }
            set { _userSince = value; }
        }

        [JsonProperty(PropertyName = "modified")]
        public object Modified {
            get { return ServerValue.Timestamp; }
        }

        [JsonProperty(PropertyName = "total_score")]
        public int TotalScore { get; set; }

        [JsonProperty(PropertyName = "is_admin")]
        public bool IsAdmin {
            get { return false; }
        }

        [JsonProperty(PropertyName = "groups")]
        public List<string> Groups { get; set; }

        [JsonIgnore] //not necessary to push with the rest of the data
        public string UserId { get; set; }

        //GroupId: GroupMembership
        [JsonIgnore] public Dictionary<string, GroupMember> GroupMemberships { get; set; }

        [JsonIgnore] public Position ServerPosition { get; set; }
        [JsonIgnore] public Position Position { get; set; }

        public User(string displayName, string userId, bool registering = false) :
            this(displayName, userId, new List<string>(), registering){ }
            
        public User(string displayName, string userId, List<string> groups, bool registering=false) {
            UserName = displayName.Replace(".", "");
            DisplayName = displayName.Replace(".", "");
            UserId = userId;
            Groups = groups;

            GroupMemberships = new Dictionary<string, GroupMember>();
            if (!registering) {
                //retrieving group memberships from server
                if (Groups.Count == 0) {
                    //hardcoded based on current server value -> joining base group
                    JoinGroup("-La2SQPdOkCSsz-Eygd");
                } else {
                    //Already member of group(s): retrieving membership info from server.
                    GetUserGroupMemberships();
                }
                
            }
        }

        public void GetUserGroupMemberships() {
            foreach (string group in Groups) {
                AuthManager.Instance.GetUserGroupMembership(group).ContinueWith(task => {
                    if (task.IsFaulted) {
                        // Handle the error...
                        Debug.Log("[User] Failed getting membership for " + group + ": " + task.Exception);
                    } else if (task.IsCompleted) {
                        Dictionary<string, object> snapshotVal = task.Result.Value as Dictionary<string, object>;

                        if (snapshotVal != null) {
                            //don't really need the position, already know what it should be.

                            //adding to GroupMemberships
                            GroupMember membership = new GroupMember(Position, new MemberPublic((int)snapshotVal["score"], snapshotVal["member_since"], snapshotVal["modified"], (bool)snapshotVal["share_position"]));
                            GroupMemberships.Add(group, membership);
                        }
                    }
                });
            }
        }

        public void JoinGroup(string groupId) {
            Debug.Log("[User] joining group: " + groupId);

            GroupMember membership = new GroupMember(Position);

            //pushing to server
            string newGroupKey = RealtimeDatabaseManager.Instance.DBReference.Child("users/" + UserId + "/groups").Push().Key;

            Dictionary<string, System.Object> groupDict = new Dictionary<string, System.Object>();
            groupDict["/users/" + UserId + "/groups/" + newGroupKey] = groupId;

            Dictionary<string, System.Object> memberDict = membership.ToDictionary();
            Debug.Log("[User] groupDict: " + membership);

            groupDict["/groups/map/" + groupId + "/protected/members/" + UserId] = membership.ToDictionary();


            RealtimeDatabaseManager.Instance.DBReference.UpdateChildrenAsync(groupDict)
                .ContinueWith(res => {

                    if (res.IsCompleted) {
                        //updating user Group list and memberships in app
                        Groups.Add(groupId);
                        GroupMemberships.Add(groupId, membership);

                        //todo: react to this change in-game
                        Debug.Log("group joined successfully!");
                    } else {
                        Debug.LogError("Something went wrong while joining group. Fuck! error: " + res.Exception);
                    }

                }
            );
        }

        public void OnLocationChanged(Coordinates newPos) {
            Position.Coordinates = newPos;
            
            //I think this gives me distance in meters. If more than 10 meter difference: update and push to server.
            if (ServerPosition != null && ServerPosition.Coordinates.DistanceFromOtherGPSCoordinate(newPos) > 10) {
                ServerPosition.Coordinates = newPos;

                foreach (KeyValuePair<string, GroupMember> membership in GroupMemberships) {
                    
                }
            }

            //todo: push to server for all group memberships
        }

        public Dictionary<string, System.Object> ToDictionary() {
            Dictionary<string, System.Object> toReturn = new Dictionary<string, System.Object>();

            toReturn["username"] = UserName;
            toReturn["displayname"] = DisplayName;
            toReturn["user_since"] = UserSince;
            toReturn["modified"] = Modified;
            toReturn["total_score"] = TotalScore;
            toReturn["is_admin"] = IsAdmin;
            if (Groups.Count > 0) {
                toReturn["groups"] = Groups;
            }

            return toReturn;
        }

        public override string ToString() {
            return UserName;
        }
    }
}