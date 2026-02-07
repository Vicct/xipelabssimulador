# Guía de Implementación - Juego de Finanzas Personales "Money Matters"

## ✅ Estado Actual: Scripts Completados

Se han creado **todos los scripts C#** necesarios para el juego:

### Arquitectura de Datos ✅
- `ProfessionData.cs` - ScriptableObject para profesiones
- `FinancialEventData.cs` - ScriptableObject para eventos financieros
- `GameConfigData.cs` - ScriptableObject para configuración del juego
- `PlayerState.cs` - Estado del jugador con transacciones
- `GameState.cs` - Estado global del juego

### Managers ✅
- `GameManager.cs` - Controlador central del juego
- `SaveManager.cs` - Guardar/cargar partidas
- `TurnManager.cs` - Sistema de turnos
- `EconomyManager.cs` - Sistema económico (salarios, gastos)
- `EventManager.cs` - Generador de eventos aleatorios
- `UIManager.cs` - Coordinador de UI

### UI Controllers ✅
- `MainMenuController.cs` - Menú principal
- `RoleSelectionController.cs` - Selección de profesión
- `RoleCard.cs` - Tarjeta de profesión
- `DashboardPanel.cs` - Panel financiero del jugador
- `EventPopup.cs` - Popup de eventos
- `FinancialChart.cs` - Gráfico de historial financiero
- `PlayerStatusPanel.cs` - Panel de estado de jugador
- `TurnIndicator.cs` - Indicador de turno
- `GameSummaryController.cs` - Pantalla de resultados finales

### Utilities ✅
- `MoneyFormatter.cs` - Formateo de dinero
- `NetworkManager.cs` - Placeholder para multijugador (Photon futuro)
- `TurnSynchronizer.cs` - Placeholder para sincronización

### Estructura de Carpetas ✅
```
Assets/
├── Scenes/
├── Scripts/
│   ├── Data/
│   ├── Managers/
│   ├── UI/
│   ├── Multiplayer/
│   └── Utilities/
├── Data/
│   ├── Professions/
│   ├── Events/
│   └── Config/
├── Prefabs/
├── Sprites/
└── Audio/
```

---

## 📋 Próximos Pasos en Unity Editor

### Paso 1: Crear las Escenas

#### 1.1 Crear Escena MainMenu
1. En Unity: `File > New Scene`
2. Guardar como `Assets/Scenes/MainMenu.unity`
3. Agregar `Canvas` (Right-click en Hierarchy > UI > Canvas)
4. Agregar `EventSystem` (se crea automáticamente)
5. Crear UI:
   - Panel de fondo
   - Texto: "Money Matters - Financial Education Game"
   - 4 Botones:
     - "Solo Mode"
     - "Multiplayer" (puede estar disabled por ahora)
     - "Continue"
     - "Quit"
6. Crear GameObject vacío llamado "GameManager"
   - Agregar componente `GameManager`
   - Agregar componente `SaveManager`
   - **IMPORTANTE**: Asignar el `GameConfigData` ScriptableObject en el inspector (crear primero en Paso 2)
7. Al GameObject Canvas, agregar componente `MainMenuController`
   - Asignar los 4 botones en el inspector

#### 1.2 Crear Escena RoleSelection
1. `File > New Scene`
2. Guardar como `Assets/Scenes/RoleSelection.unity`
3. Agregar Canvas y EventSystem
4. Crear UI:
   - Texto header: "Choose Your Profession"
   - Panel contenedor con `Grid Layout Group` component
   - Button "Confirm Selection"
5. Crear prefab `RoleCard`:
   - Crear GameObject UI > Panel
   - Agregar: Image (icono), 3 Texts (nombre, salario, descripción), Background Image
   - Agregar componente `RoleCard` script
   - Asignar referencias en inspector
   - Guardar como prefab en `Assets/Prefabs/UI/RoleCard.prefab`
6. Al Canvas, agregar componente `RoleSelectionController`
   - Asignar: roleCardContainer, roleCardPrefab, confirmButton, instructionText
   - **IMPORTANTE**: Asignar `GameConfigData` en el inspector

#### 1.3 Crear Escena GameBoard
1. `File > New Scene`
2. Guardar como `Assets/Scenes/GameBoard.unity`
3. Crear UI layout con 3 paneles:
   - **Left Panel**: Dashboard
     - Texts: Current Money, Total Earned, Total Spent, Profession
     - Image: Profession Icon
     - Panel vacío para FinancialChart
     - Agregar componente `DashboardPanel`
   - **Center Panel**: Board visual (puede ser simple por ahora)
   - **Right Panel**: Player status (hasta 4 slots)
     - Crear 4 `PlayerStatusPanel` components
4. Crear GameObject "EventPopup" (initially disabled)
   - Panel oscuro de fondo (semitransparente)
   - Panel central con: Image (icono), 2 Texts (nombre, descripción), Text (amount), Button (close)
   - Transform contenedor para botones de opciones
   - Agregar componente `EventPopup`
