using System.Collections.Generic;
using UnityEngine;
using Internals;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Controllers
{
    public class GameWindow : MonoBehaviour, IGameView
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

        private int _amountWallsAtMap   = 0;
        private int _connectedWallsCount = 0;

        private PointerEventData    _pointerEventData;
        private List<RaycastResult> _raycastResults = new List<RaycastResult>();

        private IGameplayControllable _gameplayController;

        private void Awake()
        {
            Register();

            _screenSize         = new Vector2(Screen.width, Screen.height);
            _gameplayController = Locator.GetObject<IGameplayControllable>();
            _pointerEventData   = new PointerEventData(_eventSystem);
        }

        private void OnDestroy() => Unregister();

        private void Update()
        {
            if (_gameplayController.IsGame())
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                if (Input.GetMouseButtonDown(0))
                {
                    OnTouchDown(Input.mousePosition);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    OnTouchUp(Input.mousePosition);
                }
#elif UNITY_IOS || UNITY_ANDROID
                if (Input.touchCount == 1)
                {
                    var touch = Input.GetTouch(0);

                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            {
                                OnTouchDown(touch.position);
                                break;
                            }

                        case TouchPhase.Ended:
                            {
                                OnTouchUp(touch.position);
                                break;
                            }
                    }
                }
#endif
                _raycastResults.Clear();
            }
        }

        public void OnTouchDown(Vector3 downPosition)
        {
            _pointerEventData.position = downPosition;
            _raycaster.Raycast(_pointerEventData, _raycastResults);

            foreach (RaycastResult result in _raycastResults)
            {
                var entityWall = result.gameObject.GetComponent<EntityWall>();
                if (entityWall != null && !entityWall.IsConnected)
                {
                    _startTouchedWall = entityWall;
                    _currentLineRenderer = Instantiate(_LineRendererPrefab, transform).GetComponent<LineEntity>();
                    _currentLineRenderer.Initialize(entityWall.Image.color, _startTouchedWall.transform.position);

                    return;
                }
            }
        }

        public void OnTouchUp(Vector3 upPosition)
        {
            if (_startTouchedWall != null)
            {
                _pointerEventData.position = upPosition;
                _raycaster.Raycast(_pointerEventData, _raycastResults);

                foreach (RaycastResult result in _raycastResults)
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

        private EntityWall CreateEntityWall(Vector2 position, Vector2 size)
        {
            var wall              = Instantiate(_PrefabWall, transform).GetComponent<RectTransform>();
            wall.sizeDelta        = size;
            wall.anchoredPosition = position;

            var entityWall                = wall.GetComponent<EntityWall>();
            entityWall.BoxCollider2D.size = wall.sizeDelta;
            _wallsStack.Push(entityWall);

            return entityWall;
        }

        #region Interfaces
        public event Action OnWallsConnected;

        public void CreateWalls(int numWalls)
        {
            _amountWallsAtMap = numWalls;

            Stack<Color> colors = new Stack<Color>();

            for (int i = 0; i < numWalls; i++)
            {
                int sideScreen = 0;
                Action onCreateAction = null;
                EntityWall entityWall = null;

                if (i < numWalls / 2)
                {
                    sideScreen = -1;

                    onCreateAction = () =>
                    {
                        entityWall.Image.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                        colors.Push(entityWall.Image.color);

                        if (i == numWalls / 2 - 1) colors = new Stack<Color>(colors.OrderBy(x => Guid.NewGuid().ToString()));
                    };
                }
                else
                {
                    sideScreen = 1;

                    onCreateAction = () =>
                    {
                        entityWall.Image.color = colors.Pop();
                    };
                }

                entityWall = CreateEntityWall(
                                                    new Vector2(sideScreen * _screenSize.x / 2 - sideScreen * _screenSize.y / (numWalls * 2 + 1) / 4, _screenSize.y / (numWalls + 1) * (3 + 4 * (i % 2)) / 2),
                                                    new Vector2(_screenSize.y / (numWalls * 2 + 1) / 2, _screenSize.y / (numWalls + 1))
                                                 );
                onCreateAction?.Invoke();
            }
        }

        public void ClearWindow()
        {
            while(_wallsStack.Count > 0) Destroy(_wallsStack.Pop().gameObject);
            while(_linesStack.Count > 0) Destroy(_linesStack.Pop().gameObject);

            if (_currentLineRenderer != null) Destroy(_currentLineRenderer.gameObject);
        }

        public void Register()   => Locator.Register(typeof(IGameView), this);
        public void Unregister() => Locator.Unregister(typeof(IGameView));
        #endregion
    }
}
