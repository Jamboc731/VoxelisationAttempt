using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private List<Vector3> usedVerts;
    private GameObject [] tempVox;
    private Material mat;
    private Texture2D tex;
    private Ray ray;
    private RaycastHit hit;
    private Color color = Color.red;

    private void Start ()
    {
        mesh = sMRend.sharedMesh;
        verts = mesh.vertices;
        voxels = new List<GameObject> ();
        mat = sMRend.material;
        tex = (Texture2D) mat.mainTexture;



        for(int i = 0; i < verts.Length; i++)
        {
            if (Physics.OverlapBox (transform.TransformPoint (verts [i]), voxel.transform.localScale / 2, Quaternion.identity, LayerMask.GetMask("Default")).Length < 1)
            {
                ray = new Ray (verts [i] + (transform.TransformPoint (verts [i]) - transform.position), transform.position - transform.TransformPoint (verts [i]));
                if(Physics.Raycast (ray, out hit, Mathf.Infinity, LayerMask.GetMask("Voxel")))
                {
                    color = ((Texture2D) hit.transform.gameObject.GetComponent<SkinnedMeshRenderer> ().material.mainTexture).GetPixel ((int) hit.textureCoord [1], (int) hit.textureCoord [1]);
                }
                GameObject inst = Instantiate (voxel, transform.TransformPoint (verts [i]), Quaternion.identity);
                inst.GetComponent<MeshRenderer> ().material.color = color;
                voxels.Add (inst);
                //usedVerts.Add (verts [i]);                
            }
        }

        //sMRend.sharedMesh = null;
        Destroy(this.gameObject.GetComponent<Collider> ());

        bones = sMRend.bones;
        
        foreach (GameObject vox in voxels)
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

        tempVox = voxels.ToArray ();

        voxels = tempVox.OrderBy (tempVox => tempVox.transform.position.y).ToList();

        voxels.Reverse ();

        StartCoroutine (Die ());

    }

    //private void Update ()
    //{
    //    RaycastHit hit;
    //    if (Input.GetButtonDown ("Fire1"))
    //    {
    //        Physics.Raycast (cam.ScreenPointToRay (Input.mousePosition), out hit, Mathf.Infinity);
    //        Debug.Log (((Texture2D) hit.transform.gameObject.GetComponent<SkinnedMeshRenderer> ().material.mainTexture).GetPixel ((int) hit.textureCoord [1], (int) hit.textureCoord [1]));
    //        //Debug.DrawRay (cam.ScreenPointToRay (Input.mousePosition).origin, cam.ScreenPointToRay (Input.mousePosition).direction * 10, Color.red, 5);
    //    }
    //}

    IEnumerator Die ()
    {
        yield return new WaitForSeconds (5);

        for (int i = 0; i < voxels.Count; i+=3)
        {
            for (int a = 0; a < 3; a++)
            {

                voxels [i + a].transform.parent = null;
                voxels [i + a].GetComponent<Rigidbody> ().isKinematic = false;
                Destroy (voxels [i + a].GetComponent<Rotatey> ());
                //vox.GetComponent<Rigidbody> ().AddExplosionForce (250, transform.position, 5);

            }
            yield return new WaitForSeconds (0.00001f);
        }
        
    }

}
