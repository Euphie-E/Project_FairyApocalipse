using System.Collections.Generic;
using UnityEngine;

public class Collectible : Savable
{
    private GameObject collectible;
    public bool Collected = false;
    [Header("Effect")]
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float verticalAmplitude = 2;
    [SerializeField] private float verticalSpeed = 1;
    [SerializeField] protected int id = -1;

    Vector3 originalPos;
    float timeVariation;

    void Start()
    {
        collectible = transform.GetChild(0).gameObject;
        originalPos = transform.position;
        saveData = new Save(Types.Collectible,id);
        LevelManager.Instance?.Add(this);
    }

    void Update()
    {
        if (!Collected)
        {
            timeVariation = Mathf.Abs(Mathf.Sin(Time.time * 1.5f)) * 0.45f + 0.55f;
            transform.Rotate(Vector3.up, rotationSpeed/10 * timeVariation);
            transform.position = originalPos + transform.up * verticalAmplitude/100 * Mathf.Sin(Time.time * verticalSpeed);
        }
    }

    /// <summary>
    /// Chama o efeito após coletar e passa infos para outros códigos.
    /// Chamar código original no fim do override.
    /// </summary>
    protected virtual void Collect()
    {
        Collected = true;
        LevelManager.Instance?.SaveMe(this);
        CollectEffect();
        CollectInfo();
    }

    protected virtual void CollectEffect()
    {
        collectible.SetActive(false);
    }

    /// <summary>
    /// Passa para outros códigos a informação de que o coletavel foi pego.
    /// </summary>
    protected virtual void CollectInfo()
    {
        
    }

    public override Save Save()
    {
        //Debug.Log(saveData);
        if(saveData == null) new Save(Types.Collectible,id);
        saveData.Add(new KeyValue{key = "Collected",value = Collected?1:0});
        return saveData;
    }

    public override void Load(List<Save> save)
    {
        saveData = GetSave(save,Types.Collectible,id);
        if(saveData == null) 
            return;
        int b = saveData.Get("Collected");
        if(b == 1) {
            Collected = true;
            CollectEffect();
        }
    }
    public void OnDestroy()
    {
        LevelManager.Instance?.Remove(this);
    }
}