5. Header con `TurnIndicator` component
6. Crear GameObject vacío "Managers":
   - Agregar componentes: TurnManager, EconomyManager, EventManager, UIManager
   - **Conectar todas las referencias en inspector**
     - TurnManager → EconomyManager, EventManager
     - EconomyManager → (sus UnityEvents se conectan en UIManager)
     - EventManager → EconomyManager
     - UIManager → DashboardPanel, EventPopup, TurnIndicator, PlayerStatusPanels, TurnManager, EconomyManager, EventManager

#### 1.4 Crear Escena GameSummary
1. `File > New Scene`
2. Guardar como `Assets/Scenes/GameSummary.unity`
3. Crear UI:
   - Header: "Game Over!"
   - Text grande: Winner name y dinero
   - Panel contenedor para ranking (Vertical Layout Group)
   - 2 Botones: "Play Again", "Main Menu"
4. Al Canvas, agregar componente `GameSummaryController`
   - Asignar referencias

#### 1.5 Configurar Build Settings
1. `File > Build Settings`
2. Agregar escenas en orden:
   1. MainMenu
   2. RoleSelection
   3. GameBoard
   4. GameSummary

---

### Paso 2: Crear ScriptableObject Assets

#### 2.1 Crear GameConfig
1. En Project: `Right-click en Assets/Data/Config`
2. `Create > Finance Game > Game Config`
3. Nombrar: "GameConfig"
4. Configurar:
   - Total Turns: 12
   - Starting Cash: 5000
   - Event Chance Per Turn: 0.7
   - (Los arrays de profesiones y eventos se llenan después)

#### 2.2 Crear 10 Profesiones
Para cada profesión, hacer:
1. `Right-click en Assets/Data/Professions > Create > Finance Game > Profession`
2. Configurar según la siguiente tabla:

| Nombre | Salario Mensual | Bono Inicial | Tier | Seguro | Educación | Tasa Impuesto | Crecimiento |
|--------|----------------|--------------|------|---------|-----------|---------------|-------------|
| Street Sweeper | 2200 | 0 | Low | No | 0 | 0.10 | 0.01 |
| Cashier | 2500 | 500 | Low | No | 0 | 0.12 | 0.02 |
| Police Officer | 4800 | 2000 | Medium | Yes | 15000 | 0.18 | 0.03 |
| Teacher | 4200 | 1000 | Medium | Yes | 40000 | 0.16 | 0.025 |
| Chef | 3800 | 500 | Medium | No | 25000 | 0.15 | 0.04 |
| Engineer | 7500 | 5000 | High | Yes | 80000 | 0.22 | 0.05 |
| Doctor | 12000 | 10000 | High | Yes | 200000 | 0.28 | 0.04 |
| Pilot | 9500 | 7000 | High | Yes | 100000 | 0.24 | 0.035 |
| Programmer | 8200 | 8000 | High | Yes | 50000 | 0.20 | 0.06 |
| Lawyer | 10500 | 5000 | High | Yes | 150000 | 0.26 | 0.045 |

**Para profesiones con seguro médico**: Medical Expense Modifier = 0.5 (50% descuento)

#### 2.3 Crear 20 Eventos Financieros
Para cada evento, crear en `Assets/Data/Events`:

**Gastos Simples:**
1. **Car Repair** - Expense, -1200, Prob 0.15
2. **Parking Ticket** - Expense, -150, Prob 0.18
3. **Home Appliance** - Expense, -800, Prob 0.14
4. **Pet Emergency** - Emergency, -900, Prob 0.10
5. **Speeding Ticket** - Expense, -300, Prob 0.12
6. **Phone Replacement** - Expense, -800, Prob 0.13

**Emergencias con Modificadores:**
7. **Medical Emergency** - Emergency, -3500, Prob 0.08, AffectedByModifiers: TRUE

**Ingresos:**
8. **Tax Refund** - Income, +1800, Prob 0.12, Min Turn 3, Can't Repeat
9. **Surprise Bonus** - Income, +2500, Prob 0.10
10. **Found Money** - Income, +200, Prob 0.05
11. **Insurance Claim** - Income, +2200, Prob 0.07, Can't Repeat
12. **Freelance Project** - Opportunity, +2800, Prob 0.09, Min Turn 3
13. **Side Hustle** - Opportunity, +1500, Prob 0.13

**Eventos con Decisiones (Has Choice = TRUE):**

14. **Birthday Party**
   - Choices:
     - "Big Party" → -800
     - "Small Gathering" → -300
     - "Skip It" → 0

15. **Wedding Invitation**
   - Choices:
     - "Attend + Gift" → -400
     - "Gift Only" → -200
     - "Decline" → 0

16. **Stock Investment**
   - Choices:
     - "Invest $2000" → -2000
     - "Invest $1000" → -1000
     - "Skip" → 0

