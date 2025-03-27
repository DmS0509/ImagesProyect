document.addEventListener('DOMContentLoaded', () => {
    console.log("Script ejecutándose...");

    const fileInput = document.getElementById('fileInput');
    const uploadBtn = document.getElementById('uploadBtn');
    const saveBtn = document.getElementById('saveBtn');
    const preview = document.getElementById('preview');
    const statusMessage = document.getElementById('statusMessage');
    let selectedFile = null;

    if (!fileInput || !uploadBtn || !saveBtn) {
        console.error("Uno o más elementos no fueron encontrados en el DOM.");
        return;
    }

    uploadBtn.addEventListener('click', () => {
        fileInput.click();
    });

    fileInput.addEventListener('change', () => {
        const file = fileInput.files[0];
        if (file) {
            selectedFile = file;
            const reader = new FileReader();
            reader.onload = (e) => {
                preview.innerHTML = `
                    <p>Vista previa:</p>
                    <img src="${e.target.result}" alt="Vista previa" style="max-width: 100%; margin-top: 10px; border-radius: 10px;">
                `;
            };
            reader.readAsDataURL(file);

            console.log("Mostrando botón de guardar...");
            saveBtn.style.display = "block"; // Mostrar el botón
        }
    });

    saveBtn.addEventListener('click', () => {
        if (selectedFile) {
            console.log("Subiendo imagen...");
            uploadImage(selectedFile);
        } else {
            alert("Por favor, selecciona una imagen antes de guardar.");
        }
    });

    function uploadImage(file) {
        const formData = new FormData();
        formData.append("file", file);

        fetch("/api/images/upload", {
            method: "POST",
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                console.log("Imagen subida correctamente:", data);
                statusMessage.innerText = data.message;
                saveBtn.style.display = "none"; // Ocultar el botón
                preview.innerHTML = "";
                fileInput.value = "";
            })
            .catch(error => {
                console.error("Error al subir la imagen:", error);
            });
    }
});
