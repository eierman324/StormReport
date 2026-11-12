namespace ChestCounter;

/// <summary>
/// The page of the graphical interface: one HTML file (styles and script inside), drawn by the script from the state
/// the counter sends (<see cref="GuiServer"/>). Texts come with the state, in the chosen language.
/// </summary>
public static class GuiPage
{
    /// <summary>Every text of the page, in the chosen language.</summary>
    public static Dictionary<string, string> Texts() => new()
    {
        ["home"] = L.T("Accueil", "Home", "Inicio"),
        ["report"] = L.T("Rapport", "Report", "Informe"),
        ["overlay"] = L.T("Overlay", "Overlay", "Overlay"),
        ["settings"] = L.T("Réglages", "Settings", "Ajustes"),
        ["help"] = L.T("Aide", "Help", "Ayuda"),
        ["total"] = L.T("Coffres au total", "Chests in total", "Cofres en total"),
        ["totalSub"] = L.T("ouverts ou détruits par toi", "opened or destroyed by you", "abiertos o destruidos por ti"),
        ["byHand"] = L.T("dont {0} ajoutés à la main", "{0} of them added by hand", "de ellos, {0} añadidos a mano"),
        ["session"] = L.T("Session", "Session", "Sesión"),
        ["last"] = L.T("Dernière partie", "Last match", "Última partida"),
        ["squad"] = L.T("Total d'équipe", "Squad total", "Total del equipo"),
        ["squadSub"] = L.T("toi + {0} par tes coéquipiers", "you + {0} by your teammates", "tú + {0} de tus compañeros"),
        ["inaccurate"] = L.T("imprécis", "inaccurate", "impreciso"),
        ["inaccurateWhy"] = L.T("Compté dans TES replays : un coffre ouvert par un coéquipier à plus de 150 m de toi n'y est pas, et rien n'est enregistré après ton départ de la partie. Le vrai total de l'équipe est plus haut.",
            "Counted in YOUR replays: a chest a teammate opens more than 150 m away from you is not in them, and nothing is recorded once you leave the match. The real squad total is higher.",
            "Contado en TUS repeticiones: un cofre que un compañero abre a más de 150 m de ti no está en ellas, y no se graba nada después de que salgas de la partida. El total real del equipo es mayor."),
        ["treasures"] = L.T("Trésors", "Treasures", "Tesoros"),
        ["matches"] = L.T("parties", "matches", "partidas"),
        ["perMatch"] = L.T("{0} par partie", "{0} per match", "{0} por partida"),
        ["openReport"] = L.T("Ouvrir le rapport", "Open the report", "Abrir el informe"),
        ["showOverlay"] = L.T("Afficher l'overlay", "Show the overlay", "Mostrar el overlay"),
        ["hideOverlay"] = L.T("Masquer l'overlay", "Hide the overlay", "Ocultar el overlay"),
        ["lock"] = L.T("Verrouiller l'overlay", "Lock the overlay", "Bloquear el overlay"),
        ["unlock"] = L.T("Déverrouiller pour le déplacer", "Unlock to move it", "Desbloquear para moverlo"),
        ["activity"] = L.T("Activité", "Activity", "Actividad"),
        ["noActivity"] = L.T("Rien pour l'instant.", "Nothing yet.", "Nada por ahora."),
        ["analyzing"] = L.T("Analyse de tes replays…", "Analyzing your replays…", "Analizando tus repeticiones…"),
        ["whatsNew"] = L.T("Nouveautés de la {0}", "What's new in {0}", "Novedades de la {0}"),
        ["newVersion"] = L.T("Nouvelle version {0} disponible", "New version {0} available", "Nueva versión {0} disponible"),
        ["install"] = L.T("Installer", "Install", "Instalar"),
        ["checkUpdate"] = L.T("Rechercher une mise à jour", "Check for updates", "Buscar actualizaciones"),
        ["welcome"] = L.T("Bienvenue !", "Welcome!", "¡Bienvenido!"),
        ["welcomeText"] = L.T("Active d'abord les rediffusions dans Fortnite : Paramètres → Jeu → Rediffusions → Enregistrer les rediffusions. Ensuite, laisse le compteur ouvert : chaque partie est comptée quelques secondes après sa fin. Des coffres ouverts avant ? Le crayon de « Coffres au total » permet de les ajouter.",
            "First turn on replays in Fortnite: Settings → Game → Replays → Record Replays. Then keep the counter open: each match is counted a few seconds after it ends. Chests opened before? The pencil of \"Chests in total\" adds them.",
            "Primero activa las repeticiones en Fortnite: Ajustes → Juego → Repeticiones → Grabar repeticiones. Luego deja el contador abierto: cada partida se cuenta unos segundos después de terminar. ¿Cofres abiertos antes? El lápiz de «Cofres en total» permite añadirlos."),
        ["ok"] = L.T("OK", "OK", "OK"),
        ["moreInfo"] = L.T("En savoir plus", "More info", "Más información"),
        ["wMin"] = L.T("Réduire", "Minimize", "Minimizar"),
        ["wMax"] = L.T("Agrandir", "Maximize", "Maximizar"),
        ["wRestore"] = L.T("Niveau inférieur", "Restore down", "Restaurar"),
        ["wClose"] = L.T("Fermer", "Close", "Cerrar"),
        ["keepOpen"] = L.T("Garder le compteur ouvert", "Keep the counter open", "Mantener el contador abierto"),
        ["autoCloseText"] = L.T("Fortnite est fermé : le compteur, ouvert avec Fortnite, va se fermer tout seul.", "Fortnite is closed: the counter, opened with Fortnite, is about to close by itself.", "Fortnite está cerrado: el contador, abierto con Fortnite, se va a cerrar solo."),
        ["welcomeQuick"] = L.T("Tes premiers réglages (tu les retrouves à tout moment dans Réglages) :", "Your first settings (you find them at any time in Settings):", "Tus primeros ajustes (los encuentras en cualquier momento en Ajustes):"),
        ["welcomeDone"] = L.T("C'est parti !", "Let's go!", "¡Vamos!"),
        ["inGame"] = L.T("Overlay en jeu", "In-game overlay", "Overlay en el juego"),
        ["inGameHelp"] = L.T("Petite fenêtre toujours au premier plan qui affiche tes coffres en jeu (Fortnite en « Plein écran fenêtré »). Ctrl+Maj+O : afficher / masquer, Ctrl+Maj+L : verrouiller.",
            "Small always-on-top window that shows your chests in game (Fortnite in \"Windowed Fullscreen\"). Ctrl+Shift+O: show / hide, Ctrl+Shift+L: lock.",
            "Pequeña ventana siempre visible que muestra tus cofres en el juego (Fortnite en «Pantalla completa en ventana»). Ctrl+Mayús+O: mostrar / ocultar, Ctrl+Mayús+L: bloquear."),
        ["style"] = L.T("Style", "Style", "Estilo"),
        ["hud"] = L.T("HUD (intégré au jeu)", "HUD (blends into the game)", "HUD (integrado en el juego)"),
        ["classic"] = L.T("Classique (carte sombre)", "Classic (dark card)", "Clásico (tarjeta oscura)"),
        ["size"] = L.T("Taille", "Size", "Tamaño"),
        ["opacity"] = L.T("Opacité", "Opacity", "Opacidad"),
        ["squadToggle"] = L.T("Afficher le total d'équipe", "Show the squad total", "Mostrar el total del equipo"),
        ["squadHelp"] = L.T("Tes coffres + ceux de tous tes coéquipiers (même aléatoires), sur l'overlay et l'accueil.",
            "Your chests + those of all your teammates (random ones too), on the overlay and the home tab.",
            "Tus cofres + los de todos tus compañeros (también los aleatorios), en el overlay y el inicio."),
        ["obs"] = L.T("Overlay pour OBS (stream)", "Overlay for OBS (stream)", "Overlay para OBS (directo)"),
        ["obsSteps"] = L.T("Dans OBS : Sources → + → Navigateur, coche « Fichier local », choisis ce fichier. Largeur 360, hauteur 220 (450 × 300 en classique). Pour ne l'avoir que sur le stream, désactive l'overlay en jeu : la page continue d'être mise à jour.",
            "In OBS: Sources → + → Browser, tick \"Local file\", pick this file. Width 360, height 220 (450 × 300 for classic). To have it on the stream only, turn the in-game overlay off: the page keeps being updated.",
            "En OBS: Fuentes → + → Navegador, marca «Archivo local», elige este archivo. Ancho 360, alto 220 (450 × 300 en clásico). Para tenerlo solo en el directo, desactiva el overlay en el juego: la página se sigue actualizando."),
        ["copy"] = L.T("Copier le chemin", "Copy the path", "Copiar la ruta"),
        ["copied"] = L.T("Copié !", "Copied!", "¡Copiado!"),
        ["preview"] = L.T("Aperçu", "Preview", "Vista previa"),
        ["language"] = L.T("Langue", "Language", "Idioma"),
        ["tray"] = L.T("Réduire dans la zone de notification", "Minimize to the notification area", "Minimizar al área de notificación"),
        ["trayHelp"] = L.T("Oui : réduire la fenêtre la range à côté de l'horloge, en bas à droite de la barre des tâches (icône du coffre, parfois derrière la petite flèche ^). Non : elle reste dans la barre des tâches comme une fenêtre normale. Dans les deux cas, le compteur continue de compter.",
            "Yes: minimizing the window puts it next to the clock, at the bottom right of the taskbar (chest icon, sometimes behind the small ^ arrow). No: it stays in the taskbar like a normal window. Either way, the counter keeps counting.",
            "Sí: minimizar la ventana la coloca junto al reloj, abajo a la derecha de la barra de tareas (icono del cofre, a veces detrás de la pequeña flecha ^). No: se queda en la barra de tareas como una ventana normal. En ambos casos, el contador sigue contando."),
        ["footerWindow"] = L.T("La croix de la fenêtre ferme le compteur (après confirmation).", "The close button of the window stops the counter (after asking).", "La cruz de la ventana cierra el contador (tras confirmarlo)."),
        ["withFortnite"] = L.T("Ouvrir avec Fortnite", "Open with Fortnite", "Abrir con Fortnite"),
        ["withFortniteHelp"] = L.T("Un petit processus invisible attend Fortnite et ouvre le compteur, qui se ferme après.", "A tiny invisible process waits for Fortnite and opens the counter, which closes afterwards.", "Un pequeño proceso invisible espera a Fortnite y abre el contador, que se cierra después."),
        ["updates"] = L.T("Mises à jour", "Updates", "Actualizaciones"),
        ["upPrompt"] = L.T("Proposées", "Offered", "Propuestas"),
        ["upAuto"] = L.T("Installées automatiquement", "Installed automatically", "Instaladas automáticamente"),
        ["upOff"] = L.T("Désactivées", "Off", "Desactivadas"),
        ["profile"] = L.T("Compte (profil)", "Account (profile)", "Cuenta (perfil)"),
        ["demos"] = L.T("Dossier des replays", "Replays folder", "Carpeta de repeticiones"),
        ["demosMissing"] = L.T("Introuvable : Fortnite le crée à la première partie enregistrée.", "Not found: Fortnite creates it with the first recorded match.", "No encontrada: Fortnite la crea con la primera partida grabada."),
        ["save"] = L.T("Enregistrer", "Save", "Guardar"),
        ["editChests"] = L.T("Modifier mon nombre de coffres", "Edit my number of chests", "Modificar mi número de cofres"),
        ["close"] = L.T("Fermer", "Close", "Cerrar"),
        ["manual"] = L.T("Coffres ajoutés à la main", "Chests added by hand", "Cofres añadidos a mano"),
        ["manualHelp"] = L.T("Les coffres ouverts avant d'utiliser le compteur. Un nombre négatif en retire, 0 enlève l'ajout.", "The chests opened before using the counter. A negative number removes some, 0 clears it.", "Los cofres abiertos antes de usar el contador. Un número negativo quita cofres, 0 lo borra."),
        ["estimate"] = L.T("Ou estimer à partir de tes parties", "Or estimate from your matches", "O estimar a partir de tus partidas"),
        ["estimateHelp"] = L.T("Nombre de parties Battle Royale de la saison, dans Fortnite : Carrière → Statistiques.", "Number of Battle Royale matches this season, in Fortnite: Career → Stats.", "Número de partidas de Battle Royale de la temporada, en Fortnite: Carrera → Estadísticas."),
        ["career"] = L.T("Parties jouées", "Matches played", "Partidas jugadas"),
        ["compute"] = L.T("Calculer", "Compute", "Calcular"),
        ["add"] = L.T("Ajouter {0} coffres", "Add {0} chests", "Añadir {0} cofres"),
        ["estimateTooFew"] = L.T("Il faut au moins {0} parties comptées pour une moyenne fiable.", "At least {0} counted matches are needed for a reliable average.", "Hacen falta al menos {0} partidas contadas para una media fiable."),
        ["estimateNothing"] = L.T("Rien à ajouter : tes parties comptées couvrent déjà ce nombre.", "Nothing to add: your counted matches already cover that number.", "Nada que añadir: tus partidas contadas ya cubren ese número."),
        ["dataFolder"] = L.T("Ouvrir le dossier des données", "Open the data folder", "Abrir la carpeta de datos"),
        ["errorLog"] = L.T("Ouvrir le journal d'erreurs", "Open the error log", "Abrir el registro de errores"),
        ["issue"] = L.T("Signaler un problème (GitHub)", "Report a problem (GitHub)", "Informar de un problema (GitHub)"),
        ["project"] = L.T("Page du projet (GitHub)", "Project page (GitHub)", "Página del proyecto (GitHub)"),
        ["helpText"] = L.T("Le compteur lit les replays que Fortnite enregistre et compte tes coffres quelques secondes après chaque partie (Battle Royale de la saison seulement). Laisse-le ouvert pendant que tu joues : tu peux réduire la fenêtre, il continue de compter.",
            "The counter reads the replays Fortnite saves and counts your chests a few seconds after each match (the season's Battle Royale only). Keep it open while you play: you can minimize the window, it keeps counting.",
            "El contador lee las repeticiones que guarda Fortnite y cuenta tus cofres unos segundos después de cada partida (solo el Battle Royale de la temporada). Déjalo abierto mientras juegas: puedes minimizar la ventana, sigue contando."),
        ["uninstall"] = L.T("Désinstaller ChestCounter", "Uninstall ChestCounter", "Desinstalar ChestCounter"),
        ["keepData"] = L.T("Garder mon historique et mes réglages", "Keep my history and settings", "Conservar mi historial y mis ajustes"),
        ["deleteData"] = L.T("Tout supprimer", "Delete everything", "Borrarlo todo"),
        ["uninstallConfirm"] = L.T("Désinstaller maintenant ? Le compteur se ferme et ChestCounter.exe est supprimé.", "Uninstall now? The counter closes and ChestCounter.exe is deleted.", "¿Desinstalar ahora? El contador se cierra y se borra ChestCounter.exe."),
        ["quit"] = L.T("Quitter le compteur", "Quit the counter", "Salir del contador"),
        ["quitConfirm"] = L.T("Arrêter le compteur ? Les parties jouées ensuite ne seront comptées qu'à sa prochaine ouverture.", "Stop the counter? Matches played afterwards are only counted when it is opened again.", "¿Detener el contador? Las partidas jugadas después solo se cuentan cuando se vuelva a abrir."),
        ["closed"] = L.T("Le compteur est arrêté. Tu peux fermer cette fenêtre.", "The counter is stopped. You can close this window.", "El contador está detenido. Puedes cerrar esta ventana."),
        ["offline"] = L.T("Le compteur ne répond plus (fermé ?). Relance ChestCounter.exe.", "The counter does not answer (closed?). Start ChestCounter.exe again.", "El contador no responde (¿cerrado?). Vuelve a abrir ChestCounter.exe."),
        ["footer"] = L.T("Fermer cette fenêtre ne coupe pas le compteur.", "Closing this window does not stop the counter.", "Cerrar esta ventana no detiene el contador."),
    };