17. **Gym Membership**
   - Choices:
     - "Annual" → -600
     - "Monthly Trial" → -60
     - "Skip" → 0

18. **Holiday Shopping**
   - Choices:
     - "Big Spender" → -1500
     - "Budget Shopper" → -700
     - "Handmade Gifts" → -200

19. **Computer Upgrade**
   - Choices:
     - "High-End PC" → -2500
     - "Mid-Range" → -1200
     - "Keep Old One" → 0

20. **Charity Donation**
   - Choices:
     - "Generous" → -500
     - "Small Donation" → -100
     - "Not Now" → 0

#### 2.4 Actualizar GameConfig
1. Abrir GameConfig asset
2. En "Available Professions": asignar las 10 profesiones (arrastrar desde Project)
3. En "Available Events": asignar los 20 eventos

---

### Paso 3: Crear Prefab GameManager
1. En MainMenu scene, seleccionar GameObject "GameManager"
2. Arrastrar a `Assets/Prefabs/Managers/`
3. Este prefab debe tener:
   - GameManager component (con GameConfig asignado)
   - SaveManager component

---

### Paso 4: Configurar Sprites/Iconos (Opcional)
Por ahora, puedes:
1. Crear cuadrados de colores en Photoshop/GIMP
2. Cada profesión un color diferente
3. Guardar en `Assets/Sprites/Professions/`
4. Asignar a cada ProfessionData asset

O usar placeholders:
1. En Unity: `GameObject > UI > Image` (crear temporalmente)
2. Right-click en sprite blanco > Export PNG
3. Duplicar y renombrar para cada profesión

---

### Paso 5: Testing Inicial

#### 5.1 Test de Flujo Básico
1. Abrir MainMenu scene
2. Click Play
3. Verificar que aparecen botones
4. Click "Solo Mode"
5. Debe cargar RoleSelection
6. Debe mostrar las 10 profesiones
7. Click en una profesión
8. Click "Confirm"
9. Debe cargar GameBoard
10. Debe ejecutar turnos automáticamente

#### 5.2 Test de Eventos
- Verificar que aparecen eventos aleatoriamente
- Probar eventos con decisiones
- Verificar que el dinero cambia correctamente

#### 5.3 Test de Final de Juego
- Esperar 12 turnos
- Debe cargar GameSummary
- Debe mostrar ganador

---

## 🎮 Características Implementadas

✅ **Sistema de Profesiones**: 10 roles con perfiles financieros realistas
✅ **Sistema de Eventos**: 20 eventos (gastos, ingresos, decisiones)
✅ **Sistema de Turnos**: 12 turnos automáticos
✅ **Economía**: Salarios, impuestos, modificadores, crecimiento
✅ **Dashboard Financiero**: Dinero actual, ganancias, gastos, gráfico
✅ **Sistema de Decisiones**: Eventos con múltiples opciones
✅ **Guardado Local**: Save/Load con JSON
✅ **Arquitectura Extensible**: Preparado para multijugador

---

## 🔧 Troubleshooting

### Error: "GameManager.Instance is null"
- Asegúrate de que GameManager prefab está en MainMenu scene
- Verifica que GameConfig está asignado en GameManager

### Error: "NullReferenceException en RoleSelection"
- Verifica que GameConfigData tiene profesiones asignadas
- Verifica que RoleCard prefab tiene todas las referencias

### Eventos no aparecen
- Verifica que GameConfig.EventChancePerTurn está en 0.7
- Verifica que los eventos tienen Probability > 0
- Verifica que MinTurnToAppear está configurado

### UI no se actualiza
- Verifica que UIManager tiene todas las referencias conectadas
- Verifica que los UnityEvents están conectados en los managers

---

## 📝 Próximas Mejoras Sugeridas

### Versión 0.2 - Multijugador
- Instalar Photon PUN2
- Implementar NetworkManager real
- Crear Lobby scene
- Sincronizar turnos entre jugadores

### Versión 0.3 - Polish
- Animaciones UI
- Efectos de sonido
- Música de fondo
- Partículas para eventos

### Versión 1.0 - Lanzamiento
- PlayFab Leaderboards
- Achievements
- Tutorial
- Más profesiones y eventos
- Balance testing

---

## 📚 Recursos

- **Plan Completo**: `C:\Users\victo\.claude\plans\enumerated-plotting-diffie.md`
- **Unity Docs**: https://docs.unity3d.com/
- **PlayFab Docs**: https://docs.microsoft.com/gaming/playfab/
- **Photon Docs**: https://doc.photonengine.com/

---

## ✨ Notas Finales

Este juego educativo está diseñado para enseñar:
- **Presupuesto**: Cómo manejar ingresos y gastos
- **Decisiones financieras**: Consecuencias de diferentes opciones
- **Perfiles de carrera**: Diferencias entre profesiones (salario vs educación)
- **Eventos inesperados**: Preparación para emergencias
- **Seguros**: Beneficios de tener seguro médico

¡Buena suerte con la implementación! 🎉
