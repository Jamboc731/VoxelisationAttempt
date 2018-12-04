using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxelize : MonoBehaviour {

    public GameObject voxel;
    public SkinnedMeshRenderer sMRend;
    private Mesh mesh;
    private Vector3 [] verts;
    private Transform [] bones;
    private List<GameObject> voxels;
    private float dist;
    private Transform closestBone;
    private float curDist;

    private void Start ()
    {
        mesh = sMRend.sharedMesh;
        sMRend.sharedMesh = null;
        verts = mesh.vertices;
        voxels = new List<GameObject> ();

        for(int i = 0; i < verts.Length; i++)
        {
            if (Physics.OverlapBox (transform.TransformPoint (verts [i]), voxel.transform.localScale/2).Length < 1)
            {
                Debug.Log (transform.TransformPoint (verts [i]));
                GameObject inst = Instantiate (voxel, transform.TransformPoint (verts [i]), Quaternion.identity);
                Debug.Log (inst);
                voxels.Add (inst);
            }
        }

        bones = sMRend.bones;
        

        foreach(GameObject vox in voxels)
        {
            dist = Mathf.Infinity;
            for (int i = 0; i < bones.Length; i++)
            {
                curDist = Vector3.Distance (vox.transform.position, bones [i].position);
                if (curDist < dist)
                {
                    dist = curDist;
                    closestBone = bones [i];
                }
            }
            vox.transform.SetParent (closestBone);

        }

    }

    public GameObject[] FindObjectsInLayer ()
    {

        List<GameObject> objs = new List<GameObject>();

        foreach (GameObject ob in GameObject.FindObjectsOfType<GameObject> ())
            if (ob.layer == 9)
                objs.Add (ob);
        return(objs.ToArray());

    }

}
