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
        ["tiktok"] = L.T("TikTok LIVE Studio (fenêtre)", "TikTok LIVE Studio (window)", "TikTok LIVE Studio (ventana)"),
        ["streamWin"] = L.T("Fenêtre de stream", "Stream window", "Ventana de directo"),
        ["streamHelp"] = L.T("Ouvre le compteur dans une fenêtre normale. Dans TikTok LIVE Studio : + Ajouter une source → Fenêtre (capture de fenêtre) → « Chest Addiction Counter – Stream ». Agrandis la fenêtre pour un texte plus net. Elle peut rester derrière le jeu, mais pas réduite.",
            "Opens the counter in a normal window. In TikTok LIVE Studio: + Add source → Window (window capture) → \"Chest Addiction Counter – Stream\". Make the window bigger for sharper text. It can stay behind the game, but not minimized.",
            "Abre el contador en una ventana normal. En TikTok LIVE Studio: + Añadir fuente → Ventana (captura de ventana) → «Chest Addiction Counter – Stream». Agranda la ventana para un texto más nítido. Puede quedar detrás del juego, pero no minimizada."),
        ["streamBg"] = L.T("Fond", "Background", "Fondo"),
        ["bgDark"] = L.T("Sombre (carte)", "Dark (card)", "Oscuro (tarjeta)"),
        ["bgGreen"] = L.T("Vert (chroma key)", "Green (chroma key)", "Verde (chroma key)"),
        ["streamBgHelp"] = L.T("Vert : ajoute un filtre Chroma key (vert) à la source pour ne garder que les chiffres.",
            "Green: add a Chroma key (green) filter to the source to keep only the figures.",
            "Verde: añade un filtro Chroma key (verde) a la fuente para dejar solo las cifras."),
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
        ["countExtras"] = L.T("Compter aussi", "Also count", "Contar también"),
        ["countAmmo"] = L.T("Caisses de munitions", "Ammo boxes", "Cajas de munición"),
        ["countCannons"] = L.T("Canons à butin", "Loot cannons", "Cañones de botín"),
        ["countFood"] = L.T("Caisses de nourriture", "Food boxes", "Cajas de comida"),
        ["countExtrasHelp"] = L.T("Ajoute les caisses de munitions, les canons à butin et les caisses de nourriture au total, à la session et à la dernière partie (aussi dans l'overlay et le rapport). Les chances de trésor restent calculées sur les vrais coffres.",
            "Adds ammo boxes, loot cannons and food boxes to the total, the session and the last match (overlay and report too). Treasure odds stay based on real chests only.",
            "Suma las cajas de munición, los cañones de botín y las cajas de comida al total, la sesión y la última partida (también en el overlay y el informe). Las probabilidades de tesoro siguen contando solo cofres reales."),
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
        ["upstream"] = L.T("Projet d'origine (LED-N)", "Original project (LED-N)", "Proyecto original (LED-N)"),
        ["upstreamHelp"] = L.T("Chest Addiction Counter est un fork de Fortnite Chest Counter de LED-N : le comptage et les données des trésors viennent de ce projet.",
            "Chest Addiction Counter is a fork of Fortnite Chest Counter by LED-N: the counting and the treasure data come from that project.",
            "Chest Addiction Counter es un fork de Fortnite Chest Counter de LED-N: el conteo y los datos de los tesoros vienen de ese proyecto."),
        ["project"] = L.T("Page du projet (GitHub)", "Project page (GitHub)", "Página del proyecto (GitHub)"),
        ["helpText"] = L.T("Le compteur lit les replays que Fortnite enregistre et compte tes coffres quelques secondes après chaque partie (Battle Royale de la saison seulement). Laisse-le ouvert pendant que tu joues : tu peux réduire la fenêtre, il continue de compter.",
            "The counter reads the replays Fortnite saves and counts your chests a few seconds after each match (the season's Battle Royale only). Keep it open while you play: you can minimize the window, it keeps counting.",
            "El contador lee las repeticiones que guarda Fortnite y cuenta tus cofres unos segundos después de cada partida (solo el Battle Royale de la temporada). Déjalo abierto mientras juegas: puedes minimizar la ventana, sigue contando."),
        ["uninstall"] = L.T("Désinstaller", "Uninstall", "Desinstalar"),
        ["keepData"] = L.T("Garder mon historique et mes réglages", "Keep my history and settings", "Conservar mi historial y mis ajustes"),
        ["deleteData"] = L.T("Tout supprimer", "Delete everything", "Borrarlo todo"),
        ["uninstallConfirm"] = L.T("Désinstaller maintenant ? Le compteur se ferme et ChestAddictionCounter.exe est supprimé.", "Uninstall now? The counter closes and ChestAddictionCounter.exe is deleted.", "¿Desinstalar ahora? El contador se cierra y se borra ChestAddictionCounter.exe."),
        ["quit"] = L.T("Quitter le compteur", "Quit the counter", "Salir del contador"),
        ["quitConfirm"] = L.T("Arrêter le compteur ? Les parties jouées ensuite ne seront comptées qu'à sa prochaine ouverture.", "Stop the counter? Matches played afterwards are only counted when it is opened again.", "¿Detener el contador? Las partidas jugadas después solo se cuentan cuando se vuelva a abrir."),
        ["closed"] = L.T("Le compteur est arrêté. Tu peux fermer cette fenêtre.", "The counter is stopped. You can close this window.", "El contador está detenido. Puedes cerrar esta ventana."),
        ["offline"] = L.T("Le compteur ne répond plus (fermé ?). Relance ChestAddictionCounter.exe.", "The counter does not answer (closed?). Start ChestAddictionCounter.exe again.", "El contador no responde (¿cerrado?). Vuelve a abrir ChestAddictionCounter.exe."),
        ["footer"] = L.T("Fermer cette fenêtre ne coupe pas le compteur.", "Closing this window does not stop the counter.", "Cerrar esta ventana no detiene el contador."),
    };

    public static string Html(string token) => Template.Replace("__TOKEN__", token);

    private const string Template = """
        <!doctype html><html><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1">
        <title>Chest Addiction Counter</title>
        <script src="/ui/tailwind.js"></script>
        <script src="/ui/obsidian.js"></script>
        <style>
        @font-face{font-family:"IBM Plex Mono";font-weight:400;font-display:swap;src:url(/ui/plex-400.woff2) format("woff2")}
        @font-face{font-family:"IBM Plex Mono";font-weight:500;font-display:swap;src:url(/ui/plex-500.woff2) format("woff2")}
        :root{color-scheme:dark;--mono:"IBM Plex Mono","Cascadia Mono",Consolas,monospace}
        html,body{margin:0;height:100%;background:#0d0d0d;color:#fff;font:12px/1.45 var(--mono);overflow:hidden}
        .no-scrollbar{-ms-overflow-style:none;scrollbar-width:none}.no-scrollbar::-webkit-scrollbar{width:0;height:0;display:none}
        .obsidian-theme-container,.obsidian-theme-container *{font-family:var(--mono)!important}
        ::-webkit-scrollbar{width:6px;height:6px}::-webkit-scrollbar-track{background:transparent}::-webkit-scrollbar-thumb{background:#282828;border-radius:3px}
        #wrap{position:fixed;inset:0}
        obsidian-widget,.obsidian-widget-host,.obsidian-widget-host>div{display:block;width:100%;height:100%}
        #rep{position:fixed;border:0;display:none;background:#0d0d0d;z-index:5}
        #msg{position:fixed;inset:0;display:none;place-items:center;background:#0d0d0d;z-index:30;color:rgba(255,255,255,.7);padding:24px;text-align:center}
        #msg.on{display:grid}
        /* The counter's own window: its title bar is drawn here (drag to move, double click to maximize). */
        .tbar{display:none}
        body.windowed .tbar{display:flex;justify-content:space-between;align-items:center;position:fixed;top:0;left:0;right:0;height:30px;background:#0d0d0d;border-bottom:1px solid #282828;z-index:20;app-region:drag;user-select:none}
        body.windowed #wrap{top:30px}
        .tname{padding-left:12px;color:rgba(255,255,255,.5)}
        .wctl{display:flex;app-region:no-drag}
        .wctl button{width:44px;height:29px;padding:0;border:0;background:transparent;color:rgba(255,255,255,.6);display:grid;place-items:center;cursor:pointer}
        .wctl button:hover{background:#191919;color:#fff}
        .wctl button.x:hover{background:#c42b1c;color:#fff}
        .wctl svg{display:block}
        /* The edit box of "chests in total", drawn like an Obsidian groupbox. */
        dialog{width:min(480px,calc(100% - 32px));padding:0;border:1px solid #282828;border-radius:4px;background:#0f0f0f;color:#fff;font:12px/1.45 var(--mono);box-shadow:0 20px 60px rgba(0,0,0,.7)}
        dialog::backdrop{background:rgba(0,0,0,.6)}
        .dhead{display:flex;align-items:center;justify-content:space-between;min-height:34px;padding:0 6px 0 10px;border-bottom:1px solid #282828}
        .dhead h2{margin:0;font-size:13px;font-weight:400}
        #dlgB{padding:10px}
        .dclose{width:26px;height:26px;border:0;border-radius:4px;background:transparent;color:rgba(255,255,255,.6);font-size:16px;cursor:pointer}
        .dclose:hover{background:#191919;color:#fff}
        .note{color:rgba(255,255,255,.5);margin:4px 0 8px}
        .t{margin-top:12px}
        .actions{display:flex;flex-wrap:wrap;gap:6px}
        .b{min-height:23px;padding:2px 12px;border-radius:4px;border:1px solid #282828;background:#191919;color:rgba(255,255,255,.8);font:12px var(--mono);cursor:pointer}
        .b:hover{filter:brightness(1.25)}
        .b.main{border-color:#7d55ff;color:#fff}
        input[type=number]{min-height:23px;width:150px;padding:2px 8px;border-radius:4px;border:1px solid #282828;background:#191919;color:#fff;font:12px var(--mono);outline:none}
        input[type=number]:focus{border-color:#7d55ff}
        .estimate{margin-top:8px}
        </style></head><body><div class="tbar"><span class="tname" id="tname"></span><div class="wctl">
          <button type="button" id="wmin"><svg width="10" height="10" viewBox="0 0 10 10" aria-hidden="true"><path d="M0 5.5h10" stroke="currentColor" stroke-width="1.1"/></svg></button>
          <button type="button" id="wmax"><svg width="10" height="10" viewBox="0 0 10 10" aria-hidden="true"><rect id="wmaxicon" x=".55" y=".55" width="8.9" height="8.9" fill="none" stroke="currentColor" stroke-width="1.1"/><path id="wresicon" d="M2.5 2.5V.55h6.95v6.95H7.5M.55 2.5h6.95v6.95H.55z" fill="none" stroke="currentColor" stroke-width="1.1" style="display:none"/></svg></button>
          <button type="button" id="wclose" class="x"><svg width="10" height="10" viewBox="0 0 10 10" aria-hidden="true"><path d="M.5.5l9 9M9.5.5l-9 9" stroke="currentColor" stroke-width="1.2"/></svg></button>
        </div></div>
        <div id="wrap"><obsidian-widget id="ob" width="100%" height="100%"></obsidian-widget></div>
        <iframe id="rep" title="Report"></iframe><div id="msg"></div>
        <dialog id="dlg" aria-labelledby="dlgT"><div class="dhead"><h2 id="dlgT"></h2><button type="button" class="dclose" id="dlgX">×</button></div><div id="dlgB"></div></dialog>
        <script>
        // The window is the Obsidian UI widget (mspaint): the page builds its data (tabs, groupboxes, elements) from the
        // counter's state, and reads back what the player clicks or changes in it (the widget has no callbacks).
        const TOKEN='__TOKEN__';let S=null,estimate=null,stopped=false,copied=false;
        let epoch=1,uid=0,lastJson='',binds=[],curTab='',savedTab=null,restored=false;
        try{savedTab=localStorage.getItem('cc-gui-tab2')}catch(e){}
        const $=s=>document.querySelector(s),ob=$('#ob');
        // In the counter's own window (?w=1): the title bar of the page, its buttons sent to the window.
        const host=window.chrome&&window.chrome.webview;let maximized=false;
        if(host&&new URLSearchParams(location.search).get('w')==='1')document.body.classList.add('windowed');
        function winBtn(a){if(host)host.postMessage({win:a})}
        function winTitles(){if(!S)return;const T=S.txt;$('#wmin').title=T.wMin;const m=maximized?T.wRestore:T.wMax;$('#wmax').title=m;$('#wclose').title=T.wClose}
        if(host)host.addEventListener('message',e=>{const d=e.data;if(d&&typeof d.maximized==='boolean'){maximized=d.maximized;$('#wmaxicon').style.display=maximized?'none':'';$('#wresicon').style.display=maximized?'':'none';winTitles()}});
        const esc=s=>String(s??'').replace(/[&<>"']/g,c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
        const fmt=(s,...a)=>a.reduce((r,v,i)=>r.replace('{'+i+'}',v),s);
        async function api(action,value){const r=await fetch('/api/action',{method:'POST',headers:{'X-Token':TOKEN,'Content-Type':'application/json'},body:JSON.stringify({action,value})});const j=await r.json();await refresh(true);return j.result}
        async function refresh(force){if(stopped)return;try{const r=await fetch('/api/state',{headers:{'X-Token':TOKEN}});S=await r.json();$('#msg').classList.remove('on');render(force)}catch(e){if(S)showMsg(S.txt.offline)}}
        function showMsg(t){const m=$('#msg');m.textContent=t;m.classList.add('on')}

        // ---- Elements of the widget (and what each one does) ----
        const el=(type,text,props,extra)=>Object.assign({index:epoch*100000+(uid++),visible:true,type,text:String(text??''),disabled:false,properties:props||{}},extra||{});
        const tip=(e,t)=>{if(t)e.tooltip=String(t);return e};
        const L=(t,h)=>tip(el('Label',t,{doesWrap:true}),h);
        const DV=()=>el('Divider','',{marginTop:0,marginBottom:0});
        function TG(text,value,act,h){binds.push({k:'toggle',tab:curTab,text,act,value:!!value});return tip(el('Toggle',text,{risky:false,variant:'Switch'},{value:!!value}),h)}
        function BT(text,fn,risky,h){binds.push({k:'button',tab:curTab,text,fn});return tip(el('Button',text,{risky:!!risky,doubleClick:false}),h)}
        function DD(text,value,opts,act){const o=opts.map(x=>[x[0],String(x[1])]);const cur=(o.find(x=>String(x[0])===String(value))||o[0]||['',''])[1];binds.push({k:'dd',tab:curTab,text,act,value:cur,opts:o});return el('Dropdown',text,{values:o.map(x=>x[1]),disabledValues:[],multi:false,searchable:false},{value:cur})}
        function IN(text,value,act,ph){binds.push({k:'input',tab:curTab,text,act,value:String(value??'')});return el('Input',text,{placeholder:ph||'',finished:true,emptyReset:'',numeric:false,clearTextOnFocus:false,allowEmpty:true},{value:String(value??'')})}
        const SL=(text,value,max)=>el('Slider',text,{min:0,max:Math.max(1,max),rounding:0,prefix:'',suffix:'',compact:false,hideMax:false},{value,disabled:true});
        const G=(name,order,elements)=>({type:'Groupbox',name,order,side:'Unknown',elements:elements.filter(Boolean)});
        const box=list=>{const o={};list.filter(Boolean).forEach(g=>o[g.name]=g);return o};
        const TAB=(order,icon,left,right)=>({name:curTab,type:'Tab',order,icon,description:S.status.text,elements:[],tabboxes:{Left:[],Right:[],Unknown:[]},groupboxes:{Left:box(left),Right:box(right),Unknown:{}},warningBox:{Visible:false,Title:'',IsNormal:true,Text:'',LockSize:false}});
        const LOGO='data:image/svg+xml,'+encodeURIComponent('<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="#7d55ff" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M4 10a4 4 0 0 1 4-4h8a4 4 0 0 1 4 4v9H4z"/><path d="M4 12h16"/><rect x="10.5" y="10.5" width="3" height="4" rx="1"/></svg>');

        function homeTab(T){
          const left=[],right=[],s=S.settings;
          if(S.autoClose)left.push(G('⚠ '+S.status.text,0,[L(T.autoCloseText),BT(T.keepOpen,()=>api('keepOpen'))]));
          if(S.firstRun)left.push(G(T.welcome,1,[L(T.welcomeText),DV(),L(T.welcomeQuick),DD(T.language,s.language,s.languages,'language'),TG(T.inGame,S.overlay.show,'showOverlay',T.inGameHelp),s.windowed?TG(T.tray,s.minimizeToTray,'minimizeToTray',T.trayHelp):null,BT(T.welcomeDone,()=>api('dismissWelcome'))]));
          if(S.progress)left.push(G(T.analyzing,2,[SL(S.progress.done+' / '+S.progress.total,S.progress.done,S.progress.total)]));
          (S.notices||[]).forEach((n,i)=>left.push(G((n.level==='warning'?'⚠ ':'')+(n.title||T.moreInfo),3+i/100,[L(n.text),n.link?BT(T.moreInfo+(i?' '+(i+1):''),()=>window.open(n.link,'_blank')):null,n.level==='info'?BT(T.close+(i?' '+(i+1):''),()=>api('dismissNotice',n.id)):null])));
          left.push(G(T.total,10,[
            L(T.total+': '+S.total,T.totalSub+(S.manual?' · '+fmt(T.byHand,S.manual):'')),DV(),
            L(T.session+': '+S.session+' · '+S.sessionMatches+' '+T.matches),
            L(T.last+': '+(S.last?S.last.chests+' · '+S.last.when+' · '+S.last.mode:'—')),
            S.squad?L(T.squad+': '+S.squad.total+' ('+T.inaccurate+')',fmt(T.squadSub,S.squad.team)+'. '+T.inaccurateWhy):null,
            L(T.treasures+': ★ '+S.treasures+' · '+S.matches+' '+T.matches+' · '+fmt(T.perMatch,S.average)),DV(),
            BT(T.editChests,openManual),BT(T.openReport,()=>clickTab(T.report))]));
          if(S.whatsNew&&S.whatsNew.length)right.push(G(fmt(T.whatsNew,S.version),1,[...S.whatsNew.map(n=>L('• '+n)),BT(T.ok,()=>api('dismissWhatsNew'))]));
          if(S.update)right.push(G(fmt(T.newVersion,S.update.version),2,[...S.update.notes.map(n=>L('• '+n)),BT(T.install,()=>api('installUpdate'))]));
          const quick=[];
          if(S.overlay.running){quick.push(BT(S.overlay.visible?T.hideOverlay:T.showOverlay,()=>api('toggleOverlayVisible')));quick.push(BT(S.overlay.locked?T.unlock:T.lock,()=>api('toggleOverlayLock')))}
          if(!S.update&&S.updatesOn)quick.push(BT(T.checkUpdate,()=>api('checkUpdate')));
          if(quick.length)right.push(G(T.overlay,3,quick));
          right.push(G(T.activity,4,S.activity.length?S.activity.slice(-14).map(l=>L(l.map(x=>x.t).join(''))):[L(T.noActivity)]));
          return TAB(1,'House',left,right);
        }
        function overlayTab(T){
          const o=S.overlay;
          return TAB(3,'Layers',[G(T.inGame,1,[TG(T.inGame,o.show,'showOverlay',T.inGameHelp),DV(),
            DD(T.style,o.style,[['hud',T.hud],['classic',T.classic]],'overlayStyle'),
            o.show?DD(T.size,o.size,o.sizes.map(x=>[x,x+' %']),'overlaySize'):null,
            o.show?DD(T.opacity,o.opacity,o.opacities.map(x=>[x,x+' %']),'overlayOpacity'):null,DV(),
            TG(T.squadToggle+' ('+T.inaccurate+')',o.squad,'showSquadTotal',T.squadHelp+' '+T.inaccurateWhy)])],
            [G(T.obs,1,[L(S.paths.overlay,T.obsSteps),BT(copied?T.copied:T.copy,copyObs,false,T.obsSteps),BT(T.preview,()=>window.open('/overlay?t='+TOKEN,'_blank'))]),
             G(T.tiktok,2,[TG(T.streamWin,o.stream,'showStreamWindow',T.streamHelp),o.stream?tip(DD(T.streamBg,o.streamBg,[['dark',T.bgDark],['green',T.bgGreen]],'streamBackground'),T.streamBgHelp):null])]);
        }
        function settingsTab(T){
          const s=S.settings;
          return TAB(4,'Settings',[G(T.settings,1,[DD(T.language,s.language,s.languages,'language'),
            S.profiles.length>1?DD(T.profile,S.profileKey,S.profiles.map(p=>[p.key,p.name+' ('+p.matches+' · '+p.chests+')']),'profile'):null,
            DD(T.updates,s.updateMode,[['prompt',T.upPrompt],['auto',T.upAuto],['off',T.upOff]],'updateMode'),DV(),

            s.windowed?TG(T.tray,s.minimizeToTray,'minimizeToTray',T.trayHelp):null,
            s.canStartWithFortnite?TG(T.withFortnite,s.startWithFortnite,'startWithFortnite',T.withFortniteHelp):null])],
            [G(T.countExtras,0,[TG(T.countAmmo,s.countAmmoBoxes,'countAmmoBoxes',T.countExtrasHelp),TG(T.countCannons,s.countLootCannons,'countLootCannons',T.countExtrasHelp),TG(T.countFood,s.countFoodBoxes,'countFoodBoxes',T.countExtrasHelp)]),G(T.demos,1,[IN(T.demos,s.demos,'demosFolder'),s.demosExists?null:L(T.demosMissing)]),G(T.manual,2,[BT(T.editChests,openManual,false,T.manualHelp)])]);
        }
        function helpTab(T){
          const right=[];
          if(S.canUninstall)right.push(G(T.uninstall,1,[L(S.paths.data),BT(T.keepData,()=>uninstall(false),true,T.uninstallConfirm),BT(T.deleteData,()=>uninstall(true),true,T.uninstallConfirm)]));
          if(!S.settings.windowed)right.push(G(T.quit,2,[L(T.footer),BT(T.quit,quit,true)]));
          return TAB(5,'CircleHelp',[G(T.help,1,[L(T.helpText),DV(),BT(T.dataFolder,()=>api('openDataFolder')),S.paths.errorLog?BT(T.errorLog,()=>api('openErrorLog')):null,BT(T.issue,()=>api('openIssues')),BT(T.project,()=>api('openRepo')),BT(T.upstream,()=>api('openUpstream'),false,T.upstreamHelp)])],right);
        }

        // ---- Drawing ----
        function busy(){const a=document.activeElement;if(a&&a.tagName==='INPUT'&&a.closest('obsidian-widget')&&a.getAttribute('aria-label')!=='Search')return true;return !!document.querySelector('obsidian-widget [aria-expanded="true"]')}
        function render(force){
          if(!force&&busy())return;
          const T=S.txt;document.documentElement.lang=S.lang;$('#tname').textContent=S.appName;
          binds=[];uid=0;const tabs={};
          curTab=T.home;tabs[curTab]=homeTab(T);
          curTab=T.report;tabs[curTab]=TAB(2,'ChartColumn',[],[]);
          curTab=T.overlay;tabs[curTab]=overlayTab(T);
          curTab=T.settings;tabs[curTab]=settingsTab(T);
          curTab=T.help;tabs[curTab]=helpTab(T);
          ob.setAttribute('title',S.appName);ob.setAttribute('icon',LOGO);
          ob.setAttribute('footer','version: '+S.version+(S.player?' · '+S.player:''));
          const json=JSON.stringify({tabs,metadata:{}});
          if(json!==lastJson){lastJson=json;ob.setAttribute('ui-data',json);setTimeout(checkDrift,80)}
          if(!restored){restored=true;setTimeout(()=>{if(savedTab)clickTab(savedTab);placeReport()},150)}
          placeReport();winTitles();
        }
        // Toggles keep their own state: when the counter changed one (hotkey, other window...), mount them again.
        let drifted=false;
        function checkDrift(){const p=panel(),t=activeTab();if(!p)return;const bad=binds.some(b=>b.k==='toggle'&&b.tab===t&&readToggle(p,b)!==null&&readToggle(p,b)!==b.value);if(bad&&!drifted){drifted=true;epoch++;lastJson='';render(true);setTimeout(()=>drifted=false,500)}}

        // ---- Reading the widget back ----
        const panel=()=>document.querySelector('obsidian-widget [role=tabpanel][data-state=active]');
        function activeTab(){const t=document.querySelector('obsidian-widget [role=tab][data-state=active]');return t?t.textContent.trim():null}
        function labelOf(p,text){return [...p.querySelectorAll('span')].find(s=>s.childElementCount===0&&s.textContent.trim()===text)}
        function readToggle(p,b){const s=labelOf(p,b.text);if(!s)return null;const tr=s.parentElement.querySelector('.rounded-full');return tr?(tr.getAttribute('style')||'').includes('accent'):null}
        function readDD(p,b){const s=labelOf(p,b.text);if(!s)return null;const v=s.parentElement.querySelector('button span');return v?v.textContent.trim():null}
        function readIN(p,b){const s=labelOf(p,b.text);if(!s)return null;const i=s.parentElement.querySelector('input')||s.parentElement.parentElement.querySelector('input');return i?i.value:null}
        function sync(){
          const p=panel(),t=activeTab();if(!p||!S)return;
          for(const b of binds){if(b.tab!==t)continue;
            if(b.k==='toggle'){const v=readToggle(p,b);if(v!==null&&v!==b.value){b.value=v;api(b.act,v)}}
            else if(b.k==='dd'){const v=readDD(p,b);if(v!==null&&v!==b.value){b.value=v;const o=b.opts.find(x=>x[1]===v);if(o)api(b.act,o[0])}}}
        }
        function commitInputs(){const p=panel(),t=activeTab();if(!p)return;for(const b of binds){if(b.k!=='input'||b.tab!==t)continue;const v=readIN(p,b);if(v!==null&&v!==b.value){b.value=v;api(b.act,v)}}}
        function clickTab(name){const t=[...document.querySelectorAll('obsidian-widget [role=tab]')].find(x=>x.textContent.trim()===name);if(!t)return;
          for(const type of ['pointerdown','mousedown','pointerup','mouseup','click'])t.dispatchEvent(new (type.startsWith('pointer')?PointerEvent:MouseEvent)(type,{bubbles:true,cancelable:true,button:0,pointerType:'mouse'}));
          setTimeout(afterTab,30)}
        function afterTab(){const t=activeTab();if(t){try{localStorage.setItem('cc-gui-tab2',t)}catch(e){}}placeReport();setTimeout(checkDrift,60)}
        // Any click (a dropdown option is gone from the page once clicked): read the widget back a moment later.
        document.addEventListener('pointerup',()=>setTimeout(()=>{sync();afterTab()},80),true);
        document.addEventListener('click',e=>{
          if(!e.target.closest||!e.target.closest('obsidian-widget'))return;
          const btn=e.target.closest('button');
          if(btn&&!btn.hasAttribute('aria-haspopup')&&!btn.closest('[role=tablist]')){const txt=btn.textContent.trim(),t=activeTab();const b=binds.find(x=>x.k==='button'&&x.tab===t&&x.text===txt);if(b){b.fn();return}}
          setTimeout(afterTab,60)});
        document.addEventListener('keyup',e=>{if(e.key===' '||e.key==='Enter')setTimeout(sync,80)});
        document.addEventListener('keydown',e=>{if(e.key==='Enter'&&e.target.closest&&e.target.closest('obsidian-widget'))commitInputs()});
        document.addEventListener('focusout',e=>{if(e.target.closest&&e.target.closest('obsidian-widget')&&e.target.tagName==='INPUT')setTimeout(commitInputs,0)});
        window.addEventListener('resize',placeReport);

        // ---- The report: shown over the content of the "Report" tab ----
        let repStamp=null;
        function placeReport(){
          const f=$('#rep');if(!S||activeTab()!==S.txt.report){f.style.display='none';return}
          const p=panel();if(!p){f.style.display='none';return}
          const r=p.getBoundingClientRect();Object.assign(f.style,{display:'block',left:r.left+'px',top:r.top+'px',width:r.width+'px',height:r.height+'px'});
          if(S.reportStamp===repStamp)return;repStamp=S.reportStamp;
          f.onload=()=>{try{const d=f.contentDocument;d.documentElement.style.overflow='auto';d.body.style.overflow='visible';d.body.style.background='#0d0d0d';d.querySelector('main').style.padding='10px'}catch(e){}};
          f.src='/report?t='+TOKEN+'&v='+encodeURIComponent(repStamp);
        }
        setInterval(()=>{if(S)placeReport()},500);

        // ---- Chests added by hand (and the estimate from the Career matches) ----
        function manualForm(T){
          return `<p class="note">${esc(T.manualHelp)}</p>
            <div class="actions"><input type="number" id="dmanual" value="${S.manual}" step="1" aria-label="${esc(T.manual)}"><button class="b main" onclick="saveManual(parseInt($('#dmanual').value||'0',10))">${esc(T.save)}</button></div>
            <div class="t">${esc(T.estimate)}</div><p class="note">${esc(T.estimateHelp)}</p>
            <div class="actions"><input type="number" id="dcareer" min="0" placeholder="${esc(T.career)}" aria-label="${esc(T.career)}"><button class="b" onclick="doEstimate()">${esc(T.compute)}</button></div>
            <div class="estimate" id="dest"></div>`;
        }
        function estimateHtml(T){if(estimate.error)return esc(estimate.error);return esc(estimate.text)+`<div class="actions" style="margin-top:8px"><button class="b main" onclick="saveManual(${estimate.chests})">${esc(fmt(T.add,estimate.chestsText))}</button></div>`}
        async function doEstimate(){const r=await api('estimate',parseInt($('#dcareer').value||'0',10));estimate=r||{};$('#dest').innerHTML=estimateHtml(S.txt)}
        async function saveManual(n){estimate=null;await api('manualChests',n);$('#dlg').close()}
        function openManual(){const T=S.txt,d=$('#dlg');estimate=null;$('#dlgT').textContent=T.editChests;$('#dlgB').innerHTML=manualForm(T);$('#dlgX').setAttribute('aria-label',T.close);if(!d.open)d.showModal();const i=$('#dmanual');if(i){i.focus();i.select()}}
        $('#dlgX').onclick=()=>$('#dlg').close();
        $('#dlg').addEventListener('click',e=>{if(e.target.id==='dlg')$('#dlg').close()});
        $('#wmin').onclick=()=>winBtn('min');$('#wmax').onclick=()=>winBtn('max');$('#wclose').onclick=()=>winBtn('close');
        window.addEventListener('message',e=>{if(e.origin===location.origin&&e.data&&e.data.cc==='editChests'&&S)openManual()});
        async function copyObs(){await api('copyObsPath');copied=true;render(true);setTimeout(()=>{copied=false;render(true)},2000)}
        function closedView(){stopped=true;$('#rep').style.display='none';showMsg(S.txt.closed)}
        async function uninstall(all){if(!confirm(S.txt.uninstallConfirm))return;await api('uninstall',all);closedView()}
        async function quit(){if(!confirm(S.txt.quitConfirm))return;try{await api('quit')}catch(e){}closedView()}
        refresh(true);setInterval(()=>refresh(false),2000);
        </script></body></html>
        """;
}
