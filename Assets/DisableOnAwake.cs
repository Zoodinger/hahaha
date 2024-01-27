using UnityEngine;

namespace Hahaha {
    public class DisableOnAwake : MonoBehaviour {
        private void Awake() {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
