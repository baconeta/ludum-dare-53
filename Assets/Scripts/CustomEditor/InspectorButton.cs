using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CustomEditor
{
    public class InspectorButton : MonoBehaviour
    {
        public string buttonLabel = "Button";
        public UnityEvent onClickEvent;
    }    
}

