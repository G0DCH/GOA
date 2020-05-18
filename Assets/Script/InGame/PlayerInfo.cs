using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerInfo
{
    private string playerName;//플레이어 이름
    private int money;//플레이어 소지금
    private int tmoney;//턴당 수익
    private int LandNum;

    public PlayerInfo(string name, int amoney, int atmoney, int alandnum)
    {
        playerName = name;
        money = amoney;
        tmoney = atmoney;
        LandNum = alandnum;
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    public void SetMoney(int m)
    {
        money = m;
    }

    public void SetTMoney(int tm)
    {
        tmoney = tm;
    }

    public void SetLandNum(int tl)
    {
        LandNum = tl;
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public int GetMoney()
    {
        return money;
    }

    public int GetTMoney()
    {
        return tmoney;
    }

    public int GetLandNum()
    {
        return LandNum;
    }
};