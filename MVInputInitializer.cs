using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Movements.XR.HoloLens
{
    public class MVInputInitializer : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            MVInput.Initialize();
        }

    }
}