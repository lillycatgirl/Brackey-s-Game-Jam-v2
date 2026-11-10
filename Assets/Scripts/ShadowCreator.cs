using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class ShadowCaster2DExtensions
{
    public static void SetPath(this ShadowCaster2D shadowCaster, Vector3[] path)
    {
        FieldInfo shapeField = typeof(ShadowCaster2D).GetField("m_ShapePath",
                                                               BindingFlags.NonPublic |
                                                               BindingFlags.Instance);
        shapeField.SetValue(shadowCaster, path);
    }
    public static void SetPathHash(this ShadowCaster2D shadowCaster, int hash)
    {
        FieldInfo hashField = typeof(ShadowCaster2D).GetField("m_ShapePathHash",
                                                              BindingFlags.NonPublic |
                                                              BindingFlags.Instance);
        hashField.SetValue(shadowCaster, hash);
    }
}

public class ShadowCaster2DGenerator
{

#if UNITY_EDITOR

    [UnityEditor.MenuItem("Generate Shadow Casters", menuItem = "Tools/Generate Shadow Casters")]
    public static void GenerateShadowCasters()
    {
        CompositeCollider2D[] colliders = GameObject.FindObjectsOfType<CompositeCollider2D>();

        for(int i = 0; i < colliders.Length; ++i)
        {
            if (colliders[i].CompareTag("Wall"))
                GenerateTilemapShadowCastersInEditor(colliders[i], false);
        }
    }

    [UnityEditor.MenuItem("Generate Shadow Casters (Self Shadows)", menuItem = "Tools/Generate Shadow Casters (Self Shadows)")]
    public static void GenerateShadowCastersSelfShadows()
    {
        CompositeCollider2D[] colliders = GameObject.FindObjectsOfType<CompositeCollider2D>();

        for (int i = 0; i < colliders.Length; ++i)
        {
            if (colliders[i].CompareTag("Wall"))
                GenerateTilemapShadowCastersInEditor(colliders[i], true);
        }
    }
    public static void GenerateTilemapShadowCastersInEditor(CompositeCollider2D collider, bool selfShadows)
    {
        GenerateTilemapShadowCasters(collider, selfShadows);

        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
    }

#endif

    public static void GenerateTilemapShadowCasters(CompositeCollider2D collider, bool selfShadows)
    {
        // First, it destroys the existing shadow casters
        ShadowCaster2D[] existingShadowCasters = collider.GetComponentsInChildren<ShadowCaster2D>();
        CompositeShadowCaster2D[] existingCompositeShadowCasters = collider.GetComponents<CompositeShadowCaster2D>();
        
        for (int i = 0; i < existingShadowCasters.Length; ++i)
        {
            if(existingShadowCasters[i].transform.parent != collider.transform)
            {
                continue;
            }

            GameObject.DestroyImmediate(existingShadowCasters[i].gameObject);
        }
        
        for (int i = 0; i < existingCompositeShadowCasters.Length; ++i)
        {
            GameObject.DestroyImmediate(existingCompositeShadowCasters[i]);
        }

        // Then it creates the new shadow casters, based on the paths of the composite collider
        int pathCount = collider.pathCount;
        List<Vector2> pointsInPath = new List<Vector2>();
        List<Vector3> pointsInPath3D = new List<Vector3>();
        
        Debug.Log(pathCount);
        collider.gameObject.AddComponent<CompositeShadowCaster2D>();

        for (int i = 0; i < pathCount; ++i)
        {
            collider.GetPath(i, pointsInPath);

            GameObject newShadowCaster = new GameObject("ShadowCaster2D");
            newShadowCaster.isStatic = true;
            newShadowCaster.transform.SetParent(collider.transform, false);

            for(int j = 0; j < pointsInPath.Count; ++j)
            {
                pointsInPath3D.Add(pointsInPath[j]);
            }

            ShadowCaster2D component = newShadowCaster.AddComponent<ShadowCaster2D>();
            component.SetPath(pointsInPath3D.ToArray());
            component.SetPathHash(Random.Range(int.MinValue, int.MaxValue)); // The hashing function GetShapePathHash could be copied from the LightUtility class
            component.selfShadows = selfShadows;
            component.Update();

            pointsInPath.Clear();
            pointsInPath3D.Clear();
        }
    }
}