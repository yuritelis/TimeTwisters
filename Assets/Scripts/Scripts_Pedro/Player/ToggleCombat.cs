using UnityEngine;

public class ToggleCombat : MonoBehaviour, IInteractable
{
    [Header("Configuração")]
    public bool enableCombat = true;                    // ⬅️ ativa ou desativa combate
    public bool lockMovement = false;                   // ⬅️ opcional

    [Header("Diálogo opcional antes de alterar o combate")]
    public Dialogo dialogoAntes;                        // ⬅️ se quiser que abra diálogo ao interagir

    private bool jaInteragiu = false;
    private GameObject player;

    public bool CanInteract()
    {
        return !jaInteragiu; // impede múltiplas ativações
    }

    public void Interact()
    {
        if (jaInteragiu) return;

        jaInteragiu = true;

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("❌ Player não encontrado!");
            return;
        }

        // Se existe um diálogo para exibir ANTES de aplicar o efeito
        if (dialogoAntes != null)
        {
            DialogoManager.Instance.StartDialogo(dialogoAntes);
            DialogoManager.Instance.OnFalaIniciada = null;
            DialogoManager.Instance.OnFalaIniciada += HandleDialogFinish;
            return;
        }

        // Se não tem diálogo -> aplica direto
        AplicarAlteracao();
    }

    private void HandleDialogFinish(DialogoFalas fala)
    {
        // Quando o diálogo terminar, esta callback dispara automaticamente
        if (!DialogoManager.Instance.dialogoAtivoPublico)
        {
            DialogoManager.Instance.OnFalaIniciada -= HandleDialogFinish;
            AplicarAlteracao();
        }
    }

    private void AplicarAlteracao()
    {
        var controller = player.GetComponent<PlayerController>();
        var combat = player.GetComponent<Player_Combat>();

        if (controller != null && lockMovement)
            controller.canMove = false;

        if (combat != null)
            combat.enabled = enableCombat;

        Debug.Log(enableCombat
            ? "🟢 Combate foi ATIVADO por interação."
            : "🔴 Combate foi DESATIVADO por interação.");
    }
}
