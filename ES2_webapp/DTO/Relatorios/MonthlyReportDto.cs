using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ES2_webapp.DTO.Relatorios
{
    public class MonthlyReportDto
    {
        [JsonInclude]
        public int Mes { get; set; }

        [JsonInclude]
        public int Ano { get; set; }

        [JsonInclude]
        public decimal HorasDiariasPermitidas { get; set; }

        [JsonInclude]
        public List<DayReportDto> Dias { get; set; }

        [JsonInclude]
        public decimal HorasMesTotal { get; set; }

        [JsonInclude]
        public decimal CustoMesTotal { get; set; }
    }
}