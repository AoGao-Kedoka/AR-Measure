using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DistanceDisplay : MonoBehaviour
{
    /// <summary>
    /// Text that distance should be updated to
    /// </summary>
    [SerializeField]
    TMP_Text m_DistanceText;

    /// <summary>
    /// Reference to the main camera
    /// </summary>
    private Camera m_Cam;

    private void OnEnable()
    {
        m_Cam = Camera.main;
    }

    public void UpdateText(string text)
    {
        m_DistanceText.text = text;
    }

    private void Update()
    {
        this.transform.LookAt(2 * transform.position - m_Cam.transform.position);
    }
}
