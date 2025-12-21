using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    [Header("Mesh Trail Settings")]
    [SerializeField] private float meshRefreshRate = 0.1f;  // Time between objects
    [SerializeField] private float meshTrailScale = 0.5f;
    [SerializeField] private float meshDestroyDelay = 1f;
    [SerializeField] private Transform positionToSpawn;

    [Header("Custom Material")]
    [SerializeField] private Material shaderMat;
    [SerializeField] private float shaderVarRate = 0.1f;
    [SerializeField] private float shaderVarRefreshRate = 0.05f;

    private string SHADER_VAR_REF = "_Alpha";
    private float activeTime;         // Duration for the "trail"
    private MeshRenderer[] meshRenderers;
    private bool isTrailActive;

    public void StartTrailCoroutine(float dashDuration)
    {
        activeTime = dashDuration;

        if(!isTrailActive)
        {
            isTrailActive = true;
            StartCoroutine(AcitvateTrail(activeTime));
        }
    }

    private IEnumerator AcitvateTrail(float timeActive)
    {
        while (timeActive > 0)
        {
            timeActive -= meshRefreshRate;

            // Checks how many objects the parent has.
            if (meshRenderers == null)
                meshRenderers = GetComponentsInChildren<MeshRenderer>();

            // For every object it creates a dupe game object and meshrendere + filter
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                GameObject obj = new GameObject();
                obj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);
                obj.transform.localScale = meshRenderers[i].transform.localScale * meshTrailScale;

                MeshRenderer mr = obj.AddComponent<MeshRenderer>();
                MeshFilter mf = obj.AddComponent<MeshFilter>();

                mf.mesh = Instantiate(meshRenderers[i].GetComponent<MeshFilter>().sharedMesh);

                if(shaderMat != null)
                    mr.material = shaderMat;
                else
                    mr.material = meshRenderers[i].sharedMaterial;

                StartCoroutine(AnimateMaterialFloat(mr.material, 0f, shaderVarRate, shaderVarRefreshRate));

                Destroy(obj, meshDestroyDelay);
            }

            yield return new WaitForSeconds(meshRefreshRate);
        }

        isTrailActive = false;
    }

    private IEnumerator AnimateMaterialFloat(Material mat, float goal, float rate, float refreshRate)
    {
        float valueToAnimate = mat.GetFloat(SHADER_VAR_REF);

        while (valueToAnimate > goal)
        {
            valueToAnimate -= rate;
            mat.SetFloat(SHADER_VAR_REF, valueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
