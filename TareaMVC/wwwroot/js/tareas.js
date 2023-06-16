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
    const respusta = await fetch(urlTareas, {
        method: "Post",
        body: data,
        headers: {
            'Content-Type':'application/json'
        }
    });

    if (respusta.ok) {
        const json = await respusta.json();
        tarea.id(json.id);
    } else {
        //Mostrar mensaje de error
    }
}