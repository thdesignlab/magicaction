using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class EnemyBossActionAsset : PlayableAsset
{
    public GameObject boss;

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go) {
        EnemyBossActionBehaviour behaviour = new EnemyBossActionBehaviour();
        behaviour.SetAsset(this);
        return ScriptPlayable<EnemyBossActionBehaviour>.Create(graph, behaviour);
    }
}
