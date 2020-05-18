using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandBlockManager : MonoBehaviour
{
    //private bool IsActive = false;//보드판 활성화 여부
    public GameObject board;//보드판
    //public GameObject boardtext;//보드판 글자
    //public GameObject AuctionUI;//경매창

    /*
    private void Start()
    {
        
        boardtext = GameObject.Find("GameBoard");
        AuctionUI = GameObject.Find("AuctionUI");
        board = GameObject.Find("Lands");
        board.SetActive(false);
        
        for (int i = 1; i <= 5; i++)
        {
            board.transform.Find("Line" + i).gameObject.SetActive(IsActive);
        }
        boardtext.SetActive(false);
    }
    */

    // Update is called once per frame
    /*
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Tab))//탭 키를 누르면 보드판과 경매창이 교체됨.
        {
            IsActive = !IsActive;
            boardtext.SetActive(IsActive);
            //board.SetActive(IsActive);
            for (int i = 1; i <= 5; i++)
            {
                board.transform.Find("Line" + i).gameObject.SetActive(IsActive);
            }
            AuctionUI.SetActive(!IsActive);
        }
	}
    */

    public GameObject GetClickedBlock(string p, string c)//부모와 찾을 게임 오브젝트 이름을 받아서 해당 게임 오브젝트 return
    {
        return board.transform.Find(p).Find(c).gameObject;
    }

    /*
    void CheckMoneyBomb(int linenum, int blocknum)
        //줄 번호와 블록 번호(Line N 의 N과 Land M 의 M)을 넘겨받아
        //상하좌우 2칸까지 검사해서 그것의 owner들이 같다면 턴당 수익 1.5배로 함
    {
        string[] tmplands = new string[5];
        Transform linetmp = board.transform.Find("Line" + linenum.ToString());
        LandEvent blocktmp = linetmp.Find("Land" + blocknum.ToString()).GetComponent<LandEvent>();
        int midpos = tmplands.Length / 2;
        tmplands[midpos] = blocktmp.GetOwner();
        
        for(int i = 1; i<=2; i++)
        {
            Transform otherblock = linetmp.Find("Land" + (blocknum + i).ToString());
            if (otherblock != null)
                tmplands[midpos + i] = otherblock.GetComponent<LandEvent>().GetOwner();//소유자의 이름 저장
            otherblock = linetmp.Find("Land" + (blocknum - i).ToString());
            if (otherblock != null)
                tmplands[midpos - i] = otherblock.GetComponent<LandEvent>().GetOwner();//소유자의 이름 저장
        }

        for(int i = 1; i<=3; i++)
        {
            float multi = 1.5f;
            if (tmplands[i-1]==tmplands[i]&&tmplands[i]==tmplands[i+1])
            {
                //3개 연속으로 소유자가 같다면 1.5배로 변경
                
                blocktmp.SetMultiple(multi);
                linetmp.Find("Land" + (i + 1).ToString()).GetComponent<LandEvent>().SetMultiple(multi);
                linetmp.Find("Land" + (i - 1).ToString()).GetComponent<LandEvent>().SetMultiple(multi);
            }
        }

        for (int i = 0; i < 5; i++)
            tmplands[i] = string.Empty;

        for (int i = 1; i <= 2; i++)
        {
            Transform otherblock = board.transform.Find("Line" + (linenum + i).ToString()).Find("Land" + blocknum.ToString());
            if (otherblock != null)
                tmplands[midpos + i] = otherblock.GetComponent<LandEvent>().GetOwner();//소유자의 이름 저장
            otherblock = board.transform.Find("Line" + (linenum - i).ToString()).Find("Land" + blocknum.ToString());
            if (otherblock != null)
                tmplands[midpos - i] = otherblock.GetComponent<LandEvent>().GetOwner();//소유자의 이름 저장
        }

        for (int i = 1; i <= 3; i++)
        {
            float multi = 1.5f;
            if (tmplands[i - 1] == tmplands[i] && tmplands[i] == tmplands[i + 1])
            {
                //3개 연속으로 소유자가 같다면 1.5배로 변경
                //bool 형 배열 하나 둬서 가로 세로의 배율이 적용 되었는가 그거 검사해라
                blocktmp.SetMultiple(multi);
                board.transform.Find("Line" + (i + 1).ToString()).Find("Land" + blocknum.ToString());
                board.transform.Find("Line" + (i - 1).ToString()).Find("Land" + blocknum.ToString());
            }
        }
    }
    */
}