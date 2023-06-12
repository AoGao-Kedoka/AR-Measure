using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    TMP_Text m_Text;
    Image m_Image;
    private void OnEnable()
    {
        m_Text = GetComponent<TMP_Text>();
        m_Image = GetComponent<Image>();
    }
    public IEnumerator FadeToFullAlpha(float t)
    {
        if (m_Text)
        {
            m_Text.color = new Color(m_Text.color.r, m_Text.color.g, m_Text.color.b, 0);
            while (m_Text.color.a < 1.0f)
            {
                m_Text.color = new Color(m_Text.color.r, m_Text.color.g, m_Text.color.b, m_Text.color.a + (Time.deltaTime / t));
                yield return null;
            }
        } else if (m_Image)
        {
            m_Image.color = new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, 0);
            while (m_Image.color.a < 1.0f)
            {
                m_Image.color = new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, m_Image.color.a + (Time.deltaTime / t));
                yield return null;
            }
        }
    }

    public IEnumerator FadeToZeroAlpha(float t)
    {
        if (m_Text)
        {
            m_Text.color = new Color(m_Text.color.r, m_Text.color.g, m_Text.color.b, 1);
            while (m_Text.color.a > 0)
            {
                m_Text.color = new Color(m_Text.color.r, m_Text.color.g, m_Text.color.b, m_Text.color.a - (Time.deltaTime / t));
                yield return null;
            }
        } else if (m_Image)
        {
            m_Image.color = new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, 1);
            while (m_Image.color.a > 0)
            {
                m_Image.color = new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, m_Image.color.a - (Time.deltaTime / t));
                yield return null;
            }
        }
    }
}
