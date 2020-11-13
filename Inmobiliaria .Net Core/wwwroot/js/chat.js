"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " dice " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.on("UsuarioConectado", function (connId) {
    console.log("Conectado:", connId);
});

connection.on("UsuarioDesconectado", function (connId) {
    console.log("Desconectado:", connId);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var connId = document.getElementById("connIdInput").value;
    var message = document.getElementById("messageInput").value;
    if (connId) {
        connection.invoke("SendMP", user, message, connId).catch(function (err) {
            return console.error(err.toString());
        });
    } else {
        connection.invoke("SendMessage", user, message).catch(function (err) {
            return console.error(err.toString());
        });
	}
    event.preventDefault();
});