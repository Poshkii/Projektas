using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterManager : MonoBehaviour
{
    public RectTransform[] indications;
    public GameObject[] indicationObjects;
    private List<string> activeIndicators = new List<string>();
    private Dictionary<string, int> dic = new Dictionary<string, int>() {
        {"DoubleCoins", 0},
        {"ExtraLife", 1},
        {"ExtraJump", 2},
        {"SlowTime", 3},
        };

    private Dictionary<int, string> dic2 = new Dictionary<int, string>() {
        {0, "DoubleCoins"},
        {1, "ExtraLife"},
        {2, "ExtraJump"},
        {3, "SlowTime"},
        };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Active boosters: " + activeIndicators.Count);
        
    }

    public void AddBooster(string name, float duration)
    {
        if (!activeIndicators.Contains(name))
        {
            activeIndicators.Add(name);            
        }
        int index = 0;
        dic.TryGetValue(name, out index);
        Vector3 currentPos = indications[index].localPosition;
        currentPos.y += (activeIndicators.Count - 1) * 200;
        indications[index].localPosition = currentPos;
        indicationObjects[index].SetActive(true);
        indicationObjects[index].GetComponent<PickupIndication>().StartTimer(duration);
    }

    public void RemoveBooster(string name)
    {
        activeIndicators.Remove(name);
        RearangeBoosters();
    }

    private void RearangeBoosters()
    {
        foreach(string name in activeIndicators)
        {
            int index = 0;
            dic.TryGetValue(name, out index);
            Vector3 currentPos = indications[index].localPosition;
            currentPos.y -= 200;
            indications[index].localPosition = currentPos;
        }
    }
}
