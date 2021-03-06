using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    [SerializeField] private GameObject _player = null;

    [SerializeField] private float _moveSpeed = 0f;

    [SerializeField] private float _reviseCameraPositionTime = 0f;

    private BezierCurve _bezierCurve = new BezierCurve();

    private Transform _cameraTransform = null;

    private Vector3 _bezierStartPoint = new Vector3();
    private Vector3 _bezierTempPoint = new Vector3();
    private Vector3 _bezierEndPoint = new Vector3();

    private ECameraChasingDirections _cameraDirection = ECameraChasingDirections.None;

    private float _horizontalDistance = 0f;
    private float _verticalDistance = 0f;

    private bool _isChasingPlayer = false;

    private const int STOP_HORIZONTAL_CHASING = 4;

    private readonly Vector3 _moveToForwardBetweenTwoPoints = new Vector3(0f, 0f, 1.25f);
    private readonly Vector3 _moveToLeftBetweenTwoPoints = new Vector3(-1.25f, 0f, 0f);
    private readonly Vector3 _moveToRightBetweenTwoPoints = new Vector3(1.25f, 0f, 0f);

    private readonly Vector3 _nextForwardPoint = new Vector3(0f, 0f, 2f);
    private readonly Vector3 _nextLeftPoint = new Vector3(-2f, 0f, 0f);
    private readonly Vector3 _nextRightPoint = new Vector3(2f, 0f, 0f);

    private void Start()
    {
        _cameraTransform = GetComponent<Transform>();
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameOver)
        {
            return;
        }

        Move();

        if (false == _isChasingPlayer && ECameraChasingDirections.None == _cameraDirection)
        {
            _horizontalDistance = _player.transform.position.x - _cameraTransform.position.x;
            _verticalDistance = _player.transform.position.z - _cameraTransform.position.z;

            if (_verticalDistance > 8.5f)
            {
                _cameraDirection = ECameraChasingDirections.Forward;

                _isChasingPlayer = true;
            }
            else if (_horizontalDistance < -4.5f && _player.transform.position.x > -STOP_HORIZONTAL_CHASING)
            {
                _cameraDirection = ECameraChasingDirections.Left;

                _isChasingPlayer = true;
            }
            else if (_horizontalDistance > -1.5f && _player.transform.position.x < STOP_HORIZONTAL_CHASING)
            {
                _cameraDirection = ECameraChasingDirections.Right;

                _isChasingPlayer = true;
            }
            else
            {
                _cameraDirection = ECameraChasingDirections.None;
            }

            MakeBezierPoint();

            StartCoroutine(ChasingPlayer());
        }
    }

    private void MakeBezierPoint()
    {
        _bezierStartPoint = _cameraTransform.position;

        switch (_cameraDirection)
        {
            case ECameraChasingDirections.Forward:
                _bezierTempPoint = _bezierStartPoint + _moveToForwardBetweenTwoPoints;
                _bezierEndPoint = _bezierStartPoint + _nextForwardPoint;
                break;
            case ECameraChasingDirections.Left:
                _bezierTempPoint = _bezierStartPoint + _moveToLeftBetweenTwoPoints;
                _bezierEndPoint = _bezierStartPoint + _nextLeftPoint;
                break;
            case ECameraChasingDirections.Right:
                _bezierTempPoint = _bezierStartPoint + _moveToRightBetweenTwoPoints;
                _bezierEndPoint = _bezierStartPoint + _nextRightPoint;
                break;
            default:
                break;
        }
    }

    private void Move()
    {
        Vector3 moveVec = new Vector3(0f, 0f, _moveSpeed * Time.deltaTime);

        _cameraTransform.position += moveVec;
    }

    private IEnumerator ChasingPlayer()
    {
        float elapsedTime = 0f;

        while (_isChasingPlayer)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= _reviseCameraPositionTime)
            {
                elapsedTime = _reviseCameraPositionTime;

                _isChasingPlayer = false;
            }

            _cameraTransform.position = _bezierCurve.OnePointBezierCurve(_bezierStartPoint, _bezierTempPoint, _bezierEndPoint, elapsedTime / _reviseCameraPositionTime);

            yield return null;
        }

        _cameraDirection = ECameraChasingDirections.None;
    }
}
