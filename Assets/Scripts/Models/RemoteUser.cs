using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.Models {
    public class RemoteUser : MonoBehaviour
    {
        public Text DisplayNameText;

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

        public void Initialize(string displayname)
        {
            DisplayNameText.text = displayname;
        }

        public void UpdatePosition(Position newPos)
        {
            StartCoroutine(LerpToPosition( newPos.Coordinates.convertCoordinateToVector(transform.position.y), 0.5f));
        }
    }
}