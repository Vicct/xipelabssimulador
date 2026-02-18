# Money Matters - Contexto del Proyecto

## Descripción
Juego de mesa digital educativo para enseñar finanzas personales. Los jugadores eligen profesiones con perfiles financieros realistas y toman decisiones económicas durante 12 turnos (1 año simulado).

## Estructura Monorepo
```
XipeLabsSimulador/
├── CLAUDE.md              # Este archivo
├── README.md
├── .github/workflows/     # CI/CD pipelines
├── api/Xipe.Api/          # Azure Functions backend
├── infra/main.bicep       # Infrastructure as Code
└── unity/                 # Proyecto Unity 2022.3 LTS ← UNITY HUB APUNTA AQUÍ
    ├── Assets/
    │   ├── Scenes/        # MainMenu, RoleSelection, GameBoard, GameSummary, Lobby
    │   ├── Scripts/       # Managers, UI, Data, Multiplayer, Utilities
    │   │   ├── Editor/    # CreateAllGameAssets.cs, CreateLobbyScene.cs, etc.
    │   │   ├── Multiplayer/ # PhotonManager, NetworkTurnManager, PlayFabManager
    │   │   └── UI/Lobby/  # LobbyController, RoomListItem, PlayerListItem
    │   ├── Data/          # ScriptableObjects (Config, Professions, Events)
    │   └── PreFabs/       # UI Prefabs (RoleCard, RoomListItem, PlayerListItem, etc.)
    ├── Packages/
    └── ProjectSettings/
```

## IMPORTANTE - Configuración Unity
- Unity Hub debe apuntar a `XipeLabsSimulador/unity/` (NO a la raíz)
- La carpeta `Assets/` raíz fue eliminada para evitar clases duplicadas
- Si Unity muestra errores de compilación, verificar que no haya scripts duplicados fuera de unity/
- **Photon PUN2** y **PlayFab SDK** deben importarse manualmente en Unity (ver Sub-Fase 0 abajo)

## Stack Tecnológico
- **Motor**: Unity 2022.3 LTS | C# (.NET 4.7.1)
- **Backend**: Azure Functions (api/Xipe.Api/)
- **Infra**: Bicep (infra/)
- **Multiplayer**: Photon PUN2 (networking) + PlayFab (matchmaking)
- **Plataforma**: Mobile (Android/iOS)

## Arquitectura
- **Patrones**: Singleton (GameManager, SaveManager, PhotonManager, PlayFabManager), Observer (UnityEvents), ScriptableObject (data-driven), State Machine (GamePhase), Master Client (Photon PUN2)
- **Managers**: GameManager, TurnManager, EconomyManager, EventManager, UIManager, SaveManager
- **Multiplayer Managers**: PhotonManager (MonoBehaviourPunCallbacks), NetworkTurnManager (MonoBehaviourPun + RPCs), PlayFabManager
- **UI Controllers**: MainMenuController, LobbyController, RoleSelectionController, RoleCard, DashboardPanel, EventPopup, FinancialChart, PlayerStatusPanel, TurnIndicator, GameBoardController, GameSummaryController, DisconnectDialog
- **Data**: ProfessionData, FinancialEventData, GameConfigData, PlayerState, GameState
- **Nota técnica**: Los ScriptableObjects usan campos privados con [SerializeField], requieren reflection para asignar por código

## Arquitectura Multiplayer
```
Master Client (Host)                    Other Clients
├─ Genera eventos aleatorios ──RPC──→   Reciben evento específico
├─ Controla flujo de turnos ──RPC──→    Ven el mismo turno
├─ Avanza fases del juego ───RPC──→     UI se actualiza
└─ Carga escenas ──AutoSync──→          Misma escena automáticamente
```
- **PhotonManager**: Conexión, lobby, salas, custom properties. Reemplaza el antiguo NetworkManager stub.
- **NetworkTurnManager**: RPCs para sincronizar turnos, eventos, dinero, fin de juego.
- **PlayFabManager**: Auth con device ID, matchmaking con polling.
- **Flujo multiplayer**: MainMenu → Photon Connect → Lobby → RoleSelection → GameBoard → GameSummary
- **Flujo solo**: MainMenu → RoleSelection → GameBoard → GameSummary (sin cambios)

## Estado Actual (Febrero 2026)

### Completado - MVP 0.1 (Solo Mode)
- [x] 30+ scripts C#, 4 escenas, 10 profesiones, 20 eventos
- [x] Flujo completo: MainMenu → RoleSelection → GameBoard → GameSummary
- [x] Turnos manuales (botón Next Turn), eventos con opciones
- [x] EventPopup mejorado (3+ opciones visibles, impacto financiero)
- [x] GameSummary con ranking, profesión, earned/spent
- [x] Editor scripts para setup automatizado de escenas

### Completado - Fase 3 (Versión 0.2 - Multijugador) - CÓDIGO
- [x] PhotonManager.cs - Conexión Photon PUN2, lobby, salas, custom properties
- [x] NetworkTurnManager.cs - RPCs: turnos, eventos, dinero, fin de juego
- [x] PlayFabManager.cs - Auth device ID, matchmaking con queue polling
- [x] LobbyController.cs + RoomListItem.cs + PlayerListItem.cs - UI del lobby
- [x] CreateLobbyScene.cs - Editor script para generar escena Lobby + prefabs
- [x] DisconnectDialog.cs - Popup de desconexión mid-game
- [x] GameManager modificado: PrepareMultiplayerGame(), StartMultiplayerGame(), disconnect handling
- [x] TurnManager modificado: RequestNetworkTurn(), skip jugadores desconectados, money sync
- [x] EventManager modificado: MasterGenerateAndBroadcastEvent(), TriggerSpecificEvent()
- [x] MainMenuController modificado: Photon connect → Lobby scene
- [x] RoleSelectionController modificado: sync profesión via Custom Properties
- [x] GameBoardController modificado: Master Client controls Next Turn
- [x] GameState: GetPlayerIndexById() helper
- [x] PlayerState: isDisconnected field
- [x] Eliminados stubs: NetworkManager.cs, TurnSynchronizer.cs

