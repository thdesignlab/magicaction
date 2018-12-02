using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// A behaviour that is attached to a playable
public class EnemyBossBehaviour : PlayableBehaviour
{
    //AssetInfo
    private EnemyBossAsset asset;
    private GameObject enemy;
    private Vector2 spawnPos = Vector2.zero;

    private Quaternion spawnQua = default(Quaternion);
    private bool isStart = false;

    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        isStart = true;
        BattleManager.Instance.SqawnEnemyBoss(enemy, spawnPos, spawnQua);
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (!isStart || !StageManager.Instance.IsEndless()) return;
        BattleManager.Instance.TimelineLoop();
    }

	// Called each frame while the state is set to Play
	public override void PrepareFrame(Playable playable, FrameData info)
    {
    }

    //### Asset情報 ###

    public void SetAsset(EnemyBossAsset ea)
    {
        asset = ea;
        enemy = asset.enemy;
        spawnPos = StageManager.Instance.GetPoint(asset.pos, true, true);
        spawnQua = StageManager.Instance.GetQuaternion(spawnPos);
    }
}
