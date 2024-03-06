using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Player : Unit
{
    protected override void Update()
    {
        base.Update();
    }
    public void AddRequest(Skill addSkill)
    {
        if (!isAttack)
        {
            print(addSkill.skillName);

            var newSkill = ConvertRequest(addSkill);
            newSkill.insertImage = UIManager.instance.AddImage(newSkill.icon);
            attackRequest.Enqueue(newSkill);
        }
    }
}
