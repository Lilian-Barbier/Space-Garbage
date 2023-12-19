﻿using Assets.Scripts.Enums;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Assets.Scripts.Machines
{
    public class FusionBehaviour : TableBehaviour
    {

        [SerializeField] private float waitingTime = 1f;
        [SerializeField] private GameObject trashPrefab;
        [SerializeField] private int fusionSize = 4;
        [SerializeField] private float ejectForce = 2;
        [SerializeField] List<SpriteRenderer> hologramBlock;
        [SerializeField] Sprite GreenBlock;
        [SerializeField] Sprite GreyBlock;

        private Animator animator;

        private Slider slider;

        private List<GameObject> trashList = new List<GameObject>();
        private int currentTrashSize;

        private MaterialType currentFusionType;

        private bool merging = false;
        private float timeOnMergint = 0f;

        private void Start()
        {
            slider = GetComponentInChildren<Slider>();
            animator = GetComponent<Animator>();

            slider.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (merging)
            {
                timeOnMergint += Time.deltaTime;
                slider.value = timeOnMergint / waitingTime;
            }
        }

        public override void SetObjectCarried(Transform newObjectCarried)
        {
            if (merging)
            {
                return;
            }

            trashList.Add(newObjectCarried.gameObject);
            TrashBehaviour trashBehaviour = newObjectCarried.GetComponent<TrashBehaviour>();

            if (!trashBehaviour.IsOnlyOneMaterialTrash())
            {
                EjectPiece();
                return;
            }

            if (currentTrashSize == 0)
            {
                currentFusionType = trashBehaviour.GetTrashMaterial();
            }

            if (trashBehaviour.GetTrashSize() + currentTrashSize > fusionSize || !trashBehaviour.IsOnlyOneMaterialTrash(currentFusionType))
            {
                EjectPiece();
            }
            else
            {
                currentTrashSize += trashBehaviour.GetTrashSize();
                newObjectCarried.transform.position = new Vector2(200, 200);
                animator.SetBool("IsPainting", true);


                if (currentTrashSize >= 1)
                {
                    hologramBlock[0].sprite = currentFusionType == MaterialType.Metal ? GreyBlock : GreenBlock;
                    hologramBlock[0].color = new Color(hologramBlock[0].color.r, hologramBlock[0].color.g, hologramBlock[0].color.b, 1f);
                }
                if (currentTrashSize >= 2)
                {
                    hologramBlock[1].sprite = currentFusionType == MaterialType.Metal ? GreyBlock : GreenBlock;
                    hologramBlock[1].color = new Color(hologramBlock[1].color.r, hologramBlock[1].color.g, hologramBlock[1].color.b, 1f);
                }
                if (currentTrashSize >= 3)
                {
                    hologramBlock[2].sprite = currentFusionType == MaterialType.Metal ? GreyBlock : GreenBlock;
                    hologramBlock[2].color = new Color(hologramBlock[2].color.r, hologramBlock[2].color.g, hologramBlock[2].color.b, 1f);
                }
                if (currentTrashSize >= 4)
                {
                    hologramBlock[3].sprite = currentFusionType == MaterialType.Metal ? GreyBlock : GreenBlock;
                    hologramBlock[3].color = new Color(hologramBlock[3].color.r, hologramBlock[3].color.g, hologramBlock[3].color.b, 1f);
                }

                if (currentTrashSize == fusionSize)
                {
                    StartCoroutine(nameof(Fusion));
                }
            }
        }

        private void EjectPiece()
        {
            foreach (GameObject trash in trashList)
            {
                trash.transform.position = transform.position + Vector3.up;
                trash.GetComponent<Collider2D>().isTrigger = false;
                trash.GetComponent<Rigidbody2D>().isKinematic = false;
                trash.GetComponent<Rigidbody2D>().AddForce(Vector3.up * ejectForce);
            }
            trashList.Clear();
            currentTrashSize = 0;
            animator.SetBool("IsPainting", false);
            ResetHologram();
        }

        private IEnumerator Fusion()
        {
            merging = true;
            slider.gameObject.SetActive(true);

            yield return new WaitForSeconds(waitingTime);

            slider.gameObject.SetActive(false);
            merging = false;
            timeOnMergint = 0f;

            foreach (GameObject trash in trashList)
            {
                Destroy(trash);
            }
            var fusionTrash = Instantiate(trashPrefab, transform.position + Vector3.right, Quaternion.identity);
            fusionTrash.GetComponent<TrashBehaviour>().SetTrash(new Trash(currentFusionType == MaterialType.Metal ? TrashUtils.MetallicSquare : TrashUtils.OrganicSquare));
            trashList.Clear();
            currentTrashSize = 0;
            animator.SetBool("IsPainting", false);
            ResetHologram();
            if (!TutorielManager.Instance.tutorialMergerPassed)
            {
                TutorielManager.Instance.tutorialMergerPassed = true;
                TutorielManager.Instance.NextTutorial();
            }
        }

        private void ResetHologram()
        {
            hologramBlock[0].sprite = GreyBlock;
            hologramBlock[0].color = new Color(hologramBlock[0].color.r, hologramBlock[0].color.g, hologramBlock[0].color.b, 0.2f);
            hologramBlock[1].sprite = GreyBlock;
            hologramBlock[1].color = new Color(hologramBlock[1].color.r, hologramBlock[1].color.g, hologramBlock[1].color.b, 0.2f);
            hologramBlock[2].sprite = GreyBlock;
            hologramBlock[2].color = new Color(hologramBlock[2].color.r, hologramBlock[2].color.g, hologramBlock[2].color.b, 0.2f);
            hologramBlock[3].sprite = GreyBlock;
            hologramBlock[3].color = new Color(hologramBlock[3].color.r, hologramBlock[3].color.g, hologramBlock[3].color.b, 0.2f);
        }

        public override Transform GetObjectCarried()
        {
            return null;
        }
    }

}
