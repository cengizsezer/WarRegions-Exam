using UnityEngine;

namespace Magnistart_CharacterPack
{
    public class CharacterMover : MonoBehaviour
    {
        [Header("Attributes")]
        //Movement speed of the character.
        [SerializeField] private float speed = 5f;

        [Header("References")]
        //A reference to visual indicator to show if character is selected.
        [SerializeField] private GameObject selectedIndicator = null;
        //A reference to animator of character.
        [SerializeField] private Animator animator = null;

        //Is the character selected?
        private bool _isSelected = false;
        public bool isSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                //Show or hide selected indicator based on character being selected.
                selectedIndicator.SetActive(value);
            }
        }

        //Is the character moving right now?
        private bool isMoving = false;
        //Target position to move at.
        private Vector3 targetPos = Vector2.zero;


        public void MoveTo(Vector2 pos)
        {
            if (!isMoving)
                animator.CrossFadeInFixedTime("Run", 0.15f);
            isMoving = true;
            targetPos = pos;
            if (targetPos.x < transform.position.x) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        private void Update()
        {
            if (isMoving)
            {
                Vector3 movementVector = ((Vector2)targetPos - (Vector2)transform.position);
                /*If distance to target position is so small, move character to exact target position and
                 stop moving.*/
                if (movementVector.magnitude <= Time.deltaTime * speed)
                {
                    transform.position = targetPos;
                    isMoving = false;
                    animator.CrossFadeInFixedTime("Idle", 0.15f);
                }
                else
                    transform.position += movementVector.normalized * Time.deltaTime * speed;
            }
        }

        private void Reset()
        {
            animator = GetComponent<Animator>();
            selectedIndicator = transform.GetChild(transform.childCount - 1).gameObject;
        }

    }
}