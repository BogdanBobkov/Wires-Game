using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Internals;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Controllers
{
    public class GameWindow : MonoBehaviour
    {
        [SerializeField] private GraphicRaycaster _raycaster;
        [SerializeField] private EventSystem      _eventSystem;
        [SerializeField] private GameObject       _PrefabWall;
        [SerializeField] private GameObject       _LineRendererPrefab;
        private Stack<EntityWall>                 _wallsStack = new Stack<EntityWall>();
        private Stack<LineEntity>                 _linesStack = new Stack<LineEntity>();

        private EntityWall _startTouchedWall;
        private LineEntity _currentLineRenderer;

        private Vector2 _screenSize;

        public event Action OnWallsConnected;

        private int _amountWallsAtMap   = 0;
        private int _connectedWallsCount = 0;

        private PointerEventData _pointerEventData;

        private void Awake()
        {
            _screenSize = new Vector2(Screen.width, Screen.height);
        }

        private void Update()
        {
            if (Locator.GameplayController.IsGame)
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                if (Input.GetMouseButtonDown(0))
                {
                    TouchDown(Input.mousePosition);
                }
                else if(Input.GetMouseButtonUp(0))
                {
                    TouchUp(Input.mousePosition);
                }
#elif UNITY_IOS || UNITY_ANDROID
                if (Input.touchCount == 1)
                {
                    var touch = Input.GetTouch(0);

                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            {
                                TouchDown(touch.position);
                                break;
                            }

                        case TouchPhase.Ended:
                            {
                                TouchUp(touch.position);
                                break;
                            }
                    }
                }
#endif
            }
            else if (_currentLineRenderer != null)
            {
                if (_currentLineRenderer != null) Destroy(_currentLineRenderer.gameObject);
            }
        }

        private void TouchDown(Vector3 position)
        {
            _pointerEventData = new PointerEventData(_eventSystem);
            _pointerEventData.position = position;
            List<RaycastResult> results = new List<RaycastResult>();
            _raycaster.Raycast(_pointerEventData, results);

            foreach (RaycastResult result in results)
            {
                var entityWall = result.gameObject.GetComponent<EntityWall>();
                if (entityWall != null && !entityWall.IsConnected)
                {
                    _startTouchedWall = entityWall;
                    _currentLineRenderer = Instantiate(_LineRendererPrefab, transform).GetComponent<LineEntity>();
                    _currentLineRenderer.Initialize(entityWall.Image.color, _startTouchedWall.transform.position);
                }
            }
        }

        private void TouchUp(Vector3 position)
        {
            if (_startTouchedWall != null)
            {
                _pointerEventData = new PointerEventData(_eventSystem);
                _pointerEventData.position = position;
                var results = new List<RaycastResult>();
                _raycaster.Raycast(_pointerEventData, results);

                foreach (RaycastResult result in results)
                {
                    var entityWall = result.gameObject.GetComponent<EntityWall>();
                    if (entityWall != null && entityWall != _startTouchedWall && entityWall.Image.color == _startTouchedWall.Image.color)
                    {
                        _startTouchedWall.IsConnected = true;
                        entityWall.IsConnected = true;
                        _connectedWallsCount += 2;
                        _currentLineRenderer.OnEndMovingLine();
                        _linesStack.Push(_currentLineRenderer);
                        _currentLineRenderer = null;
                        _startTouchedWall = null;

                        if (_connectedWallsCount == _amountWallsAtMap)
                        {
                            _connectedWallsCount = 0;
                            OnWallsConnected?.Invoke();
                        }

                        return;
                    }
                }

                if (_currentLineRenderer != null) Destroy(_currentLineRenderer.gameObject);
                _startTouchedWall = null;
            }
        }

        public void CreateWalls(int numWalls)
        {
            _amountWallsAtMap = numWalls;

            Stack<Color> colors = new Stack<Color>();

            for(int i = 0; i < numWalls / 2; i++)
            {
                var wall = Instantiate(_PrefabWall, transform).GetComponent<RectTransform>();
                wall.sizeDelta = new Vector2(_screenSize.y / (numWalls * 2 + 1) / 2, _screenSize.y / (numWalls + 1));
                wall.anchoredPosition = new Vector2(-_screenSize.x / 2 + wall.sizeDelta.x / 2, wall.sizeDelta.y * (2 * i + 1) + wall.sizeDelta.y / 2);

                var entityWall = wall.GetComponent<EntityWall>();
                entityWall.BoxCollider2D.size = wall.sizeDelta;
                entityWall.Image.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);

                colors.Push(entityWall.Image.color);
                _wallsStack.Push(entityWall);
            }

            colors = new Stack<Color>(colors.OrderBy(x => Guid.NewGuid().ToString()));

            for (int i = 0; i < numWalls / 2; i++)
            {
                var wall = Instantiate(_PrefabWall, transform).GetComponent<RectTransform>();
                wall.sizeDelta = new Vector2(_screenSize.y / (numWalls * 2 + 1) / 2, _screenSize.y / (numWalls + 1));
                wall.anchoredPosition = new Vector2(_screenSize.x / 2 - wall.sizeDelta.x / 2, wall.sizeDelta.y * (2 * i + 1) + wall.sizeDelta.y / 2);

                var entityWall = wall.GetComponent<EntityWall>();
                entityWall.BoxCollider2D.size = wall.sizeDelta;
                entityWall.Image.color = colors.Pop();

                _wallsStack.Push(entityWall);
            }
        }

        public void ClearWindow()
        {
            while(_wallsStack.Count > 0) Destroy(_wallsStack.Pop());
            while(_linesStack.Count > 0) Destroy(_linesStack.Pop());
        }
    }
}
