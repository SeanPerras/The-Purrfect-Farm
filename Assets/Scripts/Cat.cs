using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Cat : MonoBehaviour
{
    public static bool hasCat = false;
    public static Cat selectedCat = null;
    //private bool isSelectedCat = false;

    //public Sprite sprite;
    public Vector3 catioPos;
    public CatStats stats;

    private string color = "White";//, spritePath;
    private int sOrder, dirx, diry;
    public bool isWalking = false;
    private float time = 0;
    private Vector3 mousePos;
    private GameObject rightClickMenu;
    private Farm farm;
    private SpriteRenderer renderer;
    // Start is called before the first frame update
    void Awake()
    {
        //spritePath = "Sprites/Cats/Cat_Sit/";
        //transform.position = WithinRange(catioPos);//<-This is for spawning the cats at the catio.
        //SetColor(color);
        transform.SetParent(GameObject.Find("Cats").transform);
        transform.position = WithinRange(catioPos);
        renderer = GetComponent<SpriteRenderer>();
        renderer.sortingOrder = sOrder;
        rightClickMenu = GameObject.Find("Farm (9x9)").GetComponent<Farm>().sellMenu;
        farm = GameObject.Find("Farm (9x9)").GetComponent<Farm>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isWalking)
        {
            Walk(dirx, diry);
        }
        else if(stats.walkSprites?.Length > 0)
        {
            renderer.sprite = stats.sprite;
            time = Time.time;
            isWalking = Random.Range(0, 1000) >= 999;
            do {
                do dirx = Random.Range(-1, 2);
                while ((dirx > 0 && transform.position.x > -10) || (dirx < 0 && transform.position.x < -60));
                do diry = Random.Range(-1, 2);
                while ((diry > 0 && transform.position.y > 30) || (diry < 0 && transform.position.y < 17));
            } while (dirx == 0 && diry == 0);
        }
        /*//if(transform.position)
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
        //    mousePos = Vector3.zero;*/
    }
    private void OnMouseEnter()
    {
        if (!farm.IsAnyUIOpen())
        {
            rightClickMenu.transform.position = transform.position + new Vector3(0, 1, 0);
            rightClickMenu.SetActive(true);
            rightClickMenu.GetComponent<TextMeshPro>().text = name;
        }
    }
    private void OnMouseOver()
    {
        //Debug.Log("Mouse is over.");
        if (Input.GetMouseButtonUp(1) && !farm.IsAnyUIOpen())
        {
            Transform temp = rightClickMenu.transform.GetChild(0);
            temp.GetComponentInChildren<TMP_Text>().text = "Sell: " + stats.value.ToString();
            temp.gameObject.SetActive(true);
            UnityEvent click = temp.GetComponent<Button>().onClick;
            click.RemoveAllListeners();
            click.AddListener(() => GameManager.instance.ObjectToConfirm(gameObject));
            click.AddListener(() => GameManager.instance.WaitForConfirmation("Cat.Sell"));
        }
    }
    private void OnMouseExit()
    {
        if(!rightClickMenu.transform.GetChild(0).gameObject.activeSelf)
            rightClickMenu.SetActive(false);
    }
    private void OnMouseDrag()
    {
        if(!farm.IsAnyUIOpen())
        {
            //isSelectedCat = true;
            hasCat = true;
            selectedCat = this;
            GetComponent<Collider2D>().enabled = false;
            gameObject.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
            //float posY = gameObject.transform.position.y;
            renderer.sortingOrder = (int)Mathf.Round(transform.position.y * -.4f + 10f);
        }
    }
    private void OnMouseUp()
    {
        hasCat = false;
        selectedCat = null;
        //isSelectedCat = false;
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
        //Debug.Log(xpos + "," + ypos + "," + sOrder);
        return new Vector3(xpos, ypos, 0);
    }
    public void SetColor(string c)
    {
        color = c.Replace("_", " ");
        stats = Resources.Load<CatStats>("CatData/" + c.Replace(" ", "_") + "CatData");
        renderer.sprite = stats.sprite;
        //renderer.color = UnityEngine.ColorUtility.TryParseHtmlString(color, out Color co) ? co : Color.white;
        //float r = Mathf.Clamp(co.r + 40f/255f, 0, 1), g = Mathf.Clamp(co.g + 40f / 255f, 0, 1), b = Mathf.Clamp(co.b + 40f / 255f, 0, 1);
        //renderer.color = new Color(r, g, b);
        name = color + " Cat";
    }
    public string GetColor() { return color; }
    private void Walk(int directionX, int directionY)
    {
        int frame = (int)(Time.time * 5);
        // loop
        frame %= stats.walkSprites.Length;
        // set sprite
        renderer.sprite = stats.walkSprites[frame];
        if(directionX > 0) renderer.flipX = true;
        else renderer.flipX = false;
        transform.position += new Vector3(.01f * directionX, .01f * directionY);
        renderer.sortingOrder = (int)Mathf.Round(transform.position.y * -.4f + 10f);
        if (Time.time - time > 3) isWalking = false;
    }
    public IEnumerator Sell()
    {
        yield return new WaitUntil(() => GameManager.instance.IsConfirmed());
        GameManager.instance.AddCoin(stats.value);
        rightClickMenu.transform.GetChild(0).GetComponent<Button>().onClick.RemoveListener(
            () => GameManager.instance.ObjectToConfirm(gameObject));
        rightClickMenu.transform.GetChild(0).GetComponent<Button>().onClick.RemoveListener(
            () => GameManager.instance.WaitForConfirmation("Cat.Sell"));
        rightClickMenu.transform.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(Farm.DelayMenu(rightClickMenu));//.SetActive(false);
        Destroy(gameObject);
        farm.UpdateIcons(name, stats.value);
    }
}
