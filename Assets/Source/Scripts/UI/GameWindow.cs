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
        private List<EntityWall>                  _wallsList = new List<EntityWall>();
        private List<LineEntity>                _linesList = new List<LineEntity>();

        private EntityWall _startTouchedWall;
        private LineEntity _currentLineRenderer;

        private Vector2 _screenSize;

        public event Action OnWallsConnected;

        private int _amountWallsAtMap = 0;
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
                    _pointerEventData           = new PointerEventData(_eventSystem);
                    _pointerEventData.position  = Input.mousePosition;
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
                else if(Input.GetMouseButtonUp(0))
                {
                    if (_startTouchedWall != null)
                    {
                        _pointerEventData = new PointerEventData(_eventSystem);
                        _pointerEventData.position = Input.mousePosition;
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
                                _linesList.Add(_currentLineRenderer);
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
                        _currentLineRenderer = null;
                        _startTouchedWall = null;
                    }
                }
#elif UNITY_IOS || UNITY_ANDROID
                if (Input.touchCount == 1)
                {
                    Touch touch = Input.GetTouch(0);

                    Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
                    RaycastHit hit;

                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            {
                                if (Physics.Raycast(ray, out hit))
                                {
                                    var entityWall = hit.collider.GetComponent<EntityWall>();
                                    if (entityWall != null && !entityWall.IsConnected)
                                    {
                                        _startTouchedWall = entityWall;
                                        Debug.Log("You touched on " + _startTouchedWall.name);
                                    }
                                }
                                break;
                            }

                        case TouchPhase.Ended:
                            {
                                if (Physics.Raycast(ray, out hit))
                                {
                                    var entityWall = hit.collider.GetComponent<EntityWall>();
                                    if (entityWall != null && entityWall != _startTouchedWall)
                                    {
                                        _startTouchedWall.IsConnected = true;
                                        entityWall.IsConnected = true;
                                        _connectedWallsCount += 2;
                                        Debug.Log("You touched off " + entityWall.name);

                                        if (_connectedWallsCount == _amountWallsAtMap) OnWallsConnected?.Invoke();
                                    }
                                }

                                _startTouchedWall = null;
                                break;
                            }
                    }
                }
#endif
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
                _wallsList.Add(entityWall);
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

                _wallsList.Add(entityWall);
            }
        }

        public void ClearWindow()
        {
            foreach(var wall in _wallsList) Destroy(wall.gameObject);
            foreach (var line in _linesList) Destroy(line.gameObject);

            _wallsList.Clear();
            _linesList.Clear();
        }
    }
}
