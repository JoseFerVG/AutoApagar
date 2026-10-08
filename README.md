# AutoApagar ⚡

Aplicación nativa para Windows con interfaz gráfica moderna (Dark Mode) para programar el apagado del ordenador de forma sencilla y sin complicaciones.

![AutoApagar](https://img.shields.io/badge/Windows-10%2F11-0078D6?style=flat&logo=windows)
![License](https://img.shields.io/badge/license-MIT-green)

---

## 🚀 Características

- **Sin dependencias externas**: Ejecutable ligero y autónomo (`.exe`).
- **Interfaz moderna**: Diseño limpio con modo oscuro, bordes redondeados y efectos interactivos.
- **Accesos directos rápidos**: Botones preconfigurados de **15 min**, **30 min**, **1 hora** y **2 horas**.
- **Ajuste personalizado**: Control de horas y minutos.
- **Temporizador en tiempo real**: Cuenta atrás visible con indicación de la hora programada de apagado.
- **Cancelación segura**: Botón para cancelar el apagado en cualquier momento.
- **Protección de cierre**: Pregunta de confirmación si cierras la ventana con una programación activa.

---

## 🛠️ Compilación manual

Si deseas compilar el ejecutable tú mismo desde el código fuente sin instalar herramientas adicionales, puedes usar el compilador C# nativo de Windows:

```powershell
& "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /target:winexe /optimize+ /out:AutoApagar.exe Program.cs
```
