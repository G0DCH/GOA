using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLogUpdater : MonoBehaviour
{
    public Text LogText;
    float lifetime = 0f;

    private void Start()
    {
        //3초 뒤에 제거
        Destroy(gameObject, 3f);
    }

    private void Update()
    {
        lifetime += Time.deltaTime;
        //2초 동안은 제자리에 있음
        if (lifetime < 2f)
            return;
        transform.position += Vector3.up;
    }

    //카드 사용자 이름, 사용 대상, 변화 대상(소지금, 소득), 변화 량을 출력
    public void UpdateText(string usedname, string targetname, string subtarget, int diff)
    {
        LogText.text = usedname + "님이 " + targetname + "님의 "
            + subtarget + "을 " + diff.ToString() + "만큼 변경";
    }
}
