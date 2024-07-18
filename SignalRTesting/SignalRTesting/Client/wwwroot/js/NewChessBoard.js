function initialiseChessBoard(dotnetObjectRef) {
    const squares = document.querySelectorAll(".chess-square");
    let draggedImage = null;
    let originID = "";
    let selectedSquare = null;

    squares.forEach(square => {
        square.addEventListener('dragstart', function (e) {
            draggedImage = this.querySelector('img'); // Assuming each square might contain an image
            if (draggedImage) {
                e.dataTransfer.setDragImage(draggedImage, 0, 0);
            }
            originId = this.id; // Assuming the id is in the format 'r-c'
            selectedSquare = this;
            selectedSquare.classList.add('selected');
            // Show dots on empty squares (You'll need to define `showDotsOnEmptySquares`)
            showDotsOnEmptySquares();
        });

        square.addEventListener('dragover', function (e) {
            e.preventDefault(); // Necessary to allow dropping
            this.classList.add('highlight'); // Add highlighting class
        });

        square.addEventListener('dragleave', function (e) {
            this.classList.remove('highlight'); // Remove highlighting class
        });

        square.addEventListener('drop', function (e) {
            e.preventDefault();
            const targetId = this.id;
            if (draggedImage && originId !== targetId) {
                // Move the piece (You'll need to implement `movePieceInBoardArray`)
                dotnetObjectRef.invokeMethodAsync("MovePieceAsync", originId, targetId)
                    .then(data => {
                        console.log("piece moved");
                    }).catch(error => {
                        console.log(error);
                    })
                // Assuming you have a method to update the UI based on the board array
                updateUI();
            }
        });

        square.addEventListener('click', function (e) {
            if (selectedSquare) {
                // If a square is already selected, attempt to move the piece
                const targetId = this.id;
                if (selectedSquare !== this && selectedSquare.querySelector('img')) {
                    // Move the piece
                    dotnetObjectRef.invokeMethodAsync("MovePieceAsync", selectedSquare.id, targetId)
                        .then(data => {
                            console.log("piece moved via click");
                            updateUI(); // Update the UI to reflect the new board state
                        }).catch(error => {
                            console.error(error);
                        });
                }
                // Deselect the square after attempting to move
                selectedSquare.classList.remove('selected');
                selectedSquare = null;
            } else {
                dotnetObjectRef.invokeMethodAsync("IsSquareEmpty", this.id)
                    .then(empty => {
                        if (!empty) {
                            selectedSquare = this;
                            // Optionally, add some visual indication that the square is selected
                            this.classList.add('selected');
                        }
                    });
            }
        });

        square.addEventListener('dragend', function (e) {
            // Clean up: remove all highlights and dots
            squares.forEach(sq => sq.classList.remove('highlight'));
            removeDotsFromEmptySquares(); // You'll need to define this
            if (selectedSquare) {
                selectedSquare.classList.remove('selected'); // Remove visual indication of selection
                selectedSquare = null;
            }
        });
    });
};

function showDotsOnEmptySquares() {
    // Implementation depends on how you're marking empty squares and how you want to show dots
}

function movePieceInBoardArray(originId, targetId) {
    // Parse the origin and target IDs to get row and column
    // Update your Board array accordingly
    // Trigger state change if necessary
}

function updateUI() {
    // Update the UI based on the new state of the Board array
}

function removeDotsFromEmptySquares() {
    // Remove dots from empty squares
}