'use strict';

// The following sample code uses modern ECMAScript 6 features
// that aren't supported in Internet Explorer 11.

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/scraperHub")
    .build();

connection.on("ShowFound", (show) => {
    const message = "[" + show.id + "] " + show.name + " (" + show.castCount + ")";
    const msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    const li = document.createElement("li");
    li.textContent = msg;
    const list = document.getElementById("messagesList");
    list.appendChild(li);

    // make sure list doesn't get too long
    if (list.children.length > 25) {
        list.children[0].parentNode.removeChild(list.children[0]);
    }

    // update "val" value when higher
    var oldmax = document.getElementById("val").value;
    if (show.id > oldmax) {
        document.getElementById("val").value = show.id;
    }
});

connection.start().catch(err => console.error(err.toString()));

async function startScraping(val) {
    console.log("Start scraping from " + val);

    await fetch("api/scraper/start?showId=" + val);
    // connection.invoke("StartScraping", val).catch(err => console.error(err.toString()));
}

async function stopScraping() {
    console.log("Stopping the scraping");
    await fetch("api/scraper/stop");

    // connection.invoke("StopScraping").catch(err => console.error(err.toString()));
}

async function startFrom(val) {
    console.log("start from: " + val);
    if (!val || val <= 0) {
        val = 1;
    }

    await startScraping(val);
}

async function startFromEnd() {
    var val = document.getElementById("max").value;
    await startFrom(val);
}

async function startFromVal() {
    var val = document.getElementById("val").value;
    await startFrom(val);
}
