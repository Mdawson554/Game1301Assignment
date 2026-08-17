using UnityEngine;
using UnityEngine.WSA;

public class PlayerInteractor : MonoBehaviour, IInteractable
    {
        public IInteractable CurrentInteractable;

        private void OnTriggerEnter(Collider other)
        {
            var interactableinterface = other.TryGetComponent(out IInteractable interactable);
            if (interactableinterface)
            {
                CurrentInteractable = interactable;
            }
        }

        public void OnInteract()
        {
            Debug.Log("interact");
        }
    }