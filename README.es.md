> [!WARNING]
> Todo lo que este fork añade o cambia (código, README, todo) lo ha escrito Claude (la IA de Anthropic). Ninguna línea la ha escrito un humano. Se basa en [Fortnite Chest Counter](https://github.com/LED-N/Fortnite-ChestCounter) de LED-N y los proyectos citados en los créditos. Úsalo bajo tu propia responsabilidad.

<div align="center">

[English](README.md) · [Français](README.fr.md) · **Español**

<img src="docs/icon.png" alt="" width="88">

# Storm Report

[![versión](https://img.shields.io/github/v/release/eierman324/ChestAddictionCounter?style=flat-square&label=versi%C3%B3n&color=7d55ff)](../../releases/latest) [![descargas](https://img.shields.io/github/downloads/eierman324/ChestAddictionCounter/total?style=flat-square&label=descargas&color=7d55ff)](../../releases) ![Windows 10 | 11](https://img.shields.io/badge/Windows-10%20%7C%2011-3a7bd5?style=flat-square) [![licencia](https://img.shields.io/github/license/eierman324/ChestAddictionCounter?style=flat-square&label=licencia&color=555)](LICENSE)

**Tu informe de Fortnite: victorias, eliminaciones, daño, cofres y tesoros, tu taquilla, con un overlay para el juego y para los directos.**

[**⬇ Descargar StormReport.exe**](../../releases/latest) · Windows 10 / 11 · gratis · sin instalación

</div>

---

> Storm Report nació como fork de [Fortnite Chest Counter](https://github.com/LED-N/Fortnite-ChestCounter) de LED-N. Conserva su conteo de cofres y los datos de la búsqueda del tesoro, y lo convierte en un informe completo de Fortnite. ¿Usabas Fortnite Chest Counter o Chest Addiction Counter? Tu historial y tus ajustes se conservan tal cual.

## Qué muestra

Storm Report une dos fuentes y nunca muestra la misma cifra de las dos:

| De tu **cuenta de Epic** (inicio de sesión obligatorio) | De tus **repeticiones** (las partidas grabadas en tu PC) |
|---|---|
| Partidas, victorias, % de victorias, eliminaciones, K/D y tiempo de juego, en total y por modo, de todas tus partidas en todos tus dispositivos | Cada partida: puesto, eliminaciones y asistencias, daño hecho y recibido, precisión, materiales, distancia |
| Toda tu taquilla (búsqueda, categorías, favoritos), paVos, compras, temporadas pasadas | Puesto medio, daño por partida, la sesión actual, las cifras en directo durante una partida |
| Los tesoros que tienes (la colección se rellena sola) | Cada cofre abierto o destruido, cofres por día y por hora, tesoros encontrados y su origen |

- **Informe**: resumen, una tabla por modo con las dos fuentes lado a lado, las últimas partidas, y después cofres y tesoros (tus probabilidades del atuendo de oro, la colección, los tesoros que otros jugadores consiguieron cerca de ti).
- **Cuenta / Taquilla**: tu cuenta de Epic, solo lectura, y un botón **Transferir toda mi taquilla a fortnite.gg**.
- **Overlay**: en el juego (sobre Fortnite en pantalla completa en ventana), para OBS (una página local) y una **ventana de directo** para apps sin fuente de navegador como TikTok LIVE Studio (captura de ventana, fondo oscuro o verde para chroma key). La cifra grande puede ser cofres, eliminaciones o victorias.

## Para empezar

1. Descarga **[`StormReport.exe`](../../releases/latest)** y ábrelo. No hace falta instalar nada.
   Windows puede mostrar *"Windows protegió su PC"* (exe sin firmar): *Más información → Ejecutar de todas formas*. La versión incluye también `StormReport.exe.sha256` para comprobar la descarga (`Get-FileHash .\StormReport.exe`).
2. Activa las repeticiones en Fortnite: *Ajustes → Juego → Repeticiones → Grabar repeticiones: **Activado***.
3. Inicia sesión en Epic en la pestaña **Cuenta**: el navegador abre el inicio de sesión de Epic; cuando una página muestre un texto con `authorizationCode`, pulsa Ctrl+A, Ctrl+C (se detecta solo) o pégalo en la ventana.

## Privacidad y seguridad

- **Cuenta de Epic, solo lectura**: Storm Report usa las mismas direcciones de Epic y clientes del juego que las herramientas de la comunidad (por ejemplo la importación de taquilla de fortnite.gg). Nunca se cambia nada en tu cuenta. No es una API pública oficial: Epic puede cambiarla o bloquearla.
- **Mantener la sesión** guarda un acceso de Epic cifrado con DPAPI de Windows (legible solo por tu usuario de Windows). **Cerrar sesión** lo borra en tu PC y en Epic.
- **Las repeticiones solo se leen**: `%LOCALAPPDATA%\FortniteGame\Saved\Demos`, abiertas en solo lectura. Ninguna modificación del juego, ningún acceso a su memoria.
- **Tus datos se quedan en tu PC** (`%APPDATA%\ChestCounter`). La ventana es una página servida solo a tu PC (`127.0.0.1`, clave aleatoria).
- **Red**: Epic (tu cuenta), fortnite-api.com (nombres e iconos de los objetos), fortnite.gg (solo cuando transfieres tu taquilla), GitHub (datos de la búsqueda del tesoro del proyecto original y actualizaciones de Storm Report).

## ¿Son fiables las cifras?

- **De Epic**: los totales oficiales de tu cuenta.
- **De las repeticiones**: puesto, eliminaciones, daño, precisión y las demás estadísticas de fin de partida las escribe el propio Fortnite en la repetición. Los cofres se reconocen a partir de los eventos de la repetición: los cofres que destruye un vehículo a veces no se graban (esas partidas se señalan), y los compañeros lejos de ti faltan en las cifras del equipo. El origen de un tesoro se estima y se señala en caso de duda.
- El lector de repeticiones no es de Epic: tras una gran actualización de Fortnite puede necesitar una actualización antes de volver a leer las repeticiones.

## Créditos

- [Fortnite Chest Counter](https://github.com/LED-N/Fortnite-ChestCounter) de LED-N: conteo de cofres, datos e imágenes de la búsqueda del tesoro.
- Correcciones e ideas de los forks de cevival (origen de los tesoros), koiseka (probabilidades semanales), tbpg58 (sumas de comprobación, registro de actividad), voiddog30 (cofres y tesoros por hora) y cherjp (comprobación de la clave de las repeticiones).
- Interfaz [Obsidian](https://docs.mspaint.cc/obsidian) de mspaint, [Tailwind CSS](https://tailwindcss.com) (MIT), [IBM Plex Mono](https://github.com/IBM/plex) (SIL OFL).
- [fortnite-api.com](https://fortnite-api.com) para nombres e iconos; el enlace de taquilla de fortnite.gg se construye como [FNGG-LockerGenerator](https://github.com/Liqutch/FNGG-LockerGenerator) (MIT).

Storm Report no está afiliado ni respaldado por Epic Games. Fortnite es una marca de Epic Games, Inc.

## Compilarlo tú mismo

Instala el [SDK de .NET 10](https://dotnet.microsoft.com/download/dotnet/10.0) y ejecuta `build.bat`: el exe aparece en `dist\`.
