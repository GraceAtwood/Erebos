﻿namespace Erebos.Engine.GameManagement
{
    public interface IInteractable
    {
        void OnPrimaryMouseUp(MouseEventArgs mouseEventArgs);
        void OnPrimaryMouseDown(MouseEventArgs mouseEventArgs);
        void OnSecondaryMouseUp(MouseEventArgs mouseEventArgs);
        void OnSecondaryMouseDown(MouseEventArgs mouseEventArgs);
        void OnMouseHover(MouseEventArgs mouseEventArgs);
    }
}
