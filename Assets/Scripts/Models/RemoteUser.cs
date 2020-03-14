using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Models {
    public class RemoteUser : MonoBehaviour
    {
        public Text DisplayNameText;
        private string text;
        private bool changed = false; 

        private IEnumerator LerpToPosition(Vector3 position, float time)
        {
            float elapsedTime = 0;
            Vector3 previousPosition = transform.localPosition;

            while (elapsedTime < time)
            {
                transform.localPosition = Vector3.Lerp(previousPosition, position, elapsedTime/time);
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        void Update()
        {
            if (changed)
            {
                DisplayNameText.text = text;
                changed = false;
            }
        }

        public void InitializeData(string displayname, Position position)
        {
            changed = true;
            text = displayname;
            transform.position = position.Coordinates.convertCoordinateToVector(transform.position.y);
        }
        public void UpdatePosition(Position pos, float time )
        {
            StartCoroutine(LerpToPosition( pos.Coordinates.convertCoordinateToVector(transform.position.y), time));
        }
    }
}