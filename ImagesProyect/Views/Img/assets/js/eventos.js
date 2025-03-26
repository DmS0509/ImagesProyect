document.addEventListener('DOMContentLoaded', () => {
   
    document.querySelectorAll('.download-btn').forEach(button => {
        button.addEventListener('click', (e) => {
            const imageCard = e.target.closest('.image-card');
            const img = imageCard.querySelector('img');

            
            const a = document.createElement('a');
            a.href = img.src;
            a.setAttribute('download', img.src.split('/').pop());
            a.style.display = 'none'; 
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
        });
    });

    
    document.querySelectorAll('.delete-btn').forEach(button => {
        button.addEventListener('click', (e) => {
            const imageCard = e.target.closest('.image-card');
            if (imageCard) {
                imageCard.remove();
            }
        });
    });
});