### Pendiente - Fase 3 (Configuración manual requerida)
- [ ] **Sub-Fase 0**: Importar Photon PUN2 Free en Unity (Asset Store)
- [ ] **Sub-Fase 0**: Configurar Photon App ID (dashboard.photonengine.com)
- [ ] **Sub-Fase 0**: Importar PlayFab SDK (.unitypackage)
- [ ] **Sub-Fase 0**: Configurar PlayFab Title ID
- [ ] Ejecutar: Tools > Finance Game > Create Lobby Scene
- [ ] Agregar PhotonManager y NetworkTurnManager al GameObject de GameManager
- [ ] Agregar PhotonView al GameObject de GameManager
- [ ] Agregar PlayFabManager al GameObject de GameManager
- [ ] Probar lobby con 2 instancias
- [ ] Crear queue "MoneyMattersQueue" en PlayFab Game Manager
- [ ] Probar matchmaking PlayFab

### Pendiente - Fase 4 (Versión 1.0 - Lanzamiento)
- [ ] Tutorial interactivo
- [ ] Música, SFX, animaciones UI
- [ ] PlayFab Leaderboards y achievements
- [ ] 20+ profesiones adicionales, 40+ eventos nuevos
- [ ] Soporte multi-idioma

## Datos de Referencia Rápida

### 10 Profesiones (salario/mes)
| Profesión | Salario | Tier | Seguro | Educación | Impuestos | Crecimiento |
|-----------|---------|------|--------|-----------|-----------|-------------|
| Street Sweeper | $2,200 | Low | No | $0 | 10% | 1% |
| Cashier | $2,500 | Low | No | $0 | 12% | 2% |
| Chef | $3,800 | Medium | No | $25k | 15% | 4% |
| Teacher | $4,200 | Medium | Sí | $40k | 16% | 2.5% |
| Police Officer | $4,800 | Medium | Sí | $15k | 18% | 3% |
| Engineer | $7,500 | High | Sí | $80k | 22% | 5% |
| Programmer | $8,200 | High | Sí | $50k | 20% | 6% |
| Pilot | $9,500 | High | Sí | $100k | 24% | 3.5% |
| Lawyer | $10,500 | High | Sí | $150k | 26% | 4.5% |
| Doctor | $12,000 | High | Sí | $200k | 28% | 4% |

### 20 Eventos Financieros
**Gastos (6):** Car Repair -$1,200 | Parking Ticket -$150 | Home Appliance -$800 | Pet Emergency -$900 | Speeding Ticket -$300 | Phone Replacement -$800
**Emergencia (1):** Medical Emergency -$3,500 (afectada por seguro médico, 50% descuento)
**Ingresos (6):** Tax Refund +$1,800 | Surprise Bonus +$2,500 | Found Money +$200 | Insurance Claim +$2,200 | Freelance Project +$2,800 | Side Hustle +$1,500
**Decisiones (7):** Birthday Party | Wedding Invitation | Stock Investment | Gym Membership | Holiday Shopping | Computer Upgrade | Charity Donation

### Configuración del Juego
- Turnos totales: 12 (1 año)
- Dinero inicial: $5,000
- Probabilidad de evento por turno: 70%

## Archivos Clave
- `unity/Assets/Scripts/Editor/CreateAllGameAssets.cs` - Crea TODOS los ScriptableObjects (menú Tools)
- `unity/Assets/Scripts/Editor/SetupAllScenes.cs` - Genera escena GameBoard completa con UI y Managers
- `unity/Assets/Scripts/Editor/CreateGameSummaryScene.cs` - Genera escena GameSummary
- `unity/Assets/Scripts/Editor/CreateLobbyScene.cs` - Genera escena Lobby con UI + prefabs
- `unity/Assets/Scripts/Editor/FixRoleSelectionScene.cs` - Fix scroll RoleSelection
- `unity/Assets/Scripts/Editor/FixRoleCardPrefab.cs` - Fix layout RoleCard prefab
- `unity/Assets/Scripts/Editor/FixEventPopup.cs` - Fix EventPopup layout (3+ opciones)
- `unity/Assets/Scripts/Managers/` - GameManager, TurnManager, EconomyManager, EventManager, UIManager, SaveManager
- `unity/Assets/Scripts/Multiplayer/` - PhotonManager, NetworkTurnManager, PlayFabManager
- `unity/Assets/Scripts/UI/Lobby/` - LobbyController, RoomListItem, PlayerListItem
- `unity/Assets/Scripts/UI/Common/` - DisconnectDialog
- `unity/Assets/Scripts/UI/GameBoard/` - GameBoardController, EventPopup
- `unity/Assets/Scripts/Data/ScriptableObjects/` - ProfessionData, FinancialEventData, GameConfigData
- `unity/Assets/Data/` - Assets ScriptableObject instanciados
- `unity/Assets/Scenes/` - MainMenu, RoleSelection, GameBoard, GameSummary, Lobby
- `unity/Assets/PreFabs/UI/` - RoleCard, PlayerRankItem, ChoiceButton, RoomListItem, PlayerListItem

## Convenciones
- Commits: feat/fix/docs/style/refactor/test/chore
- Idioma del código: Inglés
- Idioma de comunicación: Español
- Co-author en commits de Claude
