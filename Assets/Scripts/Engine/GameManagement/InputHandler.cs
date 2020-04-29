using UnityEngine;

namespace Erebos.Engine.GameManagement
{
    public class InputHandler : MonoBehaviour
    {
        private void Update()
        {
            HandleMouseEvents();
        }

        private static void HandleMouseEvents()
        {
            if (!RaycastMouseToInteractable(out var mouseEventInfo))
                return;

            // Here we know that the mouse is at least over an interactable object.
            // Now let's determine what the mouse was doing when it was over an interactable object.
            if (Input.GetMouseButtonDown(0))
                mouseEventInfo.Interactable.OnPrimaryMouseDown(mouseEventInfo);
            else if (Input.GetMouseButtonUp(0))
                mouseEventInfo.Interactable.OnPrimaryMouseUp(mouseEventInfo);
            else if (Input.GetMouseButtonDown(1))
                mouseEventInfo.Interactable.OnSecondaryMouseUp(mouseEventInfo);
            else if (Input.GetMouseButtonDown(1))
                mouseEventInfo.Interactable.OnSecondaryMouseDown(mouseEventInfo);
            else
                // The mouse was over the object without interacting with it.
                mouseEventInfo.Interactable.OnMouseHover(mouseEventInfo);
        }

        private static bool RaycastMouseToInteractable(out MouseEventArgs mouseEventArgs)
        {
            mouseEventArgs = null;

            var ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

            if (!Physics.Raycast(ray, out var hit))
                return false;

            var interactable = hit.collider.gameObject.GetComponent<IInteractable>();
            if (interactable == null)
                return false;

            mouseEventArgs = new MouseEventArgs
            {
                Interactable = interactable,
                Normal = hit.normal,
                Point = hit.point
            };
            return true;
        }
    }
}