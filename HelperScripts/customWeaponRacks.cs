using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeDucky.MemeWars.WeaponRacks
{
    public class customWeaponRacks : MonoBehaviour
    {
        public GameObject[] RacksToHide;

        // Start is called before the first frame update
        void Start()
        {
            Invoke("eatWhatYouKill", 5.0f);
        }

        void eatWhatYouKill()
        {
            foreach (GameObject rackParent in RacksToHide)
            {
                Transform trans = rackParent.transform;
                Transform transChild = trans.Find("Functional_WeaponRack_Mod(Clone)/Renderable");
                Transform UILabel = trans.Find("Functional_WeaponRack_Mod(Clone)/UI Interaction Label");
                Component[] comp = UILabel.gameObject.GetComponents(typeof(Component));

                transChild.gameObject.SetActive(false);

                foreach(Component component in comp) {
                    Debug.Log(component.ToString());
                }
            }
        }
    }

}