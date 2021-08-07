using UnityEngine;
using UnityEngine.UI;
using Internals;
using Inputs;
using UnityEngine.EventSystems;

namespace Controllers
{
    [RequireComponent(typeof(BoxCollider2D), typeof(Image))]
    public class EntityWall : MonoBehaviour
    {
        [HideInInspector] public bool IsConnected;

        public Image         Image { private set; get; }
        public BoxCollider2D BoxCollider2D { private set; get; }

        private void Awake()
        {
            Image         = GetComponent<Image>();
            BoxCollider2D = GetComponent<BoxCollider2D>();
        }
    }
}
