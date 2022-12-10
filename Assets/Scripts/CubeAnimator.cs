using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CubeMover))]
public class CubeAnimator : MonoBehaviour
{
    [SerializeField] AnimationCurve _animationCurve;

    private CubeMover _cubeMover;
    private float _currentTimeStamp;

    private void Awake() => _cubeMover = GetComponent<CubeMover>();

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        //transform.DOScale(Vector3.one, 0.3f).OnComplete(() => transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 5, 2));
        transform.DOScale(Vector3.one, 0.5f).SetEase(_animationCurve);
            //.OnComplete(() => transform.DOShakeScale(0.5f, vibrato: 5, randomness: 0f));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _currentTimeStamp = (_cubeMover.Distance - _cubeMover.CurrentDistance) / _cubeMover.MoveSpeed;

        if(_currentTimeStamp <= 0.4f)
            transform.DOScale(Vector3.zero, 0.4f);
    }
}
