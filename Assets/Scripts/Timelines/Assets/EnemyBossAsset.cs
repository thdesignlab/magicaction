using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class EnemyBossAsset : PlayableAsset
{
    public GameObject enemy;
    public Vector2 pos;

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go) {
        EnemyBossBehaviour behaviour = new EnemyBossBehaviour();
        behaviour.SetAsset(this);
        return ScriptPlayable<EnemyBossBehaviour>.Create(graph, behaviour);
    }

    public int GetBossHp()
    {
        return enemy.GetComponent<UnitController>().GetMaxHp();
    }
}
