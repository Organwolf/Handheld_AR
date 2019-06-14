using System.Collections;
using UnityEngine;

public class MatScript : MonoBehaviour
{
    Color[] colors;
    Renderer thisRend;
    float transitionTime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        thisRend = GetComponent<Renderer>();
        colors = new Color[6];
        //colors[0] = Color.white;
        colors[0] = new Color(0.3f, 0.4f, 0.6f, 0.3f);
        colors[1] = Color.white;
        colors[2] = Color.white;
        colors[3] = Color.white;
        colors[4] = new Color(0.3f, 0.4f, 0.6f, 0.3f);
        colors[5] = new Color(0.3f, 0.4f, 0.6f, 0.3f);
        StartCoroutine(ColorChange());
    }

    IEnumerator ColorChange()
    {
        while(true)
        {
            Color newColor = colors[(UnityEngine.Random.Range(0, 5))];
            float transitionRate = 0;

            while (transitionRate < 1)
            {
                thisRend.material.SetColor("_Color", Color.Lerp(thisRend.material.color, 
                    newColor, Time.deltaTime * transitionRate));
                transitionRate += Time.deltaTime / transitionTime;
                yield return null;
            }
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
