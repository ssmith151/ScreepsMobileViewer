using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private const float BeamDuration = 1;
    private const float HalfDuration = BeamDuration / 2;

    public LineRenderer lineRenderer;
    public Color healColor;
    public Color rangedAttackColor;
    public Color workColor;
    private float _time;
    private Vector3 _startPos;
    private Vector3 _endPos;

    public void SetWidth(float widthIn)
    {
        lineRenderer.startWidth = widthIn;
        lineRenderer.endWidth = widthIn;
    }

    public void Load(Vector2Int startPos, Vector2Int endPos, string action)
    {
        Color color = workColor;
        if (action == "heal")
            color = healColor; 
        if (action == "rangedHeal")
            color = healColor;
        if (action == "rangedAttack")
            color = rangedAttackColor;
        _time = 0f;
        _startPos = new Vector3(startPos.x * 0.16f + 0.08f, startPos.y * 0.16f + 0.08f, -1);
        _endPos = new Vector3(endPos.x * 0.16f + 0.08f, endPos.y * 0.16f + 0.08f, -1);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        StartCoroutine(Fire());
    }

    private IEnumerator Fire()
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, _startPos);
        while (_time < HalfDuration)
        {
            var factor = _time / HalfDuration;
            var point = (_endPos - _startPos) * factor + _startPos;
            lineRenderer.SetPosition(1, point);
            _time += Time.unscaledDeltaTime;
            yield return null;
        }

        lineRenderer.SetPosition(1, _endPos);
        while (_time < BeamDuration)
        {
            var factor = (_time - HalfDuration) / HalfDuration;
            var point = (_endPos - _startPos) * factor + _startPos;
            lineRenderer.SetPosition(0, point);
            _time += Time.unscaledDeltaTime;
            yield return null;
        }

        lineRenderer.enabled = false;
    }
}
