"use strict";

var connection = null;

function iniciarConexion(token) {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatsegurohub", { accessTokenFactory: () => token })//pasar el token
        .build();

    connection.on("ReceiveMessage", function (mje) {
        console.log("Mensaje recibido:", mje);
        var msg = mje.cuerpo.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        var encodedMsg;
        var li = document.createElement("li");
        encodedMsg = `${mje.emisor} a ${mje.destinatario ? mje.destinatario : 'todos'}: ${msg}`;
        if (mje.destinatario) {
            li.style.color = "red";
        }
        li.textContent = encodedMsg;
        document.getElementById("messagesList").appendChild(li);
    });

    connection.on("UsuarioConectado", function (user) {
        console.log("Conectado:", user);
        var option = document.createElement("option");
        option.id = "u_" + user.usuario;
        option.value = user.usuario;
        option.textContent = user.nombre;
        document.getElementById("destinoInput").appendChild(option);
    });

    connection.on("UsuarioDesconectado", function (user) {
        console.log("Desconectado:", user);
        var option = document.getElementById("u_" + user.usuario);
        document.getElementById("destinoInput").removeChild(option);
    });

    connection.start().then(function () {
        document.getElementById("divLogin").style.display = "none";
        document.getElementById("divChat").style.display = "block";
    }).catch(function (err) {
        return console.error(err.toString());
    });
}

document.getElementById("loginButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var pass = document.getElementById("passInput").value;
    $.post(`/api/propietarios/login`, {
        Usuario: user,
        Clave: pass,
    }).done((e) => {
        console.log("Token:", e);
        iniciarConexion(e);
    });
    event.preventDefault();
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("destinoInput").value;
    var message = document.getElementById("messageInput").value;
    var mje = {
        Cuerpo: message,
        Destinatario: user,
	}
    connection.invoke("SendMessage", mje).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});