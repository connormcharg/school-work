window.initializeChessBoard = () => {
    const squares = document.querySelectorAll('.square');
    let draggedElement = null;

    squares.forEach(square => {
        square.addEventListener('dragstart', (e) => {
            draggedElement = e.target;
            e.dataTransfer.effectAllowed = 'move';
            e.dataTransfer.setData('text/plain', e.target.id);
        });

        square.addEventListener('dragover', (e) => {
            e.preventDefault();
            e.dataTransfer.dropEffect = 'move';
        });

        square.addEventListener('drop', (e) => {
            e.preventDefault();
            const target = e.target;
            if (target.classList.contains('square') || target.parentElement.classList.contains('square')) {
                const targetSquare = target.classList.contains('square') ? target : target.parentElement;
                targetSquare.appendChild(draggedElement);
                DotNet.invokeMethodAsync('SignalRTesting.Client', 'OnPieceMoved', draggedElement.id, targetSquare.id);
            }
        });
    });
};