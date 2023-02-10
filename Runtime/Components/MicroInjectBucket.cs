using System;
using UnityEngine;

namespace FurrFieldStudio.MicroInject.Components
{
    public class MicroInjectBucket : MonoBehaviour
    {
        public MicroInject MicroInject { get; private set; }

        private void Awake()
        {
            MicroInject = new MicroInject();
        }
    }
}