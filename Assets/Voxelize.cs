﻿using System.Collections;
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
    //public Camera cam;
    public int deathDelay;
    private List<GameObject> points;
    public bool ShouldUndie;

    private void Start()
    {
        mesh = sMRend.sharedMesh;
        verts = mesh.vertices;
        voxels = new List<GameObject>();
        mat = sMRend.material;

        points = new List<GameObject>() { };
        usedVerts = new List<Vector3>() { };

        for (int i = 0; i < verts.Length; i++)
        {
            if (Physics.OverlapBox(transform.TransformPoint(verts[i]), voxel.transform.localScale / 2, Quaternion.identity, LayerMask.GetMask("Voxel")).Length < 1)
            {
                GameObject inst = Instantiate(voxel, transform.TransformPoint(verts[i]), Quaternion.identity);
                voxels.Add(inst);
                points.Add(new GameObject("Point " + i));
                usedVerts.Add(transform.TransformPoint(verts[i]));
            }
        }

        for (int i = 0; i < usedVerts.Count; i++)
        {
            points[i].transform.position = usedVerts[i];
        }

        bones = sMRend.bones;
        int j = 0;
        foreach (GameObject vox in voxels)
        {
            dist = Mathf.Infinity;
            for (int i = 0; i < bones.Length; i++)
            {
                curDist = Vector3.Distance(vox.transform.position, bones[i].position);
                if (curDist < dist)
                {
                    dist = curDist;
                    closestBone = bones[i];
                }
            }
            vox.transform.SetParent(closestBone);
            points[j].transform.SetParent(closestBone);
            j++;
            
            ray = new Ray(vox.transform.position - ((closestBone.position) - (vox.transform.position)) * 1f, (closestBone.position) - vox.transform.position);
            //Debug.DrawRay(ray.origin, ray.direction * 0.5f, Color.red, 5);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Default")))
            {
                tex = (Texture2D)hit.transform.gameObject.GetComponent<SkinnedMeshRenderer>().material.mainTexture;
                color = tex.GetPixel((int)(hit.textureCoord[0] * tex.width), (int)(hit.textureCoord[1] * tex.height));
                Debug.Log(tex.GetPixel((int)(hit.textureCoord[0] * tex.width), (int)(hit.textureCoord[1] * tex.height)));
                //color = new Color(color.r / 255, color.b / 255, color.g / 255, 1);
                //Debug.Log(color);
                
                //Debug.Log(((Texture2D)hit.transform.gameObject.GetComponent<SkinnedMeshRenderer>().material.mainTexture).GetPixel((int)hit.textureCoord[0], (int)hit.textureCoord[1]));
                vox.GetComponent<MeshRenderer>().material.color = color;

            }
        }
        sMRend.sharedMesh = null;
        Destroy(this.gameObject.GetComponent<Collider>());

        tempVox = voxels.ToArray();

        voxels = tempVox.OrderBy(tempVox => tempVox.transform.position.y).ToList();

        voxels.Reverse();
        if (ShouldUndie)
            StartCoroutine (Die());

    }

    //private void Update()
    //{
    //    RaycastHit hit;
    //    if (Input.GetButtonDown("Fire1"))
    //    {
    //        Debug.Log("Fire");
    //        ray = cam.ScreenPointToRay(Input.mousePosition);
    //        Physics.Raycast(ray, out hit, Mathf.Infinity);
    //        Debug.DrawRay(ray.origin, ray.direction * 5, Color.red, 5);
    //        tex = ((Texture2D)hit.transform.gameObject.GetComponent<SkinnedMeshRenderer>().material.mainTexture);
    //        Debug.Log(tex.GetPixel((int)hit.textureCoord[0] * tex.width, (int)hit.textureCoord[1]) * tex.height);
    //        //Debug.DrawRay (cam.ScreenPointToRay (Input.mousePosition).origin, cam.ScreenPointToRay (Input.mousePosition).direction * 10, Color.red, 5);
    //    }
    //}

    IEnumerator Die ()
    {
        yield return new WaitForSeconds (deathDelay);

        for (int i = 0; i < voxels.Count; i+=3)
        {
            try
            {
                for (int a = 0; a < 3; a++)
                {

                    voxels [i + a].transform.parent = null;
                    voxels [i + a].GetComponent<Rigidbody> ().isKinematic = false;
                    voxels[i + a].GetComponent<MeshRenderer>().material.color = Color.red;
                    voxels[i + a].GetComponent<Rotatey>().enabled = false;
                    //voxels[i + a].GetComponent<Rigidbody>().AddExplosionForce(250, transform.position, 5);

                }

            }
            catch
            {
                break;
            }
            yield return new WaitForSeconds(0.00001f);
        }
        
        StartCoroutine(Undie());
        
    }

    IEnumerator Undie()
    {
        Debug.Log(points.Count);
        yield return new WaitForSeconds(5);
        float frac = 0;
        int cur = 0;
        List<Vector3> startPs = new List<Vector3>() { };

        foreach (GameObject vox in voxels)
        {
            startPs.Add(vox.transform.position);
            vox.GetComponent<Rigidbody>().isKinematic = true;
        }

        while(frac < 1)
        {
            cur = 0;
            foreach(GameObject vox in voxels)
            {
                Debug.Log(points[cur].transform.position);
                vox.transform.position = Vector3.Lerp(startPs[cur], points[cur].transform.position, frac);
                cur++;
            }
            yield return new WaitForSeconds(0.00001f);
            frac += 0.01f;
        }

        for (int i = 0; i < voxels.Count; i++)
        {
            voxels[i].transform.SetParent(points[i].transform);
            voxels [i].transform.rotation = Quaternion.identity;
            voxels[i].GetComponent<Rotatey>().enabled = true;
        }

    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    foreach (Transform bone in bones)
    //    {
    //        Gizmos.DrawSphere(bone.position, 0.01f);
    //    }
    //}

}

class RebuildSystem : ComponentSystem
{



}
