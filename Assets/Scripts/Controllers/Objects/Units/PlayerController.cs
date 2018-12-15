using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : UnitController
{
    [SerializeField]
    private int maxMp;
    [SerializeField]
    private int recoverHp;
    [SerializeField]
    private int recoverMp;
    [SerializeField]
    private float walkSpeed = 1.0f;

    //武器
    [SerializeField]
    private GameObject enemyTapWeapon;
    private WeaponController enemyTapWeaponCtrl;
    [SerializeField]
    private List<GameObject> tapWeaponList = new List<GameObject>();
    private List<WeaponController> tapWeaponCtrlList = new List<WeaponController>();
    [SerializeField]
    private List<GameObject> playerTapWeaponList = new List<GameObject>();
    private List<WeaponController> playerTapWeaponCtrlList = new List<WeaponController>();
    [SerializeField]
    private List<GameObject> dragWeaponList = new List<GameObject>();
    private List<WeaponController> dragWeaponCtrlList = new List<WeaponController>();
    [SerializeField]
    private List<GameObject> dragingWeaponList = new List<GameObject>();
    private List<WeaponController> dragingWeaponCtrlList = new List<WeaponController>();
    [SerializeField]
    private List<GameObject> playerDragWeaponList = new List<GameObject>();
    private List<WeaponController> playerDragWeaponCtrlList = new List<WeaponController>();
    [SerializeField]
    private List<GameObject> playerDragingWeaponList = new List<GameObject>();
    private List<WeaponController> playerDragingWeaponCtrlList = new List<WeaponController>();
    [SerializeField]
    private GameObject flickWeapon;
    private WeaponController flickWeaponCtrl;
    [SerializeField]
    private GameObject playerFlickWeapon;
    private WeaponController playerFlickWeaponCtrl;
    [SerializeField]
    private GameObject pinchWeapon;
    private WeaponController pinchWeaponCtrl;
    [SerializeField]
    private GameObject twistWeapon;
    private WeaponController twistWeaponCtrl;

    private int mp = 0;
    private float rhp = 0;
    private float rmp = 0;
    private bool isCharge = false;
    private Dictionary<string, WeaponController> weaponList = new Dictionary<string, WeaponController>();
    private Vector2 popPos;

    const float CHARGE_RECOVER_RATE = 0.75f;

    protected override void Awake()
    {
        base.Awake();
        InputManager.Instance.SetTapAction(TapAction);
        InputManager.Instance.SetLongTapAction(LongTapAction);
        InputManager.Instance.SetFlickAction(FlickAction);
        InputManager.Instance.SetDragAction(DragAction);
        InputManager.Instance.SetDragingAction(DragingAction);
        InputManager.Instance.SetPinchAction(PinchAction);
        InputManager.Instance.SetTwistAction(TwistAction);

        InputManager.Instance.SetLongTapLevelAction(LongTapLevelAction);
        InputManager.Instance.SetReleaseAction(ReleaseAction);
        SetMp(maxMp);
        popPos = GameObject.FindGameObjectWithTag(Common.CO.TAG_PLAYER_POP).transform.position;
    }

    protected override void Update()
    {
        base.Update();

        //HP回復
        rhp += recoverHp * deltaTime;
        if (rhp >= 1.0f)
        {
            int r = (int)Mathf.Floor(rhp);
            SetHp(r);
            rhp -= r;
        }

        //MP回復
        rmp += recoverMp * deltaTime * (isCharge ? CHARGE_RECOVER_RATE : 1);
        if (rmp >= 1.0f)
        {
            int r = (int)Mathf.Floor(rmp);
            SetMp(r);
            rmp -= r;
        }

        //定位置へ移動
        moveVelocity = Vector2.zero;
        if (!isCharge) SetMoveVelocity(popPos, walkSpeed);
    }

    //HP割合取得
    public float GetHpRate()
    {
        return (float)hp / maxHp;
    }

    //MP割合取得
    public float GetMpRate()
    {
        return (float)mp / maxMp;
    }

    //被弾
    public override bool Damage(int damage)
    {
        BattleManager.Instance.AddHit();
        if (mp > 0) OnBarrier();
        UseMp(damage);
        return false;
    }

    //MP設定
    protected virtual void SetMp(int diff = 0)
    {
        mp += diff;
        if (mp > maxMp) mp = maxMp;
    }

    //MP消費
    public bool UseMp(int use)
    {
        mp -= use;
        if (mp < 0)
        {
            base.Damage(Mathf.Abs(mp));
            mp = 0;
        }
        return true;
    }

    //武器設定
    protected override void SetWeapon()
    {
        base.SetWeapon();
        enemyTapWeaponCtrl = EquipWeapon(enemyTapWeapon);
        tapWeaponCtrlList = EquipWeapon(tapWeaponList);
        playerTapWeaponCtrlList = EquipWeapon(playerTapWeaponList);
        dragWeaponCtrlList = EquipWeapon(dragWeaponList);
        dragingWeaponCtrlList = EquipWeapon(dragingWeaponList);
        playerDragWeaponCtrlList = EquipWeapon(playerDragWeaponList);
        playerDragingWeaponCtrlList = EquipWeapon(playerDragingWeaponList);
        flickWeaponCtrl = EquipWeapon(flickWeapon);
        playerFlickWeaponCtrl = EquipWeapon(playerFlickWeapon);
        pinchWeaponCtrl = EquipWeapon(pinchWeapon);
        twistWeaponCtrl = EquipWeapon(twistWeapon);
    }
    public override WeaponController EquipWeapon(GameObject weapon)
    {
        WeaponController  weaponCtrl = base.EquipWeapon(weapon);
        if (weaponCtrl != null) weaponCtrl.SetPlayer(this);
        return weaponCtrl;
    }

    //攻撃
    protected void Fire(List<WeaponController> weaponCtrlList, InputStatus input)
    {
        WeaponController weaponCtrl = SelectWeapon(weaponCtrlList, input.pressLevel);
        if (weaponCtrl == null) return;
        Fire(weaponCtrl, input);
    }
    protected void Fire(WeaponController weaponCtrl, InputStatus input)
    {
        if (weaponCtrl == null) return;
        weaponCtrl.Fire(input);
    }

    //### ジェスチャーアクション ###

    //タップ(base)
    private void TapAction(InputStatus input)
    {
        if (input.isTapPlayer)
        {
            Fire(playerTapWeaponCtrlList, input);
        }
        else if (input.IsTapEnemy())
        {
            Fire(enemyTapWeaponCtrl, input);
        }
        else
        {
            Fire(tapWeaponCtrlList, input);
        }
    }

    //ロングタップ(base)
    private void LongTapAction(InputStatus input)
    {
        if (input.isTapPlayer)
        {
            Fire(playerTapWeaponCtrlList, input);
        }
        else
        {
            Fire(tapWeaponCtrlList, input);
        }
    }

    //フリック(base)
    private void FlickAction(InputStatus input)
    {
        //Vector2 vector = input.GetEndPoint() - input.GetStartPoint();
        //KnockBack(vector.normalized * runSpeed, runLimit);
        Fire(flickWeaponCtrl, input);
    }

    //ドラッグ(base)
    private void DragAction(InputStatus input)
    {
        if (input.isTapPlayer)
        {
            Fire(playerDragWeaponCtrlList, input);
        }
        else
        {
            Fire(dragWeaponCtrlList, input);
        }
    }

    //ドラッグ中
    private void DragingAction(InputStatus input)
    {
        if (input.isTapPlayer)
        {
            Fire(playerDragingWeaponCtrlList, input);
        }
        else
        {
            Fire(dragingWeaponCtrlList, input);
        }
    }

    //ピンチイン・アウト(base)
    private void PinchAction(InputStatus input)
    {
        Fire(pinchWeaponCtrl, input);
    }

    //ツイスト(base)
    private void TwistAction(InputStatus input)
    {
        Fire(twistWeaponCtrl, input);
    }

    //ロングタップレベル変更
    private void LongTapLevelAction(InputStatus input)
    {
        //オーラ表示
        isCharge = true;
        OnChant(input.pressLevel, true);
    }

    //リリース
    private void ReleaseAction(InputStatus input)
    {
        //オーラ非表示
        isCharge = false;
        OnChant(input.pressLevel, false);
    }
}