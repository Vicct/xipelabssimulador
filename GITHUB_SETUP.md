# 🚀 Instrucciones para Subir a GitHub

## ✅ Estado Actual

Tu repositorio local está listo con:
- ✅ Git inicializado en branch `master`
- ✅ 2 commits realizados
- ✅ .gitignore configurado para Unity
- ✅ README.md completo
- ✅ LICENSE (MIT) agregada
- ✅ Todos los scripts C# commiteados
- ✅ Configuración del proyecto Unity

---

## 📋 Pasos para Crear Repositorio en GitHub

### Opción 1: Usando GitHub CLI (gh) - Recomendado

Si tienes GitHub CLI instalado, es el método más rápido:

```bash
# 1. Autenticarse con GitHub (si no lo has hecho)
gh auth login

# 2. Crear el repositorio directamente desde la terminal
gh repo create XipeLabsSimulador --public --source=. --remote=origin --push

# Esto automáticamente:
# - Crea el repo en GitHub
# - Agrega el remote "origin"
# - Hace push de todos los commits
```

**Alternativa con descripción:**
```bash
gh repo create XipeLabsSimulador \
  --public \
  --description "Juego educativo de finanzas personales - Educational personal finance game" \
  --source=. \
  --remote=origin \
  --push
```

---

### Opción 2: Usando la Interfaz Web de GitHub

#### Paso 1: Crear el Repositorio en GitHub.com

1. Ve a https://github.com/new
2. Configura el repositorio:
   - **Repository name**: `XipeLabsSimulador`
   - **Description**: "Juego educativo de finanzas personales - Educational personal finance game"
   - **Visibility**: Public (o Private si lo prefieres)
   - ❌ **NO marques** "Initialize this repository with a README"
   - ❌ **NO agregues** .gitignore ni license (ya los tienes)
3. Click en "Create repository"

#### Paso 2: Conectar tu Repositorio Local

GitHub te mostrará instrucciones, pero básicamente debes ejecutar:

```bash
# Agregar el remote (reemplaza TU_USUARIO con tu username de GitHub)
git remote add origin https://github.com/TU_USUARIO/XipeLabsSimulador.git

# Verificar que el remote se agregó correctamente
git remote -v

# Hacer push del branch master
git push -u origin master
```

**Si prefieres usar SSH en lugar de HTTPS:**
```bash
git remote add origin git@github.com:TU_USUARIO/XipeLabsSimulador.git
git push -u origin master
```

---

### Opción 3: Usando Visual Studio Code

Si tienes VS Code con extensión de GitHub:

1. Abrir la carpeta del proyecto en VS Code
2. Click en el ícono de Source Control (Ctrl+Shift+G)
3. Click en los tres puntos (...) → "Publish to GitHub"
4. Seleccionar "Publish to GitHub public repository"
5. Nombrar: `XipeLabsSimulador`
6. VS Code automáticamente hará el push

---

## 🔍 Verificar que Funcionó

Después de hacer push, verifica:

1. **En la terminal:**
   ```bash
   git remote -v
   # Debería mostrar:
   # origin  https://github.com/TU_USUARIO/XipeLabsSimulador.git (fetch)
   # origin  https://github.com/TU_USUARIO/XipeLabsSimulador.git (push)

   git branch -vv
   # Debería mostrar que master está trackeando origin/master
   ```

2. **En GitHub:**
   - Ve a `https://github.com/TU_USUARIO/XipeLabsSimulador`
   - Deberías ver:
     - ✅ README.md renderizado en la página principal
     - ✅ Carpeta `Assets/` con todos los scripts
     - ✅ 2 commits en el historial
     - ✅ Badges en el README (Unity version, License, Status)
     - ✅ LICENSE file

---

## 📦 Comandos Útiles de Git

### Ver el Estado
```bash
git status                  # Ver archivos modificados
git log --oneline          # Ver historial de commits
git log --graph --oneline  # Ver historial con gráfico
```

### Hacer Cambios Futuros
```bash
# 1. Hacer cambios en archivos
# 2. Ver qué cambió
git status
git diff

# 3. Agregar archivos al staging
git add .                  # Agregar todos los cambios
git add Assets/Scripts/    # Agregar solo una carpeta

# 4. Hacer commit
git commit -m "feat: descripción del cambio"

# 5. Subir a GitHub
git push
```

