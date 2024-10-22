export function onLoad() {
    var nicknameInput = document.getElementById("nicknameInput");
    var regenerateButton = document.getElementById("regenerateNicknameButton");

    regenerateButton.addEventListener("click", function (e) {
        fetch("/api/chess/nickname")            
            .then(response => {
                if (!response.ok) {
                    throw new Error("Network response was not ok");
                }
                return response.text();
            })
            .then(data => {
                console.log("Nickname: ", data);
                nicknameInput.value = data;
            })
            .catch(error => console.error("Error: ", error));
    });
}