    public static string Html(string token) => Template.Replace("__TOKEN__", token).Replace("__PENCIL__", ReportWriter.PencilIcon);

    private const string Template = """
        <!doctype html><html><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1">
        <title>Chest Counter</title>
        <style>
        :root{color-scheme:dark;--bg:#0d0d0d;--side:#0f0f0f;--panel:#0f0f0f;--main:#191919;--line:#282828;--line2:#363636;--ink:#ffffff;--muted:rgba(255,255,255,.6);--dim:rgba(255,255,255,.4);--title:#fff;--card:#0f0f0f;--card2:#191919;--cink:#fff;--cmuted:rgba(255,255,255,.6);--track:#191919;--gold:#7d55ff;--accent:#7d55ff;--accentInk:#fff;--accentSoft:rgba(125,85,255,.14);--team:#2fd28a;--warn:#ff5a4f;--warnSoft:rgba(255,90,79,.12);--focus:#7d55ff;--mono:"IBM Plex Mono","Cascadia Mono","Cascadia Code",Consolas,monospace}
        *{box-sizing:border-box}
        html,body{height:100%}
        body{margin:0;background:var(--bg);color:var(--ink);font:12.5px/1.45 var(--mono)}
        button,select,input{font-family:var(--mono)}
        .pirate{font-family:inherit;font-weight:500}
        .num{font-family:var(--mono);font-variant-numeric:tabular-nums}
        #scroll{min-height:100%;display:flex;flex-direction:column}
        .shell{flex:1;display:grid;grid-template-columns:220px minmax(0,1fr)}
        aside{background:var(--side);border-right:1px solid var(--line)}
        .sidein{position:sticky;top:0;display:flex;flex-direction:column}
        .brand{height:44px;display:flex;align-items:center;justify-content:center;gap:8px;border-bottom:1px solid var(--line);background:#0d0d0d;font-size:13px;color:var(--ink);padding:0 10px;white-space:nowrap;overflow:hidden;text-overflow:ellipsis}
        .logo{color:var(--accent);display:grid;place-items:center;flex:none}
        .logo svg{width:16px;height:16px}
        aside nav{display:flex;flex-direction:column}
        aside nav button{display:flex;align-items:center;gap:10px;padding:10px 12px;border:0;background:transparent;color:var(--muted);font:500 13px var(--mono);text-align:left;cursor:pointer;transition:color .1s,background .1s}
        aside nav button svg{width:16px;height:16px;flex:none;color:var(--accent)}
        aside nav button:hover{color:var(--ink)}
        aside nav button.on{background:var(--main);color:var(--ink)}
        aside nav button:focus-visible{outline:1px solid var(--accent);outline-offset:-1px}
        .who{display:flex;flex-direction:column;gap:6px;padding:12px;border-top:1px solid var(--line);color:var(--muted);font-size:12px}
        .who select{width:100%}
        .content{min-width:0}
        main{max-width:1000px;padding:10px 12px 24px;display:flex;flex-direction:column;gap:10px}
        #app{display:flex;flex-direction:column;gap:10px}
        #hdr{display:block}
        #view{display:flex;flex-direction:column;gap:10px}
        #view:empty{display:none}
        #rep{width:100%;min-height:400px;border:0;display:block;background:transparent;overflow:hidden}
        header{height:30px;display:flex;align-items:center;justify-content:space-between;gap:12px;padding:0 10px;background:var(--main);border:1px solid var(--line);border-radius:4px}
        .crumb{font-size:13px;color:var(--muted)}
        .crumb b{color:var(--ink);font-weight:500}
        .pill{display:inline-flex;align-items:center;gap:7px;font-size:12px;color:var(--muted)}
        .pill i{width:7px;height:7px;border-radius:50%;background:var(--team)}
        .pill.live i{background:#ff3b30}.pill.busy i{background:#e0a526}.pill.warn i{background:var(--warn)}
        h1,h2{margin:0;font-size:13px;font-weight:400;color:var(--ink)}
        .cols{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:10px;align-items:start}
        @media (max-width:900px){.cols{grid-template-columns:1fr}}
        .gb{background:var(--panel);border:1px solid var(--line);border-radius:4px;min-width:0}
        .gbh{display:flex;align-items:center;justify-content:space-between;gap:8px;min-height:34px;padding:0 10px;border-bottom:1px solid var(--line);font-size:13px}
        .gbb{padding:8px 10px}
        .tiles{display:grid;grid-template-columns:repeat(auto-fit,minmax(150px,1fr));gap:10px}
        .tile{position:relative;background:var(--panel);border:1px solid var(--line);border-radius:4px;padding:0;display:flex;flex-direction:column}
        .tile .th{min-height:34px;padding:0 34px 0 10px;display:flex;align-items:center;border-bottom:1px solid var(--line);font-size:12.5px;color:var(--ink)}
        .tile .v{padding:10px 10px 0;font-size:26px;line-height:1.1;color:var(--ink)}
        .tile .l{padding:2px 10px 10px;font-size:11.5px;color:var(--muted)}
        .tile.hero{border-color:var(--accent)}
        .tile.hero .v{font-size:34px;color:var(--accent)}
        .editbtn{position:absolute;top:5px;right:6px;width:22px;height:22px;padding:0;border-radius:4px;border:1px solid var(--line);background:var(--main);color:var(--muted);cursor:pointer;display:grid;place-items:center}
        .editbtn:hover{color:var(--ink);border-color:var(--line2)}.editbtn:focus-visible{outline:1px solid var(--accent)}
        .editbtn svg{width:12px;height:12px}
        dialog{width:min(500px,calc(100% - 32px));padding:0;border:1px solid var(--line);border-radius:4px;background:var(--panel);color:var(--ink);box-shadow:0 20px 60px rgba(0,0,0,.6)}
        dialog::backdrop{background:rgba(0,0,0,.6)}
        .dhead{display:flex;align-items:center;justify-content:space-between;gap:12px;min-height:34px;padding:0 6px 0 10px;border-bottom:1px solid var(--line)}
        #dlgB{padding:10px}
        .dclose{width:26px;height:26px;border-radius:4px;border:0;background:transparent;color:var(--muted);font-size:16px;line-height:1;cursor:pointer}
        .dclose:hover{background:var(--main);color:var(--ink)}
        .card{background:var(--panel);border:1px solid var(--line);border-radius:4px;padding:8px 10px}
        .gb .card{border:0;padding:0;background:transparent}
        ul{margin:4px 0 10px;padding-left:18px;color:var(--muted)}
        p{margin:4px 0 8px;color:var(--muted)}
        .row{display:flex;align-items:center;justify-content:space-between;gap:16px;padding:7px 0}
        .row+.row{border-top:1px solid var(--line)}
        .row .t{color:var(--ink)}.row .h{font-size:11.5px;color:var(--dim);margin-top:2px;max-width:600px}
        .actions{display:flex;flex-wrap:wrap;gap:6px}
        button.b{min-height:26px;padding:3px 12px;border-radius:4px;border:1px solid var(--line);background:var(--main);color:var(--ink);font:400 12px var(--mono);cursor:pointer;transition:border-color .1s,color .1s}
        button.b:hover{border-color:var(--line2)}
        button.b:focus-visible{outline:1px solid var(--accent)}
        button.b.main{border-color:var(--accent);background:var(--accentSoft)}
        button.b.main:hover{background:rgba(125,85,255,.24)}
        button.b.danger{color:var(--warn)}
        button.b.danger:hover{border-color:var(--warn)}
        select,input[type=text],input[type=number]{min-height:26px;padding:2px 8px;border-radius:4px;border:1px solid var(--line);background:var(--main);color:var(--ink);font:12px var(--mono)}
        select:focus,input:focus{outline:none;border-color:var(--accent)}
        select option{background:var(--main);color:var(--ink)}
        input[type=text]{width:min(520px,100%)}
        input[type=number]{width:140px}
        .switch{position:relative;display:inline-block;width:30px;height:16px;flex:none;vertical-align:middle}
        .switch input{opacity:0;width:100%;height:100%;margin:0;cursor:pointer;position:absolute;z-index:1}
        .switch span{position:absolute;inset:0;border-radius:8px;background:var(--main);border:1px solid var(--line);transition:background .12s,border-color .12s}
        .switch span:after{content:"";position:absolute;top:2px;left:2px;width:10px;height:10px;border-radius:50%;background:#fff;transition:transform .12s}
        .switch input:checked+span{background:var(--accent);border-color:var(--accent)}.switch input:checked+span:after{transform:translateX(14px)}
        .switch input:focus-visible+span{outline:1px solid var(--accent);outline-offset:1px}
        .badge{display:inline-block;margin-left:6px;padding:0 5px;border-radius:3px;border:1px solid rgba(255,90,79,.45);color:var(--warn);font-size:10.5px;vertical-align:1px;cursor:help}
        .log{font:12px/1.6 var(--mono);max-height:280px;overflow:auto;white-space:pre-wrap;color:var(--muted)}
        .log .err{color:#ff6b61}.log .warn{color:#f0b23e}.log .ok{color:#2fd28a}.log .info{color:#5ab4ff}.log .tre{color:#a68bff}.log .mut{color:var(--dim)}
        .bar{position:relative;height:17px;background:var(--main);border:1px solid var(--line);border-radius:4px;overflow:hidden;margin-top:8px}.bar i{display:block;height:100%;background:var(--accent)}
        .path{font:12px var(--mono);word-break:break-all;background:var(--main);border:1px solid var(--line);padding:5px 8px;border-radius:4px;display:block;margin:2px 0 8px}
        .note{font-size:11.5px;color:var(--dim)}
        .hidden{display:none!important}
        footer{height:24px;display:flex;justify-content:center;align-items:center;gap:14px;background:#171717;border-top:1px solid var(--line);color:var(--muted);font-size:11.5px;padding:0 10px;position:sticky;bottom:0;z-index:5}
        footer button.b{min-height:18px;padding:0 8px;font-size:11px}
        .estimate{margin-top:8px}
        .notice{position:relative;display:flex;gap:10px;align-items:flex-start;border-left:2px solid var(--accent)}
        .notice.warning{border-left-color:var(--warn)}
        .notice .ni{flex:none;width:16px;height:16px;border-radius:3px;display:grid;place-items:center;font:600 11px var(--mono);background:var(--accent);color:#fff;margin-top:1px}
        .notice.warning .ni{background:var(--warn)}
        .notice .nb{flex:1;min-width:0}.notice .nt{color:var(--ink);margin-bottom:2px}.notice .nx{white-space:pre-line;color:var(--muted)}
        .notice .actions{margin-top:8px}
        .notice .nclose{flex:none;width:22px;height:22px;margin:-3px -4px 0 0;border:0;border-radius:4px;background:transparent;color:var(--muted);font-size:15px;line-height:1;cursor:pointer}
        .notice .nclose:hover{background:var(--main);color:var(--ink)}
        @media (max-width:700px){.shell{grid-template-columns:1fr}aside{border-right:0;border-bottom:1px solid var(--line)}aside nav{flex-direction:row;flex-wrap:wrap}.who{display:none}.row{flex-direction:column;align-items:flex-start}}
        /* The counter's own window: its title bar is drawn here (drag to move, double click to maximize). */
        .tbar{display:none}
        body.windowed{overflow:hidden}
        body.windowed .tbar{display:flex;justify-content:flex-end;position:fixed;top:0;left:0;right:0;height:30px;background:#0d0d0d;border-bottom:1px solid var(--line);z-index:20;app-region:drag;user-select:none}
        body.windowed #scroll{position:absolute;top:30px;left:0;right:0;bottom:0;overflow:auto;min-height:0}
        body.windowed .sidein{height:calc(100vh - 30px - 24px)}
        body.windowed .brand{app-region:drag}
        .wctl{display:flex;app-region:no-drag}
        .wctl button{width:44px;height:29px;padding:0;border:0;border-radius:0;background:transparent;color:var(--muted);display:grid;place-items:center;cursor:pointer}
        .wctl button:hover{background:var(--main);color:var(--ink)}
        .wctl button.x:hover{background:#c42b1c;color:#fff}
        .wctl button:focus-visible{outline:1px solid var(--accent);outline-offset:-1px}
        .wctl svg{display:block}
        .sidein .who{margin-top:auto}
        #scroll::-webkit-scrollbar,html::-webkit-scrollbar,.log::-webkit-scrollbar{width:8px;height:8px}
        #scroll::-webkit-scrollbar-track,html::-webkit-scrollbar-track,.log::-webkit-scrollbar-track{background:transparent}
        #scroll::-webkit-scrollbar-thumb,html::-webkit-scrollbar-thumb,.log::-webkit-scrollbar-thumb{background:var(--line);border-radius:4px}
        #scroll::-webkit-scrollbar-thumb:hover,html::-webkit-scrollbar-thumb:hover{background:var(--line2)}        </style></head><body><div class="tbar"><div class="wctl">
          <button type="button" id="wmin"><svg width="10" height="10" viewBox="0 0 10 10" aria-hidden="true"><path d="M0 5.5h10" stroke="currentColor" stroke-width="1.1"/></svg></button>
          <button type="button" id="wmax"><svg width="10" height="10" viewBox="0 0 10 10" aria-hidden="true"><rect id="wmaxicon" x=".55" y=".55" width="8.9" height="8.9" fill="none" stroke="currentColor" stroke-width="1.1"/><path id="wresicon" d="M2.5 2.5V.55h6.95v6.95H7.5M.55 2.5h6.95v6.95H.55z" fill="none" stroke="currentColor" stroke-width="1.1" style="display:none"/></svg></button>
          <button type="button" id="wclose" class="x"><svg width="10" height="10" viewBox="0 0 10 10" aria-hidden="true"><path d="M.5.5l9 9M9.5.5l-9 9" stroke="currentColor" stroke-width="1.2"/></svg></button>
        </div></div><div id="scroll"><div class="shell"><aside><div class="sidein" id="side"></div></aside><div class="content"><main><div id="app"><p class="note">…</p></div><section id="reportPane" class="hidden"><iframe id="rep" title="Report" scrolling="no"></iframe></section></main></div></div><div id="foot"></div></div>
        <dialog id="dlg" aria-labelledby="dlgT"><div class="dhead"><h2 id="dlgT"></h2><button type="button" class="dclose" id="dlgX">×</button></div><div id="dlgB"></div></dialog>
        <script>
        const TOKEN='__TOKEN__';let S=null,tab=(()=>{const h=location.hash.slice(1);if(['home','report','overlay','settings','help'].includes(h))return h;try{return localStorage.getItem('cc-gui-tab')||'home'}catch(e){return 'home'}})(),estimate=null,stopped=false,copied=false;
        const $=s=>document.querySelector(s);
        // In the counter's own window (?w=1): the title bar of the page, its buttons sent to the window.
        const host=window.chrome&&window.chrome.webview;let maximized=false;
        if(host&&new URLSearchParams(location.search).get('w')==='1')document.body.classList.add('windowed');
        const scroller=()=>document.body.classList.contains('windowed')?document.getElementById('scroll'):document.scrollingElement;
        function winBtn(a){if(host)host.postMessage({win:a})}
        function winTitles(){if(!S)return;const T=S.txt;$('#wmin').title=T.wMin;$('#wmin').setAttribute('aria-label',T.wMin);const m=maximized?T.wRestore:T.wMax;$('#wmax').title=m;$('#wmax').setAttribute('aria-label',m);$('#wclose').title=T.wClose;$('#wclose').setAttribute('aria-label',T.wClose)}
        if(host)host.addEventListener('message',e=>{const d=e.data;if(d&&typeof d.maximized==='boolean'){maximized=d.maximized;$('#wmaxicon').style.display=maximized?'none':'';$('#wresicon').style.display=maximized?'':'none';winTitles()}});
        const esc=s=>String(s??'').replace(/[&<>"']/g,c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
        const fmt=(s,...a)=>a.reduce((r,v,i)=>r.replace('{'+i+'}',v),s);
        async function api(action,value){const r=await fetch('/api/action',{method:'POST',headers:{'X-Token':TOKEN,'Content-Type':'application/json'},body:JSON.stringify({action,value})});const j=await r.json();await refresh(true);return j.result}
        async function refresh(force){if(stopped)return;try{const r=await fetch('/api/state',{headers:{'X-Token':TOKEN}});S=await r.json();render(force)}catch(e){if(S){$('#app').innerHTML='<div class="card">'+esc(S.txt.offline)+'</div>'}}}
        function busy(){const a=document.activeElement;return a&&a.closest&&a.closest('#view')&&/INPUT|SELECT|TEXTAREA/.test(a.tagName)}
        function busyHeader(){const a=document.activeElement;return a&&a.closest&&a.closest('#side')&&a.tagName==='SELECT'}
        function sw(id,on,act){return `<label class="switch"><input type="checkbox" id="${id}" ${on?'checked':''} onchange="api('${act}',this.checked)"><span></span></label>`}
        function row(title,help,control){return `<div class="row"><div><div class="t">${title}</div>${help?`<div class="h">${help}</div>`:''}</div><div>${control}</div></div>`}
        function sel(act,value,opts){return `<select onchange="api('${act}',this.value)">${opts.map(o=>`<option value="${esc(o[0])}" ${String(o[0])===String(value)?'selected':''}>${esc(o[1])}</option>`).join('')}</select>`}
        function setTab(t){tab=t;try{localStorage.setItem('cc-gui-tab',t)}catch(e){}render(true)}
        const ICON={
          logo:'<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M4 10a4 4 0 0 1 4-4h8a4 4 0 0 1 4 4v9H4z"/><path d="M4 12h16"/><rect x="10.5" y="10.5" width="3" height="4" rx="1"/></svg>',
          home:'<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M3 11l9-7 9 7"/><path d="M5 10v10h14V10"/></svg>',
          report:'<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M4 20V10M10 20V4M16 20v-7M22 20H2"/></svg>',
          overlay:'<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><rect x="3" y="4" width="18" height="14" rx="2"/><rect x="12" y="7" width="6" height="4"/></svg>',
          settings:'<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><circle cx="12" cy="12" r="3"/><path d="M19.4 15a1.7 1.7 0 0 0 .3 1.8l.1.1a2 2 0 1 1-2.8 2.8l-.1-.1a1.7 1.7 0 0 0-1.8-.3 1.7 1.7 0 0 0-1 1.5V21a2 2 0 1 1-4 0v-.1a1.7 1.7 0 0 0-1.1-1.5 1.7 1.7 0 0 0-1.8.3l-.1.1a2 2 0 1 1-2.8-2.8l.1-.1a1.7 1.7 0 0 0 .3-1.8 1.7 1.7 0 0 0-1.5-1H3a2 2 0 1 1 0-4h.1a1.7 1.7 0 0 0 1.5-1.1 1.7 1.7 0 0 0-.3-1.8l-.1-.1a2 2 0 1 1 2.8-2.8l.1.1a1.7 1.7 0 0 0 1.8.3H9a1.7 1.7 0 0 0 1-1.5V3a2 2 0 1 1 4 0v.1a1.7 1.7 0 0 0 1 1.5 1.7 1.7 0 0 0 1.8-.3l.1-.1a2 2 0 1 1 2.8 2.8l-.1.1a1.7 1.7 0 0 0-.3 1.8V9a1.7 1.7 0 0 0 1.5 1H21a2 2 0 1 1 0 4h-.1a1.7 1.7 0 0 0-1.5 1z"/></svg>',
          help:'<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><circle cx="12" cy="12" r="9"/><path d="M9.5 9a2.5 2.5 0 0 1 4.9.8c0 1.7-2.4 2.2-2.4 3.7"/><path d="M12 17h.01"/></svg>'};
        // A groupbox: a titled panel, the building block of the page.
        function gb(title,body,right){return `<section class="gb"><div class="gbh"><h2>${title}</h2>${right||''}</div><div class="gbb">${body}</div></section>`}
        function tile(cls,label,value,sub,extra){return `<div class="tile ${cls}">${extra||''}<div class="th">${label}</div><div class="v num">${value}</div>${sub?`<div class="l">${sub}</div>`:''}</div>`}
        function render(force){
          const T=S.txt;document.documentElement.lang=S.lang;
          const side=`<div class="brand"><span class="logo">${ICON.logo}</span><span>${esc(S.appName)}</span></div>
          <nav>${['home','report','overlay','settings','help'].map(t=>`<button class="${t===tab?'on':''}" onclick="setTab('${t}')">${ICON[t]}<span>${esc(T[t])}</span></button>`).join('')}</nav>
          <div class="who">${S.profiles.length>1?`<select aria-label="${esc(T.profile)}" title="${esc(T.profile)}" onchange="api('profile',this.value)">${S.profiles.map(p=>`<option value="${esc(p.key)}" ${p.key===S.profileKey?'selected':''}>${esc(p.name)} (${esc(p.matches)})</option>`).join('')}</select>`:`<span>${esc(S.player||'')}</span>`}</div>`;
          const head=`<header><div class="crumb">${esc(S.appName)} / <b>${esc(T[tab])}</b></div><span class="pill ${S.status.kind}"><i></i>${esc(S.status.text)}</span></header>`;
          if(!force&&busyHeader())return;
          $('#side').innerHTML=side;
          if(!force&&busy()){$('#hdr').innerHTML=head;return}
          let v='';
          if(tab==='home')v=home(T);else if(tab==='overlay')v=overlay(T);else if(tab==='settings')v=settings(T);else if(tab==='help')v=help(T);
          $('#app').innerHTML=`<div id="hdr">${head}</div>${notices(T)}<div id="view">${v}</div>`;
          // In the counter's own window the close button asks and stops it: no "quit" button needed.
          $('#foot').innerHTML=`<footer><span>version: ${esc(S.version)}</span><span>·</span><span>${esc(S.settings.windowed?T.footerWindow:T.footer)}</span>${S.settings.windowed?'':`<button class="b danger" onclick="quit()">${esc(T.quit)}</button>`}</footer>`;
          showReport();winTitles();
        }
        // The report, in the window: loaded once, then again only when the counter rewrote it (new match, setting...).
        let repStamp=null,repObs=null;
        // Height of the report's content (not of its frame: it shrinks again when a list is folded).
        function fitReport(){const f=$('#rep');try{const m=f.contentDocument.querySelector('main');f.style.height=Math.ceil(m.getBoundingClientRect().height)+'px'}catch(e){}}
        function showReport(){
          const pane=$('#reportPane'),f=$('#rep');pane.classList.toggle('hidden',tab!=='report');
          if(tab!=='report'||S.reportStamp===repStamp)return;
          const y=scroller().scrollTop;repStamp=S.reportStamp;
          f.onload=()=>{fitReport();scroller().scrollTop=y;try{if(repObs)repObs.disconnect();repObs=new ResizeObserver(fitReport);repObs.observe(f.contentDocument.querySelector('main'))}catch(e){}};
          f.src='/report?t='+TOKEN+'&v='+encodeURIComponent(repStamp);
        }
        // Messages of content.json (a known problem, a fix to install...): above every tab.
        function notices(T){return (S.notices||[]).map(n=>`<div class="card notice ${n.level==='warning'?'warning':''}" role="${n.level==='warning'?'alert':'status'}"><span class="ni" aria-hidden="true">${n.level==='warning'?'!':'i'}</span><div class="nb">${n.title?`<div class="nt">${esc(n.title)}</div>`:''}<div class="nx">${esc(n.text)}</div>${n.link?`<div class="actions"><button class="b ${n.level==='warning'?'main':''}" onclick="window.open('${esc(n.link)}','_blank')">${esc(T.moreInfo)}</button></div>`:''}</div>${n.level==='info'?`<button class="nclose" title="${esc(T.close)}" aria-label="${esc(T.close)}" onclick="api('dismissNotice','${esc(n.id)}')">×</button>`:''}</div>`).join('')}
        function home(T){
          let h='';
          if(S.autoClose)h+=`<div class="card notice warning"><span class="ni" aria-hidden="true">!</span><div class="nb"><div class="nt">${esc(S.status.text)}</div><div class="nx">${esc(T.autoCloseText)}</div><div class="actions"><button class="b main" onclick="api('keepOpen')">${esc(T.keepOpen)}</button></div></div></div>`;
          if(S.firstRun){const s=S.settings;h+=gb(esc(T.welcome),`<p>${esc(T.welcomeText)}</p>
            <div class="note" style="margin:10px 0 2px">${esc(T.welcomeQuick)}</div>
            ${row(esc(T.language),'',sel('language',s.language,s.languages))}
            ${row(esc(T.inGame),esc(T.inGameHelp),sw('wov',S.overlay.show,'showOverlay'))}
            ${s.windowed?row(esc(T.tray),esc(T.trayHelp),sw('wtr',s.minimizeToTray,'minimizeToTray')):''}
            <div class="actions" style="margin-top:8px"><button class="b main" onclick="api('dismissWelcome')">${esc(T.welcomeDone)}</button></div>`)}
          if(S.progress)h+=gb(`${esc(T.analyzing)} <span class="num">${S.progress.done} / ${S.progress.total}</span>`,`<div class="bar" style="margin:0"><i style="width:${Math.round(100*S.progress.done/Math.max(1,S.progress.total))}%"></i></div>`);
          if(S.whatsNew&&S.whatsNew.length)h+=gb(esc(fmt(T.whatsNew,S.version)),`<ul>${S.whatsNew.map(n=>`<li>${esc(n)}</li>`).join('')}</ul><button class="b main" onclick="api('dismissWhatsNew')">${esc(T.ok)}</button>`);
          if(S.update)h+=gb(esc(fmt(T.newVersion,S.update.version)),`<ul>${S.update.notes.map(n=>`<li>${esc(n)}</li>`).join('')}</ul><button class="b main" onclick="api('installUpdate')">${esc(T.install)}</button>`);
          h+=`<section class="tiles">${tile('hero',esc(T.total),esc(S.total),esc(T.totalSub)+(S.manual?'<br>'+esc(fmt(T.byHand,S.manual)):''),`<button type="button" class="editbtn" onclick="openManual()" title="${esc(T.editChests)}" aria-label="${esc(T.editChests)}">__PENCIL__</button>`)}
            ${tile('',esc(T.session),esc(S.session),esc(S.sessionMatches)+' '+esc(T.matches))}
            ${tile('',esc(T.last),esc(S.last?S.last.chests:'—'),S.last?esc(S.last.when)+' · '+esc(S.last.mode):'')}
            ${S.squad?tile('',esc(T.squad)+`<span class="badge" title="${esc(T.inaccurateWhy)}">${esc(T.inaccurate)}</span>`,esc(S.squad.total),esc(fmt(T.squadSub,S.squad.team))):''}
            ${tile('',esc(T.treasures),'★ '+esc(S.treasures),esc(S.matches)+' '+esc(T.matches)+' · '+esc(fmt(T.perMatch,S.average)))}</section>`;
          let a=`<div class="actions"><button class="b main" onclick="setTab('report')">${esc(T.openReport)}</button>`;
          if(S.overlay.running)a+=`<button class="b" onclick="api('toggleOverlayVisible')">${esc(S.overlay.visible?T.hideOverlay:T.showOverlay)}</button><button class="b" onclick="api('toggleOverlayLock')">${esc(S.overlay.locked?T.unlock:T.lock)}</button>`;
          if(!S.update&&S.updatesOn)a+=`<button class="b" onclick="api('checkUpdate')">${esc(T.checkUpdate)}</button>`;
          a+='</div>';
          h+=a+gb(esc(T.activity),`<div class="log">${S.activity.length?S.activity.map(l=>l.map(s=>`<span class="${s.c}">${esc(s.t)}</span>`).join('')).join('\n'):esc(T.noActivity)}</div>`);
          return h;
        }
        function overlay(T){
          const o=S.overlay;
          return `<div class="cols">${gb(esc(T.inGame),`${row(esc(T.inGame),esc(T.inGameHelp),sw('ov',o.show,'showOverlay'))}
            ${row(esc(T.style),'',sel('overlayStyle',o.style,[['hud',T.hud],['classic',T.classic]]))}
            ${o.show?row(esc(T.size),'',sel('overlaySize',o.size,o.sizes.map(x=>[x,x+' %'])))+row(esc(T.opacity),'',sel('overlayOpacity',o.opacity,o.opacities.map(x=>[x,x+' %']))):''}
            ${row(esc(T.squadToggle)+`<span class="badge" title="${esc(T.inaccurateWhy)}">${esc(T.inaccurate)}</span>`,esc(T.squadHelp)+'<br>'+esc(T.inaccurateWhy),sw('sq',o.squad,'showSquadTotal'))}`)}
            ${gb(esc(T.obs),`<span class="path">${esc(S.paths.overlay)}</span><p class="note">${esc(T.obsSteps)}</p>
            <div class="actions"><button class="b main" onclick="copyObs()">${esc(copied?T.copied:T.copy)}</button><button class="b" onclick="window.open('/overlay?t='+TOKEN,'_blank')">${esc(T.preview)}</button></div>`)}</div>`;
        }
        function settings(T){
          const s=S.settings;
          return gb(esc(T.settings),`${row(esc(T.language),'',sel('language',s.language,s.languages))}
            ${S.profiles.length>1?row(esc(T.profile),'',sel('profile',S.profileKey,S.profiles.map(p=>[p.key,p.name+' ('+p.matches+' · '+p.chests+')']))):''}
            ${s.windowed?row(esc(T.tray),esc(T.trayHelp),sw('tr',s.minimizeToTray,'minimizeToTray')):''}
            ${s.canStartWithFortnite?row(esc(T.withFortnite),esc(T.withFortniteHelp),sw('wf',s.startWithFortnite,'startWithFortnite')):''}
            ${row(esc(T.updates),'',sel('updateMode',s.updateMode,[['prompt',T.upPrompt],['auto',T.upAuto],['off',T.upOff]]))}
            <div class="row"><div style="flex:1"><div class="t">${esc(T.demos)}</div>${s.demosExists?'':`<div class="h">${esc(T.demosMissing)}</div>`}<div class="actions" style="margin-top:6px"><input type="text" id="demos" value="${esc(s.demos)}"><button class="b" onclick="api('demosFolder',$('#demos').value)">${esc(T.save)}</button></div></div></div>`);
        }
        // Chests added by hand (and the estimate from the Career matches): p = 's' in the settings, 'd' in the edit box.
        function manualForm(T,p){
          return `<p class="note">${esc(T.manualHelp)}</p>
            <div class="actions"><input type="number" id="${p}manual" value="${S.manual}" step="1" aria-label="${esc(T.manual)}"><button class="b main" onclick="saveManual('${p}',parseInt($('#${p}manual').value||'0',10))">${esc(T.save)}</button></div>
            <div class="t" style="margin-top:14px">${esc(T.estimate)}</div><p class="note">${esc(T.estimateHelp)}</p>
            <div class="actions"><input type="number" id="${p}career" min="0" placeholder="${esc(T.career)}" aria-label="${esc(T.career)}"><button class="b" onclick="doEstimate('${p}')">${esc(T.compute)}</button></div>
            <div class="estimate" id="${p}est">${estimate&&estimate.p===p?estimateHtml(T,p):''}</div>`;
        }
        function estimateHtml(T,p){if(estimate.error)return esc(estimate.error);return esc(estimate.text)+`<div class="actions" style="margin-top:8px"><button class="b main" onclick="saveManual('${p}',${estimate.chests})">${esc(fmt(T.add,estimate.chestsText))}</button></div>`}
        async function doEstimate(p){const r=await api('estimate',parseInt($('#'+p+'career').value||'0',10));estimate=Object.assign({p},r);const e=$('#'+p+'est');if(e)e.innerHTML=estimateHtml(S.txt,p)}
        async function saveManual(p,n){estimate=null;await api('manualChests',n);if(p==='d')$('#dlg').close();else render(true)}
        // The edit box of "chests in total" (home tab, and the report's pencil through a message).
        function openManual(){const T=S.txt,d=$('#dlg');estimate=null;$('#dlgT').textContent=T.editChests;$('#dlgB').innerHTML=manualForm(T,'d');$('#dlgX').setAttribute('aria-label',T.close);if(!d.open)d.showModal();const i=$('#dmanual');if(i){i.focus();i.select()}}
        $('#dlgX').onclick=()=>$('#dlg').close();
        $('#wmin').onclick=()=>winBtn('min');$('#wmax').onclick=()=>winBtn('max');$('#wclose').onclick=()=>winBtn('close');
        $('#dlg').addEventListener('click',e=>{if(e.target.id==='dlg')$('#dlg').close()});
        window.addEventListener('message',e=>{if(e.origin===location.origin&&e.data&&e.data.cc==='editChests'&&S)openManual()});
        async function copyObs(){await api('copyObsPath');copied=true;render(true);setTimeout(()=>{copied=false;render(true)},2000)}
        function help(T){
          return `<div class="cols">${gb(esc(T.help),`<p>${esc(T.helpText)}</p><div class="actions"><button class="b" onclick="api('openDataFolder')">${esc(T.dataFolder)}</button>${S.paths.errorLog?`<button class="b" onclick="api('openErrorLog')">${esc(T.errorLog)}</button>`:''}<button class="b" onclick="api('openIssues')">${esc(T.issue)}</button><button class="b" onclick="api('openRepo')">${esc(T.project)}</button></div>`)}
          ${S.canUninstall?gb(esc(T.uninstall),`<span class="path">${esc(S.paths.data)}</span>
          <div class="actions"><button class="b danger" onclick="uninstall(false)">${esc(T.uninstall)} · ${esc(T.keepData)}</button><button class="b danger" onclick="uninstall(true)">${esc(T.uninstall)} · ${esc(T.deleteData)}</button></div>`):''}</div>`;
        }
        function closedView(){stopped=true;$('#app').innerHTML='<div class="card">'+esc(S.txt.closed)+'</div>';$('#foot').innerHTML='';$('#reportPane').classList.add('hidden')}
        async function uninstall(all){if(!confirm(S.txt.uninstallConfirm))return;await api('uninstall',all);closedView()}
        async function quit(){if(!confirm(S.txt.quitConfirm))return;try{await api('quit')}catch(e){}closedView()}
        refresh(true);setInterval(()=>refresh(false),2000);
        </script></body></html>
        """;
}
