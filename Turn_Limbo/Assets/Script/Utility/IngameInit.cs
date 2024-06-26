using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngameInit : InitActivator
{
    // [SerializeField] SkillUpgrade upgrade;
    // [SerializeField] StageStartBtn stageStart;
    // [SerializeField] UIOptionClick options;
    // [SerializeField] UIShelfClick shelf;
    protected override void StartInit()
    {
        var aliceInits = FindObjectsOfType<GameObject>()
            .Select(x => x.GetComponent<IInitObserver>())
            .Where(x => x != null)
            .OrderBy(x => x.Priority);

        foreach (var method in aliceInits)
        {
            Debug.Log(method.gameObject.name);
            method.Init();
        }
    }
}
