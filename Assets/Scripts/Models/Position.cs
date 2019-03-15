using System;
using GoShared;
using Newtonsoft.Json;

namespace Assets.Scripts.Models {

    public class Position {

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

        private readonly Coordinates _coordinates;

        public Position(double lat, double lng, double alt) {
            _coordinates = new Coordinates(lat, lng, alt);
        }

        public override string ToString() {
            return string.Format("lat: {0}, lng: {1}, alt: {2}", Latitude, Longitude, Altitude);
        }
    }

}