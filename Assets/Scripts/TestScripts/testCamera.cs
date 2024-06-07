using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testCamera : MonoBehaviour
{
    [SerializeField] private float _cameraSmoothness = 6f;
    private GameObject _player = default;
    private Vector2 _playerPos = default;
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        _playerPos = _player.transform.position;
        var targetPos = new Vector3(_playerPos.x, _playerPos.y, transform.position.z);
        SetCameraPos(targetPos);
        
    }
    private void SetCameraPos(Vector3 targetPos) => transform.position = Vector3.Lerp(transform.position, targetPos,
        _cameraSmoothness * Time.deltaTime);
}
