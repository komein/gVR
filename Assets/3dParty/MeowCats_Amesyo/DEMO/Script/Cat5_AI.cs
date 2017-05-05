using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace MeowCats_amesyo
{
    public class Cat5_AI : MonoBehaviour
    {
        [Header("Base Object")]
        public GameObject P_Cat;
        public GameObject MoveTarget;
        public GameObject MoveArea;

        [Header("Move Area")]
        public float L_xlimit = 1.5f;
        public float R_xlimit = -1.5f;
        public float F_zlimit = -2f;
        public float B_zlimit = 2f;
        [Header("Vit=50 Peppy  Vit=2000 Meek")]
        public float Vitality = 50;
        private Rigidbody rb;
        private Animator animator;
        private Vector3 cat_init_pos;

        [Header("for animeter Status")]
        public int c_state;
        [Header("Flag")]
        public bool OnMove_Flag = false;
        public bool MoveTarget_MoveFlag = false;
        public bool OnAttack_Flag = false;
        [Header("重力")]
        public float gravity = 20f;
        private Vector3 moveDirection;
        public float speed = 0;
        [Header("移動先の座標")]
        public Vector3 endPosition = Vector3.zero;

        [Header("敵の座標")]
        public Vector3 EZAHYOU;

        void Start()
        {
            L_xlimit = MoveArea.transform.localScale.x / 2;
            R_xlimit = -1 * (MoveArea.transform.localScale.x / 2);
            F_zlimit = -1 * (MoveArea.transform.localScale.z / 2);
            B_zlimit = MoveArea.transform.localScale.z / 2;

            endPosition = RandPosition();

            rb = P_Cat.GetComponentInChildren<Rigidbody>();
            animator = P_Cat.GetComponentInChildren<Animator>();
        }

        void Update()
        {
            c_state = Random.Range(0, 10);
            animator.SetInteger("pat", c_state);

            if (OnMove_Flag == false)
            {
                ResetAnimatorStates();

                float cRnd = Random.Range(0, (Vitality * 2));

                if (cRnd < 1 && MoveTarget_MoveFlag == false)
                {
                    OnMove_Flag = true;
                    SpeedPattern();
                    endPosition = RandPosition();

                    if (Vector3.Distance(this.transform.position, endPosition) > 0.8)
                    {
                        OnMove_Flag = true;
                        MoveTarget_MoveFlag = true;
                        SpeedPattern();
                    }
                    if (Vector3.Distance(P_Cat.transform.position, endPosition) < 0.4)
                    {
                        OnMove_Flag = false;
                        MoveTarget_MoveFlag = false;
                        ResetAnimatorStates();
                    }
                }

            }

            if (OnMove_Flag)
            {
                float cRnd = Random.Range(0, (Vitality * 2));
                if (cRnd < 1 && MoveTarget_MoveFlag == false)
                {
                    endPosition = RandPosition();
                    MoveTarget.transform.position = endPosition;
                    SpeedPattern();
                }

                Vector3 movePos = (endPosition - P_Cat.transform.position);
                movePos.y = 0;
                rb.velocity = movePos.normalized * Time.deltaTime * speed * 100;

                P_Cat.transform.rotation = Quaternion.LookRotation(movePos);
            }
            if (Vector3.Distance(P_Cat.transform.position, endPosition) < 0.4)
            {
                MoveTarget_MoveFlag = false;
                OnMove_Flag = false;
                ResetAnimatorStates();

            }
        }

        private void ResetAnimatorStates()
        {
            animator.SetBool("walk", false);
            animator.SetBool("slow", false);
            animator.SetBool("run", false);
        }

        Vector3 RandPosition()
        {
            Vector3 Move_Pos;

            Move_Pos.x = Random.Range(MoveArea.transform.position.x + R_xlimit, MoveArea.transform.position.x + L_xlimit);
            Move_Pos.z = Random.Range(MoveArea.transform.position.z + F_zlimit, MoveArea.transform.position.z + B_zlimit);
            Move_Pos.y = P_Cat.transform.position.y;

            MoveTarget.transform.position = Move_Pos;

            return Move_Pos;

        }

        void SpeedPattern()
        {
            ResetAnimatorStates();

            float sRnd = Random.Range(0, 5);

            if (sRnd >= 0 && sRnd < 2)
            {
                speed = 2;
                animator.SetBool("run", true);
            }
            if (sRnd >= 2 && sRnd < 4.5f)
            {
                speed = 1;
                animator.SetBool("walk", true);
            }
            if (sRnd >= 4.5F)
            {
                speed = 0.3F;
                animator.SetBool("walk", true);
            }

        }

    }
}
