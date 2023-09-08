'use strict';

const http = require('http');
const socket = require('socket.io');
const server = http.createServer();
const port = 11100;

var players = []

class player {
  constructor(username, x, y, connection, isHost) {
    this.username = username;
    this.x = x;
    this.y = y;
    this.connection = connection; //socket
    this.isHost = isHost;
  }
}

var io = socket(server, {
  pingInterval: 10000,
  pingTimeout: 5000
});

io.use((socket, next) => {
  if (socket.handshake.query.token === "UNITY") {
    next();
  } else {
    next(new Error("Authentication error"));
  }
});

io.on('connection', socket => {
  console.log('connection');

  //send informations of current players to the new player
  for (let x in players) {
    socket.emit('newPlayerConnected', { id: x, username: players[x].username, x: players[x].x, y: players[x].y });
  }

  players[socket.id] = new player("", 100, 100, socket, players.length == 0);
  socket.broadcast.emit('newPlayerConnected', { id: socket.id, username: '', x: 100, y: 100 });
  socket.emit('yourPlayerInfo', { id: socket.id, username: '', x: 100, y: 100 });


  socket.on('move', param => {

    console.log(param.x);
    players[socket.id].x = param.x;
    players[socket.id].y = param.y;


    let playersJson = [];
    for (let x in players) {
      playersJson.push({ id: x, username: players[x].username, x: players[x].x, y: players[x].y });
    }

    socket.broadcast.emit('playerMoved', playersJson);
  });

  setTimeout(() => {
    socket.emit('connection', { date: new Date().getTime(), data: "Hello Unity" })
  }, 1000);

  socket.on('hello', (data) => {
    console.log('hello', data);
    socket.emit('hello', { date: new Date().getTime(), data: data });
  });

  socket.on('spin', (data) => {
    console.log('spin');
    socket.emit('spin', { date: new Date().getTime(), data: data });
  });

  socket.on('class', (data) => {
    console.log('class', data);
    socket.emit('class', { date: new Date().getTime(), data: data });
  });
});


server.listen(port, () => {
  console.log(`Socket.IO server running at http://localhost:${port}/`);
});