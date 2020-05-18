using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUpdater : MonoBehaviour
{
    #region 플레이어 정보 표시 GameObj
    public Text PlayerName;
    public Text PlayerMoney;
    public Text PlayerTMoney;
    public Text PlayerLandNum;
    public Text ChangedText;
    public RawImage BackGroundImage;
    public int PNumber = 1;//플레이어 번호 1~4까지 존재
    #endregion

    #region Private Values
    private AuctionManager auctionManager;
    private bool isposchanged = false;
    private Transform highlight;
    private int lastmoney;
    private int lastTmoney;
    private Color imagecolor;
    #endregion

    #region Unity CallBacks
    private void Start()
    {
        auctionManager = GameObject.Find("GameManager").GetComponent<AuctionManager>();
        highlight = GameObject.Find("HighLight").transform;

        imagecolor = BackGroundImage.color;
        /*
        switch(PNumber)
        {
            case 1:
                gameObject.GetComponent<Renderer>().material.color = Color.red;
                break;
            case 2:
                gameObject.GetComponent<Renderer>().material.color = Color.green;
                break;
            case 3:
                gameObject.GetComponent<Renderer>().material.color = Color.yellow;
                break;
            case 4:
                gameObject.GetComponent<Renderer>().material.color = Color.blue;
                break;
        }
         */
    }

    private void Update()
    {
        PlayerInfo tmpplayer = auctionManager.GetPlayer(PNumber - 1);

        if (tmpplayer.GetPlayerName() == "")
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            if (!isposchanged && tmpplayer.GetPlayerName() == AuctionManager.me.GetPlayerName())
            {
                GameObject mypanel = GameObject.Find("MyInfo");

                transform.SetParent(mypanel.transform);
                transform.position = transform.parent.position;
                isposchanged = true;
            }
        }
        int tmpmoney;
        if (lastmoney != (tmpmoney = tmpplayer.GetMoney()))
        {
            Text diffmoney = Instantiate(ChangedText, PlayerMoney.transform.position, Quaternion.identity);
            diffmoney.transform.SetParent(transform.parent.parent, true);
            int diff =  tmpmoney - lastmoney;
            if (diff > 0)
                diffmoney.text = "+" + diff.ToString();
            else
                diffmoney.text = diff.ToString();
        }
        int tmptmoney;
        if(lastTmoney!=(tmptmoney = tmpplayer.GetTMoney()))
        {
            Text difftmoney = Instantiate(ChangedText, PlayerTMoney.transform.position, Quaternion.identity);
            difftmoney.transform.SetParent(transform.parent.parent, true);

            int diff = tmptmoney - lastTmoney;
            if (diff > 0)
                difftmoney.text = "+" + diff.ToString();
            else
                difftmoney.text = diff.ToString();
        }

        PlayerName.text = tmpplayer.GetPlayerName();
        PlayerMoney.text = tmpplayer.GetMoney().ToString();
        PlayerTMoney.text = "+" + tmpplayer.GetTMoney().ToString();
        PlayerLandNum.text = "땅 개수:" + tmpplayer.GetLandNum().ToString();

        lastmoney = tmpplayer.GetMoney();
        lastTmoney = tmpplayer.GetTMoney();

        if(auctionManager.turnpname==PlayerName.text)
        {
            highlight.transform.position = transform.position;
            highlight.transform.SetParent(transform.parent.parent);
            highlight.transform.SetAsFirstSibling();
        }

        if(auctionManager.IsPlayersPlayingCard[PNumber-1])
        {
            BackGroundImage.color = new Color(imagecolor.r, imagecolor.g, imagecolor.b, 1);
        }
        else
            BackGroundImage.color = new Color(imagecolor.r, imagecolor.g, imagecolor.b, 0.5f);
    }
    #endregion

}
