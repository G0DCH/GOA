using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSetter : MonoBehaviour
{
    #region public Value
    [Tooltip("효과 텍스트 뒷 배경그림")]
    public Image EffectBackGround;

    [Tooltip("효과 텍스트")]
    public Text Effect;

    [Tooltip("카드 비용")]
    public Text Cost;
    #endregion

    private CardManager.CardInfo mycardinfo;

    #region public Function
    public void SetMyCardInfo(CardManager.CardInfo cinfo)
    {
        mycardinfo = cinfo;
        Effect.text = mycardinfo.CardEffect;
        Cost.text = mycardinfo.CardCost.ToString();

        switch(mycardinfo.CardCategory)
        {
            case 0:
                EffectBackGround.color = Color.green;
                break;
            case 1:
                EffectBackGround.color = Color.red;
                break;
            case 2:
                EffectBackGround.color = Color.cyan;
                break;
            case 3:
                EffectBackGround.color = Color.magenta;
                break;
        }
    }

    public CardManager.CardInfo GetCardInfo()
    {
        return mycardinfo;
    }
    #endregion
}
