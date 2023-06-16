using System.Security.Claims;

namespace TareaMVC.Services
{
    public interface IServicioUsuarios
    {
        string ObtenerUsuarioId();
    }

    public class ServicioUsuarios : IServicioUsuarios
    {
        private readonly HttpContext httpContext;


        public ServicioUsuarios(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContext = httpContextAccessor.HttpContext;
            //De esta manera podemos acceder al context, es diferente por que no estamos desde un controlador
        }


        public string ObtenerUsuarioId()
        {
            if (httpContext.User.Identity.IsAuthenticated) //Si el user esta autenitcado
            {
                var idClaim = httpContext.User.Claims.Where(user => user.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
                return idClaim.Value;
            }
            else
            {
                throw new Exception("El usuario no esta autenticado");
            }
        }
    }
}


//Se creo un Servicio "IServicioUsuarios" mismo que se implementa en ServicioUsuarios
//Con la unica funcion "ObtenerUsuario" de saber si el usuario esta autenticado,
//Esto se creo por que se crearan controladores al estilo API y requerimos una forma 
//de validar al usuario que hace peticiones desde el JS.
