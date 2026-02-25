using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Script de Editor para montar automaticamente a cena do projeto VR.
/// Como usar: no menu do Unity, clique em  VR Projeto > Montar Cena Completa
/// </summary>
public static class SceneSetup
{
    private const string MATERIAIS_PATH = "Assets/Materials";

    // ─── Menu ────────────────────────────────────────────────────────────────

    [MenuItem("VR Projeto/Montar Cena Completa")]
    public static void MontarCena()
    {
        bool confirmar = EditorUtility.DisplayDialog(
            "Montar Cena VR",
            "Isso vai limpar a cena atual e criar o ambiente completo.\n\nDeseja continuar?",
            "Sim, montar!", "Cancelar");

        if (!confirmar) return;

        // 1. Limpa objetos existentes
        LimparCena();

        // 2. Cria materiais e salva em Assets/Materials
        Material matChao     = CriarMaterial("Mat_Chao",     new Color(0.20f, 0.50f, 0.10f)); // grama
        Material matCasa     = CriarMaterial("Mat_Casa",     new Color(0.85f, 0.75f, 0.60f)); // bege
        Material matTelhado  = CriarMaterial("Mat_Telhado",  new Color(0.60f, 0.20f, 0.10f)); // terracota
        Material matTronco   = CriarMaterial("Mat_Tronco",   new Color(0.40f, 0.25f, 0.10f)); // marrom
        Material matFolhagem = CriarMaterial("Mat_Folhagem", new Color(0.10f, 0.40f, 0.10f)); // verde escuro
        Material matBanco    = CriarMaterial("Mat_Banco",    new Color(0.55f, 0.35f, 0.15f)); // madeira
        Material matPoste    = CriarMaterial("Mat_Poste",    new Color(0.30f, 0.30f, 0.35f)); // cinza metal
        Material matLuz      = CriarMaterial("Mat_Luz",      new Color(1.00f, 0.95f, 0.70f)); // amarelo claro
        AssetDatabase.SaveAssets();

        // 3. Pastas organizadoras na hierarquia
        GameObject goAmbiente   = CriarPasta("[ Ambiente ]");
        GameObject goIluminacao = CriarPasta("[ Iluminacao ]");
        GameObject goPlayer     = CriarPasta("[ Player ]");

        // 4. Objetos do ambiente
        CriarChao    (goAmbiente, matChao);
        CriarCasa    (goAmbiente, new Vector3( 0, 0,  10), matCasa, matTelhado);
        CriarArvore  (goAmbiente, "Arvore_01", new Vector3(-6, 0,  4), matTronco, matFolhagem);
        CriarArvore  (goAmbiente, "Arvore_02", new Vector3( 6, 0,  5), matTronco, matFolhagem);
        CriarArvore  (goAmbiente, "Arvore_03", new Vector3(-5, 0, -4), matTronco, matFolhagem);
        CriarBanco   (goAmbiente, "Banco_01",  new Vector3(-3, 0, -1), matBanco);
        CriarBanco   (goAmbiente, "Banco_02",  new Vector3( 3, 0, -1), matBanco, 180f);
        CriarPoste   (goAmbiente, "Poste_01",  new Vector3( 0, 0,  2), matPoste, matLuz);
        CriarPoste   (goAmbiente, "Poste_02",  new Vector3(-8, 0,  6), matPoste, matLuz);

        // 5. Iluminação
        CriarSol(goIluminacao);

        // 6. Skybox e ambiente
        ConfigurarAmbiente();

        // 7. Player (câmera + controlador)
        CriarPlayer(goPlayer);

        // 8. Salva a cena
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("[VR Projeto] Cena montada com sucesso! Pressione Play para testar com WASD + Mouse.");
    }

    // ─── Objetos da Cena ─────────────────────────────────────────────────────

    static void CriarChao(GameObject pai, Material mat)
    {
        GameObject chao = GameObject.CreatePrimitive(PrimitiveType.Plane);
        chao.name = "Chao";
        chao.transform.SetParent(pai.transform);
        chao.transform.localPosition = Vector3.zero;
        chao.transform.localScale = new Vector3(5f, 1f, 5f); // 50 x 50 metros
        chao.GetComponent<Renderer>().sharedMaterial = mat;
    }

