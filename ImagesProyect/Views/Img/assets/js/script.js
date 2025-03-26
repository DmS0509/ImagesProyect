document.addEventListener('DOMContentLoaded', () => {
    const fileInput = document.getElementById('fileInput');
    const uploadBtn = document.getElementById('uploadBtn');
    const preview = document.getElementById('preview');

    uploadBtn.addEventListener('click', () => {
        fileInput.click();
    });

    fileInput.addEventListener('change', () => {
        const file = fileInput.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = (e) => {
                preview.innerHTML = `
                    <p>Vista previa:</p>
                    <img src="${e.target.result}" alt="Vista previa" style="max-width: 100%; margin-top: 10px; border-radius: 10px;">
                `;
            };
            reader.readAsDataURL(file);
        }
    });

    
});
