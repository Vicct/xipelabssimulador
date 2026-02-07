# 💰 Money Matters - Juego Educativo de Finanzas Personales

![Unity Version](https://img.shields.io/badge/Unity-2022.3%20LTS-blue)
![License](https://img.shields.io/badge/License-MIT-green)
![Status](https://img.shields.io/badge/Status-En%20Desarrollo-yellow)

## 📖 Descripción

**Money Matters** es un juego de mesa digital educativo diseñado para enseñar finanzas personales de manera interactiva y divertida. Los jugadores asumen roles profesionales con perfiles financieros realistas y deben tomar decisiones financieras inteligentes para acumular la mayor cantidad de dinero posible.

### 🎯 Objetivos Educativos

- **Presupuesto**: Aprender a manejar ingresos y gastos mensuales
- **Decisiones Financieras**: Entender las consecuencias de diferentes opciones económicas
- **Perfiles de Carrera**: Comparar profesiones (salario alto vs costo de educación)
- **Eventos Inesperados**: Prepararse para emergencias financieras
- **Seguros**: Comprender los beneficios de tener seguro médico y otros

---

## 🎮 Características del Juego

### Versión Actual: MVP 0.1 (Modo Solo)

- ✅ **10 Profesiones Jugables** con perfiles financieros únicos
  - Desde barrendero ($2,200/mes) hasta doctor ($12,000/mes)
  - Cada profesión tiene: salario, bonos, costos de educación, seguros, impuestos
  - Crecimiento salarial anual según experiencia

- ✅ **20 Eventos Financieros Aleatorios**
  - 7 gastos comunes (reparación de auto, multas, emergencias)
  - 6 fuentes de ingreso extra (bonos, proyectos freelance)
  - 7 eventos con decisiones múltiples (fiestas, inversiones, compras)

- ✅ **Sistema de Turnos** (12 turnos = 1 año simulado)
  - Cada turno representa un mes
  - Pago automático de salario con impuestos
  - Eventos aleatorios ponderados por probabilidad

- ✅ **Dashboard Financiero Interactivo**
  - Dinero actual, total ganado, total gastado
  - Gráfico de historial de ingresos/gastos
  - Historial de transacciones detallado

- ✅ **Sistema de Guardado Local**
  - Save/Load de partidas en progreso
  - Persistencia con JSON

---

## 🏗️ Arquitectura Técnica

### Stack Tecnológico

- **Motor**: Unity 2022.3 LTS
- **Lenguaje**: C# (.NET 4.7.1)
- **Backend** (futuro): PlayFab + Azure Functions
- **Multiplayer** (futuro): Photon PUN2
- **Plataforma**: Mobile (Android/iOS)

### Estructura del Proyecto

```
Assets/
├── Scenes/                  # 4 escenas del juego
│   ├── MainMenu.unity
│   ├── RoleSelection.unity
│   ├── GameBoard.unity
│   └── GameSummary.unity
│
├── Scripts/
│   ├── Data/               # Modelos de datos
│   │   ├── ScriptableObjects/
│   │   │   ├── ProfessionData.cs
│   │   │   ├── FinancialEventData.cs
│   │   │   └── GameConfigData.cs
│   │   ├── PlayerState.cs
│   │   └── GameState.cs
│   │
│   ├── Managers/           # Controladores del juego
│   │   ├── GameManager.cs
│   │   ├── TurnManager.cs
│   │   ├── EconomyManager.cs
│   │   ├── EventManager.cs
│   │   ├── UIManager.cs
│   │   └── SaveManager.cs
│   │
│   ├── UI/                 # Componentes de interfaz
│   │   ├── MainMenu/
│   │   ├── RoleSelection/
│   │   ├── GameBoard/
│   │   └── GameSummary/
│   │
│   ├── Multiplayer/        # Placeholders para futuro
│   └── Utilities/
│
├── Data/                   # ScriptableObject instances
│   ├── Professions/       # 10 profesiones
│   ├── Events/            # 20 eventos financieros
│   └── Config/            # Configuración global
│
├── Prefabs/
├── Sprites/
└── Audio/
```

### Patrones de Diseño

- **Singleton Pattern**: GameManager, SaveManager
- **Observer Pattern**: UnityEvents para comunicación entre managers
- **ScriptableObject Pattern**: Configuración data-driven
- **State Machine**: GamePhase enum para fases del juego

---

## 🎲 Profesiones Disponibles

| Profesión | Salario/Mes | Educación | Seguro | Tier | Impuestos |
|-----------|-------------|-----------|--------|------|-----------|
| Barrendero | $2,200 | $0 | ❌ | Low | 10% |
| Cajero | $2,500 | $0 | ❌ | Low | 12% |
| Policía | $4,800 | $15,000 | ✅ | Medium | 18% |
| Maestro | $4,200 | $40,000 | ✅ | Medium | 16% |
| Chef | $3,800 | $25,000 | ❌ | Medium | 15% |
| Ingeniero | $7,500 | $80,000 | ✅ | High | 22% |
| Doctor | $12,000 | $200,000 | ✅ | High | 28% |
| Piloto | $9,500 | $100,000 | ✅ | High | 24% |
| Programador | $8,200 | $50,000 | ✅ | High | 20% |
| Abogado | $10,500 | $150,000 | ✅ | High | 26% |

---

## 📋 Eventos Financieros (Ejemplos)

### Gastos Automáticos
- **Reparación de Auto**: -$1,200 (15% probabilidad)
- **Emergencia Médica**: -$3,500 → -$1,750 con seguro (8% prob)
- **Multa de Estacionamiento**: -$150 (18% prob)

### Ingresos Extra
- **Reembolso de Impuestos**: +$1,800 (12% prob, turno 3+)
- **Bono Sorpresa**: +$2,500 (10% prob)
- **Proyecto Freelance**: +$2,800 (9% prob)

### Eventos con Decisiones
- **Fiesta de Cumpleaños**:
  - Fiesta grande: -$800
  - Reunión pequeña: -$300
  - Omitir: $0

- **Inversión en Acciones**:
  - Invertir $2,000
  - Invertir $1,000
  - No invertir

---

## 🚀 Instalación y Setup

### Requisitos

- Unity 2022.3 LTS o superior
- Visual Studio 2022 o VS Code
- Git

### Pasos de Instalación

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/TU_USUARIO/XipeLabsSimulador.git
   cd XipeLabsSimulador
   ```

2. **Abrir en Unity**
   - Abrir Unity Hub
   - Click en "Add" → Seleccionar la carpeta del proyecto
   - Abrir con Unity 2022.3 LTS

3. **Configurar ScriptableObjects**
   - Seguir instrucciones en `Assets/IMPLEMENTATION_GUIDE.md`
   - Crear los 10 assets de profesiones
   - Crear los 20 assets de eventos
   - Crear GameConfig y asignar arrays

4. **Crear las Escenas**
   - Seguir guía detallada en `IMPLEMENTATION_GUIDE.md`
   - Configurar UI en cada escena
   - Conectar referencias en Inspector

5. **Build Settings**
   - File > Build Settings
   - Agregar escenas en orden: MainMenu → RoleSelection → GameBoard → GameSummary

---

## 🎯 Roadmap

### ✅ Versión 0.1 - MVP (Actual)
- [x] Modo solo jugador
- [x] 10 profesiones
- [x] 20 eventos financieros
- [x] Sistema de turnos
- [x] Dashboard básico
- [x] Guardado local

### 🔄 Versión 0.2 - Multijugador (En Desarrollo)
- [ ] Integración con Photon PUN2
- [ ] Lobby para 2-4 jugadores
- [ ] Sincronización de turnos
- [ ] Matchmaking con PlayFab
- [ ] Chat entre jugadores

### 📅 Versión 1.0 - Lanzamiento Completo
- [ ] PlayFab Leaderboards globales
- [ ] Sistema de achievements
- [ ] Tutorial interactivo
- [ ] 20+ profesiones adicionales
- [ ] 40+ eventos nuevos
- [ ] Música y efectos de sonido
- [ ] Animaciones UI avanzadas
- [ ] Soporte multi-idioma

### 🌟 Versión 2.0 - IA Educativa
- [ ] Azure OpenAI para asesor financiero NPC
- [ ] Análisis de decisiones del jugador
- [ ] Consejos personalizados
- [ ] Modo campaña narrativo
- [ ] Reportes financieros detallados

---

## 🛠️ Desarrollo

### Compilar el Proyecto

```bash
# Abrir Unity y hacer Build
File > Build Settings > Build
```

### Testing

1. **Test de Flujo Básico**
   - MainMenu → RoleSelection → GameBoard → GameSummary

2. **Test de Profesiones**
   - Jugar con cada una de las 10 profesiones
   - Verificar salarios, bonos, impuestos

3. **Test de Eventos**
   - Verificar que aparecen aleatoriamente
   - Probar eventos con decisiones
   - Verificar modificadores (seguro médico)

### Estructura de Commits

```
feat: nueva característica
fix: corrección de bug
docs: documentación
style: formato de código
refactor: refactorización
test: agregar tests
chore: tareas de mantenimiento
```

---

## 📚 Documentación

- **Guía de Implementación**: [IMPLEMENTATION_GUIDE.md](Assets/IMPLEMENTATION_GUIDE.md)
- **Plan Completo**: `~/.claude/plans/enumerated-plotting-diffie.md`
- **Unity Docs**: https://docs.unity3d.com/
- **PlayFab Docs**: https://docs.microsoft.com/gaming/playfab/

---

## 🤝 Contribución

¡Las contribuciones son bienvenidas! Por favor:

1. Fork el proyecto
2. Crea una branch (`git checkout -b feature/NuevaCaracteristica`)
3. Commit tus cambios (`git commit -m 'feat: agregar nueva característica'`)
4. Push a la branch (`git push origin feature/NuevaCaracteristica`)
5. Abre un Pull Request

---

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver archivo `LICENSE` para más detalles.

---

## 👥 Autores

- **XipeLabs** - Desarrollo inicial
- **Claude AI** - Asistencia en arquitectura y código

---

## 🙏 Agradecimientos

- Unity Technologies por el motor de juego
- PlayFab por servicios backend
- Photon por networking multiplayer
- Comunidad de desarrolladores de juegos educativos

---

## 📞 Contacto

- **Proyecto**: [GitHub Repository](https://github.com/TU_USUARIO/XipeLabsSimulador)
- **Issues**: [Reportar Bug](https://github.com/TU_USUARIO/XipeLabsSimulador/issues)

---

## 📊 Estado del Proyecto

**Última Actualización**: 2026-02-06

**Estado Actual**:
- ✅ Arquitectura de código completa (30+ scripts C#)
- ✅ Sistema de datos con ScriptableObjects
- ✅ Managers de juego funcionales
- ✅ UI controllers implementados
- ⏳ Escenas Unity por configurar
- ⏳ Assets ScriptableObject por crear
- ⏳ UI por diseñar en Unity Editor

**Próximo Milestone**: Completar configuración de escenas Unity y assets

---

## 🎓 Valor Educativo

Este juego enseña conceptos financieros clave:

1. **Valor del Tiempo**: Profesiones con alta educación (Doctor) tardan más en ser rentables
2. **Gestión de Riesgos**: Importancia de seguros para emergencias
3. **Toma de Decisiones**: Cada elección tiene consecuencias financieras
4. **Planificación**: Balance entre gastos inmediatos y objetivos a largo plazo
5. **Comparación de Carreras**: Entender trade-offs entre diferentes profesiones

---

**¡Aprende finanzas mientras juegas!** 💰🎮
