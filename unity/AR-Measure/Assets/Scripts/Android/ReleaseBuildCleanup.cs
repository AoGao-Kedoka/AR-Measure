using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReleaseBuildCleanup : MonoBehaviour
{
    /// <summary>
    /// Elements that should be disabled in release build
    /// </summary>
    [SerializeField]
    List<GameObject> elements = new List<GameObject>();

    private void Awake()
    {
        if (!Debug.isDebugBuild)
        {
            elements.ForEach(e => { e.SetActive(false); });
        }
    }
}
