using System.Collections;
using System.Threading.Tasks;
using Csluder2.FusionWork;
using UnityEngine;
using UnityEngine.UI;

public class RefreshButton : MonoBehaviour

{
    private Button refreshButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (refreshButton == null)
        {
            refreshButton = GetComponent<Button>();
        }
        refreshButton.onClick.AddListener(Refresh);
    }

    private void Refresh()
    {
        StartCoroutine(RefreshWait());
    }

    private IEnumerator RefreshWait()
    {
        refreshButton.interactable = false;

        FusionConnection.instance.RefreshSessionListUI();
        yield return new WaitForSeconds(3f);
        refreshButton.interactable = true;
    }
}
