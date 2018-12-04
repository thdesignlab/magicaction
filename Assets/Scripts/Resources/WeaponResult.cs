using UnityEngine;
using System.Collections.Generic;

public class WeaponResultDetail
{
    public int damage = 0;
    public int kill = 0;
}

public class WeaponResult
{
    public string name;
    public Dictionary<int, WeaponResultDetail> details = new Dictionary<int, WeaponResultDetail>();
    private float floatDamage = 0;

    public WeaponResult(string s)
    {
        name = s.Replace("(Clone)", "");
    }

    public void AddDamage(int i, float d, bool isKill = false)
    {
        if (!details.ContainsKey(i))
        {
            details.Add(i, new WeaponResultDetail());
        }
        floatDamage += d;
        details[i].damage = Mathf.FloorToInt(floatDamage);
        if (isKill) details[i].kill += 1;
    }
}
