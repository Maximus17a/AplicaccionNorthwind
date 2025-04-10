//// Este archivo contiene funciones auxiliares para la exportación a Excel
//// Dependencias: ExcelJS, JSZip (ya se manejan en el servidor en este caso)

//// Función para permitir la descarga de archivos Excel
//function downloadExcel(blob, filename) {
//    // Crear una URL para el blob
//    const url = window.URL.createObjectURL(blob);

//    // Crear un enlace temporal para descargar el archivo
//    const a = document.createElement('a');
//    a.href = url;
//    a.download = filename || 'report.xlsx';

//    // Agregar el enlace al documento y hacer clic en él
//    document.body.appendChild(a);
//    a.click();

//    // Limpiar
//    window.URL.revokeObjectURL(url);
//    document.body.removeChild(a);
//}

//// Función para mostrar un mensaje de éxito después de la descarga
//function showExportSuccess() {
//    // Crear un elemento div para el mensaje
//    const messageDiv = document.createElement('div');
//    messageDiv.className = 'alert alert-success alert-dismissible fade show';
//    messageDiv.role = 'alert';
//    messageDiv.innerHTML = `
//        <i class="bi bi-check-circle-fill me-2"></i>
//        El archivo se ha exportado correctamente.
//        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
//    `;

//    // Agregar el mensaje al contenedor principal
//    const container = document.querySelector('.container-fluid');
//    container.insertBefore(messageDiv, container.firstChild);

//    // Configurar un temporizador para eliminar el mensaje después de unos segundos
//    setTimeout(() => {
//        messageDiv.classList.remove('show');
//        setTimeout(() => messageDiv.remove(), 150);
//    }, 3000);
//}