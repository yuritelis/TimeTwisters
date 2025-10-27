using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCDialogo", menuName = "NPC Dialogo")]
public class NPCDialogo : ScriptableObject
{
    [Header("> Foto e Nome <")]
    public string nomeNpc;
    public Sprite npcPortrait;

    [Header("> Som e Fala <")]
    public AudioClip somVoz;
    public float volFala = 1f;
    public float autoProgressDelay = 1.5f;
    public float velFala = 0.05f;

    [Header("> Arrays de Diálogo <")]
    public string[] linhasDialogo;
    public bool[] autoProgressLine;

}
