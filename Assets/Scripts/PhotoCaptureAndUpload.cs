using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PhotoCaptureAndUpload : MonoBehaviour
{
    [SerializeField] private string apiBase;

    public void CaptureAndUpload(string claimId)
    {
        StartCoroutine(CaptureRoutine(claimId));
    }

    IEnumerator CaptureRoutine(string claimId)
    {
        yield return new WaitForEndOfFrame();
        var tex = ScreenCapture.CaptureScreenshotAsTexture();
        var bytes = tex.EncodeToJPG(85);
        Destroy(tex);

        string fileName = $"photo_{System.DateTime.UtcNow:yyyyMMdd_HHmmss}.jpg";

        string sasReqUrl = $"{apiBase}/{claimId}/photos/sas?fileName={fileName}";
        var sasReq = UnityWebRequest.Post(sasReqUrl, "");
        yield return sasReq.SendWebRequest();

        if (sasReq.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("SAS request failed: " + sasReq.error);
            yield break;
        }

        string sasUrl = sasReq.downloadHandler.text.Trim('\"');

        var put = UnityWebRequest.Put(sasUrl, bytes);
        put.SetRequestHeader("x-ms-blob-type", "BlockBlob");

        yield return put.SendWebRequest();

        if (put.result != UnityWebRequest.Result.Success)
            Debug.LogError("Upload failed: " + put.error);
        else
            Debug.Log("Upload OK");
    }
}
