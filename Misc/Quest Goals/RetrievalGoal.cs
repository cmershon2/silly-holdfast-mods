using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetrievalGoal : Quest.QuestGoal
{
    public string Item;

    public override void Initialize()
    {
        base.Initialize();
        EventManager.Instance.AddListener<RetrievalGameEvent>(OnItemRetrieval);
    }

    private void OnItemRetrieval(RetrievalGameEvent eventInfo)
    {
        if(eventInfo.RetrievalName == Item)
        {
            CurrencyAmount++;
            Evaluate();
        }
    }
}
