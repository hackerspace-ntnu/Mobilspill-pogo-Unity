using System;
using System.Collections.Generic;
using Firebase.Database;
using Newtonsoft.Json;

namespace Assets.Scripts.Models {

    #region helperClases
    public class MemberPublic {

        [JsonProperty("score")]
        public int Score;

        [JsonProperty("member_since")]
        public object MemberSince { get; set; }

        [JsonProperty("modified")]
        public object LastModified { get; set; }

        [JsonProperty("is_owner")]
        public bool IsOwner { get; set; }

        [JsonProperty("share_position")]
        public bool SharePosition { get; set; }

        public MemberPublic() : this(0, ServerValue.Timestamp, ServerValue.Timestamp, true) { }

        public MemberPublic(int score, object memberSince, object lastModified, bool sharePosition) {
            Score = score;
            MemberSince = memberSince;
            LastModified = lastModified;
            SharePosition = sharePosition;
            
        }

    }

    public class MemberProtected {

        public MemberProtected(Position pos) {
            Position = pos;

            //TODO:change
            LastModified = ServerValue.Timestamp;
        }

        [JsonProperty("position")]
        public Position Position { get; set; }

        //currently useless - position contains timestamp
        [JsonProperty("modified")]
        public object LastModified { get; set; }


    }
    #endregion

    public class GroupMember : IDictionaryObject {

        [JsonProperty("public")]
        public MemberPublic PublicInfo { get; set; }

        [JsonProperty("protected")]
        public MemberProtected Position { get; set; }


        public GroupMember(Position pos) : this(pos, new MemberPublic()) { }
        

        public GroupMember(Position pos, MemberPublic publicInfo) {
            PublicInfo = publicInfo;
            Position = new MemberProtected(pos);
        }

        public void SetPosition(Position newPos) {
            Position.Position = newPos;
        }

        public Dictionary<string, object> ToDictionary() {
            Dictionary<string, Object> toReturn = new Dictionary<string, Object>();

            toReturn["score"] = PublicInfo.Score;
            toReturn["member_since"] = PublicInfo.MemberSince;
            toReturn["modified"] = PublicInfo.LastModified;
            toReturn["is_owner"] = PublicInfo.IsOwner;
            toReturn["share_position"] = PublicInfo.SharePosition;

            if (Position.Position != null) {
                toReturn["position"] = Position.Position.ToDictionary();
            }

            return toReturn;
        }


        public override string ToString(){
            return PublicInfo.Score + "," + PublicInfo.LastModified;
        }
    }
}