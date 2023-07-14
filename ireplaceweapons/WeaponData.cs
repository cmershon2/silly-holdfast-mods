using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : MonoBehaviour
{
    public string WeaponName;
    public string ReplacementWeaponName;
    public bool replaceLogicContains = false;
    public bool hierarchySearch = false;
    public string[] WeaponHierarchy;
    public Vector3 WeaponPrefabRotationalOffset;

    public bool hideRamRod = false;
    public string ramRodObjectName;

    public bool hideBayonet = false;
    public string bayonetObjectName;

    public bool hasScope = false;
    public RenderTexture baseScopeRenderTexture;
    public Camera scopeCamera;
    public GameObject scopeModel;
    public int scopeMaterialIndex;
    private RenderTexture rt;

    void Start(){

        if(hasScope == true){
            var scopeMat = scopeModel.GetComponent<MeshRenderer>().materials[scopeMaterialIndex];;

            rt = new RenderTexture(baseScopeRenderTexture);
            rt.Create();

            scopeCamera.targetTexture = rt;
            scopeMat.SetTexture("_MainTex", rt); // be sure to dump render texture with LOD for performance
        }
    }

    void scopeRelease(){
        if(hasScope == true){
            rt.Release();
        }
    }
}
