using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectGetter : MonoBehaviour
{
    AuctionManager amanager;
    string targetname;

    private void Start()
    {
        amanager = GameObject.Find("GameManager").GetComponent<AuctionManager>();
        targetname = gameObject.GetComponent<PlayerInfoUpdater>().PlayerName.text;
    }

    public void dropcard(GameObject dragobj, int cindex)
    {
        if(dragobj!=null&&dragobj.tag=="Card")//카드라면 카드의 효과를 송신
        {
            Debug.Log("카드 효과 발동!");
            targetname = gameObject.GetComponent<PlayerInfoUpdater>().PlayerName.text;
            CardSetter csetter = dragobj.GetComponent<CardSetter>();
            CardManager.CardInfo cinfo = csetter.GetCardInfo();

            if (AuctionManager.me.GetMoney() < cinfo.CardCost)
                return;
            else
            {
                //카드 효과 발동
                amanager.SendCEffect(PhotonTargets.All, targetname, AuctionManager.me.GetPlayerName(),
                    cinfo.CardCost, cinfo.CardCategory, cinfo.CardOffset, cindex);
                GameObject.Find("CardManager").GetComponent<CardManager>().DestroyCard(dragobj);
            }
        }
    }
}
