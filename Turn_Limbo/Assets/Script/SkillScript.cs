using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillScript : MonoBehaviour
{
    public List<ParticleSystem> Hitparticles = new();
    public abstract void Setting(Unit @this, Unit target);
    public abstract void Attack(Unit @this, Unit target);
    public abstract void End(Unit @this, Unit target);

    // public override void Setting(Unit @this, Unit target) { }
    // public override void Attack(Unit @this, Unit target) { }
    // public override void End(Unit @this, Unit target) { }
}
