
document.getElementById('image-File').addEventListener('change', function (e)
{
    const file = e.target.files[0];
    const img = document.getElementById('image-Preview');
    img.hidden = false;
    const allowedExtensions = ['.jpg', '.jpeg', '.png'];
    const errorElement = document.getElementById('image-Error');
    if (!file) {
        imag.src = '';
        errorElement.innerHTML = "plese select a valid file";
        return;
    }

    if (!file.type.startsWith('image/'))
    {
        img.src = '';
        errorElement.innerHTML = "Just allow .jpg, .jpeg or .png files";
        return;
    }
    if (!allowedExtensions.some(ext => file.name.toLowerCase().endsWith(ext))) {
        imag.src = '';
        errorElement.innerHTML = "Just allow .jpg, .jpeg or .png files";
        return;
    }
 
    const reader = new FileReader();
    reader.onload = function (e) {
        img.src = e.target.result;
    };
    reader.readAsDataURL(file);
    errorElement.innerHTML = '';
});