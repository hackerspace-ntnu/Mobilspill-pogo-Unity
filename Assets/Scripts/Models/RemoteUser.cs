using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.Models {
    public class RemoteUser : MonoBehaviour
    {
        public void UpdatePosition(Position newPos)
        {
            transform.localPosition = newPos.Coordinates.convertCoordinateToVector(transform.position.y);
        }
    }
}