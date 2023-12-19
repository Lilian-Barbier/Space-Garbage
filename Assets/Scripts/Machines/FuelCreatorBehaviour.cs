using Assets.Scripts.Enums;
using Models;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Assets.Scripts.Machines
{
    public class FuelCreatorBehaviour : TableBehaviour
    {
        [SerializeField] private float timeForFilter = 0.5f;
        [SerializeField] private GameObject trashPrefab;
        [SerializeField] private float ejectForce = 2000;

        private Animator animator;

        private Slider slider;

        private void Start()
        {
            slider = GetComponentInChildren<Slider>();
            animator = GetComponent<Animator>();

            slider.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (objectCarried != null)
            {
                var dominoBehaviour = objectCarried.GetComponent<TrashBehaviour>();

                timeOnTable += Time.deltaTime;
                slider.value = timeOnTable / timeForFilter;

                if (timeOnTable > timeForFilter)
                {
                    var trash = GetObjectCarried();
                    var trashSize = dominoBehaviour.GetTrashSize();

                    Destroy(trash.gameObject);
                    for (int i = 0; i < trashSize; i++)
                    {
                        GameObject fuel = Instantiate(trashPrefab, transform.position + Vector3.up, Quaternion.identity);
                        fuel.GetComponent<TrashBehaviour>().SetTrash(new Trash(TrashUtils.Fuel));
                        fuel.transform.position = transform.position + Vector3.right;
                    }
                    if (!TutorielManager.Instance.tutorialCarburatorPassed)
                    {
                        TutorielManager.Instance.tutorialCarburatorPassed = true;
                        TutorielManager.Instance.NextTutorial();
                    }
                }
            }
            else
            {
                slider.value = 0;
            }
        }

        public override void SetObjectCarried(Transform newObjectCarried)
        {
            if (!newObjectCarried.GetComponent<TrashBehaviour>().IsOnlyOneMaterialTrash(MaterialType.Organic))
            {
                newObjectCarried.transform.position = transform.position + Vector3.up;
                newObjectCarried.GetComponent<Collider2D>().isTrigger = false;
                newObjectCarried.GetComponent<Rigidbody2D>().isKinematic = false;
                newObjectCarried.GetComponent<Rigidbody2D>().AddForce(Vector3.up * ejectForce);
                return;
            }

            slider.gameObject.SetActive(true);

            base.SetObjectCarried(newObjectCarried);
            animator.SetBool("IsPainting", true);
        }

        public override Transform GetObjectCarried()
        {
            slider.gameObject.SetActive(false);

            var obj = base.GetObjectCarried();
            animator.SetBool("IsPainting", false);
            return obj;
        }
    }
}