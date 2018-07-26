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
    if (list.children.length > 25) {
        list.children[0].parentNode.removeChild(list.children[0]);
    }

    // todo update "val" value
    var oldmax = document.getElementById("val").attributes["value"].value;
    if (show.id > oldmax) {
        document.getElementById("val").attributes["value"].value = show.id;
    }
});

connection.start().catch(err => console.error(err.toString()));

function startScraping(val) {
    connection.invoke("StartScraping", val).catch(err => console.error(err.toString()));
}

function stopScraping() {
    connection.invoke("StopScraping").catch(err => console.error(err.toString()));
}

/*
document.getElementById("sendButton").addEventListener("click", event => {
    const user = document.getElementById("userInput").value;
    const message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(err => console.error(err.toString()));
    event.preventDefault();
});
*/
