app.get('/api/Players', function(req, res) {
    const fs = require('fs');
    const path = require('path');
    const charactersDir = 'Assets/Characters/Players';
    const characterFiles = fs.readdirSync(charactersDir);
    const characterNames = characterFiles.filter((file) => {
        return path.extname(file).toLowerCase() === '.png';
    }).map((file) => {
        return path.basename(file, path.extname(file));
    });
    res.send(JSON.stringify(characterNames));
});