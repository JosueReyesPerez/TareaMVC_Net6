/*
function mostrarError() {
    alert(mensajeDeError);
}*/

async function manejarErrorApi(respuesta) {
    let mensajeError = '';

    if (respuesta.status === 400)
        mensajeError = await respuesta.text();
    else if (respuesta.status === 404)
        mensajeError = rescursoNoEncontrador;
    else
        mensajeError = errorInesperado;

    mostrarMensajeError(mensajeError);
} 

function mostrarMensajeError(mensaje) {
    Swal.fire({
        icon: "error", 
        title: "Error...",
        text: mensaje
    });
}

function confirmarAccion({ callBackAceptar, callBackCancelar, titulo }) {
    Swal.fire({
        title: 'Confirmar acción',
        text: titulo,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Aceptar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.value == true) {
            callBackAceptar();
        } else {
            // El usuario ha cancelado la acción
            console.log("Accion cancelada");
            callBackCancelar();
        }
    });
}
