export async function handleBoard(dotnetRef) {
    let selectedPiece = null;
    let selectedPieceInitialRowCol = null;
    let offset = 0;
    let percentX = 0;
    let percentY = 0;
    let state = 1;

    const grid = document.querySelector(".chess-grid");
    const gridRect = grid.getBoundingClientRect();
    const hoverSquare = document.querySelector(".hover-square");
    const prevMoveSquareStart = document.querySelector("#prev-move-start");
    const prevMoveSquareEnd = document.querySelector("#prev-move-end");
    const currMoveSquare = document.querySelector("#curr-move");
    const clamp = (value) => Math.max(-50, Math.min(value, 800));

    async function noSelectedNoDragging() {
        if (selectedPiece) {
            selectedPiece.classList.remove("dragging");
            removeSquareClass(selectedPiece);
            selectedPiece.classList.add(`square-${selectedPieceInitialRowCol[0]}-${selectedPieceInitialRowCol[1]}`);
            selectedPiece.style = "";
        };
        selectedPiece = null;
        selectedPieceInitialRowCol = null;
        offset = 0;
        percentX = 0;
        percentY = 0;
        state = 1;
        currMoveSquare.style = "visibility: hidden;";
        hoverSquare.style = "visibility: hidden;";
        await dotnetRef.invokeMethodAsync("ReloadBoard");
    };
    function selectedDraggingFirstTime(e, p) {
        selectedDragging(e, p);
        state = 2;
    };
    function selectedNoDragging() {
        state = 3;
        selectedPiece.classList.remove("dragging");
        removeSquareClass(selectedPiece);
        selectedPiece.classList.add(`square-${selectedPieceInitialRowCol[0]}-${selectedPieceInitialRowCol[1]}`);
        selectedPiece.style = "";
        hoverSquare.style = "visibility: hidden;";
    };
    function selectedDragging(e, p) {
        selectedPiece = p;
        selectedPieceInitialRowCol = getRowColFromSquareClass(getSquareClass(selectedPiece));
        offset = selectedPiece.getBoundingClientRect().width / 2;
        state = 4;
        removeSquareClass(currMoveSquare);
        currMoveSquare.classList.add(`square-${selectedPieceInitialRowCol[0]}-${selectedPieceInitialRowCol[1]}`);
        currMoveSquare.style = "";
        update(e);
    };

    async function isMoveValid(s, e) {
        return await dotnetRef.invokeMethodAsync("IsMoveValid", s, e);
    };
    async function makeMove(s, e) {
        await dotnetRef.invokeMethodAsync("MakeMoveAsync", s, e);
    };
    function makeMoveAnimate(s, e) {

    };
    function getSquareClass(element) {
        let regex = /^square-\d-\d$/;
        let classList = Array.from(element.classList);
        return classList.find(className => regex.test(className));
    };
    function removeSquareClass(element) {
        let squareClass = getSquareClass(element);
        if (squareClass) {
            element.classList.remove(squareClass);
        };
    };
    function getRowColFromSquareClass(squareClass) {
        let parts = squareClass.split("-");
        return [Number(parts[1]), Number(parts[2])];
    };
    function getIndexFromPercent(percent) {
        return Math.trunc((percent + 50) / 100);
    };
    function getRowColFromMouseXY(x, y) {
        return [
            Math.trunc(((y - gridRect.top + window.scrollY) / gridRect.height) * 8),
            Math.trunc(((x - gridRect.left + window.scrollX) / gridRect.width) * 8),
        ];
    };
    function arraysEqual(a, b) {
        if (a === b) return true;
        if (a === null || b == null) return false;
        if (a.length !== b.length) return false;
        return a.every((val, idx) => val === b[idx]);
    };
    function update(e) {
        if (state === 2 || state === 4) {
            percentX = clamp(((e.clientX - gridRect.left - offset + window.scrollX) / gridRect.width) * 800);
            percentY = clamp(((e.clientY - gridRect.top - offset + window.scrollY) / gridRect.height) * 800);
            selectedPiece.style.transform = `translate(${percentX}%, ${percentY}%)`;
            selectedPiece.classList.add("dragging");

            removeSquareClass(hoverSquare);
            hoverSquare.classList.add(`square-${getIndexFromPercent(percentY)}-${getIndexFromPercent(percentX)}`);
            hoverSquare.style = "";
        };
    };

    document.querySelectorAll(".chess-piece").forEach(piece => {
        piece.addEventListener("mousedown", function (e) {
            if (e.button === 0) {
                if (state === 1) {
                    selectedDraggingFirstTime(e, piece);
                } else if (state === 3 && piece !== selectedPiece) {
                    selectedDraggingFirstTime(e, piece);
                } else if (state === 3 && piece === selectedPiece) {
                    selectedDragging(e, piece);
                };
            };
        });
    });
    grid.addEventListener("mousedown", async function (e) {
        if (e.button === 0) {
            if (state === 3 && await isMoveValid(
                selectedPieceInitialRowCol,
                getRowColFromMouseXY(e.clientX, e.clientY)
            )) {
                await makeMove(
                    selectedPieceInitialRowCol,
                    getRowColFromMouseXY(e.clientX, e.clientY)
                );
                await noSelectedNoDragging();
            } else if (state === 3) {
                await noSelectedNoDragging();
            };
        };
    });
    grid.addEventListener("mouseup", async function (e) {
        if (e.button === 0) {
            if (state === 2 && arraysEqual(selectedPieceInitialRowCol,
                [getIndexFromPercent(percentY), getIndexFromPercent(percentX)])) {
                selectedNoDragging();
            } else if (state === 2 && await isMoveValid(
                selectedPieceInitialRowCol,
                [getIndexFromPercent(percentY), getIndexFromPercent(percentX)]
            )) {
                await makeMove(
                    selectedPieceInitialRowCol,
                    [getIndexFromPercent(percentY), getIndexFromPercent(percentX)]);
                await noSelectedNoDragging();
            } else if (state === 2) {
                selectedNoDragging();
            } else if (state === 4 && arraysEqual(selectedPieceInitialRowCol,
                [getIndexFromPercent(percentY), getIndexFromPercent(percentX)])) {
                await noSelectedNoDragging();
            } else if (state === 4 && await isMoveValid(
                selectedPieceInitialRowCol,
                [getIndexFromPercent(percentY), getIndexFromPercent(percentX)]
            )) {
                await makeMove(
                    selectedPieceInitialRowCol,
                    [getIndexFromPercent(percentY), getIndexFromPercent(percentX)]);
                await noSelectedNoDragging();
            } else if (state === 4) {
                selectedNoDragging();
            };
        };
    });
    grid.addEventListener("mousemove", function (e) {
        update(e);
    });
};