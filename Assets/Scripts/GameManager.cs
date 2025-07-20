using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int itemsCollected = 0;
    public GameObject jormungandrObject;

    [SerializeField] private TMP_Text itemCountText;

    public void OnItemCollected()
    {
        itemsCollected++;
        itemCountText.text = "Items Collected: " + itemsCollected.ToString();

        if(itemsCollected == 2)
        {
            StartJormungandrEvent();
        }
    }    

    void StartJormungandrEvent()
    {
        if(jormungandrObject != null)
        {
            jormungandrObject.SetActive(true);
        }
    }
}