    static void CriarCasa(GameObject pai, Vector3 posicao, Material matParede, Material matTelhado)
    {
        GameObject casa = new GameObject("Casa");
        casa.transform.SetParent(pai.transform);
        casa.transform.localPosition = posicao;

        // Paredes
        GameObject paredes = GameObject.CreatePrimitive(PrimitiveType.Cube);
        paredes.name = "Paredes";
        paredes.transform.SetParent(casa.transform);
        paredes.transform.localPosition = new Vector3(0f, 1.5f, 0f);
        paredes.transform.localScale = new Vector3(6f, 3f, 6f);
        paredes.GetComponent<Renderer>().sharedMaterial = matParede;

        // Telhado (cubo achatado e rotacionado)
        GameObject telhado = GameObject.CreatePrimitive(PrimitiveType.Cube);
        telhado.name = "Telhado";
        telhado.transform.SetParent(casa.transform);
        telhado.transform.localPosition = new Vector3(0f, 3.6f, 0f);
        telhado.transform.localScale = new Vector3(7f, 1.5f, 7f);
        telhado.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
        telhado.GetComponent<Renderer>().sharedMaterial = matTelhado;
    }

    static void CriarArvore(GameObject pai, string nome, Vector3 posicao,
                             Material matTronco, Material matFolhagem)
    {
        GameObject arvore = new GameObject(nome);
        arvore.transform.SetParent(pai.transform);
        arvore.transform.localPosition = posicao;

        // Tronco
        GameObject tronco = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        tronco.name = "Tronco";
        tronco.transform.SetParent(arvore.transform);
        tronco.transform.localPosition = new Vector3(0f, 1f, 0f);
        tronco.transform.localScale = new Vector3(0.3f, 1f, 0.3f);
        tronco.GetComponent<Renderer>().sharedMaterial = matTronco;

        // Folhagem
        GameObject folhagem = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        folhagem.name = "Folhagem";
        folhagem.transform.SetParent(arvore.transform);
        folhagem.transform.localPosition = new Vector3(0f, 3f, 0f);
        folhagem.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        folhagem.GetComponent<Renderer>().sharedMaterial = matFolhagem;
    }

    static void CriarBanco(GameObject pai, string nome, Vector3 posicao,
                            Material mat, float rotY = 0f)
    {
        GameObject banco = new GameObject(nome);
        banco.transform.SetParent(pai.transform);
        banco.transform.localPosition = posicao;
        banco.transform.localRotation = Quaternion.Euler(0f, rotY, 0f);

        // Assento
        GameObject assento = GameObject.CreatePrimitive(PrimitiveType.Cube);
        assento.name = "Assento";
        assento.transform.SetParent(banco.transform);
        assento.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        assento.transform.localScale = new Vector3(2f, 0.12f, 0.6f);
        assento.GetComponent<Renderer>().sharedMaterial = mat;

        // Encosto
        GameObject encosto = GameObject.CreatePrimitive(PrimitiveType.Cube);
        encosto.name = "Encosto";
        encosto.transform.SetParent(banco.transform);
        encosto.transform.localPosition = new Vector3(0f, 0.85f, -0.25f);
        encosto.transform.localScale = new Vector3(2f, 0.6f, 0.1f);
        encosto.GetComponent<Renderer>().sharedMaterial = mat;

        // Pernas
        float[] posX = { -0.85f, 0.85f };
        string[] nomes = { "Perna_E", "Perna_D" };
        for (int i = 0; i < 2; i++)
        {
            GameObject perna = GameObject.CreatePrimitive(PrimitiveType.Cube);
            perna.name = nomes[i];
            perna.transform.SetParent(banco.transform);
            perna.transform.localPosition = new Vector3(posX[i], 0.25f, 0f);
            perna.transform.localScale = new Vector3(0.1f, 0.5f, 0.55f);
            perna.GetComponent<Renderer>().sharedMaterial = mat;
        }
    }

