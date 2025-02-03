using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cat : MonoBehaviour
{
    public static bool hasCat = false;
    public static Cat selectedCat = null;
    private bool isSelectedCat = false;

    //public Sprite sprite;
    public Vector3 catioPos;
    public CatStats stats;

    private string color = "White";//, spritePath;
    private int sOrder;
    private Vector3 mousePos;
    private GameObject rightClickMenu;
    // Start is called before the first frame update
    void Awake()
    {
        //spritePath = "Sprites/Cats/Cat_Sit/";
        //transform.position = WithinRange(catioPos);//<-This is for spawning the cats at the catio.
        //SetColor(color);
        transform.SetParent(GameObject.Find("Cats").transform);
        transform.position = WithinRange(catioPos);
        GetComponent<SpriteRenderer>().sortingOrder = sOrder;
        rightClickMenu = GameObject.Find("Farm (9x9)").GetComponent<Farm>().sellMenu;
        //rightClickMenu.GetComponent<Canvas>().worldCamera =
        //    GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(transform.position)
        //if (Input.GetMouseButtonDown(0))
        //    mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //if (Input.GetMouseButton(0))// && Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), gameObject.transform.position) <= 2)
        //{
        //    if (!hasCat)
        //    {
        //        hasCat = true;
        //        isSelectedCat = true;
        //    }
        //    if (isSelectedCat)
        //    {
        //        gameObject.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
        //        float posY = gameObject.transform.position.y;
        //        gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)Mathf.Round(posY * -.4f + 10f);
        //    }
        //}
        //else if (Input.GetMouseButtonUp(0))
        //{
        //    hasCat = false;
        //    isSelectedCat = hasCat;
        //}
        //if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && !thisOne)
        //{
        //    rightClickMenu.transform.GetChild(0).gameObject.SetActive(false);
        //    rightClickMenu.SetActive(false);
        //    rightClickMenu.transform.GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.RemoveListener(Sell);
        //    thisOne = false;
        //}
        //    mousePos = Vector3.zero;
    }
    private void OnMouseEnter()
    {
        rightClickMenu.transform.position = transform.position + new Vector3(0, 1, 0);
        rightClickMenu.SetActive(true);
        rightClickMenu.GetComponent<TextMeshPro>().text = name;
    }
    private void OnMouseOver()
    {
        //Debug.Log("Mouse is over.");
        if (Input.GetMouseButtonUp(1))
        {
            Transform temp = rightClickMenu.transform.GetChild(0);
            temp.GetComponentInChildren<TextMeshProUGUI>().text = "Sell: " + stats.value.ToString();
            temp.gameObject.SetActive(true);
            temp.GetComponent<Button>().onClick.AddListener(
                () => GameManager.instance.ObjectToConfirm(gameObject));
            temp.GetComponent<Button>().onClick.AddListener(
                () => GameManager.instance.WaitForConfirmation("Cat.Sell"));
        }
    }
    private void OnMouseExit()
    {
        if(!rightClickMenu.transform.GetChild(0).gameObject.activeSelf)
            rightClickMenu.SetActive(false);
    }
    private void OnMouseDrag()
    {
        if (!isSelectedCat) isSelectedCat = true;
        if (isSelectedCat)
        {
            hasCat = true;
            selectedCat = this;
            GetComponent<Collider2D>().enabled = false;
            gameObject.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
            float posY = gameObject.transform.position.y;
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)Mathf.Round(posY * -.4f + 10f);
        }
    }
    private void OnMouseUp()
    {
        hasCat = false;
        selectedCat = null;
        isSelectedCat = false;
        GetComponent<Collider2D>().enabled = true;
        rightClickMenu.transform.GetChild(0).GetComponent<Button>().onClick.RemoveListener(
            () => GameManager.instance.ObjectToConfirm(gameObject));
        rightClickMenu.transform.GetChild(0).GetComponent<Button>().onClick.RemoveListener(
            () => GameManager.instance.WaitForConfirmation("Cat.Sell"));
    }
    public Vector3 WithinRange(Vector3 loc)
    {
        //Vector3(-7.75, 21.5, 0)
        //-5.5,+1.5
        float count = transform.parent.childCount,
        minx = loc.x - 13.5f, miny = loc.y + 1.5f,
        xpos = minx + 1.5f * ((count % 6) + 2*(int)(count / 6)),
        ypos = miny + .75f * ((count % 6) - (int)(count / 6));
        sOrder = 2 - (int)count % 6;
        Debug.Log(xpos + "," + ypos + "," + sOrder);
        return new Vector3(xpos, ypos, 0);
    }
    public void SetColor(string c)
    {
        color = c.Replace("_", " ");
        stats = Resources.Load<CatStats>("CatData/" + c.Replace(" ", "_") + "CatData");
        SpriteRenderer curr_Sprite = GetComponent<SpriteRenderer>();
        curr_Sprite.sprite = stats.sprite;
        //curr_Sprite.color = UnityEngine.ColorUtility.TryParseHtmlString(color, out Color co) ? co : Color.white;
        //float r = Mathf.Clamp(co.r + 40f/255f, 0, 1), g = Mathf.Clamp(co.g + 40f / 255f, 0, 1), b = Mathf.Clamp(co.b + 40f / 255f, 0, 1);
        //curr_Sprite.color = new Color(r, g, b);
        name = color + " Cat";
    }
    public string GetColor() { return color; }
    public IEnumerator Sell()
    {
        yield return new WaitUntil(() => GameManager.instance.IsConfirmed());
        GameManager.instance.AddCoin(stats.value);
        rightClickMenu.transform.GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.RemoveListener(
            () => GameManager.instance.ObjectToConfirm(gameObject));
        rightClickMenu.transform.GetChild(0).GetComponent<UnityEngine.UI.Button>().onClick.RemoveListener(
            () => GameManager.instance.WaitForConfirmation("Cat.Sell"));
        rightClickMenu.transform.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(Farm.DelayMenu(rightClickMenu));//.SetActive(false);
        Destroy(gameObject);
    }
}
