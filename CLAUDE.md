# Money Matters - Contexto del Proyecto

## Descripción
Juego de mesa digital educativo para enseñar finanzas personales. Los jugadores eligen profesiones con perfiles financieros realistas y toman decisiones económicas durante 12 turnos (1 año simulado).

## Estructura Monorepo
```
XipeLabsSimulador/
├── CLAUDE.md              # Este archivo
├── .github/workflows/     # CI/CD pipelines
├── api/Xipe.Api/          # Azure Functions backend
├── infra/main.bicep       # Infrastructure as Code
└── unity/                 # Proyecto Unity 2022.3 LTS
    ├── Assets/
    │   ├── Scenes/        # MainMenu, RoleSelection, BaseScene
    │   ├── Scripts/       # Managers, UI, Data, Multiplayer, Utilities
    │   ├── Data/          # ScriptableObjects (Config, Professions, Events)
    │   └── PreFabs/       # UI Prefabs (RoleCard, etc.)
    ├── Packages/
    └── ProjectSettings/
```

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

## Estado Actual (Febrero 2026)

### Completado
- [x] Merge de dev a master - estructura monorepo adoptada
- [x] 30+ scripts C# (Managers, UI, Data, Utilities, Multiplayer placeholders)
- [x] 3 escenas Unity: MainMenu, RoleSelection, BaseScene (GameBoard)
- [x] ScriptableObjects: 2 profesiones (Programmer, Doctor) y 3 eventos (CarRepair, BirthdayParty, SurpriseBonus)
- [x] GameConfig.asset configurado
- [x] Prefab RoleCard
- [x] CI/CD workflows, API backend, Infra
- [x] README.md con documentación completa

### Pendiente - Fase 1 (Completar Assets)
- [ ] Crear 8 profesiones faltantes como ScriptableObjects
- [ ] Crear 17 eventos faltantes como ScriptableObjects
- [ ] Crear escena GameSummary

### Pendiente - Fase 2 (Completar MVP 0.1)
- [ ] Configurar UI completa en cada escena
- [ ] Conectar referencias en Inspector (Managers <-> UI)
- [ ] Configurar Build Settings (orden de escenas)
- [ ] Testing flujo: MainMenu -> RoleSelection -> GameBoard -> GameSummary

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
Barrendero $2,200 | Cajero $2,500 | Chef $3,800 | Maestro $4,200 | Policía $4,800 | Ingeniero $7,500 | Programador $8,200 | Piloto $9,500 | Abogado $10,500 | Doctor $12,000

### 20 Eventos Financieros
- 6 gastos simples + 1 emergencia médica (con modificador de seguro)
- 6 ingresos extra
- 7 eventos con decisiones múltiples

### Configuración del Juego
- Turnos totales: 12 (1 año)
- Dinero inicial: $5,000
- Probabilidad de evento por turno: 70%

## Archivos Clave
- `unity/Assets/Scripts/Managers/` - Controladores del juego
- `unity/Assets/Scripts/UI/` - Controllers de interfaz
- `unity/Assets/Scripts/Data/ScriptableObjects/` - Definiciones de datos
- `unity/Assets/Data/` - Assets ScriptableObject instanciados
- `unity/Assets/Scenes/` - Escenas del juego
- `unity/Assets/SetupHelper.cs` - Helper para configurar GameManager
- `unity/Assets/QuickAssetCreator.cs` - Creador rápido de assets

## Convenciones
- Commits: feat/fix/docs/style/refactor/test/chore
- Idioma del código: Inglés
- Idioma de comunicación: Español
- Co-author en commits de Claude
