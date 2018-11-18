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

    //武器
    [SerializeField]
    private GameObject tapWeapon;
    private WeaponController tapWeaponCtrl;
    [SerializeField]
    private GameObject playerTapWeapon;
    private WeaponController playerTapWeaponCtrl;
    [SerializeField]
    private GameObject enemyTapWeapon;
    private WeaponController enemyTapWeaponCtrl;
    [SerializeField]
    private List<GameObject> longTapWeaponList = new List<GameObject>();
    private List<WeaponController> longTapWeaponCtrlList = new List<WeaponController>();
    [SerializeField]
    private List<GameObject> playerLongTapWeaponList = new List<GameObject>();
    private List<WeaponController> playerLongTapWeaponCtrlList = new List<WeaponController>();
    [SerializeField]
    private List<GameObject> dragWeaponList = new List<GameObject>();
    private List<WeaponController> dragWeaponCtrlList = new List<WeaponController>();
    [SerializeField]
    private List<GameObject> playerDragWeaponList = new List<GameObject>();
    private List<WeaponController> playerDragWeaponCtrlList = new List<WeaponController>();
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

    protected override void Awake()
    {
        base.Awake();
        InputManager.Instance.SetTapAction(TapAction);
        InputManager.Instance.SetLongTapAction(LongTapAction);
        InputManager.Instance.SetFlickAction(FlickAction);
        InputManager.Instance.SetDragAction(DragAction);
        InputManager.Instance.SetPinchAction(PinchAction);
        InputManager.Instance.SetTwistAction(TwistAction);

        InputManager.Instance.SetLongTapLevelAction(LongTapLevelAction);
        InputManager.Instance.SetReleaseAction(ReleaseAction);
        SetMp(maxMp);
        SetWeapon();
    }

    protected override void Update()
    {
        base.Update();

        //HP回復
        rhp = recoverHp * deltaTime;
        if (rhp >= 1.0f)
        {
            int r = (int)Mathf.Floor(rhp);
            SetHp(r);
            rhp -= r;
        }

        //MP回復
        if (!isCharge)
        {
            rmp = recoverMp * deltaTime;
            if (rmp >= 1.0f)
            {
                int r = (int)Mathf.Floor(rmp);
                SetMp(r);
                rmp -= r;
            }
        }
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

    //MP設定
    protected virtual void SetMp(int diff = 0)
    {
        mp += diff;
        if (mp > maxMp) mp = maxMp;
    }

    //MP消費
    public bool UseMp(int use)
    {
        return true;
    }

    //武器設定
    private void SetWeapon()
    {
        tapWeaponCtrl = EquipWeapon(tapWeapon);
        longTapWeaponCtrlList = EquipWeapon(longTapWeaponList);
    }
    private WeaponController EquipWeapon(GameObject weapon)
    {
        if (weapon == null) return null;
        GameObject weaponObj = Instantiate(weapon, myTran.position, Quaternion.identity);
        weaponObj.transform.SetParent(myTran, true);
        WeaponController weaponCtrl = weaponObj.GetComponentInChildren<WeaponController>();
        if (weaponCtrl != null) weaponCtrl.SetPlayer(this);
        return weaponCtrl;
    }
    private List<WeaponController> EquipWeapon(List<GameObject> weaponList)
    {
        List<WeaponController> wepaonCtrlList = new List<WeaponController>();
        foreach (GameObject weapon in weaponList)
        {
            wepaonCtrlList.Add(EquipWeapon(weapon));
        }
        return wepaonCtrlList;
    }

    //攻撃処理
    private void Fire(List<WeaponController> weaponCtrlList, InputStatus input)
    {
        if (weaponCtrlList.Count == 0) return;
        int level = (weaponCtrlList.Count < input.pressLevel) ? weaponCtrlList.Count : input.pressLevel;
        Fire(weaponCtrlList[level], input);
    }
    private void Fire(WeaponController weaponCtrl, InputStatus input)
    {
        if (weaponCtrl == null) return;
        weaponCtrl.Fire(input.GetPoint(), input.pressLevel);
    }

    //### ジェスチャーアクション ###

    //タップ(base)
    private void TapAction(InputStatus input)
    {
        Fire(tapWeaponCtrl, input);
    }

    //ロングタップ(base)
    private void LongTapAction(InputStatus input)
    {
        Fire(longTapWeaponCtrlList, input);
    }

    //フリック(base)
    private void FlickAction(InputStatus input)
    {
        Fire(flickWeaponCtrl, input);
    }

    //ドラッグ(base)
    private void DragAction(InputStatus input)
    {
        Fire(dragWeaponCtrlList, input);
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