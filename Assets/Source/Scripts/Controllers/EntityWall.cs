using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class EntityWall : MonoBehaviour
    {
        [SerializeField] private Image _Image;
        [SerializeField] private BoxCollider2D _BoxCollider2D;
        public Image Image => _Image;
        public BoxCollider2D BoxCollider2D => _BoxCollider2D;

        public bool IsConnected;
    }
}
