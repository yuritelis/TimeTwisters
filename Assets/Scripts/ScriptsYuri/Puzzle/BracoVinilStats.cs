using UnityEngine;

public enum Encaixe { encaixeCerto, encaixe1, encaixe2, encaixe3 }
public class BracoVinilStats : MonoBehaviour
{
    public bool podeMexer;
    public bool movendo;

    public Vector2 posInicial;

    public Encaixe encaixe;

    void Start()
    {
        posInicial = transform.position;
    }

    void Update()
    {
        
    }
}
