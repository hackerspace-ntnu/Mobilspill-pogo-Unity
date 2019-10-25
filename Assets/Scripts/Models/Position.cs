using System;
using System.Collections.Generic;
using Firebase.Database;
using GoShared;
using Newtonsoft.Json;

namespace Assets.Scripts.Models {

    public class Position : IDictionaryObject {

        [JsonIgnore]
        public bool IsEmpty {
            get { return _coordinates == null; }
        }

        [JsonProperty("lat")]
        public double Latitude {
            get { return _coordinates.latitude; }
            set { _coordinates.latitude = value; }
        }

        [JsonProperty("lng")]
        public double Longitude {
            get { return _coordinates.longitude; }
            set { _coordinates.longitude = value; }
        }

        [JsonProperty("alt")]
        public double Altitude {
            get { return _coordinates.altitude; }
            set { _coordinates.altitude = value; }
        }

        private object _timestamp = ServerValue.Timestamp;
        [JsonProperty("timestamp")]
        public object Timestamp {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

        [JsonIgnore]
        private Coordinates _coordinates;

        [JsonIgnore]
        public Coordinates Coordinates { 
            get {return _coordinates;} 
            set {
                _coordinates = new Coordinates(value.latitude,value.longitude,value.altitude);
            }
        }

        public Position() { }

        public Position(double lat, double lng, double alt) {
            _coordinates = new Coordinates(lat, lng, alt);
        }

        public Position(double lat, double lng, double alt, object timestamp) {
            _coordinates = new Coordinates(lat, lng, alt);
            Timestamp = timestamp;
        }

        public override string ToString() {
            return IsEmpty ? "null" : string.Format("lat: {0}, lng: {1}, alt: {2}", Latitude, Longitude, Altitude);
        }

        public Dictionary<string, System.Object> ToDictionary() {
            Dictionary<string, System.Object> toReturn = new Dictionary<string, System.Object>();

            toReturn["lng"] = Longitude;
            toReturn["lat"] = Latitude;
            toReturn["alt"] = Altitude;
            toReturn["timestamp"] = Timestamp;

            return toReturn;
        }

        public void FromDictionary(Dictionary<string, System.Object> dictionary)
        {
            Longitude = _objectToDouble(dictionary["lng"]);
            Altitude = _objectToDouble(dictionary["alt"]);
            Latitude = _objectToDouble(dictionary["lat"]);
        }

        private double _objectToDouble(object o) {
            double d = 0d;
            IConvertible convert = o as IConvertible;

            if (convert != null) {
                d = convert.ToDouble(null);
            } 
            return d;
        }
    }


}