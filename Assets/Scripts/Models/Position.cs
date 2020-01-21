using System;
using System.Collections.Generic;
using Firebase.Database;
using GoShared;
using Newtonsoft.Json;

namespace Assets.Scripts.Models {

    public struct Position {

        [JsonProperty("lat")]
        public double latitude;

        [JsonProperty("lng")]
        public double longitude;

        [JsonProperty("alt")]
        public double altitude;

        private object _timestamp;
        
        [JsonProperty("timestamp")]
        public object Timestamp {
            get {
                _timestamp = ServerValue.Timestamp;
                return _timestamp; 
            }
            set { _timestamp = value; }
        }

        [JsonIgnore]
        public Coordinates Coordinates { 
            get {return new Coordinates(latitude,longitude,altitude);
           } 
            set {
                latitude = value.latitude;
                longitude = value.longitude;
                altitude = value.altitude;
            }
        }

        public Position(double lat, double lng, double alt) {
            latitude = lat;
            longitude = lng;
            altitude = alt;
            _timestamp = ServerValue.Timestamp;
        }

        public override string ToString() {
            return string.Format("lat: {0}, lng: {1}, alt: {2}", latitude, longitude, altitude);
        }
    }
}