function agregarNuevaTareaAlListado() {
    tareasListadoViewModel.tareas.push(new tareaElementoListadoViewModel({ id: 0, titulo: "" }));
}

async function manejarFocusoutTituloTarea(tarea) {
    const titulo = tarea.titulo();
    if (!titulo) {
        tareasListadoViewModel.tareas.pop();
        return;
    }

    const data = JSON.stringify(titulo);
    const respuesta = await fetch(urlTareas, {
        method: "POST",
        body: data,
        headers: {
            'Content-Type':'application/json'
        }
    });

    if (respuesta.ok) {
        const json = await respuesta.json();
        tarea.id(json.id);
        console.log(json.titulo);
    } else {
        //Mostrar mensaje de error
        manejarErrorApi(respuesta);
    }
}

async function obtenerTareas() {
    tareasListadoViewModel.cargando(true);

    const respuesta = await fetch(urlTareas, {
        method: "GET",
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!respuesta.ok)
        manejarErrorApi(respuesta);

    const json = await respuesta.json();
    tareasListadoViewModel.tareas([]);

    json.forEach(valor => { tareasListadoViewModel.tareas.push(new tareaElementoListadoViewModel(valor)); });
    tareasListadoViewModel.cargando(false);
}

async function actualizarOrdenTareas() {
    const ids = obtenerIdsTareas();
    await enviarIdsTareasAlBackend(ids);


    const arregloOrdenado = tareasListadoViewModel.tareas.sorted(function (a, b) {
        return ids.indexOf(a.id().toString()) - ids.indexOf(b.id().toString());
    });

    tareasListadoViewModel.tareas([]);
    tareasListadoViewModel.tareas(arregloOrdenado);
}

function obtenerIdsTareas() {
    const ids = $("[name=titulo-tarea]").map(function () {
        return $(this).attr("data-id");
    }).get();
    return ids;
}

async function enviarIdsTareasAlBackend(ids) {
    var data = JSON.stringify(ids);
    await fetch(`${urlTareas}/ordenar`, {
        method: "POST",
        body: data, 
        headers: {
            'Content-Type':'application/json'
        }
    });
}

async function manejarClickTarea(tarea) {
    if (tarea.esNuevo())
        return;

    const respuesta = await fetch(`${urlTareas}/${tarea.id()}`, {
        method: "GET",
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!respuesta.ok) {
        manejarErrorApi(respuesta);
        return
    }

    const json = await respuesta.json();

    tareaEditarViewModel.id = json.id;
    tareaEditarViewModel.titulo(json.titulo);
    tareaEditarViewModel.descripcion(json.descripcion);

    tareaEditarViewModel.pasos([]);
    json.pasos.forEach(paso => {
        tareaEditarViewModel.pasos.push(
            new pasoViewModel({ ...paso, modoEdicion: false })
            );
    });

    modalEditarTareaBoostrap.show();
}

async function manejarCambioEditarTarea() {
    const obj = {
        id: tareaEditarViewModel.id,
        titulo: tareaEditarViewModel.titulo(),
        descripcion: tareaEditarViewModel.descripcion()
    };

    if (!obj.titulo)
        return;

    await editarTareaCompleta(obj);

    const indice = tareasListadoViewModel.tareas().findIndex(t => t.id() === obj.id);
    const tarea = tareasListadoViewModel.tareas()[indice];
    tarea.titulo(obj.titulo);

}

async function editarTareaCompleta(tarea) {
    const data = JSON.stringify(tarea);

    const respuesta = await fetch(`${urlTareas}/${tarea.id}`, {
        method: "PUT",
        body: data,
        headers: {
            'Content-Type':'application/json'
        }
    });

    if (!respuesta.ok) {
        manejarErrorApi(respuesta);
        throw "Error...";
    }
}

function intentarBorraTarea(tarea) {
    modalEditarTareaBoostrap.hide();

    confirmarAccion({
        callBackAceptar: () => {
            console.log("Intentando borrar tarea");
            borrarTarea(tarea);
        },
        callBackCancelar: () => {
            modalEditarTareaBoostrap.show();
        },
        titulo: `¿Desea eleminar ${tarea.titulo()}?`
    });
}


async function borrarTarea(tarea) {
    
    const idTarea = tarea.id;

    const respuesta = await fetch(`${urlTareas}/${idTarea}`, {
        method: "DELETE",
        headers: {
            'Content-Type':'application/json'
        }
    });

    if (respuesta.ok) {
        const indice = obtenerIndiceTareaEnEdicion();
        tareasListadoViewModel.tareas.splice(indice, 1);
    }
}

function obtenerIndiceTareaEnEdicion() {
    return tareasListadoViewModel.tareas().findIndex(t => t.id() == tareaEditarViewModel.id)
}

function obtenerTareaEnEdicion() {
    const indice = obtenerIndiceTareaEnEdicion();
    return tareasListadoViewModel.tareas()[indice];
}

$(()=>{

    $("#reordenable").sortable({
        axis: 'y',
        stop: async function () {
            await actualizarOrdenTareas();
        }
    });
});