using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeAvatar : MonoBehaviour
{
    public nuitrack.JointType[] typeJoint;
    GameObject[] CreatedJoint;
    public GameObject PrefabJoint;

    string msg = "";
    // Start is called before the first frame update
    void Start()
    {
        CreatedJoint = new GameObject[typeJoint.Length];

        for (int q = 0; q < typeJoint.Length; q++)
        {
            CreatedJoint[q] = Instantiate(PrefabJoint);
            CreatedJoint[q].transform.SetParent(transform);
        }

        msg = "Skeleton created";
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentUserTracker.CurrentUser != 0)
        {
            msg = "Skeleton Found";
            nuitrack.Skeleton skeleton = CurrentUserTracker.CurrentSkeleton;
            for (int q = 0; q < typeJoint.Length; q++)
            {
                nuitrack.Joint joint = skeleton.GetJoint(typeJoint[q]);
                Vector3 newPos = 0.001f * joint.ToVector3();
                CreatedJoint[q].transform.localPosition = newPos;
            }
        }
        else
        {
            msg = "Skeleton not found";
        }
        
    }

    void OnGUI()
    {
        GUI.color = Color.red;
        GUI.skin.label.fontSize = 50;
        GUILayout.Label(msg);
    }
}
