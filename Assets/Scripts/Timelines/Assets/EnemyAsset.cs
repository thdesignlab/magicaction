using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class EnemyAsset : PlayableAsset
{
    public GameObject enemy;
    public Vector2 pos;
    public Vector2 diffPos;
    public float interval;

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go) {
        EnemyBehaviour behaviour = new EnemyBehaviour();
        behaviour.SetAsset(this);
        return ScriptPlayable<EnemyBehaviour>.Create(graph, behaviour);
    }
}
