using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class BoosterManager : MonoBehaviour
{
    public RectTransform[] indications;
    public GameObject[] indicationObjects;
    private Vector3 startPos;
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
        startPos = indications[0].localPosition;
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
        RearangeBoosters();
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
        for(int i = 0; i < activeIndicators.Count; i++)
        {
            string name = activeIndicators[i];
            int index = 0;
            dic.TryGetValue(name, out index);
            Vector3 currentPos = startPos;
            currentPos.y += 200 * i;
            indications[index].localPosition = currentPos;
        }
    }
}
