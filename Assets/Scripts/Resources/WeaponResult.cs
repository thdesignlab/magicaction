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

    public WeaponResult(string s)
    {
        name = s.Replace("(Clone)", "");
    }

    public void AddDamage(int i, int d, bool isKill = false)
    {
        if (!details.ContainsKey(i))
        {
            details.Add(i, new WeaponResultDetail());
        }
        details[i].damage += d;
        if (isKill) details[i].kill += 1;
    }
}
