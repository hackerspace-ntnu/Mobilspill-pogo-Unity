using System;
using System.Collections.Generic;
using Firebase.Database;
using GoShared;
using Newtonsoft.Json;

namespace Assets.Scripts.Models {

    public class Position : IDictionaryObject {

        [JsonProperty("lat")]
        public double Latitude {
            get { return Coordinates.latitude; }
            set { Coordinates.latitude = value; }
        }

        [JsonProperty("lng")]
        public double Longitude {
            get { return Coordinates.longitude; }
            set { Coordinates.longitude = value; }
        }

        [JsonProperty("alt")]
        public double Altitude {
            get { return Coordinates.altitude; }
            set { Coordinates.altitude = value; }
        }

        private object _timestamp = ServerValue.Timestamp;
        [JsonProperty("timestamp")]
        public object Timestamp {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

        [JsonIgnore]
        public Coordinates Coordinates { get; set;}

        public Position(double lat, double lng, double alt) {
            Coordinates = new Coordinates(lat, lng, alt);
        }

        public Position(double lat, double lng, double alt, object timestamp) {
            Coordinates = new Coordinates(lat, lng, alt);
            Timestamp = timestamp;
        }

        public override string ToString() {
            return string.Format("lat: {0}, lng: {1}, alt: {2}", Latitude, Longitude, Altitude);
        }

        public Dictionary<string, System.Object> ToDictionary() {
            Dictionary<string, System.Object> toReturn = new Dictionary<string, System.Object>();

            toReturn["lng"] = Longitude;
            toReturn["lat"] = Latitude;
            toReturn["alt"] = Altitude;
            toReturn["timestamp"] = Timestamp;

            return toReturn;
        }
    }

}