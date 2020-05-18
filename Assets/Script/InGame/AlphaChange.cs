using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaChange : MonoBehaviour
{
    private Image image;
    private bool isIncrease = false;
    private float alpha = 1;

    private void Start()
    {
        image = GetComponent<Image>();
        StartCoroutine(alphachange());
    }

    IEnumerator alphachange()
    {
        while(true)
        {
            
            if (isIncrease)
            {
                alpha += 0.05f;
                if (alpha >= 1)
                    alpha = 1;
            }

            else
            {
                alpha -= 0.05f;
                if (alpha <= 0.4f)
                    alpha = 0.4f;
            }

            image.color = new Color(255, 255, 0, alpha);

            if (alpha == 1 || alpha == 0.4f)
                isIncrease = !isIncrease;

            yield return new WaitForSeconds(0.05f);
        }
    }
}
