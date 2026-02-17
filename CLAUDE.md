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
    │   ├── Scenes/        # MainMenu, RoleSelection, BaseScene
    │   ├── Scripts/       # Managers, UI, Data, Multiplayer, Utilities
    │   │   └── Editor/    # CreateAllGameAssets.cs (crea ScriptableObjects)
    │   ├── Data/          # ScriptableObjects (Config, Professions, Events)
    │   └── PreFabs/       # UI Prefabs (RoleCard, etc.)
    ├── Packages/
    └── ProjectSettings/
```

## IMPORTANTE - Configuración Unity
- Unity Hub debe apuntar a `XipeLabsSimulador/unity/` (NO a la raíz)
- La carpeta `Assets/` raíz fue eliminada para evitar clases duplicadas
- Si Unity muestra errores de compilación, verificar que no haya scripts duplicados fuera de unity/

## Stack Tecnológico
- **Motor**: Unity 2022.3 LTS | C# (.NET 4.7.1)
- **Backend**: Azure Functions (api/Xipe.Api/)
- **Infra**: Bicep (infra/)
- **Multiplayer futuro**: Photon PUN2
- **Plataforma**: Mobile (Android/iOS)

## Arquitectura
- **Patrones**: Singleton (GameManager, SaveManager), Observer (UnityEvents), ScriptableObject (data-driven), State Machine (GamePhase)
- **Managers**: GameManager, TurnManager, EconomyManager, EventManager, UIManager, SaveManager
- **UI Controllers**: MainMenuController, RoleSelectionController, RoleCard, DashboardPanel, EventPopup, FinancialChart, PlayerStatusPanel, TurnIndicator, GameSummaryController
- **Data**: ProfessionData, FinancialEventData, GameConfigData, PlayerState, GameState
- **Multiplayer (placeholders)**: NetworkManager, TurnSynchronizer
- **Nota técnica**: Los ScriptableObjects usan campos privados con [SerializeField], requieren reflection para asignar por código

## Estado Actual (Febrero 2026)

### Completado
- [x] Merge de dev a master - estructura monorepo adoptada
- [x] 30+ scripts C# (Managers, UI, Data, Utilities, Multiplayer placeholders)
- [x] 3 escenas Unity: MainMenu, RoleSelection, BaseScene (GameBoard)
- [x] ScriptableObjects existentes: 2 profesiones (Programmer, Doctor) y 3 eventos (CarRepair, BirthdayParty, SurpriseBonus)
- [x] GameConfig.asset configurado
- [x] Prefab RoleCard
- [x] CI/CD workflows, API backend, Infra
- [x] README.md con documentación completa
- [x] Carpeta Assets/ raíz eliminada (evitar duplicados)
- [x] Script CreateAllGameAssets.cs listo en unity/Assets/Scripts/Editor/

### Completado - Fase 1 (Completar Assets)
- [x] Ejecutar en Unity: Tools > Finance Game > Create All Game Assets
  - 10 profesiones creadas
  - 20 eventos financieros creados
  - GameConfig actualizado con todas las referencias
- [x] Crear escena GameSummary (con GameSummaryController, PlayerRankItem prefab, Build Settings)

### Completado - Fase 2 (Configurar Escenas y Flujo)
- [x] SetupAllScenes.cs - Genera escena GameBoard con Canvas, TopBar, Dashboard, PlayerStatus, ActionArea, EventPopup, todos los Managers wired
- [x] GameBoardController.cs (NUEVO) - Conecta botón Next Turn a TurnManager.StartNewTurn()
- [x] TurnManager cambiado a flujo manual (botón Next Turn, ya no auto-start)
  - Start() ya no llama StartNewTurn(), solo dispara OnTurnReady
  - Nuevo UnityEvent OnTurnReady para habilitar/deshabilitar botón
  - ProcessPlayerTurn espera en fases PlayerDecision Y RandomEvent
- [x] EventManager corregido:
  - GetAvailableEvents usa displayTurn = currentTurn + 1 (1-based vs 0-based)
  - TriggerEvent siempre pone fase PlayerDecision (espera popup close)
  - ResolveCurrentEvent() nuevo método público (popup lo llama al cerrar)
- [x] EventPopup.OnCloseClicked() ahora llama ResolveCurrentEvent() antes de Hide()
- [x] EventPopup parent se mantiene activo (sin SetActive(false)), popupPanel hijo controla visibilidad
- [x] FixRoleSelectionScene.cs - Fix scroll: ContentSizeFitter, GridLayoutGroup 2 columnas 450x280
- [x] FixRoleCardPrefab.cs - Fix layout: nombre arriba, icono centro (110x110), salario visible, descripción abajo
- [x] Build Settings configurado: MainMenu(0), RoleSelection(1), GameBoard(2), GameSummary(3)

### En Progreso - Testing y Pulido MVP 0.1
- [x] Flujo MainMenu → RoleSelection funciona
- [x] RoleSelection → GameBoard funciona (profesión seleccionada, confirm, carga GameBoard)
- [x] Botón Next Turn funciona y dispara eventos
- [x] EventPopup muestra eventos y se cierra correctamente
- [x] Scroll de profesiones funciona con 10 cards
- [x] RoleCard layout corregido (nombre arriba, salario visible)
- [ ] Testing flujo completo GameBoard → GameSummary (12 turnos)
- [ ] Verificar que GameSummary muestra ranking correctamente
- [ ] Pulir UI visual (colores, espaciado, responsividad)

### Pendiente - Fase 3 (Versión 0.2 - Multijugador)
- [ ] Integración Photon PUN2
- [ ] Lobby para 2-4 jugadores
- [ ] Sincronización de turnos
- [ ] Matchmaking con PlayFab

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
- `unity/Assets/Scripts/Editor/FixRoleSelectionScene.cs` - Fix scroll RoleSelection
- `unity/Assets/Scripts/Editor/FixRoleCardPrefab.cs` - Fix layout RoleCard prefab
- `unity/Assets/Scripts/Managers/` - GameManager, TurnManager, EconomyManager, EventManager, UIManager, SaveManager
- `unity/Assets/Scripts/UI/GameBoard/GameBoardController.cs` - Conecta botón Next Turn
- `unity/Assets/Scripts/UI/GameBoard/EventPopup.cs` - Popup de eventos (espera interacción del jugador)
- `unity/Assets/Scripts/UI/` - Controllers de interfaz por escena
- `unity/Assets/Scripts/Data/ScriptableObjects/` - ProfessionData, FinancialEventData, GameConfigData
- `unity/Assets/Data/` - Assets ScriptableObject instanciados
- `unity/Assets/Scenes/` - MainMenu, RoleSelection, GameBoard, GameSummary
- `unity/Assets/PreFabs/UI/` - RoleCard.prefab, PlayerRankItem.prefab

## Convenciones
- Commits: feat/fix/docs/style/refactor/test/chore
- Idioma del código: Inglés
- Idioma de comunicación: Español
- Co-author en commits de Claude
