using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;


namespace Assets.Scripts.Models {
    public class RemoteUser : MonoBehaviour
    {
        public Text DisplayNameText;
        string text;
        bool changed = false;

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

        public void SetDisplayName(string displayname)
        {
            changed = true;
            text = displayname;
        }

        public void UpdatePosition(object sender, ValueChangedEventArgs args )
        {
            if (args.DatabaseError != null)
            {
                Debug.LogWarning(args.DatabaseError.ToString());
                return;
            }
            var pos = new Position();
            pos.FromDictionary(args.Snapshot.Value as Dictionary<string, object>);
            StartCoroutine(LerpToPosition( pos.Coordinates.convertCoordinateToVector(transform.position.y), 0.5f));
        }
    }
}