### Sincronizar con GitHub
```bash
# Descargar cambios de GitHub (si trabajas desde múltiples computadoras)
git pull

# Subir cambios locales
git push
```

---

## 🏷️ Crear un Release (Opcional)

Cuando tengas una versión estable:

```bash
# Opción 1: Con GitHub CLI
gh release create v0.1.0 \
  --title "MVP 0.1 - Modo Solo Jugador" \
  --notes "Primera versión funcional del juego con modo solo jugador"

# Opción 2: En GitHub web
# 1. Ve a tu repo en GitHub
# 2. Click en "Releases" → "Create a new release"
# 3. Tag: v0.1.0
# 4. Title: MVP 0.1 - Modo Solo Jugador
# 5. Description: características de la versión
# 6. Publish release
```

---

## 🔒 Proteger el Branch Master (Recomendado)

Si trabajas en equipo:

1. Ve a tu repo en GitHub
2. Settings → Branches
3. Add rule para `master`
4. Habilitar:
   - ✅ Require pull request reviews before merging
   - ✅ Require status checks to pass

---

## 🌿 Workflow de Desarrollo con Branches

Para desarrollo organizado:

```bash
# Crear branch para nueva característica
git checkout -b feature/multiplayer

# Hacer cambios y commits
git add .
git commit -m "feat: agregar lobby multiplayer"

# Subir branch a GitHub
git push -u origin feature/multiplayer

# En GitHub: crear Pull Request
# Después de review: merge a master

# Volver a master y actualizar
git checkout master
git pull
```

---

## 📊 Badges Adicionales para README (Opcional)

Puedes agregar más badges al README.md:

```markdown
![Build Status](https://img.shields.io/github/actions/workflow/status/TU_USUARIO/XipeLabsSimulador/build.yml)
![Last Commit](https://img.shields.io/github/last-commit/TU_USUARIO/XipeLabsSimulador)
![Issues](https://img.shields.io/github/issues/TU_USUARIO/XipeLabsSimulador)
![Stars](https://img.shields.io/github/stars/TU_USUARIO/XipeLabsSimulador?style=social)
```

---

## 🐛 Troubleshooting

### Error: "remote origin already exists"
```bash
# Ver remotes actuales
git remote -v

# Eliminar el remote existente
git remote remove origin

# Agregar el remote correcto
git remote add origin https://github.com/TU_USUARIO/XipeLabsSimulador.git
```

### Error: "failed to push some refs"
```bash
# Si hay cambios en GitHub que no tienes localmente
git pull --rebase origin master
git push
```

### Error: "Authentication failed"
```bash
# Si usas HTTPS, necesitas un Personal Access Token
# 1. Ve a GitHub → Settings → Developer settings → Personal access tokens
# 2. Generate new token (classic)
# 3. Selecciona scopes: repo (todos)
# 4. Usa el token como password al hacer push
```

---

## ✅ Checklist Final

Después de subir a GitHub, verifica:

- [ ] El README se ve correctamente en GitHub
- [ ] Todos los archivos están presentes (no faltan carpetas)
- [ ] El .gitignore está funcionando (no hay carpetas Library/ o Logs/)
- [ ] Los commits tienen buenos mensajes descriptivos
- [ ] La licencia MIT está visible
- [ ] Puedes clonar el repo en otra carpeta y funciona

---

## 🎉 ¡Listo!

Tu código ahora está:
- ✅ Versionado con Git
- ✅ Respaldado en GitHub
- ✅ Accesible desde cualquier lugar
- ✅ Listo para colaboración
- ✅ Documentado con README completo

**Siguiente paso**: Configurar las escenas Unity siguiendo [IMPLEMENTATION_GUIDE.md](Assets/IMPLEMENTATION_GUIDE.md)

---

## 📚 Recursos Adicionales

- **Git Cheat Sheet**: https://education.github.com/git-cheat-sheet-education.pdf
- **GitHub Guides**: https://guides.github.com/
- **GitHub CLI Docs**: https://cli.github.com/manual/
- **Conventional Commits**: https://www.conventionalcommits.org/

---

**¿Necesitas ayuda?** Crea un issue en el repo o contacta al equipo de desarrollo.
