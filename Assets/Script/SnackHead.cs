using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SnackHead : MonoBehaviour
{
    public float velocity = 0.35f;
    public int step;
    private int x;
    private int y;
    private Vector3 headPos;

    public List<Transform> bodyList = new List<Transform>();

    public GameObject bodyPrefab;
    public Sprite[] bodySprites = new Sprite[2];
    private Transform canvas;
    private bool isDie = false;
    public GameObject dieEffect;
    //声音添加
    public AudioClip eatClip;
    public AudioClip dieClip;

    private void Awake()
    {
        //GameObject body = Instantiate(bodyPrefab, new Vector3(2000, 2000, 0), Quaternion.identity);
        canvas = GameObject.Find("Canvas").transform;
        gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(PlayerPrefs.GetString("sh", "sh02"));
        bodySprites[0] = Resources.Load<Sprite>(PlayerPrefs.GetString("sb01", "sh0201"));
        bodySprites[1] = Resources.Load<Sprite>(PlayerPrefs.GetString("sb02", "sh0202"));
        Time.timeScale = 1;
    }

    private void Start()
    {
        InvokeRepeating("Move", 0, velocity);
        x = 0;
        y = step;
    }

    #region 方向控制
    private void Update()
    {
        if (Input.GetKey(KeyCode.W) && y != -step && !GameManager.Instance.isPause && !isDie)
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            x = 0;
            y = step;
        }
        if (Input.GetKey(KeyCode.S) && y != step && !GameManager.Instance.isPause && !isDie)
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, 180);
            x = 0;
            y = -step;
        }
        if (Input.GetKey(KeyCode.A) && x != step && !GameManager.Instance.isPause && !isDie)
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, 90);

            x = -step;
            y = 0;
        }
        if (Input.GetKey(KeyCode.D) && x != -step && !GameManager.Instance.isPause && !isDie)
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, -90);
            x = step;
            y = 0;
        }
        if (Input.GetKeyDown(KeyCode.Space) && !GameManager.Instance.isPause && !isDie)
        {
            CancelInvoke();
            InvokeRepeating("Move", 0, velocity - 0.2f);
        }
        if (Input.GetKeyUp(KeyCode.Space) && !GameManager.Instance.isPause && !isDie)
        {
            CancelInvoke();
            InvokeRepeating("Move", 0, velocity);
        }
    }
    #endregion

    void Move()
    {
        //保存蛇头移动前的位置
        headPos = gameObject.transform.localPosition;
        //蛇头向期望位置移动
        gameObject.transform.localPosition = new Vector3(headPos.x + x, headPos.y + y, headPos.z);

        if (bodyList.Count > 0)
        {
            //由于我们是双色蛇身，此方法弃用
            //将蛇尾移动到蛇头的位置
            //bodyList.Last().localPosition = headPos;
            //将蛇尾在List中的位置更新到最前
            //bodyList.Insert(0, bodyList.Last());
            //移除List最末尾的蛇尾引用
            //bodyList.RemoveAt(bodyList.Count - 1);

            for (int i = bodyList.Count-2; i >=0; i--)
            {
                bodyList[i + 1].localPosition = bodyList[i].localPosition;
            }
            bodyList[0].localPosition = headPos;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            Destroy(collision.gameObject);
            GameManager.Instance.UpdateUI();
            Grow();
            FoodMaker.instance.MakeFood(Random.Range(0,100)<20?true:false);
        }
        else if (collision.gameObject.CompareTag("Body"))
        {
            Die();
        }
        else if (collision.gameObject.CompareTag("Reward"))
        {
            Destroy(collision.gameObject);
            GameManager.Instance.UpdateUI(Random.Range(5,15)*10);
            Grow();
        }
        else
        {
            switch (collision.gameObject.name)
            {
                case "up":
                    transform.localPosition = new Vector3(transform.localPosition.x, -transform.localPosition.y + 30, transform.localPosition.z);
                    break;
                case "down":
                    transform.localPosition = new Vector3(transform.localPosition.x, -transform.localPosition.y - 30, transform.localPosition.z);
                    break;
                case "left":
                    transform.localPosition = new Vector3(-transform.localPosition.x + 180, transform.localPosition.y, transform.localPosition.z);
                    break;
                case "right":
                    transform.localPosition = new Vector3(-transform.localPosition.x + 240, transform.localPosition.y, transform.localPosition.z);
                    break;
                default:
                    break;
            }
        }

        if (collision.gameObject.CompareTag("Border"))
        {
            if (GameManager.Instance.hasBorder)
            {
                Die();
            }
        }
    }

    void Grow()
    {
        AudioSource.PlayClipAtPoint(eatClip,Vector3.zero);
        int index = (bodyList.Count % 2 == 0) ? 0 : 1;
        GameObject body = Instantiate(bodyPrefab);
        body.transform.position=new Vector3(500,500,0);
        body.GetComponent<Image>().sprite = bodySprites[index];
        body.transform.SetParent(canvas, false);
        bodyList.Add(body.transform);  
    }

    void Die()
    {
        AudioSource.PlayClipAtPoint(dieClip, Vector3.zero);
        CancelInvoke();
        isDie = true;
        Instantiate(dieEffect);
        PlayerPrefs.SetInt("lastLength", GameManager.Instance.length);
        PlayerPrefs.SetInt("lastScore", GameManager.Instance.score);
        StartCoroutine(GameOver(1.5f));
        if (PlayerPrefs.GetInt("bestScore",0)< GameManager.Instance.score)
        {
            PlayerPrefs.SetInt("bestLength", GameManager.Instance.length);
            PlayerPrefs.SetInt("bestScore", GameManager.Instance.score);
        }
    }

    IEnumerator GameOver(float t)
    {
        yield return new WaitForSeconds(t);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

}
