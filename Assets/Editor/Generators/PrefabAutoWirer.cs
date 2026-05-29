using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.Reflection;

public class PrefabAutoWirer : EditorWindow
{
    private const string PREFAB_DIR = "Assets/Resources/Prefabs";

    [MenuItem("Tools/自动挂载UI脚本并连线引用")]
    public static void AutoWireAll()
    {
        Debug.Log("========== 开始自动挂载和连线 ==========");

        WirePrefab("HomePanel", typeof(HomeUI));
        WirePrefab("TalentSelectPanel", typeof(TalentSelectUI));
        WirePrefab("HighSchoolPanel", typeof(HighSchoolUI));
        WirePrefab("GaokaoPanel", typeof(GaokaoUI));
        WirePrefab("ZhiyuanPanel", typeof(ZhiyuanUI));
        WirePrefab("CollegePanel", typeof(CollegeUI));
        WirePrefab("LifePanel", typeof(LifeUI));
        WirePrefab("ResultPanel", typeof(ResultUI));

        AssetDatabase.Refresh();
        Debug.Log("========== 全部完成 ==========");
    }

    private static void WirePrefab(string prefabName, Type componentType)
    {
        string path = PREFAB_DIR + "/" + prefabName + ".prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError($"[Wire] 找不到预制体: {path}");
            return;
        }

        GameObject contents = PrefabUtility.LoadPrefabContents(path);

        Component comp = contents.GetComponent(componentType);
        if (comp == null)
        {
            comp = contents.AddComponent(componentType);
            Debug.Log($"[Wire] {prefabName} → 添加 {componentType.Name}");
        }

        SerializedObject so = new SerializedObject(comp);
        bool changed = false;

        SerializedProperty prop = so.GetIterator();
        bool enterChildren = true;
        while (prop.NextVisible(enterChildren))
        {
            enterChildren = false;
            if (prop.propertyType != SerializedPropertyType.ObjectReference)
                continue;
            if (prop.name == "m_Script")
                continue;

            string childName = SnakeToPascal(prop.name);
            Transform found = FindChildDeep(contents.transform, childName);
            if (found != null)
            {
                Type fieldType = GetFieldType(componentType, prop.name);
                if (fieldType == typeof(GameObject))
                {
                    prop.objectReferenceValue = found.gameObject;
                }
                else
                {
                    var childComp = found.GetComponent(fieldType);
                    if (childComp != null)
                    {
                        prop.objectReferenceValue = childComp;
                    }
                }

                if (prop.objectReferenceValue != null)
                {
                    changed = true;
                }
                else
                {
                    Debug.LogWarning($"  [Wire] {prefabName}.{prop.name} → 找到 '{childName}' 但没有 {fieldType.Name} 组件");
                }
            }
            else
            {
                Debug.LogWarning($"  [Wire] {prefabName}.{prop.name} → 未找到子对象 '{childName}'，需要手动拖拽");
            }
        }

        if (changed)
        {
            so.ApplyModifiedProperties();
        }

        PrefabUtility.SaveAsPrefabAsset(contents, path);
        PrefabUtility.UnloadPrefabContents(contents);
        Debug.Log($"[Wire] {prefabName} 连线完成");
    }

    private static Type GetFieldType(Type componentType, string fieldName)
    {
        FieldInfo fi = componentType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (fi != null) return fi.FieldType;
        PropertyInfo pi = componentType.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (pi != null) return pi.PropertyType;
        return typeof(Component);
    }

    private static string SnakeToPascal(string camelCase)
    {
        if (string.IsNullOrEmpty(camelCase)) return camelCase;
        char[] chars = camelCase.ToCharArray();
        chars[0] = char.ToUpper(chars[0]);
        return new string(chars);
    }

    private static Transform FindChildDeep(Transform parent, string name)
    {
        if (parent.name.Equals(name, StringComparison.OrdinalIgnoreCase))
            return parent;
        foreach (Transform child in parent)
        {
            Transform found = FindChildDeep(child, name);
            if (found != null) return found;
        }
        return null;
    }
}