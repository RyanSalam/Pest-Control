using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageIndicator : MonoBehaviour
{

    [SerializeField] float lifeTime = 0.25f;
    Timer lifeTimeTimer;

    bool activated;
    TextMeshPro myDamageIndicator;
    MeshRenderer myMeshRenderer;
    MaterialPropertyBlock myMaterialPropBlock;
    

    //colour, float(font size)
    Color myColor;
    float fontSize = 4.5f;
    float yPos;
    float yPosScrollValue = 0.75f;
    private void Awake()
    {
        lifeTimeTimer = new Timer(lifeTime, true);
        lifeTimeTimer.OnTimerEnd += despawnText;
        myDamageIndicator = GetComponent<TextMeshPro>();
        myMeshRenderer = GetComponent<MeshRenderer>();
        
    }
    private void OnEnable()
    {
        activated = true;
        lifeTimeTimer.PlayFromStart();
    }
    private void OnDisable()
    {
        activated = false;
    }

    private void Update()
    {
        if (activated)
        {
            lifeTimeTimer.Tick(Time.deltaTime);
            myDamageIndicator.fontSize -= Time.deltaTime;
            //myColor.a -= 0.075f;
            myColor.a -= Time.deltaTime * 0.5f;
            myDamageIndicator.color = myColor;
            //myDamageIndicator.rectTransform.position += yPosScrollValue;
        }
    }

    public void setDamageIndicator( int damage, Color color)
    {
        //myDamageIndicator = gameObject.GetComponent<TextMeshPro>();

        if (!myDamageIndicator)
            return;



        myDamageIndicator.text = damage.ToString();
        myDamageIndicator.fontSize = fontSize;
        yPos = myDamageIndicator.rectTransform.position.y;

        if (color != null)
        {
            color.a = 1; //have to reset the alpha to one so its opaque. Can make this a variable so we can mess about with transparancy
            myColor = color;
            myDamageIndicator.color = color;
        }
       

    }

    private void despawnText()
    {
        //Debug.Log("Despawn text");
        gameObject.SetActive(false);
    }
}
