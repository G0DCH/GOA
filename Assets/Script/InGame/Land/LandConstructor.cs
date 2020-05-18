using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SourceInfo
{
    public string SourceName { get; set; }//자원 명
    public int SourceMoney { get; set; }//턴당 수익

    public SourceInfo(string name, int money)//무작위 자원 생성
    {
        SourceName = name;
        SourceMoney = money;
    }
};

public class LandConstructor : MonoBehaviour
{
    //서버로부터 자원 정보와 입찰자 정보를 받아서 저장한다.
    
    SourceInfo source;

    string owner;

    private string[] namearr = { "금", "은", "다이아몬드", "루비", "사파이어", "에메랄드", "구리", "철", "석유" };//자원 이름들
    private int[] moneyarr = { 1000, 100, 2000, 600, 500, 400, 50, 70, 300 };//자원 가치들

    public string sname;
    public int smoney;

    public SourceInfo GetSource()
    {
        return source;
    }

    public void setSource(string tmpsname, int tmpsmoney)
    {
        source = new SourceInfo(tmpsname, tmpsmoney);
        smoney = source.SourceMoney;
        sname = source.SourceName;
    }

    
    public void SetOwner(string own)
    {
        owner = own;
    }

    public string GetOwner()
    {
        return owner;
    }
    
    public void ChangeSource()
    {
        int tmpn = Random.Range(0, 100) % namearr.Length;
        source = new SourceInfo(namearr[tmpn], moneyarr[tmpn]);

        sname = source.SourceName;
        smoney = source.SourceMoney;
    }
}
