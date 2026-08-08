"use strict";

var connection = null;
var usuario = {
	nameid: 0,
};

function parseJWT(token) {
	var base64 = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
	// atob decodifica a "latin1": cada acento/ñ del payload queda partido en bytes.
	// Este map rearma esos bytes como UTF-8 para que el JSON se lea bien aunque
	// los claims traigan nombres con acentos, ñ, etc.
	var json = decodeURIComponent(
		Array.prototype.map.call(atob(base64), function (c) {
			return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
		}).join('')
	);
	return JSON.parse(json);
}

function iniciarConexion(token) {
	connection = new signalR.HubConnectionBuilder()
		.withUrl("/chatsegurohub", { accessTokenFactory: () => token })//pasar el token
		.build();

	connection.on("ReceiveMessage", function (mje) {
		console.log("Mensaje recibido:", mje);
		var msg = mje.cuerpo.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
		var encodedMsg;
		var li = document.createElement("li");
		// mje.destinatario es el UserIdentifier (el IdPropietario): para mostrar se
		// busca el nombre en la opción del <select>, que se cargó al conectarse.
		var destino = mje.destinatario
			? (document.getElementById("u_" + mje.destinatario)?.textContent || mje.destinatario)
			: 'todos';
		// Si el emisor soy yo, muestro "Vos". Se compara por id (no por nombre) y
		// no depende del ruteo, así que anda igual en públicos y privados.
		var emisor = String(mje.emisorId) === String(usuario.nameid) ? "Vos" : mje.emisor;
		encodedMsg = `${emisor} a ${destino}: ${msg}`;
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
		localStorage.token = e;	
		usuario = parseJWT(e);
		iniciarConexion(e);
	});
	event.preventDefault();
});

document.getElementById("logoutButton").addEventListener("click", function (event) {
	localStorage.token = '';
	usuario = { nameid: 0 };
	if (connection) {
		connection.stop();//cierra la conexión con el hub
	}
	// vuelve a mostrar el login
	document.getElementById("divChat").style.display = "none";
	document.getElementById("divLogin").style.display = "block";
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

// Al cargar la página: si quedó un token guardado y no venció, reconecta solo.
(function reconectarSiHayToken() {
	var token = localStorage.token;
	if (!token) return;
	try {
		var datos = parseJWT(token);
		// exp viene en segundos; si ya venció se descarta el token y se queda en el login.
		if (datos.exp && datos.exp * 1000 < Date.now()) {
			localStorage.token = '';
			return;
		}
		usuario = datos;
		iniciarConexion(token);//si falla la auth, iniciarConexion deja visible el login
	} catch (e) {
		console.error("Token inválido:", e);
		localStorage.token = '';
	}
})();