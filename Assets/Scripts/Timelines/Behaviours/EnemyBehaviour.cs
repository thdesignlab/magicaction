using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// A behaviour that is attached to a playable
public class EnemyBehaviour : PlayableBehaviour
{
    //AssetInfo
    private EnemyAsset asset;
    private GameObject enemy;
    private Vector2 spawnPos = Vector2.zero;
    private Vector2 spawnDiffPos = Vector2.zero;

    private Quaternion spawnQua = default(Quaternion);
    private float spawnInterval = 1;
    private float intervalTime = 0;
    private bool isGround = false;
    private bool isRandom = false;
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
        intervalTime += info.deltaTime;
        if (intervalTime < spawnInterval) return;
        if (isRandom)
        {
            spawnPos = StageManager.Instance.GetRandomPoint(isGround);
            spawnQua = StageManager.Instance.GetQuaternion(spawnPos);
        }
        BattleManager.Instance.SqawnEnemy(enemy, spawnPos, spawnQua);
        spawnPos += spawnDiffPos;
        intervalTime = 0;
    }

    //### Asset情報 ###

    public void SetAsset(EnemyAsset ea)
    {
        asset = ea;
        spawnInterval = asset.interval;
        intervalTime = spawnInterval;
        SetSpawnPos(asset.pos, asset.diffPos);
        SetEnemy(asset.enemy);
    }

    private void SetSpawnPos(Vector2 v, Vector2 diff)
    {
        if (v == Vector2.zero)
        {
            isRandom = true;
            return;
        }
        spawnPos = StageManager.Instance.GetPoint(v);
        spawnDiffPos = StageManager.Instance.GetPoint(diff, false);
        spawnQua = StageManager.Instance.GetQuaternion(spawnPos);
    }

    private void SetEnemy(GameObject e)
    {
        enemy = e;
        UnitController unitCtrl = enemy.GetComponent<UnitController>();
        isGround = unitCtrl.IsGravity();
    }
}
