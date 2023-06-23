using AutoMapper;
using TareaMVC.Entities;
using TareaMVC.Models;

namespace TareaMVC.Services
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Tarea, TareaDTO>()
                .ForMember(dto => dto.PasosTotal, ent => ent.MapFrom(x => x.pasos.Count()))
                .ForMember(dto => dto.PasosRealizados, ent => ent.MapFrom(x => x.pasos.Where(p => p.Realizado).Count()));
            //Rellenamos dos propiedades que tienen direntes nombres.
        }
    }
}
