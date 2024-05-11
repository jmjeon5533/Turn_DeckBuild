using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillScript : MonoBehaviour
{
    public List<ParticleSystem> Hitparticles = new();
    public abstract void Setting(Unit unit, Unit target);
    public abstract void End(Unit unit, Unit target);
}
