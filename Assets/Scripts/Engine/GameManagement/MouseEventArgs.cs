﻿using UnityEngine;

 namespace Erebos.Engine.GameManagement
{
    public class MouseEventArgs
    {
        public Vector3 Point { get; set; }
        public Vector3 Normal { get; set; }
        public IInteractable Interactable { get; set; }

        public override string ToString()
        {
            return $"Mouse event at position '{Point}' with normal '{Normal}'";
        }
    }
}
