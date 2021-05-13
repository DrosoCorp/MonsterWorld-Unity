using UnityEngine;

namespace MonsterWorld.Unity.Startup
{
    public class ConnectToServerState : StateMachineBehaviour
    {

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Coucou");
        }
    }
}
