//-----------------------------------------------------------------
// File:         WorldDataProviderSystem.cs
// Description:  Store and provide world data (tilemaps, navigation graph etc...)
// Module:       Client Systems
// Author:       Noé Masse
// Date:         17/03/2021
//-----------------------------------------------------------------
using System;

using UnityEngine;
using UnityEngine.InputSystem;
using MonsterWorld.Unity.Navigation;
using MonsterWorld.Unity.Systems.Characters;
using System.Collections;

namespace MonsterWorld.Unity.Systems
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterStoreSystem), typeof(WorldDataProviderSystem), typeof(PlayerInput))]
    public class PlayerMovementSystem : GameSystem
    {
        // References
        private CharacterStoreSystem _CharacterStoreSystem;
        private WorldDataProviderSystem _WorldDataProviderSystem;
        private PlayerInput _PlayerInputs;

        // Serialized Fields
        public float moveSpeed = 5.0f;
        public float snapRadius = 0.1f;

        // Fields
        private ClientNavigationGraph.Node currentNode;
        private Vector2 moveInput;
        private bool canMove = true;

        public override void Initialize()
        {
            _CharacterStoreSystem = GetSystem<CharacterStoreSystem>();
            _WorldDataProviderSystem = GetSystem<WorldDataProviderSystem>();
            _PlayerInputs = GetComponent<PlayerInput>();

            var moveAction = _PlayerInputs.currentActionMap.FindAction("Move");
            moveAction.started += OnMove;
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;

            // Next : get server information
            currentNode = _WorldDataProviderSystem.NavGraph[0];
            _CharacterStoreSystem.LocalPlayer.transform.position = currentNode.data.position;

            TerminateInitialization();
        }

        public override void Dispose()
        {
        }

        private void Update()
        {
            if (canMove)
            {
                if (moveInput.x < -0.5f)
                {
                    if (currentNode.westIndex >= 0)
                    {
                        var target = _WorldDataProviderSystem.NavGraph[currentNode.westIndex];
                        StartCoroutine(MoveAnim(target));
                    }
                }
                else if (moveInput.x > 0.5f)
                {
                    if (currentNode.eastIndex >= 0)
                    {
                        var target = _WorldDataProviderSystem.NavGraph[currentNode.eastIndex];
                        StartCoroutine(MoveAnim(target));
                    }
                }
                else if (moveInput.y < -0.5f)
                {
                    if (currentNode.southIndex >= 0)
                    {
                        var target = _WorldDataProviderSystem.NavGraph[currentNode.southIndex];
                        StartCoroutine(MoveAnim(target));
                    }
                }
                else if (moveInput.y > 0.5f)
                {
                    if (currentNode.northIndex >= 0)
                    {
                        var target = _WorldDataProviderSystem.NavGraph[currentNode.northIndex];
                        StartCoroutine(MoveAnim(target));
                    }
                }
            }
        }

        public IEnumerator MoveAnim(ClientNavigationGraph.Node target)
        {
            canMove = false;
            var playerTransform = _CharacterStoreSystem.LocalPlayer.transform;
            Vector3 movement = target.data.position - playerTransform.position;
            while (movement.magnitude > snapRadius)
            {
                playerTransform.position += movement.normalized * moveSpeed * Time.deltaTime;
                movement = target.data.position - playerTransform.position;
                yield return null;
            }
            playerTransform.position = target.data.position;
            currentNode = target;
            canMove = true;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }
    }
}
