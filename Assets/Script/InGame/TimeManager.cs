using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public Text timertext;//표시되는 타이머
    public static float lefttime;//남은 시간

	// Use this for initialization
	void Start ()
    {
        lefttime = 10;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!AuctionManager.IsGame)
            return;

        if (AuctionManager.GameState != 3 && lefttime > 0)//선택 단계가 아니라면
            lefttime -= Time.deltaTime;
        else
        {
            if(AuctionManager.GameState == 0)//카드 사용 단계라면
            {
                lefttime = 10;
                if (AuctionManager.IsHandCard)
                    GameObject.Find("GameManager").GetComponent<AuctionManager>().SendCEnd(PhotonTargets.MasterClient, 
                        AuctionManager.me.GetPlayerName(), false);
            }
            else if(AuctionManager.GameState == 1)//경매 단계라면
            {
                AuctionManager.GameState = 2;//경매가 강제 전송중 단계로 변경
                lefttime = 30;
            }
        }

        timertext.text = "남은 시간 : " + ((int)lefttime).ToString();
	}
}
