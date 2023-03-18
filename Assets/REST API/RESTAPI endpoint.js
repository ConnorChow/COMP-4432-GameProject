const express = require('express');
const app = express();

app.get('Asset/Characters/Player', (req, res) => {
    const Players = ['Hero', 'Hero Player 2'];
    const randomIndex = Math.floor(Math.random() * players.length);
    const randomPlayers = Players[randomIndex];
    res.send(randomPlayers);
});

app.listen(3000, () => console.log('Server listening on port 3000'));