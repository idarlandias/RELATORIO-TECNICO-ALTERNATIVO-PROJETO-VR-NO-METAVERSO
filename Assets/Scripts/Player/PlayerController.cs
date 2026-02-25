using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controlador de jogador em primeira pessoa para PC.
/// Usa WASD para mover e o mouse para olhar ao redor.
/// Requer: CharacterController no mesmo GameObject.
///          A câmera deve ser filho deste objeto (Camera Rig).
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // ─── Configurações de Movimento ─────────────────────────────────────────
    [Header("Movimento")]
    [Tooltip("Velocidade de deslocamento em metros por segundo")]
    [SerializeField] private float velocidade = 5f;

    [Tooltip("Força da gravidade aplicada ao jogador")]
    [SerializeField] private float gravidade = -9.81f;

    // ─── Configurações de Visão ──────────────────────────────────────────────
    [Header("Visão (Mouse)")]
    [Tooltip("Sensibilidade do mouse. Valores menores = mais suave")]
    [SerializeField] private float sensibilidade = 0.15f;

    [Tooltip("Arraste aqui o Transform da câmera (filho deste objeto)")]
    [SerializeField] private Transform cameraRig;

    // ─── Variáveis Internas ──────────────────────────────────────────────────
    private CharacterController _cc;
    private Vector3 _velocidadeVertical;
    private float _rotacaoX;           // Rotação acumulada no eixo X (cima/baixo)
    private bool _cursorTravado = true;

    // ────────────────────────────────────────────────────────────────────────
    #region Unity Callbacks

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    private void Start()
    {
        TravarCursor(true);
    }

    private void Update()
    {
        GerenciarCursor();
        Mover();
        Olhar();
        AplicarGravidade();
    }

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region Movimento

    /// <summary>
    /// Lê entrada do teclado (WASD / setas) e move o CharacterController.
    /// </summary>
    private void Mover()
    {
        if (!_cursorTravado) return;

        Vector2 input = LerTeclado();
        Vector3 direcao = transform.right * input.x + transform.forward * input.y;

        // Normaliza para evitar velocidade maior na diagonal
        if (direcao.magnitude > 1f)
            direcao.Normalize();

        _cc.Move(direcao * velocidade * Time.deltaTime);
    }

    /// <summary>
    /// Retorna o vetor 2D de entrada do teclado usando o New Input System.
    /// </summary>
    private Vector2 LerTeclado()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current == null) return input;

        // Frente / Trás
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            input.y += 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            input.y -= 1f;

        // Esquerda / Direita
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            input.x -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            input.x += 1f;

        return input;
    }

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region Visão

    /// <summary>
    /// Rotaciona o corpo do jogador horizontalmente e a câmera verticalmente.
    /// </summary>
    private void Olhar()
    {
        if (!_cursorTravado || Mouse.current == null) return;

        Vector2 delta = Mouse.current.delta.ReadValue() * sensibilidade;

        // Rotação vertical da câmera (limitada para não virar de cabeça para baixo)
        _rotacaoX -= delta.y;
        _rotacaoX = Mathf.Clamp(_rotacaoX, -80f, 80f);

        if (cameraRig != null)
            cameraRig.localRotation = Quaternion.Euler(_rotacaoX, 0f, 0f);

        // Rotação horizontal do corpo do jogador
        transform.Rotate(Vector3.up * delta.x);
    }

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region Gravidade

    /// <summary>
    /// Aplica gravidade constante para manter o jogador no chão.
    /// </summary>
    private void AplicarGravidade()
    {
        // Pequeno valor negativo para manter o grounded estável
        if (_cc.isGrounded && _velocidadeVertical.y < 0f)
            _velocidadeVertical.y = -2f;

        _velocidadeVertical.y += gravidade * Time.deltaTime;
        _cc.Move(_velocidadeVertical * Time.deltaTime);
    }

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region Cursor

    /// <summary>
    /// ESC libera o cursor; clicar na janela o trava novamente.
    /// </summary>
    private void GerenciarCursor()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            TravarCursor(false);

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && !_cursorTravado)
            TravarCursor(true);
    }

    private void TravarCursor(bool travar)
    {
        _cursorTravado = travar;
        Cursor.lockState = travar ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !travar;
    }

    #endregion
}
