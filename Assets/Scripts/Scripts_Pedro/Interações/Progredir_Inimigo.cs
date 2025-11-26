using UnityEngine;

public class AvancaEtapaAoMorrer : MonoBehaviour
{
    public bool apenasUmaVez = true;
    private bool jaAtivado = false;

    public void ForcarAvanco()
    {
        if (apenasUmaVez && jaAtivado)
            return;

        jaAtivado = true;

        if (StoryProgressManager.instance != null)
            StoryProgressManager.instance.AvancarEtapa();
    }
}
