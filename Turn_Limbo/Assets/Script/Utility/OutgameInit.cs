using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OutgameInit : InitActivator
{
    // [SerializeField] SkillUpgrade upgrade;
    // [SerializeField] StageStartBtn stageStart;
    // [SerializeField] UIOptionClick options;
    // [SerializeField] UIShelfClick shelf;
    protected override void StartInit()
    {
        var aliceInits = FindObjectsOfType<Transform>()
            .Select(x => x.GetComponent<IInitObserver>())
            .Where(x => x != null)
            .OrderBy(x => x.Priority);

        foreach (var method in aliceInits)
        {
            method.Init();
        }
    }
}