    static void CriarPoste(GameObject pai, string nome, Vector3 posicao,
                            Material matPoste, Material matLuz)
    {
        GameObject poste = new GameObject(nome);
        poste.transform.SetParent(pai.transform);
        poste.transform.localPosition = posicao;

        // Haste
        GameObject haste = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        haste.name = "Haste";
        haste.transform.SetParent(poste.transform);
        haste.transform.localPosition = new Vector3(0f, 2f, 0f);
        haste.transform.localScale = new Vector3(0.1f, 2f, 0.1f);
        haste.GetComponent<Renderer>().sharedMaterial = matPoste;

        // Luminária (esfera)
        GameObject luminaria = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        luminaria.name = "Luminaria";
        luminaria.transform.SetParent(poste.transform);
        luminaria.transform.localPosition = new Vector3(0f, 4.1f, 0f);
        luminaria.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        luminaria.GetComponent<Renderer>().sharedMaterial = matLuz;

        // Luz pontual
        GameObject luzGO = new GameObject("Luz_Ponto");
        luzGO.transform.SetParent(poste.transform);
        luzGO.transform.localPosition = new Vector3(0f, 4f, 0f);
        Light luz = luzGO.AddComponent<Light>();
        luz.type = LightType.Point;
        luz.range = 10f;
        luz.intensity = 2f;
        luz.color = new Color(1f, 0.95f, 0.7f);
        luz.shadows = LightShadows.Soft;
    }

    // ─── Iluminação ──────────────────────────────────────────────────────────

    static void CriarSol(GameObject pai)
    {
        GameObject sol = new GameObject("Sol");
        sol.transform.SetParent(pai.transform);
        sol.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        Light luz = sol.AddComponent<Light>();
        luz.type = LightType.Directional;
        luz.intensity = 1.2f;
        luz.color = new Color(1f, 0.95f, 0.85f);
        luz.shadows = LightShadows.Soft;
    }

    static void ConfigurarAmbiente()
    {
        // Skybox procedural padrão (céu azul com nuvens)
        RenderSettings.ambientMode = AmbientMode.Skybox;
        RenderSettings.ambientIntensity = 1f;

        // Nevoeiro leve para dar profundidade ao ambiente
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.7f, 0.85f, 1f);
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 35f;
        RenderSettings.fogEndDistance = 90f;
    }

    // ─── Player ──────────────────────────────────────────────────────────────

    static void CriarPlayer(GameObject pasta)
    {
        // Raiz do Player
        GameObject player = new GameObject("Player");
        player.transform.SetParent(pasta.transform);
        player.transform.position = new Vector3(0f, 0.9f, -6f);

        // CharacterController
        CharacterController cc = player.AddComponent<CharacterController>();
        cc.height = 1.8f;
        cc.radius = 0.4f;
        cc.center = new Vector3(0f, 0.9f, 0f);

        // PlayerController (script criado em Scripts/Player)
        PlayerController pc = player.AddComponent<PlayerController>();

        // CameraRig — filho do Player
        GameObject cameraRig = new GameObject("CameraRig");
        cameraRig.transform.SetParent(player.transform);
        cameraRig.transform.localPosition = new Vector3(0f, 1.6f, 0f); // altura dos olhos

        // Main Camera — filho do CameraRig
        GameObject cameraGO = new GameObject("Main Camera");
        cameraGO.transform.SetParent(cameraRig.transform);
        cameraGO.transform.localPosition = Vector3.zero;
        Camera cam = cameraGO.AddComponent<Camera>();
        cam.tag = "MainCamera";
        cam.nearClipPlane = 0.05f;
        cameraGO.AddComponent<AudioListener>();

        // Conecta o CameraRig ao campo do PlayerController via SerializedObject
        SerializedObject so = new SerializedObject(pc);
        SerializedProperty prop = so.FindProperty("cameraRig");
        prop.objectReferenceValue = cameraRig.transform;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // ─── Utilitários ─────────────────────────────────────────────────────────

    static Material CriarMaterial(string nome, Color cor)
    {
        string caminho = $"{MATERIAIS_PATH}/{nome}.mat";

        // Reutiliza se já existir
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(caminho);
        if (mat != null)
        {
            mat.color = cor;
            return mat;
        }

        // Shader URP Lit; fallback para o padrão caso URP não esteja ativo
        Shader shader = Shader.Find("Universal Render Pipeline/Lit")
                     ?? Shader.Find("Standard");

        mat = new Material(shader) { color = cor };
        AssetDatabase.CreateAsset(mat, caminho);
        return mat;
    }

    static GameObject CriarPasta(string nome)
    {
        GameObject go = new GameObject(nome);
        go.transform.position = Vector3.zero;
        return go;
    }

    static void LimparCena()
    {
        GameObject[] todos = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject go in todos)
        {
            if (go.transform.parent == null)
                Object.DestroyImmediate(go);
        }
    }
}
