using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public GameObject Card;

    #region 카드에 관련된 리스트/덱
    private Queue<GameObject> deck;
    private List<GameObject> disdeck;
    private List<List<GameObject>> playerhands;
    public List<GameObject> myhand = new List<GameObject>();
    #endregion

    private int playernum;
    private AuctionManager amanager;
    private Vector3 handoffset = new Vector3(740, 140, 0);

    public struct CardInfo
    {
        //카드 비용
        public int CardCost { get; private set; }
        //카드 효과
        public string CardEffect { get; private set; }
        //카드 종류 0은 소지금 증가 1은 소지금 감소 2는 턴 당 수익 증가, 3은 턴 당 수익 감소
        public int CardCategory { get; private set; }
        //증가하는 퍼센트 율
        public int CardOffset { get; private set; }

        public CardInfo(int cost = 0, string effect = "", int category = 0, int offset = 0)
        {
            CardCost = cost;
            CardEffect = effect;
            CardCategory = category;
            CardOffset = offset;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playernum = PhotonNetwork.playerList.Length;
        if (PhotonNetwork.isMasterClient)
        {
            deck = new Queue<GameObject>();
            disdeck = new List<GameObject>();
            playerhands = new List<List<GameObject>>();
            amanager = GameObject.Find("GameManager").GetComponent<AuctionManager>();

            #region InitArr
            int[] costarr = { 500, 1000, 1500, 2000, 500, 1000, 1500, 2000, 500, 1000, 1500, 2000, 500, 1000, 1500, 2000 };
            string[] effectarr =
                { "소지금 15% 증가", "소지금 30% 증가" , "소지금 45% 증가", "소지금 60% 증가",
              "상대방 소지금 10% 감소", "상대방 소지금 20%감소", "상대방 소지금 30% 감소", "상대방 소지금 40% 감소",
              "턴 당 수익 10% 증가", "턴 당 수익 20% 증가", "턴 당 수익 30% 증가", "턴 당 수익 40% 증가",
              "상대방 턴 당 수익 5% 감소", "상대방 턴 당 수익 10% 감소", "상대방 턴 당 수익 15% 감소", "상대방 턴 당 수익 20% 감소"};
            int[] categoryarr = { 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3 };
            int[] offarr = { 15, 30, 45, 60, 10, 20, 30, 40, 10, 20, 30, 40, 5, 10, 15, 20 };
            #endregion

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    GameObject tmp = Instantiate(Card, transform.position, Quaternion.identity);
                    tmp.transform.SetParent(transform);
                    CardSetter tmpsetter = tmp.GetComponent<CardSetter>();
                    if (tmpsetter == null)
                    {
                        Debug.Log(tmp.name + "은 이상 개체");
                    }
                    tmpsetter.SetMyCardInfo(new CardInfo(costarr[j], effectarr[j], categoryarr[j], offarr[j]));
                    disdeck.Add(tmp);
                }

            }

            for (int i = 0; i < playernum; i++)
                playerhands.Add(new List<GameObject>());

            if (PhotonNetwork.isMasterClient)
                CardShuffle();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (AuctionManager.IsGame && AuctionManager.GameState == 4)
        {
            if (PhotonNetwork.isMasterClient)
            {
                GameObject tmp;
                //카드 분배
                for (int i = 0; i < playernum; i++)
                {
                    for (int j = 0; playerhands[i].Count < 10 && j < 2; j++)
                    {
                        //Debug.Log("Update문 안 : \n"+amanager.GetPlayer(i).GetPlayerName() + "의 패 개수 : " + playerhands[i].Count.ToString());
                        while ((tmp = deck.Dequeue()) == null)
                            CardShuffle();
                        playerhands[i].Add(tmp);
                        disdeck.Add(tmp);
                        
                    }

                    SendCInfoArr(i);

                    /*
                    CardInfo[] tmpcard = new CardInfo[playerhands[i].Count];

                    tmpcard = ListToArr(playerhands[i]);
                    

                    //패 정보 송신
                    //amanager.SendCInfo(PhotonTargets.All, PhotonNetwork.playerList[i].NickName, tmpcard);
                    */
                }

                amanager.SendCTurn(PhotonTargets.All, amanager.GetPlayer(0).GetPlayerName());
            }
            AuctionManager.GameState = 0;
            TimeManager.lefttime = 30;
        }
    }

    #region private fuction

    private void CardShuffle()
    {
        while (PhotonNetwork.isMasterClient && disdeck.Count != 0)
        {
            int randnum = Random.Range(0, disdeck.Count - 1);
            GameObject tmp = disdeck[randnum];
            if (disdeck.Remove(tmp))
                deck.Enqueue(tmp);
        }
    }

    /*
    //게임 오브젝트 리스트를 받아 cardinfo 배열을 return
    private CardInfo[] ListToArr(List<GameObject> phand)
    {
        CardInfo[] cardInfos = new CardInfo[phand.Count];
        for(int i = 0; i<phand.Count; i++)
        {
            cardInfos[i] = phand[i].GetComponent<CardSetter>().GetCardInfo();
        }

        return cardInfos;
    }
    */

    //playerhands의 인덱스를 받아서 그 패에 있는 카드의 cardinfo 구조체를
    //각 멤버 변수마다 배열로 만들어서 rpc 함수의 매개 변수로 대입
    private void SendCInfoArr(int n)
    {
        int handnum = playerhands[n].Count;

        //Debug.Log("SendCInfoArr 안 : \n"+amanager.GetPlayer(n).GetPlayerName() + "의 패 개수 : " + handnum.ToString());

        int[] ccost = new int[handnum];
        string[] ceffect = new string[handnum];
        int[] ccategory = new int[handnum];
        int[] coffset = new int[handnum];

        for(int i = 0; i<handnum; i++)
        {
            CardInfo cinfo = playerhands[n][i].GetComponent<CardSetter>().GetCardInfo();
            ccost[i] = cinfo.CardCost;
            ceffect[i] = cinfo.CardEffect;
            ccategory[i] = cinfo.CardCategory;
            coffset[i] = cinfo.CardOffset;
        }

        amanager.SendCInfo(PhotonTargets.All, PhotonNetwork.playerList[n].NickName, ccost, ceffect, ccategory, coffset);
    }
    #endregion

    #region public function

    //패를 최신 상태로 갱신
    public void GiveCard(int[] ccost, string[] ceffect, int[] ccategory, int[] coffset)
    {
        //Debug.Log("내 패의 개수(myhand.count) : " + myhand.Count);
        for (int i = myhand.Count - 1; i >= 0; i--)
        {
            GameObject tmp = myhand[i];
            myhand.Remove(tmp);
            Destroy(tmp);
        }

        //Debug.Log("카드 받았다");
        //Debug.Log("내 패의 개수(ccost.length) : " + ccost.Length);
        Transform hand = transform.Find("MyHand");
        for(int i = 0; i<ccost.Length; i++)
        {
            GameObject tmp = Instantiate(Card, handoffset + Vector3.right * i * 100, Quaternion.identity);
            tmp.transform.SetParent(hand);
            CardInfo cinfo = new CardInfo(ccost[i], ceffect[i], ccategory[i], coffset[i]);
            tmp.GetComponent<CardSetter>().SetMyCardInfo(cinfo);
            myhand.Add(tmp);
        }
    }

    public void DestroyCard(GameObject activatedcard)
    {
        //Debug.Log("DestroyCard 전");
        myhand.Remove(activatedcard);
        for(int i = 0; i<myhand.Count; i++)
        {
            myhand[i].transform.position = handoffset + Vector3.right * i * 100;
        }
        Destroy(activatedcard);
        //Debug.Log("DestroyCard 후");
    }

    public void PlayerHandUpdate(int pindex,int cindex)
    {
        //Debug.Log("PlayerHandUpdate 전");
        GameObject tmp = playerhands[pindex][cindex];

        playerhands[pindex].Remove(tmp);
        //Debug.Log("PlayerHandUpdate 후");
    }

    #endregion
}
