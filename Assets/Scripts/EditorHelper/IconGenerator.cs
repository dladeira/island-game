using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IconGenerator : MonoBehaviour
{
    public string folderPath;
    public string iconName;

    public List<GameObject> sceneObjects;
    public List<InventoryItemData> dataObjects;

    [ContextMenu("Screenshot")]
    private void ProcessScreenshots()
    {
        StartCoroutine(Screenshot());
    }

    private IEnumerator Screenshot()
    {
        for (int i = 0; i < sceneObjects.Count; i++)
        {
            GameObject obj = sceneObjects[i];
            obj.gameObject.SetActive(false);
        }

        for (int i = 0; i < sceneObjects.Count; i++)
        {
            GameObject obj = sceneObjects[i];
            InventoryItemData data = dataObjects[i];

            obj.gameObject.SetActive(true);

            yield return null;

            TakeScreenshot(Application.dataPath + "/" + folderPath + "/" + data.id + "_Icon.png");

            yield return null;
            obj.gameObject.SetActive(false);

            Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/" + folderPath + "/" + data.id + "_Icon.png");
            yield return null;

            if (s != null)
            {
                data.icon = s;
                EditorUtility.SetDirty(data);
            }
            else
            {
                Debug.Log("sprite is null");
            }

            yield return null;
        }
        Debug.Log("Finished icon generation");
    }

    public void TakeScreenshot(string fullPath)
    {
        RenderTexture rt = new RenderTexture(256, 256, 24);
        GetComponent<Camera>().targetTexture = rt;
        Texture2D screenShot = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        GetComponent<Camera>().Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        GetComponent<Camera>().targetTexture = null;
        RenderTexture.active = null;

        if (Application.isEditor)
        {

            DestroyImmediate(rt);
        }
        else
        {
            Destroy(rt);
        }
        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath, bytes);
#if UNIT_EDITOR
        AssetDatabase.Refresh();
#endif
    }
}
