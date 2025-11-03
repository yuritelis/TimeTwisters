using UnityEngine;

[System.Serializable]
public class PersoInfos
{
    [Header("> Foto, Nome e Voz <")]
    public string nome;
    public Sprite portrait;
    public static AudioClip somVoz;
}

[System.Serializable]
public class Dialogo : ScriptableObject
{
    public PersoInfos personagem;

    [Header("> Falas <")]
    public string[] linhasDialogo;
    public float volFala = 1f;
    public float velFala = 0.05f;
}
