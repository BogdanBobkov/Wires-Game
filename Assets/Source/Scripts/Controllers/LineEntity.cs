using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public class LineEntity : MonoBehaviour
    {
        [SerializeField] private Material     _DottedLineMaterial;
        [SerializeField] private Material     _LineMaterial;
        [SerializeField] private LineRenderer _LineRenderer;

        public LineRenderer LineRenderer => _LineRenderer;

        private Color _color;
        private Vector3 _startPosition;
        private bool _isMove;

        public void Initialize(Color color, Vector3 startPosition)
        {
            _startPosition = startPosition;
            _color = color;
            _LineRenderer.material = _DottedLineMaterial;
            _LineRenderer.material.SetColor("_MainColor", _color);
            _isMove = true;
        }

        public void OnEndMovingLine()
        {
            _LineRenderer.material = _LineMaterial;
            _LineRenderer.material.color = _color;
            _isMove = false;
        }

        private void Update()
        {
            if (_isMove)
            {
                LineRenderer.SetPosition(0, _startPosition);
                LineRenderer.SetPosition(1, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _startPosition.z)));
            }
        }

    }
}
