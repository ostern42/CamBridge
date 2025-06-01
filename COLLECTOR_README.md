# CamBridge Source Collector v2.0

## 🚀 Schnellstart

```batch
# Standard (balanced profile)
collect-sources.bat

# Spezifisches Profil
collect-sources.bat gui

# Intelligente Auswahl
collect-smart.bat

# Hilfe anzeigen
collect-sources.bat help
```

## 📋 Profile

| Profil | Coverage | Verwendung |
|--------|----------|------------|
| **minimal** | ~5% | Quick checks, kleine Fragen |
| **core** | ~15% | Core-Funktionalität ohne GUI |
| **gui** | ~20% | GUI-Entwicklung, Settings |
| **balanced** | ~25% | Standard für meiste Aufgaben |
| **mapping** | ~20% | Mapping Editor Entwicklung |
| **full** | ~50% | Komplettübersicht (Vorsicht!) |
| **custom** | ??? | Eigene Patterns definieren |

## 🎯 Empfohlene Workflows

### Build-Fehler beheben (v0.5.1)
```batch
collect-sources.bat balanced
```
Upload: PROJECT_WISDOM.md + PROJECT_CONTEXT_BALANCED_*.md

### GUI-Entwicklung
```batch
collect-sources.bat gui
```
Upload: PROJECT_WISDOM.md + PROJECT_CONTEXT_GUI_*.md

### Core-Tests
```batch
collect-sources.bat core
```
Upload: PROJECT_WISDOM.md + PROJECT_CONTEXT_CORE_*.md

### Automatische Auswahl
```batch
collect-smart.bat
```
Analysiert Git-Änderungen und wählt passendes Profil

## 📁 Output

Alle Dateien werden mit Timestamp erstellt:
- `PROJECT_CONTEXT_BALANCED_20250601_143022.md`
- Keine Überschreibungen mehr!
- Alte Outputs können gelöscht werden

## 🧹 Migration

```batch
# Alte Scripts archivieren
cleanup-old-collectors.bat
```

## 💡 Tipps

1. **Immer PROJECT_WISDOM.md zuerst uploaden!**
2. Nutze `collect-smart.bat` wenn unsicher
3. `balanced` reicht für 90% der Fälle
4. `full` nur wenn wirklich nötig (Token-Limit!)
5. Mit VOGON INIT im Chat starten

## 🔧 Anpassungen

Das Script kann einfach erweitert werden:
1. Neue Profile in `:collect_[name]` definieren
2. Zur Profile-Liste hinzufügen
3. Fertig!

---
© 2025 Claude's Improbably Reliable Software Solutions