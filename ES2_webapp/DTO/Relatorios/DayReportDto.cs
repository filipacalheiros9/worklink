using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ES2_webapp.DTO.Relatorios
{
    public class DayReportDto
    {
        [JsonInclude]
        public DateTime Dia { get; set; }

        [JsonInclude]
        public decimal TotalHoras { get; set; }

        [JsonInclude]
        public decimal TotalCusto { get; set; }

        [JsonInclude]
        public bool ExcedeuHoras { get; set; }

        [JsonInclude]
        public List<TaskReportItemDto> TarefasDoDia { get; set; }
    }
}