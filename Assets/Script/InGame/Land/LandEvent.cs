using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandEvent : MonoBehaviour
{
    SourceInfo source;//자원
    string owner;//소유 플레이어

    private GameObject target;
    public Text text;
    public Renderer[] Edges;
    private Text ltext;
    private LandConstructor lconstructor;
    private AuctionManager amanager;
    private float multiple;//배율

    //Renderer blockcolor;

	// Use this for initialization
	void Start ()
    {
        owner = null;
        multiple = 1;

        lconstructor = GameObject.Find("LandConstructor").GetComponent<LandConstructor>();
        amanager = GameObject.Find("GameManager").GetComponent<AuctionManager>();
        //blockcolor = GetComponent<Renderer>();

        //자기 위치에 텍스트 생성
        ltext = Instantiate(text,
            Camera.main.WorldToScreenPoint(transform.position),
            Quaternion.identity);

        ltext.transform.SetParent(GameObject.Find("GameBoard").transform, true);//캔버스의 자식으로 둠
                                                                                //false로 하면 위치가 좀 이상해짐
	}

    private void Update()
    {
        if (owner == null && Input.GetMouseButtonDown(0)
            && AuctionManager.GameState == 3//타일 선택 단계일 때
            && !AuctionManager.IsWaitMaxPlayer
            && AuctionManager.maxplayer == AuctionManager.me.GetPlayerName())//경매 승리자가 클릭 했을 때의 조건을 추가해라
        {
            target = GetClickedObject();//클릭된 오브젝트 저장

            if (target == null)//이상한 곳 클릭했으면 중지.
                return;

            if(target.Equals(gameObject))//만약 자신과 같다면
            {
                //클릭 됬을 때 현재 자원 정보를 해당 타일에 등록
                source = lconstructor.GetSource();

                //ChangeColor();

                amanager.SendSPos(PhotonTargets.All, transform.parent.name, transform.name);

                amanager.SendRAuction(PhotonTargets.MasterClient);
            }
        }

        if (source.SourceName != null)
            ltext.text = source.SourceName + "\n" + (source.SourceMoney * multiple);//보드판에 해당 자원 정보 저장
        ltext.gameObject.transform.position = Camera.main.WorldToScreenPoint(transform.position);
    }

    private GameObject GetClickedObject()//자신을 클릭했으면 자신을 return 아니면 null return
    {
        RaycastHit hit;
        GameObject target = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//마우스 포인터 근처 좌표 만듬

        if (true == (Physics.Raycast(ray.origin, ray.direction, out hit)))//마우스 근처에 오브젝트 있는지 확인
            target = hit.collider.gameObject;//있다면 오브젝트 저장

        return target;
    }

    /*
    private void ChangeColor()
    {
        int i;
        for (i = 0; i < PhotonNetwork.playerList.Length; i++)
        {
            if (AuctionManager.maxplayer == amanager.GetPlayer(i).GetPlayerName())
                break;
        }
        if (i == PhotonNetwork.playerList.Length)
        {
            Debug.Log("No Player Exist(LandEvent : ChangeColor)");
            return;
        }
        switch(i)
        {
            case 0:
                blockcolor.material.color = Color.red;
                break;

            case 1:
                blockcolor.material.color = Color.green;
                break;

            case 2:
                blockcolor.material.color = Color.yellow;
                break;

            case 3:
                blockcolor.material.color = Color.blue;
                break;
        }
        owner = AuctionManager.maxplayer;
    }
    */

    public void SetLandSource(SourceInfo sinfo, string oname)
    {
        source = sinfo;
        owner = oname;
        ltext.text = source.SourceName + "\n" + (source.SourceMoney * multiple);
    }

    public string GetOwner()
    {
        return owner;
    }

    public void SetMultiple(float m)
    {
        multiple = m;
    }

    public void SetLandColor(Color color)
    {
        for (int i = 0; i < 4; i++)
            Edges[i].material.color = color;
    }
}
