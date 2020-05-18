using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuctionManager : Photon.MonoBehaviour
{
    private LandConstructor lconstructor;
    private CardManager cmanager;
    private SourceInfo source;//현재 경매에 나와있는 자원
    private GameObject startbutton;
    private GameObject endbutton;
    private GameObject ainfobutton;
    private GameObject oplayerbutton;
    private GameObject playerfield;
    private GameObject help;
    private GameObject chatfield;
    public Text auctiontext;//경매 정보 표시
    public InputField chatinputfield;
    public GameObject UsedCardLog;//카드 사용 기록
    public Text ChatBox;

    private Text systemtext;//플레이어의 차례
    private Text turntext;//현재 턴
    private Text roundtext;//현재 라운드
    private int price;//내 입찰가
    public static int GameState = 0;//게임 상태
                                    //0이면 카드 사용
                                    //1이면 경매 중
                                    //2이면 경매 제한 시간 종료로 인한 자동 입찰가 전송
                                    //3이면 선택 중
                                    //4이면 카드 분배 중
    //public static bool IsAuction = true;//경매중인지 확인
    //public static bool IsSelect = false;
    public static bool IsGame = false;//게임 중인지 확인
    //public static bool IsCard = false;//카드 사용중인지 확인
    public static bool IsHandCard = false;//내 카드 내는 차례인지 확인
    public bool[] IsPlayersPlayingCard;//카드 사용 단계 완료인지 확인
    public string turnpname { get; private set; }
    private int sendpnum = 0;//입찰가를 보낸 사람 수
    private int lnum = -1;//보드판에 있는 땅 수
    private int maxlandnum = 0;//현재 가장 많은 땅을 소지한 플레이어의 땅 갯수
    private int []playersprice;//다른 플레이어들의 입찰가 마스터 클라이언트만 사용
    public static bool IsWaitMaxPlayer = false;

    private PhotonPlayer[] players;

    //경매 정보 위치
    private Vector3 AInfopos;

    #region 플레이어 정보
    private PlayerInfo[] pinfo;
    public static PlayerInfo me;
    const string DefaultName = "";//기본 이름
    const int DefaultMoney = 9500;//시작 금액
    const int DefaultTMoney = 500;//시작 턴 수익
    private int maxprice = 0;//최대 입찰가.
    public static string maxplayer = string.Empty;//최대 입찰자
    #endregion

    // Use this for initialization
    void Start ()
    {
        lconstructor = GameObject.Find("LandConstructor").GetComponent<LandConstructor>();
        cmanager = GameObject.Find("CardManager").GetComponent<CardManager>();
        startbutton = GameObject.Find("StartButton");
        endbutton = GameObject.Find("TurnEnd");
        endbutton.SetActive(false);
        oplayerbutton = GameObject.Find("OtherPlayersButton");
        oplayerbutton.SetActive(false);
        ainfobutton = GameObject.Find("AuctionInfoButton");
        ainfobutton.SetActive(false);
        playerfield = GameObject.Find("PField");
        help = GameObject.Find("Help");
        help.transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>().value = 1;
        help.SetActive(false);
        chatfield = GameObject.Find("ChatField");
        turnpname = "";
        systemtext = GameObject.Find("SystemText").GetComponent<Text>();
        turntext = GameObject.Find("TurnText").GetComponent<Text>();
        roundtext = GameObject.Find("RoundText").GetComponent<Text>();
        IsGame = false;
        //IsAuction = true;
        //IsSelect = false;
        pinfo = new PlayerInfo[PhotonNetwork.playerList.Length];
        price = 0;
        lnum = -1;

        //처음 위치 저장
        AInfopos = auctiontext.transform.parent.position;

        InitPlayer();

        if (players.Length == 1 || !PhotonNetwork.isMasterClient)
            startbutton.SetActive(false);

        IsPlayersPlayingCard = new bool[players.Length];
        for (int i = 0; i < pinfo.Length; i++)
            IsPlayersPlayingCard[i] = true;
        if (PhotonNetwork.isMasterClient)
        {
            lconstructor.ChangeSource();
            playersprice = new int[players.Length];
            source = lconstructor.GetSource();//현재 경매에 나온 자원을 받아옴.
            SendSAuction(PhotonTargets.All, source.SourceName, source.SourceMoney);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsGame || players.Length == 1)
        {
            if (lnum == 25 || maxlandnum >= 13)//만약 보드 판이 꽉 찼거나 한 사람이 땅을 13개 모았을 경우
            {
                string winner = "";
                int winnernum = 0;
                for (int i = 0; i < players.Length; i++)//승리자를 고르고 출력
                {
                    if (winnernum < pinfo[i].GetLandNum())
                    {
                        winnernum = pinfo[i].GetLandNum();
                        winner = pinfo[i].GetPlayerName();
                    }
                }

                auctiontext.text = "축하합니다!" +
                                   "\n승자 : " + winner +
                                   "\n소지한 땅 개수 : " + winnernum.ToString();
            }
            return;
        }
        switch (GameState)
        {
            case 0:
                turntext.text = "Card Turn";

                auctiontext.text = "개발 자원 : " + source.SourceName +
                                   "\n수익 : " + source.SourceMoney +
                                   "\n\n내 입찰가 : " + price.ToString();

                if (turnpname == me.GetPlayerName())
                    systemtext.text = "내가 카드를 사용 할 차례 입니다.";
                else
                    systemtext.text = turnpname + "가 카드를 사용 할 차례 입니다.";
                break;

            case 1://경매 진행중이라면
                if (!chatinputfield.isFocused)
                    switch (Input.inputString)//입찰가 설정
                    {
                        case "1":
                            price += 1;
                            CheckPrice();
                            break;
                        case "2":
                            price += 10;
                            CheckPrice();
                            break;
                        case "3":
                            price += 100;
                            CheckPrice();
                            break;
                        case "4":
                            price += 1000;
                            CheckPrice();
                            break;
                        case "5":
                            price += 10000;
                            CheckPrice();
                            break;
                        case "0":
                            price = 0;
                            break;
                    }
                auctiontext.text = "개발 자원 : " + source.SourceName +
                                   "\n수익 : " + source.SourceMoney +
                                   "\n\n내 입찰가 : " + price.ToString();
                turntext.text = "Auction Turn";
                //Debug.Log("경매 턴");
                systemtext.text = "1 ~ 5를 입력 하시면 각각 1 ~ 10000만큼 입찰가 증가. 0은 초기화";
                break;
            case 2://경매 제한 시간 종료
                //auctiontext.text = "경매 종료!";
                SendP(PhotonTargets.MasterClient, me.GetPlayerName(), price);
                GameState = 3;
                break;
            case 3:
                if (IsWaitMaxPlayer)
                {
                    auctiontext.text = "다른 플레이어\n기다리는 중\n 내 입찰가 : " + price.ToString();
                    //Debug.Log("다른 플레이어 대기중");
                }
                else
                {
                    //Debug.Log("선택 턴");
                    turntext.text = "Select Turn";
                    auctiontext.text = "경매 종료!\n경매승자 : " + maxplayer + "\n입찰가 : " + maxprice;
                    systemtext.text = turnpname + "님이 타일을 선택 할 차례 입니다.";
                    turnpname = maxplayer;
                }
                break;
        }

    }

    void CheckPrice()//입찰가와 내 최대 소지금을 비교해서 입찰가가 내 최대 소지금을 넘어갔으면 최대 소지금으로 바꿈.
    {
        if (price > me.GetMoney())//입찰가> 최대 소지금으로 바꿔라
            price = me.GetMoney();
    }

    public PlayerInfo GetPlayer(int n)
    {
        if (n >= pinfo.Length)
            return new PlayerInfo(DefaultName, DefaultMoney, DefaultTMoney, 0);
        else
            return pinfo[n];
    }

    private void InitPlayer()
    {
        players = PhotonNetwork.playerList;//현재 방 참가자들을 갖고옴.

        string tmpname;

        for (int i = 0; i < players.Length; i++)//플레이어 정보 초기화
        {
            tmpname = players[i].NickName;
            if (tmpname != null)
                pinfo[i].SetPlayerName(tmpname);
            else
                pinfo[i].SetPlayerName(DefaultName);
            if (tmpname == PhotonNetwork.player.NickName)
            {
                me.SetPlayerName(tmpname);
                me.SetMoney(DefaultMoney);
                me.SetTMoney(DefaultTMoney);
                me.SetLandNum(0);
            }
            pinfo[i].SetMoney(DefaultMoney);
            pinfo[i].SetTMoney(DefaultTMoney);
            pinfo[i].SetLandNum(0);
        }
    }

    //n이 0이면 기본 위치, n이 1이면 중앙, n이 2이면 playerfield false
    private void AInfoMove(int n)
    {
        Transform ainfotrans = auctiontext.transform.parent;

        if (n == 1)
        {
            ainfotrans.localPosition = Vector3.zero;
            ainfotrans.localScale = Vector3.one;
            playerfield.SetActive(false);
        }
        else if (n == 0 || n == 2)
        {
            ainfotrans.position = AInfopos;
            ainfotrans.localScale = Vector3.one * 0.6f;
            if (n == 0)
                playerfield.SetActive(true);
            else
                playerfield.SetActive(false);
        }
    }

    #region 버튼 함수
    public void StartGame()
        //2인 이상부터 게임 시작 가능
    {
        SendSGame(PhotonTargets.All);
        PhotonNetwork.room.IsOpen = false;//게임 시작했으니 랜덤 매칭 거절
    }

    public void TurnEnd()
    {
        endbutton.SetActive(false);
        if (IsHandCard)
        {
            SendCEnd(PhotonTargets.MasterClient, me.GetPlayerName(), true);
        }
        else if(GameState == 1)
        {
            IsWaitMaxPlayer = true;
            GameState = 3;//선택 상태로 변경
            SendP(PhotonTargets.MasterClient, me.GetPlayerName(), price);
        }
    }

    //도움말
    public void Help()
    {
        help.SetActive(!help.activeSelf);
        chatfield.SetActive(!chatfield.activeSelf);
    }

    //경매 정보 보기
    public void ShowAuctionInfo()
    {
        GameObject tmp = auctiontext.transform.parent.gameObject;
        bool isactive = tmp.activeSelf;
        tmp.SetActive(!isactive);
    }

    //플레이어 정보 보기
    public void ShowOtherPlayers()
    {
        playerfield.SetActive(!playerfield.activeSelf);
    }
    #endregion
    /*
    public void ViewChange(int mod = 0)
    {
        GameObject ainfo = auctiontext.gameObject.transform.parent.gameObject;//경매 정보
        GameObject lands = GameObject.Find("Lands");//보드판

        if(ainfo!=null&&lands!=null)
        {
            Vector3 boardpos_big = new Vector3(990f, 540.14f, 0f);
            Vector3 boardpos_mini = new Vector3(995.35f, 540.53f, 0f);
            Vector3 ainfopos_big = new Vector3(0f, 0.6f, 0f);
            Vector3 ainfopos_mini = new Vector3(721f, 70f, 0f);
            switch (mod)//0이면 버튼 기능, 1이면 보드판 중앙 위치, 2이면 경매창 중앙 위치
            {
                case 0://버튼을 누르면 보드판과 경매 정보 위치 교체
                    IsViewMod = !IsViewMod;
                    if(IsViewMod)//보드판이 가운데라면
                    {
                        ainfo.transform.localPosition = ainfopos_mini;
                        ainfo.transform.localScale = Vector3.one * 0.5f;
                        lands.transform.position = boardpos_big;
                        lands.transform.localScale = Vector3.one * 0.8f;
                    }
                    else//경매창이 가운데라면
                    {
                        ainfo.transform.localPosition = ainfopos_big;
                        ainfo.transform.localScale = Vector3.one * 1.2f;
                        lands.transform.position = boardpos_mini;
                        lands.transform.localScale = Vector3.one * 0.3f;
                    }
                    break;
                case 1:
                    IsViewMod = true;
                    ainfo.transform.localPosition = ainfopos_mini;
                    ainfo.transform.localScale = Vector3.one * 0.5f;
                    lands.transform.position = boardpos_big;
                    lands.transform.localScale = Vector3.one * 0.8f;
                    break;
                case 2:
                    IsViewMod = false;
                    ainfo.transform.localPosition = ainfopos_big;
                    ainfo.transform.localScale = Vector3.one * 1.2f;
                    lands.transform.position = boardpos_mini;
                    lands.transform.localScale = Vector3.one * 0.3f;
                    break;
            }
        }
        else
        {
            Debug.Log("ViewChange Failed. AuctionInfo or Lands Object can't be found.");
        }
    }
    */

    void UseCardEffect(string pname, string usedpname , int ccost, int ccategory, int coffset)
    {
        int i, j;
        int tmp = 0;
        string subtarget = string.Empty;
        for(i = 0; i<players.Length; i++)
        {
            if (pinfo[i].GetPlayerName() == pname)
                break;
        }

        for(j = 0; j<players.Length; j++)
        {
            if (pinfo[j].GetPlayerName() == usedpname)
                break;
        }

        tmp = pinfo[j].GetMoney();
        pinfo[j].SetMoney(pinfo[j].GetMoney() - ccost);

        switch(ccategory)
        {
            case 0://소지금 증가
                //tmp = pinfo[j].GetMoney();
                pinfo[i].SetMoney((int)(pinfo[i].GetMoney() * (100 + coffset) / 100.0));
                subtarget = "소지금";
                tmp = pinfo[i].GetMoney() - tmp;
                //Debug.Log(pinfo[j].GetPlayerName() + "의 소지금" + pinfo[j].GetMoney().ToString() + "으로 변경");
                break;
            case 1://소지금 감소
                tmp = pinfo[i].GetMoney();
                pinfo[i].SetMoney((int)(pinfo[i].GetMoney() * (100 - coffset) / 100.0));
                subtarget = "소지금";
                tmp = pinfo[i].GetMoney() - tmp;
                //Debug.Log(pinfo[i].GetPlayerName() + "의 소지금" + pinfo[i].GetMoney().ToString() + "으로 변경");
                break;
            case 2://턴 당 수익 증가
                tmp = pinfo[i].GetTMoney();
                pinfo[i].SetTMoney((int)(pinfo[i].GetTMoney() * (100 + coffset) / 100.0));
                subtarget = "수익";
                tmp = pinfo[i].GetTMoney() - tmp;
                //Debug.Log(pinfo[j].GetPlayerName() + "의 턴수익" + pinfo[j].GetTMoney().ToString() + "으로 변경");
                break;
            case 3://턴 당 수익 감소
                tmp = pinfo[i].GetTMoney();
                pinfo[i].SetTMoney((int)(pinfo[i].GetTMoney() * (100 - coffset) / 100.0));
                subtarget = "수익";
                tmp = pinfo[i].GetTMoney() - tmp;
                //Debug.Log(pinfo[i].GetPlayerName() + "의 턴수익" + pinfo[i].GetTMoney().ToString() + "으로 변경");
                break;
        }

        if(pinfo[j].GetPlayerName()==me.GetPlayerName())
        {
            me.SetMoney(pinfo[j].GetMoney());
            me.SetTMoney(pinfo[j].GetTMoney());
        }

        if(pinfo[i].GetPlayerName()==me.GetPlayerName())
        {
            me.SetMoney(pinfo[i].GetMoney());
            me.SetTMoney(pinfo[i].GetTMoney());
        }

        //로그 메시지 출력
        GameObject tmpobj = Instantiate(UsedCardLog);

        tmpobj.transform.parent = GameObject.Find("AuctionUI").transform;
        tmpobj.transform.localPosition = new Vector3(0, 300, 0);
        tmpobj.GetComponent<PlayerLogUpdater>().UpdateText(usedpname, pname, subtarget, tmp);

        if (usedpname == me.GetPlayerName())
            SendCEnd(PhotonTargets.MasterClient, usedpname, false);
    }

    #region PRC 함수
    [PunRPC]
    void SendPrice(string pname, int pri)//최대입찰가와 입찰자 갱신
    {
        if (maxprice < pri)
        {
            maxprice = pri;
            maxplayer = pname;
        }
        sendpnum++;

        for(int i = 0; i<players.Length; i++)
        {
            if (pname == pinfo[i].GetPlayerName())//다른 플레이어의 입찰가 기록
            {
                playersprice[i] = pri;
            }
        }

        if(sendpnum==players.Length)//다 받았다면 모든 플레이어에게 최대 입찰자 알림
        {
            string tmppris = string.Empty;//다른 플레이어들에게 보낼 입찰 정보
            for(int i = 0; i<playersprice.Length; i++)//타 플레이어의 입찰 정보를 문자열로 변경
            {
                tmppris += pinfo[i].GetPlayerName() + " : " + playersprice[i].ToString() + "\n";
            }
            SendM(PhotonTargets.All, maxplayer, maxprice, tmppris);
            sendpnum = 0;
        }
    }

    [PunRPC]
    void SendMaxPlayer(string pname, int pri, string pris)//땅 선택
    {
        maxprice = pri;
        maxplayer = pname;
        lconstructor.SetOwner(maxplayer);
        TimeManager.lefttime = 30;
        IsWaitMaxPlayer = false;

        AInfoMove(2);

        //pricetext.text = pris;//다른 플레이어들의 경매가 출력
        ChatBox.text += pris;

        //isrotateturnstate = false;

        for (int i = 0; i < players.Length; i++)
        {
            if (pinfo[i].GetPlayerName() == maxplayer)
            {
                //plandnum[i]++;
                pinfo[i].SetMoney(pinfo[i].GetMoney() - maxprice);
                pinfo[i].SetTMoney(pinfo[i].GetTMoney() + source.SourceMoney);
                pinfo[i].SetLandNum(pinfo[i].GetLandNum() + 1);
                //Debug.Log(pinfo[i].GetPlayerName() + "의 땅 개수 : " + pinfo[i].GetLandNum());
                break;
            }
        }
    }

    [PunRPC]
    void RestartAuction()
    {
        lconstructor.ChangeSource();
        source = lconstructor.GetSource();
        SendSAuction(PhotonTargets.All, source.SourceName, source.SourceMoney);
    }

    [PunRPC]
    void StartAuction(string sname, int tmoney)
    {

        Debug.Log("StartAuction 함수 실행됨");
        lconstructor.setSource(sname, tmoney);
        source = lconstructor.GetSource();

        for(int i = 0; i<players.Length; i++)
        {
            pinfo[i].SetMoney(pinfo[i].GetMoney() + pinfo[i].GetTMoney());
            if(pinfo[i].GetPlayerName()==me.GetPlayerName())
            {
                me.SetMoney(pinfo[i].GetMoney());
                me.SetTMoney(pinfo[i].GetTMoney());
                me.SetLandNum(pinfo[i].GetLandNum());
            }

            if (PhotonNetwork.isMasterClient)
                IsPlayersPlayingCard[i] = true;
        }
        TimeManager.lefttime = 60;
        maxplayer = string.Empty;
        maxprice = 0;
        price = 0;
        maxlandnum = 0;

        GameObject.Find("WPText").GetComponent<Text>().text
            = "땅 : " + me.GetLandNum().ToString() + "/13";

        GameState = 4;//카드 분배 상태로 변경
        IsWaitMaxPlayer = false;
        //isrotateturnstate = false;

        AInfoMove(0);

        lnum++;

        roundtext.text = "Round" + (lnum + 1).ToString();

        //한 플레이어의 땅 갯수가 13개면 게임 종료
        for(int i = 0; i<players.Length; i++)
        {
            if (maxlandnum < pinfo[i].GetLandNum())
                maxlandnum = pinfo[i].GetLandNum();
        }
        if (maxlandnum >= 13)
            IsGame = false;
        else if (lnum == 25)//땅이 모두 찼다면 게임 종료
            IsGame = false;
    }

    [PunRPC]
    void SendSelectPos(string p, string c)
    {
        GameObject target = null;

        Debug.Log("SendSelectPos 함수 호출 됨");

        target = GameObject.Find("LandBlockManager").GetComponent<LandBlockManager>().GetClickedBlock(p, c);

        int i;
        for(i = 0; i<players.Length; i++)
        {
            if (pinfo[i].GetPlayerName() == maxplayer)
                break;
        }
        switch (i)
        {
            case 0:
                target.GetComponent<LandEvent>().SetLandColor(Color.red);
                break;

            case 1:
                target.GetComponent<LandEvent>().SetLandColor(Color.green);
                break;

            case 2:
                target.GetComponent<LandEvent>().SetLandColor(Color.yellow);
                break;

            case 3:
                target.GetComponent<LandEvent>().SetLandColor(Color.blue);
                break;
        }

        target.GetComponent<LandEvent>().SetLandSource(source, maxplayer);
    }
    [PunRPC]
    void SendStartGame()
    {
        IsGame = true;
        startbutton.SetActive(false);
        endbutton.SetActive(true);
        ainfobutton.SetActive(true);
        oplayerbutton.SetActive(true);
    }

    [PunRPC]
    void SendCardInfo(string pname, int[] ccost, string[] ceffect, int[] ccategory, int[] coffset)
    {

        if (pname == me.GetPlayerName())//내 패라면 패를 받는다.
            cmanager.GiveCard(ccost, ceffect, ccategory, coffset);
    }

    [PunRPC]
    void SendCardEffect(string pname, string usedpname , int ccost, int ccategory, int coffset, int cindex)
    {
        if(PhotonNetwork.isMasterClient)
        {
            for(int i = 0; i<pinfo.Length; i++)
            {
                if (pinfo[i].GetPlayerName() == usedpname)
                {
                    cmanager.PlayerHandUpdate(i, cindex);
                    break;
                }
            }
        }
        UseCardEffect(pname, usedpname, ccost, ccategory, coffset);
    }

    [PunRPC]
    void SendCardEnd(string pname, bool isrealend)
    {
        if (PhotonNetwork.isMasterClient)
        {
            bool iscardend = true;//카드 사용이 끝났나?
            int j = 0;
            for (int i = 0; i < players.Length; i++)
            {
                if (pname == pinfo[i].GetPlayerName())
                {
                    if (isrealend)
                    {
                        IsPlayersPlayingCard[i] = false;
                        SendEPlayer(PhotonTargets.All, pname);
                    }
                    j = i + 1;
                    //Debug.Log("플레이어 이름 : " + pname + "인덱스 : " + j);
                }
                if (IsPlayersPlayingCard[i])//한명이라도 카드 사용 중이라면 false
                    iscardend = false;
            }

            if (iscardend)//카드 사용이 끝났다면
            {
                SendAPhase(PhotonTargets.All);//경매 단계로 변경
                endbutton.SetActive(true);
                AInfoMove(1);
            }
            else//아니라면
            {
                while (true)
                {
                    j = j % players.Length;
                    if (IsPlayersPlayingCard[j] == false)
                    {
                        //Debug.Log("카드 사용 종료 플레이어 이름 : " + pinfo[j].GetPlayerName() + " 인덱스 : " + j);
                        j++;
                    }
                    else
                    {
                        //Debug.Log("턴 획득 플레이어 이름 : " + pinfo[j].GetPlayerName() + " 인덱스 : " + j);
                        break;
                    }
                }
                //다음 차례의 플레이어에게 카드 사용 턴을 줘라
                SendCTurn(PhotonTargets.All, pinfo[j].GetPlayerName());
            }
        }
    }

    [PunRPC]
    void SendAuctionPhase()
    {
        TimeManager.lefttime = 60;
        //isrotateturnstate = false;
        turnpname = "";
        GameObject.Find("HighLight").transform.position = new Vector3(3000, 2000, 0);
        GameState = 1;//경매 상태로 변경
        IsHandCard = false;
        endbutton.SetActive(true);
        AInfoMove(1);//중앙으로 이동

        for (int i = 0; i < players.Length; i++)
            IsPlayersPlayingCard[i] = true;
        
        //ChatBox.text += "System : 경매 시작!\n";
    }

    [PunRPC]
    void SendCardTurn(string pname)
    {
        TimeManager.lefttime = 10;
        turnpname = pname;
        if (pname == me.GetPlayerName())
        {
            //pricetext.text = "나의 턴!";
            //ChatBox.text += "System : 나의 턴!\n";
            systemtext.text = "내가 카드를 사용 할 차례 입니다.";
            IsHandCard = true;
            endbutton.SetActive(true);
            
        }
        else
        {
            //pricetext.text = pname + "의 턴!";
            //ChatBox.text += "System : " + pname + "의 턴!\n";
            systemtext.text = turnpname + "가 카드를 사용 할 차례 입니다.";
            IsHandCard = false;
            endbutton.SetActive(false);
        }
    }

    //호스트가 다른 플레이어 들에게 turnend 버튼을 누른 플레이어를 알려줌
    [PunRPC]
    void SendEndPlayer(string pname)
    {
        int i;
        for(i = 0; i<players.Length; i++)
        {
            if (pname == pinfo[i].GetPlayerName())
            {
                IsPlayersPlayingCard[i] = false;
                return;
            }
        }
    }

    #endregion

    #region RPC 함수를 호출하는 함수들
    void SendP(PhotonTargets targets, string pname, int pri)
        //클라이언트들이 마스터 클라이언트에게 자신의 경매가 보냄
    {
        photonView.RPC("SendPrice", targets, pname, pri);
    }

    void SendM(PhotonTargets targets, string pname, int pri, string pris)
        //마스터 클라이언트가 다른 클라이언트 들에게 최대 입찰자를 보냄.
    {
        photonView.RPC("SendMaxPlayer", targets, pname, pri, pris);
    }

    public void SendRAuction(PhotonTargets targets)
        //최대 입찰자가 마스터 클라이언트에게 경매 진행 요청을 보냄
    {
        photonView.RPC("RestartAuction", targets);
    }

    void SendSAuction(PhotonTargets targets, string sname, int tmoney)
    {
        photonView.RPC("StartAuction", targets, sname, tmoney);
    }

    public void SendSPos(PhotonTargets targets, string p, string c)
    {
        photonView.RPC("SendSelectPos", targets, p, c);
    }

    void SendSGame(PhotonTargets targets)
    {
        photonView.RPC("SendStartGame", targets);
    }

    //카드 정보 송신
    public void SendCInfo(PhotonTargets targets, string pname, int[] ccost, string[] ceffect, int[] ccategory, int[] coffset)
    {
        photonView.RPC("SendCardInfo", targets, pname, ccost, ceffect, ccategory, coffset);
    }

    //카드 효과 발동
    public void SendCEffect(PhotonTargets targets, string pname, string usedpname, int ccost, int ccategory, int coffset, int cindex)
    {
        photonView.RPC("SendCardEffect", targets, pname, usedpname, ccost, ccategory, coffset, cindex);
    }

    public void SendCEnd(PhotonTargets targets, string pname, bool isrealend)
    {
        photonView.RPC("SendCardEnd", targets, pname, isrealend);
    }

    void SendAPhase(PhotonTargets targets)
    {
        photonView.RPC("SendAuctionPhase", targets);
    }

    public void SendCTurn(PhotonTargets targets, string pname)
    {
        photonView.RPC("SendCardTurn", targets, pname);
    }

    void SendEPlayer(PhotonTargets targets, string pname)
    {
        photonView.RPC("SendEndPlayer", targets, pname);
    }
    #endregion

    /*
    #region 코루틴
    IEnumerator RotateTurnStateImage(int n)//n*120도 까지 돌린다.
    {
        Debug.Log("코루틴 시작");
        int lastrotation = (int)turnstate.transform.rotation.eulerAngles.z;
        isrotateturnstate = true;

        if (lastrotation >= 360)
        {
            lastrotation = 0;
        }

        while (lastrotation < 120 * n)
        {
            lastrotation += 2;

            //Debug.Log("현재 각도 : " + lastrotation.ToString());

            turnstate.transform.rotation = Quaternion.Euler(new Vector3(0, 0, lastrotation));

            yield return new WaitForSeconds(0.02f);
        }
    }
    #endregion
    */
}
