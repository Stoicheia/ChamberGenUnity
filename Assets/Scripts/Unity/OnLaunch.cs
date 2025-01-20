using System;
using UnityEngine;
using Visualisation;

namespace Unity
{
    public class OnLaunch : MonoBehaviour
    {
        [SerializeField] private string _bmpPathDest;
        /*[SerializeField] private int w;
        [SerializeField] private int h;*/
        private void Start()
        {
            DrawTests.DrawTestFull(_bmpPathDest);
        }
    }
}