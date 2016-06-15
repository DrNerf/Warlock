using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Warlock.UI
{
    public class InputFieldNavigation : MonoBehaviour
    {

        public enum NavigationDirection { Down, Up, };

        public NavigationDirection Direction;

        private EventSystem system;

        void Start()
        {
            system = EventSystem.current;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab) && system.currentSelectedGameObject == gameObject)
            {

                Selectable next = null;

                if (Direction == NavigationDirection.Down)
                {
                    next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
                }
                else
                {
                    next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
                }

                if (next != null)
                {
                    system.SetSelectedGameObject(next.gameObject);
                    next.Select();
                    var field = next as InputField;
                    if (field != null) field.OnPointerClick(new PointerEventData(system));
                }
                //else Debug.Log("next nagivation element not found");

            }
        }
    } 
}
