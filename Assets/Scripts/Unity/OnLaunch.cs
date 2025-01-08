using System;
using UnityEngine;

namespace Unity
{
    public class OnLaunch : MonoBehaviour
    {
        [SerializeField] private string _bmpPathDest;
        [SerializeField] private int w;
        [SerializeField] private int h;
        private void Start()
        {
            MapDrawer mapDrawer = new MapDrawer();
            mapDrawer.DrawMap(_bmpPathDest, w, h);
        }
    }
}