﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    //### 定数 ###
    public static class CO
    {
        //アプリID
        public const string APP_NAME_IOS = "";
        public const string APP_NAME_ANDROID = "";

        //HomePage
        public const string WEBVIEW_KEY = "?webview";
        public static string HP_URL = "";

        //シーン名
        public const string SCENE_TITLE = "TitleScene";
        public const string SCENE_BATTLE = "BattleScene";

        //タグ
        public const string TAG_PLAYER = "Player";
        public const string TAG_ENEMY = "Enemy";
        public const string TAG_STAGE = "Stage";
        public const string TAG_OBJECT = "Object";
        public const string TAG_PHYSICS = "Physics";
        public const string TAG_EFFECT = "Effect";
        public const string TAG_LASER = "Laser";
        public const string TAG_MUZZLE = "Muzzle";

        //ユニットタグ
        public static string[] unitTags = new string[]
        {
            TAG_PLAYER,
            TAG_ENEMY,
        };

        //ダメージオブジェクトタグ
        public static string[] damageObjectTags = new string[]
        {
            TAG_PHYSICS,
            TAG_EFFECT,
            TAG_LASER,
        };

        //ステージタグ
        public static string[] stageTags = new string[]
        {
            TAG_STAGE,
            TAG_OBJECT,
        };

        //レベル接頭文字
        public static string LEVEL_PREFIX = "Level";
    }

    //### ユニット共通 ###
    public static class UNIT
    {
        //詠唱エフェクト
        public const string PARTS_CHANT = "Chants";
    }

    //### プレイヤー ###
    public static class PLAYER
    {
        //武器種類
        public const string PARTS_WEAPON_TAP = "TapWeapon";
        public const string PARTS_WEAPON_LONG_TAP = "LongTapWeapon";
        public static string[] weaponPartsList = {
            PARTS_WEAPON_TAP,
            PARTS_WEAPON_LONG_TAP,
        };
    }

    //### 関数 ###
    public static class FUNC
    {
        //platform確認
        public static bool IsAndroid()
        {
            if (Application.platform == RuntimePlatform.Android) return true;
            return false;
        }
        public static bool IsIos()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer) return true;
            return false;
        }
        public static bool IsPc()
        {
            if (IsAndroid() || IsIos()) return false;
            return true;
        }

        //タグチェック
        public static bool IsDamageObjectTag(string tag)
        {
            return InArrayString(CO.damageObjectTags, tag);
        }
        public static bool IsStageTag(string tag)
        {
            return InArrayString(CO.stageTags, tag);
        }
        public static bool IsUnitTag(string tag)
        {
            return InArrayString(CO.unitTags, tag);
        }

        //配列チェック
        private static bool InArrayString(string[] tags, string tagName)
        {
            bool flg = false;
            foreach (string tag in tags)
            {
                if (tagName == tag)
                {
                    flg = true;
                    break;
                }
            }
            return flg;
        }
        //抽選
        public static T Draw<T>(Dictionary<T, int> targets)
        {
            T drawObj = default(T);
            int sumRate = 0;
            List<T> targetValues = new List<T>();
            foreach (T obj in targets.Keys)
            {
                sumRate += targets[obj];
                targetValues.Add(obj);
            }
            if (sumRate == 0) return drawObj;

            int drawNum = Random.Range(1, sumRate + 1);
            sumRate = 0;
            for (int i = 0; i < targets.Count; i++)
            {
                int key = Random.Range(0, targetValues.Count);
                sumRate += targets[targetValues[key]];
                if (sumRate >= drawNum)
                {
                    drawObj = targetValues[key];
                    break;
                }
                targetValues.RemoveAt(key);
            }
            return drawObj;
        }
        public static TKey RandomDic<TKey, TValue>(Dictionary<TKey, TValue> dic)
        {
            return dic.ElementAt(Random.Range(0, dic.Count)).Key;
        }
        public static T RandomList<T>(List<T> list)
        {
            return list.ElementAt(Random.Range(0, list.Count));
        }

        //三角関数
        public static float GetSin(float time, float anglePerSec = 360, float startAngle = 0)
        {
            float angle = (startAngle + anglePerSec * time) % 360;
            float radian = Mathf.PI / 180 * angle;
            return Mathf.Sin(radian);
        }

        //NaNチェック
        public static bool IsNanVector(Vector2 v)
        {
            return (float.IsNaN(v.x) && float.IsNaN(v.y));
        }

        //2D変換
        public static Vector2 ParseVector2(Vector3 v3)
        {
            return new Vector2(v3.x, v3.y);
        }

        //3D変換
        public static Vector3 ParseVector3(Vector2 v2)
        {
            return new Vector3(v2.x, v2.y, 0);
        }

        //誤差値
        public static float GetRandom(float num)
        {
            return Random.Range(-num, num);
        }

        //2DLookAt
        public static void LookAt(Transform tran, Vector3 target)
        {
            Vector3 diff = (target - tran.position).normalized;
            tran.rotation = Quaternion.FromToRotation(Vector3.right, diff);
        }

    }
}