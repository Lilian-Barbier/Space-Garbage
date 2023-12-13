using Assets.Scripts.Enums;
using Models;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using static Unity.Collections.AllocatorManager;

namespace Assets.Scripts.Machines
{
    public class FusionBehaviour : TableBehaviour
    {

        [SerializeField] private float timeForSeparate = 0.5f;
        [SerializeField] private GameObject trashPrefab;
        [SerializeField] private int fusionSize = 4;
        [SerializeField] private float ejectForce = 2;

        private Animator animator;

        private Slider slider;

        private List<GameObject> trashList;
        private int currentTrashSize;

        private void Start()
        {
            slider = GetComponentInChildren<Slider>();
            animator = GetComponent<Animator>();

            slider.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }


        public override void SetObjectCarried(Transform newObjectCarried)
        {
            //slider.gameObject.SetActive(true);
            
            trashList.Add(newObjectCarried.gameObject);
            TrashBehaviour trashBehaviour = newObjectCarried.GetComponent<TrashBehaviour>();

            if(trashBehaviour.GetTrashSize() + currentTrashSize > fusionSize || !trashBehaviour.IsOnlyOneMaterialTrash(MaterialType.Metal))
            {
                EjectPiece();
            }

            currentTrashSize += trashBehaviour.GetTrashSize();
            newObjectCarried.transform.position = new Vector2(200, 200);

            //base.SetObjectCarried(newObjectCarried);
            animator.SetBool("IsPainting", true);

            if (currentTrashSize == fusionSize)
            {
                Fusion();
            }

        }

        private void EjectPiece()
        {
            foreach(GameObject trash in trashList)
            {
                trash.transform.position = transform.position + Vector3.up;
                trash.GetComponent<Rigidbody2D>().AddForce(Vector3.up * ejectForce);
            }
        }

        private void Fusion()
        {
            foreach (GameObject trash in trashList)
            {
                Destroy(trash);
            }   
            var fusionTrash = Instantiate(trashPrefab, transform.position + Vector3.right, Quaternion.identity);
            fusionTrash.GetComponent<TrashBehaviour>().SetTrash(new Trash(TrashUtils.MetallicSquare));
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
