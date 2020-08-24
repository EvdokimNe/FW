using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class WheelController : MonoBehaviour
{
    [Header("SectorVisualSettings")]
    [SerializeField]private float _visualLineLenght;
    [SerializeField]private float _offset = 0;
    [Header("WheelRotateAnimSettings")]
    [SerializeField]private AnimationCurve _rotateCurve;
    [SerializeField]private float time = 1;
    [SerializeField] private int _addCircle = 2;
    [Header("UI")]
    [SerializeField] private GameObject _imagGo;
    [SerializeField] private GameObject _textGo;
    [SerializeField] private GameObject _parrentSectrors;
    private List<float> _changeList = new List<float>();
    private List<float> _angles = new List<float>();
    private List<float> _halfAngles = new List<float>();
    [HideInInspector]
    public List<Sector> _sectors = new List<Sector>();
      
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(WinRotate());
        }
    }

    private void WinMethod(int winSector)
    {
        switch (_sectors[winSector]._enum)
        {
            case MethodsEnumName.DebugHelloWorld:
                Debug.Log("DebugHelloWorld"); //вызов любово метода
                break;
            case MethodsEnumName.DebugHell:
                Debug.Log("Hello");
                break;
        }
    }
    private IEnumerator WinRotate()
    {
        HalfSectore();
        yield return null;
        int winSectorNumber = ChoiseRandomPrize();
        float angle = _halfAngles[winSectorNumber];
        float addAngle = 0;
        if (angle >= 90)
        {
            addAngle = 360 - angle + 90 + _addCircle * 360;
        }
        if (angle < 90)
        {
            addAngle = 90 - angle + _addCircle * 360;
        }
        float i = 0;
        float duration = 1 / time;
        var StartLerpPos = _parrentSectrors.transform.eulerAngles.z;
        while (i < 1)
        {
            i += duration * Time.deltaTime;
            var b = Mathf.Lerp(StartLerpPos, addAngle, _rotateCurve.Evaluate(i));
            _parrentSectrors.transform.rotation = Quaternion.Euler(0,0,b);
            yield return null;
        }
        WinMethod(winSectorNumber);
        Debug.Log(_sectors[winSectorNumber]._text + "WINSECTOR");
    }

    private int ChoiseRandomPrize()
    {
        var list = ChangePercentage();
        var random = Random.Range(0, 100);
        int returnInt = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (random <= list[i])
            {
                returnInt = i;
                break;
            } 
        }
        return returnInt;
    }
    private List<float> ChangePercentage()
    {
        float _summ = 0;
        _changeList.Clear();
        foreach (var sector in _sectors)
        {
            _changeList.Add(sector._changeWin);
            _summ += sector._changeWin;
        }
        float percent = _summ/100;
        for (int i = 0; i < _sectors.Count; i++)
        {
            _changeList[i] = _sectors[i]._changeWin / percent;
        }
        float change = 0f;
        for (int i = 0; i < _changeList.Count; i++)
        {
            change += _changeList[i];
            _changeList[i] = change;
        }
        return _changeList;
    }
    private void HalfSectore()
    {
        Angles();
        _halfAngles.Clear();
        for (int i = 0; i < _angles.Count - 1; i++)
        {
            var c = _angles[i] + (_angles[i + 1] - _angles[i]) / 2;

            if (_angles[i] > _angles[i + 1])
            {
                c = _angles[i] + (360 - _angles[i] + _angles[i + 1]) / 2;
            }
            _halfAngles.Add(c);
        }
    }
    
    private void Angles()
    {
        if (_sectors.Count != 0)
        {
            _angles.Clear();
            var angle = 360 / _sectors.Count;
            for (int i = 0; i < _sectors.Count; i++)
            {
                var ang = angle * i + _sectors[i]._sectorOffset + _offset;
                _angles.Add(ang);
                if (_angles[i] < 0)
                {
                    _angles[i] = 360f + _angles[i];
                }
                if (_angles[i] > 360)
                {
                    _angles[i] = _angles[i] % 360;
                }
            } 
            _angles.Add(_angles[0]);
        }     
    }
    private void SectorUITostartPos()
    {
        HalfSectore();
        for (int i = 0; i < _sectors.Count; i++)
        {
            if (_sectors[i]._spriteObj != null)
            {
                if (_sectors[i]._autoPos)
                {
                    var vectorFromAngle = GetVectorFromAngle(_halfAngles[i]+_sectors[i]._posYsprite); 
                    _sectors[i]._spriteObj.transform.localPosition = vectorFromAngle * _sectors[i]._posXsprite;
                }

                if (_sectors[i]._autoRot)
                {
                    _sectors[i]._spriteObj.transform.up = GetVectorFromAngle(_halfAngles[i]);
                }
            }

            if (_sectors[i]._textObj != null)
            {
                if (_sectors[i]._autoPos)
                {
                    var vectorFromAngle = GetVectorFromAngle(_halfAngles[i]+_sectors[i]._posYtext); 
                    _sectors[i]._textObj.transform.localPosition = vectorFromAngle * _sectors[i]._posXtext;
                }
                if (_sectors[i]._autoRot)
                {
                    _sectors[i]._textObj.transform.up = GetVectorFromAngle(_halfAngles[i]);
                }
                _sectors[i]._textObj.GetComponent<Text>().text = _sectors[i]._text;
            }
        }
        
    }
    public void SpawnSectorObj()
    {  
        _sectors.Add(new Sector());
        _sectors[_sectors.Count-1]._spriteObj = Instantiate(_imagGo, _parrentSectrors.transform);
        _sectors[_sectors.Count - 1]._textObj = Instantiate(_textGo, _parrentSectrors.transform);
        var st = "SectorText   " + (_sectors.Count-1).ToString();
        var ss = "SectorSprite   " + (_sectors.Count-1).ToString();
        _sectors[_sectors.Count - 1]._textObj.name = st;
        _sectors[_sectors.Count - 1]._spriteObj.name = ss;
        SectorUITostartPos();
    }

    public void DeleteSectoreObj(int SecNumb)
    {
        DestroyImmediate(_sectors[SecNumb]._spriteObj);
        DestroyImmediate(_sectors[SecNumb]._textObj);
        _sectors.Remove(_sectors[SecNumb]);
    }
    public void OnGuiChange()
    {
        HalfSectore();
        SectorUITostartPos();

    }
    private void OnDrawGizmosSelected()
    {
        HalfSectore();
        if (_sectors != null)
        {
            Gizmos.color = new Color(0.04f, 0f, 1f);
            for (int i = 0; i < _angles.Count-1; i++)
            {
                Gizmos.DrawRay(transform.position, GetVectorFromAngle(_angles[i]) * _visualLineLenght);
            }
         
            Gizmos.color = new Color(0.2f, 1f, 0f);
            for (int i = 0; i < _halfAngles.Count; i++)
            {
                Gizmos.DrawRay(transform.position ,GetVectorFromAngle(_halfAngles[i]) * _visualLineLenght);
            }
        }
    }
    
    public static Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad),0); 
    }
}


[System.Serializable]
public class Sector
{
    public string _text;
    public int _amountWin;
    public int _changeWin;
    public bool _autoPos = true;
    public bool _autoRot = true;
    public float _posXsprite = 2;
    public float _posYsprite = 0;
    public float _posXtext = 3;
    public float _posYtext = 0;
    public GameObject _spriteObj;
    public GameObject _textObj;
    public float _sectorOffset;
    public MethodsEnumName _enum; // вызов метода через enum т.к через custom editor не получилось вывести unity events
}

public enum MethodsEnumName
{
    DebugHelloWorld,
    DebugHell,
}
