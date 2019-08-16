using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RMA : MonoBehaviour
{
    public SpriteRenderer rmaSr;
    private float _time;
    private Vector3 _startPos;
    private float duration = 2.0f;
    private Color rmaColor = new Color(0.6f, 0.7f, 1, 1);

    private void Start()
    {
        Load(new Vector2Int(0, 0));
    }

    public void Load(Vector2Int startPos)
    {
        _time = 0f;
        _startPos = new Vector3(startPos.x * 0.16f + 0.08f, startPos.y * 0.16f + 0.08f, -1);
        rmaSr.color = rmaColor;
        StartCoroutine(Fire());
    }

    private IEnumerator Fire()
    {
        transform.localScale = Vector3.zero;
        rmaSr.enabled = true;
        transform.position = _startPos;
        float lastBit = 2 * duration / 3;
        float lastBitDivisor = duration / 3;
        while (_time < duration)
        {
            _time += Time.unscaledDeltaTime;
            float currentScale = _time / duration;
            transform.localScale = new Vector3(currentScale, currentScale, 1f);
            if (_time > lastBit) {
                float a = Mathf.Lerp(1, 0, (_time - lastBit) / lastBitDivisor);
                rmaSr.color = new Color(rmaSr.color.r, rmaSr.color.g, rmaSr.color.b, a );
            }
            yield return null;
        }
        transform.localScale = Vector3.one;
        rmaSr.enabled = false;
    }
}
