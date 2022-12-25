using ISL.Modelos;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISL.Servicios;

public class LaborServicio
{
    public LaborView ToLaborView(Labor entity, DateTime fecha)
    {
        return new LaborView()
        {
            Dia = DateOnly.FromDateTime(fecha),
            HorarioEntrada = entity.HorarioEntrada is null ? null : TimeOnly.FromDateTime((DateTime)entity.HorarioEntrada),
            HorarioSalida = TimeOnly.FromDateTime((DateTime)entity.HorarioSalida),
            NoActividades = entity.Actividades?.Any() ?? false
            ? entity.Actividades.Count > 1 ? $"{entity.Actividades.Count} Actividades" : $"Una actividad"
            : "No hay actividades"
        };
    }
}
